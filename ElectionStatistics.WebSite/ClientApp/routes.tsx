import * as React from 'react';
import { Route } from 'react-router-dom';

import * as Chart from './Chart';
import * as Import from './Import';
import { EditDataset } from './Import/EditDataset';
import { NewDataset } from './Import/NewDataset';
import { ProtocolSets } from './Import/ProtocolSets';
import { Layout } from './Layout/Component';

export const routes = <Layout>
    <Route exact path='/histogram' component={ Chart.HistogramPage } />
    <Route exact path='/location-scatterplot' component={ Chart.LocationScatterplotPage } />
    <Route exact path='/scatterplot' component={ Chart.ScatterplotPage } />

    <Route exact path='/signIn' component={ Import.AuthPage } />

    <Route exact path='/import' component={ Import.ImportPage } />
    <Route exact path='/import/protocolSets/new' component={ NewDataset } />
    <Route exact path='/import/protocolSets' component={ ProtocolSets } />
    <Route path='/import/protocolSets/edit/:id' component={ EditDataset } />
</Layout>;
