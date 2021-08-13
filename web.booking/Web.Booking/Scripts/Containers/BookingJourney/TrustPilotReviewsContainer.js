import 'widgets/bookingjourney/_trustpilotreviews.scss';

import React from 'react';
import TrustPilotReviews from '../../widgets/bookingjourney/trustpilotreviews';
import UrlFunctions from '../../library/urlfunctions';

class TrustPilotReviewsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getTrustPilotReviewsProps() {
        const props = {
            propertyId: UrlFunctions.getQueryStringValue('propertyId'),
            OSReference: UrlFunctions.getQueryStringValue('OSReference'),
        };
        return props;
    }
    render() {
        return (
            <div className="trust-pilot-reviews-content container">
                <h3 className="h-tertiary">Reviews</h3>
                <TrustPilotReviews {...this.getTrustPilotReviewsProps()} />
            </div>
        );
    }
}

TrustPilotReviewsContainer.propTypes = {
};

export default TrustPilotReviewsContainer;
