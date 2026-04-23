using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlOnAir.DotNet.Lib
{
    /// <summary>
    /// Static class providing data formula functions for use in C# applications
    /// These functions mirror the behavior of common data formula functions
    /// </summary>
    public static class DataExtensions
    {
        #region Logical Functions

        /// <summary>
        /// Returns true if all arguments are true, false otherwise
        /// </summary>
        public static bool AND(params bool[] conditions)
        {
            return conditions.All(c => c);
        }

        /// <summary>
        /// Returns true if all arguments are true, false otherwise (null values are treated as false)
        /// </summary>
        public static bool AND(params bool?[] conditions)
        {
            return conditions.All(c => c ?? false);
        }

        /// <summary>
        /// Returns true if any argument is true, false otherwise
        /// </summary>
        public static bool OR(params bool[] conditions)
        {
            return conditions.Any(c => c);
        }

        /// <summary>
        /// Returns true if any argument is true, false otherwise (null values are treated as false)
        /// </summary>
        public static bool OR(params bool?[] conditions)
        {
            return conditions.Any(c => c ?? false);
        }

        /// <summary>
        /// Returns the opposite of the given boolean value
        /// </summary>
        public static bool NOT(bool condition)
        {
            return !condition;
        }

        /// <summary>
        /// Returns the opposite of the given nullable boolean value (null is treated as false)
        /// </summary>
        public static bool NOT(bool? condition)
        {
            return !(condition ?? false);
        }

        /// <summary>
        /// Returns true if the value is blank (null, empty string, or whitespace)
        /// </summary>
        public static bool BLANK(object? value)
        {
            return value == null || 
                   (value is string str && string.IsNullOrWhiteSpace(str)) ||
                   (value is DateTime dt && dt == default(DateTime));
        }

        /// <summary>
        /// Returns the first value if condition is true, otherwise returns the second value
        /// </summary>
        public static T IF<T>(bool condition, T trueValue, T falseValue)
        {
            return condition ? trueValue : falseValue;
        }

        /// <summary>
        /// Returns true constant
        /// </summary>
        public static bool TRUE() => true;

        /// <summary>
        /// Returns false constant
        /// </summary>
        public static bool FALSE() => false;

        #endregion

        #region Text Functions

        /// <summary>
        /// Returns the leftmost characters from a string
        /// </summary>
        public static string LEFT(string? text, int numChars)
        {
            if (string.IsNullOrEmpty(text) || numChars <= 0) return string.Empty;
            return text.Length <= numChars ? text : text.Substring(0, numChars);
        }

        /// <summary>
        /// Returns the rightmost characters from a string
        /// </summary>
        public static string RIGHT(string? text, int numChars)
        {
            if (string.IsNullOrEmpty(text) || numChars <= 0) return string.Empty;
            return text.Length <= numChars ? text : text.Substring(text.Length - numChars);
        }

        /// <summary>
        /// Returns a substring from the middle of a string
        /// </summary>
        public static string MID(string? text, int startPos, int numChars)
        {
            if (string.IsNullOrEmpty(text) || startPos < 1 || numChars <= 0) return string.Empty;
            int startIndex = startPos - 1; // Airtable uses 1-based indexing
            if (startIndex >= text.Length) return string.Empty;
            int remainingChars = text.Length - startIndex;
            int charsToTake = Math.Min(numChars, remainingChars);
            return text.Substring(startIndex, charsToTake);
        }

        /// <summary>
        /// Returns the length of a string
        /// </summary>
        public static int LEN(string? text)
        {
            return text?.Length ?? 0;
        }

        /// <summary>
        /// Converts text to uppercase
        /// </summary>
        public static string UPPER(string? text)
        {
            return text?.ToUpper() ?? string.Empty;
        }

        /// <summary>
        /// Converts text to lowercase
        /// </summary>
        public static string LOWER(string? text)
        {
            return text?.ToLower() ?? string.Empty;
        }

        /// <summary>
        /// Removes leading and trailing whitespace
        /// </summary>
        public static string TRIM(string? text)
        {
            return text?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Concatenates multiple strings
        /// </summary>
        public static string CONCATENATE(params string?[] texts)
        {
            return string.Concat(texts.Where(t => t != null));
        }

        /// <summary>
        /// Replaces occurrences of old text with new text
        /// </summary>
        public static string SUBSTITUTE(string? text, string? oldText, string? newText, int? instanceNum = null)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(oldText)) return text ?? string.Empty;
            newText ??= string.Empty;

            if (instanceNum.HasValue && instanceNum.Value > 0)
            {
                // Replace only the specified instance
                int currentInstance = 0;
                int startIndex = 0;
                while (startIndex < text.Length)
                {
                    int index = text.IndexOf(oldText, startIndex, StringComparison.Ordinal);
                    if (index == -1) break;
                    
                    currentInstance++;
                    if (currentInstance == instanceNum.Value)
                    {
                        return text.Substring(0, index) + newText + text.Substring(index + oldText.Length);
                    }
                    startIndex = index + oldText.Length;
                }
                return text;
            }
            else
            {
                // Replace all instances
                return text.Replace(oldText, newText);
            }
        }

        /// <summary>
        /// Finds the position of a substring within a string (1-based indexing)
        /// </summary>
        public static int FIND(string? findText, string? withinText, int? startNum = null)
        {
            if (string.IsNullOrEmpty(findText) || string.IsNullOrEmpty(withinText)) return 0;
            
            int startIndex = (startNum ?? 1) - 1; // Convert to 0-based indexing
            if (startIndex < 0 || startIndex >= withinText.Length) return 0;
            
            int index = withinText.IndexOf(findText, startIndex, StringComparison.Ordinal);
            return index == -1 ? 0 : index + 1; // Convert back to 1-based indexing
        }

        /// <summary>
        /// Searches for a substring within a string (case-insensitive, 1-based indexing)
        /// </summary>
        public static int SEARCH(string? findText, string? withinText, int? startNum = null)
        {
            if (string.IsNullOrEmpty(findText) || string.IsNullOrEmpty(withinText)) return 0;
            
            int startIndex = (startNum ?? 1) - 1; // Convert to 0-based indexing
            if (startIndex < 0 || startIndex >= withinText.Length) return 0;
            
            int index = withinText.IndexOf(findText, startIndex, StringComparison.OrdinalIgnoreCase);
            return index == -1 ? 0 : index + 1; // Convert back to 1-based indexing
        }

        /// <summary>
        /// Repeats text a specified number of times
        /// </summary>
        public static string REPT(string? text, int numTimes)
        {
            if (string.IsNullOrEmpty(text) || numTimes <= 0) return string.Empty;
            return string.Concat(Enumerable.Repeat(text, numTimes));
        }

        #endregion

        #region Numeric Functions

        /// <summary>
        /// Returns the absolute value of a number
        /// </summary>
        public static double ABS(double number)
        {
            return Math.Abs(number);
        }

        /// <summary>
        /// Rounds a number up to the nearest integer
        /// </summary>
        public static double CEILING(double number)
        {
            return Math.Ceiling(number);
        }

        /// <summary>
        /// Rounds a number down to the nearest integer
        /// </summary>
        public static double FLOOR(double number)
        {
            return Math.Floor(number);
        }

        /// <summary>
        /// Returns the integer portion of a number
        /// </summary>
        public static double INT(double number)
        {
            return Math.Truncate(number);
        }

        /// <summary>
        /// Returns the remainder after division
        /// </summary>
        public static double MOD(double number, double divisor)
        {
            if (divisor == 0) throw new DivideByZeroException("Division by zero in MOD function");
            return number % divisor;
        }

        /// <summary>
        /// Raises a number to a power
        /// </summary>
        public static double POWER(double number, double power)
        {
            return Math.Pow(number, power);
        }

        /// <summary>
        /// Rounds a number to a specified number of digits
        /// </summary>
        public static double ROUND(double number, int numDigits = 0)
        {
            return Math.Round(number, numDigits);
        }

        /// <summary>
        /// Returns the square root of a number
        /// </summary>
        public static double SQRT(double number)
        {
            if (number < 0) throw new ArgumentException("Cannot calculate square root of negative number");
            return Math.Sqrt(number);
        }

        /// <summary>
        /// Returns the sum of numbers
        /// </summary>
        public static double SUM(params double[] numbers)
        {
            return numbers.Sum();
        }

        /// <summary>
        /// Returns the average of numbers
        /// </summary>
        public static double AVERAGE(params double[] numbers)
        {
            if (numbers.Length == 0) return 0;
            return numbers.Average();
        }

        /// <summary>
        /// Returns the maximum value from a set of numbers
        /// </summary>
        public static double MAX(params double[] numbers)
        {
            if (numbers.Length == 0) return 0;
            return numbers.Max();
        }

        /// <summary>
        /// Returns the minimum value from a set of numbers
        /// </summary>
        public static double MIN(params double[] numbers)
        {
            if (numbers.Length == 0) return 0;
            return numbers.Min();
        }

        /// <summary>
        /// Counts the number of non-empty values
        /// </summary>
        public static int COUNT(params object?[] values)
        {
            return values.Count(v => !BLANK(v));
        }

        #endregion

        #region Date Functions

        /// <summary>
        /// Returns the current date and time
        /// </summary>
        public static DateTime NOW()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Returns the current date (without time)
        /// </summary>
        public static DateTime TODAY()
        {
            return DateTime.Today;
        }

        /// <summary>
        /// Returns the year from a date
        /// </summary>
        public static int YEAR(DateTime date)
        {
            return date.Year;
        }

        /// <summary>
        /// Returns the month from a date (1-12)
        /// </summary>
        public static int MONTH(DateTime date)
        {
            return date.Month;
        }

        /// <summary>
        /// Returns the day from a date (1-31)
        /// </summary>
        public static int DAY(DateTime date)
        {
            return date.Day;
        }

        /// <summary>
        /// Returns the hour from a datetime (0-23)
        /// </summary>
        public static int HOUR(DateTime dateTime)
        {
            return dateTime.Hour;
        }

        /// <summary>
        /// Returns the minute from a datetime (0-59)
        /// </summary>
        public static int MINUTE(DateTime dateTime)
        {
            return dateTime.Minute;
        }

        /// <summary>
        /// Returns the second from a datetime (0-59)
        /// </summary>
        public static int SECOND(DateTime dateTime)
        {
            return dateTime.Second;
        }

        /// <summary>
        /// Creates a date from year, month, and day
        /// </summary>
        public static DateTime DATE(int year, int month, int day)
        {
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Adds days to a date
        /// </summary>
        public static DateTime DATEADD(DateTime date, int days, string unit = "days")
        {
            return unit.ToLower() switch
            {
                "days" => date.AddDays(days),
                "months" => date.AddMonths(days),
                "years" => date.AddYears(days),
                "hours" => date.AddHours(days),
                "minutes" => date.AddMinutes(days),
                "seconds" => date.AddSeconds(days),
                _ => date.AddDays(days)
            };
        }

        /// <summary>
        /// Returns the difference between two dates in days
        /// </summary>
        public static int DATETIME_DIFF(DateTime date1, DateTime date2, string unit = "days")
        {
            var diff = date2 - date1;
            return unit.ToLower() switch
            {
                "days" => (int)diff.TotalDays,
                "hours" => (int)diff.TotalHours,
                "minutes" => (int)diff.TotalMinutes,
                "seconds" => (int)diff.TotalSeconds,
                "months" => ((date2.Year - date1.Year) * 12) + date2.Month - date1.Month,
                "years" => date2.Year - date1.Year,
                _ => (int)diff.TotalDays
            };
        }

        /// <summary>
        /// Formats a date as a string
        /// </summary>
        public static string DATETIME_FORMAT(DateTime date, string format = "MM/dd/yyyy")
        {
            return date.ToString(format);
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// Converts a value to text
        /// </summary>
        public static string VALUE_TO_TEXT(object? value)
        {
            return value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Converts text to a number
        /// </summary>
        public static double VALUE(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            return double.TryParse(text.Trim(), out double result) ? result : 0;
        }

        /// <summary>
        /// Returns true if the value is a number
        /// </summary>
        public static bool ISNUMBER(object? value)
        {
            return value is double || value is float || value is int || value is long || value is decimal ||
                   (value is string str && double.TryParse(str, out _));
        }

        /// <summary>
        /// Returns true if the value is an error
        /// </summary>
        public static bool ISERROR(object? value)
        {
            return value is Exception;
        }

        /// <summary>
        /// Returns a value if it's not an error, otherwise returns an alternative value
        /// </summary>
        public static T IFERROR<T>(T value, T errorValue)
        {
            return ISERROR(value) ? errorValue : value;
        }

        /// <summary>
        /// Generates a regular expression pattern match
        /// </summary>
        public static bool REGEX_MATCH(string? text, string pattern)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern)) return false;
            try
            {
                return Regex.IsMatch(text, pattern);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Extracts text using a regular expression
        /// </summary>
        public static string REGEX_EXTRACT(string? text, string pattern)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern)) return string.Empty;
            try
            {
                var match = Regex.Match(text, pattern);
                return match.Success ? match.Value : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Replaces text using a regular expression
        /// </summary>
        public static string REGEX_REPLACE(string? text, string pattern, string replacement)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern)) return text ?? string.Empty;
            try
            {
                return Regex.Replace(text, pattern, replacement ?? string.Empty);
            }
            catch
            {
                return text;
            }
        }

        #endregion

        #region Array/Collection Functions

        /// <summary>
        /// Joins array elements with a separator
        /// </summary>
        public static string ARRAYJOIN<T>(IEnumerable<T> array, string separator = ", ")
        {
            return string.Join(separator, array.Where(item => !BLANK(item)));
        }

        /// <summary>
        /// Returns the number of items in an array
        /// </summary>
        public static int ARRAYLEN<T>(IEnumerable<T>? array)
        {
            return array?.Count() ?? 0;
        }

        /// <summary>
        /// Returns unique values from an array
        /// </summary>
        public static IEnumerable<T> ARRAYUNIQUE<T>(IEnumerable<T> array)
        {
            return array.Distinct();
        }

        /// <summary>
        /// Flattens nested arrays into a single array
        /// </summary>
        public static IEnumerable<T> ARRAYFLATTEN<T>(IEnumerable<IEnumerable<T>> arrays)
        {
            return arrays.SelectMany(arr => arr);
        }

        #endregion

        #region Encoding Functions

        /// <summary>
        /// Encodes text for use in URLs
        /// </summary>
        public static string ENCODE_URL_COMPONENT(string? text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : Uri.EscapeDataString(text);
        }

        /// <summary>
        /// Encodes text to Base64
        /// </summary>
        public static string BASE64_ENCODE(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decodes Base64 text
        /// </summary>
        public static string BASE64_DECODE(string? base64Text)
        {
            if (string.IsNullOrEmpty(base64Text)) return string.Empty;
            try
            {
                byte[] bytes = Convert.FromBase64String(base64Text);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
    }
}

