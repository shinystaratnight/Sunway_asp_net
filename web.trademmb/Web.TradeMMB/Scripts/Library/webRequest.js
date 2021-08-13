import fetch from 'isomorphic-fetch';

class WebRequest {

    static send(url) {
        return fetch(url)
            .then(response => {
                try {
                    return response.json();
                } catch (e) {
                    console.log(url, e);
                }
                return '{}';
            });
    }
}

export default WebRequest;
