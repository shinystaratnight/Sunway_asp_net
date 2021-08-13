class DateFunctions {
    /**
     * Function to get safe time zone date format
     * @param {object} date - The query key
     * @return {object} value - The query key values
    */
    static timeZoneSafeDate(date) {
        const minutesInHour = 60;
        let dDate = date;
        if (typeof date === 'string') {
            dDate = new Date(date);
        }
        dDate.setHours(dDate.getHours() - dDate.getTimezoneOffset() / minutesInHour);
        return dDate;
    }

    /**
     * Parse ISO date string format into date object
     * @param {string} s - an ISO 8001 format date and time string
     * with all components, e.g. 2015-11-24T19:40:00
     * @return {Date} - Date instance from parsing the string.
     */
    static parseISOLocal(s) {
        const b = s.split(/\D/);
        return new Date(b[0], b[1] - 1, b[2], b[3], b[4], b[5]);
    }
}

export default DateFunctions;
