import * as React from 'react';

import { RouteComponentProps, Link } from 'react-router-dom';
import * as QueryString from 'query-string'

import Select, { Option } from 'react-select';

import { LazyItems } from '../Common';
import { HighchartComponent } from '../Highchart/Component';

import { ElectionDto, ElectoralDistrictDto, CandidateDto, DictionariesController } from './DictionariesController';
import { ChartsController } from './ChartsController';



interface DistrictSelectorProps {
}

interface DistrictSelectorState extends LazyItems<ElectoralDistrictDto> {
}

export class DistrictSelector extends React.Component<DistrictSelectorProps, DistrictSelectorState> {
    public componentWillMount() {                
        this.loadDisticts();
    }

    public render() {
        if (this.state.chart.electionId == null) {
            return null;
        }
        else if (this.state.candidates.isLoading) {
            return (
                <Select
                    isLoading={true}
                />);
        }
        else {
            const options = this.state.candidates.items
                .map(candidate => ({ value: candidate.id as number, label: candidate.name }));

            return (
                <Select
                    clearable={false}
                    onChange={this.handleCandidateSelected}
                    value={this.state.chart.candidateId}
                    options={options}
                />
            );
        }
    }

    private loadDisticts() {
        if (this.state.chart.electionId != null) {
            this.setState({
                ...this.state,
                districts: {
                    isLoading: true,
                    items: []
                }
            });

            DictionariesController.Instance.getDistricts(this.state.chart.electionId)
                .then(districts => {
                    this.setState({
                        ...this.state,
                        districts: {
                            isLoading: false,
                            items: districts
                        }
                    });
                });
        }
    }

    private handleDistrictSelected = (selectedOption: any) => {
        if (selectedOption == null) {
            this.state.chart.districtIds.pop();
        }
        else {
            this.state.chart.districtIds.push(selectedOption.value);
        }
        this.setState({
            ...this.state
        });
    }    
}