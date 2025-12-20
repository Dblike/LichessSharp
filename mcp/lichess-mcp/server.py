import json
import os
import subprocess
from pathlib import Path
from typing import Any, Dict, Optional, List

from mcp.server.fastmcp import FastMCP

ROOT = Path(__file__).resolve().parents[2]  # repo root (adjust if needed)

# Load project MCP config (the anchor file)
MCP_CONFIG_PATH = ROOT / "mcp" / "mcp.json"
CONFIG = json.loads(MCP_CONFIG_PATH.read_text(encoding="utf-8"))

OPENAPI_PATH = ROOT / CONFIG["openapi"]
SNAPSHOTS_DIR = ROOT / CONFIG["openapiSnapshotsDir"]
IMPLEMENTED_ENDPOINTS_PATH = ROOT / CONFIG["implementedEndpoints"]
COVERAGE_REPORT_PATH = ROOT / CONFIG["coverageReport"]
COVERAGE_SCRIPT_PATH = ROOT / CONFIG["coverageScript"]

mcp = FastMCP("lichess-mcp")


def _load_openapi(path: Path) -> Dict[str, Any]:
    return json.loads(path.read_text(encoding="utf-8"))


def _op_key(method: str, path: str) -> str:
    return f"{method.upper()} {path}"


@mcp.resource("lichess://openapi")
def openapi_resource() -> str:
    """Return the current Lichess OpenAPI spec JSON as a string."""
    return OPENAPI_PATH.read_text(encoding="utf-8")


@mcp.resource("lichess://coverage")
def coverage_resource() -> str:
    """Return the current endpoint coverage report markdown (if it exists)."""
    if not COVERAGE_REPORT_PATH.exists():
        return "# Endpoint coverage report not generated yet.\n"
    return COVERAGE_REPORT_PATH.read_text(encoding="utf-8")


@mcp.tool()
def get_openapi_version() -> Dict[str, Any]:
    """Return the OpenAPI spec info.version and title."""
    spec = _load_openapi(OPENAPI_PATH)
    info = spec.get("info", {})
    return {
        "title": info.get("title"),
        "version": info.get("version"),
    }


@mcp.tool()
def get_operation(method: str, path: str) -> Dict[str, Any]:
    """
    Lookup a single operation from the OpenAPI spec by HTTP method and path.
    Returns summary, tags, deprecated, parameters, requestBody schema (if any), and response codes.
    """
    spec = _load_openapi(OPENAPI_PATH)
    paths = spec.get("paths", {})
    path_item = paths.get(path)
    if not path_item:
        return {"found": False, "error": f"Path not found: {path}"}

    op = path_item.get(method.lower())
    if not op:
        return {"found": False, "error": f"Method not found: {method.upper()} {path}"}

    # Lightly normalize the payload to keep it model-friendly.
    responses = op.get("responses", {})
    response_codes = sorted(list(responses.keys()))

    return {
        "found": True,
        "key": _op_key(method, path),
        "summary": op.get("summary"),
        "description": op.get("description"),
        "tags": op.get("tags", []),
        "deprecated": bool(op.get("deprecated", False)),
        "parameters": op.get("parameters", []),
        "requestBody": op.get("requestBody"),
        "responseCodes": response_codes,
    }


@mcp.tool()
def list_operations(tag: Optional[str] = None, include_deprecated: bool = False) -> Dict[str, Any]:
    """
    List all operations in the OpenAPI spec, optionally filtered by tag.
    """
    spec = _load_openapi(OPENAPI_PATH)
    paths = spec.get("paths", {})

    ops: List[Dict[str, Any]] = []
    for p, item in paths.items():
        for method in ["get", "post", "put", "delete", "patch", "head", "options"]:
            if method in item:
                op = item[method]
                if (not include_deprecated) and op.get("deprecated", False):
                    continue
                tags = op.get("tags", [])
                if tag and tag not in tags:
                    continue
                ops.append({
                    "key": _op_key(method, p),
                    "summary": op.get("summary"),
                    "tags": tags,
                    "deprecated": bool(op.get("deprecated", False)),
                })

    ops.sort(key=lambda x: x["key"])
    return {"count": len(ops), "operations": ops}


@mcp.tool()
def diff_openapi_versions(from_version: str, to_version: str) -> Dict[str, Any]:
    """
    Compare two snapshot specs (e.g., 2.0.106 -> 2.0.107) and return added/removed operation keys.
    Snapshots must be saved as: openapi/snapshots/lichess.openapi.<version>.json
    """
    a = SNAPSHOTS_DIR / f"lichess.openapi.{from_version}.json"
    b = SNAPSHOTS_DIR / f"lichess.openapi.{to_version}.json"

    if not a.exists():
        return {"ok": False, "error": f"Snapshot not found: {a}"}
    if not b.exists():
        return {"ok": False, "error": f"Snapshot not found: {b}"}

    spec_a = _load_openapi(a)
    spec_b = _load_openapi(b)

    def collect(spec: Dict[str, Any]) -> set[str]:
        out = set()
        for p, item in spec.get("paths", {}).items():
            for method in item.keys():
                out.add(_op_key(method, p))
        return out

    ops_a = collect(spec_a)
    ops_b = collect(spec_b)

    added = sorted(list(ops_b - ops_a))
    removed = sorted(list(ops_a - ops_b))

    return {"ok": True, "from": from_version, "to": to_version, "added": added, "removed": removed}


def _normalize_path(path: str) -> str:
    """Normalize path parameters to a canonical form for comparison.

    Converts {paramName} to {param} to handle naming differences between
    OpenAPI spec and our implementation (e.g., {broadcastTournamentId} vs {tournamentId}).
    """
    import re
    # Replace any {word} with just {param} for comparison
    return re.sub(r'\{[^}]+\}', '{param}', path)


def _parse_implemented_endpoints() -> Dict[str, Dict[str, str]]:
    """Parse ImplementedEndpoints.cs and return a dict of key -> {api, method}."""
    import re
    if not IMPLEMENTED_ENDPOINTS_PATH.exists():
        return {}

    content = IMPLEMENTED_ENDPOINTS_PATH.read_text(encoding="utf-8")
    pattern = r'new\s*\(\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"\s*\)'
    matches = re.findall(pattern, content)

    return {
        f"{m[0]} {m[1]}": {"api": m[2], "method": m[3], "path": m[1]}
        for m in matches
    }


@mcp.tool()
def get_coverage_gaps(include_deprecated: bool = False) -> Dict[str, Any]:
    """
    Compare OpenAPI spec with ImplementedEndpoints.cs to find coverage gaps.

    Returns:
    - missing: Endpoints in OpenAPI but not implemented (true gaps)
    - pathVariations: Endpoints that appear missing due to path parameter naming differences
    - extra: Endpoints implemented but not in OpenAPI (external APIs like explorer/tablebase)
    - stats: Summary statistics
    """
    spec = _load_openapi(OPENAPI_PATH)
    paths = spec.get("paths", {})

    # Collect OpenAPI operations
    openapi_ops: Dict[str, Dict[str, Any]] = {}
    for p, item in paths.items():
        for method in ["get", "post", "put", "delete", "patch", "head", "options"]:
            if method in item:
                op = item[method]
                if (not include_deprecated) and op.get("deprecated", False):
                    continue
                key = _op_key(method, p)
                openapi_ops[key] = {
                    "key": key,
                    "path": p,
                    "method": method.upper(),
                    "summary": op.get("summary"),
                    "tags": op.get("tags", []),
                    "deprecated": bool(op.get("deprecated", False)),
                    "normalizedPath": _normalize_path(p),
                }

    # Get implemented endpoints
    implemented = _parse_implemented_endpoints()

    # Build normalized lookup for implemented endpoints
    impl_normalized: Dict[str, str] = {}
    for key, info in implemented.items():
        method, path = key.split(" ", 1)
        normalized = f"{method} {_normalize_path(path)}"
        impl_normalized[normalized] = key

    # Categorize gaps
    missing: List[Dict[str, Any]] = []
    path_variations: List[Dict[str, Any]] = []

    for key, op in openapi_ops.items():
        if key in implemented:
            continue  # Exact match - implemented

        # Check for path variation match
        method = op["method"]
        normalized_key = f"{method} {op['normalizedPath']}"

        if normalized_key in impl_normalized:
            impl_key = impl_normalized[normalized_key]
            path_variations.append({
                "openapi": key,
                "implemented": impl_key,
                "summary": op["summary"],
                "tags": op["tags"],
            })
        else:
            missing.append({
                "key": key,
                "summary": op["summary"],
                "tags": op["tags"],
                "deprecated": op["deprecated"],
            })

    # Find extra (implemented but not in OpenAPI - external APIs)
    extra: List[Dict[str, Any]] = []
    openapi_normalized = {f"{op['method']} {op['normalizedPath']}" for op in openapi_ops.values()}

    for key, info in implemented.items():
        if key not in openapi_ops:
            method, path = key.split(" ", 1)
            normalized = f"{method} {_normalize_path(path)}"
            if normalized not in openapi_normalized:
                extra.append({
                    "key": key,
                    "api": info["api"],
                    "method": info["method"],
                })

    return {
        "stats": {
            "openApiTotal": len(openapi_ops),
            "implementedTotal": len(implemented),
            "missingCount": len(missing),
            "pathVariationsCount": len(path_variations),
            "extraCount": len(extra),
        },
        "missing": sorted(missing, key=lambda x: x["key"]),
        "pathVariations": sorted(path_variations, key=lambda x: x["openapi"]),
        "extra": sorted(extra, key=lambda x: x["key"]),
    }


@mcp.tool()
def generate_coverage_report() -> Dict[str, Any]:
    """
    Run the repo's coverage generator script to update docs/api-coverage.md.
    Tries pwsh (PowerShell 7+) first, then falls back to powershell (Windows PowerShell 5).
    """
    if not COVERAGE_SCRIPT_PATH.exists():
        return {"ok": False, "error": f"Coverage script not found: {COVERAGE_SCRIPT_PATH}"}

    # Try pwsh first (PowerShell 7+), then fall back to powershell (Windows PowerShell 5)
    ps_commands = [
        ["pwsh", "-ExecutionPolicy", "Bypass", "-File", str(COVERAGE_SCRIPT_PATH)],
        ["powershell", "-ExecutionPolicy", "Bypass", "-File", str(COVERAGE_SCRIPT_PATH)],
    ]

    last_error = None
    for cmd in ps_commands:
        try:
            result = subprocess.run(
                cmd,
                cwd=str(ROOT),
                capture_output=True,
                text=True,
                check=False,
            )
            return {
                "ok": result.returncode == 0,
                "exitCode": result.returncode,
                "stdout": result.stdout[-4000:],  # keep it model-friendly
                "stderr": result.stderr[-4000:],
                "coverageReportPath": str(COVERAGE_REPORT_PATH),
            }
        except FileNotFoundError:
            last_error = f"{cmd[0]} not found"
            continue

    return {"ok": False, "error": f"PowerShell not found. Tried pwsh and powershell. Last error: {last_error}"}


if __name__ == "__main__":
    # stdio transport is the common local integration path
    mcp.run()
