import 'components/_propertyresult.scss';

import CarouselContent from 'widgets/common/CarouselContent';
import MarkdownText from 'components/common/MarkdownText';
import ObjectFunctions from 'library/objectfunctions';
import Price from 'components/common/price';
import RadioInput from 'components/form/radioinput';
import React from 'react';
import StringFunctions from 'library/stringfunctions';

export default class PropertyResult extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            cheapestRooms: this.getCheapestRooms(this.props.result),
            selectedSubComponentTokens: [],
        };
        this.handleRoomOptionSelect = this.handleRoomOptionSelect.bind(this);
        this.updateSelectedComponentTokens = this.updateSelectedComponentTokens.bind(this);

        if (this.isMultiRoomSearch()) {
            this.state.cheapestRooms.forEach((cheapestRoom, index) => {
                this.state.selectedSubComponentTokens[index] = cheapestRoom.ComponentToken;
            });
        }
    }
    componentWillReceiveProps(nextProps) {
        this.state.cheapestRooms = this.getCheapestRooms(nextProps.result);
    }
    isMultiRoomSearch() {
        return this.props.rooms.length > 1;
    }
    isCharterPackage() {
        return (this.props.selectedFlight
            && this.props.selectedFlight.MetaData.Source === 'Own')
            || (this.props.basketFlight && this.props.basketFlight.Source === 'Own');
    }
    getCarouselContentProps() {
        const carouselItems = [];
        this.props.result.MetaData.Images.forEach(image => {
            const item = {
                ImageUrl: image.Url,
            };
            carouselItems.push(item);
        });
        const carouselContentProps = {
            context: `property-carousel-${this.props.result.MetaData.PropertyReferenceId}`,
            carouselHeight: 'Extra Small',
            carouselItemType: 'ImagePanel',
            carouselItems,
            carouselTiles: 0,
            autoScroll: false,
            displayArrows: true,
            indicatorType: 'None',
        };
        return carouselContentProps;
    }
    getFromPrice() {
        let fromPrice = 0;
        this.state.cheapestRooms.forEach(room => {
            fromPrice += room ? room.TotalPrice : 0;
        });

        if (this.props.selectedFlight) {
            fromPrice += this.props.selectedFlight.Price;
        }

        fromPrice += this.props.adjustmentAmount ? this.props.adjustmentAmount : 0;
        return fromPrice;
    }
    getCheapestRooms(result) {
        const cheapestRooms = [];
        this.props.rooms.forEach(room => {
            const cheapestResult = result.SubResults.filter(subResult =>
                subResult.Display === true && subResult.Sequence === room.Id)[0];
            cheapestRooms.push(cheapestResult);
        });
        return cheapestRooms;
    }
    getSubComponent(roomIndex, componentToken) {
        const room = this.props.result.SubResults.find(subResult =>
            subResult.Sequence === roomIndex && subResult.ComponentToken === componentToken);
        return room;
    }
    getLocationDisplay(countryId, regionId, resortId) {
        let locationDisplay = '';
        this.props.countries.forEach(country => {
            if (country.Id === countryId) {
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
    getLinkUrl(result) {
        const url = window.location.pathname;
        const page = url.split('/')[2];
        let linkUrl = url.replace(`/booking/${page}`, '/booking/details');
        linkUrl = linkUrl.indexOf('?') === -1 ? `${linkUrl}?` : `${linkUrl}&`;
        linkUrl += `propertyId=${result.MetaData.PropertyReferenceId}`;
        linkUrl += `&h=${this.props.resultTokens.Hotel}`;
        if (this.props.resultTokens.Flight) {
            linkUrl += `&f=${this.props.resultTokens.Flight}`;
        }
        if (this.props.selectedFlight) {
            linkUrl += `&sf=${this.props.selectedFlight.ComponentToken}`;
        }
        if (this.props.basketFlight && this.props.basketToken) {
            linkUrl += `&t=${this.props.basketToken}`;
        }
        return linkUrl;
    }
    getMealBasisName(mealBasisId) {
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
    getRoomPaxSummary(room) {
        let paxSummary = StringFunctions.getPluralisationValue(room.Adults, 'Adult', 'Adults');
        paxSummary += room.Children > 0
            ? `, ${StringFunctions.getPluralisationValue(room.Children, 'Child', 'Children')}` : '';
        paxSummary += room.Infants > 0
            ? `, ${StringFunctions.getPluralisationValue(room.Infants, 'Infant', 'Infants')}` : '';
        return paxSummary;
    }
    handleRoomOptionSelect(event) {
        const field = event.target.name;
        const value = parseInt(event.target.value, 10);
        this.updateSelectedComponentTokens(field, value);
    }
    updateSelectedComponentTokens(field, value) {
        let updatedState = Object.assign({}, this.state);
        updatedState = ObjectFunctions.setValueByStringPath(updatedState, field, value);
        this.setState({ selectedSubComponentTokens: updatedState.selectedSubComponentTokens });
    }
    renderMultiRoomBook() {
        const resultComponentToken = this.props.result.ComponentToken;
        const buttonProps = {
            className: 'btn btn-default btn-sm',
            onClick: () => {
                this.props.onComponentAdd(resultComponentToken,
                    this.state.selectedSubComponentTokens,
                    this.props.result.MetaData.ArrivalDate, this.props.result.MetaData.Duration);
            },
        };

        let combinedTotal = 0;
        this.props.result.SubResults.forEach(subResult => {
            if (this.state.selectedSubComponentTokens.indexOf(subResult.ComponentToken) > -1) {
                combinedTotal += subResult.TotalPrice;
            }
        });

        if (this.props.selectedFlight) {
            combinedTotal += this.props.selectedFlight.Price;
        }

        if (this.props.basketFlight) {
            combinedTotal += this.props.basketFlight.TotalPrice;
        }

        const priceProps = {
            amount: combinedTotal,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            classes: 'price font-weight-bold',
            prependText: 'Combined Room Total ',
            inlinePrependText: true,
            displayTotalGroupPrice: false,
        };

        return (
            <div className="result-multi-room-book row mt-3">
                <div className="col-xs-12 col-md-9">
                   <Price {...priceProps} />
                </div>
                <div className="col-xs-12 col-md-3 text-right">
                    <button {...buttonProps}>Book Now</button>
                 </div>
            </div>
        );
    }
    renderSubResults(result) {
        return (
            <div className="result-options-container">
                <h2 className="h-tertiary">Room Options</h2>
                {this.props.rooms.map((room, index) =>
                    this.renderSubResultsRoom(room, index, result.SubResults), this)}
                {this.isMultiRoomSearch()
                    && this.renderMultiRoomBook()}
            </div>
        );
    }
    renderSubResultsRoom(room, index, subResults) {
        const roomSubResults = subResults.filter(subResult =>
            subResult.Display === true && subResult.Sequence === room.Id);
        const priceLabel = this.props.pricingConfiguration.PerPersonPricing
            ? 'Price PP' : 'Price';
        return (
            <div key={`options-room-${index}`} className="room-result-options mt-3">
                {this.isMultiRoomSearch()
                   && <h3>Room {room.Id} <small>- {this.getRoomPaxSummary(room)}</small></h3>}
                <div className="room-option-header">
                    <div className="room-col room-type">
                        <p>Room Type</p>
                    </div>
                    <div className="room-col room-meal-basis">
                        <p>Meal Basis</p>
                    </div>
                    <div className="room-col room-price text-right">
                        <p>{priceLabel}</p>
                    </div>
                    <div className="room-col room-book text-right hidden-xs">
                        <p>Select Room</p>
                    </div>
                </div>
                {roomSubResults.map((subResult, subResultIndex) =>
                    this.renderSubResult(index, subResult, subResultIndex), this)}
            </div>
        );
    }
    renderSubResult(roomIndex, subResult, index) {
        const resultComponentToken = this.props.result.ComponentToken;
        const selectedToken = this.state.selectedSubComponentTokens[roomIndex];
        const isSelected = selectedToken === subResult.ComponentToken;
        const isPackageMultiRoomSelected = (this.props.selectedFlight || this.props.basketFlight)
            && this.isMultiRoomSearch() && isSelected;

        let amount = subResult.TotalPrice;
        amount += this.props.selectedFlight ? this.props.selectedFlight.Price : 0;
        amount += this.props.adjustmentAmount ? this.props.adjustmentAmount : 0;

        if (this.props.basketFlight) {
            amount += this.props.basketFlight.TotalPrice;
        }

        const pricingConfig = Object.assign({}, this.props.pricingConfiguration);
        if (this.props.rooms.length >= subResult.Sequence - 1) {
            const room = this.props.rooms[subResult.Sequence - 1];
            if (room) {
                pricingConfig.NumberOfPeople = room.Adults + room.Children;
            }
        }

        const priceProps = {
            amount,
            currency: this.props.currency,
            pricingConfiguration: pricingConfig,
            classes: 'price font-weight-bold',
            displayTotalGroupPrice: false,
            displayPerPersonText: false,
            inlinePrependText: true,
            loading: this.props.updatingPrice,
        };

        if ((this.props.selectedFlight || this.props.basketFlight)
                && this.isMultiRoomSearch() && !isSelected) {
            const selectedSubResult = this.getSubComponent(roomIndex + 1, selectedToken);
            const priceDiff = subResult.TotalPrice - selectedSubResult.TotalPrice;
            if (priceDiff < 0) {
                priceProps.amount = priceDiff * -1;
                priceProps.prependText = '- ';
            } else if (priceDiff > 0) {
                priceProps.amount = priceDiff;
                priceProps.prependText = '+ ';
            } else {
                priceProps.amount = priceDiff;
            }
        }

        let bookInput = '';
        if (this.isMultiRoomSearch()) {
            const radioInputProps = {
                key: `selectedSubComponentTokens[${roomIndex}]`,
                name: `selectedSubComponentTokens[${roomIndex}]`,
                value: subResult.ComponentToken,
                checked: isSelected,
                onChange: this.handleRoomOptionSelect,
            };
            bookInput = <RadioInput {...radioInputProps} />;
        } else {
            const buttonProps = {
                className: 'btn btn-default btn-sm',
                onClick: () => {
                    this.props.onComponentAdd(resultComponentToken, [subResult.ComponentToken],
                    this.props.result.MetaData.ArrivalDate, this.props.result.MetaData.Duration);
                },
            };
            bookInput = <button {...buttonProps}>Book Now</button>;
        }

        const containerProps = {
            key: `${roomIndex}-${index}`,
            className: 'room-option',
        };

        if (isSelected) {
            containerProps.className += ' selected';
        }

        if (this.isMultiRoomSearch()) {
            containerProps.className += ' room-multiple';
            containerProps.onClick
                = () => this.updateSelectedComponentTokens(
                    `selectedSubComponentTokens[${roomIndex}]`, subResult.ComponentToken);
        }
        const hasRoomView = subResult.RoomView !== '' && subResult.RoomView !== 'No View';
        return (
            <div {...containerProps}>
                <div className="room-details">
                    <div className="room-col room-type">
                        <p>{subResult.RoomType}
                            {hasRoomView
                                && <span>{subResult.RoomView}</span>}
                        </p>
                    </div>
                    <div className="room-col room-meal-basis">
                        <p>{this.getMealBasisName(subResult.MealBasisId)}</p>
                    </div>
                    <div className="room-col room-price text-right">
                        {isPackageMultiRoomSelected
                            && <p>Selected</p>}
                        {!isPackageMultiRoomSelected
                            && <Price {...priceProps} />}
                    </div>
                    <div className="room-col room-book text-right hidden-xs">
                        {bookInput}
                    </div>
                </div>
                {!this.isMultiRoomSearch()
                    && <div className="room-book-xs hidden-sm-up">{bookInput}</div>}
            </div>
        );
    }
    renderFromPriceRoom(room) {
        const limit = this.props.roomTypeCharacterLimit;
        const cheapestRoomType = room.RoomType;
        const cheapestMealBasis = this.getMealBasisName(room.MealBasisId);
        const fromPriceRoom = `${cheapestRoomType}, ${cheapestMealBasis}`;
        const roomLabel = `Room ${room.Sequence} - `;
        const toShow = limit > 0 && fromPriceRoom.length > limit
                        ? `${fromPriceRoom.substring(0, limit)}...` : fromPriceRoom;
        return (
            <p key={`from-price-room-${room.Sequence}`} className="from-price-room">
                {this.isMultiRoomSearch()
                && roomLabel}
                {toShow}
            </p>
        );
    }
    renderSummary() {
        let readMore = '';
        switch (this.props.summaryBehaviour) {
            case 'Link': {
                const result = this.props.result;
                if (result.MetaData && result.MetaData.Summary) {
                    readMore = this.renderSummaryLink();
                }
                break;
            }
            case 'Popup':
                break;
            case 'Toggle': {
                const result = this.props.result;
                if (result.MetaData && result.MetaData.Summary) {
                    readMore = this.renderSummaryToggle();
                }
                break;
            }
            case 'Full': {
                if (this.props.content && this.props.content.Description) {
                    readMore = this.renderDescription();
                }
                break;
            }
            default:
        }
        return readMore;
    }
    renderSummaryLink() {
        const result = this.props.result;
        const linkUrl = `${this.getLinkUrl(result)}`;
        const summaryText = result.MetaData.Summary;
        const characterLimit = 300;
        const markdown = summaryText.length > characterLimit
                        ? `${summaryText.substr(0, characterLimit)}...`
                        : summaryText;

        const markdownProps = {
            markdown,
            containerStyle: 'result-summary',
        };
        return (
            <div className="hidden-xs">
                <MarkdownText {...markdownProps} />
                <a href={linkUrl} target="_blank" className="arrow-link">
                    Read More
                </a>
            </div>
        );
    }
    renderSummaryToggle() {
        const result = this.props.result;
        const summaryText = result.MetaData.Summary;
        const characterLimit = 300;
        const buttonText = result.DisplayExpandedSummary ? 'Show less' : 'Show more';

        const fullMarkdown = result.MetaData.Description
            ? `${summaryText} \n ${result.MetaData.Description}` : summaryText;
        const markdown
            = !result.DisplayExpandedSummary && summaryText.length > characterLimit
                ? `${summaryText.substr(0, characterLimit)}...`
                : fullMarkdown;
        const markdownProps = {
            markdown,
            containerStyle: 'result-summary',
        };
        return (
            <div className="hidden-xs">
                <MarkdownText {...markdownProps} />
                <span className="arrow-link"
                onClick={() => this.props.onToggleSummary(result.MetaData.PropertyReferenceId)}>
                    {buttonText}
                </span>
            </div>
        );
    }
    renderDescription() {
        const markdownProps = {
            markdown: this.props.content.Description,
            containerStyle: 'result-summary',
        };
        return (
            <MarkdownText {...markdownProps} />
        );
    }
    renderPaxSummary() {
        const paxTotal = {
            Adults: 0,
            Children: 0,
            Infants: 0,
        };

        this.props.rooms.forEach(room => {
            paxTotal.Adults += room.Adults;
            paxTotal.Children += room.Children;
            paxTotal.Infants += room.Infants;
        });

        return (
            <p className="pax-summary">{this.getRoomPaxSummary(paxTotal)}</p>
        );
    }
    renderMainImage() {
        const noImagePath = '/images/NoHotelImage.png';
        const backgroundImage = this.props.result.MetaData.MainImage
            ? `url('${this.props.result.MetaData.MainImage}')`
            : `url(${noImagePath})`;

        const backgroundDiv = {
            className: 'img-background',
            style: {
                backgroundImage,
            },
        };

        return (
            <div className="img-container">
                <div {...backgroundDiv}></div>
            </div>
        );
    }
    render() {
        const result = this.props.result;
        const countryId = result.MetaData.GeographyLevel1Id;
        const regionId = result.MetaData.GeographyLevel2Id;
        const resortId = result.MetaData.GeographyLevel3Id;

        const fromPrice = this.getFromPrice();

        const priceProps = {
            amount: fromPrice,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            prependText: 'From ',
            appendText: '',
            classes: 'price h-secondary h-alt mb-0',
            displayTotalGroupPrice: true,
            inlinePrependText: false,
            loading: this.props.updatingPrice,
        };

        const mobilePriceProps = Object.assign({}, priceProps);
        mobilePriceProps.classes = 'h-secondary';

        const linkUrl = `${this.getLinkUrl(this.props.result)}`;

        let containerClass = 'property-result';
        if (this.props.renderPaxSummary) {
            containerClass += ' display-pax';
        }
        if (this.isCharterPackage() && this.props.charterPackageText) {
            containerClass += ' display-text';
        }

        return (
            <div className={containerClass}>
                <div className="property-result-content">
                    <a href={linkUrl} className="property-link hidden-sm-up"></a>
                    {result.MetaData.Images.length > 0
                        && <CarouselContent {...this.getCarouselContentProps() }/>}
                    {result.MetaData.Images.length === 0
                        && this.renderMainImage()}
                    <div className="hotel-information">
                        <span className="fa fa-chevron-right hidden-sm-up"></span>
                        <h3 className="property-name">{result.DisplayName}</h3>
                        <p>{this.getLocationDisplay(countryId, regionId, resortId)}</p>
                        <span data-rating={result.MetaData.Rating} className="rating"></span>
                        {this.renderSummary()}
                        <div className="mobile-from-price hidden-sm-up">
                            <Price {...mobilePriceProps} />
                        </div>
                    </div>
                    <div className="from-price hidden-xs">
                        <Price {...priceProps} />

                        {this.props.renderPaxSummary
                           && this.renderPaxSummary()}

                        {this.isCharterPackage()
                            && this.props.charterPackageText
                            && <p>{this.props.charterPackageText}</p>}

                        {!this.props.renderSubResults
                            && this.props.rooms.length === 1
                            && this.state.cheapestRooms.map(this.renderFromPriceRoom, this)}
                        {!this.props.renderSubResults
                            && <a className="btn btn-alt" href={linkUrl}>Book Now</a>}
                    </div>
                </div>

                {this.props.renderSubResults
                    && this.renderSubResults(result)}
            </div>
        );
    }
}

PropertyResult.propTypes = {
    resultTokens: React.PropTypes.object.isRequired,
    rooms: React.PropTypes.array.isRequired,
    result: React.PropTypes.object.isRequired,
    content: React.PropTypes.object,
    countries: React.PropTypes.array.isRequired,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    mealBasis: React.PropTypes.array.isRequired,
    renderSubResults: React.PropTypes.bool.isRequired,
    selectedFlight: React.PropTypes.object,
    summaryBehaviour: React.PropTypes.oneOf(['None', 'Toggle', 'Popup', 'Link', 'Full']),
    onToggleSummary: React.PropTypes.func,
    onComponentAdd: React.PropTypes.func,
    renderPaxSummary: React.PropTypes.bool,
    charterPackageText: React.PropTypes.string,
    basketFlight: React.PropTypes.object,
    basketToken: React.PropTypes.string,
    roomTypeCharacterLimit: React.PropTypes.number,
    updatingPrice: React.PropTypes.bool,
    adjustmentAmount: React.PropTypes.number,
};
