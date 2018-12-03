import React from 'react';
import { NavLink } from 'react-router-dom';
import { ChartPageRouteProps } from '../Chart/OldChartPage';
import { QueryString } from '../Common';

// FIXME: abandoned class
export class NavigationMenuComponent extends React.Component {
    public render() {
        const routeProps = QueryString.parse(window.location.search) as ChartPageRouteProps;
        const strippedRouteProps: ChartPageRouteProps = {
            electionId: routeProps.electionId,
            districtId: routeProps.districtId
        };
        let queryString = QueryString.stringify(strippedRouteProps);
        if (queryString != null) {
            queryString = '?' + queryString;
        }

        return (
            <div className='main-nav'>
                <div className='navbar navbar-inverse'>
                    <div className='clearfix'></div>
                    <div className='navbar-collapse collapse'>
                        <ul className='nav navbar-nav'>
                            <li>
                                <NavLink to={ '/histogram' + queryString } activeClassName='active'>
                                    <span className='glyphicon glyphicon-th'></span> Гистограмма
                                </NavLink>
                            </li>
                            <li>
                                <NavLink to={ '/scatterplot' + queryString } activeClassName='active'>
                                    <span className='glyphicon glyphicon-th'></span> Скаттерплот
                                </NavLink>
                            </li>
                            <li>
                                <NavLink to={ '/location-scatterplot' + queryString } activeClassName='active'>
                                    <span className='glyphicon glyphicon-th'></span> Диаграмма Габдулвалеева
                                </NavLink>
                            </li>
                            <li>
                                <NavLink to={ '/last-digit-analyzer' } activeClassName='active'>
                                    <span className='glyphicon glyphicon-th'></span> Метод последней цифры
                                </NavLink>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        );
    }
}
