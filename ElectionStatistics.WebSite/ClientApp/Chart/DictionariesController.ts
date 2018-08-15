import * as Highcharts from 'highcharts';

import { INumberDictionary } from '../Common'

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
    private static instance: DictionariesController;

    private elections?: Promise<ElectionDto[]>;
    private districtsByElection: INumberDictionary<Promise<ElectoralDistrictDto[]>> = {};
    private parametersByElection: INumberDictionary<Promise<NamedChartParameter[]>> = {};
    private summaryParameters?: Promise<NamedChartParameter[]>;

    private constructor()
    {
    }

    public static get Instance()
    {
        return this.instance || (this.instance = new this());
    }

    public getElections() : Promise<ElectionDto[]> {
        if (!this.elections) {
            this.elections = fetch(`api/elections`)
                .then(response => response.json() as Promise<ElectionDto[]>)
        }
        return this.elections;
    }

    public getDistricts(electionId: number) : Promise<ElectoralDistrictDto[]> {
        if (!this.districtsByElection[electionId]) {
            this.districtsByElection[electionId] = fetch(`api/districts?electionId=${electionId}`)
                .then(response => response.json() as Promise<ElectoralDistrictDto[]>)
        }
        return this.districtsByElection[electionId];
    }

    public getParameters(electionId: number) : Promise<NamedChartParameter[]> {
        if (!this.parametersByElection[electionId]) {
            this.parametersByElection[electionId] = fetch(`api/parameters?electionId=${electionId}`)
                .then(response => response.json() as Promise<NamedChartParameter[]>)
        }
        return this.parametersByElection[electionId];
    }

    public getSummaryParameters() : Promise<NamedChartParameter[]> {
        if (!this.summaryParameters) {
            this.summaryParameters = fetch(`api/summary-parameters`)
                .then(response => response.json() as Promise<NamedChartParameter[]>)
        }
        return this.summaryParameters;
    }
}
