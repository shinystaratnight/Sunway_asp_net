class UrlFunctions {
    /**
     * Function to get query string values from a url
     * @param {string} name - The query key
     * @return {string} value - The query key values
    */
    static getQueryStringValue(name) {
        const regExp = new RegExp(`[?|&]${name}=([^&;]+?)(&|#|;|$)`);
        return decodeURIComponent((regExp.exec(location.search)
            || [null, ''])[1].replace(/\+/g, '%20')) || null;
    }

    static getQueryStringValueWithSpaces(name) {
        const regExp = new RegExp(`[?|&]${name}=([^&;]+?)(&|#|;|$)`);
        return decodeURIComponent((regExp.exec(location.search)
            || [null, ''])[1]) || null;
    }
}

export default UrlFunctions;
