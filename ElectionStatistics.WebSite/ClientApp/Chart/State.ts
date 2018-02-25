import { Action, Reducer, ActionCreator } from 'redux';
import { LOCATION_CHANGE, LocationChangeAction } from 'react-router-redux'
import * as QueryString from 'query-string'
import * as Highcharts from 'highcharts';

import { AppThunkAction, LazyItems } from '../ApplicationState';
import { ElectionsState, electionsActionCreators, Election } from '../Elections/State';
import { addTask } from 'domain-task';

export interface ChartState {
    selectedElectionId?: number;
    regionId?: number;
    showChart?: boolean;
    isDataLoaded?: boolean;
    series?: Highcharts.IndividualSeriesOptions[];
}

export interface ChartBuildParameters {
    electionId: number;
    candidateId: number;
    stepSize: number;
}

export interface ChartPageRouteProps {
    electionId?: number;
    showChart?: boolean;
}

interface SelectElectionAction {
    type: "SELECT_ELECTION";
    electionId: number;
}

interface SelectRegionAction {
    type: "SELECT_REGION";
    regionId: number;
}

interface RecieveDataForHistogramAction {
    type: "RECEIVE_DATA_FOR_HISTOGRAM";
    series: Highcharts.IndividualSeriesOptions[];
}

type KnownAction = SelectElectionAction | SelectRegionAction | RecieveDataForHistogramAction;

export const chartActionCreators = {
    ...electionsActionCreators,
    selectElection: (electionId: number) => <SelectElectionAction>{ type: "SELECT_ELECTION", electionId: electionId },
    selectRegion: (regionId: number) => <SelectRegionAction>{ type: "SELECT_REGION", regionId: regionId },
    requestChartData: (buildParameters: ChartBuildParameters): AppThunkAction<KnownAction> => (dispatch, getState) => {
        const state = getState();
        if (!state.chart.isDataLoaded) {
            let fetchTask = fetch(`api/charts/histogram?${QueryString.stringify(buildParameters)}`)
                .then(response => response.json() as Promise<Highcharts.IndividualSeriesOptions[]>)
                .then(data => {
                    dispatch({ type: 'RECEIVE_DATA_FOR_HISTOGRAM', series: data });
                });

            addTask(fetchTask); // Ensure server-side prerendering waits for this to complete
        }
    }
};

export const chartInitialState: ChartState = { };

export const chartReducer: Reducer<ChartState> = (state: ChartState, incomingAction: Action) => {
    if (incomingAction.type == LOCATION_CHANGE)    {
        const locationChangeAction = incomingAction as LocationChangeAction;
        const routeProps = QueryString.parse(locationChangeAction.payload.search) as ChartPageRouteProps;

        return {
            ...state,
            selectedElectionId: routeProps.electionId,
            showChart: routeProps.showChart,
            isDataLoaded: false
        };
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case "SELECT_ELECTION":
            return {
                ...state,
                selectedElectionId: action.electionId
            };
        case "SELECT_REGION":
            return {
                ...state,
                selectedElectionId: action.regionId
            };
        case "RECEIVE_DATA_FOR_HISTOGRAM":
            return {
                ...state,
                isDataLoaded: true,
                series: action.series
            };
        default:
            const exhaustiveCheck: never = action;
    }

    return state || chartInitialState;
};
