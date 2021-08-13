import '../../../styles/widgets/content/_keyvalueitems.scss';

import MarkdownText from '../../components/common/MarkdownText';
import React from 'react';

export default class KeyValueItems extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.renderKeyValueRow = this.renderKeyValueRow.bind(this);
        this.renderKeyValueItem = this.renderKeyValueItem.bind(this);
        this.renderButton = this.renderButton.bind(this);
    }
    renderKeyValueRow(keyValueItems, index) {
        const key = `keyvalueitems-${index}`;
        let rowPadding = '';
        switch (this.props.configuration.KeyValuePadding) {
            case 'Small':
                rowPadding = 'row-padding-sm';
                break;
            case 'Medium':
                rowPadding = 'row-padding-md';
                break;
            case 'Large':
                rowPadding = 'row-padding-lg';
                break;
            default:
        }
        const keyValueRowClasses = `key-value-row ${rowPadding}`;

        return (
            <div key={key} className={keyValueRowClasses}>
                <div className="container">
                    <div className="row">
                        {keyValueItems.map(this.renderKeyValueItem, this)}
                    </div>
                </div>
            </div>
        );
    }
    getHighlightClass(configuration) {
        let highlightClass = 'default';
        switch (configuration) {
            case 'Highlight':
                highlightClass = 'highlight';
                break;
            case 'Alternate':
                highlightClass = 'alt';
                break;
            default:
        }
        return (
            highlightClass
        );
    }
    renderButton() {
        let btnClass = '';

        switch (this.props.button.Style) {
            case 'Arrow':
                btnClass = `${this.props.button.Style.toLowerCase()}-link`;
                break;
            default:
                btnClass = 'btn btn-large btn-default';
        }

        return (
            <div className="row center-block">
                <a className={btnClass} href={this.props.button.URL}>
                    {this.props.button.Text}
                </a>
            </div>
        );
    }
    renderKeyValueItem(keyValueItem, index) {
        const key = `keyvalueitem-${index}`;
        const numberOfColumns = this.props.configuration.NumberOfColumns;
        let keyClasses = '';
        let valueClasses = '';
        switch (numberOfColumns) {
            case 1:
                keyClasses = 'col-xs-4 col-md-3';
                valueClasses = 'col-xs-8 col-md-9';
                break;
            default:
                keyClasses = 'col-xs-4 col-md-2';
                valueClasses = 'col-xs-8 col-md-4';
        }
        return (
            <div key={key} className="key-value-item">
                <div className="key-value-inner-container">
                    <div className={keyClasses}>{keyValueItem.Key}</div>
                    <div className={valueClasses}>
                        <MarkdownText markdown={keyValueItem.Value} />
                    </div>
                </div>
            </div>
        );
    }
    sortKeyValueItems(keyValueItemA, keyValueItemB) {
        return (
            (keyValueItemA.Sequence - keyValueItemB.Sequence)
        );
    }
    filterKeyValueItem(keyValueItem) {
        return (
            !keyValueItem.Hidden
        );
    }
    render() {
        let sortedKeyValueItems = this.props.keyValueItems.filter(this.filterKeyValueItem);
        sortedKeyValueItems = sortedKeyValueItems.sort(this.sortKeyValueItems);
        const numberOfColumns = this.props.configuration.NumberOfColumns;
        const numRows = Math.ceil(sortedKeyValueItems.length / numberOfColumns);
        const rows = [];
        for (let i = 0; i < numRows; i++) {
            const row = [];
            for (let j = 0; j < numberOfColumns; j++) {
                const index = (numberOfColumns * i) + j;
                if (index < sortedKeyValueItems.length) {
                    const item = sortedKeyValueItems[(numberOfColumns * i) + j];
                    row.push(item);
                }
            }
            rows.push(row);
        }
        const highlightedRowStyle
            = this.getHighlightClass(this.props.configuration.HighlightedRowStyle);
        const mainClasses = `row-highlighted-${highlightedRowStyle}`;
        return (
            <div className={mainClasses}>
                {(this.props.title !== '' || this.props.leadParagraph !== '')
                && <div className="container center-block">
                       <div className="row">
                           <div className="col-xs-12">
                               {this.props.title !== ''
                               && <h2 className="h-secondary">
                                      {this.props.title}
                                  </h2>}
                               {this.props.leadParagraph !== ''
                               && <p className="preamble">
                                      {this.props.leadParagraph}
                                  </p>}
                            </div>
                        </div>
                    </div>}
                    {rows.map(this.renderKeyValueRow, this)}

                    {this.props.button.Text
                     && this.renderButton()}
            </div>
        );
    }
}


KeyValueItems.propTypes = {
    title: React.PropTypes.string,
    leadParagraph: React.PropTypes.string,
    keyValueItems: React.PropTypes.arrayOf(
        React.PropTypes.shape({
            Key: React.PropTypes.string,
            Value: React.PropTypes.string,
            Sequence: React.PropTypes.number,
            Hidden: React.PropTypes.boolean,
        })
    ),
    button: React.PropTypes.shape({
        Text: React.PropTypes.string,
        URL: React.PropTypes.string,
        Style: React.PropTypes.string,
    }),
    configuration: React.PropTypes.shape({
        ContainerStyle: React.PropTypes.string,
        HighlightedRowStyle: React.PropTypes.string,
        ContainerPadding: React.PropTypes.string,
        KeyValuePadding: React.PropTypes.string,
        NumberOfColumns: React.PropTypes.number,
    }),
};
