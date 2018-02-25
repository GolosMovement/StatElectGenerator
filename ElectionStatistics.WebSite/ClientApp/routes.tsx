import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './Layout/Component';
import { ChartPage } from './Chart/Page';

export const routes = 
    <Layout>
        <Route path='/histogram' component={ ChartPage } />
    </Layout>;
