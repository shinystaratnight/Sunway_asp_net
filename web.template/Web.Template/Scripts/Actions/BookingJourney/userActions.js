import * as types from './actionTypes';
import WebRequest from 'library/webrequest';

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
    const userUrl = '/api/user';
    const options = {
        credentials: 'same-origin',
    };
    return function load(dispatch) {
        return WebRequest.send(userUrl, options)
            .then(response => {
                if (response.ok) {
                    dispatch(userLoadSuccess(response.object));
                }
            });
    };
}

/**
 * Redux Action method for trade login.
 * @param {string} email - the trade email
 * @param {string} password - the trade password
 * @param {boolean} saveDetails - whether to save details and auto login
 * @return {object} The user
 */
export function userLogin(email, password, saveDetails) {
    let loginURL = '/api/user/login/trade?';
    loginURL = `${loginURL}emailaddress=${email}&password=${password}&savedetails=${saveDetails}`;
    const options = {
        credentials: 'same-origin',
    };
    return function load(dispatch) {
        return WebRequest.send(loginURL, options)
            .then(response => {
                if (response.ok) {
                    dispatch(userLoginSuccess(response.object));
                }
            });
    };
}

/**
 * Redux Action method for trade login.
 * @param {string} username - the trade username
 * @param {string} password - the trade password
 * @param {boolean} saveDetails - whether to save details and auto login
 * @return {object} The user
 */
export function userLoginUsername(username, password, saveDetails) {
    let loginURL = '/api/user/login/trade?';
    loginURL = `${loginURL}username=${username}&websitepassword=${password}`;
    loginURL = `${loginURL}&savedetails=${saveDetails}`;
    const options = {
        credentials: 'same-origin',
    };
    return function load(dispatch) {
        return WebRequest.send(loginURL, options)
            .then(response => {
                if (response.ok) {
                    dispatch(userLoginSuccess(response.object));
                }
            });
    };
}

/**
 * Redux Action method for logging out.
 * @return {object} The user
 */
export function userLogOut() {
    const logOutUrl = '/api/user/logout';
    const options = {
        credentials: 'same-origin',
    };
    return function load(dispatch) {
        return WebRequest.send(logOutUrl, options)
            .then(response => {
                if (response.ok) {
                    dispatch(userLogOutSuccess(response.object));
                }
            });
    };
}

/**
 * Redux Action method for logging out admin.
 * @return {object} The user
 */
export function userLogOutAdmin() {
    const logOutUrl = '/api/user/logoutadmin';
    const options = {
        credentials: 'same-origin',
    };
    return function load(dispatch) {
        return WebRequest.send(logOutUrl, options)
            .then(response => {
                if (response.ok) {
                    dispatch(userLogOutSuccess(response.object));
                }
            });
    };
}

/**
 * Redux Action method for logging out.
 * @param {string} website - the website to change the user to.
 * @return {object} The user
 */
export function userChangeSite(website) {
    const changeUserUrl = `/api/user/website/${website}`;
    const options = {
        credentials: 'same-origin',
    };
    return function load() {
        return WebRequest.send(changeUserUrl, options)
            .then(response => {
                if (response.ok) {
                    location.reload();
                }
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
    let loginUrl = '/api/sitebuilder/login?';
    loginUrl = `${loginUrl}username=${username}&password=${password}`;
    const options = {
        credentials: 'same-origin',
    };
    return function load(dispatch) {
        return WebRequest.send(loginUrl, options)
            .then(response => {
                if (response.ok) {
                    dispatch(userLoginSuccess(response.object));
                }
            });
    };
}
