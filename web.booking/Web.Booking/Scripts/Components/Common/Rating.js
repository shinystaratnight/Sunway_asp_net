import React from 'react';

export default class Rating extends React.Component {
    constructor() {
        super();
        this.state = {
        };
    }
    getAppendText() {
        let text = '';
        this.props.appendText.forEach(appendText => {
            if (parseFloat(appendText.Rating) === parseFloat(this.props.rating)) {
                text = appendText.Text;
            }
        });
        return text;
    }
    render() {
        const rating = this.props.displayHalfRatings
            ? this.props.rating : Math.floor(this.props.rating);
        const props = {
            itemProp: 'award',
            className: 'rating',
        };
        props['data-rating'] = rating;

        props.className += this.props.ratingClass ? ` ${this.props.ratingClass}` : '';

        let containerClass = `rating-container rating-${this.props.size}`;
        containerClass += this.props.displayInline ? ' inline' : '';
        containerClass += this.props.containerClass ? ` ${this.props.containerClass}` : '';

        let ratingText = rating > 0 ? `${rating} Stars` : 'Unrated';
        ratingText = ratingText.replace('.0', '').replace('.5', '½');

        const appendText = this.getAppendText();
        return (
            <div className={containerClass}>
                <span {...props}>{ratingText}</span>
                {appendText
                    && <span className="rating-append-text">{appendText}</span>}
            </div>
        );
    }
}

Rating.propTypes = {
    rating: React.PropTypes.oneOfType([
        React.PropTypes.string,
        React.PropTypes.number,
    ]).isRequired,
    size: React.PropTypes.oneOf(['xs', 'sm', 'md', 'lg']),
    appendText: React.PropTypes.array,
    displayInline: React.PropTypes.bool,
    displayHalfRatings: React.PropTypes.bool,
    containerClass: React.PropTypes.string,
    ratingClass: React.PropTypes.string,
    displayRatingAsStars: React.PropTypes.bool,
};

Rating.defaultProps = {
    size: 'md',
    appendText: [],
    displayHalfRatings: true,
    displayInline: false,
    displayRatingAsStars: true,
};
