import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './Layout/Component';
import { AboutPage } from './About/Page';
import { ChartsPage } from './Charts/Page';

export const routes = 
    <Layout>
        <Route exact path='/' component={ AboutPage } />
        <Route path='/charts' component={ ChartsPage } />
    </Layout>;
