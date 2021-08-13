import React from 'react';

export default class GoogleMap extends React.Component {
    constructor() {
        super();
        this.state = {
        };
    }

    componentDidMount() {
        const script = document.createElement('script');
        script.src = `https://maps.googleapis.com/maps/api/js?key=${this.props.mapConfiguration.Key}&callback=initMap`;
        script.async = false;
        script.defer = false;
        document.body.appendChild(script);

        const mapScript = document.createElement('script');
        mapScript
            .innerHTML = 'var map;\r\n'
        + 'var fullscreen = false;\r\n'
        + 'function CenterControl(controlDiv, map) {\r\n\r\n'

        + 'var controlUI = document.createElement(\'div\');\r\n'
        + 'controlUI.classList.add(\'hotel-map-container\');\r\n'
        + 'controlUI.title = \'Click to open the map in another window\';\r\n'
        + 'controlDiv.appendChild(controlUI);\r\n\r\n'

        + 'var controlText = document.createElement(\'div\');\r\n'
        + 'controlText.classList.add(\'hotel-map\');\r\n'
        + 'controlText.innerHTML = \'View Map\';\r\n'
        + 'controlUI.appendChild(controlText);\r\n\r\n'

        + 'controlUI.addEventListener(\'click\', function() {\r\n'
        + 'fullScreen();'
        + '});\r\n\r\n'
        + '}\r\n'

        + 'function fullScreen() {\r\n'
        + 'var button = document.getElementsByClassName("hotel-map")[0];\r\n'
        + 'if(fullscreen) {\r\n'
        + 'if (document.exitFullscreen) {\r\n'
        + 'document.exitFullscreen();\r\n'
        + '}\r\n'
        + 'if (document.webkitExitFullscreen) {\r\n'
        + 'document.webkitExitFullscreen();\r\n'
        + '}\r\n'
        + 'if (document.mozCancelFullScreen) {\r\n'
        + 'document.mozCancelFullScreen();\r\n'
        + '}\r\n'
        + 'if (document.msExitFullscreen) {\r\n'
        + 'document.msExitFullscreen();\r\n'
        + '}\r\n'
        + 'button.innerHTML = \'View Map\';\r\n'
        + 'fullscreen = false;\r\n'
        + '}\r\n'
        + 'else{\r\n'
        + 'var element = map.getDiv();\r\n'
        + 'if (element.requestFullscreen) {\r\n'
        + 'element.requestFullscreen();\r\n'
        + '}\r\n'
        + 'if (element.webkitRequestFullScreen) {\r\n'
        + 'element.webkitRequestFullScreen();\r\n'
        + '}\r\n'
        + 'if (element.mozRequestFullScreen) {\r\n'
        + 'element.mozRequestFullScreen();\r\n'
        + '}\r\n'
        + 'if (element.msRequestFullscreen) {\r\n'
        + 'element.msRequestFullscreen();\r\n'
        + '}\r\n'
        + 'button.innerHTML = \'Close Map\';\r\n'
        + 'fullscreen = true;\r\n'
        + '}\r\n'
        + '}\r\n'

        + 'function initMap() {\r\n'
        + 'map = new google.maps.Map(document.getElementById(\'map\'), {\r\n'
        + 'center: {\r\n'
        + `lat: ${this.props.coordinates.Latitude},\r\n`
        + `lng: ${this.props.coordinates.Longitude}},\r\n`
        + 'zoom: 17,\r\n'
        + 'disableDefaultUI: true\r\n'
        + '});\r\n'

        + 'var centerControlDiv = document.createElement(\'a\');\r\n'
        + 'var centerControl = new CenterControl(centerControlDiv, map);\r\n\r\n'
        + 'centerControlDiv.index = 1;\r\n'
        + 'map.controls[google.maps.ControlPosition.BOTTOM_CENTER].push(centerControlDiv);\r\n'
        + '}\r\n';
        document.body.appendChild(mapScript);
    }

    render() {
        return (
            <div className="map-container">
                <div ref="map" id="map" className="map"></div>
            </div>
        );
    }
}

GoogleMap.propTypes = {
    coordinates: React.PropTypes.shape({
        Latitude: React.PropTypes.number,
        Longitude: React.PropTypes.number,
    }).isRequired,
    mapStyle: React.PropTypes.object,
    mapConfiguration: React.PropTypes.object.isRequired,
};
