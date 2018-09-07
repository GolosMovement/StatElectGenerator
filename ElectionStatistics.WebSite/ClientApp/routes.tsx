import React from 'react';
import { Route } from 'react-router-dom';

import * as Admin from './Admin';
import * as Chart from './Chart';
import * as Import from './Import';
import * as Presets from './Presets';

import { Layout } from './Layout/Component';

export const routes = <Layout>
    <Route exact path='/histogram' component={ Chart.HistogramPage } />
    <Route exact path='/location-scatterplot' component={ Chart.LocationScatterplotPage } />
    <Route exact path='/scatterplot' component={ Chart.ScatterplotPage } />
    <Route exact path='/last-digit-analyzer' component={ Chart.LastDigitAnalyzer } />

    <Route exact path='/signIn' component={ Admin.AuthPage } />
    <Route exact path='/admin' component={ Admin.AdminPage } />

    <Route exact path='/import' component={ Import.ImportPage } />
    <Route exact path='/import/protocolSets/new' component={ Import.NewProtocolSet } />
    <Route exact path='/import/protocolSets' component={ Import.ProtocolSets } />
    <Route path='/import/protocolSets/:id/edit' component={ Import.EditProtocolSet } />

    <Route exact path='/presets' component={ Presets.PresetList } />
    <Route exact path='/presets/new' component={ Presets.NewPreset } />
    <Route path='/presets/:id/edit' component={ Presets.EditPreset } />
</Layout>;
