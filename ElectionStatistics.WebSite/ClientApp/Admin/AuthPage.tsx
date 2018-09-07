import React from 'react';
import { Redirect } from 'react-router-dom';

import { ImportController } from '../Import/ImportController';

interface IAuthProps {
    isAuthenticated: boolean;
    login: string;
    password: string;
    showAlert: boolean;
}

export class AuthPage extends React.Component<IAuthProps> {
    public state = {
        isAuthenticated: false,
        login: '',
        password: '',
        showAlert: false
    };

    constructor(props: IAuthProps) {
        super(props);

        this.setLogin = this.setLogin.bind(this);
        this.setPassword = this.setPassword.bind(this);
        this.trySignIn = this.trySignIn.bind(this);
    }

    public render(): React.ReactNode {
        if (this.state.isAuthenticated) {
            return <Redirect to='/admin' />;
        } else {
            return (
                <div className='sign-in col-sm-6 col-sm-offset-3'>
                    {this.failAlert()}
                    <div className='panel panel-default'>
                        <div className='panel-body'>
                            <form onSubmit={this.trySignIn}>
                                <div className='form-group'>
                                    <input id='login' className='form-control' onChange={this.setLogin}
                                        placeholder='login'/>
                                </div>

                                <div className='form-group'>
                                    <input id='password' type='password' className='form-control'
                                        onChange={this.setPassword} placeholder='password' />
                                </div>

                                <div className='form-group'>
                                    <input type='submit' className='btn btn-primary' value='Sign in' />
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            );
        }
    }

    private setLogin(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, login: e.currentTarget.value });
    }

    private setPassword(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, password: e.currentTarget.value });
    }

    private failAlert(): React.ReactNode {
        if (this.state.showAlert) {
            return (
                <div className='alert alert-danger alert-dismissible' role='alert'>
                    <button type='button' className='close' data-dismiss='alert' aria-label='Close'>
                        <span aria-hidden='true'>&times;</span>
                    </button>
                    <p>Wrong login or password</p>
                </div>
            );
        }
    }

    private trySignIn(e: React.FormEvent<HTMLFormElement>): void {
        e.preventDefault();

        ImportController.Instance.signIn(this.state.login, this.state.password)
            .then((result) => {
                this.setState({ ...this.state, isAuthenticated: result, showAlert: !result });
            });
    }
}
