import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './Layout/Component';
import { HistogramPage, ScatterplotPage } from './Chart/Page';

export const routes = <Layout>
	<Route exact path='/' component={ HistogramPage } />
	<Route exact path='/scatterplot' component={ ScatterplotPage } />
</Layout>;
