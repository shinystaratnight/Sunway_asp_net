import 'main.scss';
import '../styles/main.scss';

import * as CustomerContainers from './containers';
import Page from './exportPage';

const page = new Page(CustomerContainers);
page.render();
