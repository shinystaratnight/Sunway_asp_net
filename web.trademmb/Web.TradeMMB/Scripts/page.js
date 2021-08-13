import { Provider } from 'react-redux';
import React from 'react';
import ReactDOM from 'react-dom';
import WidgetContainer from './containers/common/widgetcontainer';

import configureStore from './store/configureStore';

import { errorLog } from './actions/common/errorActions';
import { loadPage } from './actions/content/pageActions';
import { loadSite } from './actions/bookingjourney/siteActions';
import { loadUser } from './actions/bookingjourney/userActions';

let pageInstance = null;
class Page {
    constructor(customerContainers) {
        if (!pageInstance) {
            pageInstance = this;
        }

        this.customerContainers = customerContainers;

        const preloadedState = JSON.parse(window.__PRELOADED_STATE__);
        Object.keys(preloadedState.entities).map((key) => {
            preloadedState.entities[key].jsonSchema
                = JSON.parse(preloadedState.entities[key].jsonSchema);
            preloadedState.entities[key].model
                = JSON.parse(preloadedState.entities[key].model);
            preloadedState.entities[key].lastModifiedDate
                = this.parseISOLocal(preloadedState.entities[key].lastModifiedDate);
        });

        this.store = configureStore(preloadedState);

        this.store.dispatch(loadUser());
        this.store.dispatch(loadSite());
        this.store.dispatch(loadPage());

        this.unsubscribe = null;
        this.handleStoreChange = this.handleStoreChange.bind(this);
    }
    render() {
        this.unsubscribe = this.store.subscribe(this.handleStoreChange);
    }
    handleStoreChange() {
        const state = this.store.getState();
        if (state.page.isLoaded && state.session.isLoaded && state.site.isLoaded) {
            this.unsubscribe();
            this.renderWidgets();
        }
    }
    renderWidget(widget, siteName) {
        const entityName = widget.Name;
        const context = widget.Context ? widget.Context : '';
        console.log('Widget: ', widget);
        const widgetContainerProps = {
            entityName,
            context: widget.Shared ? `${context}-${siteName.toLowerCase()}` : context,
            entitySpecific: widget.EntitySpecific ? widget.EntitySpecific : false,
            editable: !widget.NotEditable,
            shared: widget.Shared ? widget.Shared : false,
            customerContainers: this.customerContainers,
        };

        let containerId = `widget-${entityName.toLowerCase()}-container`;
        if (context !== '') {
            containerId = `${containerId}-${context}`;
        }

        try {
            ReactDOM.render(
                <Provider store={this.store}>
                    <WidgetContainer {...widgetContainerProps} />
                </Provider>,
                document.getElementById(containerId)
            );
        } catch (error) {
            this.store.dispatch(errorLog(error.message));
        }
    }

    renderWidgets() {
        const state = this.store.getState();
        const adminMode = state.session.UserSession.AdminMode;
        const siteName = state.site.Name;
        console.log('Widgets: ', state.page.Widgets);
        state.page.Widgets.forEach(widget => {
            if ((widget.ClientSideRender
                && (widget.AccessDisplay !== 'AdminLoggedIn' || adminMode))
                || ((adminMode && widget.ServerSideRender)
                && !widget.NotEditable)) {
                this.renderWidget(widget, siteName);
            }
        });
    }
    /**
     * Parse ISO date string format into date object
     * @param {string} s - an ISO 8001 format date and time string
     * with all components, e.g. 2015-11-24T19:40:00
     * @returns {Date} - Date instance from parsing the string.
     */
    parseISOLocal(s) {
        const b = s.split(/\D/);
        return new Date(b[0], b[1] - 1, b[2], b[3], b[4], b[5]);
    }
}

export default Page;
