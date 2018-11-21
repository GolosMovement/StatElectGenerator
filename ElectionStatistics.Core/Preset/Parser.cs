using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ElectionStatistics.Core.Preset
{
    public class Parser
    {
        public List<int> Execute(string expression)
        {
            if (expression == null)
            {
                return null;
            }

            // TODO: memoization
            return Regex.Matches(expression, @"(?<=\[)\d+(?=\])")
                .Select(x => Int32.Parse(x.Value))
                .ToList();
        }
    }
}
