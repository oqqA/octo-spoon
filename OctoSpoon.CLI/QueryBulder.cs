using System.Diagnostics.Contracts;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OctoSpoon.CLI
{
    public class QueryBuilder
    {
        public string? Query { get; set; }
        public object? Variables { get; set; }

        public StringContent GetJson()
        {
            var clearQuery = Query?.Substring(Query.IndexOf("query") + 5);
            clearQuery = clearQuery?.Replace(System.Environment.NewLine, string.Empty);
            var clearQueryArray = clearQuery?.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
            clearQuery = string.Join(" ", string.Join(" ", clearQueryArray ?? Array.Empty<string>()));

            var properties = Variables?.GetType().GetProperties();

            if (properties == null)
                throw new Exception("Properties is null");

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(Variables);
                if (propertyValue == null)
                    throw new Exception("Property is null");

                string replaceValue = propertyValue.GetType().Equals(typeof(string)) ? $"\\\"{propertyValue}\\\"" : propertyValue?.ToString() ?? "";
                clearQuery = clearQuery?.Replace("$" + property.Name, replaceValue);
            }

            var rawQuery = "{\"query\": \"" + clearQuery + "\" }";

            return new StringContent(rawQuery, Encoding.UTF8, "application/json");
        }
    }
}