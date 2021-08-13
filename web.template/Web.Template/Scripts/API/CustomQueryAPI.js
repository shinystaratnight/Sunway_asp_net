import fetch from 'isomorphic-fetch';

class CustomQueryApi {
    constructor() {
        this.content = {};
    }
    getCustomQueryContent(queryName, params) {
        let apiURL = '/api/customquery?';

        apiURL += `queryName=${queryName}`;
        apiURL += '&parameterCsv=';
        params.forEach(param => {
            apiURL += `${param},`;
        });
        return fetch(apiURL, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(result => {
                let content = {};
                if (result.CustomQueryResponse.CustomXML) {
                    content = result.CustomQueryResponse.CustomXML;
                }
                return content;
            }
        );
    }
}

export default CustomQueryApi;
