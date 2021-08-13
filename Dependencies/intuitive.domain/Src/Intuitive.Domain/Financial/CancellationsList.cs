//-----------------------------------------------------------------------
// <copyright file="CancellationsList.cs" company="Sunway travel">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------

namespace Intuitive.Domain.Financial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Intuitive.Domain.Financial.Enums;
    using Intuitive.Domain.Financial.Interfaces;

    /// <summary>
    /// Cancellations list.
    /// </summary>
    /// <typeparam name="T">
    /// Cancellation type.
    /// </typeparam>
    /// <seealso cref="System.Collections.Generic.List{T}"/>
    public class CancellationsList<T> : List<T>, ICancellationsList<T>
        where T : class, ICancellation, new()
    {
        /// <summary>
        ///     The empty date and time.
        /// </summary>
        public static readonly DateTime EmptyDateTime = new DateTime(1900, 1, 1, 0, 0, 0, 1);

        /// <summary>
        ///     Gets the total cancellations amount.
        /// </summary>
        /// <value>
        ///     The total cancellations amount.
        /// </value>
        public decimal TotalAmount
        {
            get
            {
                return this.Sum(cancellation => cancellation.Amount);
            }
        }

        /// <summary>
        /// Merges multiple cancellation lists.
        /// </summary>
        /// <param name="cancellations">
        /// The cancellations.
        /// </param>
        /// <returns>
        /// The <see cref="T:CancellationsList"/>.
        /// </returns>
        public static CancellationsList<T> MergeMultiple(params CancellationsList<T>[] cancellations)
        {
            CancellationsList<T> mergedCancellations = new CancellationsList<T>();

            // Find the earliest end date of all the cancellations, and add all the cancellations into our final merged one.
            DateTime earliestEndDate = EmptyDateTime;

            foreach (CancellationsList<T> innerCancellations in cancellations)
            {
                // Find end date.
                DateTime endDateForThisCancellation = EmptyDateTime;

                if (innerCancellations.Count > 0)
                {
                    endDateForThisCancellation = innerCancellations.Max(x => x.EndDate);

                    if (earliestEndDate == EmptyDateTime || endDateForThisCancellation < earliestEndDate)
                    {
                        earliestEndDate = endDateForThisCancellation;
                    }

                    // Add the cancellations.
                    mergedCancellations.AddRange(innerCancellations);
                }
            }

            // Get rid of any rules that start too late.
            mergedCancellations.RemoveAll(cancellation => cancellation.StartDate > earliestEndDate);

            // Chop the end off of any rules that end too late.
            foreach (T cancellation in
                mergedCancellations.Where(cancellation => cancellation.EndDate > earliestEndDate))
            {
                cancellation.EndDate = earliestEndDate;
            }

            // Now solidify the rules (sum them all up).
            mergedCancellations.Solidify(CancellationSolidifyType.Sum);

            // Return our new merged cancellations.
            return mergedCancellations;
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <param name="match">
        /// The match.
        /// </param>
        /// <returns>
        /// Cancellations list.
        /// </returns>
        public new CancellationsList<T> FindAll(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match", "Cancellations list find all match.");
            }

            CancellationsList<T> cancellations = new CancellationsList<T>();

            for (int i = 0; i < this.Count; i++)
            {
                if (match(this[i]))
                {
                    cancellations.Add(this[i]);
                }
            }

            return cancellations;
        }

        /// <summary>
        /// Adds the specified start date, end date, and rounded amount as a cancellation..
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="roundDecimals">
        /// The round decimals.
        /// </param>
        public void Add(DateTime startDate, DateTime endDate, decimal amount, int roundDecimals)
        {
            T cancellation = new T
            {
                StartDate = startDate,
                EndDate = endDate,
                Amount = decimal.Round(amount, roundDecimals)
            };

            this.Add(cancellation);
        }

        /// <summary>
        /// Adds the specified start date, end date, and amount as a cancellation.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        public void Add(DateTime startDate, DateTime endDate, decimal amount)
        {
            T cancellation = new T { StartDate = startDate, EndDate = endDate, Amount = amount };

            this.Add(cancellation);
        }

        /// <summary>
        /// Solidifies the cancellations.
        /// </summary>
        /// <param name="solidifyType">
        /// Type of the solidify.
        /// </param>
        /// <param name="solidifyTo">
        /// f
        ///     The solidify to.
        /// </param>
        /// <param name="finalCost">
        /// The final cost.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Incorrect solidify type.
        /// </exception>
        public void Solidify(CancellationSolidifyType solidifyType, DateTime? solidifyTo = null, decimal finalCost = 0)
        {
            if (!solidifyTo.HasValue)
            {
                solidifyTo = EmptyDateTime;
            }

            CancellationsList<T> finalCancellations = new CancellationsList<T>();

            // First of all, we need to get all the start dates in order.
            List<DateTime> startDates = this.Select(x => x.StartDate).Distinct().ToList();

            startDates.Sort();

            // Loop through the dates.
            decimal lastAmount = 0;

            foreach (DateTime startDate in startDates)
            {
                // Get all the rules that pass over this date.
                DateTime compareTo = startDate.Date;
                CancellationsList<T> cancellationsInEffect =
                    this.FindAll(x => x.StartDate.Date <= compareTo && x.EndDate.Date >= compareTo);

                // All we need now is to choose the correct amount to apply at this date band point.
                T cancellationToApply = new T { StartDate = startDate, EndDate = EmptyDateTime, Amount = 0 };

                switch (solidifyType)
                {
                    case CancellationSolidifyType.Min:

                        if (cancellationsInEffect.Count > 0)
                        {
                            cancellationToApply.Amount = cancellationsInEffect.Min(x => x.Amount);
                        }

                        break;

                    case CancellationSolidifyType.Max:

                        if (cancellationsInEffect.Count > 0)
                        {
                            cancellationToApply.Amount = cancellationsInEffect.Max(x => x.Amount);
                        }

                        break;

                    case CancellationSolidifyType.Sum:

                        cancellationToApply.Amount = cancellationsInEffect.Sum(x => x.Amount);

                        break;

                    case CancellationSolidifyType.LatestStartDate:
                        if (cancellationsInEffect.Count > 0)
                        {
                            cancellationToApply.Amount = cancellationsInEffect[0].Amount;
                        }

                        DateTime latestStartDateSofar = EmptyDateTime;
                        foreach (T cancellation in cancellationsInEffect)
                        {
                            if (cancellation.StartDate > latestStartDateSofar
                                || (cancellation.StartDate == latestStartDateSofar
                                    && cancellation.Amount > cancellationToApply.Amount))
                            {
                                latestStartDateSofar = cancellation.StartDate;
                                cancellationToApply.Amount = cancellation.Amount;
                            }
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException(
                            "solidifyType",
                            solidifyType,
                            "Incorrect solidify type in solidify cancellations list.");
                }

                // If the amount has actually changed, then add it in to the final policy.
                if (cancellationToApply.Amount != lastAmount)
                {
                    finalCancellations.Add(cancellationToApply);
                }

                // Store this amount to compare with next time round.
                lastAmount = cancellationToApply.Amount;
            }

            // Work out all the end dates except the last one.
            DateTime lastEndDateFromOriginalCancellation = EmptyDateTime;

            if (this.Count > 0)
            {
                lastEndDateFromOriginalCancellation = this.Max(x => x.EndDate);
            }

            if (finalCancellations.Count > 1)
            {
                for (int i = 0; i < finalCancellations.Count - 1; i++)
                {
                    finalCancellations[i].EndDate = finalCancellations[i + 1].StartDate.AddDays(-1);
                }
            }

            // Do the last end date, and add a final date band if necessary.
            if (finalCancellations.Count > 0)
            {
                if (solidifyTo != EmptyDateTime && lastEndDateFromOriginalCancellation < solidifyTo.Value
                    && (finalCost == 0 || finalCost == lastAmount))
                {
                    finalCancellations[finalCancellations.Count - 1].EndDate = solidifyTo.Value;
                }
                else
                {
                    finalCancellations[finalCancellations.Count - 1].EndDate = lastEndDateFromOriginalCancellation;

                    if (solidifyTo != EmptyDateTime && lastEndDateFromOriginalCancellation < solidifyTo.Value)
                    {
                        finalCancellations.Add(
                            lastEndDateFromOriginalCancellation.AddDays(1),
                            solidifyTo.Value,
                            finalCost);
                    }
                }
            }

            // Add the final cancellations.
            this.Clear();
            this.AddRange(finalCancellations);
        }
    }
}