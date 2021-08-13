import * as RedirectActions from 'actions/content/redirectActions';
import * as RedirectWarnings from 'constants/RedirectWarnings';

import ModalPopup from 'components/common/modalpopup';
import React from 'react';
import RedirectEdit from 'widgets/common/redirectedit';
import RedirectList from 'widgets/common/redirectlist';
import StringFunctions from 'library/stringfunctions';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class RedirectEditContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            widgetEdit: false,
            entityListEdit: false,
            entity: {},
            errors: {},
            valid: true,
            displayDeletePrompt: false,
            deleteItem: {
                key: '',
                entity: '',
                context: '',
                site: '',
                isConfirmed: false,
            },
            newContextCheck: {
                entity: '',
                key: '',
            },
            contextFilter: '',
        };
        this.handleClose = this.handleClose.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
        this.handleAddNewItem = this.handleAddNewItem.bind(this);
        this.handleRedirectItemClick = this.handleRedirectItemClick.bind(this);
        this.handleSave = this.handleSave.bind(this);
        this.handleUpdate = this.handleUpdate.bind(this);
        this.handleDelete = this.handleDelete.bind(this);
        this.handleFilter = this.handleFilter.bind(this);
    }
    componentDidMount() {
        this.props.actions.loadRedirectList();
    }
    validateAddRedirect(redirect) {
        let valid = true;
        this.props.actions.resetRedirectWarnings();

        const previousURL = redirect.Url;
        const currentURL = redirect.RedirectUrl;
        const redirectID = redirect.RedirectId;

        // If there is already an entry with this as the previous URL do not allow it.
        if (this.props.redirects.redirects.filter(r => r.Url === previousURL
            && r.RedirectId !== redirectID).length > 0) {
            this.props.actions.addRedirectWarning('Url', RedirectWarnings.WARNING_DUPLICATE_URL);
            valid = false;
        }

        // If there is already an entry with this as the current url,
        // do not set one up with this as the previous.
        if (this.props.redirects.redirects.filter(r => r.RedirectUrl === previousURL
            && r.RedirectId !== redirectID).length > 0) {
            this.props.actions.addRedirectWarning('Url', RedirectWarnings.WARNING_ALREADY_CURRENT);
            valid = false;
        }

        // If there is already an entry with this as the previous url,
        // do not set one up with this as the current.
        if (this.props.redirects.redirects.filter(r => r.Url === currentURL
            && r.RedirectId !== redirectID && r.RedirectId !== redirectID).length > 0) {
            this.props.actions.addRedirectWarning('RedirectUrl',
               RedirectWarnings.WARNING_ALREADY_PREVIOUS);
            valid = false;
        }

        // Cannot have the previous url be the same as current
        if (previousURL === currentURL) {
            this.props.actions.addRedirectWarning('Url', RedirectWarnings.WARNING_THE_SAME);
            this.props.actions.addRedirectWarning('RedirectUrl', RedirectWarnings.WARNING_THE_SAME);
            valid = false;
        }

        if (previousURL.indexOf('/') !== 0) {
            this.props.actions.addRedirectWarning('Url', RedirectWarnings.WARNING_MISSING_SLASH);
            valid = false;
        }

        if (currentURL.indexOf('/') !== 0) {
            const warning = RedirectWarnings.WARNING_MISSING_SLASH;
            this.props.actions.addRedirectWarning('RedirectUrl', warning);
            valid = false;
        }

        if (previousURL === '') {
            this.props.actions.addRedirectWarning('Url', RedirectWarnings.WARNING_FIELD_BLANK);
            valid = false;
        }

        if (currentURL === '') {
            this.props.actions.addRedirectWarning('RedirectUrl',
                RedirectWarnings.WARNING_FIELD_BLANK);
            valid = false;
        }

        return valid;
    }


    handleClose() {
        this.props.actions.cancelDisplayRedirects();
    }
    handleSave() {
        const redirect = this.props.redirects.selectedRedirect;

        if (this.validateAddRedirect(redirect)) {
            if (redirect.RedirectId > 0) {
                this.props.actions.modifyRedirect(redirect, this.props.site.Name);
            } else {
                this.props.actions.addRedirect(redirect, this.props.site.Name);
            }
        }
    }
    handleDelete() {
        const redirect = this.props.redirects.selectedRedirect;

        this.props.actions.deleteRedirect(redirect, this.props.site.Name);
    }
    handleUpdate(event) {
        const field = event.target.name;
        const value = event.target.value;

        this.props.actions.updateEntityValue(field, value);
    }
    handleCancel() {
        this.props.actions.cancelEditRedirects();
    }
    handleAddNewItem() {
        this.props.actions.editRedirects();
    }
    handleRedirectItemClick(redirectId) {
        this.props.actions.selectRedirect(redirectId);
    }
    handleFilter(event) {
        this.setState({ contextFilter: event.target.value });
    }
    getRedirectEditProps() {
        const redirectEditProps = {
            onCancel: this.handleCancel,
            onClose: this.handleClose,
            onChange: this.handleUpdate,
            onSave: this.handleSave,
            onDelete: this.handleDelete,
            siteName: StringFunctions.safeUrl(this.props.site.Name),
            redirects: this.props.redirects,
        };
        return redirectEditProps;
    }
    getRedirectListProps() {
        const contextFilter = this.state.contextFilter.toLowerCase();

        const redirects = this.props.redirects.redirects.filter(redirect =>
            (redirect.Url.toLowerCase().indexOf(contextFilter) > -1)
            || (redirect.RedirectUrl.toLowerCase().indexOf(contextFilter) > -1));

        const redirectEditProps = {
            redirects,
            selectedRedirectID: this.props.redirects.selectedRedirect.RedirectId,
            onAddNewItem: this.handleAddNewItem,
            onRedirectItemClick: this.handleRedirectItemClick,
            onChange: this.handleFilter,
            filterValue: this.state.contextFilter,
        };
        return redirectEditProps;
    }
    render() {
        let containerClass = 'redirect-edit-container redirect-list-view';
        if (this.props.redirects.isEditing) {
            containerClass = `${containerClass} redirect-list-view`;
        }
        return (
            <div>
                {this.props.redirects.isDisplaying
                    && <ModalPopup>
                           <div className={containerClass}>
                               <div className="redirect-list-wrapper">
                                   <nav className="redirect-list-nav">
                                       <RedirectList {...this.getRedirectListProps()} />
                                   </nav>
                               </div>
                               <RedirectEdit {...this.getRedirectEditProps()} />
                           </div>
                    </ModalPopup>}
            </div>
        );
    }
}

RedirectEditContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    redirects: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
};

/**
* map state to props
* @param {object} state - The current state.
* @return {object} the props mapped to state.
*/
function mapStateToProps(state) {
    const redirects = state.redirects ? state.redirects : {};
    return {
        redirects,
    };
}

/**
* map dispatch to props
* @param {object} dispatch - redux dispatch
* @return {object} entity actions
*/
function mapDispatchToProps(dispatch) {
    return {
        actions: bindActionCreators(RedirectActions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(RedirectEditContainer);
