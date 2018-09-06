import { Select } from 'antd';
import { SelectValue } from 'antd/lib/select';
import React from 'react';

import { IMappingColumn } from './MappingTable';

// TODO: enums?
const langs = ['russian', 'english', 'other'];
const types = ['string', 'number'];

interface IHierarchyLevel {
    type: string;
    level: number;
    key: string;
    name: string;
}

interface IColumnProps {
    index: number;
    callbackFromParent: (nextColumn: IMappingColumn, index?: number) => void;
    initial?: IMappingColumn;
}

interface IColumnState {
    mapping: IMappingColumn;
}

export abstract class MappingColumn extends React.Component<IColumnProps, IColumnState> {
    constructor(props: IColumnProps) {
        super(props);
        this.state = {
            mapping: props.initial ? props.initial : this.initialMapping(props)
        };

        this.changeColumnNumber = this.changeColumnNumber.bind(this);
        this.changeType = this.changeType.bind(this);
        this.changeTitle = this.changeTitle.bind(this);
        this.changeTitleRus = this.changeTitleRus.bind(this);
        this.changeDescr = this.changeDescr.bind(this);
        this.changeDescrRus = this.changeDescrRus.bind(this);
        this.changeDescrNative = this.changeDescrNative.bind(this);
        this.changeVoteResult = this.changeVoteResult.bind(this);
        this.changeCalcResult = this.changeCalcResult.bind(this);
        this.updateHierarchyLang = this.updateHierarchyLang.bind(this);
        this.updateHierarchyLevel = this.updateHierarchyLevel.bind(this);
        this.submitColumn = this.submitColumn.bind(this);
    }

    public render(): React.ReactNode {
        return (
            <form onSubmit={this.submitColumn} >
                <div className='row'>
                    <div className='form-group'>
                        <label htmlFor='index' className='control-label col-sm-3'>Column number</label>
                        <div className='col-sm-9'>
                            <input id='index' type='number' value={this.state.mapping.columnNumber}
                                onChange={this.changeColumnNumber} className='form-control' />
                        </div>
                    </div>
                </div>

                <div className='row'>
                    {this.typeSelect()}
                </div>

                <div className='row'>
                    <div className='col-sm-5'>
                        <div className='form-group'>
                            <label htmlFor='titleRus' className='control-label'>TitleRus</label>
                            <input id='titleRus' value={this.state.mapping.titleRus} onChange={this.changeTitleRus}
                                className='form-control' />
                        </div>
                    </div>
                    <div className='col-sm-7'>
                        <div className='form-group'>
                            <label htmlFor='descrRus' className='control-label'>DescriptionRus</label>
                            <textarea id='descrRus' value={this.state.mapping.descriptionRus}
                                onChange={this.changeDescrRus} className='form-control' />
                        </div>
                    </div>
                </div>

                <div className='row'>
                    <div className='col-sm-5'>
                        <div className='form-group'>
                            <label htmlFor='titleEng' className='control-label'>TitleEng</label>
                            <input id='titleEng' value={this.state.mapping.titleEng} onChange={this.changeTitle}
                                className='form-control' />
                        </div>
                    </div>
                    <div className='col-sm-7'>
                        <div className='form-group'>
                            <label htmlFor='descrEng' className='control-label'>DescriptionEng</label>
                            <textarea id='descrEng' value={this.state.mapping.descriptionEng}
                                onChange={this.changeDescr} className='form-control' />
                        </div>
                    </div>
                </div>

                <div className='row'>
                    <div className='col-sm-7 col-sm-offset-5'>
                        <div className='form-group'>
                            <label htmlFor='descr-native' className='control-label'>Description native</label>
                            <textarea id='descr-native' value={this.state.mapping.descriptionNative}
                                onChange={this.changeDescrNative} className='form-control' />
                        </div>
                    </div>
                </div>

                <div className='row'>
                    <div className='col-sm-6'>
                        <div className='checkbox'>
                            <label>
                                <input id='vote-result' type='checkbox' checked={this.state.mapping.isVoteResult}
                                    onChange={this.changeVoteResult} />
                                Is vote result?
                            </label>
                        </div>
                    </div>
                    <div className='col-sm-6'>
                        <div className='checkbox'>
                            <label>
                                <input id='calc-result' type='checkbox' checked={this.state.mapping.isCalcResult}
                                    onChange={this.changeCalcResult} />
                                Is calc result?
                            </label>
                        </div>
                    </div>
                </div>

                <div className='row'>
                    <div className='form-group'>
                        <div className='col-sm-6'>
                            {this.hierarchyLevelSelect()}
                        </div>
                        <div className='col-sm-6'>
                            {this.hierarchyLangSelect()}
                        </div>
                    </div>
                </div>

                <div className='row'>
                    <div className='col-sm-6'>
                        <input type='submit' className='btn btn-primary' />
                    </div>
                </div>
            </form>
        );
    }

    public componentWillReceiveProps(nextProps: IColumnProps) {
        this.setState({
            ...this.state, mapping: nextProps.initial ? nextProps.initial : this.initialMapping(nextProps)
        });
    }

    private initialMapping(props: IColumnProps): IMappingColumn {
        return {
            columnNumber: props.index, descriptionEng: '', descriptionNative: '', descriptionRus: '',
            isHierarchy: false, hierarchyLang: langs[0], hierarchyLevel: 0, hierarchyType: '',
            isCalcResult: false, isNumber: false, isVoteResult: false, titleEng: '', titleRus: '', type: 0
        };
    }

    private typeSelect(): React.ReactNode {
        const options = types.map((type, i) => <Select.Option key={i} value={i}>{type}</Select.Option>);

        return (
            <div className='form-group'>
                <label htmlFor='type' className='control-label col-sm-3'>Column type</label>
                <div className='col-sm-9'>
                    <Select value={types[Number(this.state.mapping.isNumber)]} onChange={this.changeType}>
                        {options}
                    </Select>
                </div>
            </div>
        );
    }

    private hierarchyLevelSelect(): React.ReactNode {
        const levels: IHierarchyLevel[] = [
            { level: 1, type: 'number', key: 'number_1', name: 'Committee number (lowest) level 1' },
            { level: 1, type: 'name', key: 'name_1', name: 'Committee name (lowest) level 1' }
        ];
        for (let i = 2; i <= 4; ++i) {
            levels.push({ level: i, type: 'number', key: `number_${i}`, name: `Committee number level ${i}` });
            levels.push({ level: i, type: 'name', key: `name_${i}`, name: `Committee name level ${i}` });
        }

        const options = levels.map((level, i) => <Select.Option key={i} value={level.key}>{level.name}</Select.Option>);
        let selectedVal: string;
        if (this.state.mapping.isHierarchy) {
            const prefix = this.state.mapping.hierarchyType == 'number' ? 'number' : 'name';
            selectedVal = `${prefix}_${this.state.mapping.hierarchyLevel}`;
        } else {
            selectedVal = '';
        }

        return (
            <Select value={selectedVal} onChange={this.updateHierarchyLevel}>
                <Select.Option value=''>No level</Select.Option>{options}
            </Select>
        );
    }

    private hierarchyLangSelect(): React.ReactNode {
        if (!this.state.mapping.isHierarchy || (this.state.mapping.hierarchyType == 'number')) {
            return;
        }

        const options = langs.map((lang, i) => <Select.Option key={i} value={lang}>{lang}</Select.Option>);

        return <Select onChange={this.updateHierarchyLang} defaultValue={langs[0]}>{options}</Select>;
    }

    private changeColumnNumber(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({
            ...this.state, mapping: {
                ...this.state.mapping,
                columnNumber: parseInt(e.currentTarget.value, 10)
            }
        });
    }

    // TODO: DRY
    private changeType(val: SelectValue): void {
        this.setState({
            ...this.state,
            mapping: { ...this.state.mapping, type: parseInt(val.toString(), 10), isNumber: val.toString() == '1' }
        });
    }

    private changeTitle(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, mapping: { ...this.state.mapping, titleEng: e.currentTarget.value } });
    }

    private changeTitleRus(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, mapping: { ...this.state.mapping, titleRus: e.currentTarget.value } });
    }

    private changeDescr(e: React.ChangeEvent<HTMLTextAreaElement>): void {
        this.setState({ ...this.state, mapping: { ...this.state.mapping, descriptionEng: e.currentTarget.value } });
    }

    private changeDescrRus(e: React.ChangeEvent<HTMLTextAreaElement>): void {
        this.setState({ ...this.state, mapping: { ...this.state.mapping, descriptionRus: e.currentTarget.value } });
    }

    private changeDescrNative(e: React.ChangeEvent<HTMLTextAreaElement>): void {
        this.setState({ ...this.state, mapping: { ...this.state.mapping, descriptionNative: e.currentTarget.value } });
    }

    private changeVoteResult(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, mapping: { ...this.state.mapping, isVoteResult: e.currentTarget.checked } });
    }

    private changeCalcResult(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, mapping: { ...this.state.mapping, isCalcResult: e.currentTarget.checked } });
    }

    private updateHierarchyLevel(val: SelectValue): void {
        const level = val.toString().match(/\d+/);
        this.setState({
            ...this.state,
            mapping: {
                ...this.state.mapping, isHierarchy: !!val, hierarchyLevel: level ? parseInt(level[0], 10) : 0,
                hierarchyType: val.toString().search('name_') == -1 ? 'number' : 'name'
            }
        });
    }

    private updateHierarchyLang(val: SelectValue): void {
        this.setState({ ...this.state, mapping: { ...this.state.mapping, hierarchyLang: val.toString() } });
    }

    private submitColumn(e: React.ChangeEvent<HTMLFormElement>): void {
        e.preventDefault();

        this.props.callbackFromParent(this.state.mapping);
    }
}
