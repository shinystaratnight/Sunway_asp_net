import brands from './lookups/brandsReducer';
import { combineReducers } from 'redux';
import directDebit from './mmb/directDebitReducer';
import emailForm from './mmb/emailReducer';
import entities from './content/entitiesReducer';
import entityList from './content/entityListReducer';
import mmb from './mmb/mmbReducer';
import page from './content/pageReducer';
import quotes from './mmb/quotesReducer';
import session from './bookingjourney/userReducer';
import site from './bookingjourney/siteReducer';
import siteSearch from './content/siteSearchReducer';

const rootReducer = combineReducers({
    brands,
    directDebit,
    emailForm,
    entities,
    entityList,
    mmb,
    page,
    quotes,
    session,
    site,
    siteSearch,
});

export default rootReducer;
