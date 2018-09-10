import React from 'react';
import { Route } from 'react-router-dom';

import * as Chart from './Chart';
import * as Import from './Import';
import { EditProtocolSet } from './Import/EditProtocolSet';
import { NewProtocolSet } from './Import/NewProtocolSet';
import { ProtocolSets } from './Import/ProtocolSets';
import { Layout } from './Layout/Component';

export const routes = <Layout>
    <Route exact path='/histogram' component={ Chart.HistogramPage } />
    <Route exact path='/location-scatterplot' component={ Chart.LocationScatterplotPage } />
    <Route exact path='/scatterplot' component={ Chart.ScatterplotPage } />
    <Route exact path='/last-digit-analyzer' component={ Chart.LastDigitAnalyzer } />

    <Route exact path='/signIn' component={ Import.AuthPage } />

    <Route exact path='/import' component={ Import.ImportPage } />
    <Route exact path='/import/protocolSets/new' component={ NewProtocolSet } />
    <Route exact path='/import/protocolSets' component={ ProtocolSets } />
    <Route path='/import/protocolSets/edit/:id' component={ EditProtocolSet } />
</Layout>;
