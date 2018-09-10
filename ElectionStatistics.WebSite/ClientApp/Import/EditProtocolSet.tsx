import React from 'react';
import { RouteComponentProps } from 'react-router';
import { Spin } from 'antd';

import { ImportController } from './ImportController';
import { IProtocolSet } from './NewProtocolSet';

interface IEditProtocolSetState {
    protocolSet: IProtocolSet;
    isLoading: boolean;
}

interface IEditProtocolSetProps extends RouteComponentProps<any> { }

export class EditProtocolSet extends React.Component<IEditProtocolSetProps, IEditProtocolSetState> {
    constructor(props: IEditProtocolSetProps) {
        super(props);

        this.state = {
            protocolSet: {
                titleEng: '', descriptionEng: '', titleRus: '', descriptionRus: '', hidden: false,
                id: props.match.params.id
            }, isLoading: false
        };

        this.changeTitleRu = this.changeTitleRu.bind(this);
        this.changeTitle = this.changeTitle.bind(this);
        this.changeDescrRu = this.changeDescrRu.bind(this);
        this.changeDescr = this.changeDescr.bind(this);
        this.changeHidden = this.changeHidden.bind(this);
        this.onSubmitForm = this.onSubmitForm.bind(this);
    }

    public render(): React.ReactNode {
        return (
            <div>
                <h1>{`Edit ProtocolSet #${this.state.protocolSet.id}`}</h1>
                <form onSubmit={this.onSubmitForm}>
                    <div className='row'>
                        <div className='col-sm-3'>
                            <div className='form-group'>
                                <label htmlFor='titleRus'>TitleRus</label>
                                <input id='titleRus' value={this.state.protocolSet.titleRus}
                                    onChange={this.changeTitleRu} className='form-control' />
                            </div>
                        </div>
                        <div className='col-sm-4'>
                            <div className='form-group'>
                                <label htmlFor='descriptionRus'>DescriptionRus</label>
                                <textarea id='descriptionRus' value={this.state.protocolSet.descriptionRus}
                                    onChange={this.changeDescrRu} className='form-control' ></textarea>
                            </div>
                        </div>

                        <div className='col-sm-5'>
                            <div className='checkbox'>
                                <label className='control-label'>
                                    <input type='checkbox' id='hidden' checked={this.state.protocolSet.hidden}
                                        onChange={this.changeHidden} />
                                    Hidden
                                </label>
                            </div>
                        </div>
                    </div>

                    <div className='row'>
                        <div className='col-sm-3'>
                            <div className='form-group'>
                                <label htmlFor='titleEng'>TitleEng</label>
                                <input id='titleEng' value={this.state.protocolSet.titleEng}
                                    onChange={this.changeTitle} className='form-control' />
                            </div>
                        </div>
                        <div className='col-sm-4'>
                            <div className='form-group'>
                                <label htmlFor='descriptionEng'>DescriptionEng</label>
                                <textarea id='descriptionEng' value={this.state.protocolSet.descriptionEng}
                                    onChange={this.changeDescr} className='form-control' />
                            </div>
                        </div>
                    </div>

                    <div className='row'>
                        <div className='form-group'>
                            <div className='col-sm-1'>{this.errorLogFileDownload()}</div>
                        </div>
                    </div>

                    <div className='row'>
                        <div className='col-sm-6'>
                            {this.submitButton()}
                        </div>
                    </div>
                </form>
            </div>
        );
    }

    public componentWillMount(): void {
        ImportController.Instance.protocolSet(this.props.match.params.id)
            .then((result) => this.setState({ ...this.state, protocolSet: result }));
    }

    private submitForm(): void {
        ImportController.Instance.updateDataset(this.props.match.params.id, this.state.protocolSet)
            .then((result) => {
                this.setState({ ...this.state, isLoading: false });

                if (result.status == 'ok') {
                    this.props.history.push('/import/protocolSets');
                } else {
                    alert('Fail!');
                }
            });
    }

    private onSubmitForm(e: React.FormEvent<HTMLFormElement>): void {
        e.preventDefault();

        this.setState({ ...this.state, isLoading: true });

        this.submitForm();
    }

    private errorLogFileDownload(): React.ReactNode {
        if (this.state.protocolSet.id) {
            const href = `/api/import/protocolSets/${this.state.protocolSet.id}/log`;
            return <a href={href} className='btn btn-default'>Download error logfile</a>;
        }
    }

    private submitButton(): React.ReactNode {
        if (this.state.isLoading) {
            return <Spin />;
        } else {
            return <input type='submit' className='btn btn-primary' />;
        }
    }

    private changeTitleRu(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, protocolSet: { ...this.state.protocolSet, titleRus: e.currentTarget.value } });
    }

    private changeDescrRu(e: React.ChangeEvent<HTMLTextAreaElement>): void {
        this.setState({
            ...this.state, protocolSet: {
                ...this.state.protocolSet,
                descriptionRus: e.currentTarget.value
            }
        });
    }

    private changeTitle(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, protocolSet: { ...this.state.protocolSet, titleEng: e.currentTarget.value } });
    }

    private changeDescr(e: React.ChangeEvent<HTMLTextAreaElement>): void {
        this.setState({
            ...this.state,
            protocolSet: { ...this.state.protocolSet, descriptionEng: e.currentTarget.value }
        });
    }

    private changeHidden(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, protocolSet: { ...this.state.protocolSet, hidden: e.currentTarget.checked } });
    }
}
