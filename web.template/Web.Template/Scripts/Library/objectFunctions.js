
class ObjectFunctions {
    /**
     * Function to determine if an object is null or empty
     * @param {string} obj - The object to check
     * @return {boolean} true if null or empty, false if not.
    */
    static isNullOrEmpty(obj) {
        return typeof obj === 'undefined' || Object.keys(obj).length === 0;
    }

    /**
     * Function to get the value of an object property by a given string path.
     * @param {object} obj - The object to get the value from
     * @param {string} stringPath - The string path of the property
     * e.g. 'Object.Property.SubProperty'
     * @return {*} The value of the property, or null if not found.
    */
    static getValueByStringPath(obj, stringPath) {
        let path = stringPath ? stringPath : '';
        path = path.replace(/\[(\w+)\]/g, '.$1');
        path = path.replace(/^\./, '');

        const fields = path.split('.');
        let currentObject = obj ? obj : {};
        let match = false;

        for (let i = 0; i < fields.length; i++) {
            const field = fields[i];
            if (field in currentObject) {
                currentObject = currentObject[field];
                match = true;
            } else {
                match = false;
                break;
            }
        }
        return match ? currentObject : null;
    }

    /**
     * Function to set the value of an object property by a given string path.
     * @param {object} obj - The object to set the value on
     * @param {string} stringPath - The string path of the property
     * e.g. 'Object.Property.SubProperty'
     * @param {*} val - The new value to update.
     * @return {object} The updated object with the new value set.
    */
    static setValueByStringPath(obj, stringPath, val) {
        let path = stringPath.replace(/\[(\w+)\]/g, '.$1');
        path = path.replace(/^\./, '');
        const fields = path.split('.');
        let model = obj ? obj : {};

        while (fields.length > 1) {
            const field = fields[0];
            const nextField = fields[1];
            const defaultValue = !isNaN(nextField) ? [] : {};
            model[field] = model[field] ? model[field] : defaultValue;
            model = model[fields.shift()];
        }
        model[fields.shift()] = val;
        return obj;
    }
}

export default ObjectFunctions;
