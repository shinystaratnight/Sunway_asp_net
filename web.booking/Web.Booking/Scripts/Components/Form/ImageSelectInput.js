import React from 'react';
import TextInput from './textinput';
import fetch from 'isomorphic-fetch';

export default class ImageSelectInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            edit: false,
            focusObject: '',
            images: [],
            imageSearch: '',
            searchComplete: false,
        };
        this.closeEditImage = this.closeEditImage.bind(this);
        this.deleteImage = this.deleteImage.bind(this);
        this.editImage = this.editImage.bind(this);
        this.getImages = this.getImages.bind(this);
        this.setImage = this.setImage.bind(this);
        this.setImageSearch = this.setImageSearch.bind(this);
    }
    componentDidUpdate() {
        if (this.state.focusObject !== '') {
            document.getElementById(this.state.focusObject).focus();
            this.state.focusObject = '';
        }
    }
    editImage() {
        this.setState({ edit: true, focusObject: this.props.name });
    }
    closeEditImage() {
        this.setState({ edit: false });
    }
    setImageSearch(event) {
        const value = event.target.value;
        return this.setState({ imageSearch: value });
    }
    deleteImage() {
        this.props.onChange(this.props.name, '');
    }
    setImage(image) {
        this.props.onChange(this.props.name, image.Url);
    }
    getImages() {
        this.setState({ searchComplete: false });
        const searchText = this.state.imageSearch;
        const imagesURL = `/booking/api/sitebuilder/images/${searchText}`;
        fetch(imagesURL).then(response => response.json()).then(result => {
            const images = JSON.parse(result);
            this.setState({ images, searchComplete: true });
        });
    }
    renderImageSearch() {
        const objectKey = this.props.name;
        const textInputProps = {
            key: `${objectKey}_imageselect`,
            name: objectKey,
            label: 'Image Tag',
            labelIconClass: 'fa fa-tag',
            error: this.props.error,
            description: '',
            placeholder: 'Search Image Tag...',
            type: 'text',
            onChange: this.setImageSearch,
            value: this.state.imageSearch,
        };
        return (
            <div className="image-search form-inline">
                <TextInput {...textInputProps} />
                <button
                    type="button"
                    className="btn btn-default btn-xs"
                    onClick={() => this.getImages()}>Search</button>
            </div>
        );
    }
    renderImageOptions() {
        const resultSummary = `${this.state.images.length} images found`;
        return (
            <div className="image-options">
                {this.state.images.length > 0
                    && <p>{resultSummary}</p>}
                {this.state.images.length > 0
                    && this.state.images.map(this.renderImageThumbnail, this)}
                {this.state.images.length === 0
                    && <p className="no-results">No Images found</p>}
            </div>
        );
    }
    renderImageThumbnail(image) {
        return (
            <img
                key={image.Name}
                className="img-thumb"
                src={image.ThumbnailUrl}
                onClick={() => this.setImage(image)}/>
        );
    }
    renderImageSelectorForm() {
        return (
           <div className="image-selector-form">
                <h3>Image Select</h3>
                {this.renderImageSearch()}

                {this.state.searchComplete
                    && this.renderImageOptions()}

                <span className="close fa fa-times" onClick={() =>
                    this.closeEditImage()}></span>
            </div>
        );
    }
    render() {
        return (
            <div key={this.props.name} className="image-selector">
                <div className="form-group">
                    <label>{this.props.label}</label>
                    {this.props.value
                        && <img className="img-selected" src={this.props.value} />}
                    {!this.props.value
                        && <span className="no-image">Not Set</span>}
                    <span className="img-option img-edit" onClick={() =>
                        this.editImage()}>Edit</span>
                    {this.props.value
                        && <span className="img-option img-delete" onClick={() =>
                                this.deleteImage()}>Delete</span>}
                    {this.props.description
                        && <p className="help-block">{this.props.description}</p>}
                </div>
                {this.state.edit
                    && this.renderImageSelectorForm()}
            </div>
        );
    }
}

ImageSelectInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    label: React.PropTypes.string.isRequired,
    labelIconClass: React.PropTypes.string,
    value: React.PropTypes.string,
    onChange: React.PropTypes.func,
    error: React.PropTypes.string,
    placeholder: React.PropTypes.string,
    required: React.PropTypes.bool,
    description: React.PropTypes.string,
};
