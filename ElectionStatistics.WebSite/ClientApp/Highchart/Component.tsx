import * as Highcharts from 'highcharts';
import * as React from 'react';

export class HighchartComponent extends React.Component<{}, {}> {
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
            chart: {
                type: 'bar',
                renderTo: this.chartRef,
            },
            title: {
                text: 'Fruit Consumption'
            },
            xAxis: {
                categories: ['Apples', 'Bananas', 'Oranges']
            },
            yAxis: {
                title: {
                    text: 'Fruit eaten'
                }
            },
            series: [{
                name: 'Jane',
                data: [1, 0, 4]
            }, {
                name: 'John',
                data: [5, 7, 3]
            }]
        })
    }
}