import * as React from 'react';

import { HighchartComponent } from '../Highchart/Component';

import { ChartsController, ChartBuildParameters } from './ChartsController';
import { OldChartPage } from './OldChartPage';
import { ElectoralDistrictDto, DictionariesController, INamedChartParameter } from './DictionariesController';

export class ScatterplotPage extends OldChartPage {
    protected renderAdditionalParameterSelectors(): JSX.Element[] {
        return [
        (<div key={1} className="row">
            <div className="col-md-3">
                {this.renderParametersSelect(
                    "Выберите параметр для оси X",
                    this.state.x,
                    electionId => DictionariesController.Instance.getParameters(electionId),
                    this.onXChange
                )}
            </div>
        </div>),
        (<div key={2} className="row">
            <div className="col-md-3">
                {this.renderParametersSelect(
                    "Выберите параметр для оси Y",
                    this.state.y,
                    electionId => DictionariesController.Instance.getParameters(electionId),
                    this.onYChange
                )}
            </div>
        </div>)
        ];
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
