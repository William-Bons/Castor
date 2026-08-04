using Castor.database;
using Castor.database.tab_medis;
using Castor.Properties;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Castor.gui.reports
{
    /// <summary>
    /// Представляет параметр отчета с именем, типом, значением и метаданными
    /// </summary>
    public class ReportParameter
    {
        public ReportParameter() { }
        public string Name { get; set; } = string.Empty;
        public Type Type { get; set; } = typeof(string);
        public object? Value { get; set; } = null;
        public string? Query { get; set; } = string.Empty;
        public Type? Context { get; set; } = typeof(CastorContext);
        public List<object>? Items { get; set; }
    }

    /// <summary>
    /// Представляет SQL-запрос отчета с именем и контекстом выполнения
    /// </summary>
    public class ReportQuery
    {
        public ReportQuery() { }
        public string Query { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Type Context { get; set; } = typeof(CastorContext);
    }

    /// <summary>
    /// Основной класс для вычисления отчетов
    /// </summary>
    public class ReportCalculator
    {
        private readonly IBrowserAdapter _browserAdapter;
        private readonly Dictionary<string, ReportParameter> _parameters = new Dictionary<string, ReportParameter>();
        private IEnumerable<ReportQuery>? _queries;
        private string? _html;
        private bool _isParameterRequired = false;

        /// <summary>
        /// Путь к текущему загруженному отчету
        /// </summary>
        public string? CurrentReportPath { get; private set; }

        /// <summary>
        /// Конструктор с адаптером браузера
        /// </summary>
        /// <param name="browserAdapter">Адаптер для взаимодействия с браузером</param>
        public ReportCalculator(IBrowserAdapter browserAdapter)
        {
            _browserAdapter = browserAdapter ?? throw new ArgumentNullException(nameof(browserAdapter));
        }

        /// <summary>
        /// Асинхронно выполняет расчет отчета на основе HTML шаблона
        /// </summary>
        /// <param name="htmlPath">Путь к HTML файлу с шаблоном отчета</param>
        public async Task CalculateAsync(string htmlPath)
        {
            if (string.IsNullOrEmpty(htmlPath))
                throw new ArgumentException("Путь к HTML файлу не может быть пустым", nameof(htmlPath));

            if (!File.Exists(htmlPath))
                throw new FileNotFoundException($"HTML файл не найден: {htmlPath}");

            CurrentReportPath = htmlPath;
            _html = File.ReadAllText(htmlPath);

            // Парсим параметры отчета
            ParseParameters(_html);

            // Если требуются параметры - открываем окно и завершаем
            if (_isParameterRequired)
            {
                await SetParameters();
                return;
            }

            // Иначе парсим SQL запросы и загружаем браузер
            await ParseSqlQueriesAsync(_html);
        }

        /// <summary>
        /// Загружает список доступных отчетов
        /// </summary>
        public async Task LoadReportListAsync(string htmlPath)
        {
            if (!File.Exists(htmlPath))
                throw new FileNotFoundException($"Файл списка отчетов не найден: {htmlPath}");

            var html = File.ReadAllText(htmlPath);
            await _browserAdapter.NavigateToStringAsync(html);
        }

        /// <summary>
        /// Парсит параметры отчета из HTML
        /// </summary>
        private async void ParseParameters(string html)
        {
            _parameters.Clear();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Ищем блок параметров
            var sectionNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'parameter-section')]");

            // Проверяем наличие элемента с классом required внутри блока parameter-section
            var isRequired = false;
            if (sectionNode != null)
            {
                var requiredNode = sectionNode.SelectSingleNode(".//div[contains(@class, 'required')]");
                isRequired = requiredNode != null;
            }

            _isParameterRequired = isRequired;

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

                // Извлекаем значение по умолчанию
                var preNode = block.SelectSingleNode(".//pre");
                if (preNode != null)
                {
                    var valueStr = preNode.InnerText.Trim();
                    param.Value = ConvertValue(valueStr, param.Type);
                }

                // Извлекаем запрос для параметров выбора
                var qnode = block.SelectSingleNode(".//div[contains(@class, 'parameter-query')]");
                if (qnode != null)
                {
                    param.Query = qnode.InnerText?.Trim() ?? string.Empty;
                    param.Context = GetContextType(qnode.GetAttributeValue("context", string.Empty));
                    param.Items = new List<object>();

                    try
                    {
                        // Заменяем параметры в запросе
                        var sql = ReplaceParameters(param.Query);
                        using DbContext context = (DbContext)Activator.CreateInstance(param?.Context ?? typeof(CastorContext));
                        using DbConnection connection = context is CastorContext ?
                            new SqliteConnection(context.Database.GetConnectionString()) :
                            new NpgsqlConnection(context.Database.GetConnectionString());

                        await connection.OpenAsync();

                        using DbCommand command = connection is SqliteConnection ?
                            new SqliteCommand(sql, (SqliteConnection?)connection) :
                            new NpgsqlCommand(sql, (NpgsqlConnection?)connection);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                param.Items.Add(new { ID = reader.GetValue(0), Value = reader.GetValue(1) });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Ошибка выполнения запроса для параметра {param.Name}: {ex.Message}");
                        param.Items = new List<object>();
                    }
                }

                // Извлекаем значение по умолчанию из настроек приложения
                var snode = block.SelectSingleNode(".//div[contains(@class, 'parameter-current')]");
                if (snode != null)
                {
                    var settingName = snode.InnerText?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(settingName))
                    {
                        try
                        {
                            param.Value = Settings.Default[settingName];
                        }
                        catch
                        {
                            // Если настройка не найдена, оставляем значение по умолчанию
                        }
                    }
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
                var updatedValues = window.UpdatedValues;

                foreach (var kvp in updatedValues)
                {
                    if (_parameters.ContainsKey(kvp.Key))
                    {
                        _parameters[kvp.Key].Value = kvp.Value;
                    }
                }

                // Сохраняем настройки
                SaveParametersToSettings();

                // Загружаем браузер с данными и выполняем скрипты
                await LoadBrowser();
            }
        }

        /// <summary>
        /// Сохраняет параметры в настройки приложения
        /// </summary>
        private void SaveParametersToSettings()
        {
            try
            {
                foreach (var param in _parameters.Values)
                {
                    // Сохраняем только если есть атрибут parameter-current
                    // Ищем соответствующий элемент в HTML для определения имени настройки
                    // Этот метод может быть расширен в зависимости от логики приложения
                    if (param.Value != null)
                    {
                        // Пример сохранения - можно адаптировать под свои нужды
                        var settingName = $"Param_{param.Name}";
                        if (Settings.Default.Properties[settingName] != null)
                        {
                            Settings.Default[settingName] = param.Value;
                        }
                    }
                }
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения параметров: {ex.Message}");
            }
        }

        /// <summary>
        /// Парсит блоки SQL-запросов из HTML
        /// </summary>
        private async Task ParseSqlQueriesAsync(string html)
        {
            var result = new List<ReportQuery>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Ищем все div с атрибутом data-query внутри блока #sqlQueries
            var queryNodes = doc.DocumentNode.SelectNodes("//div[@data-query]");

            if (queryNodes != null && queryNodes.Count > 0)
            {
                foreach (var node in queryNodes)
                {
                    var query = new ReportQuery();

                    // Извлекаем имя запроса (data-query)
                    query.Name = node.GetAttributeValue("data-query", string.Empty);

                    // Извлекаем контекст (data-context)
                    var contextName = node.GetAttributeValue("data-context", "CastorContext");
                    query.Context = GetContextType(contextName);

                    // Извлекаем текст SQL-запроса
                    query.Query = CleanQuery(node.InnerText.Trim());

                    result.Add(query);
                }
            }

            _queries = result;

            // Загружаем браузер с данными и выполняем скрипты
            await LoadBrowser();
        }

        /// <summary>
        /// Загружает браузер с HTML и выполняет JavaScript для обновления данных
        /// </summary>
        private async Task LoadBrowser()
        {
            var results = new List<object>();
            var pvalues = new List<string>();

            // Выполняем SQL запросы
            if (_queries != null)
            {
                foreach (var query in _queries)
                {
                    try
                    {
                        var sql = ReplaceParameters(query.Query);
                        using DbContext context = (DbContext)Activator.CreateInstance(query?.Context ?? typeof(CastorContext));
                        using DbConnection connection = context is CastorContext ?
                            new SqliteConnection(context.Database.GetConnectionString()) :
                            new NpgsqlConnection(context.Database.GetConnectionString());

                        await connection.OpenAsync();

                        using DbCommand command = connection is SqliteConnection ?
                            new SqliteCommand(sql, (SqliteConnection?)connection) :
                            new NpgsqlCommand(sql, (NpgsqlConnection?)connection);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    results.Add(reader.IsDBNull(i) ? string.Empty : reader.GetValue(i));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add($"Ошибка: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Ошибка выполнения запроса {query.Name}: {ex.Message}");
                    }
                }
            }

            // Добавляем значения параметров
            pvalues.AddRange(_parameters.Values.Select(p => p.Value?.ToString() ?? string.Empty));

            // Загружаем HTML в браузер
            if (!string.IsNullOrEmpty(_html))
            {
                await _browserAdapter.NavigateToStringAsync(_html);
            }
            else
            {
                throw new InvalidOperationException("HTML контент не загружен");
            }

            // Ожидаем загрузки HTML страницы
            await Task.Delay(500);

            // Сериализуем результаты в JSON
            var dataJson = JsonConvert.SerializeObject(results.ToArray());
            var paraJson = JsonConvert.SerializeObject(pvalues.ToArray());

            // Формируем JavaScript код для вызова функции updateReport
            var script = $"updateReport({dataJson}, {paraJson});";

            // Выполняем JavaScript код в загруженной HTML странице
            await _browserAdapter.InvokeScriptAsync(script);
        }

        /// <summary>
        /// Преобразует строковое имя контекста в тип
        /// </summary>
        private Type GetContextType(string contextName)
        {
            return contextName switch
            {
                "MedisContext" => typeof(MedisContext),
                "CastorContext" => typeof(CastorContext),
                _ => typeof(CastorContext)
            };
        }

        /// <summary>
        /// Очищает SQL-запрос от лишних символов
        /// </summary>
        private string CleanQuery(string query)
        {
            query = Regex.Replace(query, @"<!\[CDATA\[|\]\]>", "");
            query = Regex.Replace(query, @"--.*?$", "", RegexOptions.Multiline);
            query = Regex.Replace(query, @"/\*.*?\*/", "", RegexOptions.Singleline);
            query = Regex.Replace(query, @"\s+", " ");
            return query.Trim();
        }

        /// <summary>
        /// Заменяет параметры в SQL-запросе на их значения
        /// </summary>
        private string ReplaceParameters(string query)
        {
            if (string.IsNullOrEmpty(query))
                return query;

            var result = query;
            foreach (var param in _parameters)
            {
                var placeholder = $"@{{{param.Key}}}";
                if (result.Contains(placeholder))
                {
                    var value = FormatValue(param.Value?.Value);
                    result = result.Replace(placeholder, value);
                }
            }
            return result;
        }

        /// <summary>
        /// Форматирует значение для вставки в SQL-запрос
        /// </summary>
        private string FormatValue(object? value)
        {
            if (value == null) return "NULL";

            if (value is string str)
                return $"'{str.Replace("'", "''")}'";

            if (value is DateTime dt)
                return $"{dt:yyyy-MM-dd}";

            if (value is DateOnly date)
                return $"{date:yyyy-MM-dd}";

            if (value is bool b)
                return b ? "1" : "0";

            if (value is Enum)
                return $"{(int)value}";

            if (value is int || value is long || value is decimal || value is double || value is float)
                return value.ToString() ?? "0";

            return $"'{value.ToString()?.Replace("'", "''")}'";
        }

        /// <summary>
        /// Преобразует строковое значение в указанный тип
        /// </summary>
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
                return value;
            }
        }

        /// <summary>
        /// Получает значение параметра по имени
        /// </summary>
        public object? GetParameterValue(string name)
        {
            if (_parameters.TryGetValue(name, out var param))
            {
                return param.Value;
            }
            return null;
        }

        /// <summary>
        /// Устанавливает значение параметра по имени
        /// </summary>
        public void SetParameterValue(string name, object? value)
        {
            if (_parameters.TryGetValue(name, out var param))
            {
                param.Value = value;
            }
        }

        /// <summary>
        /// Получает список всех параметров
        /// </summary>
        public Dictionary<string, ReportParameter> GetParameters()
        {
            return new Dictionary<string, ReportParameter>(_parameters);
        }
    }
}