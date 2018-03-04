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
    
    protected getChartData(parameters: ChartBuildParameters): Promise<Highcharts.IndividualSeriesOptions[]> {
        return ChartsController.Instance.getScatterplotData(parameters);
    }

    protected renderChart(): JSX.Element {
        return <HighchartComponent 
                title={{ text: '' }}
                chart={{ type: 'scatter' }}
                yAxis={{
                    title: {
                        text: ''
                    }
                }}
                xAxis={{
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
                            pointFormat: '{point.name}<br />{point.y:.1f}%'
                        }
                    }))
                }
            />;
    }
}