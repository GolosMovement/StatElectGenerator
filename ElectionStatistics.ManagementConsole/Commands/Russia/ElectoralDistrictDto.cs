using System;
using System.Collections.Generic;
using System.Text;

namespace ElectionStatistics.ManagementConsole
{
	internal class ElectoralDistrictDto
	{
		public string Name;
		public string Vibid;
		public List<ElectoralDistrictDto> InnerDistricts;
		public ElectionResultsDto Results;
	}
}
