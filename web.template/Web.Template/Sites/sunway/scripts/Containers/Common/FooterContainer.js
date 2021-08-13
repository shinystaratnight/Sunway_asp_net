import Footer from '../../widgets/footer';
import React from 'react';

export class FooterContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = this.props.entity.model;
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
    entity: React.PropTypes.object.isRequired,
};

export default FooterContainer;
