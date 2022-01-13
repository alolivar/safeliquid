namespace SafeLiquid.Tags
{
    using System;
    using System.Globalization;

    public class DateTimeOffsetHelpers
    {
        /// <summary>
        /// The parse utc.
        /// </summary>
        /// <param name="dateTimeString">
        /// The date time string.
        /// </param>
        /// <returns>
        /// The <see cref="DateTimeOffset"/>.
        /// </returns>
        // ReSharper disable once StyleCop.SA1650
        public static DateTimeOffset ParseAssumingUtc(string dateTimeString)
        {
            return DateTimeOffset.Parse(dateTimeString, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AssumeUniversal);
        }

        /// <summary>
        /// The try parse assuming utc.
        /// </summary>
        /// <param name="dateTimeString">
        /// The date time string.
        /// </param>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool TryParseAssumingUtc(string dateTimeString, out DateTimeOffset dateTime)
        {
            return DateTimeOffset.TryParse(dateTimeString, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AssumeUniversal, out dateTime);
        }

        /// <summary>
        /// The try parse assuming utc.
        /// </summary>
        /// <param name="dateTimeString">
        /// The date time string.
        /// </param>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool TryParseNullableAssumingUtc(string dateTimeString, out DateTimeOffset? dateTime)
        {
            var result = DateTimeOffset.TryParse(dateTimeString, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AssumeUniversal, out var value);
            dateTime = result ? value : (DateTimeOffset?)null;
            return result;
        }
    }
}