import React from 'react';

export default class Prompt extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            transitionEnd: false,
        };
        this.transitionTimeout = {};
        this.transitionTime = 100;
        this.setTransitionEnd = this.setTransitionEnd.bind(this);
    }
    componentDidMount() {
        this.transitionTimeout = setTimeout(this.setTransitionEnd, this.transitionTime);
    }
    setTransitionEnd() {
        this.setState({ transitionEnd: true });
    }
    render() {
        let containerClass = 'prompt-container fade';
        containerClass = this.state.transitionEnd ? `${containerClass} show` : containerClass;

        const confirmButton = {
            type: 'button',
            className: 'btn btn-primary',
            onClick: this.props.onConfirm,
        };

        const cancelButton = {
            type: 'button',
            className: 'btn btn-default',
            onClick: this.props.onCancel,
        };

        return (
            <div className={containerClass}>
                <div className="prompt panel panel-basic">
                    <header className="panel-header">
                        <h3 className="h-tertiary">{this.props.title}</h3>
                        <span className="close fa fa-times" onClick={this.props.onCancel}></span>
                    </header>
                    <div className="panel-body">
                       <p>{this.props.message}</p>
                    </div>
                    <footer className="panel-footer">
                        <button {...confirmButton}>{this.props.confirmText}</button>
                        <button {...cancelButton}>{this.props.cancelText}</button>
                    </footer>
                </div>
            </div>
        );
    }
}

Prompt.propTypes = {
    title: React.PropTypes.string.isRequired,
    message: React.PropTypes.string.isRequired,
    confirmText: React.PropTypes.string,
    cancelText: React.PropTypes.string,
    onConfirm: React.PropTypes.func.isRequired,
    onCancel: React.PropTypes.func.isRequired,
};

Prompt.defaultProps = {
    confirmText: 'Ok',
    cancelText: 'Cancel',
};
