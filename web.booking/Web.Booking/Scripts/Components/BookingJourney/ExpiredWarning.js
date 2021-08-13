import React from 'react';

export default class ExpiredWarning extends React.Component {
    constructor() {
        super();
        this.state = {};
    }
    render() {
        const buttonProps = {
            className: 'btn btn-primary',
        };
        if (this.props.onClickFunction) {
            buttonProps.onClick = this.props.onClickFunction;
        } else if (this.props.onClickLink) {
            buttonProps.href = this.props.onClickLink;
        }
        const containerClass = 'panel panel-basic modal-container';

        return (
            <div className={containerClass}>
                <header className="panel-header">
                    <h3 className="h-tertiary">{this.props.title}</h3>
                </header>
                <div className="panel-body">
                    <p>{this.props.body}</p>
                </div>
                <footer className="panel-footer">
                    <a {...buttonProps}>{this.props.button}</a>
                </footer>
            </div>
        );
    }
}

ExpiredWarning.propTypes = {
    onClickLink: React.PropTypes.string,
    onClickFunction: React.PropTypes.func,
    title: React.PropTypes.string.isRequired,
    body: React.PropTypes.string.isRequired,
    button: React.PropTypes.string.isRequired,
};
