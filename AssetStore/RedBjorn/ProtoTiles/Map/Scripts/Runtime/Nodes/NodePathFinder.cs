using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedBjorn.ProtoTiles
{
    public class NodePathFinder
    {
        static Dictionary<INode, float> ScoreG = new Dictionary<INode, float>();
        static Dictionary<INode, float> ScoreF = new Dictionary<INode, float>();
        static Dictionary<INode, INode> CameFrom = new Dictionary<INode, INode>();

        public static HashSet<INode> AccessibleArea(IMapNode map, INode origin)
        {
            map.Reset();
            var open = new Queue<INode>();
            var closed = new HashSet<INode>();

            open.Enqueue(origin);
            var index = 0;
            while (open.Count > 0 && index < 100000)
            {
                var current = open.Dequeue();
                current.Considered = true;
                foreach (var n in map.NeighborsMovable(current).Where(neigh => neigh != null))
                {
                    if (n.Vacant && !n.Considered)
                    {
                        n.Considered = true;
                        open.Enqueue(n);
                        index++;
                    }
                }
                current.Visited = true;
                closed.Add(current);

            }
            return closed;
        }

        public static HashSet<INode> WalkableArea(IMapNode map, INode origin, float range)
        {
            map.Reset(Mathf.CeilToInt(range), origin);
            origin.Depth = 0f;

            var open = new List<INode> { origin };
            var reachable = new SortedDictionary<int, List<INode>>();
            var closed = new HashSet<INode>();

            var index = 0;
            while (open.Any() && index < 100000)
            {
                open.ForEach(current =>
                {
                    foreach (var n in map.NeighborsMovable(current).Where(neigh => neigh != null))
                    {
                        var distance = current.Depth + map.Distance(current, n) * n.Weight;
                        if (n.Vacant && !n.Visited && distance <= range && distance < n.Depth)
                        {
                            n.Depth = distance;
                            int intDistance = Mathf.CeilToInt(distance);

                            if (reachable.TryGetValue(intDistance, out List<INode> list))
                            {
                                list.Add(n);
                            } else
                            {
                                reachable.Add(intDistance, new List<INode> { n });
                            }
                        }
                    }

                    current.Visited = true;
                    closed.Add(current);
                    index++;
                });

                using var enumerator = reachable.GetEnumerator();
                open.Clear();
                while (enumerator.MoveNext())
                {
                    var smallestKey = enumerator.Current.Key;
                    open = enumerator.Current.Value;
                    open.RemoveAll(n => Mathf.CeilToInt(n.Depth) != smallestKey);
                    reachable.Remove(smallestKey);
                    if (open.Any()) break;
                }
            }
            return closed;
        }

        public static HashSet<Vector3Int> WalkableAreaPositions(IMapNode map, INode origin, float range)
        {
            map.Reset(Mathf.CeilToInt(range), origin);
            origin.Depth = 0f;
            var open = new List<INode> { origin };
            var reachable = new SortedDictionary<int, List<INode>>();
            var closed = new HashSet<Vector3Int>();

            var index = 0;
            while (open.Any() && index < 100000)
            {
                open.ForEach(current =>
                {
                    foreach (var n in map.NeighborsMovable(current).Where(neigh => neigh != null))
                    {
                        var distance = current.Depth + map.Distance(current, n) * n.Weight;
                        if (n.Vacant && !n.Visited && distance <= range && distance < n.Depth)
                        {
                            n.Depth = distance;
                            int intDistance = Mathf.CeilToInt(distance);

                            if (reachable.TryGetValue(intDistance, out List<INode> list))
                            {
                                list.Add(n);
                            } else
                            {
                                reachable.Add(intDistance, new List<INode> { n });
                            }
                        }
                    }

                    current.Visited = true;
                    closed.Add(current.Position);
                    index++;
                });

                using var enumerator = reachable.GetEnumerator();
                open.Clear();
                while (enumerator.MoveNext())
                {
                    var smallestKey = enumerator.Current.Key;
                    open = enumerator.Current.Value;
                    open.RemoveAll(n => Mathf.CeilToInt(n.Depth) != smallestKey);
                    reachable.Remove(smallestKey);
                    if (open.Any()) break;
                }
            }
            return closed;
        }

        public static List<INode> Path(IMapNode map, INode start, INode finish, float range)
        {
            if (start.MovableArea != finish.MovableArea)
            {
                return null;
            }
            var fullPath = FindPath(map, start, finish);
            return TrimPath(map, fullPath, range);
        }

        public static List<INode> Path(IMapNode map, INode start, INode finish)
        {
            if (start.MovableArea != finish.MovableArea)
            {
                return null;
            }
            return FindPath(map, start, finish);
        }

        static List<INode> FindPath(IMapNode map, INode start, INode finish)
        {
            // Avoid calculation for unreachable tiles
            if (!finish.Vacant || finish.Weight == float.MaxValue || finish.MovableArea != start.MovableArea) 
                return new List<INode>();

            map.Reset();
            start.Depth = 0f;

            var open = new List<(INode, INode)> { (start, null) };
            var reachable = new SortedDictionary<int, List<(INode, INode)>>();
            var closed = new Dictionary<INode, INode>();

            var index = 0;
            while (open.Any() && !closed.ContainsKey(finish) && index < 100000)
            {
                open.ForEach(pair =>
                {
                    var current = pair.Item1;
                    foreach (var n in map.NeighborsMovable(current).Where(neigh => neigh != null))
                    {
                        var distance = current.Depth + map.Distance(current, n) * n.Weight;
                        if (n.Vacant && !n.Visited && distance < n.Depth)
                        {
                            n.Considered = true;
                            n.Depth = distance;
                            int intDistance = Mathf.CeilToInt(distance);

                            if (reachable.TryGetValue(intDistance, out List<(INode, INode)> list))
                            {
                                list.Add((n, current));
                            } else
                            {
                                reachable.Add(intDistance, new List<(INode, INode)> { (n, current) });
                            }
                        }
                    }

                    current.Visited = true;
                    closed.Add(current, pair.Item2);
                    index++;
                });

                using var enumerator = reachable.GetEnumerator();
                open.Clear();
                while (enumerator.MoveNext())
                {
                    var smallestKey = enumerator.Current.Key;
                    open = enumerator.Current.Value;
                    open.RemoveAll(n => Mathf.CeilToInt(n.Item1.Depth) != smallestKey);
                    reachable.Remove(smallestKey);
                    if (open.Any()) break;
                }
            }

            var current = finish;
            var path = new List<INode>();
            while (current != null && closed.TryGetValue(current, out INode previous))
            {
                path.Add(current);
                current = previous;
            }
            path.Reverse();

            return path;
        }

        static List<INode> TrimPath(IMapNode map, List<INode> path, float range)
        {
            var distance = 0f;
            int trimIndex = -1;
            for (int i = 0; i < path.Count - 1; i++)
            {
                var step = distance + map.Distance(path[i], path[i + 1]) * path[i + 1].Weight;
                if (step <= range)
                {
                    distance = step;
                }
                else
                {
                    trimIndex = i + 1;
                    break;
                }
            }
            if (trimIndex >= 0)
            {
                path.RemoveRange(trimIndex, path.Count - trimIndex);
            }
            return path;
        }
    }
}