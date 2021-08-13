namespace Intuitive.Domain.Tests.Legacy
{
    using System.Collections.Generic;
    using Intuitive.Domain.Legacy;
    using NUnit.Framework;
    
    /// <summary>
    /// Unit tests for the booking data class
    /// </summary>
    [TestFixture]
    public class BookingDataTests
    {
        #region Validate

        /// <summary>
        /// Validate test
        /// </summary>
        [Test]
        public void Validate_Should_Fail_When_BookingSourceNotSet()
        {
            // Arrange
            BookingData data = GetBookingData(bookingSourceId: 0);

            // Act
            List<string> warnings = data.Validate();

            // Assert
            Assert.AreEqual(warnings.Count, 1);
            Assert.AreEqual(warnings[0], "The Booking Source Id must be set.");
        }

        /// <summary>
        /// Validate test
        /// </summary>
        [Test]
        public void Validate_Should_Fail_When_CustomerCurrencyNotSet()
        {
            // Arrange
            BookingData data = GetBookingData(customerCurrencyId: 0);

            // Act
            List<string> warnings = data.Validate();

            // Assert
            Assert.AreEqual(warnings.Count, 1);
            Assert.AreEqual(warnings[0], "The Customer Currency Id must be set.");
        }

        /// <summary>
        /// Validate test
        /// </summary>
        [Test]
        public void Validate_Should_Fail_When_SalesChannelNotSet()
        {
            // Arrange
            BookingData data = GetBookingData(salesChannelId: 0);

            // Act
            List<string> warnings = data.Validate();

            // Assert
            Assert.AreEqual(warnings.Count, 1);
            Assert.AreEqual(warnings[0], "The Sales Channel Id must be set.");
        }

        /// <summary>
        /// Validate test
        /// </summary>
        [Test]
        public void Validate_Should_Fail_When_SellingCountryNotSet()
        {
            // Arrange
            BookingData data = GetBookingData(sellingCountryId: 0);

            // Act
            List<string> warnings = data.Validate();

            // Assert
            Assert.AreEqual(warnings.Count, 1);
            Assert.AreEqual(warnings[0], "The Selling Country Id must be set.");
        }

        /// <summary>
        /// Validate test
        /// </summary>
        [Test]
        public void Validate_Should_Succeed_When_InputIsValid()
        {
            // Arrange
            BookingData data = GetBookingData();

            // Act
            List<string> warnings = data.Validate();

            // Assert
            Assert.AreEqual(warnings.Count, 0);
        }

        #endregion

        #region Setup

        /// <summary>
        /// Setup input data
        /// </summary>
        /// <param name="bookingSourceId">The booking source id</param>
        /// <param name="customerCurrencyId">The customer currency id</param>
        /// <param name="salesChannelId">the sales channel id</param>
        /// <param name="sellingCountryId">The selling country id</param>
        /// <returns>The booking data</returns>
        private static BookingData GetBookingData(int bookingSourceId = 1, int customerCurrencyId = 1, int salesChannelId = 1, int sellingCountryId = 1)
        {
            return new BookingData()
            {
                BookingSourceId = bookingSourceId,
                CustomerCurrencyId = customerCurrencyId,
                SalesChannelId = salesChannelId,
                SellingCountryId = sellingCountryId
            };
        }

        #endregion
    }
}