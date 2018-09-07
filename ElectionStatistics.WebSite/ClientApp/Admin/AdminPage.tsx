import React from 'react';
import { Link } from 'react-router-dom';

export class AdminPage extends React.Component {
    public render(): React.ReactNode {
        return (
            <div className='col-sm-3'>
                <ul className='list-group'>
                    <li>
                        <Link to='/import' className='btn btn-default'>
                            <span className='glyphicon glyphicon-import' aria-hidden='true' /> DB manager
                        </Link>
                    </li>
                    <li>
                        <Link to='/presets' className='btn btn-default'>
                            <span className='glyphicon glyphicon-circle-arrow-down'
                                aria-hidden='true' /> Presets manager
                        </Link>
                    </li>
                </ul>
            </div>
        );
    }
}
