import fetch from 'isomorphic-fetch';

class LookupAPI {
    getLookup(type, site) {
        let objectType = type;
        let lookupUrl = '';

        if (type === 'propertySummary' || type === 'propertyFull') {
            objectType = 'property/propertyreference';
        }

        const fetchOptions = {
            credentials: 'omit',
        };

        if (objectType.indexOf('|') > -1) {
            const lookupType = objectType.split('|')[0];
            objectType = objectType.split('|')[1];

            if (lookupType === 'context') {
                lookupUrl = `/booking/api/sitebuilder/${site}/${objectType}/contexts`;
                return fetch(lookupUrl, fetchOptions)
                    .then(response => response.json()).then(result => result);
            }

            if (lookupType === 'custom') {
                lookupUrl = `/booking/api/sitebuilder/model/CustomLookup/${objectType}/live`;
                return fetch(lookupUrl, fetchOptions)
                    .then(response => response.json())
                    .then(result => {
                        const lookup = JSON.parse(result);
                        return lookup.Items;
                    });
            }
        }

        lookupUrl = `/booking/api/${objectType}`;
        if (document.getElementById('hidApiBaseUrl')) {
            const apiBaseUrl = document.getElementById('hidApiBaseUrl').value;
            lookupUrl = `${apiBaseUrl}${lookupUrl}`;
        }
        return fetch(lookupUrl, fetchOptions)
            .then(response => response.json());
    }
}

export default LookupAPI;
