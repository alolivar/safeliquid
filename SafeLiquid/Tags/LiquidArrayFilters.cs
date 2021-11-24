// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiquidArrayFilters.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System.Collections;
    using System.Linq;

    using SafeLiquid;

    /// <summary>
    /// The liquid array filters.
    /// </summary>
    public static class LiquidArrayFilters
    {
        /// <summary>
        /// The batch.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// <see cref="IEnumerable"/>
        /// </returns>
        public static IEnumerable Batch(IEnumerable source, int size)
        {
            object[] bucket = null;
            var count = 0;
            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new object[size];
                bucket[count++] = item;
                if (count != size)
                    continue;
                yield return bucket;
                bucket = null;
                count = 0;
            }
            if (bucket != null && count > 0)
                yield return bucket.Take(count).ToArray();
        }
        /// <summary>
        /// The group by.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// <see cref="IEnumerable"/>
        /// </returns>
        public static IEnumerable GroupBy(IEnumerable enumerable, string propertyName)
        {
            return enumerable
                        .Cast<object>()
                        .GroupBy(x => x is Hash h ? h[propertyName] : x.GetType().GetProperty(propertyName).GetValue(x, null))
                        .Select(g => new { key = g.Key, items = g.ToList() })
                        .ToList();
        }

        /// <summary>
        /// The range.
        /// </summary>
        /// <param name="startValue">
        /// The start value.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static string CommaDelimitedRange(int startValue, int count)
        {
            return string.Join(", ", Enumerable.Range(startValue, count));
        }
    }
}