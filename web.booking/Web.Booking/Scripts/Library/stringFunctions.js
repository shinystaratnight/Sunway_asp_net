class StringFunctions {
    /**
     * Function to set the pluralisation of a string based on given value
     * @param {number} value - The value to check against
     * @param {string} singular - The singular string value
     * @param {string} plural - The plural string value
     * @return {string} - The safe Url string
    */
    static getPluralisationValue(value, singular, plural) {
        let pluralisedValue = value;
        if (value > 1) {
            pluralisedValue += ` ${plural}`;
        } else if (value > 0) {
            pluralisedValue += ` ${singular}`;
        } else {
            pluralisedValue = '';
        }
        return pluralisedValue;
    }

    /**
     * Function to determine if a string is null or empty
     * @param {string} str - The string to check
     * @return {boolean} true if null or empty, false if not.
    */
    static isNullOrEmpty(str) {
        return typeof str === 'undefined' || str === '';
    }

    /**
     * Function to get height class from height setting enum
     * @param {string} value - The enum value
     * @return {string} className - the className that corresponds
    */
    static heightClassFromEnum(value) {
        let className = 'height-';
        switch (value) {
            case 'Extra Small':
                className += 'xs';
                break;
            case 'Small':
                className += 'sm';
                break;
            case 'Small-Plus':
                className += 'sm-plus';
                break;
            case 'Medium':
                className += 'md';
                break;
            case 'Large':
                className += 'lg';
                break;
            case 'Extra Large':
                className += 'xl';
                break;
            case 'Extra Extra Large':
                className += 'xxl';
                break;
            default:
                className += 'md';
                break;
        }
        return className;
    }
    /**
     * Function to format given string as a safe url string
     * @param {string} str - The string to format
     * @return {string} - The safe Url string
    */
    static safeUrl(str) {
        let url = str.toLowerCase();
        url = url.replace(/ /g, '-');
        url = url.replace(/&/g, 'and');
        url = url.replace(/#/g, '-');
        url = url.replace(/\+/g, '-');
        url = url.replace(/\*/g, '');
        url = url.replace(/\./g, '');
        url = url.replace(/:/g, '');
        url = url.replace(/,/g, '');
        url = url.replace(/'/g, '');
        url = url.replace(/à/g, 'a');
        url = url.replace(/è/g, 'e');
        url = url.replace(/ì/g, 'i');
        url = url.replace(/ò/g, 'o');
        url = url.replace(/ù/g, 'u');

        url = url.replace(/á/g, 'a');
        url = url.replace(/é/g, 'e');
        url = url.replace(/í/g, 'i');
        url = url.replace(/ó/g, 'o');
        url = url.replace(/ú/g, 'u');
        url = url.replace(/ý/g, 'y');

        url = url.replace(/â/g, 'a');
        url = url.replace(/ê/g, 'e');
        url = url.replace(/î/g, 'i');
        url = url.replace(/ô/g, 'o');
        url = url.replace(/û/g, 'u');

        url = url.replace(/ã/g, 'a');
        url = url.replace(/ñ/g, 'n');
        url = url.replace(/õ/g, 'o');

        url = url.replace(/ä/g, 'a');
        url = url.replace(/ë/g, 'e');
        url = url.replace(/ï/g, 'i');
        url = url.replace(/ö/g, 'o');
        url = url.replace(/ü/g, 'u');
        url = url.replace(/ÿ/g, 'u');
        return url;
    }

    /**
     * Function to format given string from a safe url string
     * @param {string} str - The string to format
     * @return {string} - The normal string
    */
    static undoSafeUrl(str) {
        let url = str.toLowerCase();
        url = url.replace(/-/g, ' ');
        return url;
    }

    /**
     * Function to check 2 urls are equal
     * @param {string} stringA - First url to check
     * @param {string} stringB - Second url to check
     * @return {boolean} - The result
    */
    static equalSafeUrl(stringA, stringB) {
        return this.safeUrl(stringA).trim() === this.safeUrl(stringB).trim();
    }
}

export default StringFunctions;
