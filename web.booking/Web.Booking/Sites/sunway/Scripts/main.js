import 'main.scss';
import '../styles/main.scss';

import * as CustomerComponents from './components';
import * as CustomerContainers from './containers';
import * as CustomerWidgets from './widgets';

import Page from 'page';

const page = new Page(CustomerContainers, CustomerWidgets, CustomerComponents);
page.render();
