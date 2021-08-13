class ValidateFunctions {
    /**
     * Function to determine if a string is a valid email
     * @param {string} sEmail - The string to check
     * @return {boolean} true if an email, false if not.
    */
    static isEmail(sEmail) {
        // eslint-disable-next-line max-len
        const o = new RegExp(/^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
        return o.test(sEmail);
    }

    /**
     * Function to determine if a string is a valid phone number
     * @param {string} sAlpha - The string to check
     * @return {boolean} true if an phone number, false if not.
    */
    static isNumericPhoneNumber(sAlpha) {
        const sAlphaRegEx = /^[ 0-9\(\)\/\+]*$/;
        const o = new RegExp(sAlphaRegEx);
        return o.test(sAlpha);
    }

    /**
     * Function to determine if a string is a numeric key
     * @param {integer} whichCode - The code from event.which field
     * @return {boolean} true if 0 - 9 or numpad 0 - numpad 9, false if not.
    */
    static isNumericKey(whichCode) {
        const code = parseInt(whichCode, 10);
        let isNumericKey = false;
        const startOfNumKeys = 48;
        const endOfNumKeys = 57;
        const startOfNumPadKeys = 96;
        const endOfNumPadKeys = 105;
        if ((code >= startOfNumKeys && code <= endOfNumKeys)
            || (code >= startOfNumPadKeys && code <= endOfNumPadKeys)) {
            isNumericKey = true;
        }
        return isNumericKey;
    }

    /**
     * Function to determine if a string is a valid edit key
     * @param {integer} whichCode - The code from event.which field
     * @return {boolean} true if backspace, tab, enter or delete.
    */
    static isEditKey(whichCode) {
        const code = parseInt(whichCode, 10);
        let isEditKey = false;
        const backspaceKey = 8;
        const tabKey = 9;
        const enterKey = 13;
        const deleteKey = 46;
        if ((code === backspaceKey) || (code === tabKey)
            || (code === enterKey) || (code === deleteKey)) {
            isEditKey = true;
        }
        return isEditKey;
    }

    /**
     * Function to determine if a string is a valid phone number
     * @param {integer} whichCode - The code from event.which field
     * @return {boolean} true if isEditKey or isNUmericKey functions return true.
    */
    static isNumericOrEditKey(whichCode) {
        let isNumericOrEditKey = false;
        if ((ValidateFunctions.isNumericKey(whichCode) === true)
            || (ValidateFunctions.isEditKey(whichCode) === true)) {
            isNumericOrEditKey = true;
        }
        return isNumericOrEditKey;
    }
}

export default ValidateFunctions;
