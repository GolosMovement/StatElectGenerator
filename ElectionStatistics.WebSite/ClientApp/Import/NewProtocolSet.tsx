import React from 'react';
import { Select, Spin } from 'antd';
import { SelectValue } from 'antd/lib/select';

import { ImportController } from './ImportController';
import { MappingColumn } from './MappingColumn';
import { IMappingColumn, IMappingTableState, MappingTable } from './MappingTable';

export interface IProtocolSet {
    id?: number;
    titleEng: string;
    descriptionEng: string;
    titleRus: string;
    descriptionRus: string;
    hidden: boolean;
}

export interface IMappingList {
    entry: { id: number; name: string, dataLineNumber: number };
    lines: IMappingColumn[];
}

interface IDataset {
    startLine: number;
    position: string;
    index: number;
    columnForm: string;
    columnNumber: number;
    file: File;
    mappingTable: IMappingTableState;
    protocolSet: IProtocolSet;
    initialColumn?: IMappingColumn;
    initialIndexColumn?: number;
}

interface IDatasetState extends IDataset {
    mappings?: IMappingList[];
    mappingToLoad?: number;
    isLoading: boolean;
}

export class NewProtocolSet extends React.Component<{}, IDatasetState> {
    private static DEFAULT_NEW_COLUMN_INDEX = 1;
    private static DEFAULT_START_LINE = 2;

    constructor(props: {}) {
        super(props);

        this.state = {
            columnForm: 'new', columnNumber: NewProtocolSet.DEFAULT_NEW_COLUMN_INDEX, file: new File([], ''),
            index: NewProtocolSet.DEFAULT_NEW_COLUMN_INDEX, mappingTable: { dataset: [] }, position: 'end',
            protocolSet: {
                titleEng: '', descriptionEng: '', titleRus: '', descriptionRus: '', hidden: false
            }, startLine: NewProtocolSet.DEFAULT_START_LINE, isLoading: false
        };

        this.openColumnForm = this.openColumnForm.bind(this);
        this.pickFile = this.pickFile.bind(this);

        this.changeStartLine = this.changeStartLine.bind(this);
        this.changeTitleRu = this.changeTitleRu.bind(this);
        this.changeTitle = this.changeTitle.bind(this);
        this.changeDescrRu = this.changeDescrRu.bind(this);
        this.changeDescr = this.changeDescr.bind(this);
        this.changeHidden = this.changeHidden.bind(this);
        this.changePosition = this.changePosition.bind(this);
        this.onSubmitForm = this.onSubmitForm.bind(this);

        this.changeSelectedMapping = this.changeSelectedMapping.bind(this);
        this.loadMapping = this.loadMapping.bind(this);
        this.saveMapping = this.saveMapping.bind(this);
    }

    public render(): React.ReactNode {
        // TODO: CSRF protection
        return (
            <div>
                <h1>{'Import new ProtocolSet'}</h1>
                <form onSubmit={this.onSubmitForm}>
                    <div className='row'>
                        <div className='col-sm-3'>
                            <div className='form-group'>
                                <label htmlFor='titleRus'>TitleRus</label>
                                <input id='titleRus' value={this.state.protocolSet.titleRus}
                                    onChange={this.changeTitleRu} className='form-control' />
                            </div>
                        </div>
                        <div className='col-sm-4'>
                            <div className='form-group'>
                                <label htmlFor='descriptionRus'>DescriptionRus</label>
                                <textarea id='descriptionRus' value={this.state.protocolSet.descriptionRus}
                                    onChange={this.changeDescrRu} className='form-control' ></textarea>
                            </div>
                        </div>

                        <div className='col-sm-5'>
                            <div className='checkbox'>
                                <label className='control-label'>
                                    <input type='checkbox' id='hidden' checked={this.state.protocolSet.hidden}
                                        onChange={this.changeHidden} />
                                    Hidden
                                </label>
                            </div>
                        </div>
                    </div>

                    <div className='row'>
                        <div className='col-sm-3'>
                            <div className='form-group'>
                                <label htmlFor='titleEng'>TitleEng</label>
                                <input id='titleEng' value={this.state.protocolSet.titleEng}
                                    onChange={this.changeTitle} className='form-control' />
                            </div>
                        </div>
                        <div className='col-sm-4'>
                            <div className='form-group'>
                                <label htmlFor='descriptionEng'>DescriptionEng</label>
                                <textarea id='descriptionEng' value={this.state.protocolSet.descriptionEng}
                                    onChange={this.changeDescr} className='form-control' />
                            </div>
                        </div>
                    </div>

                    <div className='row'>
                        <div className='form-group'>
                            <label htmlFor='start-line' className='col-sm-3'>Start line:</label>
                            <div className='col-sm-3'>
                                <input id='start-line' type='number' value={this.state.startLine}
                                    onChange={this.changeStartLine} className='form-control' />
                            </div>
                            <div className='col-sm-1'>{this.errorLogFileDownload()}</div>
                        </div>
                    </div>

                    <MappingTable dataset={this.state.mappingTable.dataset}
                        updateParentCallback={this.updateMappingTableCallback}
                        editColumnParentCallback={this.editButtonColumnCallback} />

                    <div className='row'>
                        <div className='col-sm-3'>
                            {this.newColumnLink()}
                        </div>

                        <div className='col-sm-3'>
                            <div className='radio'>
                                <label className='radio-inline'>
                                    <input type='radio' name='add-column-position' id='end' defaultChecked={true}
                                        value='end' onChange={this.changePosition} />
                                    End
                                </label>
                                <label className='radio-inline'>
                                    <input type='radio' name='add-column-position' id='begin'
                                        value='begin' onChange={this.changePosition}  />
                                    Begin
                                </label>
                                <label className='radio-inline'>
                                    <input type='radio' name='add-column-position' id='index'
                                            value='index' onChange={this.changePosition} />
                                    Index:
                                </label>
                            </div>
                        </div>

                        <div className='col-sm-3'>
                            <input type='number' id='add-column-index' className='form-control'
                                defaultValue={NewProtocolSet.DEFAULT_NEW_COLUMN_INDEX.toString()}
                                onChange={this.changePosition} />
                        </div>
                    </div>

                    <div className='row'>
                        <div className='col-sm-6'>
                            <div className='col-sm-8'>
                                {this.mappingList()}
                            </div>
                            <div className='col-sm-4'>
                                <button type='button' id='load-mapping' onClick={this.loadMapping}>
                                    Load mapping
                                </button>
                            </div>
                        </div>

                        <div className='col-sm-3'>
                            <button type='button' id='save-mapping' onClick={this.saveMapping} >
                                Save mapping
                            </button>
                        </div>
                    </div>

                    <div className='row'>
                        <div className='col-sm-3'>
                            <label htmlFor='file'>Choose file: </label>
                        </div>
                        <div className='col-sm-3'>
                            <input type='file' name='file' id='file' onChange={this.pickFile} accept='.xlsx' />
                        </div>
                    </div>

                    <div className='row'>
                        {this.submitButton()}
                    </div>
                </form>

                {this.mappingColumnForm()}
            </div>
        );
    }

    public componentWillMount(): void {
        this.fetchMappings();
    }

    public updateMappingTableCallback = (nextTable: IMappingTableState) => {
        this.setState({ ...this.state, mappingTable: nextTable });
    }

    public newColumnCallback = (nextColumn: IMappingColumn) => {
        let newDataset: IMappingColumn[] = [];
        switch (this.state.position) {
            case 'end':
                newDataset = [...this.state.mappingTable.dataset, nextColumn];
                break;
            case 'begin':
                newDataset = [nextColumn, ...this.state.mappingTable.dataset];
                break;
            case 'index':
                if (this.state.index != null) {
                    newDataset = [...this.state.mappingTable.dataset.slice(0, this.state.index - 1),
                        nextColumn, ...this.state.mappingTable.dataset.slice(this.state.index - 1)];
                    break;
                }
        }

        this.toggleColumnModal();
        this.setState({
            ...this.state, columnNumber: newDataset[newDataset.length - 1].columnNumber + 1,
            mappingTable: { dataset: newDataset }
        });
    }

    public editButtonColumnCallback = (editColumn: IMappingColumn, index?: number) => {
        if (index != undefined) {
            this.setState({ ...this.state, columnForm: 'edit', initialColumn: editColumn, initialIndexColumn: index });
            this.toggleColumnModal();
        }
    }

    public editColumnCallback = (updatedColumn: IMappingColumn) => {
        if (this.state.initialIndexColumn != undefined) {
            const updatedDataset = [...this.state.mappingTable.dataset.slice(0, this.state.initialIndexColumn),
                updatedColumn, ...this.state.mappingTable.dataset.slice(this.state.initialIndexColumn + 1)];
            this.setState({ ...this.state, mappingTable: { dataset: updatedDataset } });
            this.toggleColumnModal();
        }
    }

    private submitForm(): void {
        ImportController.Instance.createDataset(this.state.mappingTable.dataset, this.state.file,
            this.state.protocolSet, this.state.startLine)
            .then((result) => {
                this.setState({ ...this.state, isLoading: false });

                if (result.status == 'ok') {
                    this.setState({ ...this.state, protocolSet: { ...this.state.protocolSet, id: result.data } });
                    alert('Success!');
                } else {
                    alert(`Fail! ${result.message}`);
                }
            }).catch((err) => {
                this.setState({ ...this.state, isLoading: false });
                alert('Server error!');
            });
    }

    private onSubmitForm(e: React.FormEvent<HTMLFormElement>): void {
        e.preventDefault();

        this.setState({ ...this.state, isLoading: true });

        this.submitForm();
    }

    private errorLogFileDownload(): React.ReactNode {
        if (this.state.protocolSet.id) {
            const href = `/api/import/protocolSets/${this.state.protocolSet.id}/log`;
            return <a href={href} className='btn btn-default'>Download error logfile</a>;
        }
    }

    private toggleColumnModal(): void {
        // TODO: rewrite to more _react_ way
        const modal = document.getElementById('column-form');
        if (modal) { modal.style.display = modal.style.display == 'block' ? 'none' : 'block'; }
    }

    private newColumnLink(): React.ReactNode {
        return (
            <button type='button' id='open-column-form' className='btn btn-default'
                onClick={this.openColumnForm}>Add column</button>
        );
    }

    private submitButton(): React.ReactNode {
        if (this.state.isLoading) {
            return <Spin/>;
        } else {
            return <input type='submit' className='btn btn-primary' />;
        }
    }

    private mappingColumnForm(): React.ReactNode {
        let component: React.ReactNode;
        if (this.state.columnForm == 'new') {
            component = <MappingColumn index={this.state.columnNumber}
                callbackFromParent={this.newColumnCallback} />;
        } else {
            component = <MappingColumn index={this.state.columnNumber}
                callbackFromParent={this.editColumnCallback} initial={this.state.initialColumn} />;
        }

        return (
            <div id='column-form' className='modal'>
                <div className='modal-content'>
                    <div className='modal-header text-center'>
                        <h4 className='inline-block'>Add column</h4>
                        <span className='modal-close' onClick={this.toggleColumnModal}>&times;</span>
                    </div>
                    {component}
                </div>
            </div>
        );
    }

    private mappingList(): React.ReactNode {
        if (this.state.mappings != undefined) {
            const options = this.state.mappings.map((mapping, i) =>
                <Select.Option key={i} value={mapping.entry.id}>{mapping.entry.name}</Select.Option>);
            return (
                <Select onSelect={this.changeSelectedMapping}>{options}</Select>
            );
        } else {
            return <Select></Select>;
        }
    }

    private openColumnForm(): void {
        this.setState({ ...this.state, columnForm: 'new' });
        this.toggleColumnModal();
    }

    private changeStartLine(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, startLine: parseInt(e.currentTarget.value, 10) });
    }

    private changePosition(): void {
        const position = document.querySelector('input[name="add-column-position"]:checked') as HTMLInputElement;
        const index = document.getElementById('add-column-index') as HTMLInputElement;
        const indexVal = parseInt(index.value, 10);

        let columnNumber = 2;
        switch (position.value) {
            case 'end':
                const columnCount = this.state.mappingTable.dataset.length;
                if (columnCount > 0) {
                    columnNumber = this.state.mappingTable.dataset[columnCount - 1].columnNumber + 1;
                }
                break;
            case 'index':
                columnNumber = indexVal;
                break;
        }

        this.setState({
            ...this.state, columnNumber, index: indexVal, position: position.value
        });
    }

    private pickFile(e: React.ChangeEvent<HTMLInputElement>): void {
        if (e.currentTarget.files) {
            this.setState({ ...this.state, file: e.currentTarget.files[0] });
        }
    }

    // TODO: DRY
    private changeTitleRu(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, protocolSet: { ...this.state.protocolSet, titleRus: e.currentTarget.value } });
    }

    private changeDescrRu(e: React.ChangeEvent<HTMLTextAreaElement>): void {
        this.setState({
            ...this.state, protocolSet: {
                ...this.state.protocolSet,
                descriptionRus: e.currentTarget.value
            }
        });
    }

    private changeTitle(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, protocolSet: { ...this.state.protocolSet, titleEng: e.currentTarget.value } });
    }

    private changeDescr(e: React.ChangeEvent<HTMLTextAreaElement>): void {
        this.setState({
            ...this.state,
            protocolSet: { ...this.state.protocolSet, descriptionEng: e.currentTarget.value }
        });
    }

    private changeHidden(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, protocolSet: { ...this.state.protocolSet, hidden: e.currentTarget.checked } });
    }

    private changeSelectedMapping(val: SelectValue): void {
        this.setState({
            ...this.state,
            mappingToLoad: parseInt(val.toString(), 10)
        });
    }

    private loadMapping(e: React.FormEvent<HTMLButtonElement>): void {
        e.preventDefault();

        if (this.state.mappings != undefined && this.state.mappingToLoad != undefined) {
            if (!confirm('Are you sure?')) { return; }

            this.state.mappings.forEach((mapping) => {
                if (mapping.entry.id == this.state.mappingToLoad) {
                    this.setState({
                        ...this.state, mappingTable: { dataset: mapping.lines },
                        startLine: mapping.entry.dataLineNumber
                    });
                    return;
                }
            });
        } else {
            alert('Select mapping first');
        }
    }

    private saveMapping(e: React.FormEvent<HTMLButtonElement>): void {
        e.preventDefault();

        const name = prompt('Enter new mapping name');
        if (name != null && name != '') {
            ImportController.Instance.createMapping(name, this.state.startLine, this.state.mappingTable.dataset)
                .then((result) => {
                    if (result.status == 'ok') {
                        this.fetchMappings();
                        alert('Mapping saved');
                    } else {
                        alert('Failed mapping save');
                    }
                })
                .catch(() => alert('Server error'));
        }
    }

    private fetchMappings(): void {
        ImportController.Instance.mappings()
            .then((result) => this.setState({ ...this.state, mappings: result }));
    }
}
