import { ImportController } from './ImportController';
import { ProtocolSetForm } from './ProtocolSetForm';

export class EditDataset extends ProtocolSetForm {
    protected headerText(): string {
        return 'Edit ProtocolSet';
    }

    protected submitForm(e: React.FormEvent<HTMLFormElement>): void {
        e.preventDefault();

        ImportController.Instance.updateDataset(this.props.match.params.id, this.state.protocolSet)
            .then((result) => {
                if (result.status == 'ok') {
                    alert('Success!');
                } else {
                    alert('Fail!');
            }
        });
    }
}
