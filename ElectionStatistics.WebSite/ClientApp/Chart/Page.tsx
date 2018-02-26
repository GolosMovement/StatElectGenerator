import * as React from 'react';

import { RouteComponentProps, Link } from 'react-router-dom';
import * as QueryString from 'query-string'

import Select, { Option } from 'react-select';

import { LazyItems } from '../Common';
import { HighchartComponent } from '../Highchart/Component';

import { ElectionDto, ElectoralDistrictDto, CandidateDto, DictionariesController } from './DictionariesController';
import { ChartsController } from './ChartsController';

interface ChartPageRouteProps {
    electionId?: number;
    candidateId?: number;
}

interface ChartPageProps extends RouteComponentProps<ChartPageRouteProps> {
}

interface ChartState {
    isLoading: boolean;
    electionId?: number;
    districtIds: number[];
    candidateId?: number;
    series?: Highcharts.IndividualSeriesOptions[];
}

interface ChartPageState {
    elections: LazyItems<ElectionDto>;
    districts: LazyItems<ElectoralDistrictDto>;
    candidates: LazyItems<CandidateDto>;
    chart: ChartState;
}

export abstract class ChartPage extends React.Component<ChartPageProps, ChartPageState> {
    constructor(props: ChartPageProps) {
        super(props);

        this.state = this.getStateWithRouteProps({
            elections: {
                isLoading: true,
                items: []
            },
            districts: {
                isLoading: false,
                items: []
            },
            candidates: {
                isLoading: false,
                items: []
            },
            chart: {                
                isLoading: false,
                districtIds: []
            }
        });
    }

    public componentWillMount() {                
        this.loadElections();
        this.tryLoadCandidates();
        this.tryLoadChartData();
    }

    public componentWillReceiveProps() {
        this.setState(
            this.getStateWithRouteProps(this.state)
        );
        this.tryLoadCandidates();
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

    private loadElections() {        
        DictionariesController.Instance.getElections()
            .then(elections => {
                this.setState({
                    ...this.state,
                    elections: {
                        isLoading: false,
                        items: elections
                    }
                });
            });
    }

    private handleElectionSelected = (selectedOption: any) => {
        this.setState(
            {
                ...this.state,
                chart: {
                    ...this.state.chart,
                    electionId: selectedOption.value
                }
            },
            this.tryLoadCandidates);        
    }

    private renderElectionsSelect() {
        if (this.state.elections.isLoading) {
            return (
                <Select
                    isLoading={true}
                />);
        }
        else {
            const options = this.state.elections.items
                .map(election => ({ value: election.id as number, label: election.name }));

            return (
                <Select
                    clearable={false}
                    onChange={this.handleElectionSelected}
                    value={this.state.chart.electionId}
                    options={options}
                />
            );
        }
    }

    private tryLoadDisticts() {
        if (this.state.chart.electionId != null) {
            this.setState({
                ...this.state,
                districts: {
                    isLoading: true,
                    items: []
                }
            });

            DictionariesController.Instance.getDistricts(this.state.chart.electionId)
                .then(districts => {
                    this.setState({
                        ...this.state,
                        districts: {
                            isLoading: false,
                            items: districts
                        }
                    });
                });
        }
    }

    private handleDistrictSelected = (selectedOption: any) => {
        if (selectedOption == null) {
            this.state.chart.districtIds.pop();
        }
        else {
            this.state.chart.districtIds.push(selectedOption.value);
        }
        this.setState({
            ...this.state
        });
    }

    private renderDistrictsSelects() {
        if (this.state.chart.electionId == null) {
            return null;
        }
        else if (this.state.districts.isLoading) {
            return (
                <Select
                    isLoading={true}
                />);
        }
        else {
            const options = this.state.districts.items
                .map(candidate => ({ value: candidate.id as number, label: candidate.name }));

            return (
                <Select
                    onChange={this.handleDistrictSelected}
                    value={this.state.chart.candidateId}
                    options={options}
                />
            );
        }
    }

    private tryLoadCandidates() {
        if (this.state.chart.electionId != null) {
            this.setState({
                ...this.state,
                candidates: {
                    isLoading: true,
                    items: []
                }
            });

            DictionariesController.Instance.getCandidates(this.state.chart.electionId)
                .then(candidates => {
                    this.setState({
                        ...this.state,
                        candidates: {
                            isLoading: false,
                            items: candidates
                        }
                    });
                });
        }
    }

    private handleCandidateSelected = (selectedOption: any) => {
        this.setState({
            ...this.state,
            chart: {
                ...this.state.chart,
                candidateId: selectedOption.value
            }
        });
    }

    private renderCandidatesSelect() {
        if (this.state.chart.electionId == null) {
            return null;
        }
        else if (this.state.candidates.isLoading) {
            return (
                <Select
                    isLoading={true}
                />);
        }
        else {
            const options = this.state.candidates.items
                .map(candidate => ({ value: candidate.id as number, label: candidate.name }));

            return (
                <Select
                    clearable={false}
                    onChange={this.handleCandidateSelected}
                    value={this.state.chart.candidateId}
                    options={options}
                />
            );
        }
    }

    private renderButton() {
        let className = "btn btn-primary"
        let disabled = false;
        if (this.state.chart.electionId == null) {
            className += "btn btn-primary disabled";
            disabled = true;
        }

        const queryParams: ChartPageRouteProps = {
            electionId: this.state.chart.electionId,
            candidateId: this.state.chart.candidateId
        }

        return (
            <Link 
                className={className}
                disabled={disabled}
                to={{ search: QueryString.stringify(queryParams)}}>
                Построить
            </Link>
        );
    }

    protected abstract loadChartData() : Promise<Highcharts.IndividualSeriesOptions[]>;

    private tryLoadChartData() {
        if (this.state.chart.electionId != null &&
            this.state.chart.candidateId != null) {
            this.setState({
                ...this.state,
                chart: {
                    ...this.state.chart,
                    isLoading: true
                }
            });
            
            this.loadChartData()
                .then(series => {
                    this.setState({
                        ...this.state,
                        chart: {
                            ...this.state.chart,
                            isLoading: false,
                            series: series
                        }
                    });
                });
        }
    }

    private tryRenderChart() {
        if (this.state.chart.isLoading) {
            return (
                <div>
                    <img 
                        src="preloader.svg" 
                        height="200px"
                        width="200px" />
                </div>
            );
        }
        else if (this.state.chart.series != null) {
            return this.renderChart();
        }
        else {
            return null;
        }
    }

    protected abstract renderChart() : JSX.Element;

    private getStateWithRouteProps(state: ChartPageState): ChartPageState {
        const routeProps = QueryString.parse(this.props.location.search) as ChartPageRouteProps;
        
        return {
            ...state,
            chart: {
                ...state.chart,
                electionId: routeProps.electionId,
                candidateId: routeProps.candidateId
            }
        };
    }
}

export class LinePage extends ChartPage {    
    protected loadChartData(): Promise<Highcharts.IndividualSeriesOptions[]> {
        return ChartsController.Instance.getHistogramData({
            electionId: this.state.chart.electionId as number,
            candidateId: this.state.chart.candidateId as number,
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
                series={this.state.chart.series}                
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

export class ScatterplotPage extends ChartPage {
    protected loadChartData(): Promise<Highcharts.IndividualSeriesOptions[]> {
        return ChartsController.Instance.getScatterplotData({
            electionId: this.state.chart.electionId as number,
            candidateId: this.state.chart.candidateId as number
        });
    }

    protected renderChart(): JSX.Element {
        return <HighchartComponent 
                title={{ text: '' }}
                chart={{ type: 'scatter' }}
                yAxis={{
                    title: {
                        text: 'Количество избирателей зарегистрированных на участках'
                    }
                }}
                series={this.state.chart.series}
            />;
    }
}