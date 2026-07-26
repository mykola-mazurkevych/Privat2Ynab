using System.Reflection;
using System.Text;

using Privat2Ynab.Application.Attributes;

namespace Privat2Ynab.Application.Extensions;

internal static class EnumerableExtensions
{
    extension<TModel>(IEnumerable<TModel> models)
    {
        public string ToTable(bool headless)
        {
            List<PropertyInfo> propertyInfos = [.. typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)];

            List<List<string>> rows = [];

            if (!headless)
            {
                rows.Add([.. propertyInfos.Select(propertyInfo => propertyInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? propertyInfo.Name)]);
            }
            rows.AddRange([.. models.Select(model => propertyInfos.Select(propertyInfo => propertyInfo.GetValue(model)?.ToString() ?? string.Empty).ToList())]);

            List<int> widths = [.. propertyInfos.Select((_, index) => rows.Max(tableCell => tableCell[index].Length))];

            var tableStringBuilder = new StringBuilder();

            tableStringBuilder.AppendLine(BuildLine(LinePosition.Top, widths));
            if (!headless)
            {
                tableStringBuilder.AppendLine(BuildLine(rows[0], widths));
                tableStringBuilder.AppendLine(BuildLine(LinePosition.Middle, widths));
                rows.RemoveAt(0);
            }

            foreach (var row in rows)
            {
                tableStringBuilder.AppendLine(BuildLine(row, widths));
            }

            tableStringBuilder.AppendLine(BuildLine(LinePosition.Bottom, widths));

            return tableStringBuilder.ToString().Trim();
        }
    }

    private static string BuildLine(LinePosition position, IReadOnlyList<int> widths)
    {
        (char left, char middle, char right) = position switch
        {
            LinePosition.Top => ('┌', '┬', '┐'),
            LinePosition.Middle => ('├', '┼', '┤'),
            LinePosition.Bottom => ('└', '┴', '┘'),
            _ => throw new InvalidOperationException(),
        };

        var builder = new StringBuilder();
        builder.Append(left);

        for (var i = 0; i < widths.Count; i++)
        {
            builder.Append(new string('─', widths[i] + 2));
            builder.Append(i == widths.Count - 1 ? right : middle);
        }

        return builder.ToString();
    }

    private static string BuildLine(IReadOnlyList<string> row, IReadOnlyList<int> widths)
    {
        var builder = new StringBuilder();
        builder.Append('│');

        for (var i = 0; i < row.Count; i++)
        {
            builder.Append(' ');
            builder.Append(row[i].PadRight(widths[i], ' '));
            builder.Append(' ');
            builder.Append('│');
        }

        return builder.ToString();
    }

    private enum LinePosition
    {
        Top,
        Middle,
        Bottom,
    }
}