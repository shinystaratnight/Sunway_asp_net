import * as types from './actionTypes';

/**
 * Redux Action method for handling errors
 * @param {object} error - The error to be added
 * @return {object} The action type and warning object
 */
export function errorLog(error) {
    // eslint-disable-next-line no-console
    console.log(error);
    return { type: types.ERROR_LOG, error };
}
