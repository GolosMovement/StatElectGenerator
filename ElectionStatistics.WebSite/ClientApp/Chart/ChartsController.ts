import Highcharts from 'highcharts';

import { IStringDictionary } from '../Common';
import { ChartParameter } from './DictionariesController';
import { ILDAChartBuildParameters, ILDAData } from './LastDigitAnalyzer';

export interface ChartBuildParameters {
    electionId: number;
    districtId: number | null;
    x?: ChartParameter;
    y: ChartParameter;
}

export interface HistogramBuildParameters extends ChartBuildParameters {
    stepSize: number;
}

export class ChartsController {
    private static instance: ChartsController;

    public static get Instance() {
        return this.instance || (this.instance = new this());
    }

    private readonly histogramDataPromises: IStringDictionary<Promise<Highcharts.Options>> = {};
    private readonly scatterplotDataPromises: IStringDictionary<Promise<Highcharts.Options>> = {};
    private readonly locationScatterplotDataPromises: IStringDictionary<Promise<Highcharts.Options>> = {};
    private readonly lastDigitAnalyzerDataPromises: IStringDictionary<Promise<ILDAData>> = {};

    private constructor() {}

    public getHistogramData(parameters: HistogramBuildParameters): Promise<Highcharts.Options> {
        const parametersString = JSON.stringify(parameters);
        if (!this.histogramDataPromises[parametersString]) {
            this.histogramDataPromises[parametersString] =
                fetch(`api/charts/histogram?parametersString=${parametersString}`)
                    .then((response) => response.json() as Promise<Highcharts.Options>);
        }
        return this.histogramDataPromises[parametersString];
    }

    public getScatterplotData(parameters: ChartBuildParameters) : Promise<Highcharts.Options> {
        const parametersString = JSON.stringify(parameters);
        if (!this.scatterplotDataPromises[parametersString]) {
            this.scatterplotDataPromises[parametersString] =
                fetch(`api/charts/scatterplot?parametersString=${parametersString}`)
                    .then((response) => response.json() as Promise<Highcharts.Options>);
        }
        return this.scatterplotDataPromises[parametersString];
    }

    public getLocationScatterplotData(parameters: ChartBuildParameters) : Promise<Highcharts.Options> {
        const parametersString = JSON.stringify(parameters);
        if (!this.locationScatterplotDataPromises[parametersString]) {
            this.locationScatterplotDataPromises[parametersString] =
                fetch(`api/charts/location-scatterplot?parametersString=${parametersString}`)
                    .then((response) => response.json() as Promise<Highcharts.Options>);
        }
        return this.locationScatterplotDataPromises[parametersString];
    }

    public getLastDigitAnalyzerData(parameters: ILDAChartBuildParameters): Promise<ILDAData> {
        const parametersString = JSON.stringify(parameters);
        if (!this.lastDigitAnalyzerDataPromises[parametersString]) {
            this.lastDigitAnalyzerDataPromises[parametersString] =
                fetch(`api/charts/last-digit-analyzer?parametersString=${parametersString}`)
                .then((response) => response.json());
        }
        return this.lastDigitAnalyzerDataPromises[parametersString];
    }
}
