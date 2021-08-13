namespace Web.Template.Application.Tests.Prebook
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Property;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Prebook.Builders;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Tests for the Prebook Return Builder
    /// </summary>
    [TestFixture]
    public class PrebookReturnBuilderTests
    {
        /// <summary>
        /// Sets up fake response.
        /// </summary>
        /// <returns>A prebook response</returns>
        private iVectorConnectInterface.Basket.PreBookResponse SetUpFakeResponse()
        {
            var response = new ivci.Basket.PreBookResponse()
                               {
                                   PropertyBookings = new List<PreBookResponse>() { new PreBookResponse() { ReturnStatus = new ivci.ReturnStatus() { Success = false } } }, 
                                   FlightBookings =
                                       new List<iVectorConnectInterface.Flight.PreBookResponse>()
                                           {
                                               new ivci.Flight.PreBookResponse() { ReturnStatus = new ivci.ReturnStatus() { Success = false } }, 
                                               new ivci.Flight.PreBookResponse() { ReturnStatus = new ivci.ReturnStatus() { Success = false } }, 
                                           }, 
                                   TransferBookings =
                                       new List<iVectorConnectInterface.Transfer.PreBookResponse>() { new ivci.Transfer.PreBookResponse() { ReturnStatus = new ivci.ReturnStatus() { Success = false } } }
                               };

            return response;
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Build_Should_ReturnAPrebookReturn_When_Called()
        {
            ////Arrange
            var prebookReturnMock = new Mock<IPrebookReturn>();
            var prebookReturnBuilder = new PrebookReturnBuilder(prebookReturnMock.Object);

            ////Act
            var prebookReturn = prebookReturnBuilder.Build();

            ////Assert 
            Assert.IsNotNull(prebookReturn);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Build_Should_ReturnAPrebookReturnWithABakset_When_CalledWithHavingSetOne()
        {
            ////Arrange
            var prebookReturnMock = new Mock<IPrebookReturn>();
            prebookReturnMock.SetupAllProperties();
            var prebookReturnBuilder = new PrebookReturnBuilder(prebookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            prebookReturnBuilder.SetBasket(new Mock<IBasket>().Object);
            var prebookReturn = prebookReturnBuilder.Build();

            ////Assert 
            Assert.IsNotNull(prebookReturn.Basket);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Build_Should_ReturnAPrebookReturnWithNoBakset_When_CalledWithoutHavingSetOne()
        {
            ////Arrange
            var prebookReturnMock = new Mock<IPrebookReturn>();
            prebookReturnMock.SetupAllProperties();
            var prebookReturnBuilder = new PrebookReturnBuilder(prebookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            var prebookReturn = prebookReturnBuilder.Build();

            ////Assert 
            Assert.IsNull(prebookReturn.Basket);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Build_Should_ReturnSetTheSuccessToFalse_When_UnsuccessfulComponentPrebooksReturn()
        {
            ////Arrange
            var prebookReturnMock = new Mock<IPrebookReturn>();
            prebookReturnMock.SetupAllProperties();
            var prebookReturnBuilder = new PrebookReturnBuilder(prebookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            prebookReturnBuilder.AddResponse(response);
            var prebookReturn = prebookReturnBuilder.Build();

            ////Assert 
            prebookReturnMock.VerifySet(x => x.Success = false, Times.Exactly(4));
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Build_Should_ReturnWarnings_When_UnsuccessfulComponentPrebooksReturn()
        {
            ////Arrange
            var prebookReturnMock = new Mock<IPrebookReturn>();
            prebookReturnMock.SetupAllProperties();
            var prebookReturnBuilder = new PrebookReturnBuilder(prebookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            prebookReturnBuilder.AddResponse(response);
            var prebookReturn = prebookReturnBuilder.Build();

            ////Assert 
            Assert.AreEqual(prebookReturn.Warnings.Count, 4);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void BuildShouldReturnWarningsWhenAddedThroughAddWarning()
        {
            ////Arrange
            var prebookReturnMock = new Mock<IPrebookReturn>();
            prebookReturnMock.SetupAllProperties();
            var prebookReturnBuilder = new PrebookReturnBuilder(prebookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            prebookReturnBuilder.AddWarning("Test Warning 1");
            prebookReturnBuilder.AddWarning("Test Warning 2");
            prebookReturnBuilder.AddWarning("Test Warning 3");
            prebookReturnBuilder.AddWarning("Test Warning 4");
            prebookReturnBuilder.AddWarning("Test Warning 5");
            var prebookReturn = prebookReturnBuilder.Build();

            ////Assert 
            Assert.AreEqual(prebookReturn.Warnings.Count, 5);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void BuildShouldReturnWarningsWhenAddedThroughAddWarnings()
        {
            ////Arrange
            var prebookReturnMock = new Mock<IPrebookReturn>();
            prebookReturnMock.SetupAllProperties();
            var prebookReturnBuilder = new PrebookReturnBuilder(prebookReturnMock.Object);

            var response = this.SetUpFakeResponse();

            ////Act
            prebookReturnBuilder.AddWarnings(new List<string>() { "Test Warning 1", "Test Warning 2", "Test Warning 3" });

            var prebookReturn = prebookReturnBuilder.Build();

            ////Assert 
            Assert.AreEqual(prebookReturn.Warnings.Count, 3);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void SetToFailure_Should_TryToSetSuccessToFalse_When_Called()
        {
            ////Arrange
            var prebookReturnMock = new Mock<IPrebookReturn>();
            var prebookReturnBuilder = new PrebookReturnBuilder(prebookReturnMock.Object);

            ////Act
            prebookReturnBuilder.SetToFailure();
            var prebookReturn = prebookReturnBuilder.Build();

            ////Assert 
            prebookReturnMock.VerifySet(x => x.Success = false, Times.Once());
        }
    }
}