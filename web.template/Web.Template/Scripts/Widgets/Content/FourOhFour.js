import React from 'react';

export default class FourOhFour extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        return (
            <div className="four-oh-four">
                <h1 className="h-primary">{this.props.title}</h1>
                <h2 className="h-secondary">{this.props.subTitle}</h2>
                <p className="text">{this.props.text}</p>
                <a className="btn btn-default" href={this.props.button.URL}>
                    {this.props.button.Text}
                </a>
            </div>
        );
    }
}

FourOhFour.propTypes = {
    title: React.PropTypes.string,
    subTitle: React.PropTypes.string,
    text: React.PropTypes.string,
    button: React.PropTypes.shape({
        Text: React.PropTypes.string,
        URL: React.PropTypes.string,
    }),
    background: React.PropTypes.string,
};
