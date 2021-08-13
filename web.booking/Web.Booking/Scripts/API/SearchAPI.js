import * as SearchConstants from '../constants/search';

import DateFunctions from '../library/datefunctions';
import UrlFunctions from '../library/urlfunctions';
import fetch from 'isomorphic-fetch';
import moment from 'moment';

class SearchAPI {
    storageAvailable(type) {
        try {
            const storage = window[type];
            const x = '__storage_test__';
            storage.setItem(x, x);
            storage.removeItem(x);
            return true;
        } catch (exception) {
            return false;
        }
    }
    getSearchDetails(searchConfig) {
        return new Promise((resolve) => {
            let searchDetails = {};
            if (this.storageAvailable('sessionStorage')) {
                const searchDetailsJson = sessionStorage.getItem('__searchdetails__');
                searchDetails = JSON.parse(searchDetailsJson);
                if (!searchDetails) {
                    searchDetails = this.defaultSearchDetails(searchConfig);
                }
            } else {
                searchDetails = {};
            }
            resolve(Object.assign({}, searchDetails));
        });
    }
    getSafeDateSearchDetails(searchDetails) {
        const safeSearchDetails = Object.assign({}, searchDetails);
        safeSearchDetails.DepartureDate
            = DateFunctions.timeZoneSafeDate(safeSearchDetails.DepartureDate);
        return safeSearchDetails;
    }
    setSearchDetailsFromUrl(url) {
        return new Promise((resolve) => {
            const searchDetails = this.urlToSearchDetails(url);
            sessionStorage.setItem('__searchdetails__', JSON.stringify(searchDetails));
            resolve(Object.assign({}, searchDetails));
        });
    }
    defaultSearchDetails(searchConfig) {
        const defaultSearchMode = searchConfig ? searchConfig.SearchModes[0] : 'FlightPlusHotel';
        const packageSearch = defaultSearchMode === 'FlightPlusHotel'
            && searchConfig
            && searchConfig.PackageSearch;

        const searchDetails = {
            ArrivalId: null,
            ArrivalLatitude: 0,
            ArrivalLongitude: 0,
            ArrivalRadius: 0,
            ArrivalType: '',
            DepartureDate: null,
            DepartureId: null,
            DepartureType: '',
            DepartureTime: '00:00',
            Direct: false,
            Duration: null,
            FlightClassId: 0,
            MealBasisId: null,
            MinRating: 0,
            OneWay: false,
            PackageSearch: packageSearch,
            ReturnTime: '00:00',
            Rooms: [
                {
                    Id: 1,
                    Adults: 2,
                    Children: 0,
                    Infants: 0,
                    ChildAges: [],
                },
            ],
            SearchMode: defaultSearchMode,
            SearchURL: '',
            BrandOverride: 0,
        };
        return searchDetails;
    }
    performSearch(searchDetails, isMainSearch) {
        const safeSearchDetails = this.getSafeDateSearchDetails(searchDetails);
        if (isMainSearch) {
            sessionStorage.setItem('__searchdetails__', JSON.stringify(safeSearchDetails));
        }
        const validatedSearchDetails = this.validateSearchDetails(safeSearchDetails);
        const searchUrl = this.searchDetailsToURL(validatedSearchDetails);
        return fetch(searchUrl, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(searchResult => {
                const resultReturn = {
                    searchResult,
                    searchDetails: safeSearchDetails,
                };
                return resultReturn;
            });
    }
    performExtraBasketSearch(searchDetails, identifier) {
        let searchUrl = '/booking/searchapi/extra/basket/';
        searchUrl += `${searchDetails.basketToken}/${searchDetails.extraId}/`;
        searchUrl += `${searchDetails.extraGroupId}/`;
        searchDetails.extraTypes.forEach(extraType => {
            searchUrl += `${extraType.Id},`;
        });
        if (searchDetails.extraTypes.length > 0) {
            searchUrl = searchUrl.substring(0, searchUrl.length - 1);
        }
        return fetch(searchUrl, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(searchResult => {
                const resultCount = searchResult.ResultCounts.Extra;
                const resultToken = searchResult.ResultTokens.Extra;
                const resultReturn = {
                    searchResult: {
                        ResultCounts: {
                            [identifier]: resultCount,
                        },
                        ResultTokens: {
                            [identifier]: resultToken,
                        },
                    },
                    searchDetails,
                };
                return resultReturn;
            });
    }
    validateSearchDetails(searchDetails) {
        const validatedSearchDetails = Object.assign({}, searchDetails);
        switch (validatedSearchDetails.SearchMode) {
            case 'Transfer': {
                let totalAdults = 0;
                let totalChildren = 0;
                let totalInfants = 0;
                let totalChildAges = [];
                validatedSearchDetails.Rooms.forEach(r => {
                    totalAdults += r.Adults;
                    totalChildren += r.Children;
                    totalInfants += r.Infants;
                    totalChildAges = totalChildAges.concat(r.ChildAges);
                });
                const room = {
                    Id: 1,
                    Adults: totalAdults,
                    Children: totalChildren,
                    Infants: totalInfants,
                    ChildAges: totalChildAges,
                };
                validatedSearchDetails.Rooms = [room];
                validatedSearchDetails.DepartureTime
                    = validatedSearchDetails.DepartureTime.replace(/:/g, '');
                validatedSearchDetails.ReturnTime
                    = validatedSearchDetails.ReturnTime.replace(/:/g, '');
                break;
            }
            default:
        }
        return validatedSearchDetails;
    }
    performDeeplinkSearch(url) {
        const page = url.split('/')[2];
        const searchUrl = url.replace(`/booking/${page}`, '');
        const searchDetails = this.urlToSearchDetails(searchUrl);
        const safeSearchDetails = this.getSafeDateSearchDetails(searchDetails);

        sessionStorage.setItem('__searchdetails__', JSON.stringify(safeSearchDetails));

        let apiUrl = `/booking/searchapi${searchUrl}${location.search}`;
        if (safeSearchDetails.PackageSearch) {
            apiUrl = apiUrl.replace('/package', '/flightplushotel');
            apiUrl += location.search ? '&' : '?';
            apiUrl += 'packagesearch=true';
        }

        return fetch(apiUrl, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(searchResult => {
                const resultReturn = {
                    searchResult,
                    searchDetails: safeSearchDetails,
                };
                return resultReturn;
            });
    }
    performAdjustmentSearch(searchDetails) {
        const searchUrl = '/booking/searchapi/bookingadjustment';
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'same-origin',
            method: 'Post',
            body: JSON.stringify(searchDetails),
        };
        return fetch(searchUrl, fetchOptions)
            .then(response => response.json());
    }
    searchDetailsToURL(searchDetails) {
        let searchUrl = `/booking/searchapi/${searchDetails.SearchMode.toLowerCase()}`;

        if (searchDetails.SearchMode === 'Flight'
            || searchDetails.SearchMode === 'FlightPlusHotel'
            || searchDetails.SearchMode === 'Transfer') {
            searchUrl = `${searchUrl}/${searchDetails.DepartureType}/${searchDetails.DepartureId}`;
        }

        let arrivalId = searchDetails.ArrivalId;
        const idAdapter
            = SearchConstants.ID_ADAPTERS.filter(ida => ida.type === searchDetails.ArrivalType)[0];
        if (idAdapter) {
            arrivalId = idAdapter.inverseShift(arrivalId);
        }

        searchUrl = `${searchUrl}/${searchDetails.ArrivalType}/${arrivalId}`;
        searchUrl = `${searchUrl}/${moment(searchDetails.DepartureDate).format('ll')}`;
        searchUrl = `${searchUrl}/${searchDetails.DepartureTime}`;
        searchUrl = `${searchUrl}/${searchDetails.Duration}`;
        searchUrl = `${searchUrl}/${searchDetails.ReturnTime}`;
        let adults = '';
        let children = '';
        let childAges = '';
        let infants = '';

        searchDetails.Rooms.forEach(room => {
            adults = adults !== '' ? `${adults}_${room.Adults}` : `${room.Adults}`;
            children = children !== '' ? `${children}_${room.Children}` : `${room.Children}`;
            infants = infants !== '' ? `${infants}_${room.Infants}` : `${room.Infants}`;

            let roomChildAges = '';
            room.ChildAges.forEach(childAge => {
                roomChildAges = roomChildAges !== ''
                    ? `${roomChildAges}-${childAge}` : `${childAge}`;
            });
            childAges = childAges !== '' ? `${childAges}_${roomChildAges}` : `${roomChildAges}`;
        });

        if (searchDetails.SearchMode !== 'Flight') {
            searchUrl = `${searchUrl}/${searchDetails.Rooms.length}`;
        }

        childAges = childAges ? childAges : '0';

        searchUrl = `${searchUrl}/${adults}/${children}/${infants}/${childAges}`;

        if (searchDetails.SearchMode === 'FlightPlusHotel') {
            searchUrl = `${searchUrl}?packagesearch=${searchDetails.PackageSearch}`;
        }

        if (searchDetails.SearchMode === 'FlightPlusHotel'
                || searchDetails.SearchMode === 'Flight') {
            searchUrl += searchUrl.indexOf('?') > -1 ? '&' : '?';
            searchUrl += `flightclassid=${searchDetails.FlightClassId}`;
        }

        return searchUrl;
    }
    urlToSearchDetails(url) {
        const searchMode = url.split('/')[1];
        let searchDetails = {};

        switch (searchMode.toLowerCase()) {
            case 'hotel':
                searchDetails = this.urlToSearchDetailsHotel(url);
                break;
            case 'flight':
                searchDetails = this.urlToSearchDetailsFlight(url);
                break;
            case 'flightplushotel':
            case 'package':
                searchDetails = this.urlToSearchDetailsFlightPlusHotel(url);
                break;
            default:
        }
        return searchDetails;
    }
    urlToSearchDetailsHotel(url) {
        const searchDetails = this.defaultSearchDetails();

        searchDetails.SearchMode = 'Hotel';

        searchDetails.ArrivalType = url.split('/')[2];
        searchDetails.ArrivalId = parseInt(url.split('/')[3], 10);

        const dateString = url.split('/')[4];
        searchDetails.DepartureDate = this.datestringToDate(dateString);
        searchDetails.Duration = parseInt(url.split('/')[5], 10);

        const rooms = parseInt(url.split('/')[6], 10);
        const adults = url.split('/')[7].split('_');
        const children = url.split('/')[8].split('_');
        const infants = url.split('/')[9].split('_');
        const childAges = url.split('/')[10].split('_');

        searchDetails.Rooms = [];
        for (let i = 0; i < rooms; i++) {
            const room = {
                Id: i + 1,
                Adults: adults[i] ? parseInt(adults[i], 10) : 0,
                Children: children[i] ? parseInt(children[i], 10) : 0,
                Infants: infants[i] ? parseInt(infants[i], 10) : 0,
                ChildAges: childAges[i] ? childAges[i].split('-') : [],
            };
            searchDetails.Rooms.push(room);
        }
        const mealBasisId = UrlFunctions.getQueryStringValue('mealbasisid');
        searchDetails.MealBasisId = mealBasisId ? mealBasisId : 0;

        const minRating = UrlFunctions.getQueryStringValue('minrating');
        searchDetails.MinRating = minRating ? minRating : 0;

        const brandOverrideId = UrlFunctions.getQueryStringValue('bid');
        searchDetails.BrandOverride = brandOverrideId ? parseInt(brandOverrideId, 10) : 0;

        return searchDetails;
    }
    urlToSearchDetailsFlight(url) {
        const searchDetails = this.defaultSearchDetails();

        searchDetails.SearchMode = 'Flight';

        searchDetails.DepartureType = url.split('/')[2];
        searchDetails.DepartureId = parseInt(url.split('/')[3], 10);

        searchDetails.ArrivalType = url.split('/')[4];
        searchDetails.ArrivalId = parseInt(url.split('/')[5], 10);

        const dateString = url.split('/')[6];
        searchDetails.DepartureDate = this.datestringToDate(dateString);
        searchDetails.Duration = parseInt(url.split('/')[7], 10);

        searchDetails.Rooms = [];
        const room = {
            Id: 1,
            Adults: parseInt(url.split('/')[8], 10),
            Children: parseInt(url.split('/')[9], 10),
            Infants: parseInt(url.split('/')[10], 10),
            ChildAges: url.split('/')[11].split('-'),
        };
        searchDetails.Rooms.push(room);

        const flightClassId = UrlFunctions.getQueryStringValue('flightclassid');
        searchDetails.FlightClassId = flightClassId ? parseInt(flightClassId, 10) : 0;

        const brandOverrideId = UrlFunctions.getQueryStringValue('bid');
        searchDetails.BrandOverride = brandOverrideId ? parseInt(brandOverrideId, 10) : 0;

        return searchDetails;
    }
    urlToSearchDetailsFlightPlusHotel(url) {
        const searchDetails = this.defaultSearchDetails();
        const searchMode = url.split('/')[1];

        searchDetails.SearchMode = 'FlightPlusHotel';
        searchDetails.PackageSearch = searchMode.toLowerCase() === 'package';

        searchDetails.DepartureType = url.split('/')[2];
        searchDetails.DepartureId = parseInt(url.split('/')[3], 10);

        searchDetails.ArrivalType = url.split('/')[4];
        searchDetails.ArrivalId = parseInt(url.split('/')[5], 10);

        const dateString = url.split('/')[6];
        searchDetails.DepartureDate = this.datestringToDate(dateString);
        searchDetails.Duration = parseInt(url.split('/')[7], 10);

        const rooms = parseInt(url.split('/')[8], 10);
        const adults = url.split('/')[9].split('_');
        const children = url.split('/')[10].split('_');
        const infants = url.split('/')[11].split('_');
        const childAges = url.split('/')[12].split('_');

        searchDetails.Rooms = [];
        for (let i = 0; i < rooms; i++) {
            const room = {
                Id: i + 1,
                Adults: adults[i] ? parseInt(adults[i], 10) : 0,
                Children: children[i] ? parseInt(children[i], 10) : 0,
                Infants: infants[i] ? parseInt(infants[i], 10) : 0,
                ChildAges: childAges[i] ? childAges[i].split('-') : [],
            };
            searchDetails.Rooms.push(room);
        }
        const mealBasisId = UrlFunctions.getQueryStringValue('mealbasisid');
        searchDetails.MealBasisId = mealBasisId ? mealBasisId : 0;

        const minRating = UrlFunctions.getQueryStringValue('minrating');
        searchDetails.MinRating = minRating ? minRating : 0;

        const flightClassId = UrlFunctions.getQueryStringValue('flightclassid');
        searchDetails.FlightClassId = flightClassId ? parseInt(flightClassId, 10) : 0;

        const brandOverrideId = UrlFunctions.getQueryStringValue('bid');
        searchDetails.BrandOverride = brandOverrideId ? parseInt(brandOverrideId, 10) : 0;

        return searchDetails;
    }
    datestringToDate(dateString) {
        const year = dateString.split('-')[0];
        const month = dateString.split('-')[1];
        const day = dateString.split('-')[2];
        return new Date(year, month - 1, day);
    }
}

export default SearchAPI;
