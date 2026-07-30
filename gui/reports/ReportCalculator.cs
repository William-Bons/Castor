using Castor.database;
using Castor.gui.reports;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Castor.gui.reports
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
        private readonly WebBrowser Browser;
        private IEnumerable<string>? queries;
        private string? html;

        public ReportCalculator(WebBrowser browser)
        {
            Browser = browser;
        }

        /// <summary>
        /// Асинхронно выполняет расчет отчета на основе HTML шаблона
        /// </summary>
        /// <param name="htmlPath">Путь к HTML файлу с шаблоном отчета</param>
        public async Task CalculateAsync(string htmlPath)
        {
            // Читаем содержимое HTML файла по указанному пути
            // html - поле класса, хранящее HTML содержимое
            html = File.ReadAllText(htmlPath);

            // Парсим HTML для извлечения параметров отчета
            // _parameters - словарь, заполняемый в процессе парсинга
            ParseParameters(html);

            // Извлекаем SQL запросы из HTML
            // queries - список строк с SQL запросами
            queries = ExtractQueries(html);

            // Загружаем браузер с данными и выполняем скрипты
            await LoadBrowser();
        }


        /// <summary>
        /// Асинхронно загружает HTML, подключается к БД и вызывает JavaScript функцию
        /// </summary>
        private async Task LoadBrowser()
        {
            // Список для хранения результатов выполнения SQL запросов
            var results = new List<string>();

            // Список для хранения значений параметров
            var pvalues = new List<string>();

            // Создаем контекст базы данных Castor
            using CastorContext context = new CastorContext();

            // Создаем подключение к SQLite базе данных
            using (var connection = new SqliteConnection(context.Database.GetConnectionString()))
            {
                // Открываем подключение к базе данных асинхронно
                await connection.OpenAsync();

                // Перебираем все SQL запросы из списка
                foreach (var query in queries ?? [])
                {
                    try
                    {
                        // Заменяем параметры в SQL запросе на их значения
                        var sql = ReplaceParameters(query);

                        // Выполняем SQL запрос и получаем скалярное значение
                        var result = await ExecuteScalarAsync(connection, sql);

                        // Добавляем результат в список (если null, то Empty)
                        results.Add(result?.ToString() ?? string.Empty);
                    }
                    catch (Exception ex)
                    {
                        // В случае ошибки добавляем сообщение об ошибке
                        results.Add($"Ошибка: {ex.Message}");
                    }
                }
            }

            // Извлекаем значения параметров из словаря _parameters и добавляем в список pvalues
            pvalues.AddRange(_parameters.Values.Select(p => p.Value.ToString()));

            // Загружаем HTML страницу в WebBrowser
            Browser.NavigateToString(html);

            // Ожидаем загрузки HTML страницы (500 мс для полной загрузки)
            await Task.Delay(500);

            // Сериализуем результаты в JSON строку
            var dataJson = JsonConvert.SerializeObject(results.ToArray());

            // Сериализуем значения параметров в JSON строку
            var paraJson = JsonConvert.SerializeObject(pvalues.ToArray());

            // Формируем JavaScript код для вызова функции updateReport
            // Передаем в функцию данные результатов и параметры
            var script = $"updateReport({dataJson}, {paraJson});";

            // Выполняем JavaScript код в загруженной HTML странице
            Browser.InvokeScript("eval", script);
        }

        /// <summary>
        /// формирует словарь параметров и заполняет значениями по умолчанию 
        /// </summary>
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

        /// <summary>
        /// Обновляет значения параметров через вызов окна ParameterWindow
        /// </summary>
        public async Task SetParameters()
        {
            var window = new ParameterWindow(_parameters);

            if (window.ShowDialog() == true)
            {
                // Получаем обновленные значения
                var updatedValues = window.UpdatedValues;

                // Применяем параметры
                foreach (var kvp in updatedValues)
                {
                    _parameters[kvp.Key].Value = kvp.Value;
                }

                // Загружаем отчет
                _ = LoadBrowser();
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

        private string FormatValue(object? value)
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
                command.CommandTimeout = 5;
                return await command.ExecuteScalarAsync();
            }
        }

    }
}