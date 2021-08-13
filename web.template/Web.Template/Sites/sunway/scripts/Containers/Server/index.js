import Containers from 'containers/server';

const customerContainers = {
    FooterContainer: require('./footercontainer').default,
    HeaderContainer: require('./headercontainer').default,
};

const serverContainers = Object.assign({}, Containers, customerContainers);

module.exports = serverContainers;
