﻿using BenchmarkDotNet.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BenchmarkDotNet.Mathematics;

namespace BenchmarkDotNet.Extensions
{
    public static class ReportExtensions
    {
        public static BenchmarkReport GetReportFor<T>(this Summary summary, Expression<Action<T>> actionExp)
        {
            if (actionExp.Body == null)
                throw new ArgumentException("Extend a an Expression with a valid Body", nameof(actionExp));

            var methodExp = actionExp.Body as MethodCallExpression;
            if (methodExp == null)
                throw new ArgumentException("Extend a MethodCallExpression, but got a " + actionExp.Body.GetType().Name, nameof(actionExp));

            return summary.Reports.First(r => r.BenchmarkCase.Descriptor.WorkloadMethod == methodExp.Method);
        }

        public static IList<Measurement> GetRunsFor<T>(this Summary summary, Expression<Action<T>> actionExp)
        {
            return summary.GetReportFor<T>(actionExp).GetResultRuns().ToList();
        }

        public static Statistics GetStatistics(this IReadOnlyCollection<Measurement> runs)
        {
            if (runs.IsEmpty())
                throw new InvalidOperationException("List of measurements contains no elements");
            return new Statistics(runs.Select(r => r.GetAverageNanoseconds()));
        }

        public static Statistics GetStatistics(this IEnumerable<Measurement> runs) =>
            GetStatistics(runs.ToList());
    }
}
