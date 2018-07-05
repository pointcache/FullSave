using System;

/// <summary>
/// The given property or field annotated with [fsRuntimeIgnore] will not be
/// serialized if the serialization mode is set to Runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class RuntimeSaveAttribute : Attribute {
}
