using System.Collections;

namespace webapi.Model
{
    /// <summary>
    /// Class <c>NumberData</c> represents the input number
    /// </summary>
    public class NumberData : IEnumerator<NumberData>
    {
        #region Fields

        private decimal number;                      // 0 - 999 999 999.99
        private Dictionary<decimal, int> digits;
        private decimal cursor;                      // 8 - (-2)
        private decimal powerOf10;                   // 100 000 000 - 0.01
        private int digit;                          // 0 - 9
        private bool isFirstIteration;
        private decimal defaultNumber;

        #endregion

        #region Enums

        public enum GroupPositionEnum
        {
            Units = 0, Tens = 1, Hundreds = 2
        }

        public enum GroupOfThousandEnum
        {
            Units = 0, Thousand = 3, Millions = 6
        }

        #endregion

        #region Constructor

        public NumberData(decimal number)
        {
            this.number = number;
            digits = new Dictionary<decimal, int>();
            cursor = GetLength(number);
            powerOf10 = PowerOf10();
            isFirstIteration = true;
            defaultNumber = number;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current digit value in the iteration
        /// </summary>
        public int Digit => digit;

        /// <summary>
        /// True if first iteration
        /// </summary>
        public bool IsFirstIteration => isFirstIteration;

        /// <summary>
        /// Current position in each group of thousand
        /// </summary>
        public GroupPositionEnum GroupPosition => (GroupPositionEnum)((cursor >= 0 ? cursor : 2 + cursor) % 3);

        /// <summary>
        /// Current group of thousand
        /// </summary>
        public GroupOfThousandEnum GroupOfThousand => (GroupOfThousandEnum)cursor;

        #endregion

        #region Inherited properties

        object IEnumerator.Current => this;

        NumberData IEnumerator<NumberData>.Current => this;

        #endregion

        #region Inherited methods

        bool IEnumerator.MoveNext()
        {
            isFirstIteration = digits.Count == 0;

            if (cursor < -2)
            {
                return false;
            }

            if (!isFirstIteration)
            {
                number = RemoveHighestDigit();
                cursor--;
                powerOf10 = PowerOf10();
            }

            digit = (int)Math.Floor((number / powerOf10));
            digits.Add(cursor, digit);

            return true;
        }

        void IEnumerator.Reset()
        {
            digits.Clear();
            isFirstIteration = true;
            number = defaultNumber;
        }

        public void Dispose()
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Current digit will be printed as a teen number in the next iteration
        /// </summary>
        /// <returns>True if the current digit will be printed as a teen number in the next iteration; otherwise, false.</returns>
        public bool IsTeenNumber()
        {
            return IsTeenNumber(cursor, digit);
        }

        /// <summary>
        /// Determines if current unit digit in the group of thousand is part of the teen number
        /// </summary>
        /// <param name="teenNumber">Provides the whole teen number if true, otherwise -1</param>
        /// <returns>True if current unit digit in the group of thousand is part of the teen number; otherwise, false.</returns>
        public bool IsTeenNumberCompleted(out int teenNumber)
        {
            int lastDigit;

            if (digits.TryGetValue(cursor + 1, out lastDigit) && IsTeenNumber(cursor + 1, lastDigit))
            {
                teenNumber = lastDigit * 10 + digit;
                return true;
            }
            else
            {
                teenNumber = -1;
                return false;
            }
        }

        /// <summary>
        /// Determines if the dash precedes current digit
        /// </summary>
        /// <returns>True if the dash precedes current digit; otherwise false</returns>
        public bool DoPrintDash()
        {
            if (GroupPosition != GroupPositionEnum.Units)
                return false;

            int lastDigit;
            digits.TryGetValue(cursor + 1, out lastDigit);

            return digits[cursor] > 0
                && lastDigit > 1
                && !isFirstIteration;
        }

        /// <summary>
        /// Determines if to print the group name e.g. million, dollars or cents
        /// </summary>
        /// <returns>True if the value of the group is greater than 0</returns>
        public bool DoPrintGroupName()
        {
            if (cursor == 0)
                return true;

            if (cursor % 3 != 0 && cursor != -2)
                return false;

            int lastDigit;
            int penultimateDigit;
            digits.TryGetValue(cursor + 1, out lastDigit);
            digits.TryGetValue(cursor + 1, out penultimateDigit);

            return digits[cursor] + lastDigit + penultimateDigit > 0;
        }

        /// <summary>
        /// Determines if the value of the number is 1 Dollar
        /// </summary>
        /// <returns>True if 1 Dollar; otherwise false</returns>
        public bool Is1Dollar()
        {
            return cursor == 0 && Math.Floor(defaultNumber) == 1;
        }

        /// <summary>
        /// Determines if the value of the number is 1 Cent
        /// </summary>
        /// <returns>True if 1 Cent; otherwise false</returns>
        public bool Is1Cent()
        {
            return cursor == -2 && Math.Round(defaultNumber - Math.Floor(defaultNumber), 2) == 0.01m;
        }

        /// <summary>
        /// Determines if there is a need to print "and" between dollars and cents
        /// </summary>
        /// <returns>True if there is a need to print "and"; otherwise false</returns>
        public bool DoPrintAnd()
        {
            return cursor == 0 && Math.Round(defaultNumber - Math.Floor(defaultNumber), 2) > 0;
        }

        /// <summary>
        /// Determines if the value of the number is 0 Dollars
        /// </summary>
        /// <returns>True if 0 Dollars; otherwise false</returns>
        public bool IsZeroDollars()
        {
            return cursor == 0 && Math.Floor(defaultNumber) == 0;
        }

        /// <summary>
        /// Determines if to print the space before current digit
        /// </summary>
        /// <returns>
        /// True if this is not the 1st iteration
        /// andthe value of the current digit is greater than 0
        /// and there is no need to print a dash
        /// and it is not a teen number
        /// </returns>
        public bool DoSpace()
        {
            try
            {
                return !IsFirstIteration
                        && digit > 0
                        && !DoPrintDash()
                        && (digits[cursor + 1] != 1
                        || GroupPosition != GroupPositionEnum.Units);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        #endregion

        #region Private methods

        private bool IsTeenNumber(decimal cursor, int digit)
        {
            return (cursor % 3 == 1 || cursor == -1) && digit == 1;
        }

        private decimal GetLength(decimal number)
        {
            return number == 0 ? 0 : Math.Floor((decimal)Math.Log10((double)number));
        }

        private decimal PowerOf10()
        {
            return (decimal)Math.Pow(10, (double)cursor);
        }

        private decimal RemoveHighestDigit()
        {
            return Math.Round(number - digit * powerOf10, 2);
        }

        #endregion
    }
}
