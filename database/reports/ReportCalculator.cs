using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Castor.database.reports
{
    public class ReportParameter
    {
        public ReportParameter() { }
        public string Name { get; set; } = string.Empty;
        public Type Type { get; set; } = typeof(string);
        public object? Value { get; set; } = null;
    }
    public class ReportCalculator
    {
        private readonly Dictionary<string, ReportParameter> _parameters = new Dictionary<string, ReportParameter>();

        public ReportCalculator()
        {
        }

        public async Task<(string[] data, string period, string department)> CalculateAsync(string htmlPath)
        {
            var html = File.ReadAllText(GetFullPath(htmlPath));
            ParseParameters(html);
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

        

        private void ParseParameters(string html)
        {
            // Clean First!
            _parameters.Clear();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Ищем все блоки параметров
            var parameterBlocks = doc.DocumentNode.SelectNodes("//div[contains(@class, 'parameter-block')]");

            if (parameterBlocks == null || parameterBlocks.Count == 0)
                return;

            foreach (var block in parameterBlocks)
            {
                var param = new ReportParameter();

                // Извлекаем имя параметра
                var nameNode = block.SelectSingleNode(".//div[contains(@class, 'parameter-name')]");
                if (nameNode != null)
                {
                    param.Name = nameNode.InnerText.Trim();
                }

                // Извлекаем тип параметра
                var typeNode = block.SelectSingleNode(".//div[contains(@class, 'parameter-type')]");
                if (typeNode != null)
                {
                    var typeName = typeNode.InnerText.Trim().ToUpper();
                    param.Type = typeName switch
                    {
                        "DATE" => typeof(DateTime),
                        "DATETIME" => typeof(DateTime),
                        "STRING" => typeof(string),
                        "INT" => typeof(int),
                        "INTEGER" => typeof(int),
                        "BOOL" => typeof(bool),
                        "BOOLEAN" => typeof(bool),
                        "DECIMAL" => typeof(decimal),
                        "DOUBLE" => typeof(double),
                        "FLOAT" => typeof(float),
                        "GUID" => typeof(Guid),
                        _ => typeof(string)
                    };
                }

                // Извлекаем значение (по умолчанию)
                var preNode = block.SelectSingleNode(".//pre");
                if (preNode != null)
                {
                    var valueStr = preNode.InnerText.Trim();
                    param.Value = ConvertValue(valueStr, param.Type);
                }

                // Добавляем в словарь
                if (!string.IsNullOrEmpty(param.Name) && !_parameters.ContainsKey(param.Name))
                {
                    _parameters.Add(param.Name, param);
                }
            }
        }

        public void SetParameters()
        {
            var window = new ParameterWindow(_parameters);

            if (window.ShowDialog() == true)
            {
                // Получаем обновленные значения
                var updatedValues = window.UpdatedValues;

                // Применяем параметры
                foreach (var kvp in updatedValues)
                {
                    //SetParameter(kvp.Key, kvp.Value?.ToString());
                }

                // Загружаем отчет
            }
        }

        private List<string> ExtractQueries(string html)
        {
            var queries = new List<string>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var queryNodes = doc.DocumentNode.SelectNodes("//div[@data-query]");
            if (queryNodes != null)
            {
                foreach (var node in queryNodes)
                {
                    var sql = node.InnerText.Trim();
                    queries.Add(CleanQuery(sql));
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
                    var value = FormatValue(param.Value?.Value);
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

        private static object? ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                if (targetType == typeof(string))
                    return value;

                if (targetType == typeof(DateTime))
                    return DateTime.Parse(value);

                if (targetType == typeof(int))
                    return int.Parse(value);

                if (targetType == typeof(bool))
                    return bool.Parse(value);

                if (targetType == typeof(decimal))
                    return decimal.Parse(value);

                if (targetType == typeof(double))
                    return double.Parse(value);

                if (targetType == typeof(float))
                    return float.Parse(value);

                if (targetType == typeof(Guid))
                    return Guid.Parse(value);

                return value;
            }
            catch
            {
                return value; // Возвращаем как строку в случае ошибки
            }
        }

        private async Task<object> ExecuteScalarAsync(SqliteConnection connection, string query)
        {
            using (var command = new SqliteCommand(query, connection))
            {
                command.CommandTimeout = 60;
                return await command.ExecuteScalarAsync();
            }
        }

        public void SetParameter(string name, ReportParameter value)
        {
            _parameters[name] = value;
        }

        private string GetFullPath(string relativePath)
        {
            if (Path.IsPathRooted(relativePath))
                return relativePath;

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDirectory, relativePath);
        }
    }
}