import { IApiResponse } from '../Import/ImportController';
import { IPreset } from './PresetList';

export class PresetsController {
    private static instance: PresetsController;

    public static get Instance() {
        return this.instance || (this.instance = new this());
    }

    public getPreset(id: number): Promise<IPreset> {
        return fetch(`/api/presets/${id}`).then((response) => response.json());
    }

    public createPreset(preset: IPreset): Promise<IApiResponse> {
        const data = new FormData();
        data.append('preset', JSON.stringify(preset));

        return fetch('/api/presets', { body: data, method: 'POST' }).then((response) => response.json());
    }

    public updatePreset(preset: IPreset): Promise<IApiResponse> {
        const data = new FormData();
        data.append('preset', JSON.stringify(preset));

        return fetch(`/api/presets/${preset.id}`, { body: data, method: 'PATCH' }).then((response) => response.json());
    }

    public deletePreset(id: number): Promise<IApiResponse> {
        return fetch(`/api/presets/${id}`, { method: 'DELETE' }).then((response) => response.json());
    }

    public recreateCalcValues(protocolSetId: number): Promise<IApiResponse> {
        return fetch(`/api/presets/recreate/protocolSet/${protocolSetId}`, { method: 'POST' })
            .then((response) => response.json());
    }
}
