import * as React from 'react';

import { HighchartComponent } from '../Highchart/Component';

import { ChartsController, ChartBuildParameters } from './ChartsController';
import { ChartPage } from './ChartPage';
import { DictionariesController, ElectoralDistrictDto, NamedChartParameter } from './DictionariesController';

export class HistogramPage extends ChartPage {
    protected getXParameters(electionId: number): Promise<NamedChartParameter[]> {
        return DictionariesController.Instance.getParameters(electionId);
    }

    protected getYParameters(electionId: number): Promise<NamedChartParameter[]> {
        return DictionariesController.Instance.getSummaryParameters();
    }

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
                series: {
                    turboThreshold: 100000
                },
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