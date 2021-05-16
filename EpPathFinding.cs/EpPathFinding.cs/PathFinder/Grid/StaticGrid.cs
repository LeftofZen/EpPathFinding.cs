/*!
@file StaticGrid.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief StaticGrid Interface
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

An Interface for the StaticGrid Class.

*/

namespace EpPathFinding.cs
{
	public class StaticGrid : BaseGrid
	{
		public override int Width { get; protected set; }

		public override int Height { get; protected set; }

		private Node[][] Nodes;

		public StaticGrid(int width, int height, bool[][] matrix = null)
		{
			Width = width;
			Height = height;
			gridRect.Left = 0;
			gridRect.Top = 0;
			gridRect.Right = width - 1;
			gridRect.Bottom = height - 1;
			Nodes = BuildNodes(width, height, matrix);
		}

		public StaticGrid(StaticGrid b)
			: base(b)
		{
			var matrix = new bool[b.Width][];

			for (var x = 0; x < b.Width; x++)
			{
				matrix[x] = new bool[b.Height];
				for (var y = 0; y < b.Height; y++)
				{
					matrix[x][y] = b.IsWalkableAt(x, y);
				}
			}

			Nodes = BuildNodes(b.Width, b.Height, matrix);
		}

		private Node[][] BuildNodes(int width, int height, bool[][] matrix)
		{
			var nodes = new Node[width][];

			for (var x = 0; x < width; x++)
			{
				nodes[x] = new Node[height];
				for (var y = 0; y < height; y++)
				{
					nodes[x][y] = new Node(x, y);
				}
			}

			if (matrix == null)
			{
				return nodes;
			}

			if (matrix.Length != width || matrix[0].Length != height)
			{
				throw new System.Exception("Matrix size does not fit");
			}

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					nodes[x][y].Walkable = matrix[x][y];
				}
			}

			return nodes;
		}

		public override Node GetNodeAt(int x, int y)
			=> Nodes[x][y];

		public override bool IsWalkableAt(int x, int y)
			=> IsInside(x, y) && Nodes[x][y].Walkable;

		protected bool IsInside(int x, int y)
			=> x >= 0 && x < Width && y >= 0 && y < Height;

		public override bool SetWalkableAt(int x, int y, bool walkable)
		{
			Nodes[x][y].Walkable = walkable;
			return true;
		}

		protected bool IsInside(GridPos pos)
			=> IsInside(pos.X, pos.Y);

		public override Node GetNodeAt(GridPos pos)
			=> GetNodeAt(pos.X, pos.Y);

		public override bool IsWalkableAt(GridPos pos)
			=> IsWalkableAt(pos.X, pos.Y);

		public override bool SetWalkableAt(GridPos pos, bool walkable)
			=> SetWalkableAt(pos.X, pos.Y, walkable);

		public override void Reset()
			=> Reset(null);

		public void Reset(bool[][] matrix)
		{
			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					Nodes[x][y].Reset();
				}
			}

			if (matrix == null)
			{
				return;
			}

			if (matrix.Length != Width || matrix[0].Length != Height)
			{
				throw new System.Exception("Matrix size does not fit");
			}

			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					Nodes[x][y].Walkable = matrix[x][y];
				}
			}
		}

		public override BaseGrid Clone()
		{
			var width = Width;
			var height = Height;
			var nodes = Nodes;
			var newGrid = new StaticGrid(width, height);
			var newNodes = new Node[width][];

			for (var x = 0; x < width; x++)
			{
				newNodes[x] = new Node[height];
				for (var y = 0; y < height; y++)
				{
					newNodes[x][y] = new Node(x, y, nodes[x][y].Walkable);
				}
			}

			newGrid.Nodes = newNodes;
			return newGrid;
		}
	}
}
