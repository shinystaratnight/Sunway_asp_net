namespace Intuitive.Domain.Tests.Financial
{
    using System;

    using Intuitive.Domain.Financial;
    using Intuitive.Domain.Financial.Enums;

    using NUnit.Framework;

    /// <summary>
    ///     The cancellations list unit test.
    /// </summary>
    [TestFixture]
    public class CancellationsListTest
    {
        #region Add

        /// <summary>
        ///     The add test method start date, end date, and amount.
        /// </summary>
        [Test]
        public void Add_Should_AddNewCancellationWithSpecifiedParameters_When_Called()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();
            DateTime startDate = new DateTime(2016, 02, 17);
            DateTime endDate = DateTime.MaxValue;
            decimal amount = 31.21702m;

            // Act
            cancellations.Add(startDate, endDate, amount);

            // Assert
            Assert.AreEqual(1, cancellations.Count);
            Assert.AreEqual(startDate, cancellations[0].StartDate);
            Assert.AreEqual(endDate, cancellations[0].EndDate);
            Assert.AreEqual(amount, cancellations[0].Amount);
        }

        /// <summary>
        ///     The add test method start date, end date, amount, and round decimals.
        /// </summary>
        [Test]
        public void Add_Should_RoundAmountTo2DP_When_Called()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();
            DateTime startDate = new DateTime(2016, 02, 17);
            DateTime endDate = DateTime.MaxValue;
            decimal amount = 31.21702m;
            int decimalPlaces = 2;

            // Act
            cancellations.Add(startDate, endDate, amount, decimalPlaces);

            // Assert
            Assert.AreEqual(1, cancellations.Count);
            Assert.AreEqual(startDate, cancellations[0].StartDate);
            Assert.AreEqual(endDate, cancellations[0].EndDate);
            Assert.AreEqual(decimal.Round(amount, decimalPlaces), cancellations[0].Amount);
        }

        /// <summary>
        ///     The add test method start date, empty end date, and amount.
        /// </summary>
        [Test]
        public void Add_Should_UseEmptyDate_When_EmptyDateUsed()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();
            DateTime startDate = new DateTime(2016, 02, 16);
            DateTime endDate = CancellationsList<Cancellation>.EmptyDateTime;
            decimal amount = 31.21702m;

            // Act
            cancellations.Add(startDate, endDate, amount);

            // Assert
            Assert.AreEqual(1, cancellations.Count);
            Assert.AreEqual(startDate, cancellations[0].StartDate);
            Assert.AreEqual(endDate, cancellations[0].EndDate);
            Assert.AreEqual(amount, cancellations[0].Amount);
        }

        #endregion

        #region FindAll

        /// <summary>
        ///     Find all test method null.
        /// </summary>
        [Test]
        public void FindAll_Should_ThrowArgumentNullException_When_MatchArgumentIsNull()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => cancellations.FindAll(null));
        }

        /// <summary>
        ///     The find all test method end date not null and greater than or equal to.
        /// </summary>
        [Test]
        public void FindAll_Should_FindCancellationsWithEndDateGreaterThanOrEqualToSpecifiedDate_When_Called()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 01, 15), new DateTime(2017, 01, 14), 137);
            cancellations.Add(new DateTime(2016, 02, 16), new DateTime(2017, 02, 15), 112);
            cancellations.Add(new DateTime(2016, 02, 17), CancellationsList<Cancellation>.EmptyDateTime, 138);
            cancellations.Add(new DateTime(2016, 03, 17), DateTime.MaxValue, 113);

            // Act
            CancellationsList<Cancellation> foundCancellations =
                cancellations.FindAll(cancellation => cancellation.EndDate >= new DateTime(2017, 02, 14));

            // Assert
            Assert.IsNotNull(foundCancellations);
            Assert.AreEqual(4, cancellations.Count);
            Assert.AreEqual(2, foundCancellations.Count);
            Assert.AreEqual(new DateTime(2016, 02, 16), foundCancellations[0].StartDate);
            Assert.AreEqual(new DateTime(2017, 02, 15), foundCancellations[0].EndDate);
            Assert.AreEqual(112, foundCancellations[0].Amount);
            Assert.AreEqual(new DateTime(2016, 03, 17), foundCancellations[1].StartDate);
            Assert.AreEqual(DateTime.MaxValue, foundCancellations[1].EndDate);
            Assert.AreEqual(113, foundCancellations[1].Amount);
        }

        /// <summary>
        ///     The find all test method start date less than or equal to.
        /// </summary>
        [Test]
        public void FindAll_Should_FindCancellationsWithStartDateLessThanOrEqualToSpecifiedDate_When_Called()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 02, 16), DateTime.MaxValue, 112);
            cancellations.Add(new DateTime(2016, 03, 17), DateTime.MaxValue, 113);

            // Act
            CancellationsList<Cancellation> foundCancellations =
                cancellations.FindAll(cancellation => cancellation.StartDate <= new DateTime(2016, 02, 17));

            // Assert
            Assert.IsNotNull(foundCancellations);
            Assert.AreEqual(2, cancellations.Count);
            Assert.AreEqual(1, foundCancellations.Count);
            Assert.AreEqual(new DateTime(2016, 02, 16), foundCancellations[0].StartDate);
            Assert.AreEqual(DateTime.MaxValue, foundCancellations[0].EndDate);
            Assert.AreEqual(112, foundCancellations[0].Amount);
        }

        #endregion

        #region MergeMultiple

        /// <summary>
        ///     The merge multiple test method.
        /// </summary>
        [Test]
        public void MergeMultiple_Should_ReturnEmptyList_When_InputsAreEmpty()
        {
            // Arrange
            CancellationsList<Cancellation> cancellationsA = new CancellationsList<Cancellation>(),
                                            cancellationsB = new CancellationsList<Cancellation>();

            // Act
            CancellationsList<Cancellation> mergedCancellations =
                CancellationsList<Cancellation>.MergeMultiple(cancellationsA, cancellationsB);

            // Assert
            Assert.AreEqual(0, mergedCancellations.Count);
        }

        /// <summary>
        ///     The merge multiple test method.
        /// </summary>
        [Test]
        public void MergeMultiple_Should_ReturnFirstList_When_SecondListIsEmpty()
        {
            // Arrange
            CancellationsList<Cancellation> cancellationsA = new CancellationsList<Cancellation>(),
                                            cancellationsB = new CancellationsList<Cancellation>();

            cancellationsA.Add(new DateTime(2016, 1, 1), new DateTime(2016, 1, 10), 10);
            cancellationsA.Add(new DateTime(2016, 1, 11), new DateTime(2016, 1, 20), 20);

            // Act
            CancellationsList<Cancellation> mergedCancellations =
                CancellationsList<Cancellation>.MergeMultiple(cancellationsA, cancellationsB);

            // Assert
            CollectionAssert.AreEqual(mergedCancellations, cancellationsA);
        }

        /// <summary>
        ///     The merge multiple test method.
        /// </summary>
        [Test]
        public void MergeMultiple_Should_ReturnSecondList_When_FirstListIsEmpty()
        {
            // Arrange
            CancellationsList<Cancellation> cancellationsA = new CancellationsList<Cancellation>(),
                                            cancellationsB = new CancellationsList<Cancellation>();

            cancellationsB.Add(new DateTime(2016, 1, 1), new DateTime(2016, 1, 10), 10);
            cancellationsB.Add(new DateTime(2016, 1, 11), new DateTime(2016, 1, 20), 20);

            // Act
            CancellationsList<Cancellation> mergedCancellations =
                CancellationsList<Cancellation>.MergeMultiple(cancellationsA, cancellationsB);

            // Assert
            CollectionAssert.AreEqual(mergedCancellations, cancellationsB);
        }

        /// <summary>
        ///     The merge multiple test method.
        /// </summary>
        [Test]
        public void MergeMultiple_Should_RemoveCancellations_When_StartDateIsAfterEarliestEndDate()
        {
            // Arrange
            CancellationsList<Cancellation> cancellationsA = new CancellationsList<Cancellation>(),
                                            cancellationsB = new CancellationsList<Cancellation>();

            cancellationsA.Add(new DateTime(2016, 1, 1), new DateTime(2016, 1, 10), 10);
            cancellationsB.Add(new DateTime(2016, 1, 1), new DateTime(2016, 1, 10), 10);
            cancellationsB.Add(new DateTime(2016, 1, 11), new DateTime(2016, 1, 20), 20);

            // Act
            CancellationsList<Cancellation> mergedCancellations =
                CancellationsList<Cancellation>.MergeMultiple(cancellationsA, cancellationsB);

            // Assert
            Assert.AreEqual(1, mergedCancellations.Count);
            Assert.AreEqual(cancellationsA[0].StartDate, mergedCancellations[0].StartDate);
            Assert.AreEqual(cancellationsA[0].EndDate, mergedCancellations[0].EndDate);
            Assert.AreEqual(cancellationsA[0].Amount + cancellationsB[0].Amount, mergedCancellations[0].Amount);
        }

        /// <summary>
        ///     The merge multiple test method.
        /// </summary>
        [Test]
        public void MergeMultiple_Should_ReturnDistinctDatebandsWithSummedAmounts_When_DateBandsOverlap()
        {
            // Arrange
            CancellationsList<Cancellation> cancellationsA = new CancellationsList<Cancellation>(),
                                            cancellationsB = new CancellationsList<Cancellation>();

            cancellationsA.Add(new DateTime(2016, 1, 5), new DateTime(2016, 1, 15), 20);
            cancellationsB.Add(new DateTime(2016, 1, 1), new DateTime(2016, 1, 10), 10);
            cancellationsB.Add(new DateTime(2016, 1, 11), new DateTime(2016, 1, 20), 30);

            // Act
            CancellationsList<Cancellation> mergedCancellations =
                CancellationsList<Cancellation>.MergeMultiple(cancellationsA, cancellationsB);

            // Assert
            Assert.AreEqual(3, mergedCancellations.Count);
            Assert.AreEqual(cancellationsB[0].StartDate, mergedCancellations[0].StartDate);
            Assert.AreEqual(cancellationsA[0].StartDate.AddDays(-1), mergedCancellations[0].EndDate);
            Assert.AreEqual(cancellationsB[0].Amount, mergedCancellations[0].Amount);
            Assert.AreEqual(cancellationsA[0].StartDate, mergedCancellations[1].StartDate);
            Assert.AreEqual(cancellationsB[0].EndDate, mergedCancellations[1].EndDate);
            Assert.AreEqual(cancellationsA[0].Amount + cancellationsB[0].Amount, mergedCancellations[1].Amount);
            Assert.AreEqual(cancellationsB[1].StartDate, mergedCancellations[2].StartDate);
            Assert.AreEqual(cancellationsB[1].EndDate, mergedCancellations[2].EndDate);
            Assert.AreEqual(cancellationsA[0].Amount + cancellationsB[1].Amount, mergedCancellations[2].Amount);
        }

        #endregion

        #region Solidify

        /// <summary>
        ///     The solidify test method minimum.
        /// </summary>
        [Test]
        public void Solidify_Should_IncludeAllStartDates_When_Called()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 01, 15), new DateTime(2017, 01, 14), 137);
            cancellations.Add(new DateTime(2016, 02, 16), new DateTime(2017, 02, 15), 112);
            cancellations.Add(new DateTime(2016, 02, 16), new DateTime(2017, 03, 15), 138);
            cancellations.Add(new DateTime(2016, 03, 17), DateTime.MaxValue, 113);

            // Act
            cancellations.Solidify(CancellationSolidifyType.Sum);

            // Assert
            Assert.AreEqual(3, cancellations.Count);
            Assert.AreEqual(new DateTime(2016, 01, 15), cancellations[0].StartDate);
            Assert.AreEqual(new DateTime(2016, 02, 16), cancellations[1].StartDate);
            Assert.AreEqual(new DateTime(2016, 03, 17), cancellations[2].StartDate);
        }

        /// <summary>
        ///     The solidify test method minimum.
        /// </summary>
        [Test]
        public void Solidify_Should_CreateConsecutiveDateBands_When_Called()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 01, 01), new DateTime(2016, 01, 14), 10);
            cancellations.Add(new DateTime(2016, 01, 14), new DateTime(2016, 02, 15), 20);
            cancellations.Add(new DateTime(2016, 02, 10), new DateTime(2016, 02, 25), 30);
            cancellations.Add(new DateTime(2016, 03, 01), new DateTime(2016, 03, 15), 40);

            // Act
            cancellations.Solidify(CancellationSolidifyType.Sum);

            // Assert
            Assert.AreEqual(4, cancellations.Count);
            Assert.AreEqual(cancellations[0].EndDate, cancellations[1].StartDate.AddDays(-1));
            Assert.AreEqual(cancellations[1].EndDate, cancellations[2].StartDate.AddDays(-1));
            Assert.AreEqual(cancellations[2].EndDate, cancellations[3].StartDate.AddDays(-1));
        }

        /// <summary>
        ///     The solidify test method minimum.
        /// </summary>
        [Test]
        public void Solidify_Should_SetFinalEndDate_When_SolidifyToIsSpecified()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 01, 01), new DateTime(2016, 01, 14), 10);
            cancellations.Add(new DateTime(2016, 01, 15), new DateTime(2016, 02, 15), 20);
            DateTime solidifyTo = new DateTime(2016, 02, 28);

            // Act
            cancellations.Solidify(CancellationSolidifyType.Min, solidifyTo);

            // Assert
            Assert.AreEqual(2, cancellations.Count);
            Assert.AreEqual(solidifyTo, cancellations[1].EndDate);
        }

        /// <summary>
        ///     The solidify test method minimum.
        /// </summary>
        [Test]
        public void Solidify_Should_AddCancellationForFinalCost_When_FinalCostIsDifferentToFinalCancellationAmount()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 01, 01), new DateTime(2016, 01, 14), 10);
            cancellations.Add(new DateTime(2016, 01, 15), new DateTime(2016, 02, 15), 20);
            DateTime solidifyTo = new DateTime(2016, 02, 28);
            decimal finalCost = 30;

            // Act
            cancellations.Solidify(CancellationSolidifyType.Min, solidifyTo, finalCost);

            // Assert
            Assert.AreEqual(3, cancellations.Count);
            Assert.AreEqual(solidifyTo, cancellations[2].EndDate);
            Assert.AreEqual(finalCost, cancellations[2].Amount);
        }

        /// <summary>
        ///     The solidify test method latest start date.
        /// </summary>
        [Test]
        public void Solidify_Should_ValueFromLastestStartDateOfOverlappingCancellations_When_Used()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 01, 15), new DateTime(2017, 01, 14), 137);
            cancellations.Add(new DateTime(2016, 02, 16), new DateTime(2017, 02, 15), 112);
            cancellations.Add(new DateTime(2016, 02, 16), new DateTime(2016, 02, 16), 124);
            cancellations.Add(new DateTime(2016, 03, 17), DateTime.MaxValue, 113);

            // Act
            cancellations.Solidify(CancellationSolidifyType.LatestStartDate);

            // Assert
            Assert.AreEqual(3, cancellations.Count);
            Assert.AreEqual(new DateTime(2016, 01, 15), cancellations[0].StartDate);
            Assert.AreEqual(new DateTime(2016, 02, 15), cancellations[0].EndDate);
            Assert.AreEqual(137, cancellations[0].Amount);
            Assert.AreEqual(new DateTime(2016, 02, 16), cancellations[1].StartDate);
            Assert.AreEqual(new DateTime(2016, 03, 16), cancellations[1].EndDate);
            Assert.AreEqual(124, cancellations[1].Amount);
            Assert.AreEqual(new DateTime(2016, 03, 17), cancellations[2].StartDate);
            Assert.AreEqual(DateTime.MaxValue, cancellations[2].EndDate);
            Assert.AreEqual(113, cancellations[2].Amount);
        }

        /// <summary>
        ///     The solidify test method maximum.
        /// </summary>
        [Test]
        public void Solidify_Should_TakeMaximumValueOfOverlappingCancellations_When_Used()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 01, 15), new DateTime(2017, 01, 14), 112);
            cancellations.Add(new DateTime(2016, 02, 16), new DateTime(2017, 02, 15), 137);
            cancellations.Add(new DateTime(2016, 03, 17), DateTime.MaxValue, 113);

            // Act
            cancellations.Solidify(CancellationSolidifyType.Max);

            // Assert
            Assert.AreEqual(2, cancellations.Count);
            Assert.AreEqual(new DateTime(2016, 01, 15), cancellations[0].StartDate);
            Assert.AreEqual(new DateTime(2016, 02, 15), cancellations[0].EndDate);
            Assert.AreEqual(112, cancellations[0].Amount);
            Assert.AreEqual(new DateTime(2016, 02, 16), cancellations[1].StartDate);
            Assert.AreEqual(DateTime.MaxValue, cancellations[1].EndDate);
            Assert.AreEqual(137, cancellations[1].Amount);
        }

        /// <summary>
        ///     The solidify test method minimum.
        /// </summary>
        [Test]
        public void Solidify_Should_TakeMinimumValueOfOverlappingCancellations_When_Used()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 01, 15), new DateTime(2017, 01, 14), 137);
            cancellations.Add(new DateTime(2016, 02, 16), new DateTime(2017, 02, 15), 112);
            cancellations.Add(new DateTime(2016, 03, 17), DateTime.MaxValue, 113);

            // Act
            cancellations.Solidify(CancellationSolidifyType.Min);

            // Assert
            Assert.AreEqual(2, cancellations.Count);
            Assert.AreEqual(new DateTime(2016, 01, 15), cancellations[0].StartDate);
            Assert.AreEqual(new DateTime(2016, 02, 15), cancellations[0].EndDate);
            Assert.AreEqual(137, cancellations[0].Amount);
            Assert.AreEqual(new DateTime(2016, 02, 16), cancellations[1].StartDate);
            Assert.AreEqual(DateTime.MaxValue, cancellations[1].EndDate);
            Assert.AreEqual(112, cancellations[1].Amount);
        }

        /// <summary>
        ///     The solidify test method sum.
        /// </summary>
        [Test]
        public void Solidify_Should_SumValuesOfOverlappingCancellations_When_Used()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 01, 15), new DateTime(2017, 01, 14), 137);
            cancellations.Add(new DateTime(2016, 02, 16), new DateTime(2017, 02, 15), 112);
            cancellations.Add(new DateTime(2016, 03, 17), DateTime.MaxValue, 113);

            // Act
            cancellations.Solidify(CancellationSolidifyType.Sum);

            // Assert
            Assert.AreEqual(3, cancellations.Count);
            Assert.AreEqual(new DateTime(2016, 01, 15), cancellations[0].StartDate);
            Assert.AreEqual(new DateTime(2016, 02, 15), cancellations[0].EndDate);
            Assert.AreEqual(137, cancellations[0].Amount);
            Assert.AreEqual(new DateTime(2016, 02, 16), cancellations[1].StartDate);
            Assert.AreEqual(new DateTime(2016, 03, 16), cancellations[1].EndDate);
            Assert.AreEqual(249, cancellations[1].Amount);
            Assert.AreEqual(new DateTime(2016, 03, 17), cancellations[2].StartDate);
            Assert.AreEqual(DateTime.MaxValue, cancellations[2].EndDate);
            Assert.AreEqual(362, cancellations[2].Amount);
        }

        #endregion

        #region TotalAmount

        /// <summary>
        ///     The total amount test method empty.
        /// </summary>
        [Test]
        public void TotalAmount_Should_BeZero_When_ListIsEmpty()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            // Act
            decimal total = cancellations.TotalAmount;

            // Assert
            Assert.AreEqual(0, total);
        }

        /// <summary>
        ///     The total amount test method.
        /// </summary>
        [Test]
        public void TotalAmount_Should_BeSumOfAmounts_When_Called()
        {
            // Arrange
            CancellationsList<Cancellation> cancellations = new CancellationsList<Cancellation>();

            cancellations.Add(new DateTime(2016, 01, 15), new DateTime(2017, 01, 14), 137);
            cancellations.Add(new DateTime(2016, 02, 16), new DateTime(2017, 02, 15), 112);
            cancellations.Add(new DateTime(2016, 02, 17), CancellationsList<Cancellation>.EmptyDateTime, 138);
            cancellations.Add(new DateTime(2016, 03, 17), DateTime.MaxValue, 113);

            // Act
            decimal total = cancellations.TotalAmount;

            // Assert
            Assert.AreEqual(500, total);
        }

        #endregion
    }
}