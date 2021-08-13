import 'main.scss';

import ExportWidgetContainer from './containers/common/exportWidgetContainer';
import { Provider } from 'react-redux';
import React from 'react';
import ReactDOM from 'react-dom';

import configureStore from 'store/configureStore';

import { loadSite } from 'actions/bookingjourney/siteActions';
import { loadUser } from 'actions/bookingjourney/userActions';

let pageInstance = null;

class ExportPage {
    constructor(customerContainers) {
        if (!pageInstance) {
            pageInstance = this;
        }
        this.customerContainers = customerContainers;
        this.store = configureStore();

        this.store.dispatch(loadUser());
        this.store.dispatch(loadSite());

        this.unsubscribe = null;
        this.handleStoreChange = this.handleStoreChange.bind(this);
    }
    getHeaderProps() {
        const props = {
            entityName: 'Header',
            context: 'default',
            shared: false,
            customerContainers: this.customerContainers,
        };
        return props;
    }
    getFooterProps() {
        const props = {
            entityName: 'Footer',
            context: 'default',
            shared: false,
            customerContainers: this.customerContainers,
        };
        return props;
    }
    render() {
        this.unsubscribe = this.store.subscribe(this.handleStoreChange);
    }
    handleStoreChange() {
        const state = this.store.getState();
        if (state.session.isLoaded && state.site.isLoaded) {
            this.unsubscribe();
            this.renderWidgets();
        }
    }
    renderWidgets() {
        ReactDOM.render(
        <Provider store={this.store}>
            <ExportWidgetContainer {...this.getHeaderProps()} />
        </Provider>,
        document.getElementById('divHeader'));

        ReactDOM.render(
        <Provider store={this.store}>
            <ExportWidgetContainer {...this.getFooterProps()} />
        </Provider>,
        document.getElementById('divFooter'));
    }
}

export default ExportPage;
