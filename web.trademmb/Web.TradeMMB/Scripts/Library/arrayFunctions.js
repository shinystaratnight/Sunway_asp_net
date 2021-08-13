class ArrayFunctions {
    /**
      * Function to ensure value is correct for sorting
      * @param {*} value - The value to check
      * @return {*} The sort safe value
     */
    static sortSafeValue(value) {
        const lastJavascriptChar = 'zzzz';
        let safeValue = (value === '') ? lastJavascriptChar : value;
        if (typeof safeValue === 'number' || !isNaN(safeValue)) {
            safeValue = parseFloat(safeValue);
        } else if (typeof safeValue === 'string') {
            safeValue = safeValue.toLowerCase();
        }
        return safeValue;
    }

    /**
      * Function to sort an array in ascending order by given property
      * @param {object} array - The array to sort
      * @param {string} property - The property to sort on
      * @return {object} The sorted array
     */
    static sortByPropertyAscending(array, property) {
        const sortedArray = Object.assign([], array);
        sortedArray.sort((a, b) => {
            const aValue = ArrayFunctions.sortSafeValue(a[property]);
            const bValue = ArrayFunctions.sortSafeValue(b[property]);
            if (aValue > bValue) {
                return 1;
            }
            if (aValue < bValue) {
                return -1;
            }
            return 0;
        });
        return sortedArray;
    }

    /**
      * Function to sort an array in descending order by given property
      * @param {object} array - The array to sort
      * @param {string} property - The property to sort on
      * @return {object} The sorted array
     */
    static sortByPropertyDescending(array, property) {
        const sortedArray = Object.assign([], array);
        sortedArray.sort((a, b) => {
            const aValue = ArrayFunctions.sortSafeValue(a[property]);
            const bValue = ArrayFunctions.sortSafeValue(b[property]);
            if (aValue > bValue) {
                return -1;
            }
            if (aValue < bValue) {
                return 1;
            }
            return 0;
        });
        return sortedArray;
    }

    /**
      * Function to remove given item from an array
      * @param {object} array - The array to update
      * @param {strng} field - The field to match the item on
      * @param {string} item - The item to remove
      * @return {object} The updated array
     */
    static removeItem(array, field, item) {
        const updatedArray = Object.assign([], array);
        const itemIndex = array.findIndex(arrayItem => arrayItem[field] === item);

        if (itemIndex > -1) {
            updatedArray.splice(itemIndex, 1);
        }

        return updatedArray;
    }
}

export default ArrayFunctions;
