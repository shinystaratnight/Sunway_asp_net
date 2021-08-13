import '../../styles/widgets/_footer.scss';
import React from 'react';

export default class Footer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    renderTopSection() {
        const footerLinksClass = 'col-xs-12 col-md-10 col-md-offset-1 col-lg-8 col-lg-offset-2';
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
                                    {this.props.contactAddress
                                        && <address>{this.props.contactAddress}</address>}
                                </div>
                                <div className="detail-item">
                                    {this.props.contactEmail
                                        && <a href={`mailto:${this.props.contactEmail}`}>
                                                {this.props.contactEmail}</a>}
                                </div>
                                <div className="detail-item">
                                        {this.props.contactNumber
                                            && <a href={`tel:${this.props.contactNumber}`}>
                                                {this.props.contactNumber}</a>}
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
        return (
            <li className="nav-item" key={key}>
                <a className="nav-link" href={footerLink.Url}>{footerLink.Name}</a>
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
    contactAddress: React.PropTypes.string,
    contactEmail: React.PropTypes.string,
    contactNumber: React.PropTypes.string,
    termsAndConditions: React.PropTypes.string,
};
