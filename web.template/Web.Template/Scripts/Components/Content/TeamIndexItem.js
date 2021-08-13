import React from 'react';
import StringFunctions from '../../library/stringfunctions';

export default class TeamIndexItem extends React.Component {
    constructor() {
        super();
        this.state = {
        };
    }
    render() {
        const backgroundDiv = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${this.props.Image}')`,
            },
        };
        const firstName = StringFunctions.safeUrl(this.props.FirstName);
        const lastName = StringFunctions.safeUrl(this.props.LastName);
        const linkUrl = `/meet-the-team/${firstName}-${lastName}`;
        return (
            <div className="tile col-xs-12 col-sm-4">
                <div className="img-container col-xs-12 alt img-zoom-hover">
                    <a className="a-container" href={linkUrl}></a>
                    {this.props.Image
                        && <div {...backgroundDiv}></div>}
                </div>
                <div className="col-xs-12 caption-container">
                    <h3>
                        <a className="h-tertiary h-link cursor-pointer" href={linkUrl}>
                            {this.props.FirstName} {this.props.LastName}
                        </a>
                    </h3>
                    <span className="sub-caption">{this.props.JobTitle}</span>
                </div>
            </div>
        );
    }
}

TeamIndexItem.propTypes = {
    FirstName: React.PropTypes.string,
    LastName: React.PropTypes.string,
    Image: React.PropTypes.string,
    JobTitle: React.PropTypes.string,
};
