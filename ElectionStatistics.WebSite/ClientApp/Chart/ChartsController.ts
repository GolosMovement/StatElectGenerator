import * as QueryString from 'query-string'
import * as Highcharts from 'highcharts';

import { IDictionary } from '../Common'

export interface ChartBuildParameters {
    electionId: number;
    candidateId: number;
    stepSize: number;
}

export class ChartsController {
    private static instance: ChartsController;

    private readonly histogramDataPromises: IDictionary<Promise<Highcharts.IndividualSeriesOptions[]>> = {};

    private constructor()
    {
    }
    
    public static get Instance()
    {
        return this.instance || (this.instance = new this());
    }

    public getHistogramData(parameters: ChartBuildParameters) : Promise<Highcharts.IndividualSeriesOptions[]> {
        const parametersString = QueryString.stringify(parameters);
        if (!this.histogramDataPromises[parametersString]) {
            this.histogramDataPromises[parametersString] = fetch(`api/charts/histogram?${parametersString}`)
                .then(response => response.json() as Promise<Highcharts.IndividualSeriesOptions[]>)
        }
        return this.histogramDataPromises[parametersString];
    }
}