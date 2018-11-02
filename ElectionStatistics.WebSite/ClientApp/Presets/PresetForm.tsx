import React from 'react';
import { RouteComponentProps } from 'react-router';
import { Spin } from 'antd';

import { IModel } from '../Chart';
import { DictionariesController } from '../Chart/DictionariesController';
import { LazySelect, ILazySelectProps } from '../Common';
import { IApiResponse } from '../Import/ImportController';
import { IProtocolSet } from '../Import/NewProtocolSet';
import { IPreset } from './PresetList';

export interface ILineDescription extends IModel {
    descriptionRus: string;
    descriptionEng: string;
    descriptionNative: string;
}

interface IPresetFormProps extends RouteComponentProps<any> {}

interface IPresetFormState {
    preset: IPreset;
    isLoading: boolean;
    lineDescriptions: ILineDescription[];
}

export abstract class PresetForm extends React.Component<IPresetFormProps, IPresetFormState> {
    constructor(props: IPresetFormProps) {
        super(props);

        const protocolSetIdMatch = props.location.search.match(/protocolSetId=(\d+)/);
        this.state = {
            preset: {
                id: props.match.params.id, titleRus: '', titleEng: '', descriptionRus: '', descriptionEng: '',
                expression: '', protocolSetId: protocolSetIdMatch ? parseInt(protocolSetIdMatch[1], 10) : null
            },
            isLoading: false, lineDescriptions: []
        };

        this.changeProtocolSetId = this.changeProtocolSetId.bind(this);
        this.changeTitleRus = this.changeTitleRus.bind(this);
        this.changeTitleEng = this.changeTitleEng.bind(this);
        this.changeDescriptionRus = this.changeDescriptionRus.bind(this);
        this.changeDescriptionEng = this.changeDescriptionEng.bind(this);
        this.changeExpression = this.changeExpression.bind(this);

        this.onSubmitForm = this.onSubmitForm.bind(this);
    }

    public render(): React.ReactNode {
        return (
            <div>
                <form onSubmit={this.onSubmitForm}>
                    <div className='col-sm-9'>
                        <div className='row'>
                            <p className='text-center'>Набор протоколов:</p>
                            {this.protocolSetSelect()}
                        </div>

                        <div className='row'>
                            <p className='text-center'>Опишите расчётную величину для визуализации (preset):</p>
                            <label className='col-sm-3 control-label'>Заголовок (рус)</label>
                            <div className='col-sm-9'>
                                <input type='text' value={this.state.preset.titleRus} onChange={this.changeTitleRus}
                                    className='form-control' />
                            </div>
                        </div>
                        <div className='row'>
                            <label className='col-sm-3 control-label'>Описание (рус)</label>
                            <div className='col-sm-9'>
                                <textarea value={this.state.preset.descriptionRus} onChange={this.changeDescriptionRus}
                                    className='form-control' />
                            </div>
                        </div>
                        <div className='row'>
                            <label className='col-sm-3 control-label'>Title (eng)</label>
                            <div className='col-sm-9'>
                                <input type='text' value={this.state.preset.titleEng} onChange={this.changeTitleEng}
                                    className='form-control' />
                            </div>
                        </div>
                        <div className='row'>
                            <label className='col-sm-3 control-label'>Заголовок (рус)</label>
                            <div className='col-sm-9'>
                                <textarea value={this.state.preset.descriptionEng} onChange={this.changeDescriptionEng}
                                    className='form-control' />
                            </div>
                        </div>

                        <div className='row'>
                            <p className='text-center'>
                                Используя обозначения ниже, составьте формулу (например: "(323+212)/382"):
                            </p>
                            <div className='col-sm-10'>
                                <input type='text' value={this.state.preset.expression} onChange={this.changeExpression}
                                    className='form-control' />
                            </div>
                            <div className='col-sm-2'>
                                {this.submitButton()}
                            </div>
                        </div>

                        <div className='row'>
                            {this.lineDescriptionTable()}
                        </div>
                    </div>
                </form>
            </div>
        );
    }

    protected abstract submitRequestFunc(): (preset: IPreset) => Promise<IApiResponse>;

    protected submitForm(): void {
        this.submitRequestFunc()(this.state.preset).then((result) => {
            this.setState({ ...this.state, isLoading: false });

            if (result.status == 'ok') {
                alert('Success!');
                DictionariesController.reset();
                this.props.history.push(`/presets?=protocolSetId=${this.state.preset.protocolSetId}`);
            } else {
                alert(`Fail! ${result.message}`);
            }
        }).catch((error) => {
            this.setState({ ...this.state, isLoading: false });
            alert('Server error!');
        });
    }

    private protocolSetSelect(): React.ReactNode {
        const LSelect = LazySelect as new (props: ILazySelectProps<IProtocolSet, number>) =>
            LazySelect<IProtocolSet, number>;

        return <LSelect
            itemsPromise={DictionariesController.Instance.getProtocolSets()}
            selectedValue={this.state.preset.protocolSetId}
            getValue={(protocolSet) => protocolSet.id}
            getText={(protocolSet) => protocolSet.titleRus}
            onChange={this.changeProtocolSetId} />;
    }

    private lineDescriptionTable(): React.ReactNode {
        if (this.state.preset.protocolSetId) {
            return (
                <table className='table table-bordered table-condensed table-hover'>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Заголовок (рус)</th>
                            <th>Title (eng)</th>
                            <th>Описание (рус)</th>
                            <th>Description (eng)</th>
                            <th>Description (native)</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.lineDescriptionRows()}
                    </tbody>
                </table>
            );
        }
    }

    private lineDescriptionRows(): React.ReactNode {
        const rows = this.state.lineDescriptions.map((line, i) =>
            <tr key={i}>
                <td><small>{line.id}</small></td>
                <td><small>{line.titleRus}</small></td>
                <td><small>{line.titleEng}</small></td>
                <td><small>{line.descriptionRus}</small></td>
                <td><small>{line.descriptionEng}</small></td>
                <td><small>{line.descriptionNative}</small></td>
            </tr>
        );

        return rows;
    }

    private changeProtocolSetId(protocolSetId: number | null): void {
        this.setState({ ...this.state, preset: { ...this.state.preset, protocolSetId } });

        if (protocolSetId) {
            DictionariesController.Instance.getLineDescriptions(protocolSetId)
                .then((result) => this.setState({ ...this.state, lineDescriptions: result }));
        }
    }

    private changeTitleRus(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, preset: { ...this.state.preset, titleRus: e.currentTarget.value } });
    }

    private changeTitleEng(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, preset: { ...this.state.preset, titleEng: e.currentTarget.value } });
    }

    private changeDescriptionRus(e: React.ChangeEvent<HTMLTextAreaElement>): void {
        this.setState({ ...this.state, preset: { ...this.state.preset, descriptionRus: e.currentTarget.value } });
    }

    private changeDescriptionEng(e: React.ChangeEvent<HTMLTextAreaElement>): void {
        this.setState({ ...this.state, preset: { ...this.state.preset, descriptionEng: e.currentTarget.value } });
    }

    private changeExpression(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ ...this.state, preset: { ...this.state.preset, expression: e.currentTarget.value } });
    }

    private submitButton(): React.ReactNode {
        if (this.state.isLoading) {
            return <Spin />;
        } else {
            return <input type='submit' className='btn btn-primary' value='Сохранить' />;
        }
    }

    private onSubmitForm(e: React.FormEvent<HTMLFormElement>): void {
        e.preventDefault();

        this.setState({ ...this.state, isLoading: true });

        this.submitForm();
    }
}
