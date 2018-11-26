import React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { Spin } from 'antd';

import { DictionariesController } from '../Chart/DictionariesController';
import { LazySelect, ILazySelectProps } from '../Common';
import { IProtocolSet } from '../Import/NewProtocolSet';
import { PresetsController } from './PresetsController';

export interface IPreset {
    id: number;
    titleRus: string;
    titleEng: string;
    descriptionRus: string;
    descriptionEng: string;
    expression: string;
    protocolSetId: number | null;
}

interface IPresetListState {
    protocolSetIndex: number | null;
    protocolSets: IProtocolSet[];
    protocolSetsPromise: Promise<IProtocolSet[]> | null;
    presets: IPreset[];
    isRecalcLoading: boolean;
}

interface IPresetListProps extends RouteComponentProps<any> { }

export class PresetList extends React.Component<IPresetListProps, IPresetListState> {
    constructor(props: IPresetListProps) {
        super(props);

        this.state = {
            protocolSetIndex: null, protocolSetsPromise: null, protocolSets: [], presets: [],
            isRecalcLoading: false
        };

        this.deletePreset = this.deletePreset.bind(this);
        this.recreateCalcValues = this.recreateCalcValues.bind(this);
    }

    public render(): React.ReactNode {
        return (
            <div>
                <h1>Presets Manager</h1>
                <div className='row'>
                    <label className='col-sm-12 control-label'>Выберите набор протоколов (т.е. выборы):</label>
                    <div className='col-md-6'>
                        {this.protocolSetSelect()}
                    </div>
                    <div className='col-sm-6'>
                        {this.recreateCalcValuesButton()}
                    </div>
                </div>
                {this.protocolSetInfo()}
                <div className='row'>
                    <label className='col-sm-12 control-label'>
                        Для данного набора протоколов определены следующие величины для анализа (presets):
                    </label>
                </div>

                {this.state.presets.length > 0 ? this.table() : ''}

                <div className='row'>
                    <div className='col-sm-6'>
                        {this.createNewButton()}
                    </div>
                </div>
            </div>
        );
    }

    public componentWillMount() {
        this.getProtocolSets().then((protocolSets) => {
            this.setState({ ...this.state, protocolSets });
            const protocolSetIdMatch = this.props.location.search.match(/protocolSetId=(\d+)/);
            const protocolSetId = protocolSetIdMatch ? parseInt(protocolSetIdMatch[1], 10) : null;
            if (protocolSetId) {
                protocolSets.forEach((protocolSet, i) => {
                    if (protocolSet.id == protocolSetId) {
                        this.setState({ ...this.state, protocolSetIndex: i });
                        this.fetchPresets(protocolSet);
                        return;
                    }
                });

            }
        });
    }

    private protocolSetSelect(): React.ReactNode {
        const LSelect = LazySelect as new (props: ILazySelectProps<IProtocolSet, number>) =>
            LazySelect<IProtocolSet, number>;

        return <LSelect
            itemsPromise={this.getProtocolSets()}
            selectedValue={this.state.protocolSetIndex}
            getValue={(protocolSet) => this.state.protocolSets.indexOf(protocolSet)}
            getText={(protocolSet) => protocolSet.titleRus}
            onChange={(protocolSet) => this.changeProtocolSet(protocolSet)} />;
    }

    private recreateCalcValuesButton(): React.ReactNode {
        if (this.state.isRecalcLoading) {
            return <Spin size='large' />;
        } else if (this.state.protocolSetIndex != null) {
            const protocolSet = this.state.protocolSets[this.state.protocolSetIndex];
            return (
                <button type='button' value={protocolSet.id} onClick={this.recreateCalcValues}
                    className='btn btn-xs btn-default'
                    disabled={!protocolSet.shouldRecalculatePresets}>
                    Пересоздать расчетную таблицу
                </button>
            );
        }
    }

    private getProtocolSets(): Promise<IProtocolSet[]> {
        if (this.state.protocolSetsPromise != null) { return this.state.protocolSetsPromise; }

        const reqPromise = DictionariesController.Instance.getProtocolSets();
        this.setState({ ...this.state, protocolSetsPromise: reqPromise });

        return reqPromise;
    }

    private protocolSetInfo(): React.ReactNode {
        if (this.state.protocolSetIndex) {
            const protocolSet = this.state.protocolSets[this.state.protocolSetIndex];
            const titleEng = protocolSet.titleEng;

            let titleEngDiv: React.ReactNode;
            if (titleEng) {
                titleEngDiv =
                    <div className='row'>
                        <div className='col-sm-6'>
                            <strong>{titleEng}</strong>
                        </div>
                    </div>;
            }

            return (
                <div>
                    {this.protocolSetDescr(protocolSet.descriptionRus)}
                    {titleEngDiv}
                    {this.protocolSetDescr(protocolSet.descriptionEng)}
                </div>
            );
        }
    }

    private protocolSetDescr(text: string): React.ReactNode {
        if (text) {
            return (
                <div className='row'>
                    <div className='col-sm-6'>
                        <div className='panel panel-default'>
                            <div className='panel-body'>{text}</div>
                        </div>
                    </div>
                </div>
            );
        }
    }

    private table(): React.ReactNode {
        return (
            <table className='table table-bordered table-condensed table-hovered'>
                <thead>
                    <tr>
                        <th>Заголовок (рус)</th>
                        <th>Описание (рус)</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {this.presetRows()}
                </tbody>
            </table>
        );
    }

    private presetRows(): React.ReactNode {
        const rows = this.state.presets.map((preset, i) =>
            <tr key={i}>
                <td>{preset.titleRus}</td>
                <td>{preset.descriptionRus}</td>
                <td>
                    <div className='btn-group'>
                        <Link to={`/presets/${preset.id}/edit`} className='btn btn-xs btn-primary' children='Edit' />
                        <button type='button' value={preset.id} onClick={this.deletePreset}
                            className='btn btn-xs btn-danger'>Delete</button>
                    </div>
                </td>
            </tr>
        );

        return rows;
    }

    private createNewButton(): React.ReactNode {
        let href = '/presets/new';

        if (this.state.protocolSetIndex != null) {
            href += `?protocolSetId=${this.state.protocolSets[this.state.protocolSetIndex].id}`;
        }

        return <a href={href} className='btn btn-primary'>Добавить Preset</a>;
    }

    private deletePreset(e: React.MouseEvent<HTMLButtonElement>): void {
        e.preventDefault();

        if (confirm('Вы уверены?')) {
            const id = parseInt(e.currentTarget.value, 10);

            PresetsController.Instance.deletePreset(id)
                .then((result) => {
                    if (result.status == 'ok') {
                        this.setState({
                            ...this.state,
                            presets: this.state.presets.filter((preset) => preset.id != id)
                        });
                        alert('Удалено!');
                    } else {
                        alert('Не удалось выполнить удаление');
                    }
                })
                .catch(() => alert('Server error!'));
        }
    }

    private changeProtocolSet(protocolSetIndex: number | null): void {
        if (protocolSetIndex != null) {
            this.setState({ ...this.state, protocolSetIndex });

            this.fetchPresets(this.state.protocolSets[protocolSetIndex]);
        }
    }

    private fetchPresets(protocolSet: IProtocolSet): void {
        DictionariesController.Instance.getPresets(protocolSet.id)
            .then((result) => this.setState({ ...this.state, presets: result }));
    }

    private recreateCalcValues(e: React.MouseEvent<HTMLButtonElement>): void {
        if (confirm('Вы уверены?')) {
            const protocolSetId = parseInt(e.currentTarget.value, 10);

            this.setState({ ...this.state, isRecalcLoading: true });

            PresetsController.Instance.recreateCalcValues(protocolSetId)
                .then((result) => {
                    if (result.status == 'ok') {
                        const protocolSets = this.state.protocolSets;
                        protocolSets.forEach((protocolSet) => {
                            if (protocolSet.id == protocolSetId) {
                                protocolSet.shouldRecalculatePresets = false;
                            }
                        });
                        this.setState({ ...this.state, isRecalcLoading: false, protocolSets });
                        alert('Расчетная таблица создана');
                    } else if (result.status === 'fail_timeout') {
                        this.setState({ ...this.state, isRecalcLoading: false });
                        alert('Ошибка: запрос создания таблицы прерван по таймауту');
                    } else {
                        this.setState({ ...this.state, isRecalcLoading: false });
                        alert('Не удалось создать расчетную таблицу');
                    }
                })
                .catch(() => {
                    this.setState({ ...this.state, isRecalcLoading: false });
                    alert('Server error!');
                });
        }
    }
}
