using Castor.database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.gui.login
{
    public static class LoginGenerator
    {
        public static async Task<string> GenerateUniqueLoginAsync(
            string fullName,
            int maxAttempts = 10)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "user01";

            var baseLogin = BuildBaseLogin(fullName);
            baseLogin = EnsureLength(baseLogin, 6);

            using CastorContext context = new CastorContext();
            string candidate = baseLogin;
            for (int i = 0; i < maxAttempts; i++)
            {
                var exists = await context.Users
                    .AnyAsync(u => u.Login == candidate && u.IsActive);

                if (!exists)
                    return candidate;

                candidate = $"{baseLogin}{i + 1}";
                candidate = EnsureLength(candidate, 6);
            }

            return $"{baseLogin}{new Random().Next(100, 999)}";
        }

        private static string BuildBaseLogin(string fullName)
        {
            var parts = fullName
                .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray();

            if (parts.Length == 0) return "usr";

            var surname = GetFirstChars(parts[0], 3);
            var name = parts.Length > 1 ? parts[1] : "";
            var patronymic = parts.Length > 2 ? parts[2] : "";

            var iPart = name.Length > 0 ? name[0].ToString() : "x";
            var oPart = patronymic.Length > 0 ? patronymic[0].ToString() : "x";

            var raw = $"{surname}{iPart}{oPart}";

            var latin = TransliterateCyrillic(raw);
            latin = System.Text.RegularExpressions.Regex.Replace(latin, "[^A-Za-z]", "")
                .ToLowerInvariant();

            return latin;
        }

        private static string GetFirstChars(string src, int count)
            => string.IsNullOrEmpty(src) ? "" : src.Length <= count ? src : src.Substring(0, count);

        private static string EnsureLength(string input, int targetLength)
        {
            if (input.Length >= targetLength)
                return input.Substring(0, targetLength);

            var sb = new System.Text.StringBuilder(input);
            while (sb.Length < targetLength)
                sb.Append((sb.Length % 10).ToString());
            return sb.ToString();
        }

        private static string TransliterateCyrillic(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            var map = new System.Collections.Generic.Dictionary<char, string>
            {
                {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"},
                {'е', "e"}, {'ё', "yo"}, {'ж', "zh"}, {'з', "z"}, {'и', "i"},
                {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"},
                {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"},
                {'у', "u"}, {'ф', "f"}, {'х', "h"}, {'ц', "ts"}, {'ч', "ch"},
                {'ш', "sh"}, {'щ', "sch"}, {'ы', "y"}, {'э', "e"}, {'ю', "yu"},
                {'я', "ya"},
                // заглавные
                {'А', "A"}, {'Б', "B"}, {'В', "V"}, {'Г', "G"}, {'Д', "D"},
                {'Е', "E"}, {'Ё', "Yo"}, {'Ж', "Zh"}, {'З', "Z"}, {'И', "I"},
                {'Й', "Y"}, {'К', "K"}, {'Л', "L"}, {'М', "M"}, {'Н', "N"},
                {'О', "O"}, {'П', "P"}, {'Р', "R"}, {'С', "S"}, {'Т', "T"},
                {'У', "U"}, {'Ф', "F"}, {'Х', "H"}, {'Ц', "Ts"}, {'Ч', "Ch"},
                {'Ш', "Sh"}, {'Щ', "Sch"}, {'Ы', "Y"}, {'Э', "E"}, {'Ю', "Yu"},
                {'Я', "Ya"}
            };

            var sb = new System.Text.StringBuilder(input.Length);
            foreach (var ch in input)
            {
                if (map.TryGetValue(ch, out var replacement))
                    sb.Append(replacement);
                else
                    sb.Append(ch); // если вдруг попадётся что-то ещё — оставляем как есть
            }

            return sb.ToString();
        }
    }

}
