import fetch from 'isomorphic-fetch';

class QuoteAPI {
    filterQuotes(quotes, filters) {
        const filterkeys = Object.keys(filters);
        let filteredQuotes = Object.assign([], quotes);
        for (let i = 0; i < filteredQuotes.length; i++) {
            filteredQuotes[i].display = true;
        }
        filterkeys.forEach(filterKey => {
            const filter = filters[filterKey];
            if (filter.value !== '') {
                switch (filter.type) {
                    case 'text':
                        filteredQuotes = this.filterText(filter, filteredQuotes);
                        break;
                    case 'multifieldText':
                        filteredQuotes = this.filterMultifieldText(filter, filteredQuotes);
                        break;
                    case 'date':
                        if (filter.selectedOption !== 'Any') {
                            filteredQuotes = this.filterDate(filter, filteredQuotes);
                        }
                        break;
                    default:
                }
            }
        });
        return filteredQuotes;
    }
    filterText(filter, quotes) {
        const filteredQuotes = [];
        quotes.forEach(quote => {
            const filteredQuote = Object.assign({}, quote);
            filteredQuote.display = quote.display
                ? quote[filter.field].toLowerCase()
                    .indexOf(filter.value.toLowerCase()) !== -1
                : quote.display;
            filteredQuotes.push(filteredQuote);
        });
        return filteredQuotes;
    }
    filterMultifieldText(filter, quotes) {
        const filteredQuotes = [];
        quotes.forEach(quote => {
            const filteredQuote = Object.assign({}, quote);
            let comparisonText = '';
            filter.fields.forEach(f => {
                comparisonText += `${filteredQuote[f]} `;
            });
            filteredQuote.display = quote.display
                ? comparisonText.toLowerCase().indexOf(filter.value.toLowerCase()) !== -1
                : quote.display;
            filteredQuotes.push(filteredQuote);
        });
        return filteredQuotes;
    }
    filterDate(filter, quotes) {
        const filteredQuotes = [];
        quotes.forEach(quote => {
            const filteredQuote = Object.assign({}, quote);
            const comparisonValue = quote[filter.field];
            filteredQuote.display = filteredQuote.display
                ? comparisonValue > filter.startDate
                : filteredQuote.display;
            filteredQuote.display = filteredQuote.display
                ? comparisonValue < filter.endDate
                : filteredQuote.display;
            filteredQuotes.push(filteredQuote);
        });
        return filteredQuotes;
    }
    search(searchModel) {
        const url = '/tradebookings/api/quote/search';
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'POST',
            credentials: 'same-origin',
            body: JSON.stringify(searchModel),
        };
        return fetch(url, fetchOptions)
            .then(response => response.json())
            .then(result => {
                for (let i = 0; i < result.Quotes.length; i++) {
                    const quote = result.Quotes[i];
                    quote.display = true;
                }
                return result;
            });
    }
    updateFilters(filters, filter) {
        const replacementFilter = {};
        replacementFilter[filter.name] = filter;
        const newFilters = Object.assign({}, filters, replacementFilter);
        return newFilters;
    }
}

export default QuoteAPI;
