using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Tests.Mocks;
using BenchmarkDotNet.Tests.Mocks.Toolchain;
using Xunit;
using Xunit.Abstractions;

namespace BenchmarkDotNet.Tests.Columns
{
    public class RatioColumnTest
    {
        private readonly ITestOutputHelper output;

        public RatioColumnTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void RatioColumnTest01()
        {
            var summary = MockHelper.Run<BenchmarkClass>(output);

            var ratioColumn = summary.GetColumns().FirstOrDefault(column => column.ColumnName == "Ratio");
            Assert.NotNull(ratioColumn);

            var fooCase = summary.BenchmarksCases.First(c => c.Descriptor.WorkloadMethod.Name == "Foo");
            var barCase = summary.BenchmarksCases.First(c => c.Descriptor.WorkloadMethod.Name == "Bar");
            Assert.Equal("1.00", ratioColumn.GetValue(summary, fooCase));
            Assert.Equal("2.00", ratioColumn.GetValue(summary, barCase));
        }

        [MockMeasurer(typeof(Measurer))]
        [MockJob]
        public class BenchmarkClass
        {
            [Benchmark(Baseline = true)]
            public void Foo() { }

            [Benchmark]
            public void Bar() { }
        }

        private class Measurer : MockMeasurer
        {
            public override List<Measurement> Measure(BenchmarkCase benchmarkCase)
            {
                return benchmarkCase.Descriptor.WorkloadMethod.Name switch
                {
                    "Foo" => CreateFromValues(new double[] { 2, 2, 2 }),
                    "Bar" => CreateFromValues(new double[] { 4, 4, 4 }),
                    _ => throw new InvalidOperationException()
                };
            }
        }
    }
}