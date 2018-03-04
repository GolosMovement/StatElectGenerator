import * as React from 'react';

import { RouteComponentProps, Link } from 'react-router-dom';
import * as QueryString from 'query-string'

import { LazyItems } from '../Common';
import { HighchartComponent } from '../Highchart/Component';

import { ElectionDto, ElectoralDistrictDto, CandidateDto, DictionariesController } from './DictionariesController';
import { ChartsController, ChartBuildParameters } from './ChartsController';
import { SelectValue } from 'antd/lib/select';
import { LazySelect, LazySelectProps, LazyTreeSelect, LazyTreeSelectProps } from '../Common';
import { Spin } from 'antd';

interface ChartPageRouteProps {
    electionId?: number;
    candidateId?: number;
    districtId?: number;
}

interface ChartPageProps extends RouteComponentProps<ChartPageRouteProps> {
}

interface ChartPageState {
    isLoading: boolean;
    electionId: number | null;
    districtId: number | null;
    candidateId: number | null;
    series?: Highcharts.IndividualSeriesOptions[];
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
            candidateId: routeProps.candidateId || null
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
                        {this.renderCandidatesSelect()}
                    </div>
                </div>
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
    
    protected abstract getDistricts(electionId: number) : Promise<ElectoralDistrictDto[]>;

    private renderDistrictsSelect() {
        if (this.state.electionId == null) {
            return null;
        }
        else {
            const TreeSelect = LazyTreeSelect as new (props: LazyTreeSelectProps<ElectoralDistrictDto, number>) => LazyTreeSelect<ElectoralDistrictDto, number>;

            return <TreeSelect
                allowClear
                placeholder="Все округа"
                itemsPromise={this.getDistricts(this.state.electionId)}
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

    private renderCandidatesSelect() {
        if (this.state.electionId == null) {
            return null;
        }
        else {
            const Select = LazySelect as new (props: LazySelectProps<CandidateDto, number>) => LazySelect<CandidateDto, number>;

            return <Select
                placeholder="Выберите кандидата"
                itemsPromise={DictionariesController.Instance.getCandidates(this.state.electionId)}
                selectedValue={this.state.candidateId}
                getValue={candidate => candidate.id} 
                getText={candidate => candidate.name}
                onChange={candidateId => this.setState({
                    ...this.state,
                    candidateId: candidateId
                })}/>
        }
    }

    private renderButton() {
        if (this.state.electionId === null ||
            this.state.candidateId === null) {
            return null;
        }
        else {
            const queryParams: ChartPageRouteProps = {
                electionId: this.state.electionId,
                districtId: this.state.districtId || undefined,
                candidateId: this.state.candidateId
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

    protected abstract getChartData(parameters: ChartBuildParameters) : Promise<Highcharts.IndividualSeriesOptions[]>;

    private tryLoadChartData() {
        if (this.state.electionId !== null &&
            this.state.candidateId !== null) {
            this.setState({
                ...this.state,
                isLoading: true
            });
            
            this.getChartData({
                    electionId: this.state.electionId,
                    districtId: this.state.districtId,
                    candidateId: this.state.candidateId
                })
                .then(series => {
                    this.setState({
                        ...this.state,
                        isLoading: false,
                        series: series
                    });
                });
        }
    }

    private tryRenderChart() {
        if (this.state.isLoading) {
            return <Spin size="large" />;
        }
        else if (this.state.series != null) {
            return this.renderChart();
        }
        else {
            return null;
        }
    }

    protected abstract renderChart() : JSX.Element;
}