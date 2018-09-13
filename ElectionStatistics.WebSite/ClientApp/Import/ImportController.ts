import { IMappingColumn } from './MappingTable';
import { IProtocolSet, IMappingList } from './NewProtocolSet';

interface IApiResponse {
    status: string;
    message: string;
    data?: any;
}

export class ImportController {
    private static instance: ImportController;

    public static get Instance() {
        return this.instance || (this.instance = new this());
    }

    public signIn(user: string, password: string): Promise<boolean> {
        const reqPromise = fetch('/api/signIn', {
            body: JSON.stringify({ user, password }),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            method: 'POST'
        });
        return reqPromise.then((response) => response.json());
    }

    public createDataset(mappingTable: IMappingColumn[], file: File, protocolSet: IProtocolSet,
                         startLine: number): Promise<IApiResponse> {
        const data = new FormData();
        data.append('mappingTable', JSON.stringify(mappingTable));
        data.append('file', file);
        data.append('protocolSet', JSON.stringify(protocolSet));
        data.append('startLine', startLine.toString());

        const reqPromise = fetch('/api/import/protocolSets', {
            body: data,
            method: 'POST'
        });

        return reqPromise.then((response) => response.json());
    }

    public progress(id: number): Promise<IApiResponse> {
        const reqPromise = fetch(`/api/import/protocolSets/${id}/progress`);
        return reqPromise.then((response) => response.json());
    }

    public updateDataset(id: number, protocolSet: IProtocolSet): Promise<IApiResponse> {
        const data = new FormData();
        data.append('protocolSet', JSON.stringify(protocolSet));

        const reqPromise = fetch(`/api/import/protocolSets/${id}`, {
            body: data,
            method: 'PATCH'
        });
        return reqPromise.then((response) => response.json());
    }

    public protocolSet(id: number): Promise<IProtocolSet> {
        const reqPromise = fetch(`/api/import/protocolSets/${id}`, {
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        });

        return reqPromise.then((result) => result.json());
    }

    public mappings(): Promise<IMappingList[]> {
        const reqPromise = fetch('/api/import/mappings', {
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        });

        return reqPromise.then((response) => response.json());
    }

    public createMapping(name: string, dataLineNumber: number, mappingTable: IMappingColumn[]): Promise<IApiResponse> {
        const data = new FormData();
        data.append('name', name);
        data.append('dataLineNumber', dataLineNumber.toString());
        data.append('mappingTable', JSON.stringify(mappingTable));

        const reqPromise = fetch('/api/import/mappings', {
            body: data,
            method: 'POST'
        });

        return reqPromise.then((response) => response.json());
    }

    public protocolSets(): Promise<IProtocolSet[]> {
        return fetch('/api/import/protocolSets')
            .then((response) => response.json());
    }
}
