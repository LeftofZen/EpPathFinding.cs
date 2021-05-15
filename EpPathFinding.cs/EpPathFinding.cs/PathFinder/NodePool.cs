/*!
@file NodePool.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief NodePool Interface
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

An Interface for the NodePool Class.

*/
using System.Collections.Generic;

namespace EpPathFinding.cs
{
	public class NodePool
	{
		protected Dictionary<GridPos, Node> nodes;

		public NodePool()
			=> nodes = new Dictionary<GridPos, Node>();

		public Dictionary<GridPos, Node> Nodes
			=> nodes;

		public Node GetNode(int x, int y)
			=> GetNode(new GridPos(x, y));

		public Node GetNode(GridPos pos)
		{
			_ = nodes.TryGetValue(pos, out var outNode);
			return outNode;
		}

		public Node SetNode(int x, int y, bool walkable = false)
			=> SetNode(new GridPos(x, y), walkable);

		public Node SetNode(GridPos pos, bool walkable = false)
		{
			if (walkable)
			{
				if (nodes.TryGetValue(pos, out var retVal))
				{
					return retVal;
				}
				var newNode = new Node(pos.X, pos.Y, walkable);
				nodes.Add(pos, newNode);
				return newNode;
			}
			else
			{
				RemoveNode(pos);
				return null;
			}
		}

		protected void RemoveNode(int x, int y)
			=> RemoveNode(new GridPos(x, y));

		protected void RemoveNode(GridPos pos)
		{
			if (nodes.ContainsKey(pos))
			{
				_ = nodes.Remove(pos);
			}
		}
	}
}