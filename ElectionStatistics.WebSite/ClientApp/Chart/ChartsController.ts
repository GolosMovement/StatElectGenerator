import * as QueryString from 'query-string'
import * as Highcharts from 'highcharts';

import { IStringDictionary } from '../Common'

export interface ChartBuildParameters {
    electionId: number;
    districtId: number | null,
    candidateId: number;
}

export interface HistogramBuildParameters extends ChartBuildParameters {
    stepSize: number;
}

export class ChartsController {
    private static instance: ChartsController;

    private readonly histogramDataPromises: IStringDictionary<Promise<Highcharts.IndividualSeriesOptions[]>> = {};
    private readonly scatterplotDataPromises: IStringDictionary<Promise<Highcharts.IndividualSeriesOptions[]>> = {};

    private constructor()
    {
    }
    
    public static get Instance()
    {
        return this.instance || (this.instance = new this());
    }

    public getHistogramData(parameters: HistogramBuildParameters) : Promise<Highcharts.IndividualSeriesOptions[]> {
        const parametersString = QueryString.stringify(parameters);
        if (!this.histogramDataPromises[parametersString]) {
            this.histogramDataPromises[parametersString] = fetch(`api/charts/histogram?${parametersString}`)
                .then(response => response.json() as Promise<Highcharts.IndividualSeriesOptions[]>)
        }
        return this.histogramDataPromises[parametersString];
    }

    public getScatterplotData(parameters: ChartBuildParameters) : Promise<Highcharts.IndividualSeriesOptions[]> {
        const parametersString = QueryString.stringify(parameters);
        if (!this.scatterplotDataPromises[parametersString]) {
            this.scatterplotDataPromises[parametersString] = fetch(`api/charts/scatterplot?${parametersString}`)
                .then(response => response.json() as Promise<Highcharts.IndividualSeriesOptions[]>)
        }
        return this.scatterplotDataPromises[parametersString];
    }
}