import React from 'react';

import Highcharts from 'highcharts';
import HighchartsExporting from 'highcharts/modules/exporting';
(HighchartsExporting as any)(Highcharts);

interface IHighchartComponentProps {
    options: Highcharts.Options;
    exportOptions?: Highcharts.ExportingOptions;
}

export class HighchartComponent extends React.Component<IHighchartComponentProps, {}> {
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
            exporting: {
                sourceWidth: 1920,
                sourceHeight: 1080,
                width: 1920,
                menuItemDefinitions: {
                    printChart: {
                        text: 'Распечатать'
                    },
                    downloadPNG: {
                        text: 'Скачать PNG'
                    },
                    downloadJPEG: {
                        text: 'Скачать JPEG'
                    },
                    downloadPDF: {
                        text: 'Скачать PDF'
                    },
                    downloadSVG: {
                        text: 'Скачать SVG'
                    }
                },
                ...this.props.exportOptions as IHighchartComponentProps
            },
            chart: {
                height: '50%',
                ...this.props.options.chart,
                zoomType: 'xy',
                renderTo: this.chartRef
            }
        } as Highcharts.Options);
    }
}
