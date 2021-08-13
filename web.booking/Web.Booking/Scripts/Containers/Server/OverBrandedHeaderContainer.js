import OverBrandedHeader from 'widgets/common/overbrandedheader';
import React from 'react';

export default class OverBrandedHeaderContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const header = contentModel.Header;
        const headerProps = {
            logo: contentModel.Logo,
            header: header.Header,
            headerColour: header.HeaderColour,
            fontColour: header.FontColour,
            navigation: {
                links: header.Navigation.Links,
                backgroundColour: header.Navigation.BackgroundColour,
                fontColour: header.Navigation.fontColour,
                activeBackgroundColour: header.Navigation.ActiveBackgroundColour,
                activeFontColour: header.Navigation.ActiveFontColour,
            },
            contact: {
                text: contentModel.Contact.CallUsText,
                phoneNumber: contentModel.Contact.PhoneNumber,
            },
        };

        const containerProps = {
            className: 'widget widget-overbrandedheader',
            style: {
                backgroundColor: header.BackgroundColour,
            },
        };

        return (
            <div {...containerProps}>
                <OverBrandedHeader {...headerProps} />
            </div>
        );
    }
}

OverBrandedHeaderContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
};
