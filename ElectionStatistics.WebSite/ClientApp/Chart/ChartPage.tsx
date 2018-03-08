import * as React from 'react';

import { RouteComponentProps, Link } from 'react-router-dom';

import { LazyItems } from '../Common';
import { HighchartComponent } from '../Highchart/Component';

import { ElectionDto, ElectoralDistrictDto, NamedChartParameter, DictionariesController, ChartParameter } from './DictionariesController';
import { ChartsController, ChartBuildParameters } from './ChartsController';
import { SelectValue } from 'antd/lib/select';
import { LazySelect, LazySelectProps, LazyTreeSelect, LazyTreeSelectProps, QueryString } from '../Common';
import { Spin } from 'antd';

interface QueryStringChartParameter {
    type: string;
}

const toQueryStringParameter = (parameter: ChartParameter) => {
    if (parameter == null) {
        return undefined;
    }

    const queryStringParameter = {
        type: parameter.$type,        
        ...parameter
    };
    delete queryStringParameter.$type;
    return queryStringParameter as QueryStringChartParameter;
}

const fromQueryStringParameter = (queryStringParameter?: QueryStringChartParameter) => {
    if (queryStringParameter == null) {
        return null;
    }

    const parameter = {
        $type: queryStringParameter.type,
        ...queryStringParameter
    };
    delete parameter.type;
    return parameter as ChartParameter;
}

interface ChartPageRouteProps {
    electionId?: number;
    districtId?: number;
    x?: QueryStringChartParameter;
    y?: QueryStringChartParameter;
}

interface ChartPageProps extends RouteComponentProps<ChartPageRouteProps> {
}

interface ChartPageState {
    isLoading: boolean;
    electionId: number | null;
    districtId: number | null;
    x: ChartParameter | null;
    y: ChartParameter | null;
    chartOptions?: Highcharts.Options;
}

export abstract class ChartPage extends React.Component<ChartPageProps, ChartPageState> {
    constructor(props: ChartPageProps) {
        super(props);

        this.state = this.getStateFromRouteProps();
    }

    private getStateFromRouteProps(): ChartPageState {
        const routeProps = QueryString.parse(this.props.location.search) as ChartPageRouteProps;

        return {
            isLoading: false,
            electionId: routeProps.electionId || null,
            districtId: routeProps.districtId || null,
            x: fromQueryStringParameter(routeProps.x),
            y: fromQueryStringParameter(routeProps.y)
        };
    }

    public componentWillMount() {        
        this.tryLoadChartData();
    }

    public componentWillReceiveProps() {
        this.setState(this.getStateFromRouteProps());
        this.tryLoadChartData();
    }

    public render() {
        return (
            <div className="container-fluid">
                <div className="row">
                    <div className="col-md-3">
                        {this.renderElectionsSelect()}
                    </div>
                </div>
                <div className="row">
                    <div className="col-md-3">
                        {this.renderDistrictsSelect()}
                    </div>
                </div>
                <div className="row">
                    <div className="col-md-3">
                        {this.renderParametersSelect(
                            "Выберите параметр для оси X",
                            this.state.x,
                            this.getXParameters,
                            this.onXChange
                        )}
                    </div>
                </div>
                <div className="row">
                    <div className="col-md-3">
                        {this.renderParametersSelect(
                            "Выберите параметр для оси Y",
                            this.state.y,
                            this.getYParameters,
                            this.onYChange
                        )}
                    </div>
                </div>
                { this.renderAdditionalParameterSelectors() }
                <div className="row">
                    <div className="col-md-3">
                        {this.renderButton()}
                    </div>
                </div>
                <div className="row">
                    {this.tryRenderChart()}
                </div>
            </div>
        );
    }

    private renderElectionsSelect() {
        const Select = LazySelect as new (props: LazySelectProps<ElectionDto, number>) => LazySelect<ElectionDto, number>;

        return <Select
            placeholder="Укажите выборы"
            itemsPromise={DictionariesController.Instance.getElections()}
            selectedValue={this.state.electionId}
            getValue={election => election.id} 
            getText={election => election.name}
            onChange={electionId => this.setState({
                ...this.state,
                electionId: electionId
            })}/>
    }

    private renderDistrictsSelect() {
        if (this.state.electionId == null) {
            return null;
        }
        else {
            const TreeSelect = LazyTreeSelect as new (props: LazyTreeSelectProps<ElectoralDistrictDto, number>) => LazyTreeSelect<ElectoralDistrictDto, number>;

            return <TreeSelect
                allowClear
                placeholder="Все округа"
                itemsPromise={DictionariesController.Instance.getDistricts(this.state.electionId)}
                selectedValue={this.state.districtId}
                getValue={district => district.id} 
                getText={district => district.name}
                getChildren={district => district.lowerDitsrticts}
                onChange={districtId => this.setState({
                    ...this.state,
                    districtId: districtId
                })}/>
        }
    }

    private onXChange = (newX: ChartParameter | null) => {
        this.setState({
            ...this.state,
            x: newX
        });
    }

    protected abstract getXParameters(electionId: number) : Promise<NamedChartParameter[]>;

    private onYChange = (newY: ChartParameter | null) => {
        this.setState({
            ...this.state,
            y: newY
        });
    }

    protected abstract getYParameters(electionId: number) : Promise<NamedChartParameter[]>;

    private renderParametersSelect(
        placeholder: string,
        selectedParameter: ChartParameter | null,
        getParametersPromise: (electionId: number) => Promise<NamedChartParameter[]>,
        onChange: (newSelectedParameter: ChartParameter | null) => void) {
        if (this.state.electionId == null) {
            return null;
        }
        else {
            const Select = LazySelect as new (props: LazySelectProps<NamedChartParameter, ChartParameter>) => LazySelect<NamedChartParameter, ChartParameter>;

            return <Select
                placeholder={placeholder}
                itemsPromise={getParametersPromise(this.state.electionId)}
                selectedValue={selectedParameter}
                getValue={namedParameter => namedParameter.parameter} 
                getText={namedParameter => namedParameter.name}
                onChange={onChange} 
            />
        }
    }

    protected renderAdditionalParameterSelectors(): JSX.Element[] {
        return [];
    }

    private renderButton() {
        if (this.state.electionId === null ||
            this.state.x === null ||
            this.state.y === null) {
            return null;
        }
        else {
            const queryParams: ChartPageRouteProps = {
                electionId: this.state.electionId,
                districtId: this.state.districtId || undefined,
                x: toQueryStringParameter(this.state.x),
                y: toQueryStringParameter(this.state.y)
            }
    
            return (
                <Link 
                    className="btn btn-primary"               
                    to={{ search: QueryString.stringify(queryParams)}}>
                    Построить
                </Link>
            );
        }
    }

    protected abstract getChartData(parameters: ChartBuildParameters) : Promise<Highcharts.Options>;

    private tryLoadChartData() {
        if (this.state.electionId !== null &&
            this.state.x !== null &&
            this.state.y !== null) {
            this.setState({
                ...this.state,
                isLoading: true
            });
            
            this.getChartData({
                    electionId: this.state.electionId,
                    districtId: this.state.districtId,
                    x: this.state.x,
                    y: this.state.y,
                })
                .then(series => {
                    this.setState({
                        ...this.state,
                        isLoading: false,
                        chartOptions: series
                    });
                });
        }
    }

    private tryRenderChart() {
        if (this.state.isLoading) {
            return <Spin size="large" />;
        }
        else if (this.state.chartOptions != null) {
            return this.renderChart(this.state.chartOptions);
        }
        else {
            return null;
        }
    }

    protected abstract renderChart(optionsFromBackend: Highcharts.Options) : JSX.Element;
}