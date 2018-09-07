import { ImportController } from './ImportController';
import { ProtocolSetForm } from './ProtocolSetForm';

export class EditDataset extends ProtocolSetForm {
    protected headerText(): string {
        return 'Edit ProtocolSet';
    }

    protected submitForm(): void {
        ImportController.Instance.updateDataset(this.props.match.params.id, this.state.protocolSet)
            .then((result) => {
                this.setState({ ...this.state, isLoading: false });

                if (result.status == 'ok') {
                    alert('Success!');
                } else {
                    alert('Fail!');
                }
        });
    }
}
