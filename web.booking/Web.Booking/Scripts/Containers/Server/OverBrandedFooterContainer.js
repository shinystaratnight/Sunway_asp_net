import OverBrandedFooter from 'widgets/common/overbrandedfooter';
import React from 'react';

export default class OverBrandedFooterContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const footer = contentModel.Footer;
        const footerProps = {
            footerLinks: footer.Links,
            fontColour: footer.FontColour,
            contact: {
                phoneNumber: contentModel.Contact.PhoneNumber,
                email: contentModel.Contact.Email,
                address: contentModel.Contact.Address,
            },
            termsAndConditions: footer.Terms,
        };

        const containerProps = {
            className: 'widget widget-overbrandedfooter',
            style: {
                backgroundColor: footer.BackgroundColour,
            },
        };

        return (
            <div {...containerProps}>
                <OverBrandedFooter {...footerProps} />
            </div>
        );
    }
}

OverBrandedFooterContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
};
