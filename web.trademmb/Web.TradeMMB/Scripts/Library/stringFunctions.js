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
        return url;
    }
}

export default StringFunctions;
