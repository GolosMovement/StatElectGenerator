import * as React from 'react';

import { TreeSelect, Spin } from 'antd';
import { IStringDictionary } from '.';
const TreeNode = TreeSelect.TreeNode;

export interface LazyTreeSelectProps<TItem, TValue extends React.Key> 
{
    selectedValue: TValue | null;
    itemsPromise: Promise<TItem[]>;
    getValue: (value: TItem) => TValue;
    getText: (value: TItem) => string;
    getChildren: (value: TItem) => TItem[];
    onChange: (newSelectedValue: TValue | null) => void;
    placeholder?: string;
    allowClear?: boolean;
}

interface LazyTreeSelectState<TItem>
{
    items: TItem[];
    itemsFilterValues: IStringDictionary<string[]>;
    itemsPromise: Promise<TItem[]> | null;
}

export class LazyTreeSelect<TItem, TValue extends React.Key> extends React.Component<LazyTreeSelectProps<TItem, TValue>, LazyTreeSelectState<TItem>>
{
    constructor(props: LazyTreeSelectProps<TItem, TValue>) {
        super(props);
        this.state = {
            items: [],
            itemsFilterValues: {},
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
            const itemsFilterValues: IStringDictionary<string[]> = {};

            for (let item of items) {
                this.addFilterValues(itemsFilterValues, item, []);
            }

            this.setState({
                    items: items,
                    itemsFilterValues: itemsFilterValues,
                    itemsPromise: this.props.itemsPromise
                });
        });
    }

    private addFilterValues(itemsFilterValues: IStringDictionary<string[]>, item: TItem, parentValues: string[]) {
        const currentValues = parentValues.concat([this.props.getText(item).toLowerCase()]);
        const value = this.props.getValue(item) as string;
        itemsFilterValues[value] = currentValues;

        const children = this.props.getChildren(item);
        if (children != null) {
            for (let childItem of children) {
                this.addFilterValues(itemsFilterValues, childItem, currentValues);
            }
        }
    }

    public render(): JSX.Element {   
        if (this.isLoading()) {
            return <Spin />;
        }
        else {
            const treeNodes = this.state.items.map(this.buildTreeNode);
            const selectedText = this.props.selectedValue == null
                ? undefined
                : this.props.getText(this.getSelectedItem(this.state.items) as TItem);

            return (
                <TreeSelect
                    showSearch
                    allowClear={this.props.allowClear}
                    style={{ width: 400 }}
                    placeholder={this.props.placeholder}
                    value={selectedText}
                    filterTreeNode={this.filterTreeNode}
                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                    onChange={this.props.onChange}>
                    {treeNodes}
                </TreeSelect>);
        }
    }

    private filterTreeNode = (input: string, treeNode: any) => {
        const itemFilterValues = this.state.itemsFilterValues[treeNode.props.value];
        const inputInLowerCase = input.toLowerCase();
        return itemFilterValues.some(text => text.indexOf(inputInLowerCase) >= 0);
    }

    private buildTreeNode = (item: TItem): JSX.Element => {
        const value = this.props.getValue(item);
        const text = this.props.getText(item);
        const children = this.props.getChildren(item);

        return (
            <TreeNode value={value} title={text} key={value}>
                {children == null ? null : children.map(this.buildTreeNode)}
            </TreeNode>);
    }

    private getSelectedItem = (items: TItem[]): TItem | null => {
        const selectedItems = items.filter(item => this.props.getValue(item) == this.props.selectedValue);
        if (selectedItems.length === 1) {
            return selectedItems[0];
        }
        else {
            const selectedChildItems = items
                .map(this.props.getChildren)
                .filter(children => children != null)
                .map(this.getSelectedItem)
                .filter(selectedChild => selectedChild != null);

            if (selectedChildItems.length == 1) {
                return selectedChildItems[0];
            }
            else {                
                return null;
            }
        }
    }

    private isLoading() {
        return this.state.itemsPromise !== this.props.itemsPromise;
    }
}