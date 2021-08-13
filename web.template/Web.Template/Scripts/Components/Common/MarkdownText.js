import CommonMark from 'commonmark';
import React from 'react';
import ReactRenderer from 'vendors/commonmarkReactRenderer';

export default class MarkdownText extends React.Component {
    constructor() {
        super();
        this.state = {
        };
    }
    render() {
        const parser = new CommonMark.Parser();
        const renderer = new ReactRenderer();
        const ast = parser.parse(this.props.markdown);
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
};

MarkdownText.defaultProps = {
    markdown: '',
    textColumns: 1,
};
