class DateFunctions {
    /**
     * Create date as utc time
     * @return {Date} the utc date.
     */
    static createDateAsUtc() {
        const now = new Date();
        const seconds = 60;
        const milliseconds = 1000;
        now.setTime(now.getTime() - now.getTimezoneOffset() * seconds * milliseconds);
        return now;
    }

    /**
     * Function to get query string values from a url
     * @param {Object} testObject - The object being tested for having valid dates
     * @return {string} value - The query key values
    */
    static hasValidPublishDates(testObject) {
        let inDate = true;
        if (testObject.hasOwnProperty('PublishDates')) {
            const now = this.createDateAsUtc().toISOString();
            const publishUp = testObject.PublishDates.PublishUp;
            const publishDown = testObject.PublishDates.PublishDown;
            const publishUpIso = `${publishUp.Date}T${publishUp.Time}`;
            const publishDownIso = `${publishDown.Date}T${publishDown.Time}`;
            inDate = now > publishUpIso && now < publishDownIso;
        }
        return inDate;
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
