import fetch from 'isomorphic-fetch';

class QuoteAPI {
    constructor() {
        this.basket = {};
        this.promises = [];
        this.success = false;
        this.warnings = [];
    }
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
    createQuote(basket) {
        const quoteUrl = `/booking/api/quote/create/${basket.BasketToken}`;
        const basketModel = {
            GuestDetails: basket.Rooms,
            TradeReference: basket.TradeReference,
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
        return fetch(quoteUrl, fetchOptions)
            .then(response => response.json())
            .then(createQuoteReturn => {
                this.success = createQuoteReturn.Success;
                return this.setupBasket(createQuoteReturn.Basket);
            })
            .then(() => Promise.all(this.promises))
            .then(() => {
                this.promises = [];
                return {
                    basket: this.basket,
                    success: this.success,
                    warnings: this.warnings,
                };
            });
    }
    emailQuote(model) {
        const quoteUrl = '/booking/api/quote/email';
        const quoteModel = {
            FlightToken: model.FlightToken,
            PropertyToken: model.PropertyToken,
            FlightSearchToken: model.FlightSearchToken,
            PropertySearchToken: model.PropertySearchToken,
            RoomOptions: model.RoomOptions,
            ToEmail: model.ToEmail,
            ToName: model.ToName,
            FromEmail: model.FromEmail,
            FromName: model.FromName,
            CCEmail: model.CCEmail,
        };
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'same-origin',
            method: 'Post',
            body: JSON.stringify(quoteModel),
        };
        return fetch(quoteUrl, fetchOptions)
            .then(response => response.json());
    }

    pdfQuote(model) {
        const pdfUrl = '/booking/api/quote/pdf';
        const quoteModel = {
            FlightToken: model.FlightToken,
            PropertyToken: model.PropertyToken,
            FlightSearchToken: model.FlightSearchToken,
            PropertySearchToken: model.PropertySearchToken,
            RoomOptions: model.RoomOptions,
            ToEmail: model.ToEmail,
            ToName: model.ToName,
            FromEmail: model.FromEmail,
            FromName: model.FromName,
        };
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'same-origin',
            method: 'Post',
            body: JSON.stringify(quoteModel),
        };
        return fetch(pdfUrl, fetchOptions)
            .then(response => response.json());
    }

    retrieveQuote(quoteReference) {
        const retrieveUrl = `/booking/api/quote/retrieve/${quoteReference}`;
        return fetch(retrieveUrl, { credentials: 'same-origin' })
            .then(response => response.json());
    }
}

export default QuoteAPI;
