import * as React from 'react';

import { HighchartComponent } from '../Highchart/Component';

import { ChartsController, ChartBuildParameters } from './ChartsController';
import { ChartPage } from './ChartPage';
import { DictionariesController, ElectoralDistrictDto } from './DictionariesController';

export class HistogramPage extends ChartPage {
    protected getDistricts(electionId: number): Promise<ElectoralDistrictDto[]> {
        return DictionariesController.Instance.getDistricts({
            electionId: electionId
        });
    }

    protected getChartData(parameters: ChartBuildParameters): Promise<Highcharts.IndividualSeriesOptions[]> {
        return ChartsController.Instance.getHistogramData({
            ...parameters,
            stepSize: 1
        });
    }

    protected renderChart(): JSX.Element {
        return <HighchartComponent 
                title={{ text: '' }}
                chart={{ type: 'line' }}
                yAxis={{
                    title: {
                        text: 'Количество избирателей зарегистрированных на участках'
                    }
                }}
                xAxis={{
                    min: 0,
                    max: 100,
                    gridLineWidth: 1
                }}
                series={this.state.series}                
                plotOptions={{
                    line: {
                        step: 'center',
                        marker: {
                            enabled: false
                        }
                    }
                }}
            />;
    }
}