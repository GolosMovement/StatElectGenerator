import React from 'react';
import { RouteComponentProps } from 'react-router';
import { Select } from 'antd';
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
}

interface IDatasetProps extends IDataset, RouteComponentProps<any> {}

export abstract class ProtocolSetForm extends React.Component<IDatasetProps, IDatasetState> {
    private static DEFAULT_NEW_COLUMN_INDEX = 1;
    private static DEFAULT_START_LINE = 2;

    constructor(props: IDatasetProps) {
        super(props);

        this.state = {
            columnForm: 'new', columnNumber: ProtocolSetForm.DEFAULT_NEW_COLUMN_INDEX, file: new File([], ''),
            index: ProtocolSetForm.DEFAULT_NEW_COLUMN_INDEX, mappingTable: { dataset: [] }, position: 'end',
            protocolSet: {
                titleEng: '', descriptionEng: '', titleRus: '', descriptionRus: '',
                id: props.match.params.id
            }, startLine: ProtocolSetForm.DEFAULT_START_LINE
        };

        this.disabledEdit = this.disabledEdit.bind(this);
        this.openColumnForm = this.openColumnForm.bind(this);
        this.pickFile = this.pickFile.bind(this);

        this.changeStartLine = this.changeStartLine.bind(this);
        this.changeTitleRu = this.changeTitleRu.bind(this);
        this.changeTitle = this.changeTitle.bind(this);
        this.changeDescrRu = this.changeDescrRu.bind(this);
        this.changeDescr = this.changeDescr.bind(this);
        this.changePosition = this.changePosition.bind(this);
        this.submitForm = this.submitForm.bind(this);

        this.changeSelectedMapping = this.changeSelectedMapping.bind(this);
        this.loadMapping = this.loadMapping.bind(this);
        this.saveMapping = this.saveMapping.bind(this);
    }

    public render(): React.ReactNode {
        // TODO: CSRF protection
        return (
            <div>
                <h1>{this.headerText()}</h1>
                <form onSubmit={this.submitForm}>
                    <div className='row'>
                        <div className='col-sm-6'>
                            <div className='col-sm-4'>
                                <div className='form-group'>
                                    <label htmlFor='titleRus'>TitleRus</label>
                                    <input id='titleRus' value={this.state.protocolSet.titleRus}
                                        onChange={this.changeTitleRu} className='form-control' />
                                </div>
                            </div>
                            <div className='col-sm-7'>
                                <div className='form-group'>
                                    <label htmlFor='descriptionRus'>DescriptionRus</label>
                                    <textarea id='descriptionRus' value={this.state.protocolSet.descriptionRus}
                                        onChange={this.changeDescrRu} className='form-control' ></textarea>
                                </div>
                            </div>
                            <div className='col-sm-4'>
                                <div className='form-group'>
                                    <label htmlFor='titleEng'>TitleEng</label>
                                    <input id='titleEng' value={this.state.protocolSet.titleEng}
                                        onChange={this.changeTitle} className='form-control' />
                                </div>
                            </div>
                            <div className='col-sm-7'>
                                <div className='form-group'>
                                    <label htmlFor='descriptionEng'>DescriptionEng</label>
                                    <textarea id='descriptionEng' value={this.state.protocolSet.descriptionEng}
                                        onChange={this.changeDescr} className='form-control' />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className='row'>
                        <div className='form-group'>
                            <label htmlFor='start-line' className='col-sm-3'>Start line:</label>
                            <div className='col-sm-3'>
                                <input id='start-line' type='number' value={this.state.startLine}
                                    onChange={this.changeStartLine} className='form-control'
                                    disabled={this.disabledEdit()} />
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
                                        value='end' onChange={this.changePosition} disabled={this.disabledEdit()}/>
                                    End
                                </label>
                                <label className='radio-inline'>
                                    <input type='radio' name='add-column-position' id='begin'
                                        value='begin' onChange={this.changePosition} disabled={this.disabledEdit()} />
                                    Begin
                                </label>
                                <label className='radio-inline'>
                                    <input type='radio' name='add-column-position' id='index'
                                            value='index' onChange={this.changePosition}
                                            disabled={this.disabledEdit()} />
                                    Index:
                                </label>
                            </div>
                        </div>

                        <div className='col-sm-3'>
                            <input type='number' id='add-column-index' className='form-control'
                                defaultValue={ProtocolSetForm.DEFAULT_NEW_COLUMN_INDEX.toString()}
                                onChange={this.changePosition} disabled={this.disabledEdit()} />
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
                            <button type='button' id='save-mapping' onClick={this.saveMapping}
                                disabled={this.disabledEdit()} >
                                Save mapping
                            </button>
                        </div>
                    </div>

                    <div className='row'>
                        <div className='col-sm-3'>
                            <label htmlFor='file'>Choose file: </label>
                        </div>
                        <div className='col-sm-3'>
                            <input type='file' name='file' id='file' onChange={this.pickFile} accept='.xlsx'
                                disabled={this.disabledEdit()} />
                        </div>
                    </div>

                    <div className='row'>
                        <input type='submit' className='btn btn-primary' />
                    </div>
                </form>

                {this.mappingColumnForm()}
            </div>
        );
    }

    public componentWillMount(): void {
        if (this.disabledEdit()) {
            ImportController.Instance.protocolSet(this.props.match.params.id)
                .then((result) => this.setState({ ...this.state, protocolSet: result }));
        } else {
            this.fetchMappings();
        }
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

    protected abstract headerText(): string;
    protected abstract submitForm(e: React.FormEvent<HTMLFormElement>): void;

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
                onClick={this.openColumnForm} disabled={this.disabledEdit()}>Add column</button>
        );
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

    private disabledEdit(): boolean {
        return this.props.match.params.id != undefined;
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
                });
        }
    }

    private fetchMappings(): void {
        ImportController.Instance.mappings()
            .then((result) => this.setState({ ...this.state, mappings: result }));
    }
}
