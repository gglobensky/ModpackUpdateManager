using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Managers
{
    public static class Utilities
    {
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public static async Task FileWriteAsync(string filePath, string message, bool append)
        {
            await _semaphore.WaitAsync();

            try
            {
                using (FileStream stream = new FileStream(filePath, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    await sw.WriteLineAsync(message);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static string ReadFile(string path)
        {
            string result = "";

            if (File.Exists(path))
            {
                using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
                {
                    result = streamReader.ReadToEnd();
                }
            }

            return result;
        }

        public static int GetIndexOfShortestString(List<string> input)
        {
            int index = 0;
            int minimum = Int32.MaxValue;

            int len = input.Count;

            for (int i = 0; i < len; i++)
            {
                if (input[i].Length < minimum)
                {
                    minimum = input[i].Length;
                    index = i;
                }
            }

            return index;
        }

        public static int CountCharsInString(string source, char target)
        {
            int count = 0;
            foreach (char c in source)
                if (c == target) count++;

            return count;
        }

        public static string ToHumanReadable(string input)
        {
            input = System.Text.RegularExpressions.Regex.Replace(input, "([^\\W](?=[A-Z][a-z]))", "$1 ", System.Text.RegularExpressions.RegexOptions.Compiled);
            return System.Text.RegularExpressions.Regex.Replace(input, "([a-z](?=[A-Z]))", "$1 ", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        public static string NormalizeString(string input)
        {
            input = ToHumanReadable(input);
            input = RemoveIrrelevantCharacters(input);
            input = RemoveVersioning(input);
            // String is a series of normalized words
            return input.ToLower();
        }

        public static string RemoveVersioning(string input)
        {
            // Remove potential versioning
            input = System.Text.RegularExpressions.Regex.Replace(input, "([a-z]|[A-Z])\\d+(\\.|)(\\w+|)", " ", System.Text.RegularExpressions.RegexOptions.Compiled);
            // Remove potential lone numbers
            input = System.Text.RegularExpressions.Regex.Replace(input, "( |^)\\d+( |$)", " ", System.Text.RegularExpressions.RegexOptions.Compiled);

            return RemoveDoubleSpacings(input);
        }

        public static string RemoveDoubleSpacings(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "\\s\\s+", " ", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        public static string RemoveIrrelevantCharacters(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "[^a-zA-Z0-9-']+", " ", System.Text.RegularExpressions.RegexOptions.Compiled);
        }
        public static bool TryParseJson<T>(this string @this, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };

            result = JsonConvert.DeserializeObject<T>(@this, settings);
            return success;
        }

        public static bool ContainsAllWords(string source, string target)
        {
            string[] sourceWords = source.ToLower().Split(' ');
            string[] targetWords = target.ToLower().Split(' ');

            foreach (string sourceWord in sourceWords)
            {
                if (!targetWords.Contains(sourceWord))
                {
                    return false;
                }
            }

            return true;
        }

        public static string FastReplace(string input, string target, string replacement, bool caseSensitive = false, bool exactMatch = false)
        {
            System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.Compiled;

            if (!caseSensitive)
            {
                options |= System.Text.RegularExpressions.RegexOptions.IgnoreCase;
            }

            if (exactMatch)
            {
                target = $"( |^){target}( |$)";
            }

            return System.Text.RegularExpressions.Regex.Replace(input, target, replacement, options );
        }

        public static bool IsSourceInTarget(string source, string target, bool caseSensitive = false, bool exactMatch = false)
        {
            System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.Compiled;

            if (!caseSensitive)
            {
                options |= System.Text.RegularExpressions.RegexOptions.IgnoreCase;
            }

            if (exactMatch)
            {
                target = $"( |^){target}( |$)";
            }

            return System.Text.RegularExpressions.Regex.IsMatch(target, source, options);
        }

        public static string BuildCurseForgeSearchUrl(string modName)
        {
            modName = System.Web.HttpUtility.UrlEncode(modName);
            return $"{AppSettings.GetCurseforgeMinecraftSearchUrl()}&search={modName}";
        }

    }
}
