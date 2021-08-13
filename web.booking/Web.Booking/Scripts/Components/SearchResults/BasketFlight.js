import React from 'react';

export default class BasketFlight extends React.Component {
    render() {
        return (
            <div></div>
        );
    }
}

BasketFlight.propTypes = {
    flight: React.PropTypes.object.isRequired,
    airports: React.PropTypes.array.isRequired,
    flightCarriers: React.PropTypes.array.isRequired,
    flightClasses: React.PropTypes.array.isRequired,
    cmsBaseUrl: React.PropTypes.string.isRequired,
};
