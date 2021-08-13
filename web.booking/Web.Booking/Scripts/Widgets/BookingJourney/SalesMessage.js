import '../../../styles/widgets/bookingjourney/_salesMessage.scss';

import React from 'react';

export default class SalesMessage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const icon = `icon-${this.props.image}inverse-md salesMessageImage`;
        const image = icon.toLowerCase();
        const phoneNumber = this.props.phoneNumber;
        return (
            <div className="container">
                <div className="row slimRow">
                    <div className="col-xs-12 sales-Message-Box">
                        <span className={image}> </span>
                        <h2 className="h-inline salesMessage">
                            {this.props.message}
                            <span className="phoneNumber h-secondary">{phoneNumber}</span>
                        </h2>
                    </div>
                </div>
            </div>
        );
    }
}

SalesMessage.propTypes = {
    message: React.PropTypes.string,
    image: React.PropTypes.string,
    phoneNumber: React.PropTypes.string,
};
