import React from 'react';
import StickyNav from '../../widgets/common/stickynav';

class StickyNavContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isSticky: false,
        };
        this.handleScroll = this.handleScroll.bind(this);
    }
    componentDidMount() {
        window.addEventListener('scroll', this.handleScroll);
    }
    componentWillUnmount() {
        window.removeEventListener('scroll', this.handleScroll);
    }
    handleScroll() {
        this.setStickyNavStyling();
    }
    setStickyNavStyling() {
        const stickyNavAbsolutePositionY = this.getStickyNavAbsolutePositionY();
        const scrollY = window.scrollY;
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
        return (
            elementPosition.top + window.scrollY
        );
    }
    render() {
        const contentModel = this.props.entity.model;
        const containerAttributes = {
            className: 'widget-sticky-nav',
        };
        if (this.state.isSticky === true) {
            containerAttributes.className = `${containerAttributes.className} sticky-nav-activated`;
        }
        const stickyNavProps = {
            navItems: contentModel.NavItems,
            containerId: `widget-stickynav-container-${this.props.context}`,
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
};


export default StickyNavContainer;
