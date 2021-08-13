import fetch from 'isomorphic-fetch';

class MMBAPI {
    constructor() {
        this.bookings = [];
    }

    /**
    * Function for loading all bookings
    * @param {string} site - The site
    * @param {string} entityType - The entity type
    * @return {object} The contexts
    */
    loadAllBookings() {
        const bookingsURL = '/tradebookings/api/bookingdetails/Search';
        this.bookings = [];
        const fetchOptions = {
            credentials: 'same-origin',
        };
        return fetch(bookingsURL, fetchOptions)
            .then(response => response.json())
            .then(response => {
                this.bookings = response.Bookings ? response.Bookings : [];
                return this.bookings;
            }
        );
    }

    /**
    * Function for updating the filters
    * @param {object} filters - The filters
    * @param {object} filter - The filter
    * @return {object} The contexts
    */
    updateFilters(filters, filter) {
        const replacementFilter = {};
        replacementFilter[filter.name] = filter;
        const newFilters = Object.assign({}, filters, replacementFilter);
        return newFilters;
    }
    filterText(filter, bookings) {
        const filteredBookings = [];
        bookings.forEach(b => {
            const filteredBooking = b;
            filteredBooking.display = filteredBooking.display
                ? filteredBooking[filter.field].toLowerCase()
                    .indexOf(filter.value.toLowerCase()) !== -1
                : filteredBooking.display;
            filteredBookings.push(filteredBooking);
        });
        return filteredBookings;
    }
    filterMultifieldText(filter, bookings) {
        const filteredBookings = [];
        bookings.forEach(b => {
            const filteredBooking = b;
            let comparisonText = '';
            filter.fields.forEach(f => {
                comparisonText += `${filteredBooking[f]} `;
            });
            filteredBooking.display = filteredBooking.display
                ? comparisonText.toLowerCase().indexOf(filter.value.toLowerCase()) !== -1
                : filteredBooking.display;
            filteredBookings.push(filteredBooking);
        });
        return filteredBookings;
    }
    filterDate(filter, bookings) {
        const filteredBookings = [];
        bookings.forEach(b => {
            const filteredBooking = b;
            const comparisonValue = b[filter.field];
            filteredBooking.display = filteredBooking.display
                ? comparisonValue > filter.startDate
                : filteredBooking.display;
            filteredBooking.display = filteredBooking.display
                ? comparisonValue < filter.endDate
                : filteredBooking.display;
            filteredBookings.push(filteredBooking);
        });
        return filteredBookings;
    }

    /**
    * Function for filtering the bookings
    * @param {array} bookings - The bookings
    * @param {object} filters - The filters
    * @return {object} The contexts
    */
    filterBookings(bookings, filters) {
        const filterkeys = Object.keys(filters);
        let filteredBookings = bookings;
        for (let i = 0; i < filteredBookings.length; i++) {
            filteredBookings[i].display = true;
        }
        filterkeys.forEach(fk => {
            const filter = filters[fk];
            if (filter.value !== '') {
                switch (filter.type) {
                    case 'text':
                        filteredBookings = this.filterText(filter, filteredBookings);
                        break;
                    case 'multifieldText':
                        filteredBookings = this.filterMultifieldText(filter, filteredBookings);
                        break;
                    case 'date':
                        if (filter.selectedOption !== 'Any') {
                            filteredBookings = this.filterDate(filter, filteredBookings);
                        }
                        break;
                    default:
                }
            }
        });
        return filteredBookings;
    }
    getPreCancellationComponents(bookingReference, bookings) {
        const preCancelComponentsUrl
            = `/tradebookings/api/bookingdetails/precancelcomponent/${bookingReference}`;
        const booking = bookings.filter(b => b.BookingReference === bookingReference)[0];
        const componentBody = [];
        booking.ComponentList.forEach(c => {
            const componentRequest = {
                ComponentBookingId: c.ComponentBookingID,
                Type: c.ComponentType.replace('Booking', ''),
            };
            componentBody.push(componentRequest);
        });
        const componentFetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'POST',
            credentials: 'same-origin',
            body: JSON.stringify(componentBody),
        };

        return fetch(preCancelComponentsUrl, componentFetchOptions)
            .then(response => response.json());
    }
    getPreCancellation(bookingReference) {
        const preCancelBookingUrl
            = `/tradebookings/api/bookingdetails/precancel/${bookingReference}`;
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'GET',
            credentials: 'same-origin',
        };

        return fetch(preCancelBookingUrl, fetchOptions)
            .then(response => response.json());
    }
    getCancellationDetails(bookingReference, bookings) {
        const promises = [];
        promises.push(this.getPreCancellationComponents(bookingReference, bookings));
        promises.push(this.getPreCancellation(bookingReference, bookings));
        return (
            Promise.all(promises))
            .then(results => {
                const cancellationInformation = {
                    components: results[0].CancellationComponents,
                    all: results[1],
                };
                return cancellationInformation;
            });
    }
    cancelBooking(bookingReference, info) {
        const queryString
            = `cancellationCost=${info.CancellationCost}&cancellationToken=${info.Token}`;
        const preCancelBookingUrl
            = `/tradebookings/api/bookingdetails/cancel/${bookingReference}?${queryString}`;
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'GET',
            credentials: 'same-origin',
        };

        return fetch(preCancelBookingUrl, fetchOptions)
            .then(response => response.json());
    }
    cancelComponents(bookingReference, tokens, components) {
        const preCancelComponentsUrl
            = `/tradebookings/api/bookingdetails/cancelcomponent/${bookingReference}`;
        const componentFetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'POST',
            credentials: 'same-origin',
            body: JSON.stringify(components),
        };

        return fetch(preCancelComponentsUrl, componentFetchOptions)
            .then(response => response.json());
    }
    sendCancellationRequests(state, bookingReference) {
        const information = state.mmb.cancellationInformation[bookingReference];
        const tokens = state.mmb.selectedComponentTokens;
        const cancelFullBooking = tokens.indexOf(information.all.Token) !== -1;
        const components = cancelFullBooking
            ? ['All'] : information.components.filter(c => tokens.indexOf(c.Token) !== -1);
        const promises = [];
        if (cancelFullBooking) {
            promises.push(this.cancelBooking(bookingReference, information.all));
        } else {
            promises.push(this.cancelComponents(bookingReference, tokens, components));
        }
        return (
            Promise.all(promises))
            .then(results => {
                const cancelReturn = results[0];
                cancelReturn.CancellationComponents = components;
                cancelReturn.type = cancelFullBooking ? 'All' : 'Components';
                return cancelReturn;
            });
    }
    viewBookingDocumentation(documentId, type, reference) {
        const urlBase = '/tradebookings/api/bookingdetails/documentation/';
        const preCancelComponentsUrl
            = `${urlBase}${documentId}/View/${type}/${reference}`;
        const componentFetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'GET',
            credentials: 'same-origin',
        };
        return fetch(preCancelComponentsUrl, componentFetchOptions)
            .then(response => response.json());
    }
    sendBookingDocumentation(documentId, type, reference, overrideEmail) {
        const urlBase = '/tradebookings/api/bookingdetails/documentation/';
        const urlDynamic = `${documentId}/Send/${type}/${reference}`;
        const queryString = overrideEmail ? `?overrideEmail=${overrideEmail}` : '';
        const preCancelComponentsUrl = `${urlBase}${urlDynamic}${queryString}`;
        const componentFetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'GET',
            credentials: 'same-origin',
        };
        return fetch(preCancelComponentsUrl, componentFetchOptions)
            .then(response => response.json());
    }
}

export default MMBAPI;
