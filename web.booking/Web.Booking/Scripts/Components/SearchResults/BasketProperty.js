import React from 'react';

export default class BasketProperty extends React.Component {
    render() {
        return (
            <div></div>
        );
    }
}

BasketProperty.propTypes = {
    property: React.PropTypes.object.isRequired,
    countries: React.PropTypes.array.isRequired,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    mealBasis: React.PropTypes.array.isRequired,
    selectedFlight: React.PropTypes.object,
};
