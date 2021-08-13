import * as types from './actionTypes';
import QuoteAPI from 'api/quoteAPI';

/**
 * Dispatch method for create quote request
 * @return {object} The action type and basket details object
 */
function quoteCreateRequest() {
    return { type: types.QUOTE_CREATE_REQUEST };
}

/**
 * Dispatch method for successfully creating a basket.
 * @param {object} basket - The updated basket
 * @return {object} The action type and basket
 */
function quoteCreateSuccess(basket) {
    return { type: types.QUOTE_CREATE_SUCCESS, basket };
}
/**
 * Dispatch method for showing the quote email popup
 * @param {string} token - the token of the property
 * @param {object} cheapestResults - the cheapest results for that property
 * @return {object} The action type, token and cheapest results
 */
function quoteEmailShow(token, cheapestResults) {
    return { type: types.QUOTE_EMAIL_SHOW, token, cheapestResults };
}
/**
 * Dispatch method for hiding the quote email popup
 * @return {object} The action type
 */
function quoteEmailHide() {
    return { type: types.QUOTE_EMAIL_HIDE };
}

/**
 * Redux Action method for showing the quote email popup
 * @param {string} token - The property token
 * @param {object} cheapestResults - The property token
 * @return {object} The dispatch
 */
export function showQuoteEmailPopup(token, cheapestResults) {
    return function load(dispatch) {
        dispatch(quoteEmailShow(token, cheapestResults));
    };
}

/**
 * Redux Action method for hiding the quote email popup
 * @param {string} token - The property token
 * @return {object} The dispatch
 */
export function hideQuoteEmailPopup() {
    return function load(dispatch) {
        dispatch(quoteEmailHide());
    };
}

/**
 * Dispatch method for quote retrieve request
 * @return {object} The action type and basket
 */
function quoteRetrieveRequest() {
    return { type: types.QUOTE_RETRIEVE_REQUEST };
}

/**
 * Dispatch method for successfully retrieving a quote
 * @param {object} result - The retrieved quote result
 * @return {object} The action type and result
 */
function quoteRetrieveSuccess(result) {
    return { type: types.QUOTE_RETRIEVE_SUCCESS, result };
}

/**
* Redux Action method for creating a quote
* @param {string} basket - the basket
* @return {object} The result
*/
export function createQuote(basket) {
    return function load(dispatch) {
        dispatch(quoteCreateRequest());
        const quoteAPI = new QuoteAPI();
        return quoteAPI.createQuote(basket)
            .then(createQuoteReturn => {
                if (createQuoteReturn.success) {
                    dispatch(quoteCreateSuccess(createQuoteReturn.basket));
                }
            });
    };
}

/**
* Redux Action method for emailing a quote
* @param {object} model - the model
* @return {object} The result
*/
export function emailQuote(model) {
    return function load() {
        /* dispatch(quoteCreateRequest());*/
        const quoteAPI = new QuoteAPI();
        return quoteAPI.emailQuote(model);
        //    .then(emailQuote => {
        // *if (createQuoteReturn.success) {
        //    dispatch(quoteCreateSuccess(createQuoteReturn.basket));
        // } */
        //    });
    };
}

/**
* Redux Action method for creating a quote pdf
* @param {object} model - the model
* @return {object} The result
*/
export function pdfQuote(model) {
    return function load() {
        /* dispatch(quoteCreateRequest());*/
        const quoteAPI = new QuoteAPI();
        return quoteAPI.pdfQuote(model)
            .then(returnedPdf => {
                if (returnedPdf !== 'undefined') {
                    const link = document.createElement('a');
                    link.download = encodeURI(returnedPdf.DocumentUrl.split('/').pop());
                    link.href = encodeURI(returnedPdf.DocumentUrl);
                    if (window.navigator.userAgent.match(/(MSIE|Trident)/)) {
                        link.target = '_blank';
                    }
                    document.body.appendChild(link);
                    link.click();
                    link.remove();
                }
            });
    };
}

/**
* Redux Action method for retrieving a quote
* @param {string} quoteReference - the quote reference
* @return {object} The result
*/
export function retrieveQuote(quoteReference) {
    return function load(dispatch) {
        dispatch(quoteRetrieveRequest());
        const quoteAPI = new QuoteAPI();
        return quoteAPI.retrieveQuote(quoteReference)
            .then(retrieveQuoteReturn => {
                dispatch(quoteRetrieveSuccess(retrieveQuoteReturn));
            });
    };
}
