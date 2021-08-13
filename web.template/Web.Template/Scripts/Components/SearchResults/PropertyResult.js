import Price from './../common/price';
import React from 'react';

export default class PropertyResult extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    isMultiRoomSearch() {
        return this.props.rooms.length > 1;
    }
    formatPrice(price) {
        const parts = price.toFixed(2).toString().split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join('.');
    }
    locationDisplay(countryId, regionId, resortId) {
        let locationDisplay = '';
        this.props.countries.forEach(country => {
            if (country.Id === regionId) {
                country.Regions.forEach(region => {
                    if (region.Id === regionId) {
                        region.Resorts.forEach(resort => {
                            if (resort.Id === resortId) {
                                locationDisplay = `${resort.Name}, ${region.Name}`;
                            }
                        });
                    }
                });
            }
        });
        return locationDisplay;
    }
    mealBasisName(mealBasisId) {
        let name = '';
        for (let i = 0; i < this.props.mealBasis.length; i++) {
            const mealBasis = this.props.mealBasis[i];
            if (mealBasis.Id === mealBasisId) {
                name = mealBasis.Name;
                break;
            }
        }
        return name;
    }
    renderSubResult(subResult, index) {
        return (
            <tr key={index}>
                <td className="roomtype">{subResult.RoomType}</td>
                <td className="mealbasis">{this.mealBasisName(subResult.MealBasisId)}</td>
                <td className="price">£{this.formatPrice(subResult.TotalPrice)}</td>
                <td className="book">
                    <button className="btn btn-default btn-sm">Book Now</button>
                </td>
            </tr>
        );
    }
    renderFromPriceRoom(room) {
        const cheapestRoomType = room.RoomType;
        const cheapestMealBasis = this.mealBasisName(room.MealBasisId);
        const fromPriceRoom = `${cheapestRoomType}, ${cheapestMealBasis}`;
        const roomLabel = `Room ${room.Sequence} - `;
        return (
            <p key={room.Sequence} className="from-price-room">
                {this.isMultiRoomSearch()
                && roomLabel}
                {fromPriceRoom}
            </p>
        );
    }
    renderSubResults(result) {
        return this.props.rooms.map(room =>
            this.renderSubResultsRoom(room, result.SubResults), this);
    }
    renderSubResultsRoom(room, subResults) {
        const roomSubResults = subResults.filter(subResult =>
            subResult.Display === true && subResult.Sequence === room.Id);
        return (
            <div className="search-result-options">
                {this.isMultiRoomSearch()
                    && <h3>Room {room.Id}</h3>}
                <table className="table">
                    <thead>
                        <tr>
                            <th>Room Type</th>
                            <th>Meal Basis</th>
                            <th className="price">Price</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {roomSubResults.map(this.renderSubResult, this)}
                    </tbody>
                </table>
            </div>
        );
    }
    render() {
        const result = this.props.result;
        const countryId = result.MetaData.GeographyLevel1Id;
        const regionId = result.MetaData.GeographyLevel2Id;
        const resortId = result.MetaData.GeographyLevel3Id;

        const cheapestRooms = [];
        this.props.rooms.forEach(room => {
            const cheapestResult = result.SubResults.filter(subResult =>
                subResult.Display === true && subResult.Sequence === room.Id)[0];
            cheapestRooms.push(cheapestResult);
        });

        let fromPrice = 0;
        cheapestRooms.forEach(room => {
            fromPrice += room.TotalPrice;
        });

        if (this.props.selectedFlight) {
            fromPrice += this.props.selectedFlight.Price;
        }

        const priceProps = {
            amount: fromPrice,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            prependText: 'From ',
            appendText: '',
            classes: 'h-secondary h-alt',
        };

        const backgroundDiv = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${result.MetaData.MainImage}')`,
            },
        };

        return (
            <div className="property-result">
                <div className="img-container img-zoom-hover">
                    <div {...backgroundDiv}></div>
                </div>
                <div className="hotel-information">
                    <h3>{result.DisplayName}</h3>
                    <p>{this.locationDisplay(countryId, regionId, resortId)}</p>
                    <span data-rating={result.MetaData.Rating} className="rating"></span>
                    <p className="result-summary">{result.MetaData.Summary}</p>
                    <a href="/" target="_blank" className="arrow-link">Read More</a>
                    {cheapestRooms.map(this.renderFromPriceRoom, this)}
                </div>
                <div className="from-price">
                    <Price {...priceProps} />
                    <button className="btn btn-alt">Book Now</button>
                </div>
                {this.props.renderSubResults
                    && this.renderSubResults(result)}
            </div>
        );
    }
}

PropertyResult.propTypes = {
    rooms: React.PropTypes.array.isRequired,
    result: React.PropTypes.object.isRequired,
    countries: React.PropTypes.array.isRequired,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    mealBasis: React.PropTypes.array.isRequired,
    renderSubResults: React.PropTypes.bool.isRequired,
    selectedFlight: React.PropTypes.object,
};
