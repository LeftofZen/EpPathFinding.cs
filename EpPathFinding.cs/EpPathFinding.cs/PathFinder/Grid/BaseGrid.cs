/*!
@file BaseGrid.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief BaseGrid Interface
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

An Interface for the BaseGrid Class.

*/

using System.Collections.Generic;

namespace EpPathFinding.cs
{
	public abstract class BaseGrid
	{
		protected BaseGrid()
			=> gridRect = new GridRect();

		protected BaseGrid(BaseGrid b)
		{
			gridRect = new GridRect(b.gridRect);
			Width = b.Width;
			Height = b.Height;
		}

		protected GridRect gridRect;
		public GridRect GridRect
			=> gridRect;

		public abstract int Width { get; protected set; }

		public abstract int Height { get; protected set; }

		public abstract Node GetNodeAt(int x, int y);

		public abstract bool IsWalkableAt(int x, int y);

		public abstract bool SetWalkableAt(int x, int y, bool walkable);

		public abstract Node GetNodeAt(GridPos pos);

		public abstract bool IsWalkableAt(GridPos pos);

		public abstract bool SetWalkableAt(GridPos pos, bool walkable);

		public List<Node> GetNeighbors(Node node, DiagonalMovement diagonalMovement)
		{
			var tX = node.X;
			var tY = node.Y;
			var neighbors = new List<Node>();

			bool tS0 = false, tD0 = false,
				 tS1 = false, tD1 = false,
				 tS2 = false, tD2 = false,
				 tS3 = false, tD3 = false;

			var pos = new GridPos();
			if (IsWalkableAt(pos.SetAndReturn(tX, tY - 1)))
			{
				neighbors.Add(GetNodeAt(pos));
				tS0 = true;
			}
			if (IsWalkableAt(pos.SetAndReturn(tX + 1, tY)))
			{
				neighbors.Add(GetNodeAt(pos));
				tS1 = true;
			}
			if (IsWalkableAt(pos.SetAndReturn(tX, tY + 1)))
			{
				neighbors.Add(GetNodeAt(pos));
				tS2 = true;
			}
			if (IsWalkableAt(pos.SetAndReturn(tX - 1, tY)))
			{
				neighbors.Add(GetNodeAt(pos));
				tS3 = true;
			}

			switch (diagonalMovement)
			{
				case DiagonalMovement.Always:
					tD0 = true;
					tD1 = true;
					tD2 = true;
					tD3 = true;
					break;
				case DiagonalMovement.Never:
					break;
				case DiagonalMovement.IfAtLeastOneWalkable:
					tD0 = tS3 || tS0;
					tD1 = tS0 || tS1;
					tD2 = tS1 || tS2;
					tD3 = tS2 || tS3;
					break;
				case DiagonalMovement.OnlyWhenNoObstacles:
					tD0 = tS3 && tS0;
					tD1 = tS0 && tS1;
					tD2 = tS1 && tS2;
					tD3 = tS2 && tS3;
					break;
			}

			if (tD0 && IsWalkableAt(pos.SetAndReturn(tX - 1, tY - 1)))
			{
				neighbors.Add(GetNodeAt(pos));
			}
			if (tD1 && IsWalkableAt(pos.SetAndReturn(tX + 1, tY - 1)))
			{
				neighbors.Add(GetNodeAt(pos));
			}
			if (tD2 && IsWalkableAt(pos.SetAndReturn(tX + 1, tY + 1)))
			{
				neighbors.Add(GetNodeAt(pos));
			}
			if (tD3 && IsWalkableAt(pos.SetAndReturn(tX - 1, tY + 1)))
			{
				neighbors.Add(GetNodeAt(pos));
			}

			return neighbors;
		}

		public abstract void Reset();

		public abstract BaseGrid Clone();
	}
}