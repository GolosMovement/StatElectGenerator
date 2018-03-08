import * as React from 'react';

import { HighchartComponent } from '../Highchart/Component';

import { ChartsController, ChartBuildParameters } from './ChartsController';
import { ChartPage } from './ChartPage';
import { DictionariesController, ElectoralDistrictDto } from './DictionariesController';

export class HistogramPage extends ChartPage {
    protected getChartData(parameters: ChartBuildParameters): Promise<Highcharts.Options> {
        return ChartsController.Instance.getHistogramData({
            ...parameters,
            stepSize: 1
        });
    }

    protected renderChart(optionsFromBackend: Highcharts.Options): JSX.Element {
        const options = {
            ...optionsFromBackend,
            title: { text: '' },
            chart: { type: 'line' },
            plotOptions: {
                line: {
                    step: 'center',
                    marker: {
                        enabled: false
                    }
                }
            }
        };

        return <HighchartComponent options={options} />;
    }
}