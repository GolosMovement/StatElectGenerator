import * as React from 'react';

import { RouteComponentProps, Link } from 'react-router-dom';
import * as QueryString from 'query-string'

import Select, { Option } from 'react-select';

import { LazyItems } from '../Common';
import { HighchartComponent } from '../Highchart/Component';

import { ElectionDto, CandidateDto, DictionariesController } from './DictionariesController';
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
    candidateId?: number;
    series?: Highcharts.IndividualSeriesOptions[];
}

interface ChartPageState {
    elections: LazyItems<ElectionDto>;
    candidates: LazyItems<CandidateDto>;
    chart: ChartState;
}

class NumberSelect extends Select<number> {
}

export class ChartPage extends React.Component<ChartPageProps, ChartPageState> {
    constructor(props: ChartPageProps) {
        super(props);

        this.state = this.getStateWithRouteProps({
            elections: {
                isLoading: true,
                items: []
            },
            candidates: {
                isLoading: false,
                items: []
            },
            chart: {                
                isLoading: false
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
                    {this.renderChart()}
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
                    name="form-field-name"
                    isLoading={true}
                />);
        }
        else {
            const options = this.state.elections.items
                .map(election => ({ value: election.id as number, label: election.name }));

            return (
                <Select
                    name="form-field-name"
                    onChange={this.handleElectionSelected}
                    value={this.state.chart.electionId}
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

            DictionariesController.Instance.getCandidatesByElection(this.state.chart.electionId)
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
                    name="form-field-name"
                    isLoading={true}
                />);
        }
        else {
            const options = this.state.candidates.items
                .map(candidate => ({ value: candidate.id as number, label: candidate.name }));

            return (
                <Select
                    name="form-field-name"
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
                Построить гистограмму
            </Link>
        );
    }

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
            
            ChartsController.Instance.getHistogramData({
                    electionId: this.state.chart.electionId,
                    candidateId: this.state.chart.candidateId,
                    stepSize: 1
                })
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

    private renderChart() {
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
            return <HighchartComponent 
                title={{ text: '' }}
                chart={{ type: 'column' }}
                yAxis={{
                    title: {
                        text: 'Количество избирателей зарегистрированных на участках'
                    }
                }}
                series={this.state.chart.series}
            />;
        }
        else {
            return null;
        }
    }

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
