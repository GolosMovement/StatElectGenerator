import * as React from 'react';
import { connect } from 'react-redux';

import { RouteComponentProps, Link } from 'react-router-dom';
import * as QueryString from 'query-string'
import Select, { Option } from 'react-select';
import { HighchartComponent } from '../Highchart/Component';

import { ChartState, chartActionCreators, ChartPageRouteProps } from './State';
import { ElectionsState } from '../Elections/State';
import { ApplicationState } from '../ApplicationState';

interface ChartPageState {
    elections: ElectionsState,
    chart: ChartState
}

// At runtime, Redux will merge together...
type ChartsPageProps = 
    ChartPageState & 
    typeof chartActionCreators &
    RouteComponentProps<ChartPageRouteProps>

class ChartPagePresenter extends React.Component<ChartsPageProps, {}> {
    public componentWillMount() {
        this.props.requestElections();
    }

    public render() {
        return (
            <div>
                {this.renderSelect()}
                {this.renderButton()}
                {this.renderChart()}
            </div>
        );
    }

    private renderSelect() {
        if (!this.props.elections || !this.props.elections.isLoaded) {
            return (
                <Select
                    name="form-field-name"
                    isLoading={true}
                />);
        }
        else {
            const options = this.props.elections.items
                .map(election => ({ value: election.id as number, label: election.name }));

            return (
                <Select
                    name="form-field-name"
                    onChange={this.handleChange}
                    value={this.props.chart.selectedElectionId}
                    options={options}
                />
            );
        }
    }

    private handleChange = (selectedOption: any) => {
        this.props.selectElection(selectedOption.value)
    }

    private renderButton() {
        let className = "btn btn-primary"
        let disabled = false;
        if (this.props.chart.selectedElectionId == null) {
            className += "btn btn-primary disabled";
            disabled = true;
        }

        const queryParams: ChartPageRouteProps = {
            electionId: this.props.chart.selectedElectionId,
            showChart: true
        }

        return (
            <Link 
                className={className}
                disabled={disabled}
                to={{ pathname: "/histogram", search: QueryString.stringify(queryParams)}}>
                Построить гистограмму
            </Link>
        );
    }

    private renderChart() {
        if (this.props.chart.showChart) {
            return <HighchartComponent />;
        }
        else {
            return null;
        }
    }
}
export const ChartPage = connect(
    (state: ApplicationState) => state as ChartPageState,
    chartActionCreators
)(ChartPagePresenter) as typeof ChartPagePresenter;
