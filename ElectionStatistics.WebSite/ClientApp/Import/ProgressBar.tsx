import React from 'react';

interface IProgressProps {
    now: number;
    event?: 'pause' | 'fail';
}

enum BarType {
    Fail = 'danger', Pause = 'info', Progress = 'primary', Success = 'success'
}

interface IProgressState {
    barType: BarType;
}

export class ProgressBar extends React.Component<IProgressProps, IProgressState> {
    constructor(props: IProgressProps) {
        super(props);

        this.state = { barType: this.barType(props) };
    }

    public render(): React.ReactNode {
        const className = `progress-bar progress-bar-${this.state.barType} progress-bar-striped active`;

        return (
            <div className='progress'>
                <div className={className} role='progressbar'
                    aria-valuenow={this.props.now} aria-valuemin={0} aria-valuemax={100}
                    style={{ width: `${this.props.now}%`, minWidth: '2em' }}>
                    {this.props.now}%
                </div>
            </div>
        );
    }

    public componentWillReceiveProps(nextProps: IProgressProps) {
        this.setState({ ...this.state, barType: this.barType(nextProps) });
    }

    private barType(props: IProgressProps): BarType {
        switch (props.event) {
            case 'fail':
                return BarType.Fail;
            case 'pause':
                return BarType.Pause;
            default:
                if (props.now == 100) {
                    return BarType.Success;
                } else {
                    return BarType.Progress;
                }
        }
    }
}
