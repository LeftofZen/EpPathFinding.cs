/*!
@file SearchGridForm.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding>
@date July 16, 2013
@brief SearchGridForm Interface
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

An Interface for the SearchGridForm Class.

*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EpPathFinding.cs;

namespace EpPathFindingDemo
{
	public partial class SearchGridForm : Form
	{
		private const int width = 64;
		private const int height = 32;
		private Graphics paper;

		private readonly GridBox[][] rectangles;
		private readonly List<ResultBox> resultBox;
		private readonly List<GridLine> resultLine;

		private GridBox lastBoxSelect;
		private BoxType lastBoxType;

		private readonly BaseGrid searchGrid;
		private readonly JumpPointParam jumpParam;
		public SearchGridForm()
		{
			InitializeComponent();
			DoubleBuffered = true;

			resultBox = new List<ResultBox>();
			Width = (width + 1) * 20;
			Height = ((height + 1) * 20) + 100;
			MaximumSize = new Size(Width, Height);
			MaximizeBox = false;

			rectangles = new GridBox[width][];
			for (var x = 0; x < width; x++)
			{
				rectangles[x] = new GridBox[height];
				for (var y = 0; y < height; y++)
				{
					rectangles[x][y] = x == (width / 3) && y == (height / 2)
						? new GridBox(x * 20, (y * 20) + 50, BoxType.Start)
						: x == 41 && y == (height / 2)
							? new GridBox(x * 20, (y * 20) + 50, BoxType.End)
							: new GridBox(x * 20, (y * 20) + 50, BoxType.Normal);
				}
			}
			_ = cbbJumpType.Items.Add("Always");
			_ = cbbJumpType.Items.Add("Never");
			_ = cbbJumpType.Items.Add("IfAtLeastOneWalkable");
			_ = cbbJumpType.Items.Add("OnlyWhenNoObstacles");
			cbbJumpType.SelectedIndex = 0;

			resultLine = new List<GridLine>();

			searchGrid = new StaticGrid(width, height);
			// searchGrid = new DynamicGrid();
			//searchGrid = new DynamicGridWPool(SingletonHolder<NodePool>.Instance);

			jumpParam = new JumpPointParam(searchGrid, EndNodeUnWalkableTreatment.Allow, (DiagonalMovement)cbbJumpType.SelectedIndex, HeuristicMode.Euclidean)
			{
				CurIterationType = cbUseRecursive.Checked ? IterationType.Recursive : IterationType.Loop
			};//new JumpPointParam(searchGrid, startPos, endPos, cbCrossCorners.Checked, HeuristicMode.EUCLIDEANSQR);
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			paper = e.Graphics;
			//Draw

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					rectangles[x][y].DrawBox(paper, BoxType.Normal);
				}
			}

			for (var resultTrav = 0; resultTrav < resultBox.Count; resultTrav++)
			{
				resultBox[resultTrav].DrawBox(paper);
			}

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					rectangles[x][y].DrawBox(paper, BoxType.Start);
					rectangles[x][y].DrawBox(paper, BoxType.End);
					rectangles[x][y].DrawBox(paper, BoxType.Wall);
				}
			}

			for (var resultTrav = 0; resultTrav < resultLine.Count; resultTrav++)
			{
				resultLine[resultTrav].DrawLine(paper);
			}
		}

		private void Form1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				lastBoxSelect = null;
			}
		}

		private void Form1_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (lastBoxSelect == null)
				{
					for (var x = 0; x < width; x++)
					{
						for (var y = 0; y < height; y++)
						{
							if (rectangles[x][y].BoxRect.Contains(e.Location))
							{
								lastBoxType = rectangles[x][y].BoxType;
								lastBoxSelect = rectangles[x][y];
								switch (lastBoxType)
								{
									case BoxType.Normal:
									case BoxType.Wall:
										rectangles[x][y].SwitchBox();
										Invalidate();
										break;
									case BoxType.Start:
									case BoxType.End:

										break;
								}
							}
						}
					}

					return;
				}
				else
				{
					for (var x = 0; x < width; x++)
					{
						for (var y = 0; y < height; y++)
						{
							if (rectangles[x][y].BoxRect.Contains(e.Location))
							{
								if (rectangles[x][y] == lastBoxSelect)
								{
									return;
								}
								else
								{
									switch (lastBoxType)
									{
										case BoxType.Normal:
										case BoxType.Wall:
											if (rectangles[x][y].BoxType == lastBoxType)
											{
												rectangles[x][y].SwitchBox();
												lastBoxSelect = rectangles[x][y];
												Invalidate();
											}
											break;
										case BoxType.Start:
											lastBoxSelect.SetNormalBox();
											lastBoxSelect = rectangles[x][y];
											lastBoxSelect.SetStartBox();
											Invalidate();
											break;
										case BoxType.End:
											lastBoxSelect.SetNormalBox();
											lastBoxSelect = rectangles[x][y];
											lastBoxSelect.SetEndBox();
											Invalidate();
											break;
									}
								}
							}
						}
					}
				}
			}
		}

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				for (var x = 0; x < width; x++)
				{
					for (var y = 0; y < height; y++)
					{
						if (rectangles[x][y].BoxRect.Contains(e.Location))
						{
							lastBoxType = rectangles[x][y].BoxType;
							lastBoxSelect = rectangles[x][y];
							switch (lastBoxType)
							{
								case BoxType.Normal:
								case BoxType.Wall:
									rectangles[x][y].SwitchBox();
									Invalidate();
									break;
								case BoxType.Start:
								case BoxType.End:

									break;
							}
						}
					}
				}
			}
		}

		private void btnSearch_Click(object sender, EventArgs e)
		{
			for (var resultTrav = 0; resultTrav < resultLine.Count; resultTrav++)
			{
				resultLine[resultTrav].Dispose();
			}
			resultLine.Clear();
			for (var resultTrav = 0; resultTrav < resultBox.Count; resultTrav++)
			{
				resultBox[resultTrav].Dispose();
			}
			resultBox.Clear();

			var startPos = new GridPos();
			var endPos = new GridPos();
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					_ = rectangles[x][y].BoxType != BoxType.Wall
						? searchGrid.SetWalkableAt(x, y, true)
						: searchGrid.SetWalkableAt(x, y, false);

					if (rectangles[x][y].BoxType == BoxType.Start)
					{
						startPos.X = x;
						startPos.Y = y;
					}

					if (rectangles[x][y].BoxType == BoxType.End)
					{
						endPos.X = x;
						endPos.Y = y;
					}
				}
			}

			jumpParam.DiagonalMovement = (DiagonalMovement)cbbJumpType.SelectedIndex;
			jumpParam.CurIterationType = cbUseRecursive.Checked ? IterationType.Recursive : IterationType.Loop;
			jumpParam.Reset(startPos, endPos);
			var resultList = JumpPointFinder.FindPath(jumpParam);

			for (var resultTrav = 0; resultTrav < resultList.Count - 1; resultTrav++)
			{
				resultLine.Add(new GridLine(rectangles[resultList[resultTrav].X][resultList[resultTrav].Y], rectangles[resultList[resultTrav + 1].X][resultList[resultTrav + 1].Y]));
			}

			for (var x = 0; x < jumpParam.SearchGrid.Width; x++)
			{
				for (var y = 0; y < jumpParam.SearchGrid.Height; y++)
				{
					if (jumpParam.SearchGrid.GetNodeAt(x, y) == null)
					{
						continue;
					}

					if (jumpParam.SearchGrid.GetNodeAt(x, y).IsOpened)
					{
						var resultBox = new ResultBox(x * 20, (y * 20) + 50, ResultBoxType.Opened);
						this.resultBox.Add(resultBox);
					}
					if (jumpParam.SearchGrid.GetNodeAt(x, y).IsClosed)
					{
						var resultBox = new ResultBox(x * 20, (y * 20) + 50, ResultBoxType.Closed);
						this.resultBox.Add(resultBox);
					}
				}
			}
			Invalidate();
		}

		private void btnClearPath_Click(object sender, EventArgs e)
		{
			for (var resultTrav = 0; resultTrav < resultLine.Count; resultTrav++)
			{
				resultLine[resultTrav].Dispose();
			}
			resultLine.Clear();

			for (var resultTrav = 0; resultTrav < resultBox.Count; resultTrav++)
			{
				resultBox[resultTrav].Dispose();
			}

			resultBox.Clear();
			Invalidate();
		}

		private void btnClearWall_Click(object sender, EventArgs e)
		{
			for (var resultTrav = 0; resultTrav < resultLine.Count; resultTrav++)
			{
				resultLine[resultTrav].Dispose();
			}
			resultLine.Clear();

			for (var resultTrav = 0; resultTrav < resultBox.Count; resultTrav++)
			{
				resultBox[resultTrav].Dispose();
			}

			resultBox.Clear();
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					switch (rectangles[x][y].BoxType)
					{
						case BoxType.Normal:
						case BoxType.Start:
						case BoxType.End:
							break;
						case BoxType.Wall:
							rectangles[x][y].SetNormalBox();
							break;
					}
				}
			}
			Invalidate();
		}
	}
}
