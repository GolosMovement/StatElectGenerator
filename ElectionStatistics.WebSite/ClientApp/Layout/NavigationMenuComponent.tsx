import * as React from 'react';
import { NavLink, Link } from 'react-router-dom';

export class NavigationMenuComponent extends React.Component<{}, {}> {
    public render() {
        return <div className='main-nav'>
                <div className='navbar navbar-inverse'>
                <div className='clearfix'></div>
                <div className='navbar-collapse collapse'>
                    <ul className='nav navbar-nav'>
                        <li>
                            <NavLink to={ '/' } activeClassName='active'>
                                <span className='glyphicon glyphicon-th'></span> Гистограмма
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={ '/scatterplot' } activeClassName='active'>
                                <span className='glyphicon glyphicon-th'></span> Скаттерплот
                            </NavLink>
                        </li>
                    </ul>
                </div>
            </div>
        </div>;
    }
}
