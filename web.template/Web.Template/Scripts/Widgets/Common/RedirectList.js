import React from 'react';
import TextInput from '../../components/form/textinput';

class RedirectList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedTab: 'Content',
        };
        this.setSelectedTab = this.setSelectedTab.bind(this);
    }
    setSelectedTab(tab) {
        this.setState({ selectedTab: tab });
    }
    renderRedirectItem(redirect) {
        const key = redirect.Url;
        let tdClass = 'url-item';
        if (this.props.selectedRedirectID === redirect.RedirectId) {
            tdClass += ' selected';
        }
        return (
            <tr key={key} onClick={() => this.props.onRedirectItemClick(redirect.RedirectId)}>
                <td className={tdClass}>{redirect.Url}</td>
                <td className={tdClass}>{redirect.RedirectUrl}</td>
            </tr>
        );
    }
    render() {
        const searchInputProps = {
            name: 'Search',
            type: 'text',
            value: this.props.filterValue,
            onChange: this.props.onChange,
            placeholder: 'Search',
        };

        return (
            <section className="redirect-list">
                <header className="header">
                    <h1>Redirects</h1>
                    <TextInput {...searchInputProps} />
                </header>

                <div className="redirect-list-items">
                    <table className="redirect-list-table-headings">
                        <thead>
                            <tr>
                                <th className="url-item">Previous URL</th>
                                <th className="url-item">Current URL</th>
                            </tr>
                        </thead>
                    </table>
                    <div className="redirect-table-scroll">
                        <table className="redirect-list-table">
                            <tbody>
                                {this.props.redirects.map(this.renderRedirectItem, this)}
                            </tbody>
                        </table>
                    </div>
                </div>
                <footer className="footer clear">
                     {<button type="button"
                                className="btn btn-primary"
                                 onClick={this.props.onAddNewItem}>
                                <span className="fa fa-floppy-o"></span>Add New</button>}
                </footer>
            </section>
        );
    }
}

RedirectList.propTypes = {
    redirects: React.PropTypes.object.isRequired,
    selectedRedirectID: React.PropTypes.number.isRequired,
    onAddNewItem: React.PropTypes.func.isRequired,
    onRedirectItemClick: React.PropTypes.func.isRequired,
    onChange: React.PropTypes.func.isRequired,
    filterValue: React.PropTypes.string,
};

export default RedirectList;
