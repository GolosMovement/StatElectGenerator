import React from 'react';
import { Link } from 'react-router-dom';

import { HighchartComponent } from '../Highchart/Component';

import { QueryString } from '../Common';
import { ChartPage, IScatterplotChartPageRouteProps } from './ChartPage';
import { ChartsController, IChartBuildParameters } from './ChartsController';
import { DictionariesController } from './DictionariesController';

export class LocationScatterplotPage extends ChartPage {
    protected renderAdditionalParameterSelectors(): React.ReactNode[] {
        return [
            <div key={1} className='row'>
                <div className='col-md-3'>
                    {this.presetSelect(
                        'Выберите параметр для оси Y',
                        this.state.yId,
                        (protocolSetId) => DictionariesController.Instance.getPresets(protocolSetId),
                        this.onYChange
                    )}
                </div>
            </div>
        ];
    }

    protected getChartData(parameters: IChartBuildParameters): Promise<Highcharts.Options> {
        return ChartsController.Instance.getLocationScatterplotData(parameters);
    }

    protected renderButton(): React.ReactNode {
        if (this.state.protocolSetId === null ||
            this.state.yId === null) {
            return null;
        } else {
            const queryParams: IScatterplotChartPageRouteProps = {
                protocolSetId: this.state.protocolSetId,
                protocolId: this.state.protocolId || undefined,
                y: this.state.yId
            };

            return (
                <Link
                    className='btn btn-primary'
                    to={{ search: QueryString.stringify(queryParams)}}>
                    Построить
                </Link>
            );
        }
    }

    protected tryLoadChartData(): void {
        if (this.state.protocolSetId !== null &&
            this.state.yId !== null) {
            this.setState({
                ...this.state,
                isLoading: true
            });

            this.getChartData({
                    protocolSetId: this.state.protocolSetId,
                    protocolId: this.state.protocolId,
                    y: this.state.yId
                })
                .then((series) => {
                    this.setState({
                        ...this.state,
                        isLoading: false,
                        chartOptions: series
                    });
                });
        }
    }

    protected renderChart(optionsFromBackend: Highcharts.Options): React.ReactNode {
        const options = {
            ...optionsFromBackend,
            title: { text: '' },
            chart: { type: 'scatter' },
            boost: {
                useGPUTranslations: true,
                usePreAllocated: true
            },
            series: (optionsFromBackend.series as Highcharts.ScatterChartSeriesOptions[])
                .map((s) => ({
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
            }
        };

        return <HighchartComponent options={options} />;
    }
}
