import * as React from 'react';

import { HighchartComponent } from '../Highchart/Component';

import { ChartsController, ChartBuildParameters } from './ChartsController';
import { ChartPage } from './ChartPage';
import { ElectoralDistrictDto, DictionariesController, NamedChartParameter } from './DictionariesController';

export class ScatterplotPage extends ChartPage {
    protected getXParameters(electionId: number): Promise<NamedChartParameter[]> {
        return DictionariesController.Instance.getParameters(electionId);
    }
    
    protected getYParameters(electionId: number): Promise<NamedChartParameter[]> {
        return DictionariesController.Instance.getParameters(electionId);
    }
    
    protected getChartData(parameters: ChartBuildParameters): Promise<Highcharts.Options> {
        return ChartsController.Instance.getScatterplotData(parameters);
    }

    protected renderChart(optionsFromBackend: Highcharts.Options): JSX.Element {    
        const options = {
            ...optionsFromBackend,
            title: { text: '' },
            chart: { type: 'scatter' },
            boost: {
                useGPUTranslations: true,
                usePreAllocated: true
            },
            series: (optionsFromBackend.series as Highcharts.ScatterChartSeriesOptions[])
                .map(s => ({
                    ...s,
                    marker: {
                        radius: 2
                    },
                    tooltip: {
                        ...s.tooltip,
                        followPointer: false
                    }
                })),
            plotOptions: {
                series: {
                    animation: false,
                    turboThreshold: 100000,
                    states: {
                        hover: {
                            enabled: false
                        }
                    }
                }
            },
        };

        return <HighchartComponent options={options} />;
    }
}