import React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';

import { Spin } from 'antd';

import { IProtocolSet } from 'ClientApp/Import';
import { LazySelect, ILazySelectProps, LazyTreeSelect, LazyTreeSelectProps, QueryString } from '../Common';
import { IChartBuildParameters } from './ChartsController';
import { DictionariesController } from './DictionariesController';
import { IProtocol } from './LastDigitAnalyzer';
import { IPreset } from 'ClientApp/Presets';

interface IChartPageRouteProps {
    protocolSetId?: number;
    protocolId?: number;
}

export interface IScatterplotChartPageRouteProps extends IChartPageRouteProps {
    x?: number;
    y?: number;
}

interface IChartPageProps extends RouteComponentProps<IScatterplotChartPageRouteProps> {}

interface IChartPageState {
    isLoading: boolean;
    protocolSetId: number | null;
    protocolId: number | null;
    xId: number | null;
    yId: number | null;
    chartOptions?: Highcharts.Options;
}

export abstract class ChartPage extends React.Component<IChartPageProps, IChartPageState> {
    constructor(props: IChartPageProps) {
        super(props);

        this.state = this.getStateFromRouteProps();
    }

    public componentWillMount() {
        this.tryLoadChartData();
    }

    public componentWillReceiveProps() {
        this.setState(this.getStateFromRouteProps());
        this.tryLoadChartData();
    }

    public render(): React.ReactNode {
        return (
            <div className='container-fluid'>
                <div className='row'>
                    <div className='col-md-3'>
                        {this.protocolSetSelect()}
                    </div>
                </div>
                <div className='row'>
                    <div className='col-md-3'>
                        {this.protocolSelect()}
                    </div>
                </div>
                {this.renderAdditionalParameterSelectors()}
                <div className='row'>
                    <div className='col-md-3'>
                        {this.renderButton()}
                    </div>
                </div>
                <div className='row'>
                    {this.tryRenderChart()}
                </div>
            </div>
        );
    }

    protected onXChange = (newX: number | null): void => {
        this.setState({
            ...this.state,
            xId: newX
        });
    }

    protected onYChange = (newY: number | null): void => {
        this.setState({
            ...this.state,
            yId: newY
        });
    }

    protected presetSelect(
        placeholder: string,
        selectedParameter: number | null,
        getParametersPromise: (protocolSetId: number) => Promise<IPreset[]>,
        onChange: (newSelectedParameter: number | null) => void): React.ReactNode {

        if (this.state.protocolSetId) {
            const LSelect = LazySelect as new (props: ILazySelectProps<IPreset, number>) =>
                LazySelect<IPreset, number>;

            return <LSelect
                placeholder={placeholder}
                itemsPromise={getParametersPromise(this.state.protocolSetId)}
                selectedValue={selectedParameter}
                getValue={(line) => line.id}
                getText={(line) => line.titleRus}
                onChange={onChange}
                className='fixed-width-select-sm' />;
        }
    }

    protected renderAdditionalParameterSelectors(): React.ReactNode[] {
        return [];
    }

    protected renderButton(): React.ReactNode {
        if (this.state.protocolSetId === null ||
            this.state.xId === null ||
            this.state.yId === null) {
            return null;
        } else {
            const queryParams: IScatterplotChartPageRouteProps = {
                protocolSetId: this.state.protocolSetId,
                protocolId: this.state.protocolId || undefined,
                x: this.state.xId,
                y: this.state.yId
            };

            return (
                <Link
                    className='btn btn-primary'
                    to={{ search: QueryString.stringify(queryParams) }}>
                    Построить
                </Link>
            );
        }
    }

    protected tryLoadChartData(): void {
        if (this.state.protocolSetId !== null &&
            this.state.xId !== null &&
            this.state.yId !== null) {
            this.setState({
                ...this.state,
                isLoading: true
            });

            this.getChartData({
                protocolSetId: this.state.protocolSetId,
                protocolId: this.state.protocolId,
                x: this.state.xId,
                y: this.state.yId,
            }).then((chartOptions) => {
                this.setState({
                    ...this.state,
                    isLoading: false,
                    chartOptions: chartOptions
                });
            }).catch(() => alert('Ошибка'));
        }
    }

    protected abstract getChartData(parameters: IChartBuildParameters): Promise<Highcharts.Options>;
    protected abstract renderChart(optionsFromBackend: Highcharts.Options): React.ReactNode;

    private getStateFromRouteProps(): IChartPageState {
        const routeProps = QueryString.parse(this.props.location.search) as IScatterplotChartPageRouteProps;

        return {
            isLoading: false,
            protocolSetId: routeProps.protocolSetId || null,
            protocolId: routeProps.protocolId || null,
            xId: routeProps.x || null,
            yId: routeProps.y || null
        };
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
            onChange={(protocolSet) => this.setState({
                ...this.state, protocolId: null, protocolSetId: protocolSet, xId: null, yId: null
            })} />;
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

    private tryRenderChart(): React.ReactNode {
        if (this.state.isLoading) {
            return <Spin size='large' />;
        } else if (this.state.chartOptions != null) {
            return this.renderChart(this.state.chartOptions);
        }
    }
}
