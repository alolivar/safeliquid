// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiquidDateFilters.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using SafeLiquid.Util;
    using System;
    using System.Globalization;

    /// <summary>
    /// The liquid date filters.
    /// </summary>
    public static class LiquidDateFilters
    {
        /// <summary>
        /// The utc date time.
        /// </summary>
        /// <param name="dateTimeString">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string UtcDateTime(string dateTimeString)
        {
            if (!DateTimeOffsetHelpers.TryParseAssumingUtc(dateTimeString, out var dateTimeValue))
            {
                throw new ArgumentException($"Cannot parse date time value '{dateTimeString}'");
            }

            return dateTimeValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        /// <summary>
        /// DotLiquid supports both Ruby and DotNet(Default) date format strings but not both at the same time. It can only be changed to Ruby by setting a global configuration.
        /// For backward compatibility, add the Ruby date filter for template author to explicitly opt in the Ruby date format.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RubyDate(object input, string format)
        {
            if (input == null)
            {
                return (string)null;
            }

            if (input is DateTime dateTime)
            {
                if (string.IsNullOrWhiteSpace(format))
                {
                    return dateTime.ToString(CultureInfo.InvariantCulture);
                }

                return new DateTimeOffset(dateTime).ToStrFTime(format);
            }

            if (string.IsNullOrWhiteSpace(format))
            {
                return input.ToString();
            }

            DateTimeOffset result;
            switch (input)
            {
                case DateTimeOffset dateTimeOffset:
                    result = dateTimeOffset;
                    break;
                case Decimal _:
                case double _:
                case float _:
                case int _:
                case uint _:
                case long _:
                case ulong _:
                case short _:
                case ushort _:
                    result = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddSeconds(Convert.ToDouble(input)).ToLocalTime();
                    break;
                default:
                    string str = input.ToString();
                    if (string.Equals(str, "now", StringComparison.OrdinalIgnoreCase) || string.Equals(str, "today", StringComparison.OrdinalIgnoreCase))
                    {
                        result = DateTimeOffset.Now;
                        break;
                    }

                    if (!DateTimeOffset.TryParse(str, out result))
                    {
                        return str;
                    }

                    break;
            }

            return result.ToStrFTime(format);
        }
    }
}
