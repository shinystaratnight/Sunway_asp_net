using Web.Template.Models.Application;

namespace Web.Template.Application.Tests.Basket
{
    using System.Collections.Generic;

    using AutoMapper;
    using Interfaces.BookingAdjustment;
    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Basket.Factories;
    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Basket.Services;
    using Web.Template.Application.Interfaces.Basket;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Payment;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Interfaces.Promocode;
    using Web.Template.Application.Interfaces.Repositories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Services;
    using Web.Template.IoC;

    /// <summary>
    /// Tests for the Basket Service
    /// </summary>
    [TestFixture]
    public class BasketServiceTests
    {
        /// <summary>
        /// Adds the component_ should add a component_ when component exists.
        /// </summary>
        [Test]
        public void AddComponent_ShouldAddAComponent_WhenComponentExists()
        {
            ////Arrange
            var basketMock = new Mock<IBasket>();
            basketMock.SetupGet(b => b.Components).Returns(new List<IBasketComponent>());

            var resultMock = new Mock<IResult>();

            var componentMock = new Mock<IBasketComponent>();

            resultMock.Setup(rm => rm.CreateBasketComponent()).Returns(componentMock.Object);

            var basketRepositoryMock = new Mock<IBasketRepository>();
            basketRepositoryMock.Setup(x => x.RetrieveBasketByToken(It.IsAny<string>())).Returns(basketMock.Object);

            var resultServiceMock = new Mock<IResultService>();
            resultServiceMock.Setup(r => r.RetrieveResult(It.IsAny<string>(), It.IsAny<int>())).Returns(resultMock.Object);

            var prebookAdaptorMock = new Mock<IBasketPrebookService>();

            var bookAdaptorMock = new Mock<IBasketBookService>();

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<IResult, IBasketComponent>(It.IsAny<IResult>())).Returns(new Mock<IBasketComponent>().Object);

            var promocodeServiceMock = new Mock<IPromoCodeService>();
            var seatlockFactoryMock = new Mock<IReleaseFlightSeatLockFactory>();
            var connectRequestMock = new Mock<IIVectorConnectRequestFactory>();

            var threeDSecureServiceMock = new Mock<IThreeDSecureService>();
            var storeBasketFactoryMock = new Mock<IStoreBasketFactory>();
            var retrieveStoredBasketFactoryMock = new Mock<IRetrieveStoredBasketFactory>();
            var siteServiceMock = new Mock<ISiteService>();
            var bookingAdjustmentMock = new Mock<IBookingAdjustmentService>();
	        var connectLoginDetailsFactory = new Mock<IConnectLoginDetailsFactory>();

            IBasketService basketService = new BasketService(
                basketRepositoryMock.Object,
                resultServiceMock.Object,
                prebookAdaptorMock.Object,
                mapperMock.Object,
                bookAdaptorMock.Object,
                promocodeServiceMock.Object,
                seatlockFactoryMock.Object,
                connectRequestMock.Object,
                threeDSecureServiceMock.Object,
                storeBasketFactoryMock.Object,
                retrieveStoredBasketFactoryMock.Object,
                siteServiceMock.Object,
                bookingAdjustmentMock.Object,
				connectLoginDetailsFactory.Object);

            AutofacRoot.SetupForTests();
            BasketComponentModel model = new BasketComponentModel()
            {
                BasketToken = "t1",
                SearchToken = "t2",
                ComponentToken = 0
            };

            ////ACT
            IBasket basket = basketService.AddComponent(model);

            ////ASSERT
            Assert.AreEqual(basket.Components.Count, 1);
        }

        /// <summary>
        /// Adds the component_ should add a sub component_ when component exists.
        /// </summary>
        [Test]
        public void AddComponent_ShouldAddASubComponent_WhenComponentExists()
        {
            ////Arrange
            var basketMock = new Mock<IBasket>();
            basketMock.SetupGet(b => b.Components).Returns(new List<IBasketComponent>());

            var subResultMock = new Mock<ISubResult>();
            subResultMock.SetupGet(s => s.ComponentToken).Returns(123456);

            var resultMock = new Mock<IResult>();

            var componentMock = new Mock<IBasketComponent>();

            resultMock.Setup(rm => rm.CreateBasketComponent()).Returns(componentMock.Object);
            resultMock.SetupGet(c => c.SubResults).Returns(new List<ISubResult>() { subResultMock.Object });

            var basketRepositoryMock = new Mock<IBasketRepository>();
            basketRepositoryMock.Setup(x => x.RetrieveBasketByToken(It.IsAny<string>())).Returns(basketMock.Object);

            var resultServiceMock = new Mock<IResultService>();
            resultServiceMock.Setup(r => r.RetrieveResult(It.IsAny<string>(), It.IsAny<int>())).Returns(resultMock.Object);

            var prebookAdaptorMock = new Mock<IBasketPrebookService>();

            var bookAdaptorMock = new Mock<IBasketBookService>();

            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(m => m.Map<IResult, IBasketComponent>(It.IsAny<IResult>())).Returns(new Mock<IBasketComponent>().Object);

            var promocodeServiceMock = new Mock<IPromoCodeService>();
            var seatlockFactoryMock = new Mock<IReleaseFlightSeatLockFactory>();
            var connectRequestMock = new Mock<IIVectorConnectRequestFactory>();

            var threeDSecureServiceMock = new Mock<IThreeDSecureService>();
            var storeBasketFactoryMock = new Mock<IStoreBasketFactory>();
            var retrieveStoredBasketFactoryMock = new Mock<IRetrieveStoredBasketFactory>();
            var siteServiceMock = new Mock<ISiteService>();
            var bookingAdjustmentMock = new Mock<IBookingAdjustmentService>();
			var connectLoginDetailsFactory = new Mock<IConnectLoginDetailsFactory>();

			IBasketService basketService = new BasketService(
                basketRepositoryMock.Object,
                resultServiceMock.Object,
                prebookAdaptorMock.Object,
                mapperMock.Object,
                bookAdaptorMock.Object,
                promocodeServiceMock.Object,
                seatlockFactoryMock.Object,
                connectRequestMock.Object,
                threeDSecureServiceMock.Object,
                storeBasketFactoryMock.Object,
                retrieveStoredBasketFactoryMock.Object,
                siteServiceMock.Object,
                bookingAdjustmentMock.Object,
				connectLoginDetailsFactory.Object);

            BasketComponentModel model = new BasketComponentModel()
            {
                BasketToken = "t1",
                SearchToken = "t2",
                ComponentToken = 0,
                SubComponentTokens = new List<int>() { 123456 }
            };

            ////ACT
            IBasket basket = basketService.AddComponent(model);

            ////ASSERT
            Assert.AreEqual(basket.Components.Count, 1);
        }

        /// <summary>
        /// Adds the component_ should not add a component_ when component is null.
        /// </summary>
        [Test]
        public void AddComponent_ShouldNotAddAComponent_WhenComponentIsNull()
        {
            ////Arrange
            var basketMock = new Mock<IBasket>();
            var componentMock = new Mock<IResult>();

            var basketRepositoryMock = new Mock<IBasketRepository>();
            basketRepositoryMock.Setup(x => x.RetrieveBasketByToken(It.IsAny<string>())).Returns(basketMock.Object);

            var resultServiceMock = new Mock<IResultService>();
            resultServiceMock.Setup(r => r.RetrieveResult(It.IsAny<string>(), It.IsAny<int>())).Returns((IResult)null);

            var prebookAdaptorMock = new Mock<IBasketPrebookService>();

            var bookAdaptorMock = new Mock<IBasketBookService>();

            var mapperMock = new Mock<IMapper>();

            var promocodeServiceMock = new Mock<IPromoCodeService>();
            var seatlockFactoryMock = new Mock<IReleaseFlightSeatLockFactory>();
            var connectRequestMock = new Mock<IIVectorConnectRequestFactory>();

            var threeDSecureServiceMock = new Mock<IThreeDSecureService>();
            var storeBasketFactoryMock = new Mock<IStoreBasketFactory>();
            var retrieveStoredBasketFactoryMock = new Mock<IRetrieveStoredBasketFactory>();
            var siteServiceMock = new Mock<ISiteService>();
            var bookingAdjustmentMock = new Mock<IBookingAdjustmentService>();
			var connectLoginDetailsFactory = new Mock<IConnectLoginDetailsFactory>();

			IBasketService basketService = new BasketService(
                basketRepositoryMock.Object,
                resultServiceMock.Object,
                prebookAdaptorMock.Object,
                mapperMock.Object,
                bookAdaptorMock.Object,
                promocodeServiceMock.Object,
                seatlockFactoryMock.Object,
                connectRequestMock.Object,
                threeDSecureServiceMock.Object,
                storeBasketFactoryMock.Object,
                retrieveStoredBasketFactoryMock.Object,
                siteServiceMock.Object,
                bookingAdjustmentMock.Object,
				connectLoginDetailsFactory.Object);

            BasketComponentModel model = new BasketComponentModel()
            {
                BasketToken = "t1",
                SearchToken = "t2",
                ComponentToken = 0
            };

            ////ACT
            IBasket basket = basketService.AddComponent(model);

            ////ASSERT
            Assert.IsNull(basket.Components);
        }

        /// <summary>
        /// Don't prebook a basket that has all component set to prebook.
        /// </summary>
        [Test]
        public void PrebookBasket_Should_AttemptAPrebook_When_AllComponentsNotPrebooked()
        {
            ////Arrange
            var basketMock = new Mock<IBasket>();
            basketMock.SetupGet(b => b.AllComponentsPreBooked).Returns(false);

            var prebookReturnMock = new Mock<IPrebookReturn>();
            prebookReturnMock.SetupGet(pr => pr.Basket).Returns(basketMock.Object);

            var prebookAdaptorMock = new Mock<IBasketPrebookService>();
            prebookAdaptorMock.Setup(pa => pa.Prebook(It.IsAny<IBasket>(), It.IsAny<int>())).Returns(prebookReturnMock.Object);
            var resultServiceMock = new Mock<IResultService>();

            var basketRepositoryMock = new Mock<IBasketRepository>();
            basketRepositoryMock.Setup(x => x.RetrieveBasketByToken(It.IsAny<string>())).Returns(basketMock.Object);

            var bookAdaptorMock = new Mock<IBasketBookService>();

            var mapperMock = new Mock<IMapper>();

            var promocodeServiceMock = new Mock<IPromoCodeService>();
            var seatlockFactoryMock = new Mock<IReleaseFlightSeatLockFactory>();
            var connectRequestMock = new Mock<IIVectorConnectRequestFactory>();

            var threeDSecureServiceMock = new Mock<IThreeDSecureService>();
            var storeBasketFactoryMock = new Mock<IStoreBasketFactory>();
            var retrieveStoredBasketFactoryMock = new Mock<IRetrieveStoredBasketFactory>();
            var siteServiceMock = new Mock<ISiteService>();
            var bookingAdjustmentMock = new Mock<IBookingAdjustmentService>();
			var connectLoginDetailsFactory = new Mock<IConnectLoginDetailsFactory>();

			IBasketService basketService = new BasketService(
                basketRepositoryMock.Object,
                resultServiceMock.Object,
                prebookAdaptorMock.Object,
                mapperMock.Object,
                bookAdaptorMock.Object,
                promocodeServiceMock.Object,
                seatlockFactoryMock.Object,
                connectRequestMock.Object,
                threeDSecureServiceMock.Object,
                storeBasketFactoryMock.Object,
                retrieveStoredBasketFactoryMock.Object,
                siteServiceMock.Object,
                bookingAdjustmentMock.Object,
				connectLoginDetailsFactory.Object);

            ////ACT
            basketService.PreBookBasket("t1");

            ////ASSERT
            prebookAdaptorMock.Verify(pa => pa.Prebook(It.IsAny<IBasket>(), It.IsAny<int>()), Times.AtLeastOnce);
        }
        
        /// <summary>
        /// Ensure we're trying to prebook if we have not prebook components
        /// </summary>
        [Test]
        public void PrebookComponent_Should_AttemptAPrebook_WhenComponentNotPrebooked()
        {
            ////Arrange
            var basketMock = new Mock<IBasket>();

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupGet(c => c.ComponentPreBooked).Returns(false);
            componentMock.SetupGet(c => c.ComponentToken).Returns(767676);

            basketMock.SetupGet(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            var prebookAdaptorMock = new Mock<IBasketPrebookService>();
            prebookAdaptorMock.Setup(pa => pa.Prebook(It.IsAny<IBasket>(), It.IsAny<int>())).Returns(new Mock<IPrebookReturn>().Object);
            var resultServiceMock = new Mock<IResultService>();

            var basketRepositoryMock = new Mock<IBasketRepository>();
            basketRepositoryMock.Setup(x => x.RetrieveBasketByToken(It.IsAny<string>())).Returns(basketMock.Object);

            var bookAdaptorMock = new Mock<IBasketBookService>();

            var mapperMock = new Mock<IMapper>();

            var promocodeServiceMock = new Mock<IPromoCodeService>();
            var seatlockFactoryMock = new Mock<IReleaseFlightSeatLockFactory>();
            var connectRequestMock = new Mock<IIVectorConnectRequestFactory>();

            var threeDSecureServiceMock = new Mock<IThreeDSecureService>();
            var storeBasketFactoryMock = new Mock<IStoreBasketFactory>();
            var retrieveStoredBasketFactoryMock = new Mock<IRetrieveStoredBasketFactory>();
            var siteServiceMock = new Mock<ISiteService>();
            var bookingAdjustmentMock = new Mock<IBookingAdjustmentService>();
			var connectLoginDetailsFactory = new Mock<IConnectLoginDetailsFactory>();

			IBasketService basketService = new BasketService(
                basketRepositoryMock.Object,
                resultServiceMock.Object,
                prebookAdaptorMock.Object,
                mapperMock.Object,
                bookAdaptorMock.Object,
                promocodeServiceMock.Object,
                seatlockFactoryMock.Object,
                connectRequestMock.Object,
                threeDSecureServiceMock.Object,
                storeBasketFactoryMock.Object,
                retrieveStoredBasketFactoryMock.Object,
                siteServiceMock.Object,
                bookingAdjustmentMock.Object,
				connectLoginDetailsFactory.Object);

            ////ACT
            basketService.PreBookComponent("t1", 767676);

            ////ASSERT
            prebookAdaptorMock.Verify(pa => pa.Prebook(It.IsAny<IBasket>(), It.IsAny<int>()), Times.Once);
        }

        /// <summary>
        /// Don't prebook an already prebook component.
        /// </summary>
        [Test]
        public void PrebookComponent_Should_NotAttemptAPrebook_WhenComponentAlreadyPrebooked()
        {
            ////Arrange
            var basketMock = new Mock<IBasket>();

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupGet(c => c.ComponentPreBooked).Returns(true);
            componentMock.SetupGet(c => c.ComponentToken).Returns(767676);

            basketMock.SetupGet(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            var prebookAdaptorMock = new Mock<IBasketPrebookService>();
            var resultServiceMock = new Mock<IResultService>();

            var basketRepositoryMock = new Mock<IBasketRepository>();
            basketRepositoryMock.Setup(x => x.RetrieveBasketByToken(It.IsAny<string>())).Returns(basketMock.Object);

            var bookAdaptorMock = new Mock<IBasketBookService>();

            var mapperMock = new Mock<IMapper>();

            var promocodeServiceMock = new Mock<IPromoCodeService>();
            var seatlockFactoryMock = new Mock<IReleaseFlightSeatLockFactory>();
            var connectRequestMock = new Mock<IIVectorConnectRequestFactory>();

            var threeDSecureServiceMock = new Mock<IThreeDSecureService>();
            var storeBasketFactoryMock = new Mock<IStoreBasketFactory>();
            var retrieveStoredBasketFactoryMock = new Mock<IRetrieveStoredBasketFactory>();
            var siteServiceMock = new Mock<ISiteService>();
            var bookingAdjustmentMock = new Mock<IBookingAdjustmentService>();
			var connectLoginDetailsFactory = new Mock<IConnectLoginDetailsFactory>();

			IBasketService basketService = new BasketService(
                basketRepositoryMock.Object,
                resultServiceMock.Object,
                prebookAdaptorMock.Object,
                mapperMock.Object,
                bookAdaptorMock.Object,
                promocodeServiceMock.Object,
                seatlockFactoryMock.Object,
                connectRequestMock.Object,
                threeDSecureServiceMock.Object,
                storeBasketFactoryMock.Object,
                retrieveStoredBasketFactoryMock.Object,
                siteServiceMock.Object,
                bookingAdjustmentMock.Object,
				connectLoginDetailsFactory.Object);

            ////ACT
            basketService.PreBookComponent("t1", 767676);

            ////ASSERT
            prebookAdaptorMock.Verify(pa => pa.Prebook(It.IsAny<IBasket>(), It.IsAny<int>()), Times.Never);
        }

        /// <summary>
        /// If we're trying to prebook a component that doesn't exist, don't.
        /// </summary>
        [Test]
        public void PrebookComponent_Should_NotAttemptAPrebook_WhenComponentDoesNotExist()
        {
            ////Arrange
            var basketMock = new Mock<IBasket>();

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupGet(c => c.ComponentPreBooked).Returns(true);
            componentMock.SetupGet(c => c.ComponentToken).Returns(999999);
            basketMock.SetupGet(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            var prebookAdaptorMock = new Mock<IBasketPrebookService>();
            var resultServiceMock = new Mock<IResultService>();

            var basketRepositoryMock = new Mock<IBasketRepository>();
            basketRepositoryMock.Setup(x => x.RetrieveBasketByToken(It.IsAny<string>())).Returns(basketMock.Object);

            var bookAdaptorMock = new Mock<IBasketBookService>();

            var mapperMock = new Mock<IMapper>();

            var promocodeServiceMock = new Mock<IPromoCodeService>();

            var seatlockFactoryMock = new Mock<IReleaseFlightSeatLockFactory>();
            var connectRequestMock = new Mock<IIVectorConnectRequestFactory>();

            var threeDSecureServiceMock = new Mock<IThreeDSecureService>();
            var storeBasketFactoryMock = new Mock<IStoreBasketFactory>();
            var retrieveStoredBasketFactoryMock = new Mock<IRetrieveStoredBasketFactory>();
            var siteServiceMock = new Mock<ISiteService>();
            var bookingAdjustmentMock = new Mock<IBookingAdjustmentService>();
			var connectLoginDetailsFactory = new Mock<IConnectLoginDetailsFactory>();

			IBasketService basketService = new BasketService(
                basketRepositoryMock.Object,
                resultServiceMock.Object,
                prebookAdaptorMock.Object,
                mapperMock.Object,
                bookAdaptorMock.Object,
                promocodeServiceMock.Object,
                seatlockFactoryMock.Object,
                connectRequestMock.Object,
                threeDSecureServiceMock.Object,
                storeBasketFactoryMock.Object,
                retrieveStoredBasketFactoryMock.Object,
                siteServiceMock.Object,
                bookingAdjustmentMock.Object,
				connectLoginDetailsFactory.Object);

            ////ACT
            basketService.PreBookComponent("t1", 767676);

            ////ASSERT
            prebookAdaptorMock.Verify(pa => pa.Prebook(It.IsAny<IBasket>(), It.IsAny<int>()), Times.Never);
        }

        /// <summary>
        /// Sets up a basket with one component and two subcomponents within that component, if we call remove component with no sub component tokens,
        /// we should remove the whole component regardless of the fact it has two subcomponents.
        /// </summary>
        [Test]
        public void RemoveComponent_Should_RemoveComponent_When_CalledWithNoSubcomponentTokens()
        {
            ////Arrange
            var basketMock = new Mock<IBasket>();

            var subcomponentMockA = new Mock<ISubComponent>();
            subcomponentMockA.SetupGet(sc => sc.ComponentToken).Returns(12345);

            var subcomponentMockB = new Mock<ISubComponent>();
            subcomponentMockB.SetupGet(sc => sc.ComponentToken).Returns(678910);

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupGet(c => c.SubComponents).Returns(new List<ISubComponent>() { subcomponentMockB.Object, subcomponentMockA.Object });
            componentMock.SetupGet(c => c.ComponentToken).Returns(767676);

            basketMock.SetupGet(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            var basketRepositoryMock = new Mock<IBasketRepository>();
            basketRepositoryMock.Setup(x => x.RetrieveBasketByToken(It.IsAny<string>())).Returns(basketMock.Object);

            var prebookAdaptorMock = new Mock<IBasketPrebookService>();
            var resultServiceMock = new Mock<IResultService>();

            var bookAdaptorMock = new Mock<IBasketBookService>();

            var mapperMock = new Mock<IMapper>();

            var promocodeServiceMock = new Mock<IPromoCodeService>();

            var seatlockFactoryMock = new Mock<IReleaseFlightSeatLockFactory>();
            var connectRequestMock = new Mock<IIVectorConnectRequestFactory>();

            var threeDSecureServiceMock = new Mock<IThreeDSecureService>();
            var storeBasketFactoryMock = new Mock<IStoreBasketFactory>();
            var retrieveStoredBasketFactoryMock = new Mock<IRetrieveStoredBasketFactory>();
            var siteServiceMock = new Mock<ISiteService>();
            var bookingAdjustmentMock = new Mock<IBookingAdjustmentService>();
			var connectLoginDetailsFactory = new Mock<IConnectLoginDetailsFactory>();

			IBasketService basketService = new BasketService(
                basketRepositoryMock.Object,
                resultServiceMock.Object,
                prebookAdaptorMock.Object,
                mapperMock.Object,
                bookAdaptorMock.Object,
                promocodeServiceMock.Object,
                seatlockFactoryMock.Object,
                connectRequestMock.Object,
                threeDSecureServiceMock.Object,
                storeBasketFactoryMock.Object,
                retrieveStoredBasketFactoryMock.Object,
                siteServiceMock.Object,
                bookingAdjustmentMock.Object,
				connectLoginDetailsFactory.Object);

            ////ACT
            IBasket basket = basketService.RemoveComponent("t1", 767676);

            ////ASSERT
            Assert.AreEqual(basket.Components.Count, 0);
        }

        /// <summary>
        /// Sets up a basket with one component and two subcomponents within that component, if we call remove component with a single sub component token,
        /// we should remove the matching one and be left with one component with one subcomponent on our basket.
        /// </summary>
        [Test]
        public void RemoveComponent_ShouldRemoveASubComponent_WhenComponentExists()
        {
            ////Arrange
            var basketMock = new Mock<IBasket>();

            var subcomponentMockA = new Mock<ISubComponent>();
            subcomponentMockA.SetupGet(sc => sc.ComponentToken).Returns(12345);

            var subcomponentMockB = new Mock<ISubComponent>();
            subcomponentMockB.SetupGet(sc => sc.ComponentToken).Returns(678910);

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupGet(c => c.SubComponents).Returns(new List<ISubComponent>() { subcomponentMockB.Object, subcomponentMockA.Object });
            componentMock.SetupGet(c => c.ComponentToken).Returns(767676);

            basketMock.SetupGet(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            var basketRepositoryMock = new Mock<IBasketRepository>();
            basketRepositoryMock.Setup(x => x.RetrieveBasketByToken(It.IsAny<string>())).Returns(basketMock.Object);

            var prebookAdaptorMock = new Mock<IBasketPrebookService>();
            var resultServiceMock = new Mock<IResultService>();

            var bookAdaptorMock = new Mock<IBasketBookService>();

            var mapperMock = new Mock<IMapper>();

            var promocodeServiceMock = new Mock<IPromoCodeService>();
            var seatlockFactoryMock = new Mock<IReleaseFlightSeatLockFactory>();
            var connectRequestMock = new Mock<IIVectorConnectRequestFactory>();

            var threeDSecureServiceMock = new Mock<IThreeDSecureService>();
            var storeBasketFactoryMock = new Mock<IStoreBasketFactory>();
            var retrieveStoredBasketFactoryMock = new Mock<IRetrieveStoredBasketFactory>();
            var siteServiceMock = new Mock<ISiteService>();
            var bookingAdjustmentMock = new Mock<IBookingAdjustmentService>();
			var connectLoginDetailsFactory = new Mock<IConnectLoginDetailsFactory>();

			IBasketService basketService = new BasketService(
                basketRepositoryMock.Object,
                resultServiceMock.Object,
                prebookAdaptorMock.Object,
                mapperMock.Object,
                bookAdaptorMock.Object,
                promocodeServiceMock.Object,
                seatlockFactoryMock.Object,
                connectRequestMock.Object,
                threeDSecureServiceMock.Object,
                storeBasketFactoryMock.Object,
                retrieveStoredBasketFactoryMock.Object,
                siteServiceMock.Object,
                bookingAdjustmentMock.Object,
				connectLoginDetailsFactory.Object);

            ////ACT
            IBasket basket = basketService.RemoveComponent("t1", 767676, new List<int>() { 12345 });

            ////ASSERT
            Assert.AreEqual(basket.Components.Count, 1);
            Assert.AreEqual(basket.Components[0].SubComponents.Count, 1);
        }

        /// <summary>
        /// Sets up a basket with one component and two subcomponents within that component, if we call remove component with a both sub component tokens,
        /// leaving us with a component with no remaining subcomponents we should remove the component itself
        /// </summary>
        [Test]
        public void RemoveComponent_ShouldRemoveComponent_WhenAllSubcomponentsRemoved()
        {
            ////Arrange
            var basketMock = new Mock<IBasket>();

            var subcomponentMockA = new Mock<ISubComponent>();
            subcomponentMockA.SetupGet(sc => sc.ComponentToken).Returns(12345);

            var subcomponentMockB = new Mock<ISubComponent>();
            subcomponentMockB.SetupGet(sc => sc.ComponentToken).Returns(678910);

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupGet(c => c.SubComponents).Returns(new List<ISubComponent>() { subcomponentMockB.Object, subcomponentMockA.Object });
            componentMock.SetupGet(c => c.ComponentToken).Returns(767676);

            basketMock.SetupGet(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            var basketRepositoryMock = new Mock<IBasketRepository>();
            basketRepositoryMock.Setup(x => x.RetrieveBasketByToken(It.IsAny<string>())).Returns(basketMock.Object);

            var prebookAdaptorMock = new Mock<IBasketPrebookService>();
            var resultServiceMock = new Mock<IResultService>();

            var bookAdaptorMock = new Mock<IBasketBookService>();

            var mapperMock = new Mock<IMapper>();

            var promocodeServiceMock = new Mock<IPromoCodeService>();
            var seatlockFactoryMock = new Mock<IReleaseFlightSeatLockFactory>();
            var connectRequestMock = new Mock<IIVectorConnectRequestFactory>();

            var threeDSecureServiceMock = new Mock<IThreeDSecureService>();
            var storeBasketFactoryMock = new Mock<IStoreBasketFactory>();
            var retrieveStoredBasketFactoryMock = new Mock<IRetrieveStoredBasketFactory>();
            var siteServiceMock = new Mock<ISiteService>();
            var bookingAdjustmentMock = new Mock<IBookingAdjustmentService>();
			var connectLoginDetailsFactory = new Mock<IConnectLoginDetailsFactory>();

			IBasketService basketService = new BasketService(
                basketRepositoryMock.Object,
                resultServiceMock.Object,
                prebookAdaptorMock.Object,
                mapperMock.Object,
                bookAdaptorMock.Object,
                promocodeServiceMock.Object,
                seatlockFactoryMock.Object,
                connectRequestMock.Object,
                threeDSecureServiceMock.Object,
                storeBasketFactoryMock.Object,
                retrieveStoredBasketFactoryMock.Object,
                siteServiceMock.Object,
                bookingAdjustmentMock.Object,
				connectLoginDetailsFactory.Object);

            ////ACT
            IBasket basket = basketService.RemoveComponent("t1", 767676, new List<int>() { 12345, 678910 });

            ////ASSERT
            Assert.AreEqual(basket.Components.Count, 0);
        }
    }
}