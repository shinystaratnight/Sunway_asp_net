import fetch from 'isomorphic-fetch';

class WebRequest {
    static send(url, options) {
        let fetchUrl = url;
        if (document.getElementById('hidApiBaseUrl')) {
            const apiBaseUrl = document.getElementById('hidApiBaseUrl').value;
            fetchUrl = `${apiBaseUrl}${fetchUrl}`;
        }
        return fetch(fetchUrl, options)
            .then(response => WebRequest.parseResponse(fetchUrl, response))
            .then(parsedResponse => parsedResponse);
    }
    static parseResponse(url, response) {
        return new Promise(resolve => {
            let result = {
                ok: response.ok,
            };
            if (response.ok) {
                try {
                    response.json()
                        .then(object => {
                            result.object = typeof object === 'string' && object.length > 0
                                ? JSON.parse(object) : object;
                            resolve(result);
                        });
                } catch (error) {
                    result = {
                        ok: false,
                        error,
                        url,
                    };
                    resolve(result);
                }
            } else {
                resolve(result);
            }
        });
    }
}

export default WebRequest;
