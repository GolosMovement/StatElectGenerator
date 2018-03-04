import * as React from 'react';

import { HighchartComponent } from '../Highchart/Component';

import { ChartsController } from './ChartsController';
import { ChartPage } from './ChartPage';

export class ScatterplotPage extends ChartPage {
    protected loadChartData(): Promise<Highcharts.IndividualSeriesOptions[]> {
        return ChartsController.Instance.getScatterplotData({
            electionId: this.state.electionId as number,
            districtId: this.state.districtId,
            candidateId: this.state.candidateId as number
        });
    }

    protected renderChart(): JSX.Element {
        return <HighchartComponent 
                title={{ text: '' }}
                chart={{ type: 'scatter' }}
                yAxis={{
                    title: {
                        text: '№ УИКа'
                    }
                }}
                xAxis={{
                    min: 0,
                    max: 100,
                    gridLineWidth: 1
                }}
                series={(this.state.series as Highcharts.IndividualSeriesOptions[])
                    .map(s => ({
                        ...s, 
                        marker: {
                            radius: 2
                        },
                        tooltip: {
                            followPointer: false,
                            pointFormat: 'УИК №{point.y}<br />{point.x:.1f}%'
                        }
                    }))
                }
            />;
    }
}