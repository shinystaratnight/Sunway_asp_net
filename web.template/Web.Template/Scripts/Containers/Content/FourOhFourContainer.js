import '../../../styles/widgets/content/_fourOhFour.scss';
import FourOhFour from '../../widgets/content/fourohfour';
import React from 'react';

class FourOhFourContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = this.props.entity.model;
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
        };
        return (
            <div {...containerProps}>
                <FourOhFour {...fourOhFourProps} />
            </div>
        );
    }
}

FourOhFourContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
};

export default FourOhFourContainer;
