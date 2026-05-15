/*!
@file DynamicGridWPool.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief DynamicGrid with Pool Interface
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

An Interface for the DynamicGrid with Pool Class.

*/

namespace EpPathFinding.cs
{
	public class DynamicGridWPool : BaseGrid
	{
		private bool notSet;
		private readonly NodePool nodePool;

		public override int Width
		{
			get
			{
				if (notSet)
				{
					SetBoundingBox();
				}

				return gridRect.Right - gridRect.Left + 1;
			}
			protected set { }
		}

		public override int Height
		{
			get
			{
				if (notSet)
				{
					SetBoundingBox();
				}

				return gridRect.Bottom - gridRect.Top + 1;
			}
			protected set { }
		}

		public DynamicGridWPool(NodePool _nodePool)
		{
			gridRect = new GridRect
			{
				Left = 0,
				Top = 0,
				Right = 0,
				Bottom = 0
			};
			notSet = true;
			nodePool = _nodePool;
		}

		public DynamicGridWPool(DynamicGridWPool b)
			: base(b)
		{
			notSet = b.notSet;
			nodePool = b.nodePool;
		}

		public override Node GetNodeAt(int x, int y)
		{
			var pos = new GridPos(x, y);
			return GetNodeAt(pos);
		}

		public override bool IsWalkableAt(int x, int y)
		{
			var pos = new GridPos(x, y);
			return IsWalkableAt(pos);
		}

		private void SetBoundingBox()
		{
			notSet = true;
			foreach (var pair in nodePool.Nodes)
			{
				if (pair.Key.X < gridRect.Left || notSet)
				{
					gridRect.Left = pair.Key.X;
				}

				if (pair.Key.X > gridRect.Right || notSet)
				{
					gridRect.Right = pair.Key.X;
				}

				if (pair.Key.Y < gridRect.Top || notSet)
				{
					gridRect.Top = pair.Key.Y;
				}

				if (pair.Key.Y > gridRect.Bottom || notSet)
				{
					gridRect.Bottom = pair.Key.Y;
				}

				notSet = false;
			}
			notSet = false;
		}

		public override bool SetWalkableAt(int x, int y, bool walkable)
		{
			var pos = new GridPos(x, y);
			_ = nodePool.SetNode(pos, walkable);

			if (walkable)
			{
				if (x < gridRect.Left || notSet)
				{
					gridRect.Left = x;
				}

				if (x > gridRect.Right || notSet)
				{
					gridRect.Right = x;
				}

				if (y < gridRect.Top || notSet)
				{
					gridRect.Top = y;
				}

				if (y > gridRect.Bottom || notSet)
				{
					gridRect.Bottom = y;
				}
			}
			else
			{
				if (x == gridRect.Left || x == gridRect.Right || y == gridRect.Top || y == gridRect.Bottom)
				{
					notSet = true;
				}
			}
			return true;
		}

		public override Node GetNodeAt(GridPos pos)
			=> nodePool.GetNode(pos);

		public override bool IsWalkableAt(GridPos pos)
			=> nodePool.Nodes.ContainsKey(pos);

		public override bool SetWalkableAt(GridPos pos, bool walkable)
			=> SetWalkableAt(pos.X, pos.Y, walkable);
		public override BaseGrid Clone()
			=> new DynamicGridWPool(nodePool);

		public override void Reset()
		{
			foreach (var keyValue in nodePool.Nodes)
			{
				keyValue.Value.Reset();
			}
		}
	}
}