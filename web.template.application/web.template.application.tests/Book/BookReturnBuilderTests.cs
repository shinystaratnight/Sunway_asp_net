namespace Web.Template.Application.Tests.Book
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Property;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Book.Builders;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Tests for the Book Return Builder
    /// </summary>
    [TestFixture]
    public class BookReturnBuilderTests
    {
        /// <summary>
        /// Sets up fake response.
        /// </summary>
        /// <returns>A Book response</returns>
        private iVectorConnectInterface.Basket.BookResponse SetUpFakeResponse()
        {
            var response = new ivci.Basket.BookResponse()
                               {
                                   PropertyBookings = new List<BookResponse>() { new BookResponse() { ReturnStatus = new ivci.ReturnStatus() { Success = false } } }, 
                                   FlightBookings =
                                       new List<iVectorConnectInterface.Flight.BookResponse>()
                                           {
                                               new ivci.Flight.BookResponse() { ReturnStatus = new ivci.ReturnStatus() { Success = false } }, 
                                               new ivci.Flight.BookResponse() { ReturnStatus = new ivci.ReturnStatus() { Success = false } }, 
                                           }, 
                                   TransferBookings = new List<iVectorConnectInterface.Transfer.BookResponse>() { new ivci.Transfer.BookResponse() { ReturnStatus = new ivci.ReturnStatus() { Success = false } } },
                                   ReturnStatus = new ivci.ReturnStatus() { Success = false }
                               };

            return response;
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Build_Should_ReturnABookReturn_When_Called()
        {
            ////Arrange
            var bookReturnMock = new Mock<IBookReturn>();
            var bookReturnBuilder = new BookReturnBuilder(bookReturnMock.Object);

            ////Act
            var bookReturn = bookReturnBuilder.Build();

            ////Assert 
            Assert.IsNotNull(bookReturn);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Build_Should_ReturnABookReturnWithABasket_When_CalledWithHavingSetOne()
        {
            ////Arrange
            var bookReturnMock = new Mock<IBookReturn>();
            bookReturnMock.SetupAllProperties();
            var bookReturnBuilder = new BookReturnBuilder(bookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            bookReturnBuilder.SetBasket(new Mock<IBasket>().Object);
            var bookReturn = bookReturnBuilder.Build();

            ////Assert 
            Assert.IsNotNull(bookReturn.Basket);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Build_Should_ReturnABookReturnWithNoBakset_When_CalledWithoutHavingSetOne()
        {
            ////Arrange
            var bookReturnMock = new Mock<IBookReturn>();
            bookReturnMock.SetupAllProperties();
            var bookReturnBuilder = new BookReturnBuilder(bookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            var bookReturn = bookReturnBuilder.Build();

            ////Assert 
            Assert.IsNull(bookReturn.Basket);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Build_Should_ReturnSetTheSuccessToFalse_When_UnsuccessfulComponentBooksReturn()
        {
            ////Arrange
            var bookReturnMock = new Mock<IBookReturn>();
            bookReturnMock.SetupAllProperties();
            var bookReturnBuilder = new BookReturnBuilder(bookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            bookReturnBuilder.AddResponse(response);
            var bookReturn = bookReturnBuilder.Build();

            ////Assert 
            bookReturnMock.VerifySet(x => x.Success = false, Times.Exactly(4));
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Build_Should_ReturnWarnings_When_UnsuccessfulComponentBooksReturn()
        {
            ////Arrange
            var bookReturnMock = new Mock<IBookReturn>();
            bookReturnMock.SetupAllProperties();
            var bookReturnBuilder = new BookReturnBuilder(bookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            bookReturnBuilder.AddResponse(response);
            var bookReturn = bookReturnBuilder.Build();

            ////Assert 
            Assert.AreEqual(4, bookReturn.Warnings.Count);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void BuildShouldReturnWarningsWhenAddedThroughAddWarning()
        {
            ////Arrange
            var bookReturnMock = new Mock<IBookReturn>();
            bookReturnMock.SetupAllProperties();
            var bookReturnBuilder = new BookReturnBuilder(bookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            bookReturnBuilder.AddWarning("Test Warning 1");
            bookReturnBuilder.AddWarning("Test Warning 2");
            bookReturnBuilder.AddWarning("Test Warning 3");
            bookReturnBuilder.AddWarning("Test Warning 4");
            bookReturnBuilder.AddWarning("Test Warning 5");
            var bookReturn = bookReturnBuilder.Build();

            ////Assert 
            Assert.AreEqual(5, bookReturn.Warnings.Count);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void BuildShouldReturnWarningsWhenAddedThroughAddWarnings()
        {
            ////Arrange
            var bookReturnMock = new Mock<IBookReturn>();
            bookReturnMock.SetupAllProperties();
            var bookReturnBuilder = new BookReturnBuilder(bookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            bookReturnBuilder.AddWarnings(new List<string>() { "Test Warning 1", "Test Warning 2", "Test Warning 3" });

            var bookReturn = bookReturnBuilder.Build();

            ////Assert 
            Assert.AreEqual(3, bookReturn.Warnings.Count);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void SetToFailure_Should_TryToSetSuccessToFalse_When_Called()
        {
            ////Arrange
            var bookReturnMock = new Mock<IBookReturn>();
            var bookReturnBuilder = new BookReturnBuilder(bookReturnMock.Object);

            ////Act
            bookReturnBuilder.SetToFailure();
            var bookReturn = bookReturnBuilder.Build();

            ////Assert 
            bookReturnMock.VerifySet(x => x.Success = false, Times.Once());
        }
    }
}