import 'widgets/common/_overbrandedheader.scss';
import React from 'react';

export default class OverBrandedHeader extends React.Component {
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
        return (
            <nav className="header-nav-links navbar navbar-inverse hidden-sm-down">
                <h2 className="hidden-sr">Navigation</h2>
                <ul className="nav">
                    {this.props.navigation.links.map(this.renderNavigationLink, this)}
                </ul>
            </nav>
        );
    }
    renderNavigationLink(link, index) {
        const navLinkProps = {
            className: 'nav-link',
            style: {
                background: this.props.navigation.backgroundColour,
                color: this.props.navigation.fontColour,
            },
            href: link.Url,
        };
        return (
            <li key={`nav-item-${index}`} className="nav-item">
                <a {...navLinkProps}>{link.Name}</a>
            </li>
        );
    }
    renderMobileNavigation() {
        return (
           <nav className="navbar navbar-inverse navbar-stacked header-mobile-menu hidden-md-up">
               <h2 className="hidden-sr">Navigation</h2>
               <ul className="nav">
                    {this.props.navigation.links.map(this.renderNavigationLink, this)}
               </ul>
           </nav>
        );
    }
    renderHeaderText() {
        const headerTextProps = {
            className: 'header-text hidden-sm-down',
            style: {
                color: this.props.headerColour,
            },
        };
        return (
            <p {...headerTextProps}>{this.props.header}</p>
        );
    }
    renderContact() {
        const contact = this.props.contact;
        const contactProps = {
            className: 'contact-number hidden-sm-down',
            style: {
                color: this.props.fontColour,
            },
        };
        return (
            <p {...contactProps}>
                {contact.text}
                <a href={`tel:${contact.phoneNumber}`}>{contact.phoneNumber}</a>
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
        };

        return (
            <div className="container">
                <div {...containerAttributes}>
                    <div className="header-brand">
                        {this.renderMobileMenuLink()}
                        <a className="header-logo" href="/">
                            <img className="header-logo-img" src={this.props.logo} alt="" />
                        </a>
                    </div>

                    <div className="header-content">
                        {this.props.header
                            && this.renderHeaderText()}

                        {this.props.contact.phoneNumber
                            && this.renderContact()}

                        {this.renderNavigation()}
                    </div>
                </div>

                 {this.state.mobileMenuDisplay
                    && this.renderMobileNavigation()}

            </div>
        );
    }
}

OverBrandedHeader.propTypes = {
    logo: React.PropTypes.string,
    header: React.PropTypes.string,
    headerColour: React.PropTypes.string,
    fontColour: React.PropTypes.string,
    navigation: React.PropTypes.shape({
        links: React.PropTypes.array,
        backgroundColour: React.PropTypes.string,
        fontColour: React.PropTypes.string,
        activeBackgroundColour: React.PropTypes.string,
        activeFontColour: React.PropTypes.string,
    }),
    contact: React.PropTypes.shape({
        text: React.PropTypes.string,
        phoneNumber: React.PropTypes.string,
    }),
};
