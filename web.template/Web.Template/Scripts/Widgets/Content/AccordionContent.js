import '../../../styles/widgets/content/_accordioncontent.scss';

import MarkdownText from '../../components/common/MarkdownText';
import React from 'react';

export default class AccordionContent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            itemsToDisplay: this.props.configuration.ItemsToShowAsDefault,
            indexOfItemsToExpand: [0],
            showAll: false,
        };
        this.renderAccordionItem = this.renderAccordionItem.bind(this);
        this.renderButton = this.renderButton.bind(this);
        this.toggleShowAll = this.toggleShowAll.bind(this);
        this.toggleItemExpand = this.toggleItemExpand.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (nextProps.configuration.ItemsToShowAsDefault !== this.state.itemsToDisplay) {
            this.setState({ itemsToDisplay: nextProps.configuration.ItemsToShowAsDefault });
        }
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
    renderAccordionItem(accordionItem, index) {
        const key = `accordionitem-${index}`;
        const highlightClass
            = this.getHighlightClass(this.props.configuration.HighlightedItemStyle);
        const containerHighlightClass
            = this.getHighlightClass(this.props.configuration.ContainerStyle);
        let itemClasses = `accordion-item container-style-${containerHighlightClass}`;

        // Here we are checking if this is an item that should be displayed
        // If yes, then we check if it should be expanded also
        if (index >= this.state.itemsToDisplay) {
            itemClasses = `${itemClasses} hidden`;
        } else if ((this.state.indexOfItemsToExpand.indexOf(index)) !== -1) {
            itemClasses = `${itemClasses} expanded`;
        } else {
            itemClasses = `${itemClasses} collapsed`;
        }

        // Here we are checking if this item should be highlighted
        const everyOtherRow = 2;
        if ((index % everyOtherRow) === 0) {
            itemClasses = `${itemClasses} ${highlightClass}`;
        }
        return (
            <div key={key} className={itemClasses}
                 onClick={() => { this.toggleItemExpand(index); }}>
                <h3 className="h-tertiary accordion-title">
                    {accordionItem.Title}
                </h3>
                <div className="accordion-content">
                    <MarkdownText markdown={accordionItem.Text} />
                </div>
            </div>
        );
    }
    toggleItemExpand(itemIndex) {
        const newIndexOfItemsToExpand = this.state.indexOfItemsToExpand;
        const oldIndex = newIndexOfItemsToExpand.indexOf(itemIndex);
        if (oldIndex === -1) {
            newIndexOfItemsToExpand.push(itemIndex);
        } else {
            newIndexOfItemsToExpand.splice(oldIndex, 1);
        }
        this.setState({
            indexOfItemsToExpand: newIndexOfItemsToExpand,
        });
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
        const btnRowClass = 'row button-row center-block';
        let buttonText = this.props.button.ShowLessText;
        if (!this.state.showAll) {
            buttonText = this.props.button.ShowAllText;
        }
        return (
            <div className={btnRowClass}>
                <a className={btnClass} onClick={() => { this.toggleShowAll(); }}>
                    {buttonText}
                </a>
            </div>
        );
    }
    toggleShowAll() {
        const showAll = !this.state.showAll;
        const itemsToDisplay = showAll
            ? this.props.accordionContent.length
            : this.props.configuration.ItemsToShowAsDefault;
        this.setState({
            itemsToDisplay,
            showAll,
        });
    }
    getPaddingClass(configuration) {
        let paddingClass = '';
        switch (configuration) {
            case 'Small':
                paddingClass = 'sm';
                break;
            case 'Medium':
                paddingClass = 'md';
                break;
            case 'Large':
                paddingClass = 'lg';
                break;
            default:
                paddingClass = 'none';
        }
        return (
            paddingClass
        );
    }
    sortAccordionContent(accordionItemA, accordionItemB) {
        return (
            (accordionItemA.Sequence - accordionItemB.Sequence)
        );
    }
    filterAccordionContent(accordionItem) {
        return (
            !accordionItem.Hidden
        );
    }
    render() {
        let sortedAccordionContent
            = this.props.accordionContent.filter(this.filterAccordionContent);
        const paddingClass = this.getPaddingClass(this.props.configuration.AccordionItemPadding);
        sortedAccordionContent = sortedAccordionContent.sort(this.sortAccordionContent);
        const mainClasses = `container accordion-item-padding-${paddingClass}`;
        const renderButton = this.props.button
            && this.props.configuration.ItemsToShowAsDefault < sortedAccordionContent.length;
        return (
            <div className={mainClasses}>
                {(this.props.title !== '' || this.props.leadParagraph !== '')
                && <div className="row center-block">
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
                    </div>}
                <div className="row">
                    {sortedAccordionContent.map(this.renderAccordionItem, this)}
                </div>
                {renderButton
                 && this.renderButton()}
            </div>
        );
    }
}


AccordionContent.propTypes = {
    title: React.PropTypes.string,
    leadParagraph: React.PropTypes.string,
    accordionContent: React.PropTypes.arrayOf(
        React.PropTypes.shape({
            Title: React.PropTypes.string,
            Text: React.PropTypes.string,
            Sequence: React.PropTypes.number,
            Hidden: React.PropTypes.boolean,
        })
    ),
    button: React.PropTypes.shape({
        ShowAllText: React.PropTypes.string,
        ShowLessText: React.PropTypes.string,
        Style: React.PropTypes.string,
    }),
    configuration: React.PropTypes.shape({
        ContainerStyle: React.PropTypes.string,
        HighlightedItemStyle: React.PropTypes.string,
        AccordionItemPadding: React.PropTypes.string,
        ItemsToShowAsDefault: React.PropTypes.number,
    }),
};
