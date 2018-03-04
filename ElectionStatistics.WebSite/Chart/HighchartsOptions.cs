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
		public decimal? Min { get; set; }
		public decimal? Max { get; set; }
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
		public Point[] Data { get; set; }
	}

	public class ScatterplotChartSeries : ChartSeries
	{
		public SeriesTooltipOptions Tooltip { get; set; }
	}

	public class SeriesTooltipOptions
	{
		public string PointFormat { get; set; }
	}

	public struct Point
	{
		public string Name { get; set; }
		public decimal X { get; set; }
		public decimal Y { get; set; }
	}
}