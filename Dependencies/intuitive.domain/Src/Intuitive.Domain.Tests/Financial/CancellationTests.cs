namespace Intuitive.Domain.Tests.Financial
{
    using System;

    using Intuitive.Domain.Financial;

    using NUnit.Framework;

    /// <summary>
    /// The cancellation unit test.
    /// </summary>
    [TestFixture]
    public class CancellationTests
    {
        #region Constructor

        /// <summary>
        /// The cancellation test method start date end date amount round decimals.
        /// </summary>
        [Test]
        public void Cancellation_Should_RoundAmountTo2DP_When_Called()
        {
            // Arrange
            DateTime startDate = new DateTime(2016, 02, 17);
            DateTime endDate = DateTime.MaxValue;
            decimal amount = 31.21702m;
            int decimalPlaces = 2;

            // Act
            Cancellation cancellation = new Cancellation(startDate, endDate, amount, decimalPlaces);

            // Asssert
            Assert.AreEqual(startDate, cancellation.StartDate);
            Assert.AreEqual(endDate, cancellation.EndDate);
            Assert.AreEqual(decimal.Round(amount, decimalPlaces), cancellation.Amount);
        }

        #endregion

        #region Equals

        /// <summary>
        /// Equality test
        /// </summary>
        [Test]
        public void Equals_Should_ReturnTrue_When_DatesAndAmountMatch()
        {
            // Arrange
            Cancellation c1 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);
            Cancellation c2 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);

            // Act
            bool equal = c1.Equals(c2);

            // Assert
            Assert.IsTrue(equal);
        }

        /// <summary>
        /// Equality test
        /// </summary>
        [Test]
        public void Equals_Should_ReturnFalse_When_ArgumentIsNull()
        {
            // Arrange
            Cancellation c1 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);
            
            // Act
            bool equal = c1.Equals(null);

            // Assert
            Assert.IsFalse(equal);
        }

        /// <summary>
        /// Equality test
        /// </summary>
        [Test]
        public void Equals_Should_ReturnFalse_When_StartDateDoesntMatch()
        {
            // Arrange
            Cancellation c1 = new Cancellation(new DateTime(2016, 1, 2), new DateTime(2016, 1, 1), 10);
            Cancellation c2 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);

            // Act
            bool equal = c1.Equals(c2);

            // Assert
            Assert.IsFalse(equal);
        }

        /// <summary>
        /// Equality test
        /// </summary>
        [Test]
        public void Equals_Should_ReturnFalse_When_EndDateDoesntMatch()
        {
            // Arrange
            Cancellation c1 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 2), 10);
            Cancellation c2 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);

            // Act
            bool equal = c1.Equals(c2);

            // Assert
            Assert.IsFalse(equal);
        }

        /// <summary>
        /// Equality test
        /// </summary>
        [Test]
        public void Equals_Should_ReturnFalse_When_AmountDoesntMatch()
        {
            // Arrange
            Cancellation c1 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 20);
            Cancellation c2 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);

            // Act
            bool equal = c1.Equals(c2);

            // Assert
            Assert.IsFalse(equal);
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Get hash code test
        /// </summary>
        [Test]
        public void GetHashCode_Should_Match_When_DatesAndAmountMatch()
        {
            // Arrange
            Cancellation c1 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);
            Cancellation c2 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);

            // Act
            int hash1 = c1.GetHashCode();
            int hash2 = c2.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }

        /// <summary>
        /// Get hash code test
        /// </summary>
        [Test]
        public void GetHashCode_Should_NotMatch_When_StartDateDoesntMatch()
        {
            // Arrange
            Cancellation c1 = new Cancellation(new DateTime(2016, 1, 2), new DateTime(2016, 1, 1), 10);
            Cancellation c2 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);

            // Act
            int hash1 = c1.GetHashCode();
            int hash2 = c2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        /// <summary>
        /// Get hash code test
        /// </summary>
        [Test]
        public void GetHashCode_Should_NotMatch_When_EndDateDoesntMatch()
        {
            // Arrange
            Cancellation c1 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 2), 10);
            Cancellation c2 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);

            // Act
            int hash1 = c1.GetHashCode();
            int hash2 = c2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        /// <summary>
        /// Get hash code test
        /// </summary>
        [Test]
        public void GetHashCode_Should_NotMatch_When_AmountDoesntMatch()
        {
            // Arrange
            Cancellation c1 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 20);
            Cancellation c2 = new Cancellation(new DateTime(2016, 1, 1), new DateTime(2016, 1, 1), 10);

            // Act
            int hash1 = c1.GetHashCode();
            int hash2 = c2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        #endregion
    }
}