/*!
@file DynamicGrid.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief DynamicGrid Interface
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

An Interface for the DynamicGrid Class.

*/

using System.Collections.Generic;

namespace EpPathFinding.cs
{
	public class DynamicGrid : BaseGrid
	{
		protected Dictionary<GridPos, Node> nodes;
		private bool notSet;

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
			protected set
			{
			}
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

		public DynamicGrid(List<GridPos> walkableGridList = null)
		{
			gridRect = new GridRect
			{
				Left = 0,
				Top = 0,
				Right = 0,
				Bottom = 0
			};
			notSet = true;
			BuildNodes(walkableGridList);
		}

		public DynamicGrid(DynamicGrid b)
			: base(b)
		{
			notSet = b.notSet;
			nodes = new Dictionary<GridPos, Node>(b.nodes);
		}

		protected void BuildNodes(List<GridPos> walkableGridList)
		{
			nodes = new Dictionary<GridPos, Node>();
			if (walkableGridList == null)
			{
				return;
			}

			foreach (var gridPos in walkableGridList)
			{
				_ = SetWalkableAt(gridPos.X, gridPos.Y, true);
			}
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
			foreach (var pair in nodes)
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

			if (walkable)
			{
				if (nodes.ContainsKey(pos))
				{
					return true;
				}
				else
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

					nodes.Add(new GridPos(pos.X, pos.Y), new Node(pos.X, pos.Y, walkable));
				}
			}
			else
			{
				if (nodes.ContainsKey(pos))
				{
					_ = nodes.Remove(pos);
					if (x == gridRect.Left || x == gridRect.Right || y == gridRect.Top || y == gridRect.Bottom)
					{
						notSet = true;
					}
				}
			}

			return true;
		}

		public override Node GetNodeAt(GridPos pos)
			=> nodes.ContainsKey(pos) ? nodes[pos] : null;

		public override bool IsWalkableAt(GridPos pos)
			=> nodes.ContainsKey(pos);

		public override bool SetWalkableAt(GridPos pos, bool walkable)
			=> SetWalkableAt(pos.X, pos.Y, walkable);

		public override void Reset()
			=> Reset(null);

		public void Reset(List<GridPos> walkableGridList)
		{
			foreach (var keyValue in nodes)
			{
				keyValue.Value.Reset();
			}

			if (walkableGridList == null)
			{
				return;
			}

			foreach (var keyValue in nodes)
			{
				_ = walkableGridList.Contains(keyValue.Key)
					? SetWalkableAt(keyValue.Key, true)
					: SetWalkableAt(keyValue.Key, false);
			}
		}

		public override BaseGrid Clone()
		{
			var tNewGrid = new DynamicGrid();

			foreach (var keyValue in nodes)
			{
				_ = tNewGrid.SetWalkableAt(keyValue.Key.X, keyValue.Key.Y, true);
			}

			return tNewGrid;
		}
	}
}
