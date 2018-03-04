import * as React from 'react';

import { HighchartComponent } from '../Highchart/Component';

import { ChartsController } from './ChartsController';
import { ChartPage } from './ChartPage';

export class HistogramPage extends ChartPage {    
    protected loadChartData(): Promise<Highcharts.IndividualSeriesOptions[]> {
        return ChartsController.Instance.getHistogramData({
            electionId: this.state.electionId as number,
            districtId: this.state.districtId,
            candidateId: this.state.candidateId as number,
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