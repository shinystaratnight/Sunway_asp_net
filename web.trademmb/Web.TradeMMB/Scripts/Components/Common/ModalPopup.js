import React from 'react';

export default class ModalPopup extends React.Component {
    constructor() {
        super();
        this.state = {
            transitionEnd: false,
        };
        this.transitionTimeout = {};
        this.transitionTime = 100;
        this.setTransitionEnd = this.setTransitionEnd.bind(this);
    }
    componentDidMount() {
        document.body.classList.add('modal-open');
        this.transitionTimeout = setTimeout(this.setTransitionEnd, this.transitionTime);
    }
    componentWillUnmount() {
        document.body.classList.remove('modal-open');
    }
    setTransitionEnd() {
        this.setState({ transitionEnd: true });
    }
    render() {
        let containerClass = 'modal fade';
        containerClass = this.state.transitionEnd ? `${containerClass} show` : containerClass;

        let backdropClass = 'modal-backdrop fade';
        backdropClass = this.state.transitionEnd ? `${backdropClass} show` : backdropClass;

        return (
            <div>
                <div className={containerClass}>
                    { this.props.children }
                </div>
                <div className={backdropClass}></div>
            </div>
        );
    }
}

ModalPopup.propTypes = {
    children: React.PropTypes.object.isRequired,
};
