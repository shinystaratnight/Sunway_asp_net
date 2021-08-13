import fetch from 'isomorphic-fetch';

class LookupAPI {
    getLookup(type) {
        let objectType = type;
        let lookupUrl = '';

        if (type === 'propertySummary' || type === 'propertyFull') {
            objectType = 'property/propertyreference';
        }

        if (objectType.indexOf('|') > -1) {
            const lookupType = objectType.split('|')[0];
            objectType = objectType.split('|')[1];

            if (lookupType === 'context') {
                lookupUrl = `/tradebookings/api/sitebuilder/${objectType}/contexts`;
                return fetch(lookupUrl).then(response => response.json()).then(result => result);
            }

            if (lookupType === 'custom') {
                lookupUrl = `/tradebookings/api/sitebuilder/model/CustomLookup/${objectType}/live`;
                return fetch(lookupUrl)
                    .then(response => response.json())
                    .then(result => {
                        const lookup = JSON.parse(result);
                        return lookup.Items;
                    });
            }
        }

        lookupUrl = `/tradebookings/api/${objectType}`;
        return fetch(lookupUrl)
            .then(response => response.json());
    }
}

export default LookupAPI;
