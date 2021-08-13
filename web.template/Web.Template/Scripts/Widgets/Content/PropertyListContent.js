import '../../../styles/widgets/content/_propertylist.scss';
import ObjectFunctions from '../../library/objectfunctions';
import PropertyListItem from '../../components/content/propertylistItem';
import React from 'react';
import SelectInput from '../../components/form/selectinput';
import StringFunctions from '../../library/stringfunctions';

export default class PropertyListContent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    renderPropertyListItem(property, index) {
        let urlBase = `/${StringFunctions.safeUrl(this.props.destination)}`;
        urlBase += `/${StringFunctions.safeUrl(property.HotelCountry)}`;
        urlBase += `/${StringFunctions.safeUrl(property.HotelRegion)}`;
        urlBase += `/${StringFunctions.safeUrl(property.HotelName)}`;

        const url = urlBase;
        const geographies = {
            country: property.HotelCountry,
            region: property.HotelRegion,
            resort: property.HotelResort,
        };
        let address = `${geographies.resort}, ${geographies.region}`;
        switch (this.props.geographyLevel) {
            case 'region':
                address = `${geographies.resort}, ${geographies.country}`;
                break;
            case 'resort':
                address = geographies.country;
                break;
            default:
        }
        const hasSpecialOffer = !ObjectFunctions.isNullOrEmpty(property.SpecialOffer);
        const propertyProps = {
            key: `property-list-${index}`,
            address,
            hotelName: property.HotelName,
            hotelSummary: property.Summary,
            hotelImage: property.MainImage,
            country: geographies.country,
            region: geographies.region,
            resort: geographies.resort,
            price: parseFloat(property.FromPrice),
            starRating: property.HotelRating,
            url,
            fallbackPriceText: this.props.fallbackPriceText,
            currency: this.props.session.UserSession.SelectCurrency,
            siteConfiguration: this.props.siteConfiguration,
            chosenByUs: property.ChosenByUs === '1',
            duration: property.Duration,
            hasSpecialOffer: !ObjectFunctions.isNullOrEmpty(property.SpecialOffer),
            specialOffer: hasSpecialOffer ? property.SpecialOffer : {},
        };
        return (
            <PropertyListItem {...propertyProps} />
        );
    }
    renderOrderSelect() {
        const sortOptions = this.props.sortOptions;
        const sortProps = {
            name: 'Property Sort',
            options: sortOptions,
            fieldValue: 'displayValue',
            selectClass: 'sort-by-select',
            onChange: (event) => this.props.onSortChange(event.target.value),
        };
        return (
            <div className="row">
                <div className="col-sm-6 col-md-9"></div>
                <div className="col-xs-12 col-sm-6 col-md-3 sort-container">
                    <span className="sort-by-select-label">Sort By:</span>
                    <SelectInput {...sortProps} />
                </div>
            </div>
        );
    }
    render() {
        return (
            <div>
                {this.props.properties
                    && this.props.properties.length !== 0
                    && this.renderOrderSelect()}
                {this.props.properties
                    && this.props.properties.length > 0
                    && this.props.properties.map(this.renderPropertyListItem, this)}
                {this.props.properties
                    && this.props.properties.length === 0
                    && <h1>No properties found</h1>}
            </div>
        );
    }
}

PropertyListContent.propTypes = {
    properties: React.PropTypes.array,
    destination: React.PropTypes.string,
    geographyLevel: React.PropTypes.string,
    fallbackPriceText: React.PropTypes.string,
    session: React.PropTypes.object,
    siteConfiguration: React.PropTypes.object,
    sortOptions: React.PropTypes.array,
    selectedSort: React.PropTypes.string,
    onSortChange: React.PropTypes.func,
};
