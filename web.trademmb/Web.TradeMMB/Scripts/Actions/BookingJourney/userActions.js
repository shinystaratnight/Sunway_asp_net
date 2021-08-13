import * as types from './actionTypes';
import fetch from 'isomorphic-fetch';

/**
 * Redux Action method for successfully loading a user.
 * @param {object} user - The user object to return
 * @return {object} The action type and user object
 */
export function userLoadSuccess(user) {
    return { type: types.USER_LOAD_SUCCESS, user };
}

/**
 * Redux Action method for successful trade login.
 * @param {object} user - The user object to return
 * @return {object} The action type and user object
 */
export function userLoginSuccess(user) {
    return { type: types.USER_LOGIN_SUCCESS, user };
}

/**
 * Redux Action method for successfully logging out
 * @param {object} user - The user object to return
 * @return {object} The action type and user object
 */
export function userLogOutSuccess(user) {
    return { type: types.USER_LOGOUT_SUCCESS, user };
}

/**
 * Redux Action method for loading the current user.
 * @return {object} The user
 */
export function loadUser() {
    const userURL = '/tradebookings/api/user';
    return function load(dispatch) {
        return fetch(userURL,
            { credentials: 'same-origin' }
            ).then(response => response.json()).then(result => {
                dispatch(userLoadSuccess(result));
            });
    };
}

/**
 * Redux Action method for trade login.
 * @param {string} email - the trade email
 * @param {string} password - the trade password
 * @return {object} The user
 */
export function userLogin(email, password) {
    let loginURL = '/booking/api/user/login/trade?';
    loginURL = `${loginURL}emailaddress=${email}&password=${password}`;
    return function load(dispatch) {
        return fetch(loginURL,
            { credentials: 'same-origin' }
            ).then(response => response.json()).then(result => {
                dispatch(userLoginSuccess(result));
            });
    };
}

/**
 * Redux Action method for logging out.
 * @return {object} The user
 */
export function userLogOut() {
    const logOutURL = '/tradebookings/api/user/logout';
    return function load(dispatch) {
        return fetch(logOutURL,
            { credentials: 'same-origin' }
            ).then(response => response.json()).then(result => {
                dispatch(userLogOutSuccess(result));
            });
    };
}

/**
 * Redux Action method for admin login.
 * @param {string} username - the login username
 * @param {string} password - the login password
 * @return {object} The user
 */
export function adminLogin(username, password) {
    let loginURL = 'api/sitebuilder/login?';
    loginURL = `${loginURL}username=${username}&password=${password}`;
    return function load(dispatch) {
        return fetch(loginURL,
            { credentials: 'same-origin' }
            ).then(response => response.json()).then(result => {
                dispatch(userLoginSuccess(result));
            });
    };
}
