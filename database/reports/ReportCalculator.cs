using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Castor.database.reports
{
    public class ReportCalculator
    {
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public ReportCalculator()
        {
        }

        public void SetParameter(string name, object value)
        {
            _parameters[name] = value;
        }

        public async Task<(string[] data, string period, string department)> CalculateAsync(string htmlPath)
        {
            var html = File.ReadAllText(GetFullPath(htmlPath));
            var queries = ExtractQueries(html);
            var results = new List<string>();

            using CastorContext context = new CastorContext();
            using (var connection = new SqliteConnection(context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                foreach (var query in queries)
                {
                    try
                    {
                        var sql = ReplaceParameters(query);
                        var result = await ExecuteScalarAsync(connection, sql);
                        results.Add(result?.ToString() ?? "0");
                    }
                    catch (Exception ex)
                    {
                        results.Add($"Ошибка: {ex.Message}");
                    }
                }
            }

            return (results.ToArray(), DateTime.Now.ToString("MMMM yyyy"), "Все отделения");
        }

        private string GetFullPath(string relativePath)
        {
            if (Path.IsPathRooted(relativePath))
                return relativePath;

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDirectory, relativePath);
        }

        private List<string> ExtractQueries(string html)
        {
            var queries = new List<string>();
            var pattern = @"<pre>(.*?)</pre>";
            var matches = Regex.Matches(html, pattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var query = CleanQuery(match.Groups[1].Value);
                if (!string.IsNullOrEmpty(query) && query.Contains("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    queries.Add(query);
                }
            }

            return queries;
        }

        private string CleanQuery(string query)
        {
            query = Regex.Replace(query, @"<!\[CDATA\[|\]\]>", "");
            query = Regex.Replace(query, @"--.*?$", "", RegexOptions.Multiline);
            query = Regex.Replace(query, @"/\*.*?\*/", "", RegexOptions.Singleline);
            query = Regex.Replace(query, @"\s+", " ");
            return query.Trim();
        }

        private string ReplaceParameters(string query)
        {
            foreach (var param in _parameters)
            {
                var placeholder = $"@{{{param.Key}}}";
                if (query.Contains(placeholder))
                {
                    var value = FormatValue(param.Value);
                    query = query.Replace(placeholder, value);
                }
            }
            return query;
        }

        private string FormatValue(object value)
        {
            if (value == null) return "NULL";

            if (value is string str)
                return $"'{str.Replace("'", "''")}'";

            if (value is DateTime dt)
                return $"'{dt:yyyy-MM-dd HH:mm:ss}'";

            if (value is DateOnly date)
                return $"'{date:yyyy-MM-dd}'";

            if (value is bool b)
                return b ? "1" : "0";

            if (value is Enum)
                return $"{(int)value}";

            return $"'{value.ToString()?.Replace("'", "''")}'";
        }

        private async Task<object> ExecuteScalarAsync(SqliteConnection connection, string query)
        {
            using (var command = new SqliteCommand(query, connection))
            {
                command.CommandTimeout = 60;
                return await command.ExecuteScalarAsync();
            }
        }
    }
}