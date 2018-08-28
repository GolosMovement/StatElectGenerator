import React from 'react';

import { ImportController } from './ImportController';
import { ProtocolSetForm } from './ProtocolSetForm';

export class NewDataset extends ProtocolSetForm {
    protected headerText(): string {
        return 'Import ProtocolSet';
    }

    protected submitForm(e: React.FormEvent<HTMLFormElement>): void {
        e.preventDefault();

        ImportController.Instance.createDataset(this.state.mappingTable.dataset, this.state.file, this.state.protocol,
            this.state.startLine)
            .then((result) => {
                if (result.status == 'ok') {
                    alert('Success!');
                } else {
                    alert(`Fail! ${result.message}`);
                }
            }).catch((err) => { alert('Server error!'); });
    }
}
