import { Action, Reducer, ActionCreator } from 'redux';
import { LOCATION_CHANGE, LocationChangeAction } from 'react-router-redux'
import * as QueryString from 'query-string'

import { AppThunkAction, LazyItems } from '../ApplicationState';
import { ElectionsState, electionsActionCreators, Election } from '../Elections/State';

export interface ChartState {
    selectedElectionId?: number;
    regionId?: number;
    showChart?: boolean;
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

type KnownAction = SelectElectionAction | SelectRegionAction;

export const chartActionCreators = {
    ...electionsActionCreators,
    selectElection: (electionId: number) => <SelectElectionAction>{ type: "SELECT_ELECTION", electionId: electionId },
    selectRegion: (regionId: number) => <SelectRegionAction>{ type: "SELECT_REGION", regionId: regionId },
};

export const chartInitialState: ChartState = { };

export const chartReducer: Reducer<ChartState> = (state: ChartState, incomingAction: Action) => {
    if (incomingAction.type == LOCATION_CHANGE)    {
        const locationChangeAction = incomingAction as LocationChangeAction;
        const routeProps = QueryString.parse(locationChangeAction.payload.search) as ChartPageRouteProps;
        return {
            ...state,
            selectedElectionId: routeProps.electionId,
            showChart: routeProps.showChart
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
        default:
            const exhaustiveCheck: never = action;
    }

    return state || chartInitialState;
};
