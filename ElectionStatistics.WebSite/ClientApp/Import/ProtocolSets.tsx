import React from 'react';
import { Link } from 'react-router-dom';

import { DictionariesController } from '../Chart/DictionariesController';
import { IProtocolSet } from './ProtocolSetForm';
import { ImportController } from './ImportController';

interface IProtocolSetState {
    list: IProtocolSet[];
}

export class ProtocolSets extends React.Component<{}, IProtocolSetState> {
    constructor(props: {}) {
        super(props);

        this.state = { list: [] };

        this.fetchList();
        this.tableRow = this.tableRow.bind(this);
    }

    // TODO: pagination
    public render(): React.ReactNode {
        return (
            <div>
                <h1>ProtocolSets</h1>
                <table className='table-bordered table-condensed table-hover'>
                    <thead>
                        <tr>
                            <th>NameRus</th>
                            <th>NameEng</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.tableRow()}
                    </tbody>
                </table>
            </div>
        );
    }

    private tableRow(): React.ReactNode {
        const elements = this.state.list.map((protocolSet, i) =>
            <tr key={i}>
                <td>{protocolSet.titleRus}</td>
                <td>{protocolSet.titleEng}</td>
                <td>
                    <Link to={`/import/protocolSets/edit/${protocolSet.id}`} className='btn btn-xs btn-primary'
                        children='Edit' />
                </td>
            </tr>
        );

        return elements;
    }

    private fetchList(): void {
        ImportController.Instance.protocolSets()
            .then((result) => this.setState({ ...this.state, list: result }));
    }
}
