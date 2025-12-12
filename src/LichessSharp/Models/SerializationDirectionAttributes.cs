namespace LichessSharp.Models;

/// <summary>
///     Indicates that this model is used only for deserializing API responses.
///     These models can be freely modified with custom converters, computed properties,
///     or <see cref="System.Text.Json.Serialization.JsonIgnoreAttribute" /> without affecting API requests.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class ResponseOnlyAttribute : Attribute;

/// <summary>
///     Indicates that this model is used only for serializing API requests.
///     These models should match the expected API request format exactly.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class RequestOnlyAttribute : Attribute;

/// <summary>
///     Indicates that this model is used for both API requests and responses.
///     Changes to these models must ensure both serialization and deserialization work correctly.
///     Avoid adding <see cref="System.Text.Json.Serialization.JsonIgnoreAttribute" /> or custom converters
///     that only handle one direction.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class BidirectionalAttribute : Attribute;
