import { DictionariesController } from '../Chart/DictionariesController';
import { IApiResponse } from '../Import/ImportController';
import { PresetForm } from './PresetForm';
import { IPreset } from './PresetList';
import { PresetsController } from './PresetsController';

export class NewPreset extends PresetForm {
    public componentWillMount(): void {
        if (this.state.preset.protocolSetId) {
            DictionariesController.Instance.getLineDescriptions(this.state.preset.protocolSetId)
                .then((res) => this.setState({ ...this.state, lineDescriptions: res }));
        }
    }

    protected submitRequestFunc(): (preset: IPreset) => Promise<IApiResponse> {
        return PresetsController.Instance.createPreset;
    }
}
