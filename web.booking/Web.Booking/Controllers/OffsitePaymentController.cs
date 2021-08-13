using ServiceStack;

namespace Web.Booking.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Web.Http;
	using System.Web.Mvc;
	using Intuitive;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Web.Booking.Models.Application;
	using Web.Template.Application.Enum;
	using Web.Template.Application.Interfaces.Configuration;
	using Web.Template.Application.Interfaces.Models;
	using Web.Template.Application.Interfaces.Payment;
	using Web.Template.Application.Interfaces.Services;
	using Web.Template.Application.Interfaces.SiteBuilder;
	using Web.Template.Application.Interfaces.User;
	using Web.Template.Application.PageDefinition;
	using Web.Template.Application.PageDefinition.Enums;
	using Web.Template.Application.Payment.Models;
	using Web.Template.Application.SiteBuilderService.Models;

	/// <summary>
	/// Class OffsitePaymentController.
	/// </summary>
	public class OffsitePaymentController : Controller
	{
		/// <summary>
		/// The content service
		/// </summary>
		private readonly IContentService contentService;

		/// <summary>
		///     The page service
		/// </summary>
		private readonly IPageService pageService;

		/// <summary>
		/// The tracking affiliate service
		/// </summary>
		private readonly ITrackingAffiliateService trackingAffiliateService;

		/// <summary>
		/// The user service
		/// </summary>
		private readonly IUserService userService;

		/// <summary>
		/// The site service
		/// </summary>
		private readonly ISiteBuilderService siteBuilderService;

		/// <summary>
		/// The log writer
		/// </summary>
		private readonly ILogWriter logWriter;

		/// <summary>
		/// The basket service
		/// </summary>
		private readonly IBasketService basketService;

		/// <summary>
		/// The site service
		/// </summary>
		private readonly ISiteService siteService;

		/// <summary>
		/// Initializes a new instance of the <see cref="OffsitePaymentController" /> class.
		/// </summary>
		/// <param name="basketService">The basket service.</param>
		/// <param name="siteService">The site service.</param>
		/// <param name="pageService">The page service.</param>
		/// <param name="contentService">The content service.</param>
		/// <param name="userService">The user service.</param>
		/// <param name="trackingAffiliateService">The tracking affiliate service.</param>
		/// <param name="siteBuilderService">The site builder service.</param>
		/// <param name="logWriter">The log writer.</param>
		public OffsitePaymentController(
			IBasketService basketService,
			ISiteService siteService,
			IPageService pageService,
			IContentService contentService,
			IUserService userService,
			ITrackingAffiliateService trackingAffiliateService,
			ISiteBuilderService siteBuilderService,
			ILogWriter logWriter)
		{
			this.basketService = basketService;
			this.siteService = siteService;
			this.pageService = pageService;
			this.contentService = contentService;
			this.userService = userService;
			this.trackingAffiliateService = trackingAffiliateService;
			this.siteBuilderService = siteBuilderService;
			this.logWriter = logWriter;
		}

		/// <summary>
		/// Setups the specified basket token.
		/// </summary>
		/// <param name="basketToken">The basket token.</param>
		/// <returns>The 3d secure page</returns>
		public async Task<ActionResult> Submit([FromUri] string t)
		{
			try
			{
				Page pageModel = this.pageService.GetPageByURL("offsitepayment");
				IUserSession userSession = this.userService.GetUser(System.Web.HttpContext.Current);

				if (pageModel.Access != AccessLevel.Public && !this.CheckAuthentication(userSession, pageModel))
				{
					System.Web.HttpContext.Current.Session.Abandon();
					return this.Redirect("/?warning=login");
				}
				pageModel.AdminMode = userSession.AdminMode;

				ReduxState state = new ReduxState { entities = new Dictionary<string, object>() };

				ISite site = this.siteService.GetSite(System.Web.HttpContext.Current);
				pageModel.SiteBaseUrl = site.Url;
				pageModel.SiteName = site.Name;

#if DEBUG
				pageModel.SiteBaseUrl = "http://localhost:64351";
#endif

				dynamic data = this.GetThemeData(site);
				pageModel.FontScript = data.Typography.FontScript;
				pageModel.FontSource = data.Typography.FontSource;

				// Get tracking Affiliates
				pageModel.TrackingAffiliates = await this.trackingAffiliateService.SetupTrackingAffiliates();

				foreach (Section section in pageModel.Sections)
				{
					foreach (Widget widget in section.Widgets)
					{
						if (widget.Overbranded && !userSession.OverBranded)
						{
							continue;
						}

						widget.ContentService = this.contentService;
						widget.UserService = this.userService;
						widget.SiteService = this.siteService;
						widget.Page = new PageViewModel()
						{
							EntityInformations = pageModel.EntityInformations,
							EntityType = pageModel.EntityType,
							PageURL = pageModel.Url,
							PageName = pageModel.Name,
							MetaInformation = pageModel.MetaInformation
						};
						widget.SetupContent();

						if (widget.ClientSideRender && widget.ServerSideRender && !string.IsNullOrEmpty(widget.Context))
						{
							var siteName = widget.Shared ? "Shared" : widget.Site.Name;
							EntityModel entityModel = this.siteBuilderService.GetEntity(siteName,
								widget.Name,
								widget.Context,
								"live");

							ReduxEntityModel entity = new ReduxEntityModel
							{
								jsonSchema = entityModel.JsonSchema,
								model = widget.ContentJSON,
								isFetching = false,
								isLoaded = true,
								name = widget.Name,
								context = widget.Context,
								lastModifiedDate = entityModel.LastModifiedDate,
								lastModifiedUser = entityModel.LastModifiedUser,
								status = entityModel.Status,
								type = entityModel.Type
							};


							string key = $"{widget.Name}-{entity.context}";
							state.entities.Add(key, entity);
						}
					}
				}

				string stateJSON = JsonConvert.SerializeObject(state, Formatting.None);
				pageModel.PreLoadedState = stateJSON;

				IBasket basket = this.basketService.GetBasket(t);

				pageModel.AdditionalHTML = basket.PaymentDetails.OffsitePaymentHtml;

				return this.View("_Submit", pageModel);
			}
			catch (Exception ex)
			{
				this.logWriter.Write("PagebuilderController", "SetupError", ex.ToString());
			}

			return null;

		}

		/// <summary>
		/// Gets the theme data.
		/// </summary>
		/// <param name="site">The site.</param>
		/// <returns>The theme data</returns>
		private dynamic GetThemeData(ISite site)
		{
			var themeKey = $"site_websites_{site.Name}";
			var cachekey = Intuitive.AsyncCache.Controller<dynamic>.GenerateKey(themeKey);

			dynamic data = Intuitive.AsyncCache.Controller<dynamic>.GetCache(
				cachekey,
				600,
				() =>
				{
					EntityModel themeModel = this.siteBuilderService.GetEntity(site.Name, "Theme", "default", "live");
					dynamic content = JObject.Parse(themeModel.Model);
					return content;
				});
			return data;
		}

		/// <summary>
		/// Checks the authentication.
		/// </summary>
		/// <param name="userSession">The user session.</param>
		/// <param name="page">The page.</param>
		/// <returns></returns>
		public bool CheckAuthentication(IUserSession userSession, Page page)
		{
			var authorized = true;

			switch (page.Access)
			{
				case AccessLevel.TradeLoggedIn:
					authorized = (userSession.TradeSession != null && userSession.TradeSession.LoggedIn) || userSession.AdminMode;
					break;
			}

			return authorized;
		}

		/// <summary>
		/// Responses this instance.
		/// </summary>
		/// <param name="basketStoreId">The basket store identifier.</param>
		/// <param name="formCollection">The form collection.</param>
		/// <returns>The ActionResult.</returns>
		public ActionResult PaymentResponse([FromUri] int t, [FromUri] string r)
		{
			var basketToken = this.basketService.CreateBasket().ToString();
			IBasket basket = this.basketService.RetrieveStoredBasket(t);

			ISite site = this.siteService.GetSite(System.Web.HttpContext.Current);

			if (basket == null)
			{
				return new RedirectResult($"{site.SiteConfiguration.OriginUrl}?warning=basketexpired");
			}

			if (r == "s")
			{
				basket.PaymentDetails.OffsitePaymentTaken = true;
				basket.PaymentDetails.PaymentType = PaymentType.Unset;
				this.basketService.UpdateBasket(basketToken, basket);
				return new RedirectResult($"/booking/completebooking?t={basketToken}");
			}

			basket.PaymentDetails.PaymentToken = string.Empty;
			this.basketService.UpdateBasket(basketToken, basket);
			return new RedirectResult($"/booking/payment?t={basketToken}&warning=paymentfailed");
		}

		/// <summary>
		/// Generically handles payment responses from offsite third parties.
		/// </summary>
		/// <param name="t">The basket token .</param>
		/// <returns>The ActionResult.</returns>
		[System.Web.Mvc.HttpPost]
		public ContentResult PaymentResponsePost([FromUri] string t)
		{
			ISite site = this.siteService.GetSite(System.Web.HttpContext.Current);
			try
			{
				logWriter.Write("OffsitePaymentReturn", "Realex Form", Request.Form.ToString());
				IBasket basket = this.basketService.GetBasket(t);

				if (basket == null)
				{
					return Content($"<script language='javascript' type='text/javascript'>window.location.replace(\"{site.SiteConfiguration.OriginUrl}?warning=basketexpired\");</script>");
				}

				if (VerifyProcessPaymentPostRequest())
				{
					string transactionID;
					try
					{
						transactionID = Request.Form["PASREF"];
					}
					catch (Exception e)
					{
						transactionID = "";
					}

					string paymentToken;
					try
					{
						paymentToken = $"{Request.Form["SAVED_PAYER_REF"]}|{Request.Form["SAVED_PMT_REF"]}";
					}
					catch (Exception e)
					{
						paymentToken = "";
					}

					basket.PaymentDetails.OffsitePaymentTaken = true;
					basket.PaymentDetails.PaymentType = PaymentType.Custom;
					basket.PaymentDetails.TransactionID = transactionID;
					basket.PaymentDetails.PaymentToken = paymentToken;
					logWriter.Write("OffsitePaymentReturn", "Basket Amount", $"Amount:{basket.PaymentDetails.Amount}, TotalAmount:{basket.PaymentDetails.TotalAmount}, Token:{basket.PaymentDetails.PaymentToken}");
					this.basketService.UpdateBasket(t, basket);

					return Content($"<script language='javascript' type='text/javascript'>window.location.replace(\"{site.Url}/booking/completebooking?t={t}\");</script>");
				}
				basket.PaymentDetails.PaymentToken = string.Empty;
				this.basketService.UpdateBasket(t, basket);

				return Content($"<script language='javascript' type='text/javascript'>window.location.replace(\"{site.Url}/booking/{site.SiteConfiguration.BookingJourneyConfiguration.PaymentUrl}?t={t}&warning=paymentfailed\");</script>");
			}
			catch (Exception ex)
			{
				this.logWriter.Write("OffsitePayment", "PaymentResponsePostError", ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException + "\n" + ex.TargetSite + ex.Data);
				return Content($"<script language='javascript' type='text/javascript'>window.location.replace(\"{site.Url}/booking/conditions?t={t}&warning=paymentfailed\");</script>");
			}
		}

		private bool VerifyProcessPaymentPostRequest()
		{
			return Request.Form["RESULT"] == "00";
		}
	}
}