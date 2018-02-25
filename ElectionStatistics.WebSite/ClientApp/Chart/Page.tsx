import * as React from 'react';

import { RouteComponentProps, Link } from 'react-router-dom';
import * as QueryString from 'query-string'

import Select, { Option } from 'react-select';

import { LazyItems } from '../Common';
import { HighchartComponent } from '../Highchart/Component';

import { Election, ElectionsController } from './ElectionsController';
import { ChartsController } from './ChartsController';

interface ChartPageRouteProps {
    electionId?: number;
}

interface ChartPageProps extends RouteComponentProps<ChartPageRouteProps> {
}

interface ChartState {
    isLoading: boolean;
    electionId?: number;
    regionId?: number;
    series?: Highcharts.IndividualSeriesOptions[];
}

interface ChartPageState {
    elections: LazyItems<Election>;
    chart: ChartState;
}

export class ChartPage extends React.Component<ChartPageProps, ChartPageState> {
    constructor(props: ChartPageProps) {
        super(props);

        const routeProps = QueryString.parse(props.location.search) as ChartPageRouteProps;

        this.state = {
            elections: {
                isLoading: true,
                items: []
            },
            chart: {                
                isLoading: false,
                electionId: routeProps.electionId
            }
        };
                
        this.loadElections();
    }

    public componentWillMount() {
        this.loadChartDataIfRequired();
    }

    public componentWillReceiveProps() {
        const routeProps = QueryString.parse(this.props.location.search) as ChartPageRouteProps;

        this.setState({
            ...this.state,
            chart: {
                ...this.state.chart,
                electionId: routeProps.electionId
            }
        });
        this.loadChartDataIfRequired();
    }

    public render() {
        return (
            <div className="container-fluid">
                <div className="row">
                    <div className="col-md-3">
                        {this.renderSelect()}
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

    private renderSelect() {
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

    private handleElectionSelected = (selectedOption: any) => {
        this.setState({
            ...this.state,
            chart: {
                ...this.state.chart,
                isLoading: false,
                electionId: selectedOption.value
            }
        });
    }

    private renderButton() {
        let className = "btn btn-primary"
        let disabled = false;
        if (this.state.chart.electionId == null) {
            className += "btn btn-primary disabled";
            disabled = true;
        }

        const queryParams: ChartPageRouteProps = {
            electionId: this.state.chart.electionId
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

    private loadElections() {        
        ElectionsController.Instance.getElections()
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

    private loadChartDataIfRequired() {
        if (this.state.chart.electionId != null) {
            this.setState({
                ...this.state,
                chart: {
                    ...this.state.chart,
                    isLoading: true
                }
            });
            
            ChartsController.Instance.getHistogramData({
                    electionId: this.state.chart.electionId as number,
                    candidateId: 20,
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
}
