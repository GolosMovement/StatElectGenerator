import * as React from 'react';

import { HighchartComponent } from '../Highchart/Component';

import { ChartsController, ChartBuildParameters } from './ChartsController';
import { ChartPage } from './ChartPage';
import { ElectoralDistrictDto, DictionariesController } from './DictionariesController';

export class ScatterplotPage extends ChartPage {
    protected getDistricts(electionId: number): Promise<ElectoralDistrictDto[]> {
        return DictionariesController.Instance.getDistricts({
            electionId: electionId,
            forScatterplot: true
        });
    }
    
    protected getChartData(parameters: ChartBuildParameters): Promise<Highcharts.Options> {
        return ChartsController.Instance.getScatterplotData(parameters);
    }

    protected renderChart(optionsFromBackend: Highcharts.Options): JSX.Element {    
        const options: Highcharts.Options = {
            ...optionsFromBackend,
            title: { text: '' },
            chart: { type: 'scatter' },
            xAxis: {
                labels: {
                    enabled: false
                }
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
                }))
        };

        return <HighchartComponent options={options} />;
    }
}