import React from 'react';
import { Link } from 'react-router-dom';

import moment from 'moment';

import { IProtocolSet } from './NewProtocolSet';
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
                        <tr className='text-center'>
                            <th>NameRus</th>
                            <th>NameEng</th>
                            <th>Hidden</th>
                            <th>Import started at</th>
                            <th>Import finished at</th>
                            <th>Import current line</th>
                            <th>Import total lines</th>
                            <th>Imported</th>
                            <th>Import error count</th>
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
                <td className='text-center'>{protocolSet.hidden ? <span>&#10003;</span> : ''}</td>
                <td className='text-center'>{this.formatTime(protocolSet.importStartedAt)}</td>
                <td className='text-center'>{this.formatTime(protocolSet.importFinishedAt)}</td>
                <td className='text-center'>{protocolSet.importCurrentLine}</td>
                <td className='text-center'>{protocolSet.importTotalLines}</td>
                <td className={`${this.importStatusColor(protocolSet)} text-center`}>
                    {this.importStatus(protocolSet)}
                </td>
                <td className='text-center'>{protocolSet.importErrorCount}</td>
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

    private importStatus(protocolSet: IProtocolSet): React.ReactNode {
        if (protocolSet.importFinishedAt && protocolSet.importSuccess) {
            return <span>&#10003;</span>;
        }

        const progressRate = this.importProgress(protocolSet);
        if (progressRate != undefined && progressRate != '100') {
            return `${progressRate}%`;
        }
    }

    private importStatusColor(protocolSet: IProtocolSet): string {
        if (protocolSet.importFinishedAt) {
            if (protocolSet.importSuccess) {
                return protocolSet.importErrorCount == 0 ? 'green' : 'yellow';
            } else {
                return 'red';
            }
        }

        return 'white';
    }

    private importProgress(protocolSet: IProtocolSet): string | undefined {
        if (protocolSet.importCurrentLine && protocolSet.importTotalLines) {
            const rate = protocolSet.importCurrentLine / protocolSet.importTotalLines;
            if (!isNaN(rate)) {
                return (rate * 100).toFixed();
            }
        }
    }

    private formatTime(timestamp: Date | undefined): string | undefined {
        if (timestamp) {
            return moment(timestamp).format('D.M.Y H:m');
        }
    }
}
