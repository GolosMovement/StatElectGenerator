import * as QueryString from 'query-string'
import * as Highcharts from 'highcharts';

import { IDictionary } from '../Common'

export interface Election {
    id: number;
    name: string;
}

export class ElectionsController {
    private static instance: ElectionsController;

    private elections: Promise<Election[]>;

    private constructor()
    {
    }
    
    public static get Instance()
    {
        return this.instance || (this.instance = new this());
    }

    public getElections() : Promise<Election[]> {
        if (!this.elections) {
            this.elections = fetch(`api/elections`)
                .then(response => response.json() as Promise<Election[]>)
        }
        return this.elections;
    }
}