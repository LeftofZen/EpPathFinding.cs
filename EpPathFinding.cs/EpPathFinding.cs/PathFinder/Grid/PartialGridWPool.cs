/*!
@file PartialGridWPool.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief PartialGrid with Pool Interface
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

An Interface for the PartialGrid with Pool Class.

*/

namespace EpPathFinding.cs
{
	public class PartialGridWPool : BaseGrid
	{
		private readonly NodePool nodePool;

		public override int Width
		{
			get => gridRect.Right - gridRect.Left + 1;
			protected set { }
		}

		public override int Height
		{
			get => gridRect.Bottom - gridRect.Top + 1;
			protected set { }
		}

		public PartialGridWPool(NodePool _nodePool, GridRect _gridRect = null)
		{
			gridRect = _gridRect ?? new GridRect();
			nodePool = _nodePool;
		}

		public PartialGridWPool(PartialGridWPool b)
			: base(b)
			=> nodePool = b.nodePool;

		public void SetGridRect(GridRect _gridRect)
			=> gridRect = _gridRect;

		public bool IsInside(int x, int y)
			=> x >= gridRect.Left && x <= gridRect.Right && y >= gridRect.Top && y <= gridRect.Bottom;

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

		public override bool SetWalkableAt(int x, int y, bool walkable)
		{
			if (!IsInside(x, y))
			{
				return false;
			}

			var pos = new GridPos(x, y);
			_ = nodePool.SetNode(pos, walkable);
			return true;
		}

		public bool IsInside(GridPos pos)
			=> IsInside(pos.X, pos.Y);

		public override Node GetNodeAt(GridPos pos)
			=> !IsInside(pos) ? null : nodePool.GetNode(pos);

		public override bool IsWalkableAt(GridPos pos)
			=> IsInside(pos) && nodePool.Nodes.ContainsKey(pos);

		public override bool SetWalkableAt(GridPos pos, bool walkable)
			=> SetWalkableAt(pos.X, pos.Y, walkable);

		public override void Reset()
		{
			var rectCount = (gridRect.Right - gridRect.Left) * (gridRect.Bottom - gridRect.Top);
			if (nodePool.Nodes.Count > rectCount)
			{
				var travPos = new GridPos(0, 0);
				for (var xTrav = gridRect.Left; xTrav <= gridRect.Right; xTrav++)
				{
					travPos.X = xTrav;
					for (var yTrav = gridRect.Top; yTrav <= gridRect.Bottom; yTrav++)
					{
						travPos.Y = yTrav;
						var curNode = nodePool.GetNode(travPos);
						curNode?.Reset();
					}
				}
			}
			else
			{
				foreach (var keyValue in nodePool.Nodes)
				{
					keyValue.Value.Reset();
				}
			}
		}

		public override BaseGrid Clone()
			=> new PartialGridWPool(nodePool, gridRect);
	}
}