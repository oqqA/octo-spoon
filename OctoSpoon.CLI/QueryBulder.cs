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
            var qquery = @"{ repository(owner: \""" + "author" + @"\"", name: \""" + "repoName" + @"\"") { discussions(first: 100) { totalCount nodes { number title } } } }";
            var rrawQuery = "{\"query\": \"" + qquery + "\" }";

            var clearQuery = Query?.Substring(Query.IndexOf("query") + 5);
            clearQuery = clearQuery.Replace(System.Environment.NewLine, string.Empty);
            clearQuery = string.Join(" ", string.Join(" ", clearQuery.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries)));

            var properties = Variables?.GetType().GetProperties();

            foreach (var p in properties)
            {
                var propertyValue = p.GetValue(Variables);
                string replaceValue = propertyValue.GetType().Equals(typeof(string)) ? $"\\\"{propertyValue}\\\"" : propertyValue.ToString();
                clearQuery = clearQuery?.Replace("$" + p.Name, replaceValue);
            }


            var rawQuery = "{\"query\": \"" + clearQuery + "\" }";

            return new StringContent(rawQuery, Encoding.UTF8, "application/json");
        }
    }
}