using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.TestTools;

namespace ArenaSystemsLab.Tests.PlayMode
{
    public sealed class ArenaProfileBaselineTests
    {
        private const int SampleCapacity = 1024;
        private const int TargetFrameRate = 120;
        private const float WarmupSeconds = 1f;
        private const float SampleSeconds = 5f;

        [UnityTest]
        public IEnumerator GameplayBaseline_RecordsCpuAllocationAndObjectCount()
        {
            yield return null;
            ArenaGame game = UnityEngine.Object.FindFirstObjectByType<ArenaGame>();
            Assert.That(game, Is.Not.Null, "Runtime bootstrap did not create ArenaGame.");

            int previousTargetFrameRate = Application.targetFrameRate;
            ProfilerRecorder mainThread = default;
            ProfilerRecorder gcAllocated = default;
            ProfilerRecorder gameObjectCount = default;

            try
            {
                Application.targetFrameRate = TargetFrameRate;
                yield return new WaitForSecondsRealtime(WarmupSeconds);

                mainThread = StartRecorder("Main Thread");
                gcAllocated = StartRecorder("GC Allocated In Frame");
                gameObjectCount = StartRecorder("Game Object Count");

                Assert.That(mainThread.Valid, Is.True, "Main Thread profiler counter is unavailable.");
                Assert.That(gcAllocated.Valid, Is.True, "GC Allocated In Frame profiler counter is unavailable.");
                Assert.That(gameObjectCount.Valid, Is.True, "Game Object Count profiler counter is unavailable.");

                yield return null;
                mainThread.Reset();
                gcAllocated.Reset();
                gameObjectCount.Reset();
                mainThread.Start();
                gcAllocated.Start();
                gameObjectCount.Start();
                yield return new WaitForSecondsRealtime(SampleSeconds);

                MetricSummary mainThreadSummary = Summarize(mainThread);
                MetricSummary gcSummary = Summarize(gcAllocated);
                MetricSummary objectSummary = Summarize(gameObjectCount);

                Assert.That(mainThreadSummary.Count, Is.GreaterThan(0));
                Assert.That(gcSummary.Count, Is.GreaterThan(0));
                Assert.That(objectSummary.Count, Is.GreaterThan(0));

                TestContext.Out.WriteLine(
                    "ARENA_PROFILE_BASELINE "
                    + $"targetFps={TargetFrameRate} "
                    + $"warmupSeconds={WarmupSeconds.ToString("F1", CultureInfo.InvariantCulture)} "
                    + $"sampleSeconds={SampleSeconds.ToString("F1", CultureInfo.InvariantCulture)} "
                    + $"samples={mainThreadSummary.Count} "
                    + $"mainThreadMeanMs={(mainThreadSummary.Mean / 1_000_000d).ToString("F3", CultureInfo.InvariantCulture)} "
                    + $"mainThreadMaxMs={(mainThreadSummary.Maximum / 1_000_000d).ToString("F3", CultureInfo.InvariantCulture)} "
                    + $"gcAllocatedMeanBytes={gcSummary.Mean.ToString("F0", CultureInfo.InvariantCulture)} "
                    + $"gcAllocatedMaxBytes={gcSummary.Maximum} "
                    + $"gameObjectCountMean={objectSummary.Mean.ToString("F1", CultureInfo.InvariantCulture)} "
                    + $"gameObjectCountMax={objectSummary.Maximum}");
            }
            finally
            {
                mainThread.Dispose();
                gcAllocated.Dispose();
                gameObjectCount.Dispose();
                Application.targetFrameRate = previousTargetFrameRate;
            }
        }

        private static ProfilerRecorder StartRecorder(string counterName)
        {
            List<ProfilerRecorderHandle> handles = new List<ProfilerRecorderHandle>();
            ProfilerRecorderHandle.GetAvailable(handles);
            for (int i = 0; i < handles.Count; i++)
            {
                ProfilerRecorderDescription description = ProfilerRecorderHandle.GetDescription(handles[i]);
                if (string.Equals(description.Name, counterName, StringComparison.Ordinal))
                {
                    return new ProfilerRecorder(
                        handles[i],
                        SampleCapacity,
                        ProfilerRecorderOptions.StartImmediately
                            | ProfilerRecorderOptions.WrapAroundWhenCapacityReached
                            | ProfilerRecorderOptions.SumAllSamplesInFrame);
                }
            }

            return default;
        }

        private static MetricSummary Summarize(ProfilerRecorder recorder)
        {
            long total = 0;
            long maximum = 0;
            for (int i = 0; i < recorder.Count; i++)
            {
                long value = recorder.GetSample(i).Value;
                total += value;
                maximum = Math.Max(maximum, value);
            }

            return new MetricSummary(recorder.Count, recorder.Count == 0 ? 0d : (double)total / recorder.Count, maximum);
        }

        private readonly struct MetricSummary
        {
            public MetricSummary(int count, double mean, long maximum)
            {
                Count = count;
                Mean = mean;
                Maximum = maximum;
            }

            public int Count { get; }
            public double Mean { get; }
            public long Maximum { get; }
        }
    }
}
