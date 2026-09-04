using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaSystemsLab
{
    public sealed class SpatialHash2D<T>
    {
        private readonly float cellSize;
        private readonly Dictionary<Vector2Int, List<Entry>> buckets = new Dictionary<Vector2Int, List<Entry>>();

        public SpatialHash2D(float cellSize)
        {
            if (cellSize <= 0f || float.IsNaN(cellSize) || float.IsInfinity(cellSize))
            {
                throw new ArgumentOutOfRangeException(nameof(cellSize));
            }

            this.cellSize = cellSize;
        }

        public int Count { get; private set; }

        public void Add(T item, Vector2 position)
        {
            ValidatePosition(position);
            Vector2Int cell = ToCell(position);
            if (!buckets.TryGetValue(cell, out List<Entry> entries))
            {
                entries = new List<Entry>();
                buckets.Add(cell, entries);
            }

            entries.Add(new Entry(item, position));
            Count++;
        }

        public int Query(Vector2 center, float radius, List<T> results)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            ValidatePosition(center);
            if (radius < 0f || float.IsNaN(radius) || float.IsInfinity(radius))
            {
                throw new ArgumentOutOfRangeException(nameof(radius));
            }

            results.Clear();
            float radiusSquared = radius * radius;
            Vector2Int minimum = ToCell(center - Vector2.one * radius);
            Vector2Int maximum = ToCell(center + Vector2.one * radius);

            for (int y = minimum.y; y <= maximum.y; y++)
            {
                for (int x = minimum.x; x <= maximum.x; x++)
                {
                    if (!buckets.TryGetValue(new Vector2Int(x, y), out List<Entry> entries))
                    {
                        continue;
                    }

                    for (int i = 0; i < entries.Count; i++)
                    {
                        Entry entry = entries[i];
                        if ((entry.Position - center).sqrMagnitude <= radiusSquared)
                        {
                            results.Add(entry.Item);
                        }
                    }
                }
            }

            return results.Count;
        }

        public void Clear()
        {
            // ponytail: rebuild-only index; add update/removal only if runtime profiling justifies adoption.
            buckets.Clear();
            Count = 0;
        }

        private Vector2Int ToCell(Vector2 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / cellSize),
                Mathf.FloorToInt(position.y / cellSize));
        }

        private static void ValidatePosition(Vector2 position)
        {
            if (float.IsNaN(position.x) || float.IsInfinity(position.x)
                || float.IsNaN(position.y) || float.IsInfinity(position.y))
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }
        }

        private readonly struct Entry
        {
            public Entry(T item, Vector2 position)
            {
                Item = item;
                Position = position;
            }

            public T Item { get; }
            public Vector2 Position { get; }
        }
    }
}
