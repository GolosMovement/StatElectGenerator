import * as React from 'react';
import { connect } from 'react-redux';

import { RouteComponentProps, Link } from 'react-router-dom';
import * as QueryString from 'query-string'
import Select, { Option } from 'react-select';
import { HighchartComponent } from '../Highchart/Component';

import { ChartsState, chartsActionCreators, ChartsPageRouteProps } from './State';
import { ElectionsState } from '../Elections/State';
import { ApplicationState } from '../ApplicationState';

interface ChartsPageState {
    elections: ElectionsState,
    charts: ChartsState
}

// At runtime, Redux will merge together...
type ChartsPageProps = 
    ChartsPageState & 
    typeof chartsActionCreators &
    RouteComponentProps<ChartsPageRouteProps>

class ChartsPagePresenter extends React.Component<ChartsPageProps, {}> {
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
                    value={this.props.charts.selectedElectionId}
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
        if (this.props.charts.selectedElectionId == null) {
            className += "btn btn-primary disabled";
            disabled = true;
        }

        const queryParams: ChartsPageRouteProps = {
            electionId: this.props.charts.selectedElectionId,
            showChart: true
        }

        return (
            <Link 
                className={className}
                disabled={disabled}
                to={{ pathname: "/charts", search: QueryString.stringify(queryParams)}}>
                Построить график
            </Link>
        );
    }

    private renderChart() {
        if (this.props.charts.showChart) {
            return <HighchartComponent />;
        }
        else {
            return null;
        }
    }
}
export const ChartsPage = connect(
    (state: ApplicationState) => state as ChartsPageState,
    chartsActionCreators
)(ChartsPagePresenter) as typeof ChartsPagePresenter;
