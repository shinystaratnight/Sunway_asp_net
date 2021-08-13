import React from 'react';
import { shallow, mount, render } from 'enzyme';
import Tabs from '../components/tabs';

jest.unmock('../components/tabs');

const myMockFn = jest.fn();

const props = {
    tabs: [
        {
            name: 'Tab 1',
            onClick: myMockFn
        },
        {
            name: 'Tab 2',
            onClick: myMockFn
        }
    ]
};

describe('Tabs Component', () => {
    it('should render ul with tabs class', () => {
        const wrapper = shallow(<Tabs {...props} />);
        expect(wrapper.find('ul.tabs').length).toBe(1);
    });
    it('should render li with tab-item class for each object in tabs props', () => {
        const wrapper = shallow(<Tabs {...props} />);
        expect(wrapper.find('li.tab-item').length).toBe(props.tabs.length);
    });
    it('should render li with text matching text of props ', () => {
        const wrapper = shallow(<Tabs {...props} />);
        expect(wrapper.find('li.tab-item a').first().text()).toBe('Tab 1');
    });
    it('should fire onclick function when clicked', () => {
        const wrapper = mount(<Tabs {...props} />);
        wrapper.find('.tab-item').first().simulate('click');
        expect(myMockFn).toBeCalled();
    });
    it('should set tab-selected class on clicked tab and none other', () => {
        const wrapper = mount(<Tabs {...props} />);
        const tab = wrapper.find('.tab-item').at(1);
        tab.simulate('click');
        expect(tab.hasClass('tab-selected')).toBeTruthy();
        expect(wrapper.find('.tab-selected').length).toBe(1);
    });
});
