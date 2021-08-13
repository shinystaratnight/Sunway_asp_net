import CarouselContent from '../../../../scripts/widgets/common/CarouselContent';
import GoogleMap from '../../../../scripts/components/common/googlemap';
import MarkdownText from '../../../../scripts/components/common/MarkdownText';
import ObjectFunctions from '../../../../scripts/library/objectfunctions';
import Price from '../../../../scripts/components/common/price';
import RadioInput from '../../../../scripts/components/form/radioinput';
import React from 'react';
import StringFunctions from '../../../../scripts/library/stringfunctions';
import moment from 'moment';

export default class PropertyResult extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            cheapestRooms: this.getCheapestRooms(this.props.result),
            selectedSubComponentTokens: [],
            activeCarouselIndex: 0,
        };
        this.renderChargeRow = this.renderChargeRow.bind(this);
        this.renderPaymentRow = this.renderPaymentRow.bind(this);
        this.updateCarouselItems = this.updateCarouselItems.bind(this);
        this.handleRoomOptionSelect = this.handleRoomOptionSelect.bind(this);
        this.updateSelectedComponentTokens = this.updateSelectedComponentTokens.bind(this);
        this.renderPaymentPlan = this.renderPaymentPlan.bind(this);
        if (this.isMultiRoomSearch()) {
            this.state.cheapestRooms.forEach((cheapestRoom, index) => {
                this.state.selectedSubComponentTokens[index] = cheapestRoom.ComponentToken;
            });
        }
    }
    componentWillReceiveProps(nextProps) {
        this.state.cheapestRooms = this.getCheapestRooms(nextProps.result);
    }
    checkFlightMetaData(property) {
        return this.props.selectedFlight
            && this.props.selectedFlight.hasOwnProperty('MetaData')
            && this.props.selectedFlight.MetaData.hasOwnProperty(property);
    }
    isMultiRoomSearch() {
        return this.props.rooms.length > 1;
    }
    isCharterPackage() {
        return (this.props.selectedFlight
            && this.props.selectedFlight.MetaData.Source === 'Own')
            || (this.props.basketFlight && this.props.basketFlight.Source === 'Own');
    }
    getCarouselItems() {
        const carouselItems = [];
        if (this.props.result.MetaData.hasOwnProperty('VideoCode')
            && this.props.result.MetaData.VideoCode !== '') {
            const item = {
                VideoCode: this.props.result.MetaData.VideoCode,
                OverrideItemType: 'VideoPanel',
            };
            carouselItems.push(item);
        }
        this.props.result.MetaData.Images.forEach(image => {
            const item = {
                ImageUrl: image.Url,
            };
            carouselItems.push(item);
        });

        return carouselItems;
    }
    updateCarouselItems(index) {
        this.setState({ activeCarouselIndex: index });
    }
    getCarouselContentProps() {
        const carouselItems = [];
        if (this.props.result.MetaData.hasOwnProperty('VideoCode')
            && this.props.result.MetaData.VideoCode !== ''
            && this.props.summaryBehaviour === 'Full') {
            const item = {
                VideoCode: this.props.result.MetaData.VideoCode,
                OverrideItemType: 'VideoPanel',
            };
            carouselItems.push(item);
        }
        this.props.result.MetaData.Images.forEach(image => {
            const item = {
                ImageUrl: image.Url,
            };
            carouselItems.push(item);
        });
        const carouselContentProps = {
            context: `property-carousel-${this.props.result.MetaData.PropertyReferenceId}`,
            carouselHeight: 'Extra Large',
            carouselItemType: 'ImagePanel',
            carouselItems,
            carouselTiles: 0,
            autoScroll: false,
            displayArrows: true,
            indicatorType: 'None',
            activeIndex: this.state.activeCarouselIndex,
            updateIndex: true,
        };
        return carouselContentProps;
    }
    getCarouselTileContentProps(tiles) {
        const carouselItems = [];
        if (this.props.result.MetaData.hasOwnProperty('VideoCode')
            && this.props.result.MetaData.VideoCode !== '') {
            const item = {
                ImageUrl: `https://img.youtube.com/vi/${this.props.result.MetaData.VideoCode}/default.jpg`,
            };
            carouselItems.push(item);
        }
        this.props.result.MetaData.Images.forEach(image => {
            const item = {
                ImageUrl: image.Url,
            };
            carouselItems.push(item);
        });
        const carouselTileContentProps = {
            context: `property-carousel-${this.props.result.MetaData.PropertyReferenceId}`,
            carouselHeight: 'Extra Small',
            carouselItemType: 'ImagePanel',
            carouselItems,
            carouselTiles: tiles,
            autoScroll: false,
            displayArrows: true,
            indicatorType: 'None',
            updateFunction: this.updateCarouselItems,
            updateIndex: false,
            customClass: 'hidden-sm hidden-xs',
        };
        return carouselTileContentProps;
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
        linkUrl += `&OSReference=${result.MetaData.OSReference}`;
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
    getRoomTypeSummary() {
        const roomTypes = [...new Set(this.state.cheapestRooms.map(o => o.RoomType))];
        let roomTypeSummary = '';
        for (let i = 0; i < roomTypes.length; i++) {
            const roomType = roomTypes[i];
            const number = this.state.cheapestRooms.filter(cr => cr.RoomType === roomType).length;
            roomTypeSummary += `${number}x ${roomType}`;
            if (i !== roomTypes.length - 1) {
                roomTypeSummary += ', ';
            }
        }
        return roomTypeSummary;
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
    shouldRenderCharges() {
        if (this.props.displayCancellationCharges) {
            if (this.props.payments && this.props.payments.length > 0
                || this.props.cancellationCharges && this.props.cancellationCharges.length > 0) {
                return true;
            }
        }
        return false;
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
        if (this.props.selectedFlight) {
            amount += this.props.selectedFlight.Price;
        }

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
                        this.props.result.MetaData.ArrivalDate,
                        this.props.result.MetaData.Duration);
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

        return (
            <div {...containerProps}>
                <div className="room-details">
                    <div className="room-col room-type">
                        <p>{subResult.RoomType}</p>
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
        const cheapestRoomType = room.RoomType;
        const cheapestMealBasis = this.getMealBasisName(room.MealBasisId);
        const fromPriceRoom = `${cheapestRoomType}, ${cheapestMealBasis}`;
        const roomLabel = `Room ${room.Sequence} - `;
        return (
            <p key={`from-price-room-${room.Sequence}`} className="from-price-room">
                {this.isMultiRoomSearch()
                    && roomLabel}
                {fromPriceRoom}
            </p>
        );
    }
    renderMap() {
        const mapProps = {
            coordinates: {
                Latitude: this.props.result.MetaData.Latitude,
                Longitude: this.props.result.MetaData.Longitude,
            },
            mapConfiguration: this.props.mapConfiguration,
        };
        return (<GoogleMap {...mapProps} />);
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
    renderPriceFrom(priceProps, linkUrl) {
        return (
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
            </div>);
    }
    renderCharges() {
        const resultComponentToken = this.props.result.ComponentToken;
        const onClick = {
            onHideClick: () => {
                this.props.onHideCancellationCharges(resultComponentToken);
            },
        };
        return (<div className="payment-plan">
            <table>
                <tr className="payment">
                    <th className="col-1">Payments Due</th>
                    <th className="col-2">
                        <a className="payment-close"
                            onClick={onClick.onHideClick}>Close</a>
                    </th>
                </tr>
                {this.props.payments.map(this.renderPaymentRow)}
                <tr className="cancellation-charge">
                    <th colSpan="2">Cancellation Charges</th>
                </tr>
                {this.props.cancellationCharges.map(this.renderChargeRow)}
            </table>
        </div>
        );
    }
    renderPaymentRow(payment) {
        const priceProps = {
            amount: payment.Amount,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            forceTotalGroupPrice: true,
            displayTotalGroupPrice: false,
            classes: 'payment-amount',
        };
        const isToday = moment(payment.DateDue).isSame(moment(), 'day');
        const paymentText = isToday
            ? 'Amount due Today'
            : `Balance due ${moment(payment.DateDue).format('DD MMM Y')}`;
        return (
            <tr className="payment">
                <td>{paymentText}</td>
                <td className="amount"><Price {...priceProps} /></td>
            </tr>
        );
    }
    renderChargeRow(charge) {
        const priceProps = {
            amount: charge.Amount,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            forceTotalGroupPrice: true,
            displayTotalGroupPrice: false,
            classes: 'payment-amount',
        };
        const isToday = moment(charge.StartDate).isSame(moment(), 'day');
        const startDate = isToday ? 'Today' : moment(charge.StartDate).format('DD MMM Y');
        const endsAfterDeparture = moment(charge.EndDate).isAfter(
            moment(this.props.searchDetails.DepartureDate));
        const endDate = endsAfterDeparture ? 'Onwards' : moment(charge.EndDate).format('DD MMM Y');

        let formattedLine = startDate;
        if (!endsAfterDeparture) {
            formattedLine += ' to';
        }
        formattedLine += ` ${endDate}`;

        return (
            <tr className="cancellation-charge">
                <td>{formattedLine}</td>
                <td className="amount"><Price {...priceProps} /></td>
            </tr>
        );
    }
    renderPrice(props) {
        const perPersonProps = props;
        perPersonProps.displayPerPersonText = false;
        perPersonProps.prependText = '';
        perPersonProps.pricingConfiguration.PerPersonPriceFormat = 'TwoDP';
        const totalPriceProps = {
            amount: props.amount,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            appendText: '',
            displayPerPersonText: false,
            displayTotalGroupPrice: false,
            inlinePrependText: false,
            forceTotalGroupPrice: true,
        };
        totalPriceProps.pricingConfiguration.PriceFormatDisplay = 'TwoDP';
        return (<table className="price-details">
            <tr className="top-row">
                <td>Per person price from</td>
                <td><Price {...perPersonProps} /></td>
            </tr>
            <tr className="bottom-row">
                <td>Total Price</td>
                <td className="total-price"><Price {...totalPriceProps} /></td>
            </tr>
        </table>
        );
    }
    renderPaymentPlan() {
        const subRoomTokens = this.state.cheapestRooms.map(cr => cr.ComponentToken);
        const resultComponentToken = this.props.result.ComponentToken;
        const onClick = {
            onShowClick: () => {
                this.props.onGetCancellationCharges(resultComponentToken,
                    subRoomTokens);
            },
        };
        return (
            <p className="included euro">Payment Plan
                <a onClick={onClick.onShowClick}>view plan</a>
                {this.shouldRenderCharges()
                    && this.renderCharges()}
            </p>
        );
    }
    renderWhatsIncluded(priceProps, linkUrl) {
        const cheapestRoom = this.state.cheapestRooms[0];
        const mealBasis = this.getMealBasisName(cheapestRoom.MealBasisId);
        const roomType = this.getRoomTypeSummary();
        const searchMode = this.props.searchDetails.SearchMode;
        let ownStockFlight = false;
        if (this.checkFlightMetaData('Source')) {
            ownStockFlight = this.props.selectedFlight.MetaData.Source === 'Own';
        }
        let containerClass = `whats-included ${searchMode}`;
        if (ownStockFlight) {
            containerClass += ' own-stock';
        }
        const isDetails = this.props.summaryBehaviour === 'Full';
        const customProps = priceProps;
        if (!isDetails) {
            customProps.pricingConfiguration.PerPersonPriceFormat = 'Rounded';
        }

        let baggageDescription = 'Baggage TBC';
        let hasBaggage = false;
        if (ownStockFlight) {
            hasBaggage = true;
            if (this.checkFlightMetaData('BaggageDescription')
                && this.props.selectedFlight.MetaData.BaggageDescription !== '') {
                baggageDescription = this.props.selectedFlight.MetaData.BaggageDescription;
            } else {
                baggageDescription = 'Checked Baggage pp';
            }
        } else if (this.checkFlightMetaData('IncludedBaggageAllowance')) {
            if (this.checkFlightMetaData('IncludedBaggageText')
                && this.props.selectedFlight.MetaData.IncludedBaggageText !== '') {
                hasBaggage = true;
                baggageDescription
               = this.props.selectedFlight.MetaData.IncludedBaggageText;
            } else if (this.props.selectedFlight.MetaData.IncludedBaggageAllowance !== 0) {
                if (this.checkFlightMetaData('IncludedBaggageWeight')) {
                    hasBaggage = true;
                    baggageDescription
                   = `${this.props.selectedFlight.MetaData.IncludedBaggageWeight}kg Checked Bag pp`;
                }
            }
        }

        return (
            <div className={containerClass}>
                {isDetails
                    && <h3>{"What's Included?"}</h3>}
                {searchMode !== 'Hotel'
                    && <p className="included flight">Return Flights</p>}
                <p className="included food">{mealBasis}</p>
                <p className="included bed">{roomType}</p>
                {searchMode !== 'Hotel'
                    && hasBaggage
                    && <p className="included bag">{baggageDescription}</p>}
                {ownStockFlight
                    && <p className="included bus">Resort Transfers & Rep</p>}
                {this.renderPaymentPlan()}
                {((!ownStockFlight
                    && hasBaggage) || searchMode === 'Hotel')
                    && <p className="included italic bus">Transfers are optional extras</p>}
                {!ownStockFlight
                    && !hasBaggage
                    && searchMode !== 'Hotel'
                    && <p className="included italic bus">
                        Baggage & Transfers are optional extras
                        </p>}
                {isDetails
                    && this.renderPrice(customProps)}
                {!isDetails
                    && <Price {...customProps} />}
                {!isDetails
                    && <a className="details" href={linkUrl}>Details</a>}
            </div>
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

        let fromPrice = 0;
        this.state.cheapestRooms.forEach(room => {
            fromPrice += room.TotalPrice;
        });

        if (this.props.selectedFlight) {
            fromPrice += this.props.selectedFlight.Price;
        }

        if (this.props.basketFlight) {
            fromPrice += this.props.basketFlight.TotalPrice;
        }
        const priceProps = {
            amount: fromPrice,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            prependText: 'From ',
            appendText: '',
            displayTotalGroupPrice: false,
            inlinePrependText: false,
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
        const propertyLocation = this.getLocationDisplay(countryId, regionId, resortId);
        const tiles = 4;

        return (
            <div className={containerClass}>
                <div className="property-result-content">
                    <div className="results-image">
                        {result.MetaData.Images.length > 0
                            && <div>
                                <CarouselContent {...this.getCarouselContentProps()} />
                                {this.props.summaryBehaviour === 'Full'
                                    && <CarouselContent
                                        {...this.getCarouselTileContentProps(tiles)} />}
                            </div>}
                        {result.MetaData.Images.length === 0
                            && this.renderMainImage()}
                        {this.props.summaryBehaviour !== 'Full'
                            && this.props.result.MetaData.ProductAttributes.length > 0
                            && <div className="hotel-attributes">
                                <p>
                                    {this.props.result.MetaData.ProductAttributes.map(o =>
                                        o).join(', ')}
                                </p>
                            </div>}
                    </div>
                    <div className="hotel-information">
                        <div className="Info">
                            <h3 className="property-name">{result.DisplayName}</h3>
                            <span data-rating={result.MetaData.Rating} className="rating"></span>
                            <p className="property-location">{propertyLocation}</p>
                        </div>
                        {this.props.displayQuoteEmail === true
                            && <div className="QuoteEmail">
                                <i className="fa fa-envelope" onClick={() => {
                                    this.props.onQuoteShow(result.ComponentToken,
                                        this.getCheapestRooms(this.props.result));
                                }
                                }></i>
                                <span className="info-tooltip">
                                    {this.props.quoteEmailTooltip}
                                </span>
                            </div>}
                        {this.props.summaryBehaviour === 'Full'
                            && this.renderSummary()}
                    </div>
                    {this.renderWhatsIncluded(priceProps, linkUrl)}
                    {this.props.summaryBehaviour === 'Full'
                        && this.renderMap()}
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
    searchDetails: React.PropTypes.object,
    onGetCancellationCharges: React.PropTypes.func,
    onHideCancellationCharges: React.PropTypes.func,
    displayCancellationCharges: React.PropTypes.bool,
    cancellationCharges: React.PropTypes.array,
    payments: React.PropTypes.array,
    displayQuoteEmail: React.PropTypes.bool,
    quoteEmailTooltip: React.PropTypes.string,
    onQuoteShow: React.PropTypes.func,
    mapConfiguration: React.PropTypes.object.isRequired,
};
