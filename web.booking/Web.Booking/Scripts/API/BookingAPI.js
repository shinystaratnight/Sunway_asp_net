import fetch from 'isomorphic-fetch';

class BookingAPI {
    constructor() {
        this.booking = {};
        this.promises = [];
    }

    /**
     * Load CMS model
     * @param {object} objectType - The object type to return
     * @param {string} id - Id of the object type
     * @return {object} The model
     */
    loadCMSModel(objectType, id) {
        let entityUrl = `/booking/api/cms/${objectType}/${id}`;
        if (document.getElementById('hidApiBaseUrl')) {
            const apiBaseUrl = document.getElementById('hidApiBaseUrl').value;
            entityUrl = `${apiBaseUrl}${entityUrl}`;
        }
        return fetch(entityUrl)
            .then(response => response.json())
            .then(result => JSON.parse(result.ContentJSON));
    }

    /**
     * Sets up the booking
     * @param {object} booking - The booking to set up
     * @return {object} The booking
     */
    setupBooking(booking) {
        this.booking = booking;
        booking.Components.forEach((component, index) => {
            switch (component.ComponentType) {
                case 'Hotel':
                    this.setupHotelComponent(component, index);
                    break;
                default:
            }
        });
        return this.booking;
    }

    /**
     * Sets up a hotel component
     * @param {object} component - The hotel component to set up
     * @param {number} index - The component index
     * @return {object} The hotel component
     */
    setupHotelComponent(component, index) {
        const updatedBooking = Object.assign({}, this.booking);
        const promise = this.loadCMSModel('Propertysummary', component.PropertyReferenceId)
            .then(result => {
                if (result.Property) {
                    updatedBooking.Components[index].content = result.Property;
                } else {
                    updatedBooking.Components[index].content = {
                        Region: '',
                        Resort: '',
                    };
                }
                this.booking = updatedBooking;
            });
        this.promises.push(promise);
    }

    /**
     * Retrieves a booking with the given reference
     * @param {string} reference - The booking reference to retrieve
     * @return {object} The booking
     */
    retrieveBooking(reference) {
        const url = `/booking/api/bookingdetails/${reference}`;
        return fetch(url, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(result => this.setupBooking(result.Booking))
            .then(() => Promise.all(this.promises))
            .then(() => {
                this.promises = [];
                return this.booking;
            });
    }
}

export default BookingAPI;
