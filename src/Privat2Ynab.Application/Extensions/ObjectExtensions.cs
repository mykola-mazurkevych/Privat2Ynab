using System.Reflection;

using Privat2Ynab.Application.Attributes;

namespace Privat2Ynab.Application.Extensions;

internal static class ObjectExtensions
{
    extension<TModel>(TModel model)
    {
        public string ToTable() =>
            typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(propertyInfo =>
                    new KeyValuePair<string, string>(
                        propertyInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? propertyInfo.Name,
                        propertyInfo.GetValue(model)?.ToString() ?? string.Empty))
                .ToTable(headless: true);
    }
}