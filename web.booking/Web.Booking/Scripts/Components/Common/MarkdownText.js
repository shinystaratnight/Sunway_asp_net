import CommonMark from 'commonmark';
import React from 'react';
import ReactRenderer from 'commonmark-react-renderer';

export default class MarkdownText extends React.Component {
    constructor() {
        super();
        this.state = {
        };
    }
    render() {
        const parser = new CommonMark.Parser();
        const renderer = new ReactRenderer();
        const textToRender = this.props.markdown.length > this.props.maxLength
            ? this.props.markdown.substring(0, this.props.maxLength).concat('...')
            : this.props.markdown;
        const ast = parser.parse(textToRender.replace(/(#)([a-z])/gi, '$1 $2'));
        const result = renderer.render(ast);

        let containerClass = 'rendered-markdown';
        if (this.props.textColumns > 1) {
            containerClass = `${containerClass} text-col-${this.props.textColumns}`;
        }
        containerClass = this.props.containerStyle
            ? `${containerClass} ${this.props.containerStyle}`
            : containerClass;

        return (
            <div className={containerClass}>
                {result}
            </div>
        );
    }
}

MarkdownText.propTypes = {
    markdown: React.PropTypes.string.isRequired,
    textColumns: React.PropTypes.number,
    containerStyle: React.PropTypes.string,
    maxLength: React.PropTypes.number,
};

MarkdownText.defaultProps = {
    markdown: '',
    textColumns: 1,
    maxLength: 100000,
};
