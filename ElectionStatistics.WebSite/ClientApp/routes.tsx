import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './Layout/Component';
import { HistogramPage, ScatterplotPage } from './Chart';

export const routes = <Layout>
	<Route exact path='/histogram' component={ HistogramPage } />
	<Route exact path='/scatterplot' component={ ScatterplotPage } />
</Layout>;
