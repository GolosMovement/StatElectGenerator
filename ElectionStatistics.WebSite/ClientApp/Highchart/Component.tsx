import * as Highcharts from 'highcharts';
import * as React from 'react';


export class HighchartComponent extends React.Component<{options: Highcharts.Options}, {}> {
    private chartRef?: HTMLElement;
   
    public componentDidMount() {
        this.renderChart();
    }
    
    public render() {
        return <div ref={this.setChartRef.bind(this)} />;
    }

    private setChartRef(chartRef: HTMLElement) {
        this.chartRef = chartRef;
    }

    private renderChart() {
        Highcharts.chart({
            ...this.props.options,
            chart: {
                ...this.props.options.chart,
                height: '50%',
                zoomType: 'xy',
                renderTo: this.chartRef,
            }
        } as Highcharts.Options);
    }
}