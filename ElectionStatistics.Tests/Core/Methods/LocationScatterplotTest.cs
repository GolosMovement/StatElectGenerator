using System;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;

using ElectionStatistics.Core.Methods;
using ElectionStatistics.Model;

namespace ElectionStatistics.Tests.Core.Methods
{
    public class LocationScatterplotTest
    {
        private static ModelContext modelContext;
        private static LocationScatterplot service;

        public LocationScatterplotTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ModelContext>();
            optionsBuilder.UseInMemoryDatabase(this.GetType().ToString());
            modelContext = new ModelContext(optionsBuilder.Options);
            service = new LocationScatterplot(modelContext);
        }

        [Fact]
        public void GetData_OK_ReturnsData()
        {
            var protocolSet = new ProtocolSet();
            modelContext.Add(protocolSet);

            var topProtocol = new Protocol
            {
                ProtocolSetId = protocolSet.Id, CommissionNumber = 0, TitleRus = "Syzran"
            };
            modelContext.Add(topProtocol);

            var protocols = new int[] { 1, 2, 3 }.Select(num =>
                new Protocol
                {
                    ProtocolSetId = protocolSet.Id,
                    CommissionNumber = num,
                    TitleRus = $"Protocol_{num}",
                    ParentId = topProtocol.Id
                }
            ).ToArray();
            modelContext.AddRange(protocols);

            var preset = new Model.Preset { ProtocolSetId = protocolSet.Id };
            modelContext.Add(preset);

            var calcValues = new int[] { 1, 2, 3 }.Select(num =>
                new LineCalculatedValue
                {
                    PresetId = preset.Id,
                    Value = new Random().Next(1, 100),
                    ProtocolId = protocols[num - 1].Id
                }
            ).ToArray();
            modelContext.AddRange(calcValues);

            modelContext.SaveChanges();

            var parameters = new LocationScatterplotBuildParameters
            {
                ProtocolId = topProtocol.Id, ProtocolSetId = protocolSet.Id, Y = preset.Id
            };
            var data = service.GetData(parameters, calcValues, new Protocol[] { topProtocol })
                .Select(group => new
                {
                    TopProtocolId = group.Key.Id,
                    Values = group.Select(a => new
                    {
                        a.Y, a.ProtocolName, a.TopParentProtocol.Id
                    }).ToArray()
                }).ToArray();

            Assert.NotEmpty(data);

            Assert.Equal(topProtocol.Id, data[0].TopProtocolId);

            for (var i = 0; i < data[0].Values.Length; ++i)
            {
                Assert.Equal(new
                {
                    Y = calcValues[i].Value,
                    ProtocolName = protocols[i].TitleRus,
                    Id = topProtocol.Id
                }, data[0].Values[i]);
            }
        }
    }
}
