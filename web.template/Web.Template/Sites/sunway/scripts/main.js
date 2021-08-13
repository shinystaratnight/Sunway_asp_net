import 'main.scss';
import '../styles/main.scss';

import * as CustomerComponents from './components';
import * as CustomerContainers from './containers';

import Page from 'page';

const page = new Page(CustomerContainers, {}, CustomerComponents);
page.render();
