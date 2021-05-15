/*!
@file JumpPointFinder.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief Node Interface
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

using System;
using System.Collections.Generic;

namespace EpPathFinding.cs
{
	public class Node : IComparable<Node>
	{
		public int X;
		public int Y;
		public bool Walkable;
		public float HeuristicStartToEndLen; // which passes current node
		public float StartToCurNodeLen;
		public float? HeuristicCurNodeToEndLen;
		public bool IsOpened;
		public bool IsClosed;
		public object Parent;

		public Node(int x, int y, bool walkable = false)
		{
			X = x;
			Y = y;
			Walkable = walkable;
			HeuristicStartToEndLen = 0;
			StartToCurNodeLen = 0;
			// this must be initialized as null to verify that its value never initialized
			// 0 is not good candidate!!
			HeuristicCurNodeToEndLen = null;
			IsOpened = false;
			IsClosed = false;
			Parent = null;
		}

		public Node(Node b)
		{
			X = b.X;
			Y = b.Y;
			Walkable = b.Walkable;
			HeuristicStartToEndLen = b.HeuristicStartToEndLen;
			StartToCurNodeLen = b.StartToCurNodeLen;
			HeuristicCurNodeToEndLen = b.HeuristicCurNodeToEndLen;
			IsOpened = b.IsOpened;
			IsClosed = b.IsClosed;
			Parent = b.Parent;
		}

		public void Reset(bool walkable = false)
		{
			Walkable = walkable;
			HeuristicStartToEndLen = 0;
			StartToCurNodeLen = 0;
			// this must be initialized as null to verify that its value never initialized
			// 0 is not good candidate!!
			HeuristicCurNodeToEndLen = null;
			IsOpened = false;
			IsClosed = false;
			Parent = null;
		}

		public int CompareTo(Node other)
		{
			var result = HeuristicStartToEndLen - other.HeuristicStartToEndLen;
			if (result > 0.0f)
			{
				return 1;
			}
			else if (result == 0.0f)
			{
				return 0;
			}

			return -1;
		}

		public static List<GridPos> Backtrace(Node node)
		{
			var path = new List<GridPos>
			{
				new GridPos(node.X, node.Y)
			};

			while (node.Parent != null)
			{
				node = (Node)node.Parent;
				path.Add(new GridPos(node.X, node.Y));
			}
			path.Reverse();

			return path;
		}

		public override int GetHashCode()
			=> X ^ Y;

		public override bool Equals(object obj)
			=> obj != null && obj is Node node && X == node.X && Y == node.Y;

		public bool Equals(Node node)
			=> node != null && (X == node.X) && (Y == node.Y);

		public static bool operator ==(Node a, Node b)
			=> ReferenceEquals(a, b) || (!(a is null) && !(b is null) && a.X == b.X && a.Y == b.Y);

		public static bool operator !=(Node a, Node b)
			=> !(a == b);
	}
}
