namespace Web.Template.Application.Interfaces.User
{
    using System;

    using Web.Template.Application.User.Enums;
    using Web.Template.Application.User.Models;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Entities.Site;

    /// <summary>
    /// Interface defining user session information
    /// </summary>
    public interface IUserSession
    {
        /// <summary>
        /// Gets or sets a value indicating whether [admin mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [admin mode]; otherwise, <c>false</c>.
        /// </value>
        bool AdminMode { get; set; }

        /// <summary>
        /// Gets or sets the admin session.
        /// </summary>
        /// <value>The admin session.</value>
        AdminSession AdminSession { get; set; }

        /// <summary>
        /// Gets or sets the a or b.
        /// </summary>
        /// <value>
        /// The a or b.
        /// </value>
        AorB AorB { get; set; }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        /// The brand identifier.
        /// </value>
        int BrandId { get; set; }

        /// <summary>
        /// Gets or sets the browser.
        /// </summary>
        /// <value>The browser.</value>
        string Browser { get; set; }

        /// <summary>
        /// Gets or sets the browser version.
        /// </summary>
        /// <value>The browser version.</value>
        int BrowserVersion { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include payment details].
        /// </summary>
        /// <value>
        /// <c>true</c> if [include payment details]; otherwise, <c>false</c>.
        /// </value>
        bool IncludePaymentDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [over branded].
        /// </summary>
        /// <value><c>true</c> if [over branded]; otherwise, <c>false</c>.</value>
        bool OverBranded { get; set; }

        /// <summary>
        /// Gets or sets the select currency.
        /// </summary>
        /// <value>
        /// The select currency.
        /// </value>
        Currency SelectCurrency { get; set; }

        /// <summary>
        /// Gets or sets the selected CMS website.
        /// </summary>
        /// <value>
        /// The selected CMS website.
        /// </value>
        CmsWebsite SelectedCmsWebsite { get; set; }

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        /// <value>
        /// The selected language.
        /// </value>
        Language SelectedLanguage { get; set; }

        /// <summary>
        /// Gets or sets the site builder authorization.
        /// </summary>
        /// <value>The site builder authorization.</value>
        string SiteBuilderAuthorization { get; set; }

        /// <summary>
        /// Gets or sets the trade session.
        /// </summary>
        /// <value>
        /// The trade session.
        /// </value>
        TradeSession TradeSession { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [logged in].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [logged in]; otherwise, <c>false</c>.
        /// </value>
        [Obsolete]
        Boolean LoggedIn { get; set; }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        void Logout();

        /// <summary>
        /// Logouts the admin.
        /// </summary>
        void LogoutAdmin();
    }
}