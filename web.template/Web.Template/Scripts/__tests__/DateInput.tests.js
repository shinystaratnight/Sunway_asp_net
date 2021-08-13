import React from 'react';
import { shallow, mount, render } from 'enzyme';
import DateInput from '../components/form/dateinput';

jest.unmock('../components/form/dateinput');
jest.unmock('react-date-picker');

const myMockFn = jest.fn();

const props = {
    name: 'Test Name',
    label: 'Test Label',
    onChange: myMockFn,
}

describe('Date Input Component', () => {
    it('should render container with form-group class', () => {
        const wrapper = shallow(<DateInput {...props} />);
        expect(wrapper.find('.form-group').length).toBe(1);
    });
    it('should render label with text matching props', () => {
        const wrapper = shallow(<DateInput {...props} />);
        expect(wrapper.find('label').text()).toBe(props.label);
    });
    it('should render date field input', () => {
        const wrapper = mount(<DateInput {...props} />);
        expect(wrapper.find('input.react-date-field__input').length).toBe(1);
    });
    it('should render monday first', () => {
        const wrapper = mount(<DateInput {...props} />);
        wrapper.find('input.react-date-field__input').first().simulate('focus');
        expect(wrapper.find('.react-date-picker__month-view-week-day-name').first().text().toLowerCase()).toBe('m');
    });
    it('should show month view when focus on date field', () => {
        const wrapper = mount(<DateInput {...props} />);
        expect(wrapper.find('.react-date-picker__month-view').length).toBe(0);
        wrapper.find('input.react-date-field__input').first().simulate('focus');
        expect(wrapper.find('.react-date-picker__month-view').length).toBe(1);
    });
    it('should call onchange function when date clicked', () => {
        const wrapper = mount(<DateInput {...props} />);
        wrapper.find('input.react-date-field__input').first().simulate('focus');
        wrapper.find('.react-date-picker__month-view-cell').at(10).simulate('click');
    });
});
