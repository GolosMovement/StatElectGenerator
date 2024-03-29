import React from 'react';
import { Spin, Select } from 'antd';
import { SelectValue } from 'antd/lib/select';

import { LazySelect, ILazySelectProps, LazyTreeSelect, LazyTreeSelectProps } from '../Common';
import { HighchartComponent } from '../Highchart/Component';
import { IProtocolSet } from '../Import/NewProtocolSet';
import { ChartsController } from './ChartsController';
import { DictionariesController } from './DictionariesController';

const chiSquaredStats = [
    [99.9, 27.877, 'совершенно невероятно'],
    [99.5, 23.589, 'невероятно'],
    [99, 21.666, 'очень редко'],
    [95, 16.919, 'раз из двадцати'],
    [90, 14.684, 'подозрительно'],
    [80, 12.242, 'нечасто'],
    [80, 0, 'чаще']
];

export interface IProtocol extends IModel {
    lowerProtocols: IProtocol[];
}

export interface IModel {
    id: number;
    titleRus: string;
    titleEng: string;
    titleNative: string;
}

export interface ILDAChartBuildParameters {
    protocolId: number | null;
    protocolSetId: number | null;
    lineDescriptionIds: number[];
    minValue: number | null;
}

export interface ILDAData {
    chartOptions: Highcharts.Options;
    chiSquared: number;
}

interface ILastDigitState extends ILDAChartBuildParameters {
    chartOptions?: {};
    chiSquared?: number;
    isLoading: boolean;
    customMinValue: boolean;
}

export class LastDigitAnalyzer extends React.Component<ILastDigitState, ILastDigitState> {
    constructor(props: ILastDigitState) {
        super(props);

        this.state = {
            isLoading: false, protocolSetId: null, lineDescriptionIds: [], minValue: null,
            protocolId: null, customMinValue: false
        };

        this.changeMinValue = this.changeMinValue.bind(this);
        this.changeCustomMinValue = this.changeCustomMinValue.bind(this);
        this.getChartData = this.getChartData.bind(this);
    }

    public render(): React.ReactNode {
        return (
            <div className='container-fluid'>
                <div className='row'>
                    <label className='col-sm-12 control-label'>Выборы</label>
                    <div className='col-md-6'>
                        {this.protocolSetSelect()}
                    </div>
                </div>
                <div className='row'>
                    <label className='col-sm-12 control-label'>Избирательный округ</label>
                    <div className='col-md-6'>
                        {this.protocolSelect()}
                    </div>
                </div>
                <div className='row'>
                    <label className='col-sm-12 control-label'>Параметры</label>
                    <div className='col-sm-6'>
                        {this.lineDescriptionsSelect()}
                    </div>
                </div>
                <div className='row'>
                    <label className='col-sm-12 control-label'>Числа в выборке должны быть</label>
                    {this.minValueSelect()}
                </div>
                <div className='row'>
                    <div className='col-md-1'>
                        {this.renderChartButton()}
                    </div>
                </div>
                <div className='row'>
                    {this.chartAndStats()}
                </div>
            </div>
        );
    }

    private protocolSetSelect(): React.ReactNode {
        const LSelect = LazySelect as new (props: ILazySelectProps<IProtocolSet, number>) =>
            LazySelect<IProtocolSet, number>;

        return <LSelect
            placeholder='Укажите выборы'
            itemsPromise={DictionariesController.Instance.getProtocolSets()}
            selectedValue={this.state.protocolSetId}
            getValue={(protocolSet) => protocolSet.id}
            getText={(protocolSet) => protocolSet.titleRus}
            getDescriptionText={(protocolSet) => protocolSet.descriptionRus}
            onChange={(protocolSet) => this.setState({
                ...this.state, protocolId: null, protocolSetId: protocolSet })} />;
    }

    private protocolSelect(): React.ReactNode {
        if (this.state.protocolSetId) {
            const TreeSelect = LazyTreeSelect as
                new (props: LazyTreeSelectProps<IProtocol, number>) => LazyTreeSelect<IProtocol, number>;

            return <TreeSelect
                allowClear
                placeholder='Все округа'
                itemsPromise={DictionariesController.Instance.getProtocols(this.state.protocolSetId)}
                selectedValue={this.state.protocolId}
                getValue={(protocol) => protocol.id}
                getText={(protocol) => protocol.titleRus}
                getChildren={(protocol) => protocol.lowerProtocols}
                onChange={(protocolId) => this.setState({ ...this.state, protocolId })} />;
        }
    }

    private lineDescriptionsSelect(): React.ReactNode {
        if (this.state.protocolSetId) {
            const LSelect = LazySelect as new (props: ILazySelectProps<IModel, number>) =>
                LazySelect<IModel, number>;

            return <LSelect
                mode='multiple'
                placeholder='Все параметры'
                itemsPromise={DictionariesController.Instance.getLineDescriptions(this.state.protocolSetId)}
                selectedValue={this.state.lineDescriptionIds}
                getValue={(line) => line.id}
                getText={(line) => line.titleRus}
                onChange={() => {}}
                onChangeMultiple={(lines) => this.setState({ ...this.state, lineDescriptionIds: lines as number[] })}
                className='fixed-width-select-sm' />;
        }
    }

    private minValueSelect(): React.ReactNode {
        const opts = [0, 10, 100, 1000].map((val, i) =>
            <Select.Option key={i} value={val}>&ge; {val}</Select.Option>);

        return (
            <div>
                <div className='col-sm-4'>
                    <Select onChange={this.changeMinValue} className='full-width-select'
                        placeholder='любые' allowClear>
                        {opts}
                        <Select.Option value='custom'>&ge; произвольного</Select.Option>
                    </Select>
                </div>
                {this.customMinValueInput()}
            </div>
        );
    }

    private customMinValueInput(): React.ReactNode {
        if (this.state.customMinValue) {
            const value = this.state.minValue != null ? this.state.minValue : '';

            return (
                <div className='col-sm-2'>
                    <input type='number' className='form-control'
                        onChange={this.changeCustomMinValue} value={value} />
                </div>
            );
        }
    }

    private renderChartButton(): React.ReactNode {
        if (this.state.protocolSetId) {
            return (
                <button type='button' id='render' onClick={this.getChartData} className='btn btn-primary'>
                    Анализировать
                </button>
            );
        }
    }

    private chartAndStats(): React.ReactNode {
        if (this.state.isLoading) {
            return <Spin size='large' />;
        } else if (this.state.chartOptions != null) {
            return (
                <div className='lda-result'>
                    <div className='row'>
                        <div className='col-sm-4'>
                            <h4 className='text-center'>По выборам в целом</h4>
                            <h5 className='text-center'>Уровень тревоги по выборке</h5>
                        </div>
                        <div className='col-sm-8'>
                            <h4 className='text-center'>По каждой отдельной цифре</h4>
                        </div>
                    </div>
                    <div className='row'>
                        <div className='col-sm-4 lda-result-col'>
                            <div>
                                {this.renderChiSquaredTable()}
                            </div>
                        </div>
                        <div className='col-sm-8 lda-result-col'>
                            <div className='chart-wrap'>
                                {this.renderChart()}
                            </div>
                        </div>
                    </div>
                </div>
            );
        }
    }

    private renderChiSquaredTable(): React.ReactNode {
        return (
            <table className='table table-bordered table-condensed text-center lda-result-table'>
                <thead>
                    <tr>
                        <th>Стат. значимость</th>
                        <th>χ2</th>
                        <th>Примечание</th>
                    </tr>
                </thead>
                <tbody>
                    {this.chiSquaredTableRows()}
                </tbody>
            </table>
        );
    }

    private chiSquaredTableRows(): React.ReactNode {
        let warnLevel: number;
        chiSquaredStats.forEach((stats, i) => {
            if (!warnLevel && this.state.chiSquared as number > stats[1]) {
                warnLevel = 7 - i;
            }
        });

        if (this.state.chiSquared != undefined) {
            return chiSquaredStats.map((stats, i) => {
                let calcValue;

                if (7 - warnLevel == i) {
                    calcValue = (this.state.chiSquared as number).toFixed(2);
                }

                if (7 - warnLevel <= i) {
                    return <tr key={i}>
                        <td className={`warn-level-${7 - i - 1}`}>
                            {stats[0]}%
                            <br />
                            <small>{stats[1] > 0 ? `(χ2>${stats[1]})` : ''}</small>
                        </td>
                        <td className={`warn-level-${warnLevel - 1}`}>{calcValue}</td>
                        <td className={`warn-level-${7 - i - 1}`}>{stats[2]}</td>
                    </tr>;
                } else {
                    return <tr key={i}>
                        <td>
                            {stats[0]}%
                            <br />
                            <small>{stats[1] > 0 ? `(χ2>${stats[1]})` : ''}</small>
                        </td>
                        <td></td>
                        <td>{stats[2]}</td>
                    </tr>;
                }
            });
        }
    }

    private renderChart(): React.ReactNode {
        const options = {
            ...this.state.chartOptions,
            chart: { height: '390px' },
            title: { text: 'Частота последних цифр в выборке' },
            plotOptions: {
                column: {
                    borderColor: 'black',
                    borderWidth: 1,
                    showInLegend: false,
                    tooltip: {
                        headerFormat: undefined,
                        pointFormatter: this.columnTooltipFormatter
                    }
                },
                line: {
                    marker: { enabled: false },
                    tooltip: {
                        headerFormat: undefined,
                        pointFormat: this.lineTooltipFormatter(),
                        valueDecimals: 4
                    }
                },
                series: {
                    turboThreshold: 100000
                }
            },
            xAxis: [{ categories: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], title: { text: 'Последняя цифра' } }]
        } as Highcharts.Options;
        const exportOptions = {
            chartOptions: {
                plotOptions: {
                    column: {
                        dataLabels: {
                            enabled: true,
                            formatter: this.exportDataLabelFormatter
                        }
                    }
                }
            }
        } as Highcharts.ExportingOptions;

        return <HighchartComponent options={options} exportOptions={exportOptions} />;
    }

    private columnTooltipFormatter(this: Highcharts.PointObject): string {
        return `<span>Цифра: <b>${this.x}</b><br />Частотность: <b>${this.y.toFixed(4)}</b></span>`;
    }

    private lineTooltipFormatter(): string {
        return '<span style="color: {point.color}">\u25CF</span> ' +
            '{series.name}: <b>{point.y}</b><br/>';
    }

    private exportDataLabelFormatter(this: Highcharts.PointObject): string {
        return `<span>${this.y.toFixed(4)}</span>`;
    }

    private changeMinValue(value: SelectValue | undefined): void {
        let minValue: number | null;
        let customMinValue = false;

        if (value !== undefined) {
            if (value.toString() == 'custom') {
                minValue = 0;
                customMinValue = true;
            } else {
                minValue = parseInt(value.toString(), 10);
            }
        } else {
            minValue = null;
        }

        this.setState({ ...this.state, minValue, customMinValue });
    }

    private changeCustomMinValue(e: React.ChangeEvent<HTMLInputElement>): void {
        const minValue = parseInt(e.currentTarget.value, 10);
        if (!isNaN(minValue) && minValue >= 0) {
            this.setState({ ...this.state, minValue });
        } else {
            this.setState({ ...this.state, minValue: null });
        }
    }

    private getChartData(): void {
        this.setState({ ...this.state, isLoading: true });

        const buildParams = {
            protocolId: this.state.protocolId,
            protocolSetId: this.state.protocolSetId,
            lineDescriptionIds: this.state.lineDescriptionIds,
            minValue: this.state.minValue
        } as ILDAChartBuildParameters;
        ChartsController.Instance.getLastDigitAnalyzerData(buildParams)
            .then((result) => {
                this.setState({
                    ...this.state, isLoading: false, chartOptions: result.chartOptions,
                    chiSquared: result.chiSquared
                });

                if (result.chiSquared == undefined || result.chartOptions == undefined) {
                    alert('Выборка пустая');
                }
            })
            .catch(() => alert('Ошибка'));
    }
}
