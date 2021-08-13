import '../../../styles/widgets/content/_blogListFilter.scss';

import ObjectFunctions from '../../library/objectfunctions';
import React from 'react';
import SelectInput from '../../components/form/selectinput';
import StringFunctions from '../../library/stringfunctions';
import moment from 'moment';

export default class BlogListFilter extends React.Component {
    itemCount(option, filter) {
        const itemCount = this.props.blogItems.filter(blog => {
            if (blog.hasOwnProperty('ValidExcludes')) {
                const exclude = blog.ValidExcludes.filter(validExclude =>
                    validExclude.name === filter.name)[0];
                if (filter.type === 'DATE') {
                    const value = ObjectFunctions.getValueByStringPath(blog, filter.field);
                    const blogDateMoment = moment(value);
                    const monthYearId = parseInt(blogDateMoment.format('YYYYMM'), 10);
                    return exclude.display && monthYearId === option.Id;
                }

                if (filter.type === 'ARRAY') {
                    const value = ObjectFunctions.getValueByStringPath(blog, filter.field);
                    let validItem = false;
                    if (value) {
                        validItem = value.indexOf(option) > -1;
                    }
                    return exclude.display && validItem;
                }

                return exclude.display;
            }
            return true;
        }).length;
        return itemCount;
    }
    renderFilter(filter, index) {
        const key = `[${index}].selectedValue`;
        const selectInputProps = {
            key,
            name: key,
            options: [],
            onChange: this.props.onChange,
            containerClass: `${filter.name} inline`,
            value: filter.selectedValue,
        };
        const validOptions = [];
        filter.options.forEach(option => {
            if (option.Id === filter.defaultValue || option === filter.defaultValue) {
                validOptions.push(option);
            } else {
                const itemCount = this.itemCount(option, filter);
                if (itemCount > 0) {
                    validOptions.push(option);
                }
            }
        });
        selectInputProps.options = validOptions;
        if (filter.type === 'ARRAY') {
            const indexOfValue = validOptions.findIndex(o =>
                StringFunctions.safeUrl(o) === StringFunctions.safeUrl(filter.selectedValue)
            );
            selectInputProps.value = validOptions[indexOfValue];
        }
        return (
            <SelectInput {...selectInputProps} />
        );
    }
    renderFilterSummary() {
        const summaryOptions = this.props.summary;
        let summary = summaryOptions.Text;

        const archiveFilter = this.props.filters.find(filter =>
            filter.name === 'archive');
        let archiveSummary = '';
        if (archiveFilter.selectedValue !== -1) {
            const selectedOption = archiveFilter.options.find(option =>
                option.Id === archiveFilter.selectedValue);
            archiveSummary = summaryOptions.ArchiveSummary
               .replace('##archive##', selectedOption.Name);
        }
        summary = summary.replace('##archive##', archiveSummary);

        const categoryFilter = this.props.filters.find(filter =>
            filter.name === 'category');

        let categorySummary = '';
        const indexOfValue = categoryFilter.options.findIndex(o =>
            StringFunctions.safeUrl(o) === StringFunctions.safeUrl(categoryFilter.selectedValue)
        );
        const actualFilterValue = categoryFilter.options[indexOfValue];

        if (actualFilterValue !== categoryFilter.defaultValue) {
            categorySummary = summaryOptions.CategorySummary
                .replace('##category##', actualFilterValue);
        }
        summary = summary.replace('##category##', categorySummary);

        return (
            <h3 className="h-tertiary blog-filter-summary">{summary}</h3>
        );
    }
    render() {
        return (
            <div className="blog-list-filter">
                <div className="blog-list-filter-options">
                    <h3 className="h-tertiary">{this.props.title}</h3>
                    {this.props.filters.map(this.renderFilter, this)}
                </div>
                {this.renderFilterSummary()}
            </div>
        );
    }
}

BlogListFilter.propTypes = {
    blogItems: React.PropTypes.array.isRequired,
    filters: React.PropTypes.array.isRequired,
    title: React.PropTypes.string.isRequired,
    summary: React.PropTypes.object.isRequired,
    onChange: React.PropTypes.func.isRequired,
};
