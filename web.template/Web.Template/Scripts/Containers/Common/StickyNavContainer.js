import React from 'react';
import StickyNav from '../../widgets/common/stickynav';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class StickyNavContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isSticky: false,
            newWidgets: false,
        };
        this.handleScroll = this.handleScroll.bind(this);
    }
    componentDidMount() {
        window.addEventListener('scroll', this.handleScroll);
    }
    componentWillUnmount() {
        window.removeEventListener('scroll', this.handleScroll);
    }
    componentWillReceiveProps(nextProps) {
        const oldKeys = Object.keys(this.props.entities);
        const newKeys = Object.keys(nextProps.entities);

        let oldLoadedEntities = 0;
        let newLoadedEntities = 0;
        oldKeys.forEach(k => {
            if (this.props.entities[k].isLoaded) {
                oldLoadedEntities += 1;
            }
        });
        newKeys.forEach(k => {
            if (nextProps.entities[k].isLoaded) {
                newLoadedEntities += 1;
            }
        });
        if ((oldLoadedEntities === newLoadedEntities) && this.state.newWidgets) {
            this.setState({ newWidgets: false });
        }
        if (oldLoadedEntities !== newLoadedEntities) {
            this.setState({ newWidgets: true });
        }
    }
    handleScroll() {
        this.setStickyNavStyling();
    }
    setStickyNavStyling() {
        const stickyNavAbsolutePositionY = this.getStickyNavAbsolutePositionY();
        const scrollY = window.scrollY || window.pageYOffset;
        const correctIsStickyValue = scrollY > stickyNavAbsolutePositionY;
        if (correctIsStickyValue !== this.state.isSticky) {
            this.setState({
                isSticky: correctIsStickyValue,
            });
        }
    }
    getStickyNavAbsolutePositionY() {
        const element = document.getElementById(`widget-stickynav-container-${this.props.context}`);
        const elementPosition = element.getBoundingClientRect();
        const scrollY = window.scrollY || window.pageYOffset;
        return (
            elementPosition.top + scrollY
        );
    }
    render() {
        const contentModel = this.props.entity.model;
        const containerAttributes = {
            className: 'widget-sticky-nav',
        };
        if (this.state.isSticky) {
            containerAttributes.className = `${containerAttributes.className} sticky-nav-activated`;
        }
        const stickyNavProps = {
            navItems: contentModel.NavItems,
            navBookButton: contentModel.NavBookButton,
            containerId: `widget-stickynav-container-${this.props.context}`,
            newWidgets: this.state.newWidgets,
            userLoggedIn: this.props.session.UserSession.LoggedIn,
            entityInformations: this.props.page.EntityInformations,
        };
        return (
            <div {...containerAttributes}>
                <div className="container">
                    <StickyNav {...stickyNavProps} />
                </div>
            </div>
        );
    }
}

StickyNavContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    entities: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    page: React.PropTypes.object,
};


/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const entities = state.entities ? state.entities : {};
    return {
        entities,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = {};
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(StickyNavContainer);
