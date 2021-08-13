
import '../../../styles/widgets/content/_tripadvisor.scss';

import Rating from 'components/common/rating';
import React from 'react';

export default class TripAdvisor extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    renderRating() {
        const starRatingConfiguration = this.props.siteConfiguration.StarRatingConfiguration;
        const ratingProps = {
            rating: this.props.rating,
            appendText: starRatingConfiguration.AppendText,
            displayHalfRatings: starRatingConfiguration.DisplayHalfRatings,
            containerClass: 'col-xs-12 center-block',
        };
        return (
            <Rating {...ratingProps} />
        );
    }
    renderTripAdvisor() {
        const tripAdvisorProps = {
            __html: this.props.tripAdvisorScript,
        };
        return (
            <div className="col-xs-12 center-block">
                <div dangerouslySetInnerHTML={tripAdvisorProps}></div>
            </div>
        );
    }
    render() {
        const rating = this.props.rating;
        const floatRating = parseFloat(rating);
        const minimumRating = 2.9;
        const renderRating = floatRating > minimumRating;
        const renderTripAdvisor = this.props.tripAdvisorScript;
        return (
            <div className="container trip-advisor-content">
                <div className="row">
                    {renderRating
                        && this.renderRating()}
                    {renderTripAdvisor
                        && this.renderTripAdvisor()}
                </div>
            </div>
        );
    }
}

TripAdvisor.propTypes = {
    tripAdvisorScript: React.PropTypes.string,
    rating: React.PropTypes.string,
    siteConfiguration: React.PropTypes.object,
};
