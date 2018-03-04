namespace ElectionStatistics.WebSite
{
	public class HighchartsOptions
	{
		public AxisOptions XAxis { get; set; }
		public AxisOptions YAxis { get; set; }
		public ChartSeries[] Series { get; set; }
	}

	public class AxisOptions
	{
		public TitleOptions Title { get; set; }
		public AxisLabels Labels { get; set; }
		public double? Min { get; set; }
		public double? Max { get; set; }
	}

	public class AxisLabels
	{
		public bool Enabled { get; set; } = true;
	}

	public class TitleOptions
	{
		public string Text { get; set; }
	}

	public class ChartSeries
	{
		public string Name { get; set; }
	}

	public class HistogramChartSeries : ChartSeries
	{
		public double[][] Data { get; set; }
	}

	public class ScatterplotChartSeries : ChartSeries
	{
		public SeriesTooltipOptions Tooltip { get; set; }
		public Point[] Data { get; set; }
	}

	public class SeriesTooltipOptions
	{
		public string PointFormat { get; set; }
	}

	public struct Point
	{
		public string Name { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
	}
}