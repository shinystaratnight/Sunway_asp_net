import FourOhFour from '../../widgets/content/fourohfour';
import React from 'react';

export default class FourOhFourContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const background = contentModel.background;
        const containerProps = {
            className: 'widget-fourohfour center-block',
            style: {
                backgroundImage: `url(${background}) no-repeat`,
            },
        };
        const fourOhFourProps = {
            title: contentModel.Title,
            subTitle: contentModel.SubTitle,
            text: contentModel.Text,
            button: contentModel.Button,
            background: contentModel.background,
        };
        return (
            <div {...containerProps}>
                <FourOhFour {...fourOhFourProps} />
            </div>
        );
    }
}

FourOhFourContainer.propTypes = {
    contentJSON: React.PropTypes.string,
    page: React.PropTypes.object,
};
