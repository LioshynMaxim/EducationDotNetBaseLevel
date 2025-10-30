using System;
using System.Linq;

namespace Task2
{
    public class NumberParser : INumberParser
    {
        public int Parse(string stringValue)
        {
            ValidateString(stringValue);
            
            var trimmed = stringValue.TrimEnd();
            var isNegative = trimmed[0] == '-';
            var index = isNegative || trimmed[0] == '+' ? 1 : 0;
            index += trimmed.Skip(index).TakeWhile(c => c == '0').Count();

            if (!trimmed.Skip(index).All(char.IsDigit))
            {
                throw new FormatException();
            }

            long result = 0;
            for (int i = index; i < trimmed.Length; i++)
            {
                int digit = trimmed[i] - '0';
                result = result * 10 + digit;
            }

            return isNegative
                ? result > int.MaxValue + 1L
                    ? throw new OverflowException()
                    : -(int)result
                : result > int.MaxValue
                    ? throw new OverflowException()
                    : (int)result;
        }

        private void ValidateString(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (string.IsNullOrEmpty(str.TrimEnd()))
            {
                throw new FormatException("Input string was not in a correct format.");
            }
        }
    }
}