import { Select } from 'antd';
import { SelectValue } from 'antd/lib/select';
import React from 'react';

// TODO: enums?
export const hierarchyLangs = ['русский', 'английский', 'нативный'];
const types = ['строка', 'число'];

export interface IMappingColumn {
    columnNumber: number;
    type: number;
    titleRus: string;
    descriptionRus: string;
    titleEng: string;
    descriptionEng: string;
    descriptionNative: string;
    isNumber: boolean;
    isVoteResult: boolean;
    isCalcResult: boolean;
    isHierarchy: boolean;
    hierarchyLanguage: number;
    hierarchyLevel: number;
    [index: string]: any;
}

interface IColumnProps {
    index: number;
    callbackFromParent: (nextColumn: IMappingColumn, index?: number) => void;
    initial?: IMappingColumn;
}

interface IColumnState {
    mapping: IMappingColumn;
}

export class MappingColumn extends React.Component<IColumnProps, IColumnState> {
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
        this.updateHierarchyType = this.updateHierarchyType.bind(this);
        this.updateHierarchyLevel = this.updateHierarchyLevel.bind(this);
        this.submitColumn = this.submitColumn.bind(this);
    }

    public render(): React.ReactNode {
        return (
            <form onSubmit={this.submitColumn} >
                <div className='row'>
                    <div className='form-group'>
                        <label htmlFor='index' className='control-label col-sm-3'>
                            В какой колонке находятся импортируемые данные
                        </label>
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
                            <label htmlFor='titleRus' className='control-label'>
                                Краткое наименование
                                (для менюшек)
                            </label>
                            <input id='titleRus' value={this.state.mapping.titleRus} onChange={this.changeTitleRus}
                                className='form-control' />
                        </div>
                    </div>
                    <div className='col-sm-7'>
                        <div className='form-group'>
                            <label htmlFor='descrRus' className='control-label'>Подробное описание</label>
                            <textarea id='descrRus' value={this.state.mapping.descriptionRus}
                                onChange={this.changeDescrRus} className='form-control' />
                        </div>
                    </div>
                </div>

                <div className='row'>
                    <div className='col-sm-5'>
                        <div className='form-group'>
                            <label htmlFor='titleEng' className='control-label'>
                                Short Title
                                (for menus)
                            </label>
                            <input id='titleEng' value={this.state.mapping.titleEng} onChange={this.changeTitle}
                                className='form-control' />
                        </div>
                    </div>
                    <div className='col-sm-7'>
                        <div className='form-group'>
                            <label htmlFor='descrEng' className='control-label'>
                                Description
                                (for clarifications)
                            </label>
                            <textarea id='descrEng' value={this.state.mapping.descriptionEng}
                                onChange={this.changeDescr} className='form-control' />
                        </div>
                    </div>
                </div>

                <div className='row'>
                    <div className='col-sm-7 col-sm-offset-5'>
                        <div className='form-group'>
                            <label htmlFor='descr-native' className='control-label'>
                                Официальное описание на родном языке<br/>
                                (на случай проблем с переводом)
                            </label>
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
                                Описывает ли строка результаты волеизъявления?
                            </label>
                        </div>
                    </div>
                    <div className='col-sm-6'>
                        <div className='checkbox'>
                            <label>
                                <input id='calc-result' type='checkbox' checked={this.state.mapping.isCalcResult}
                                    onChange={this.changeCalcResult} />
                                Описывает ли строка результаты используемые в расчётах?
                            </label>
                        </div>
                    </div>
                </div>

                <div className='row'>
                    <div className='col-sm-12'>
                        <label>Является ли свойством иерархии коммиссий, если да, то каким:</label>
                    </div>
                    <div className='form-group'>
                        {this.hierarchySelect()}
                    </div>
                </div>

                <div className='row'>
                    <div className='col-sm-6'>
                        <input type='submit' className='btn btn-primary' value='Сохранить'/>
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
            isHierarchy: false, hierarchyLanguage: 0, hierarchyLevel: 0, isCalcResult: false,
            isNumber: false, isVoteResult: false, titleEng: '', titleRus: '', type: 0
        };
    }

    private typeSelect(): React.ReactNode {
        const options = types.map((type, i) => <Select.Option key={i} value={i}>{type}</Select.Option>);

        return (
            <div className='form-group'>
                <label htmlFor='type' className='control-label col-sm-3'>Тип данных</label>
                <div className='col-sm-9'>
                    <Select value={types[Number(this.state.mapping.isNumber)]} onChange={this.changeType}>
                        {options}
                    </Select>
                </div>
            </div>
        );
    }

    private hierarchySelect(): React.ReactNode {
        const options = [['number', 'номер'], ['name', 'название']].map((level, i) =>
            <Select.Option key={i} value={level[0]}>{level[1]}</Select.Option>);

        let selectedValue = '';
        if (this.state.mapping.isHierarchy) {
            selectedValue = this.state.mapping.isNumber ? 'number' : 'name';
        }
        return (
            <div>
                <div className='col-sm-2'>
                    <Select value={selectedValue} onChange={this.updateHierarchyType}>
                        <Select.Option value=''>не является</Select.Option>{options}
                    </Select>
                </div>

                {this.hierarchyLevelSelect()}
                {this.hierarchyLangSelect()}
            </div>
        );
    }

    private hierarchyLevelSelect(): React.ReactNode {
        if (this.state.mapping.isHierarchy) {
            return (
                <div className='col-sm-2'>
                    <input type='number' id='hierarchy-level' className='form-control'
                        value={this.state.mapping.hierarchyLevel} onChange={this.updateHierarchyLevel} />
                    <small>1-й уровень нижний</small>
                </div>
            );
        }
    }

    private hierarchyLangSelect(): React.ReactNode {
        if (!this.state.mapping.isHierarchy || this.state.mapping.isNumber) {
            return;
        }

        const options = hierarchyLangs.map((lang, i) => <Select.Option key={i} value={i}>{lang}</Select.Option>);

        return (
            <div className='col-sm-4'>
                <div className='col-sm-5'>
                    <label>Название на</label>
                </div>
                <div className='col-sm-7'>
                    <Select onChange={this.updateHierarchyLang}
                        value={hierarchyLangs[this.state.mapping.hierarchyLanguage]}>
                        {options}
                    </Select>
                </div>
            </div>
        );
    }

    private changeColumnNumber(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({
            ...this.state, mapping: {
                ...this.state.mapping,
                columnNumber: parseInt(e.currentTarget.value, 10)
            }
        });
    }

    private changeType(val: SelectValue): void {
        this.setState({
            ...this.state,
            mapping: { ...this.state.mapping, type: parseInt(val.toString(), 10), isNumber: val.toString() == '1' }
        });
    }

    // TODO: DRY
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

    private updateHierarchyType(val: SelectValue): void {
        const isHierarchy = !!val;

        this.setState({
            ...this.state,
            mapping: {
                ...this.state.mapping, isHierarchy,
                hierarchyLevel: isHierarchy ? this.state.mapping.hierarchyLevel : 0
            }
        });
    }

    private updateHierarchyLevel(e: React.ChangeEvent<HTMLInputElement>): void {
        const hierarchyLevel = parseInt(e.currentTarget.value, 10);
        if (!isNaN(hierarchyLevel)) {
            this.setState({
                ...this.state, mapping: { ...this.state.mapping, hierarchyLevel }
            });
        }
    }

    private updateHierarchyLang(val: SelectValue): void {
        this.setState({
            ...this.state, mapping: {
                ...this.state.mapping,
                hierarchyLanguage: parseInt(val.toString(), 10)
            }
        });
    }

    private submitColumn(e: React.ChangeEvent<HTMLFormElement>): void {
        e.preventDefault();

        this.props.callbackFromParent(this.state.mapping);
    }
}
