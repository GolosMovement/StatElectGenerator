import * as QueryString from 'query-string'
import * as Highcharts from 'highcharts';

import { IStringDictionary } from '../Common'

export interface CandidateDto {
    id: number;
    name: string;
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

export interface GetDistrtictsRequest {
    electionId: number;
    forScatterplot?: boolean;
}

export class DictionariesController {
    private static instance: DictionariesController;

    private elections?: Promise<ElectionDto[]>;
    private districtsByElection: IStringDictionary<Promise<ElectoralDistrictDto[]>> = {};
    private candidatesByElection: IStringDictionary<Promise<CandidateDto[]>> = {};

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

    public getDistricts(request: GetDistrtictsRequest) : Promise<ElectoralDistrictDto[]> {
        const queryString = QueryString.stringify(request);

        if (!this.districtsByElection[queryString]) {
            this.districtsByElection[queryString] = fetch(`api/districts?${queryString}`)
                .then(response => response.json() as Promise<ElectoralDistrictDto[]>)
        }
        return this.districtsByElection[queryString];
    }

    public getCandidates(electionId: number) : Promise<CandidateDto[]> {
        if (!this.candidatesByElection[electionId]) {
            this.candidatesByElection[electionId] = fetch(`api/candidates?electionId=${electionId}`)
                .then(response => response.json() as Promise<CandidateDto[]>)
        }
        return this.candidatesByElection[electionId];
    }
}