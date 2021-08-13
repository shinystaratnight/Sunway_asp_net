import WebRequest from 'library/webrequest';

class RedirectAPI {
    constructor() {
        this.definitions = {};
        this.redirectPromises = [];
        this.jsonSchema = {};
        this.mode = '';
    }

    /**
     * Load CMS model
     * @param {object} objectType - The object type to return
     * @param {string} id - Id of the object type
     * @return {object} The model
     */
    loadRedirectList() {
        const entityUrl = '/api/redirect/all';
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'GET',
            credentials: 'same-origin',
        };
        return WebRequest.send(entityUrl, fetchOptions)
            .then(response => {
                let redirectList = [];
                if (response.ok) {
                    redirectList = response.object;
                }
                return redirectList;
            });
    }
    modifyRedirect(redirect, site) {
        const requestBody = Object.assign({}, redirect);
        requestBody.SiteName = site;
        const entityUrl = '/api/redirect/modify';
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'POST',
            credentials: 'same-origin',
            body: JSON.stringify(requestBody),
        };
        return WebRequest.send(entityUrl, fetchOptions)
            .then(response => {
                let success = false;
                if (response.ok) {
                    success = response.object;
                }
                return success;
            });
    }

    addRedirect(redirect, site) {
        const requestBody = Object.assign({}, redirect);
        requestBody.SiteName = site;
        const entityUrl = '/api/redirect/add';
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'PUT',
            credentials: 'same-origin',
            body: JSON.stringify(requestBody),
        };
        return WebRequest.send(entityUrl, fetchOptions)
            .then(response => {
                let success = false;
                if (response.ok) {
                    success = response.object;
                }
                return success;
            });
    }

    deleteRedirect(redirect, site) {
        const requestBody = Object.assign({}, redirect);
        requestBody.SiteName = site;
        const entityUrl = '/api/redirect/delete';
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'Delete',
            credentials: 'same-origin',
            body: JSON.stringify(requestBody),
        };
        return WebRequest.send(entityUrl, fetchOptions)
            .then(response => {
                let success = false;
                if (response.ok) {
                    success = response.object;
                }
                return success;
            });
    }
}

export default RedirectAPI;
