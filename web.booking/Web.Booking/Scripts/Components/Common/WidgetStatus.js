import React from 'react';

export default class WidgetStatusPopup extends React.Component {
    constructor() {
        super();
        this.state = {
            showPopup: false,
        };
        this.togglePopup = this.togglePopup.bind(this);
    }
    togglePopup() {
        this.setState({ showPopup: !this.state.showPopup });
    }
    displayName(name) {
        let displayName = name.replace(/([A-Z][a-z])/g, ' $1');
        displayName = displayName.replace(/^./, str => str.toUpperCase());
        return displayName;
    }
    displayDate(date) {
        const day = date.getDate();
        const monthNumber = date.getMonth();
        const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May',
            'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        const year = date.getFullYear();
        const hour = date.getHours();
        const minutes = date.getMinutes() < 10 ? `0${date.getMinutes()}` : date.getMinutes();
        const displayDate = `${day} ${months[monthNumber]} ${year} ${hour}:${minutes}`;
        return (
            <time dateTime={date.toString()}>{displayDate}</time>
        );
    }
    render() {
        const status = this.props.status ? this.props.status : '';
        const statusClass = status.replace(' ', '-').toLowerCase();
        const containerClass = `widget-status-content ${statusClass}`;
        return (
            <div className={containerClass}>
                <div className="widget-status" onClick={this.togglePopup}></div>
                {this.state.showPopup
                    && <div className="widget-info">
                        <ul>
                            <li className="title">{this.displayName(this.props.name)}</li>
                            <li className="context">{this.props.context}</li>
                            <li><span className="status">{status}</span></li>
                            <li>
                                <span className="fa fa-calendar"></span>
                                Modified {this.displayDate(this.props.lastModifiedDate)}
                            </li>
                        </ul>
                        <button className="btn btn-primary btn-xs btn-edit"
                                onClick={this.props.onEdit}>
                            <span className="fa fa-pencil"></span>Edit
                        </button>
                        <button className="btn btn-primary btn-xs btn-publish"
                            onClick={this.props.onPublish}>
                            <span className="fa fa-share"></span>Publish
                        </button>
                    </div>
                }
            </div>
        );
    }
}

WidgetStatusPopup.propTypes = {
    name: React.PropTypes.string.isRequired,
    context: React.PropTypes.string.isRequired,
    status: React.PropTypes.string.isRequired,
    lastModifiedDate: React.PropTypes.object.isRequired,
    lastModifiedUser: React.PropTypes.string.isRequired,
    onEdit: React.PropTypes.func.isRequired,
    onPublish: React.PropTypes.func.isRequired,
};
