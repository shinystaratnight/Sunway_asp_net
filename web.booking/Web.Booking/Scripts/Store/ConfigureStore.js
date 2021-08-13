import { applyMiddleware, compose, createStore } from 'redux';
import config from 'config';
import rootReducer from '../reducers';
import thunk from 'redux-thunk';

/**
 * configure redux store
 * @param {object} initialState - initial state of store
 * @return {object} redux store
 */
export default function configureStore(initialState) {
    const store = createStore(rootReducer, initialState, compose(
        applyMiddleware(thunk),
        window.devToolsExtension && config.environment === 'development'
        ? window.devToolsExtension() : f => f
    ));
    return store;
}
