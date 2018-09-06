import React from 'react';
import { Spin, Select } from 'antd';
import { SelectValue } from 'antd/lib/select';

import { LazySelect, LazySelectProps } from '../Common';
import { HighchartComponent } from '../Highchart/Component';
import { IProtocolSet } from '../Import/ProtocolSetForm';
import { ChartsController } from './ChartsController';
import { DictionariesController } from './DictionariesController';
import { AxisOptions, PlotOptions } from 'highcharts';

const chiSquaredStats = [
    [99.9, 27.877, 'совершенно невероятно'],
    [99.5, 23.589, 'невероятно'],
    [99, 21.666, 'очень редко'],
    [95, 16.919, 'раз из двадцати'],
    [90, 14.684, 'подозрительно'],
    [80, 12.242, 'нечасто'],
    [80, 0, 'чаще']
];

export interface IModel {
    id: number;
    titleRus: string;
    titleEng: string;
    titleNative: string;
}

export interface ILDAChartBuildParameters {
    protocolSetId: number | null;
    protocolTopId: number | null;
    protocolMidId: number | null;
    protocolBotId: number | null;
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
}

export class LastDigitAnalyzer extends React.Component<ILastDigitState, ILastDigitState> {
    constructor(props: ILastDigitState) {
        super(props);

        this.state = {
            isLoading: false, protocolSetId: null, protocolTopId: null, protocolMidId: null,
            protocolBotId: null, lineDescriptionIds: [], minValue: null
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
                    {this.protocolSelects()}
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
        const LSelect = LazySelect as new (props: LazySelectProps<IProtocolSet, number>) =>
            LazySelect<IProtocolSet, number>;

        return <LSelect
            itemsPromise={DictionariesController.Instance.getProtocolSets()}
            selectedValue={this.state.protocolSetId}
            getValue={(protocolSet) => protocolSet.id ? protocolSet.id : 0}
            getText={(protocolSet) => protocolSet ? protocolSet.titleRus : ''}
            onChange={(protocolSet) => this.setState({
                ...this.state,
                protocolSetId: protocolSet, protocolTopId: null, protocolMidId: null, protocolBotId: null
            })} />;
    }

    private protocolSelects(): React.ReactNode {
        return (
            <div className='col-sm-12'>
                <div className='inline-block'>{this.protocolTopSelect()}</div>
                <div className='inline-block'>{this.protocolMidSelect()}</div>
                <div className='inline-block'>{this.protocolBotSelect()}</div>
            </div>
        );
    }

    private protocolTopSelect(): React.ReactNode {
        if (this.state.protocolSetId) {
            const LSelect = LazySelect as new (props: LazySelectProps<IModel, number>) =>
                LazySelect<IModel, number>;

            return <LSelect
                itemsPromise={DictionariesController.Instance.getProtocols(this.state.protocolSetId)}
                selectedValue={this.state.protocolTopId}
                getValue={(protocol) => protocol.id ? protocol.id : 0}
                getText={(protocol) => protocol ? protocol.titleRus : ''}
                onChange={(protocolId) => this.setState({
                    ...this.state,
                    protocolTopId: protocolId, protocolMidId: null, protocolBotId: null
                })} className='fixed-width-select-sm' />;
        } else {
            return;
        }
    }

    private protocolMidSelect(): React.ReactNode {
        if (this.state.protocolTopId) {
            const LSelect = LazySelect as new (props: LazySelectProps<IModel, number>) =>
                LazySelect<IModel, number>;

            return <LSelect
                itemsPromise={DictionariesController.Instance.getProtocols(this.state.protocolSetId,
                    this.state.protocolTopId)}
                selectedValue={this.state.protocolMidId}
                getValue={(protocol) => protocol.id ? protocol.id : 0}
                getText={(protocol) => protocol ? protocol.titleRus : ''}
                onChange={(protocolId) => this.setState({
                    ...this.state,
                    protocolMidId: protocolId, protocolBotId: null
                })} className='fixed-width-select-sm' />;
        } else {
            return;
        }
    }

    private protocolBotSelect(): React.ReactNode {
        if (this.state.protocolMidId) {
            const LSelect = LazySelect as new (props: LazySelectProps<IModel, number>) =>
                LazySelect<IModel, number>;

            return <LSelect
                itemsPromise={DictionariesController.Instance.getProtocols(this.state.protocolSetId,
                    this.state.protocolMidId)}
                selectedValue={this.state.protocolBotId}
                getValue={(protocol) => protocol.id ? protocol.id : 0}
                getText={(protocol) => protocol ? protocol.titleRus : ''}
                onChange={(protocolId) => this.setState({
                    ...this.state,
                    protocolBotId: protocolId
                })} className='fixed-width-select-sm' />;
        }
    }

    private lineDescriptionsSelect(): React.ReactNode {
        if (this.state.protocolSetId) {
            const LSelect = LazySelect as new (props: LazySelectProps<IModel, number>) =>
                LazySelect<IModel, number>;

            return <LSelect
                mode='multiple'
                placeholder='Select LineDescriptions (multiple)'
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
        const opts = [0, 10, 100, 1000].map((num, i) => <Select.Option key={i} value={num}>&ge; {num}</Select.Option>);

        return (
            <div>
                <div className='col-sm-4'>
                    <Select onChange={this.changeMinValue} className='fixed-width-select-sm'>
                        {opts}
                    </Select>
                </div>
                <div className='col-sm-1'>
                    или &ge;
                </div>
                <div className='col-sm-2'>
                    <input type='number' className='form-control' onChange={this.changeCustomMinValue} />
                </div>
            </div>
        );
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
                <div>
                    <div className='col-sm-3'>
                        <h4 className='text-center'>По выборам в целом</h4>
                        {this.renderChiSquaredTable()}
                    </div>
                    <div className='col-sm-9'>
                        <h4 className='text-center'>По каждой отдельной цифре</h4>
                        {this.renderChart()}
                    </div>
                </div>
            );
        }
    }

    private renderChiSquaredTable(): React.ReactNode {
        return (
            <table className='table table-bordered table-condensed text-center'>
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
                return <tr key={i}>
                    <td className={`warn-level-${7 - i - 1}`}>
                        {stats[0]}%
                        <br />
                        <small>{stats[1] > 0 ? `(χ2>${stats[1]})` : ''}</small>
                    </td>
                    <td className={`warn-level-${warnLevel - 1}`}>{calcValue}</td>
                    <td className={`warn-level-${7 - i - 1}`}>{stats[2]}</td>
                </tr>;
            });
        }
    }

    private renderChart(): React.ReactNode {
        const options = {
            ...this.state.chartOptions,
            chart: { type: 'column' },
            title: { text: 'Частота последних цифр в выборке' },
            plotOptions: {
                column: {
                    borderColor: 'black',
                    borderWidth: 1
                },
                series: {
                    turboThreshold: 100000,
                }
            },
            xAxis: [{ categories: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], title: { text: 'Последняя цифра' } }]
        };

        return <HighchartComponent options={options} />;
    }

    private changeMinValue(value: SelectValue): void {
        this.setState({ ...this.state, minValue: parseInt(value.toString(), 10) });
    }

    private changeCustomMinValue(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, minValue: parseInt(e.currentTarget.value, 10) });
    }

    private getChartData(): void {
        this.setState({ ...this.state, isLoading: true });

        const buildParams = {
            protocolSetId: this.state.protocolSetId,
            protocolTopId: this.state.protocolTopId,
            protocolMidId: this.state.protocolMidId,
            protocolBotId: this.state.protocolBotId,
            lineDescriptionIds: this.state.lineDescriptionIds,
            minValue: this.state.minValue
        } as ILDAChartBuildParameters;
        ChartsController.Instance.getLastDigitAnalyzerData(buildParams)
            .then((result) =>
                this.setState({
                    ...this.state, isLoading: false, chartOptions: result.chartOptions,
                    chiSquared: result.chiSquared
                }));
    }
}
