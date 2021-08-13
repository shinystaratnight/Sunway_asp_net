import fetch from 'isomorphic-fetch';

class LookupAPI {
    getLookup(type, siteName) {
        let objectType = type;
        let lookupUrl = '';

        if (type === 'propertySummary' || type === 'propertyFull') {
            objectType = 'property/propertyreference';
        }

        if (objectType.indexOf('|') > -1) {
            const lookupType = objectType.split('|')[0];
            objectType = objectType.split('|')[1];

            if (lookupType === 'context') {
                lookupUrl = `/api/sitebuilder/${siteName}/${objectType}/contexts`;
                return fetch(lookupUrl, { credentials: 'same-origin' }).then(
                    response => response.json()).then(result => result);
            }

            if (lookupType === 'custom') {
                lookupUrl = `/api/sitebuilder/model/${siteName}/CustomLookup/${objectType}/live`;
                return fetch(lookupUrl, { credentials: 'same-origin' })
                    .then(response => response.json())
                    .then(result => {
                        const lookup = JSON.parse(result);
                        return lookup.Items;
                    });
            }
        }

        lookupUrl = `/api/${objectType}`;
        return fetch(lookupUrl).then(response => response.json()).then(result => result);
    }
}

export default LookupAPI;
