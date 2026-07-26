namespace Privat2Ynab.Application.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class DisplayNameAttribute(string displayName) : Attribute
{
    public string DisplayName { get; } = displayName;
}