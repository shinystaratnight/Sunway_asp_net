import React from 'react';
import { shallow, mount, render } from 'enzyme';
import SelectInput from '../components/form/selectinput';

jest.unmock('../components/form/selectinput');

const props = {
    key: 'Test Key',
    name: 'Test Name',
    label: 'Test Label',
    error: '',
    options: ['Option1', 'Option2', 'Option3']
}

describe('SelectInput Component', () => {
    it('should render container with form-group class', () => {
        const wrapper = shallow(<SelectInput {...props} />);
        expect(wrapper.find('.form-group').length).toBe(1);
    });
    it('should render label with text matching props', () => {
        const wrapper = shallow(<SelectInput {...props} />);
        expect(wrapper.find('label').text()).toBe(props.label);
    });
    it('should render select with form-control class', () => {
        const wrapper = shallow(<SelectInput {...props} />);
        expect(wrapper.find('select.form-control').length).toBe(1);
    });
    it('should render select with options matching props - string', () => {
        const wrapper = shallow(<SelectInput {...props} />);
        expect(wrapper.find('option').length).toBe(props.options.length);
    });
    it('should render select with options matching props - object', () => {
        const objectOptionProps = Object.assign({}, props);
        objectOptionProps.options = [
            {
                Id: 1,
                Name: 'Test 1'
            },
            {
                Id: 2,
                Name: 'Test 2'
            }
        ];
        const wrapper = shallow(<SelectInput {...objectOptionProps} />);
        expect(wrapper.find('option').length).toBe(objectOptionProps.options.length);
    });
    it('should render select with options matching props - range', () => {
        const optionRangeProps = Object.assign({}, props);
        optionRangeProps.options = null;
        optionRangeProps.optionsRangeMin = 0;
        optionRangeProps.optionsRangeMax = 10;
        const wrapper = shallow(<SelectInput {...optionRangeProps} />);
        expect(wrapper.find('option').length).toBe(11);
        expect(wrapper.find('option').first().text()).toBe(optionRangeProps.optionsRangeMin.toString());
        expect(wrapper.find('option').last().text()).toBe(optionRangeProps.optionsRangeMax.toString());
    });

    it('should render description when description set in props', () => {
        props.description = 'Test Description';
        const wrapper = shallow(<SelectInput {...props} />);
        expect(wrapper.find('.help-block').text()).toBe(props.description);
    });
    it('should render * in label when required set to true', () => {
        props.required = true;
        const wrapper = shallow(<SelectInput {...props} />);
        expect(wrapper.find('label').text()).toContain('*');
    });
    describe('When placeholder prop is set', () => {
        const placeholderProps = Object.assign({}, props);
        placeholderProps.placeholder = 'Test Placeholder';
        it('should render placeholder option with other options', () => {
            const wrapper = shallow(<SelectInput {...placeholderProps} />);
            expect(wrapper.find('option').length).toBe(placeholderProps.options.length + 1);
        });
        it('should render placeholder option with text matching props', () => {
            const wrapper = shallow(<SelectInput {...placeholderProps} />);
            expect(wrapper.find('.placeholder').text()).toBe(placeholderProps.placeholder);
        });
    });
    describe('When error is set', () => {
        const errorProps = Object.assign({}, props);
        errorProps.error = 'test error';
        it('should render container with form-group and has-error class', () => {
            const wrapper = shallow(<SelectInput {...errorProps} />);
            expect(wrapper.find('.form-group.has-error').length).toBe(1);
        });
        it('should render warning message', () => {
            const wrapper = shallow(<SelectInput {...errorProps} />);
            expect(wrapper.find('.text-warning').length).toBe(1);
        });
    });
});
