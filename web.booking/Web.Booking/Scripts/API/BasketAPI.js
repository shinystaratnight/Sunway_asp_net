import fetch from 'isomorphic-fetch';

class BasketAPI {
    constructor() {
        this.basket = {};
        this.promises = [];
    }

    /**
    * Method for sending a request with a data model
    * @param {string} url - The url to send to
    * @param {object} dataModel - The data model to send
    * @return {object} The return object
    */
    dataModelRequest(url, dataModel) {
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'same-origin',
            method: 'Post',
            body: JSON.stringify(dataModel),
        };
        return fetch(url, fetchOptions)
            .then(response => response.json())
            .then(basket => this.setupBasket(basket))
            .then(() => Promise.all(this.promises))
            .then(() => {
                this.promises = [];
                return this.basket;
            });
    }

    /**
     * Load CMS model
     * @param {object} objectType - The object type to return
     * @param {string} id - Id of the object type
     * @return {object} The model
     */
    loadCMSModel(objectType, id) {
        const entityUrl = `/booking/api/cms/${objectType}/${id}`;
        return fetch(entityUrl)
            .then(response => response.json())
            .then(result => JSON.parse(result.ContentJSON));
    }

    setupBasket(basket) {
        this.basket = basket;
        basket.Components.forEach((component, index) => {
            switch (component.ComponentType) {
                case 'Hotel':
                    this.setupHotelComponent(component, index);
                    break;
                default:
            }
        });
        return this.basket;
    }

    setupHotelComponent(component, index) {
        const updatedBasket = Object.assign({}, this.basket);
        const promise = this.loadCMSModel('Propertysummary', component.PropertyReferenceId)
            .then(result => {
                if (result.Property) {
                    updatedBasket.Components[index].content = result.Property;
                } else {
                    updatedBasket.Components[index].content = {
                        Region: '',
                        Resort: '',
                    };
                }
                this.basket = updatedBasket;
            });
        this.promises.push(promise);
    }


    /**
    * Method for creating a new basket
    * @return {string} The unique basket token for created basket
    */
    createBasket() {
        const basketCreateUrl = '/booking/api/basket/create';
        return fetch(basketCreateUrl)
            .then(response => response.json());
    }

    /**
    * Method for loading an existing basket
    * @param {string} token - The basket token
    * @return {object} The loaded basket
    */
    loadBasket(token) {
        const basketUrl = `/booking/api/basket/${token}`;
        return fetch(basketUrl, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(basket => {
                this.setupBasket(basket);
            })
            .then(() => Promise.all(this.promises))
            .then(() => {
                this.promises = [];
                return this.basket;
            });
    }

    /**
    * Method for adding a new component to the basket
    * @param {object} componentModel - The component model
    * @return {object} The updated basket
    */
    addComponent(componentModel) {
        const basketAddComponentUrl = '/booking/api/basket/component/add';
        return this.dataModelRequest(basketAddComponentUrl, componentModel);
    }

    /**
    * Method for adding a new component to the basket
    * @param {object} componentModel - The component model
    * @return {object} The updated basket
    */
    getComponentCancellationCharges(componentModel) {
        const basketGetChargesUrl = '/booking/api/basket/component/charges';
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'same-origin',
            method: 'Post',
            body: JSON.stringify(componentModel),
        };
        return fetch(basketGetChargesUrl, fetchOptions)
            .then(response => response.json());
    }

    /**
    * Method for updating a component on the basket
    * @param {string} basketToken - The basket token
    * @param {object} component - The component model
    * @return {object} The updated basket
    */
    updateComponent(basketToken, component) {
        const basketUpdateComponentUrl = '/booking/api/basket/component/flight/update';
        const model = {
            BasketToken: basketToken,
            ComponentToken: component.ComponentToken,
            SubComponents: component.SubComponents,
        };
        return this.dataModelRequest(basketUpdateComponentUrl, model);
    }

    /**
    * Method for adding a new component to the basket
    * @param {object} basket - The basket
    * @param {string} componentToken - The component token
    * @param {string} subcomponentToken - The sub component token
    * @param {number} quantity - The quantity of the extra to update to
    * @return {object} The updated basket
    */
    updateFlightExtra(basket, componentToken, subcomponentToken, quantity) {
        const updatedBasket = Object.assign({}, basket);

        const component = updatedBasket.Components.find(c => c.ComponentToken === componentToken);
        if (component) {
            const subcomponent
                = component.SubComponents.find(sc => sc.ComponentToken === subcomponentToken);
            if (subcomponent) {
                subcomponent.QuantitySelected = quantity;
                return this.updateComponent(updatedBasket.BasketToken, component);
            }
        }
        return updatedBasket;
    }

    /**
    * Method for removing a component from the basket
    * @param {object} componentModel - The component model
    * @return {object} The updated basket
    */
    removeComponent(componentModel) {
        const basketRemoveComponentUrl = '/booking/api/basket/component/remove';
        return this.dataModelRequest(basketRemoveComponentUrl, componentModel);
    }

    /**
    * Method for applying a promo code to the basket
    * @param {string} basketToken - The unique token of the basket to update
    * @param {string} promoCode - The promo code to apply
    * @return {object} The promo code return object
    */
    applyPromoCode(basketToken, promoCode) {
        const url = '/booking/api/basket/promocode/add';
        const changeBasketModel = {
            BasketToken: basketToken,
            PromoCode: promoCode,
        };

        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'same-origin',
            method: 'Post',
            body: JSON.stringify(changeBasketModel),
        };
        return fetch(url, fetchOptions)
            .then(response => response.json());
    }


    /**
    * Method for removing a promo code from the basket
    * @param {string} basketToken - The unique token of the basket to update
    * @return {object} The promo code return object
    */
    removePromoCode(basketToken) {
        const promoCodeAddUrl = '/booking/api/basket/promocode/remove';
        const changeBasketModel = {
            BasketToken: basketToken,
        };
        return this.dataModelRequest(promoCodeAddUrl, changeBasketModel);
    }

    /**
    * Method for prebooking a basket
    * @param {string} token - The unique token of the basket to update
    * @return {object} The prebook return object
    */
    preBookBasket(token) {
        const basketPreBookUrl = `/booking/api/basket/prebook/${token}`;
        return fetch(basketPreBookUrl, { credentials: 'same-origin' })
            .then(response => response.json());
    }

    /**
    * Method for booking a basket
    * @param {string} token - The unique token of the basket to update
    * @param {string} basket - the current client side basket
    * @param {string} request - the hotel request
    * @return {object} The prebook return object
    */
    bookBasket(token, basket, request) {
        const basketBookUrl = `/booking/api/basket/book/${token}`;
        const basketModel = {
            GuestDetails: basket.Rooms,
            TradeReference: basket.TradeReference,
            HotelRequest: request,
            LeadGuest: basket.LeadGuest,
            PaymentDetails: basket.PaymentDetails,
        };
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'same-origin',
            method: 'Post',
            body: JSON.stringify(basketModel),
        };
        return fetch(basketBookUrl, fetchOptions)
            .then(response => response.json());
    }

    /**
    * Method for releasing a flight seat lock
    * @param {string} basketToken - The unique token of the basket to update
    * @return {object} The response result
    */
    releaseFlightSeatLock(basketToken) {
        const url = `/booking/api/basket/releaseflightseatlock/${basketToken}`;
        return fetch(url, { credentials: 'same-origin' })
            .then(response => response);
    }
}

export default BasketAPI;
