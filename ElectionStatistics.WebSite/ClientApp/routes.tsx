import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './Layout/Component';
import { LinePage, ScatterplotPage } from './Chart/Page';

export const routes = <Layout>
	<Route exact path='/histogram' component={ LinePage } />
	<Route exact path='/scatterplot' component={ ScatterplotPage } />
</Layout>;
