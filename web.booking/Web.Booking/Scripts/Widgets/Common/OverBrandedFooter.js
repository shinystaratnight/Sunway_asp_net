import 'widgets/common/_overbrandedfooter.scss';
import React from 'react';

export default class Footer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    renderTopSection() {
        const footerLinksClass = 'col-xs-12 col-md-10 col-md-offset-1 col-lg-8 col-lg-offset-2';
        const detailProps = {
            style: {
                color: this.props.fontColour,
            },
        };
        return (
            <div className="footer-top-section alt">
                <div className="container">
                    <div className="row">
                        <div className={footerLinksClass}>
                            <nav>
                                <h2 className="hidden-sr">Footer Links</h2>
                                <ul className="nav nav-centered">
                                    {this.props.footerLinks.map(this.renderFooterLink, this)}
                                </ul>
                            </nav>
                            <div className="company-details">
                                <div className="detail-item">
                                    {this.props.contact.address
                                        && <address {...detailProps}>
                                            {this.props.contact.address}</address>}
                                </div>
                                <div className="detail-item">
                                    {this.props.contact.email
                                        && <a {...detailProps}
                                                href={`mailto:${this.props.contact.email}`}>
                                                {this.props.contact.email}</a>}
                                </div>
                                <div className="detail-item">
                                        {this.props.contact.phoneNumber
                                            && <a {...detailProps}
                                                href={`tel:${this.props.contact.phoneNumber}`}>
                                                {this.props.contact.phoneNumber}</a>}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
    renderFooterLink(footerLink, index) {
        const key = `footerLink-${index}`;
        const navLinkProps = {
            className: 'nav-link',
            style: {
                color: this.props.fontColour,
            },
            href: footerLink.Url,
        };
        return (
            <li className="nav-item" key={key}>
                <a {...navLinkProps}>{footerLink.Name}</a>
            </li>
        );
    }
    render() {
        return (
            <div className="footer-content">
                {this.renderTopSection()}
            <div className="footer-bottom-section">
                 <div className="container">
                    <div className="row">
                        <div className="col-xs-12">
                            {this.props.termsAndConditions
                                && <small>{this.props.termsAndConditions}</small>}
                        </div>
                    </div>
                </div>
            </div>
         </div>
        );
    }
}

Footer.propTypes = {
    footerLinks: React.PropTypes.array,
    fontColour: React.PropTypes.string,
    contact: React.PropTypes.shape({
        address: React.PropTypes.string,
        email: React.PropTypes.string,
        phoneNumber: React.PropTypes.string,
    }),
    termsAndConditions: React.PropTypes.string,
};
