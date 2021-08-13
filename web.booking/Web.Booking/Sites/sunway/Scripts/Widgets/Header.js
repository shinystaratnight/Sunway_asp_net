import '../../styles/widgets/_header.scss';
import React from 'react';

export default class Header extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            mobileMenuDisplay: false,
        };
        this.handleMobileMenuToggle = this.handleMobileMenuToggle.bind(this);
    }
    handleMobileMenuToggle() {
        const mobileMenuDisplay = !this.state.mobileMenuDisplay;
        this.setState({ mobileMenuDisplay });
    }
    renderNavigation() {
        const tradeName = this.props.user.TradeSession.Trade.Name;
        return (
            <nav className="header-nav-links navbar navbar-inverse hidden-sm-down">
                <h2 className="hidden-sr">Navigation</h2>
                <ul className="nav">
                    {this.props.navigationLinks.map(this.renderNavigationLink, this)}
                    <li key="nav-item-logout" className="nav-item">
                        <a className="nav-link" href="/login?logout=true">Logout</a>
                    </li>
                </ul>
                <p className="header-trade">Logged in as {tradeName}</p>
            </nav>
        );
    }
    renderNavigationLink(link, index) {
        return (
            <li key={`nav-item-${index}`} className="nav-item">
                <a className="nav-link" href={link.Url}>{link.Name}</a>
            </li>
        );
    }
    renderMobileNavigation() {
        return (
           <nav className="navbar navbar-inverse navbar-stacked header-mobile-menu hidden-md-up">
               <h2 className="hidden-sr">Navigation</h2>
               <ul className="nav">
                    {this.props.navigationLinks.map(this.renderNavigationLink, this)}
                    <li key="nav-item-logout" className="nav-item">
                        <a className="nav-link" href="/login?logout=true">Logout</a>
                    </li>
               </ul>
           </nav>
        );
    }
    renderOfferText() {
        return (
            <p className="offer-text hidden-sm-down">
                {this.props.offer.Text}
                <a className="offer-link" href={this.props.offer.Url}>more</a>
            </p>
        );
    }
    renderSocialMedia() {
        return (
            <nav className="header-nav-social hidden-sm-down">
                <h2 className="hidden-sr">Social Media</h2>
                <ul className="nav">
                    {this.props.socialMedia.map(this.renderSocialMediaItem, this)}
                </ul>
            </nav>
        );
    }
    renderSocialMediaItem(socialMediaItem, index) {
        return (
            <li key={`social-media-item-${index}`}
                className={`nav-item ${socialMediaItem.Type.toLowerCase()}`}>
                <a className="nav-link" href={socialMediaItem.Url} target="_blank">
                    {socialMediaItem.Type}
                </a>
            </li>
        );
    }
    renderContact() {
        const contact = this.props.contact;
        return (
            <p className="contact-number hidden-sm-down">
                {contact.Text}
                <a href={`tel:${contact.PhoneNumber}`}>{contact.PhoneNumber}</a>
            </p>
        );
    }
    renderMobileMenuLink() {
        let mobileLinkClass = 'header-mobile-link hidden-md-up';
        if (this.state.mobileMenuDisplay) {
            mobileLinkClass = `${mobileLinkClass} active`;
        }
        const mobileLinkIconClass = this.state.mobileMenuDisplay ? 'fa fa-times' : 'fa fa-bars';

        return (
            <span className={mobileLinkClass} onClick={this.handleMobileMenuToggle}>
                <i className={mobileLinkIconClass}></i>
            </span>
        );
    }

    render() {
        const containerAttributes = {
            className: 'header-container',
            style: {
                backgroundImage: `url('${this.props.bannerImage}')`,
            },
        };

        return (
            <div className="container">
                <div {...containerAttributes}>
                    <div className="header-brand">
                        {this.props.user.LoggedIn
                            && !this.props.user.AdminMode
                            && this.renderMobileMenuLink()}
                        <a className="header-logo" href={this.props.logoUrl}>
                            <img className="header-logo-img" src={this.props.logo} alt="Sunway" />
                        </a>
                    </div>

                    <div className="header-content">
                        {this.props.offer.Text
                            && this.renderOfferText()}

                        {this.props.socialMedia.length > 0
                            && this.renderSocialMedia()}

                        {this.props.contact.PhoneNumber
                            && this.renderContact()}

                        {this.props.user.LoggedIn
                            && !this.props.user.AdminMode
                            && this.renderNavigation()}
                    </div>
                </div>

                 {this.state.mobileMenuDisplay
                    && this.renderMobileNavigation()}

            </div>
        );
    }
}

Header.propTypes = {
    user: React.PropTypes.object,
    logo: React.PropTypes.string,
    logoUrl: React.PropTypes.string,
    bannerImage: React.PropTypes.string,
    offer: React.PropTypes.object,
    contact: React.PropTypes.object,
    navigationLinks: React.PropTypes.array,
    socialMedia: React.PropTypes.array,
    handleLogOut: React.PropTypes.func,
};
