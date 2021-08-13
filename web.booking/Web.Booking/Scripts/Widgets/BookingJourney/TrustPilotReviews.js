import React from 'react';

export default class TrustPilotReviews extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    componentDidMount() {
        window.Trustpilot.loadFromElement(document.getElementById('trustbox'));
    }
    returnDataSKU() {
        let sku;
        if (this.props.OSReference === null) {
            sku = this.props.propertyId;
        } else {
            sku = `${this.props.propertyId},${this.props.OSReference}`;
        }
        return sku;
    }

    render() {
        return (
            <div className="trustpilot-widget"
                id="trustbox"
                data-locale="en-US"
                data-template-id="544a426205dc0a09088833c6"
                data-businessunit-id="577b81ab0000ff000591f393"
                data-style-height="400px"
                data-style-width="100%"
                data-theme="light"
                data-sku={`${this.returnDataSKU()}`}>
                <a href="https://www.trustpilot.com/review/sunway.ie" target="_blank">Trustpilot</a>
            </div>
        );
    }
}

TrustPilotReviews.propTypes = {
    propertyId: React.PropTypes.number,
    OSReference: React.PropTypes.string,
};
