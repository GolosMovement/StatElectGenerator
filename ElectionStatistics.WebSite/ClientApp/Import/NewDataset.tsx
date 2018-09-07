import React from 'react';

import { ImportController } from './ImportController';
import { ProtocolSetForm } from './ProtocolSetForm';

export class NewDataset extends ProtocolSetForm {
    protected headerText(): string {
        return 'Import ProtocolSet';
    }

    protected submitForm(): void {
        ImportController.Instance.createDataset(this.state.mappingTable.dataset, this.state.file,
            this.state.protocolSet, this.state.startLine)
            .then((result) => {
                this.setState({ ...this.state, isLoading: false });

                if (result.status == 'ok') {
                    this.setState({ ...this.state, protocolSet: { ...this.state.protocolSet, id: result.data }});
                    alert('Success!');
                } else {
                    alert(`Fail! ${result.message}`);
                }
            }).catch((err) => {
                this.setState({ ...this.state, isLoading: false });
                alert('Server error!');
            });
    }
}
