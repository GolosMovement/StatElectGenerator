import React from 'react';

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
    hierarchyType: string;
    hierarchyLang: string;
    hierarchyLevel: number;
    [index: string]: any;
}

interface IMappingTableProps {
    dataset: IMappingColumn[];
    updateParentCallback: (nextTable: IMappingTableState) => void;
    editColumnParentCallback: (updatedColumn: IMappingColumn, index: number) => void;
}

export interface IMappingTableState {
    dataset: IMappingColumn[];
}

export class MappingTable extends React.Component<IMappingTableProps, IMappingTableState> {
    constructor(props: IMappingTableProps) {
        super(props);

        this.state = props;

        this.deleteColumn = this.deleteColumn.bind(this);
        this.editColumn = this.editColumn.bind(this);
    }

    public render(): React.ReactNode {
        return (
            <table className='table table-bordered table-condensed table-hover mapping-table'>
                <thead>
                    <tr>
                        <th>Номер колонки в таблице результатов</th>
                        {this.rowElements('columnNumber')}
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Тип данных</td>
                        {this.typeRows()}
                    </tr>
                    <tr>
                        <td>Заголовок (рус)</td>
                        {this.rowElements('titleRus')}
                    </tr>
                    <tr>
                        <td>Описание (рус)</td>
                        {this.rowElements('descriptionRus')}
                    </tr>
                    <tr>
                        <td>Заголовок (eng)</td>
                        {this.rowElements('titleEng')}
                    </tr>
                    <tr>
                        <td>Описание (eng)</td>
                        {this.rowElements('descriptionEng')}
                    </tr>
                    <tr>
                        <td>Официальное описание на родном языке (на случай проблем с переводом)</td>
                        {this.rowElements('descriptionNative')}
                    </tr>
                    <tr>
                        <td>Волеизъявление?</td>
                        {this.boolFieldRows('isVoteResult')}
                    </tr>
                    <tr>
                        <td>Расчётная?</td>
                        {this.boolFieldRows('isCalcResult')}
                    </tr>
                    <tr>
                        <td>Иерархия</td>
                        {this.boolFieldRows('isHierarchy')}
                    </tr>
                    <tr>
                        <td>Уровень иерархии</td>
                        {this.hierarchyLevelRows()}
                    </tr>
                    <tr>
                        <td>Язык иерархии</td>
                        {this.hierarchyLangRows()}
                    </tr>
                    <tr>
                        <td>Управление</td>
                        {this.columnButtonsRow()}
                    </tr>
                </tbody>
            </table>
        );
    }

    public componentWillReceiveProps(nextProps: IMappingTableState): void {
        this.setState(nextProps);
    }

    private rowElements(name: string): React.ReactNode {
        return this.state.dataset.map((column: IMappingColumn, i: number) =>
            <td key={i}>{column[name]}</td>
        );
    }

    private typeRows(): React.ReactNode {
        return this.state.dataset.map((column: IMappingColumn, i: number) =>
            <td key={i}>{column.isNumber ? 'число' : 'текст'}</td>
        );
    }

    private boolFieldRows(name: string): React.ReactNode {
        return this.state.dataset.map((column: IMappingColumn, i: number) =>
            <td key={i}>{this.boolCell(column[name])}</td>
        );
    }

    private boolCell(condition: boolean): React.ReactNode {
        return condition ? <span>&#10003;</span> : <span>&times;</span>;
    }

    private hierarchyLangRows(): React.ReactNode {
        return this.state.dataset.map((column: IMappingColumn, i: number) =>
            <td key={i}>{column.hierarchyType == 'name' ? column.hierarchyLang : ''}</td>
        );
    }

    private hierarchyLevelRows(): React.ReactNode {
        return this.state.dataset.map((column: IMappingColumn, i: number) =>
            <td key={i}>{column.isHierarchy ? column.hierarchyLevel : ''}</td>
        );
    }

    private columnButtonsRow(): React.ReactNode {
        return this.state.dataset.map((item: IMappingColumn, i: number) =>
            <td key={i}>{this.columnButtons(i)}</td>
        );
    }

    private columnButtons(index: number): React.ReactNode {
        return (
            <div className='btn-grouphtml'>
                <button type='button' value={index} onClick={this.editColumn} className='btn btn-xs btn-success'>
                    Edit
                </button>
                <button type='button' value={index} onClick={this.deleteColumn} className='btn btn-xs btn-danger'>
                    Delete
                </button>
            </div>
        );
    }

    private editColumn(e: React.MouseEvent<HTMLButtonElement>): void {
        const index = parseInt(e.currentTarget.value, 10);
        this.props.editColumnParentCallback(this.state.dataset[index], index);
    }

    private deleteColumn(e: React.MouseEvent<HTMLButtonElement>): void {
        if (confirm('Are you sure?')) {
            const index = parseInt(e.currentTarget.value, 10);
            const nextTable = [...this.state.dataset.slice(0, index), ...this.state.dataset.slice(index + 1)];
            this.props.updateParentCallback({ dataset: nextTable });
        }
    }
}
