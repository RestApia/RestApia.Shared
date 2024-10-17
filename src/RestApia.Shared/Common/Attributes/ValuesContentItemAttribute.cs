using System.Diagnostics.CodeAnalysis;
namespace RestApia.Shared.Common.Attributes;

[AttributeUsage(AttributeTargets.Property)]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class ValuesContentItemAttribute(string name, string description) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public bool IsRequired { get; init; }
}
