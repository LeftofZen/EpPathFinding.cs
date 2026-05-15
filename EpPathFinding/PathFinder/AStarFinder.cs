/*!
@file JumpPointFinder.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief A Star Algorithm Interface
@version 2.0

@section LICENSE

The MIT License (MIT)

Copyright (c) 2013 Woong Gyu La <juhgiyo@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

@section DESCRIPTION

An Interface for the Jump Point Search Algorithm Class.

*/

#if UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE || UNITY_WII || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS4 || UNITY_SAMSUNGTV || UNITY_XBOXONE || UNITY_TIZEN || UNITY_TVOS || UNITY_WP_8_1 || UNITY_WSA || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WINRT || UNITY_WINRT_8_1 || UNITY_WINRT_10_0 || UNITY_WEBGL || UNITY_ADS || UNITY_ANALYTICS || UNITY_ASSERTIONS
#define UNITY
#else
using System.Threading.Tasks;
#endif
using C5;
using System;
using System.Collections.Generic;

namespace EpPathFinding.cs
{
	public class AStarParam : ParamBase
	{
		public delegate float HeuristicDelegate(int dx, int dy);

		public float Weight;

		public AStarParam(BaseGrid grid, GridPos startPos, GridPos endPos, float weight, DiagonalMovement diagonalMovement = DiagonalMovement.Always, HeuristicMode mode = HeuristicMode.Euclidean)
			: base(grid, startPos, endPos, diagonalMovement, mode)
			=> Weight = weight;

		public AStarParam(BaseGrid grid, float weight, DiagonalMovement diagonalMovement = DiagonalMovement.Always, HeuristicMode mode = HeuristicMode.Euclidean)
			: base(grid, diagonalMovement, mode)
			=> Weight = weight;

		internal override void ResetInternal(GridPos startPos, GridPos endPos, BaseGrid searchGrid = null) { }
	}

	public static class AStarFinder
	{
		/*
		private class NodeComparer : IComparer<Node>
		{
			public int Compare(Node x, Node y)
			{
				var result = (x.heuristicStartToEndLen - y.heuristicStartToEndLen);
				if (result < 0) return -1;
				else
				if (result > 0) return 1;
				else
				{
					return 0;
				}
			}
		}
		*/

		public static List<GridPos> FindPath(AStarParam asParam)
		{
			var lo = new object();
			//var openList = new IntervalHeap<Node>(new NodeComparer());
			var openList = new IntervalHeap<Node>();
			var startNode = asParam.StartNode;
			var endNode = asParam.EndNode;
			var heuristic = asParam.HeuristicFunc;
			var grid = asParam.SearchGrid;
			var diagonalMovement = asParam.DiagonalMovement;
			var weight = asParam.Weight;

			startNode.StartToCurNodeLen = 0;
			startNode.HeuristicStartToEndLen = 0;

			openList.Add(startNode);
			startNode.IsOpened = true;

			while (openList.Count != 0)
			{
				var node = openList.DeleteMin();
				node.IsClosed = true;

				if (node == endNode)
				{
					return Node.Backtrace(endNode);
				}

				var neighbors = grid.GetNeighbors(node, diagonalMovement);

#if UNITY
                foreach(var neighbor in neighbors)
#else
				Parallel.ForEach(neighbors, neighbor =>
#endif
				{
#if UNITY
                    if (neighbor.isClosed) continue;
#else
					if (neighbor.IsClosed)
					{
						return;
					}
#endif
					var x = neighbor.X;
					var y = neighbor.Y;
					var ng = node.StartToCurNodeLen + (float)((x - node.X == 0 || y - node.Y == 0) ? 1 : Math.Sqrt(2));

					if (!neighbor.IsOpened || ng < neighbor.StartToCurNodeLen)
					{
						neighbor.StartToCurNodeLen = ng;
						if (neighbor.HeuristicCurNodeToEndLen == null)
						{
							neighbor.HeuristicCurNodeToEndLen = weight * heuristic(Math.Abs(x - endNode.X), Math.Abs(y - endNode.Y));
						}

						neighbor.HeuristicStartToEndLen = neighbor.StartToCurNodeLen + neighbor.HeuristicCurNodeToEndLen.Value;
						neighbor.Parent = node;
						if (!neighbor.IsOpened)
						{
							lock (lo)
							{
								openList.Add(neighbor);
							}
							neighbor.IsOpened = true;
						}
						else
						{
						}
					}
				}
#if !UNITY
				);
#endif
			}
			return new List<GridPos>();
		}
	}
}
