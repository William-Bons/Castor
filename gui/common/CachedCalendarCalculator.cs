using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;


namespace Castor.gui.common
{


    public class CachedCalendarCalculator
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        // Потокобезопасный кэш: Ключ — Год, Значение — строка из 365/366 символов ('0', '1', '2')
        private static readonly ConcurrentDictionary<int, string> _calendarCache = new ConcurrentDictionary<int, string>();

        /// <summary>
        /// Вычисляет 15-й день с корректировкой в уменьшение (использует локальный кэш).
        /// </summary>
        public static async Task<DateTime> Get15thDayCorrectedAsync(DateTime startDate)
        {
            // 1. Прибавляем 15 календарных дней
            DateTime targetDate = startDate.AddDays(14);

            // 2. Сдвигаем дату назад, пока она является нерабочей
            while (await IsNonWorkingDayAsync(targetDate))
            {
                targetDate = targetDate.AddDays(-1);
            }

            return targetDate;
        }

        /// <summary>
        /// Проверяет день по локальному кэшу года. Если данных нет — скачивает год целиком.
        /// </summary>
        private static async Task<bool> IsNonWorkingDayAsync(DateTime date)
        {
            int year = date.Year;

            // Если данных по году нет в кэше, скачиваем их из интернета
            if (!_calendarCache.ContainsKey(year))
            {
                string yearData = DownloadYearCalendarAsync(year);
                _calendarCache[year] = yearData;
            }

            // Получаем строку календаря для нужного года
            string cachedYear = _calendarCache[year];

            // Если кэш пустой (ошибка сети), откатываемся на стандартные Сб/Вс
            if (string.IsNullOrEmpty(cachedYear) || cachedYear.Length < 365)
            {
                return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            }

            // Порядковый номер дня в году (от 1 до 365/366)
            int dayOfYearIndex = date.DayOfYear - 1;

            // Защита от выхода за границы строки
            if (dayOfYearIndex >= cachedYear.Length) return false;

            // '1' означает нерабочий день (праздник или выходной с учетом переносов)
            return cachedYear[dayOfYearIndex] == '1';
        }

        /// <summary>
        /// Загружает из API строку календаря на весь год (где каждый символ — это статус дня)
        /// </summary>
        private static string DownloadYearCalendarAsync(int year)
        {
            string url = $"https://isdayoff.ru/api/getdata?year={year}";
            try
            {
                // API вернет строку вида "1111111100000..." (365 или 366 символов)
                string response = _httpClient.GetStringAsync(url).Result;
                return response.Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Ошибка] Не удалось загрузить календарь на {year} год: {ex.Message}");
                return string.Empty;
            }
        }
    }

}

