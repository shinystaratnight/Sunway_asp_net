import MarkdownText from 'components/common/MarkdownText';
import React from 'react';

export default class TextPanel extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }

    render() {
        const markdownProps = {
            markdown: this.props.markdown,
        };

        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h3 className="h-tertiary">{this.props.title}</h3>
                </header>
                <div className="panel-body">
                    <MarkdownText {...markdownProps}/>
                </div>
            </div>
        );
    }
}

TextPanel.propTypes = {
    title: React.PropTypes.string,
    markdown: React.PropTypes.string,
};
