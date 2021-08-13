import 'widgets/content/_awardspanel.scss';
import React from 'react';

export default class AwardsPanel extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    renderAward(award) {
        return (
            <div className="col-xs-3">
                <div className="award">
                     {award.Url
                        && <a className="a-container" href={award.Url}></a>}
                   <img className="award-image" src={award.Image} alt="" />
                </div>
            </div>
        );
    }
    render() {
        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h3 className="h-tertiary">{this.props.title}</h3>
                </header>
                <div className="panel-body">
                    <div className="row">
                        {this.props.awards.map(this.renderAward, this)}
                     </div>
                </div>
            </div>
        );
    }
}

AwardsPanel.propTypes = {
    title: React.PropTypes.string,
    awards: React.PropTypes.array,
};
