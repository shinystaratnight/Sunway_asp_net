import fetch from 'isomorphic-fetch';

class DirectDebitAPI {
    constructor() {
        this.directDebits = [];
    }

    /**
    * Function for loading all bookings
    * @param {string} site - The site
    * @param {string} entityType - The entity type
    * @return {object} The contexts
    */
    loadAllDirectDebits() {
        const directDebitsURL = '/tradebookings/api/directdebits/retrieve';
        const fetchOptions = {
            credentials: 'same-origin',
        };
        return fetch(directDebitsURL, fetchOptions)
            .then(response => response.json())
            .then(response => {
                const updatedResponse = Object.assign({}, response);
                updatedResponse.BookingLine = response.BookingLine ? response.BookingLine : [];
                this.directDebits = updatedResponse.BookingLine;
                return updatedResponse;
            }
        );
    }
}

export default DirectDebitAPI;
