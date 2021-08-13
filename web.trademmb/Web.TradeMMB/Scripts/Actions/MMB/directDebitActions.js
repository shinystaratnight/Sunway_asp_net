import * as types from './actionTypes';
import DirectDebitAPI from '../../api/DirectDebitAPI';

/**
 * Redux Action method for loading all entity models
 * @param {object} filterOverrides - overrides
 * @return {object} The action type
 */
function loadDirectDebitRequest() {
    return { type: types.DIRECT_DEBIT_LOAD_REQUEST };
}

/**
 * Redux Action method for loading custom entity models.
 * @param {object} directDebitRetrieval - The response
 * @param {object} tradeId - The trade Id
 * @return {object} The action type and models
 */
function loadDirectDebitsSuccess(directDebitRetrieval, tradeId) {
    return { type: types.DIRECT_DEBIT_LOAD_SUCCESS, directDebitRetrieval, tradeId };
}


/**
 * Redux Action method for loading a list of entities.
 * @param {object} tradeId - the trade id to filter on
 * @return {object} The direct debits
 */
export function loadAllDirectDebits(tradeId) {
    return function load(dispatch) {
        dispatch(loadDirectDebitRequest());
        const directDebitAPI = new DirectDebitAPI();
        return directDebitAPI.loadAllDirectDebits()
            .then(directDebitRetrieval => {
                dispatch(loadDirectDebitsSuccess(directDebitRetrieval, tradeId));
            }).catch(error => {
                throw (error);
            });
    };
}
