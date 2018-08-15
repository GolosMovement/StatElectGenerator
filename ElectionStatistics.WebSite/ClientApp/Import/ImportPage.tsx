import React from 'react';
import { Link } from 'react-router-dom';

export class ImportPage extends React.Component {
    public render(): React.ReactNode {
        return (
            <div className='col-sm-6'>
                <h1>Database Manager</h1>
                <div className='row'>
                    <img src={'https://user-images.githubusercontent.com/1109847/' +
                        '44078601-98b3fb06-9faf-11e8-998c-79243a8cd725.png'}
                        width='200' height='200' className='db-schema-img' />
                </div>
                <div className='row'>
                    <div className='col-sm-6'>
                        <Link to='/import/protocolSets/new' className='btn btn-primary'
                            children='Import new ProtocolSet' />
                    </div>
                    <div className='col-sm-6'>
                        <Link to='/import/protocolSets/' className='btn btn-primary'
                            children='Edit imported ProtocolSet' />
                    </div>
                </div>
            </div>
        );
    }
}
