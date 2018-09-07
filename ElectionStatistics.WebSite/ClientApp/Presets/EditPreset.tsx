import { DictionariesController } from '../Chart/DictionariesController';
import { IApiResponse } from '../Import/ImportController';
import { PresetForm } from './PresetForm';
import { IPreset } from './PresetList';
import { PresetsController } from './PresetsController';

export class EditPreset extends PresetForm {
    public componentWillMount(): void {
        PresetsController.Instance.getPreset(this.props.match.params.id)
            .then((result) => {
                this.setState({ ...this.state, preset: result });

                if (result.protocolSetId) {
                    DictionariesController.Instance.getLineDescriptions(result.protocolSetId)
                        .then((res) => this.setState({ ...this.state, lineDescriptions: res }));
                }
            });
    }

    protected submitRequestFunc(): (preset: IPreset) => Promise<IApiResponse> {
        return PresetsController.Instance.updatePreset;
    }
}
