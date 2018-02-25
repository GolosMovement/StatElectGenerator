import * as QueryString from 'query-string'
import * as Highcharts from 'highcharts';

import { IDictionary } from '../Common'

export interface ElectionDto {
    id: number;
    name: string;
}

export interface CandidateDto {
    id: number;
    name: string;
}

export class DictionariesController {
    private static instance: DictionariesController;

    private elections?: Promise<ElectionDto[]>;
    private candidatesByElection: IDictionary<Promise<CandidateDto[]>> = {};

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

    public getCandidatesByElection(electionId: number) : Promise<CandidateDto[]> {
        if (!this.candidatesByElection[electionId]) {
            this.candidatesByElection[electionId] = fetch(`api/candidates/by-election?electionId=${electionId}`)
                .then(response => response.json() as Promise<CandidateDto[]>)
        }
        return this.candidatesByElection[electionId];
    }
}