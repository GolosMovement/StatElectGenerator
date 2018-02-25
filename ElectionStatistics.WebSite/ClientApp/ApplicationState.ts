import { ElectionsState, electionsReducer, electionsInitialState } from './Elections/State';
import { ChartState, chartReducer, chartInitialState } from './Chart/State';

// The top-level state object
export interface ApplicationState {
    elections: ElectionsState;
    chart: ChartState;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    elections: electionsReducer,
    chart: chartReducer
};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}

export interface LazyItems<TItem> {
    isLoaded: boolean;
    items: TItem[];
}

export const applicationInitialState: ApplicationState = {
    elections: electionsInitialState,
    chart: chartInitialState
}
