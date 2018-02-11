import { fetch, addTask } from 'domain-task';
import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction, LazyItems } from '../ApplicationState';

export interface ElectionsState extends LazyItems<Election> {
}

export interface Election {
    id: number;
    name: string;
}

interface RequestElectionsAction {
    type: 'REQUEST_ELECTIONS';
}

interface ReceiveElectionsAction {
    type: 'RECEIVE_ELECTIONS';
    elections: Election[];
}

type KnownAction = RequestElectionsAction | ReceiveElectionsAction;

export const electionsActionCreators = {
    requestElections: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        const state = getState();
        if (!state.elections || !state.elections.isLoaded) {
            let fetchTask = fetch(`api/elections`)
                .then(response => response.json() as Promise<Election[]>)
                .then(data => {
                    dispatch({ type: 'RECEIVE_ELECTIONS', elections: data });
                });

            addTask(fetchTask); // Ensure server-side prerendering waits for this to complete
            dispatch({ type: 'REQUEST_ELECTIONS' });
        }
    }
};

export const electionsInitialState: ElectionsState = { items: [], isLoaded: false };

export const electionsReducer: Reducer<ElectionsState> = (state: ElectionsState, incomingAction: Action) => {
    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_ELECTIONS':
            return {
                ...state,
                isLoaded: false
            };
        case 'RECEIVE_ELECTIONS':
            return {
                items: action.elections,
                isLoaded: true
            };
        default:
            // The following line guarantees that every action in the KnownAction union has been covered by a case above
            const exhaustiveCheck: never = action;
    }

    return state || electionsInitialState;
};
