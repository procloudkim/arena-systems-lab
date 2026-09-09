using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using NUnit.Framework;
using UnityEngine;

namespace ArenaSystemsLab.Tests.EditMode
{
    public sealed class SpatialHash2DTests
    {
        [TestCase(0f)]
        [TestCase(-1f)]
        public void Constructor_WithInvalidCellSize_Throws(float cellSize)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SpatialHash2D<int>(cellSize));
        }

        [Test]
        public void Query_ReturnsOnlyItemsInsideRadiusAcrossNegativeCells()
        {
            SpatialHash2D<string> index = new SpatialHash2D<string>(1f);
            index.Add("left", new Vector2(-1.1f, -0.1f));
            index.Add("center", new Vector2(-0.2f, -0.1f));
            index.Add("far", new Vector2(2f, 2f));
            List<string> results = new List<string> { "stale" };

            int count = index.Query(new Vector2(-0.5f, -0.1f), 0.7f, results);

            Assert.That(count, Is.EqualTo(2));
            CollectionAssert.AreEquivalent(new[] { "left", "center" }, results);
        }

        [TestCase(0f, 1)]
        [TestCase(1f, 5)]
        public void Query_IncludesExactRadiusBoundary(float radius, int expectedCount)
        {
            SpatialHash2D<int> index = new SpatialHash2D<int>(1f);
            Vector2[] points = { Vector2.zero, Vector2.left, Vector2.right, Vector2.up, Vector2.down, new Vector2(1.01f, 0f) };
            for (int i = 0; i < points.Length; i++)
            {
                index.Add(i, points[i]);
            }

            List<int> results = new List<int> { -1 };
            Assert.That(index.Query(Vector2.zero, radius, results), Is.EqualTo(expectedCount));
            CollectionAssert.AreEquivalent(radius == 0f ? new[] { 0 } : new[] { 0, 1, 2, 3, 4 }, results);
        }

        [Test]
        public void Query_MatchesBruteForceAndReportsMeasuredCost()
        {
            const int pointCount = 20000;
            const int queryCount = 500;
            const float radius = 3f;
            System.Random random = new System.Random(20260904);
            Vector2[] points = new Vector2[pointCount];
            Vector2[] centers = new Vector2[queryCount];
            SpatialHash2D<int> index = new SpatialHash2D<int>(2f);

            for (int i = 0; i < pointCount; i++)
            {
                points[i] = NextPoint(random);
                index.Add(i, points[i]);
            }

            for (int i = 0; i < queryCount; i++)
            {
                centers[i] = NextPoint(random);
            }

            List<int> results = new List<int>();
            index.Query(centers[0], radius, results);
            CountBruteForce(points, centers[0], radius);

            Stopwatch stopwatch = Stopwatch.StartNew();
            long spatialMatchCount = 0;
            for (int i = 0; i < centers.Length; i++)
            {
                spatialMatchCount += index.Query(centers[i], radius, results);
            }
            stopwatch.Stop();
            double spatialMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

            stopwatch.Restart();
            long bruteForceMatchCount = 0;
            for (int i = 0; i < centers.Length; i++)
            {
                bruteForceMatchCount += CountBruteForce(points, centers[i], radius);
            }
            stopwatch.Stop();
            double bruteForceMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

            Assert.That(spatialMatchCount, Is.EqualTo(bruteForceMatchCount));
            List<int> expectedResults = new List<int>();
            for (int query = 0; query < centers.Length; query++)
            {
                expectedResults.Clear();
                for (int point = 0; point < points.Length; point++)
                {
                    if ((points[point] - centers[query]).sqrMagnitude <= radius * radius)
                    {
                        expectedResults.Add(point);
                    }
                }

                int count = index.Query(centers[query], radius, results);
                Assert.That(count, Is.EqualTo(results.Count));
                CollectionAssert.AreEquivalent(expectedResults, results, $"Query {query} returned different IDs.");
            }

            TestContext.Out.WriteLine(
                "SPATIAL_HASH_BENCHMARK "
                + $"points={pointCount} queries={queryCount} matches={spatialMatchCount} "
                + $"spatialMs={spatialMilliseconds.ToString("F3", CultureInfo.InvariantCulture)} "
                + $"bruteForceMs={bruteForceMilliseconds.ToString("F3", CultureInfo.InvariantCulture)}");
        }

        [Test]
        public void Clear_RemovesAllEntries()
        {
            SpatialHash2D<int> index = new SpatialHash2D<int>(1f);
            index.Add(1, Vector2.zero);
            index.Clear();
            List<int> results = new List<int>();

            Assert.That(index.Count, Is.Zero);
            Assert.That(index.Query(Vector2.zero, 1f, results), Is.Zero);
        }

        private static Vector2 NextPoint(System.Random random)
        {
            return new Vector2(
                (float)(random.NextDouble() * 200d - 100d),
                (float)(random.NextDouble() * 200d - 100d));
        }

        private static int CountBruteForce(Vector2[] points, Vector2 center, float radius)
        {
            float radiusSquared = radius * radius;
            int count = 0;
            for (int i = 0; i < points.Length; i++)
            {
                if ((points[i] - center).sqrMagnitude <= radiusSquared)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
