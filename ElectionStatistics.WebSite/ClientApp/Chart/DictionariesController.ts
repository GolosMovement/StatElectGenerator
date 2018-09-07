import { INumberDictionary, IStringDictionary } from '../Common';
import { IProtocolSet } from '../Import/NewProtocolSet';
import { IPreset } from '../Presets';
import { ILineDescription } from '../Presets/PresetForm';
import { IModel, IProtocol } from './LastDigitAnalyzer';

export interface NamedChartParameter {
    name: string;
    parameter: ChartParameter;
}

export interface ChartParameter {
    $type: string;
}

export interface ElectionDto {
    id: number;
    name: string;
}

export interface ElectoralDistrictDto {
    id: number;
    name: string;
    lowerDitsrticts: ElectoralDistrictDto[];
}

export class DictionariesController {
    public static reset(): void {
        this.initInstance();
    }

    private static instance: DictionariesController;

    public static get Instance() {
        if (!this.instance) { this.initInstance(); }

        return this.instance;
    }

    private static initInstance(): void {
        this.instance = new this();
    }

    private elections?: Promise<ElectionDto[]>;
    private districtsByElection: INumberDictionary<Promise<ElectoralDistrictDto[]>> = {};
    private parametersByElection: INumberDictionary<Promise<NamedChartParameter[]>> = {};
    private lineDescriptionsByProtocolSet: INumberDictionary<Promise<ILineDescription[]>> = {};
    private protocols: INumberDictionary<Promise<IProtocol[]>> = {};
    private presetsByProtocolSet: INumberDictionary<Promise<IPreset[]>> = {};
    private summaryParameters?: Promise<NamedChartParameter[]>;
    private protocolSets?: Promise<IProtocolSet[]>;

    private constructor() {}

    public getElections(): Promise<ElectionDto[]> {
        if (!this.elections) {
            this.elections = fetch(`api/elections`)
                .then((response) => response.json() as Promise<ElectionDto[]>);
        }
        return this.elections;
    }

    public getDistricts(electionId: number): Promise<ElectoralDistrictDto[]> {
        if (!this.districtsByElection[electionId]) {
            this.districtsByElection[electionId] = fetch(`api/districts?electionId=${electionId}`)
                .then((response) => response.json() as Promise<ElectoralDistrictDto[]>);
        }
        return this.districtsByElection[electionId];
    }

    public getParameters(electionId: number): Promise<NamedChartParameter[]> {
        if (!this.parametersByElection[electionId]) {
            this.parametersByElection[electionId] = fetch(`api/parameters?electionId=${electionId}`)
                .then((response) => response.json() as Promise<NamedChartParameter[]>);
        }
        return this.parametersByElection[electionId];
    }

    public getSummaryParameters(): Promise<NamedChartParameter[]> {
        if (!this.summaryParameters) {
            this.summaryParameters = fetch(`api/summary-parameters`)
                .then((response) => response.json() as Promise<NamedChartParameter[]>);
        }
        return this.summaryParameters;
    }

    public getProtocolSets(): Promise<IProtocolSet[]> {
        if (!this.protocolSets) {
            this.protocolSets = fetch('/api/protocolSets').then((response) => response.json());
        }
        return this.protocolSets;
    }

    public getProtocols(protocolSetId: number): Promise<IProtocol[]> {
        if (!this.protocols[protocolSetId]) {
            this.protocols[protocolSetId] = fetch(`/api/protocols?protocolSetId=${protocolSetId}`)
                .then((response) => response.json());
        }
        return this.protocols[protocolSetId];
    }

    public getLineDescriptions(protocolSetId: number): Promise<ILineDescription[]> {
        if (!this.lineDescriptionsByProtocolSet[protocolSetId]) {
            this.lineDescriptionsByProtocolSet[protocolSetId] =
                fetch(`/api/lineDescriptions?protocolSetId=${protocolSetId}`)
                    .then((response) => response.json());
        }
        return this.lineDescriptionsByProtocolSet[protocolSetId];
    }

    public getPresets(protocolSetId: number): Promise<IPreset[]> {
        if (!this.presetsByProtocolSet[protocolSetId]) {
            this.presetsByProtocolSet[protocolSetId] = fetch(`/api/presets?protocolSetId=${protocolSetId}`)
                .then((response) => response.json());
        }
        return this.presetsByProtocolSet[protocolSetId];
    }
}
