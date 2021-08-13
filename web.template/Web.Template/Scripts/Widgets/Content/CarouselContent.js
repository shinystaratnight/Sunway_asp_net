import '../../../styles/widgets/content/_carouselcontent.scss';

import * as CarouselComponents from '../../components/carousel';
import React from 'react';
import StringFunctions from '../../library/stringfunctions';

export default class CarouselContent extends React.Component {
    constructor(props) {
        super(props);
        this.events = {
            INIT: 'INIT',
            POST_INIT: 'POST_INIT',
            NONE: 'NONE',
            START_TRANSITION: 'START_TRANSITION',
            IN_TRANSITION: 'IN_TRANSITION',
        };
        this.screenMdMin = 992;
        this.initCycleDuration = 5;
        this.itemTimeout = {};
        this.transitionTimeout = {};
        const currentEvent = props.autoScroll ? this.events.INIT : this.events.NONE;
        this.state = {
            activeItem: 0,
            carouselTiles: this.props.carouselTiles,
            previousItems: [],
            previousCloneItems: [],
            activeItems: [],
            cloneItems: [],
            currentEvent,
            direction: 'left',
            queueItem: -1,
            mobileDisplay: false,
            sidePreviewDisplay: this.props.sidePreview,
            touch: {
                isTouching: false,
                startX: 0,
                startY: 0,
            },
            initCycleStarted: false,
        };

        this.loadPreviousItem = this.loadPreviousItem.bind(this);
        this.loadNextItem = this.loadNextItem.bind(this);
        this.setActiveItems = this.setActiveItems.bind(this);
        this.getActiveItems = this.getActiveItems.bind(this);
        this.setTransition = this.setTransition.bind(this);
        this.renderCarouselItems = this.renderCarouselItems.bind(this);
        this.renderCarouselItem = this.renderCarouselItem.bind(this);
        this.renderCarouselCloneItem = this.renderCarouselCloneItem.bind(this);
        this.carouselItemClass = this.carouselItemClass.bind(this);
        this.renderIndicators = this.renderIndicators.bind(this);
        this.renderIndicator = this.renderIndicator.bind(this);
        this.isWrap = this.isWrap.bind(this);
        this.handleTouchStart = this.handleTouchStart.bind(this);
        this.handleTouchMove = this.handleTouchMove.bind(this);
        this.handleTouchEnd = this.handleTouchEnd.bind(this);
        this.handleResize = this.handleResize.bind(this);

        this.state.activeItems = this.getActiveItems(0);
    }
    componentDidMount() {
        if (this.props.autoScroll) {
            this.itemTimeout = setTimeout(this.loadNextItem, this.props.delay);
        }
        const mobileDisplay = window.innerWidth < this.screenMdMin;
        const sidePreviewDisplay = this.props.sidePreview
            || (this.props.sidePreviewMobile && mobileDisplay);
        const carouselTiles = sidePreviewDisplay
            || (mobileDisplay && this.props.carouselTiles > 1) ? 1 : this.props.carouselTiles;

        this.state.mobileDisplay = mobileDisplay;
        this.state.sidePreviewDisplay = sidePreviewDisplay;
        this.state.carouselTiles = carouselTiles;
        const activeItems = this.getActiveItems(0);
        this.setState({
            mobileDisplay,
            sidePreviewDisplay,
            carouselTiles,
            activeItems,
        });
        if (this.state.currentEvent === this.events.INIT) {
            setTimeout(() => {
                this.setState({ currentEvent: this.events.POST_INIT });
            }, this.initCycleDuration);
        }
        window.addEventListener('resize', this.handleResize);
    }
    componentDidUpdate() {
        const inTransitionDelay = 100;
        switch (this.state.currentEvent) {
            case this.events.START_TRANSITION:
                clearTimeout(this.transitionTimeout);
                setTimeout(() => {
                    this.setState({ currentEvent: this.events.IN_TRANSITION });
                    this.transitionTimeout = setTimeout(this.setTransition,
                        this.props.transitionDuration);
                }, inTransitionDelay);
                break;
            case this.events.INIT:
                this.state.queueItem = -1;
                break;
            case this.events.POST_INIT:
                if (!this.state.initCycleStarted) {
                    setTimeout(() => {
                        this.setState({ currentEvent: this.events.NONE });
                    }, this.props.delay);
                    this.state.initCycleStarted = true;
                }
                break;
            case this.events.NONE:
                if (this.state.queueItem > -1 && this.state.queueItem !== this.state.activeItem) {
                    setTimeout(() => {
                        this.setActiveItems(this.state.queueItem);
                    }, inTransitionDelay);
                } else {
                    this.state.queueItem = -1;
                }
                break;
            default:
        }
    }
    componentWillUnmount() {
        clearTimeout(this.itemTimeout);
        clearTimeout(this.transitionTimeout);
        window.removeEventListener('resize', this.handleResize);
    }
    handleResize() {
        const mobileDisplay = window.innerWidth < this.screenMdMin;
        const sidePreviewDisplay = this.props.sidePreview
            || (this.props.sidePreviewMobile && mobileDisplay);
        const carouselTiles = sidePreviewDisplay
            || (mobileDisplay && this.props.carouselTiles > 1) ? 1 : this.props.carouselTiles;

        if (this.state.mobileDisplay !== mobileDisplay) {
            this.setState({ mobileDisplay });
        }
        if (this.state.sidePreviewDisplay !== sidePreviewDisplay) {
            this.setState({ sidePreviewDisplay });
        }
        if (this.state.carouselTiles !== carouselTiles) {
            this.setState({ carouselTiles });
            this.setActiveItems(1);
        }
    }
    loadPreviousItem() {
        let nextIndex = this.state.activeItem - 1;
        if (nextIndex < 0) {
            nextIndex = this.props.carouselItems.length - 1;
        }
        this.setActiveItems(nextIndex);
    }
    loadNextItem() {
        let nextIndex = this.state.activeItem + 1;
        if (nextIndex > this.props.carouselItems.length - 1) {
            nextIndex = 0;
        }
        this.setActiveItems(nextIndex);
    }
    isWrap(index) {
        return ((this.state.activeItem === this.props.carouselItems.length - 1 && index === 0)
            || (this.state.activeItem === 0 && index === this.props.carouselItems.length - 1))
            && !this.state.sidePreviewDisplay;
    }
    createCloneItems(number, direction) {
        const cloneItems = [];
        const items = Object.assign([], this.state.activeItems);
        if (direction === 'right') {
            items.reverse();
        }
        for (let i = 0; i < number; i++) {
            const item = items[i];
            cloneItems.push(item);
        }
        return cloneItems;
    }
    setActiveItems(index) {
        if (this.state.currentEvent !== this.events.NONE) {
            this.state.queueItem = index;
            return;
        }
        clearTimeout(this.itemTimeout);
        clearTimeout(this.transitionTimeout);
        const currentItem = this.state.activeItem;
        const currentItems = this.state.activeItems;
        let scrollDirection = 'left';

        if ((currentItem > index
                && !(currentItem === this.props.carouselItems.length - 1 && index === 0))
                || (index === this.props.carouselItems.length - 1 && currentItem === 0)) {
            scrollDirection = 'right';
        }

        let cloneItems = [];
        if (this.state.carouselTiles > 0) {
            const previewTiles = 2;
            const requiredTiles = this.state.sidePreviewDisplay
                ? this.state.carouselTiles + previewTiles : this.state.carouselTiles;
            const spareItems = this.props.carouselItems.length - requiredTiles;
            const indexDiff = Math.abs(currentItem - index);
            if (!this.isWrap(index) && indexDiff > spareItems) {
                const numberOfClones = this.state.sidePreviewDisplay ? 1 : (indexDiff - spareItems);
                cloneItems = this.createCloneItems(numberOfClones, scrollDirection);
            }
        }

        const activeItems = this.getActiveItems(index);
        this.setState({
            previousItems: currentItems,
            previousCloneItems: [],
            activeItem: index,
            activeItems,
            cloneItems,
            currentEvent: this.events.START_TRANSITION,
            direction: scrollDirection,
        });
        if (this.props.autoScroll) {
            this.itemTimeout = setTimeout(this.loadNextItem, this.props.delay);
        }
    }
    getActiveItems(index) {
        const activeItems = [];
        const previewTiles = 2;
        const requiredTiles = this.state.sidePreviewDisplay
            ? this.state.carouselTiles + previewTiles : this.state.carouselTiles;
        if (this.state.carouselTiles > 0) {
            let firstTile = 0;
            for (let i = 1; i <= requiredTiles; i++) {
                let nextIndex = index + (i - 1);
                if (nextIndex > this.props.carouselItems.length - 1) {
                    nextIndex = firstTile;
                    firstTile++;
                }
                activeItems.push(nextIndex);
            }
        } else {
            activeItems.push(index);
        }
        return activeItems;
    }
    setTransition() {
        switch (this.state.currentEvent) {
            case this.events.START_TRANSITION:
                this.setState({ currentEvent: this.events.IN_TRANSITION });
                break;
            default:
                this.setState({
                    currentEvent: this.events.NONE,
                    previousCloneItems: this.state.cloneItems,
                    cloneItems: [],
                });
        }
    }
    renderCarouselItems() {
        const heightClass = StringFunctions.heightClassFromEnum(this.props.carouselHeight);
        const mobileHeightClass
                = StringFunctions.heightClassFromEnum(this.props.carouselHeightMobile);
        const carouselItemsAttributes = {
            className: `carousel-items ${heightClass} mobile-${mobileHeightClass}`,
        };

        return (
             <div {...carouselItemsAttributes}>
                {this.props.carouselItems.map(this.renderCarouselItem, this)}
            </div>
        );
    }
    renderCarouselTiledItems() {
        const heightClass = StringFunctions.heightClassFromEnum(this.props.carouselHeight);
        const mobileHeightClass
            = StringFunctions.heightClassFromEnum(this.props.carouselHeightMobile);
        const carouselItemsAttributes = {
            className: `carousel-items
                        tiles-${this.state.carouselTiles}
                        ${heightClass}
                        mobile-${mobileHeightClass}`,
        };

        const percentageMultiplier = 100;
        const rowAttributes = {
            style: {
                width: `${this.state.carouselTiles * percentageMultiplier}%`,
            },
        };
        return (
            <div {...carouselItemsAttributes}>
                <div {...rowAttributes}>
                    {this.state.direction === 'right'
                        && this.state.cloneItems.map(this.renderCarouselCloneItem, this)}
                    {this.props.carouselItems.map(this.renderCarouselItem, this)}
                    {this.state.direction === 'left'
                        && this.state.cloneItems.map(this.renderCarouselCloneItem, this)}
                </div>
           </div>
        );
    }
    renderCarouselItem(carouselItem, index) {
        const CarouselComponent = CarouselComponents[this.props.carouselItemType];
        const key = `${this.props.context}-item-${index}`;
        const itemClass = this.carouselItemClass(index, false);
        const carouselItemHeight = this.props.carouselHeight;
        const carouselItemHeightMobile = this.props.carouselHeightMobile;
        const hasBackgroundImage = !!this.props.backgroundImage;
        const carouselComponentProps = {
            carouselItemHeightMobile,
            carouselItemHeight,
            hasBackgroundImage,
            carouselItem,
            siteConfiguration: this.props.siteConfiguration,
        };
        return (
            <div key={key} className={itemClass}>
                <CarouselComponent {...carouselComponentProps} />
            </div>
        );
    }
    renderCarouselCloneItem(carouselItemIndex) {
        const CarouselComponent = CarouselComponents[this.props.carouselItemType];
        const carouselItem = this.props.carouselItems[carouselItemIndex];
        const key = `${this.props.context}-itemclone-${carouselItemIndex}`;
        const itemClass = this.carouselItemClass(carouselItemIndex, true);
        const carouselItemHeight = this.props.carouselHeight;
        const carouselItemHeightMobile = this.props.carouselHeightMobile;
        const hasBackgroundImage = !!this.props.backgroundImage;

        const carouselComponentProps = {
            carouselItemHeightMobile,
            carouselItemHeight,
            hasBackgroundImage,
            carouselItem,
            siteConfiguration: this.props.siteConfiguration,
        };
        return (
            <div key={key} className={itemClass}>
                <CarouselComponent {...carouselComponentProps} />
            </div>
        );
    }
    activeColumn(index, cloneItem) {
        let activeColumn = 0;
        switch (this.state.currentEvent) {
            case this.events.START_TRANSITION:
                if (this.state.previousItems.indexOf(index) !== -1 && !cloneItem) {
                    activeColumn = this.state.previousItems.indexOf(index) + 1;
                }
                break;
            default:
                if (this.state.activeItems.indexOf(index) !== -1
                        && (cloneItem || this.state.cloneItems.indexOf(index) === -1)) {
                    activeColumn = this.state.activeItems.indexOf(index) + 1;
                }
        }
        return activeColumn;
    }
    getOldItems(addCloneItems) {
        let oldItems = this.state.previousItems.filter((item) =>
            this.state.activeItems.indexOf(item) === -1);
        if (addCloneItems) {
            oldItems = oldItems.concat(this.state.cloneItems);
        }
        return oldItems;
    }
    getNewItems(addCloneItems) {
        let newItems = this.state.activeItems.filter((item) =>
            this.state.previousItems.indexOf(item) === -1);
        if (addCloneItems) {
            newItems = newItems.concat(this.state.cloneItems);
        }
        return newItems;
    }
    nextColumn(index, cloneItem) {
        let nextColumn = 0;

        const oldItems = this.getOldItems(this.state.direction === 'right');
        const newItems = this.getNewItems(this.state.direction === 'left');

        switch (this.state.currentEvent) {
            case this.events.START_TRANSITION:
                if (this.state.direction === 'left' && newItems.indexOf(index) !== -1
                     && (cloneItem || this.state.cloneItems.indexOf(index) === -1)) {
                    nextColumn = newItems.indexOf(index) + 1;
                }
                break;
            case this.events.IN_TRANSITION:
                if (oldItems.indexOf(index) !== -1 && this.state.direction === 'right'
                        && (!cloneItem || this.state.cloneItems.indexOf(index) === -1)) {
                    nextColumn = oldItems.indexOf(index) + 1;
                }
                break;
            default:
                nextColumn = 0;
        }
        return nextColumn;
    }
    prevColumn(index, cloneItem) {
        let prevColumn = 0;

        const oldItems = this.getOldItems(this.state.direction === 'left');
        const newItems = this.getNewItems(this.state.direction === 'right');

        switch (this.state.currentEvent) {
            case this.events.START_TRANSITION:
                if (this.state.direction === 'right' && newItems.indexOf(index) !== -1
                     && (cloneItem || this.state.cloneItems.indexOf(index) === -1)) {
                    prevColumn = newItems.indexOf(index) + 1;
                }
                break;
            case this.events.IN_TRANSITION:
                if (this.state.direction === 'left' && oldItems.indexOf(index) !== -1
                        && (!cloneItem || this.state.cloneItems.indexOf(index) === -1)) {
                    prevColumn = oldItems.indexOf(index) + 1;
                }
                break;
            default:
                prevColumn = 0;
        }
        return prevColumn;
    }
    isPreviewItem(index) {
        let isPreviewItem = false;
        const activeItemIndex = this.state.activeItems.indexOf(index);
        if (this.state.sidePreviewDisplay
            && (activeItemIndex === 0 || activeItemIndex === this.state.activeItems.length - 1)) {
            isPreviewItem = true;
        }
        return isPreviewItem;
    }
    carouselItemClass(index, cloneItem) {
        let itemClass = 'carousel-item';
        const activeColumn = this.activeColumn(index, cloneItem);
        const nextColumn = this.nextColumn(index, cloneItem);
        const prevColumn = this.prevColumn(index, cloneItem);

        if ((activeColumn + nextColumn + prevColumn > 0) || this.props.carouselItems.length === 1) {
            itemClass = `${itemClass} display`;
        }
        if (activeColumn > 0
                && !cloneItem
                && this.state.previousCloneItems.indexOf(index) !== -1
                && this.state.currentEvent === this.events.NONE) {
            itemClass = `${itemClass} no-transition`;
        }

        if (activeColumn > 0) {
            itemClass = `${itemClass} active active-${activeColumn}`;
        }

        if (nextColumn > 0) {
            itemClass = `${itemClass} next next-${nextColumn}`;
        }

        if (prevColumn > 0) {
            itemClass = `${itemClass} prev prev-${prevColumn}`;
        }

        if (this.state.currentEvent === this.events.INIT) {
            itemClass = `${itemClass} init`;
        }

        if (this.isPreviewItem(index)) {
            itemClass = `${itemClass} preview`;
        }

        if (this.state.carouselTiles > 0) {
            const gridColumns = 12;
            const gridTileColumns = Math.ceil(gridColumns / this.state.carouselTiles);
            itemClass = `${itemClass} col-xs-${gridTileColumns}`;
        } else {
            itemClass = `${itemClass} fluid`;
        }
        return itemClass;
    }
    renderIndicators() {
        const listClass = `carousel-indicators ${this.props.indicatorPosition.toLowerCase()}`;
        let containerClass = 'carousel-indicators-container';
        if (this.props.carouselItems.length <= this.state.carouselTiles) {
            containerClass += ' hidden-md-up';
        }
        return (
            <div className={containerClass}>
                <div className="container">
                     <ol className={listClass}>
                        {this.props.carouselItems.map(this.renderIndicator, this)}
                    </ol>
                 </div>
            </div>
        );
    }
    renderIndicator(carouselItem, index) {
        const key = `${this.props.context}-indicator-${index}`;
        let indicatorClass = 'carousel-indicator';
        if (index === this.state.activeItem) {
            indicatorClass = `${indicatorClass} active`;
        }
        return (
            <li key={key}
                className={indicatorClass}
                onClick={() => { this.setActiveItems(index); }}></li>
        );
    }
    renderControls() {
        const controls = [];
        const leftKey = `${this.props.context}-arrow-left`;
        const rightKey = `${this.props.context}-arrow-right`;

        const controlAttributes = {
            style: {},
        };

        if (this.props.arrowOffsetY && this.props.arrowOffsetY !== 0) {
            controlAttributes.style.marginTop = `${this.props.arrowOffsetY}px`;
        }

        controls.push(
            <div key={leftKey}
                 className="carousel-control left"
                 onClick={() => { this.loadPreviousItem(); }}
                 {...controlAttributes}>
                <span className="icon arrow-left"></span>
                <span className="hidden-sr">Previous</span>
            </div>
        );
        controls.push(
            <div key={rightKey}
                 className="carousel-control right"
                 onClick={() => { this.loadNextItem(); }}
                 {...controlAttributes}>
                <span className="icon arrow-right"></span>
                <span className="hidden-sr">Next</span>
            </div>
        );
        return controls;
    }
    handleTouchStart(event) {
        const touchEvent = event.touches[0];
        const touch = this.state.touch;
        touch.isTouching = true;
        touch.startX = touchEvent.screenX;
        touch.startY = touchEvent.screenY;
        this.setState({ touch });
    }
    handleTouchMove(event) {
        const touch = this.state.touch;
        if (touch.isTouching) {
            const touchEvent = event.touches[0];
            const differenceX = touch.startX - touchEvent.screenX;
            const differenceY = touch.startY - touchEvent.screenY;

            const difference = Math.abs(differenceX) - Math.abs(differenceY);
            const tolerance = 50;
            if (difference > tolerance) {
                touch.isTouching = false;

                const direction = differenceX > 0 ? 'left' : 'right';
                if (direction === 'left') {
                    this.loadNextItem();
                    this.setState({ touch });
                }
                if (direction === 'right') {
                    this.loadPreviousItem();
                    this.setState({ touch });
                }
            }
        }
    }
    handleTouchEnd() {
        const touch = this.state.touch;
        touch.isTouching = false;
        this.setState({ touch });
    }
    getContainerClass() {
        let containerClass = 'carousel-container';
        if (this.state.carouselTiles > 0 && this.state.sidePreviewDisplay === false) {
            containerClass = `${containerClass} tiles container`;
        } else {
            containerClass = `${containerClass} fluid`;
        }
        return containerClass;
    }
    getCarouselClass() {
        let carouselClass = `carousel ${this.props.carouselItemType.toLowerCase()}`;
        carouselClass = this.props.displayArrows
            ? `${carouselClass} arrows` : carouselClass;
        carouselClass = this.state.sidePreviewDisplay
            ? `${carouselClass} side-preview` : carouselClass;
        carouselClass = (this.state.carouselTiles === 0) || (this.props.backgroundImage !== '')
            ? `${carouselClass} full-screen` : carouselClass;

        if (this.props.indicatorType !== 'None') {
            if (this.props.carouselItems.length <= this.state.carouselTiles) {
                carouselClass = `${carouselClass} indicators-md-up`;
            } else {
                carouselClass = `${carouselClass} indicators`;
            }
        }
        return carouselClass;
    }
    render() {
        const carouselAttributes = {
            className: this.getCarouselClass(),
            onTouchStart: this.handleTouchStart,
            onTouchMove: this.handleTouchMove,
            onTouchEnd: this.handleTouchEnd,
        };

        return (
            <div className={this.getContainerClass()}>
                {this.props.title
                    && <h2 className="h-secondary">{this.props.title}</h2>}

                {this.props.leadParagraph
                    && <p className="preamble">
                            {this.props.leadParagraph}</p>}

                <div {...carouselAttributes}>
                    {this.props.carouselItems.length > 0
                            && this.state.carouselTiles === 0
                            && this.renderCarouselItems()}

                    {this.props.carouselItems.length > 0
                        && this.state.carouselTiles > 0
                        && this.renderCarouselTiledItems()}

                    { this.props.carouselItems.length > 1
                        && this.props.indicatorType !== 'None'
                        && this.renderIndicators()}

                    {this.props.displayArrows
                        && this.props.carouselItems.length > this.state.carouselTiles
                        && this.props.carouselItems.length > 1
                        && this.renderControls()}
                </div>

                {this.props.buttonText && this.props.buttonURL
                    && <div className="hero-center-btn center-block">
                        <a href={this.props.buttonURL}
                        className="btn btn-default">{this.props.buttonText}</a>
                    </div>}
            </div>
        );
    }
}

CarouselContent.propTypes = {
    context: React.PropTypes.string.isRequired,
    siteConfiguration: React.PropTypes.object.isRequired,
    title: React.PropTypes.string,
    leadParagraph: React.PropTypes.string,
    backgroundImage: React.PropTypes.string,
    carouselItemType: React.PropTypes.string.isRequired,
    carouselTiles: React.PropTypes.number.isRequired,
    carouselHeightMobile: React.PropTypes.string.isRequired,
    carouselHeight: React.PropTypes.string.isRequired,
    carouselItems: React.PropTypes.array.isRequired,
    autoScroll: React.PropTypes.bool,
    pauseOnHover: React.PropTypes.bool,
    delay: React.PropTypes.number,
    displayArrows: React.PropTypes.bool,
    displayArrowsOnMobile: React.PropTypes.bool,
    arrowOffsetY: React.PropTypes.number,
    indicatorType: React.PropTypes.oneOf(['None', 'Circles', 'Numbers']),
    indicatorPosition: React.PropTypes.oneOf(['Left', 'Center', 'Right']),
    sidePreview: React.PropTypes.bool,
    sidePreviewMobile: React.PropTypes.bool,
    transitionType: React.PropTypes.oneOf(['Fade', 'Scroll']),
    transitionDuration: React.PropTypes.number,
    transitionTiming: React.PropTypes.oneOf(['Linear',
        'Ease', 'Ease-in', 'Ease-out', 'Ease-in-out']),
    buttonText: React.PropTypes.string,
    buttonURL: React.PropTypes.string,
};

CarouselContent.defaultProps = {
    autoScroll: false,
    carouselHeight: 'Medium',
    carouselHeightMobile: 'Medium',
    pauseOnHover: false,
    delay: 5000,
    displayArrows: true,
    displayArrowsOnMobile: true,
    indicatorType: 'Circles',
    indicatorPosition: 'Center',
    sidePreview: false,
    sidePreviewMobile: false,
    transititonType: 'Scroll',
    transitionDuration: 1000,
    transitionTiming: 'Ease',
};
