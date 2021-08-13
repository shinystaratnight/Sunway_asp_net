import Footer from '../../widgets/footer';
import React from 'react';

export default class FooterContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const footerProps = {
            footerLinks: contentModel.FooterLinks,
            contactAddress: contentModel.ContactAddress,
            contactEmail: contentModel.ContactEmail,
            contactNumber: contentModel.ContactNumber,
            termsAndConditions: contentModel.TermsAndConditions,
        };
        return (
            <div className="widget widget-footer-container padding-none">
                <Footer {...footerProps} />
            </div>
        );
    }
}

FooterContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
};
