import MarkdownText from '../../components/common/MarkdownText';
import React from 'react';

export default class SimpleTextContent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    renderButton(button) {
        let btnClass = 'btn btn-large btn-default mt-3';
        btnClass = this.props.backgroundImage ? `${btnClass} btn-alt` : btnClass;
        return (
            <a className={btnClass} href={button.URL}>
                {button.Text}
            </a>
        );
    }
    renderContent() {
        const containerClasses
            = `container simple-text-content text-align-${this.props.textAlignment.toLowerCase()}`;
        const containerStyle = this.props.backgroundImage ? 'alt' : '';
        const markdownProps = {
            markdown: this.props.markdown,
            containerStyle,
        };
        const renderButton = this.props.button && this.props.button.Text && this.props.button.URL;
        return (
            <div className={containerClasses}>
                <div className="row">
                    <div className="col-xs-12">
                        <MarkdownText {...markdownProps} />
                        {renderButton
                            && this.renderButton(this.props.button)}
                    </div>
                </div>
            </div>
        );
    }
    render() {
        const renderContent = this.props.markdown;
        return (
            <div className="widget-simpletextcontent">
                {renderContent
                    && this.renderContent()}
            </div>
        );
    }
}

SimpleTextContent.propTypes = {
    backgroundImage: React.PropTypes.string,
    markdown: React.PropTypes.string,
    textAlignment: React.PropTypes.string,
    button: React.PropTypes.shape({
        Text: React.PropTypes.string,
        URL: React.PropTypes.string,
    }),
};
