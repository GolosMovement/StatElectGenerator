import * as React from 'react';
import { deepEqual } from '.';

import { Select, Spin } from 'antd';
import { SelectValue, OptionProps } from 'antd/lib/select';

export interface LazySelectProps<TItem, TValue> 
{
    selectedValue: TValue | null;
    itemsPromise: Promise<TItem[]>;
    getValue: (value: TItem) => TValue;
    getText: (value: TItem) => string;
    onChange: (newSelectedValue: TValue | null) => void;
    placeholder?: string;
    allowClear?: boolean;
}

interface LazySelectState<TItem>
{
    items: TItem[];
    itemsPromise: Promise<TItem[]> | null;
}

export class LazySelect<TItem, TValue> extends React.Component<LazySelectProps<TItem, TValue>, LazySelectState<TItem>>
{
    constructor(props: LazySelectProps<TItem, TValue>) {
        super(props);
        this.state = {
            items: [],
            itemsPromise: null
        };
    }

    public componentWillMount() {
        this.loadData();
    }

    public componentWillReceiveProps() {
        if (this.isLoading()) {
            this.loadData();
        }
    }

    private loadData() {
        this.props.itemsPromise.then(items => {
            this.setState({
                    items: items,
                    itemsPromise: this.props.itemsPromise
                });
        });
    }

    public render(): JSX.Element {   
        if (this.isLoading()) {
            return <Spin />;
        }
        else {
            const options = this.state.items
                .map((item, index) => {
                    const text = this.props.getText(item);
                    return <Select.Option key={index} value={text}>{text}</Select.Option>
                });
    
            return (
                <Select
                    showSearch
                    allowClear={this.props.allowClear}
                    style={{ width: 400 }}
                    placeholder={this.props.placeholder}
                    optionFilterProp="children"
                    filterOption={this.filterOption}
                    value={this.getSelectedText()}
                    onChange={this.onChange}>
                    {options}
                </Select>
            );
        }
    }

    private filterOption(input: string, option: React.ReactElement<OptionProps>) {
        return option.props.value.toLowerCase().indexOf(input.toLowerCase()) >= 0;
    }

    private getSelectedText() {
        if (this.props.selectedValue == null) {
            return undefined;
        }
        else {
            const selectedItem = this.state.items.filter(item => deepEqual(this.props.getValue(item), this.props.selectedValue))[0];
            return this.props.getText(selectedItem);
        }
    }

    private isLoading() {
        return this.state.itemsPromise !== this.props.itemsPromise;
    }

    private onChange = (value: SelectValue, option: React.ReactElement<any> | React.ReactElement<any>[]) => {
        if (option == null) {
            this.props.onChange(null);
        }
        else {
            const selectedKey = (option as React.ReactElement<any>).key as number;
            const selectedItem = this.state.items[selectedKey];
            this.props.onChange(this.props.getValue(selectedItem));
        }
    }
}