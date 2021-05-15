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
			for (var widthTrav = 0; widthTrav < width; widthTrav++)
			{
				rectangles[widthTrav] = new GridBox[height];
				for (var heightTrav = 0; heightTrav < height; heightTrav++)
				{
					rectangles[widthTrav][heightTrav] = widthTrav == (width / 3) && heightTrav == (height / 2)
						? new GridBox(widthTrav * 20, (heightTrav * 20) + 50, BoxType.Start)
						: widthTrav == 41 && heightTrav == (height / 2)
							? new GridBox(widthTrav * 20, (heightTrav * 20) + 50, BoxType.End)
							: new GridBox(widthTrav * 20, (heightTrav * 20) + 50, BoxType.Normal);
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

			for (var widthTrav = 0; widthTrav < width; widthTrav++)
			{
				for (var heightTrav = 0; heightTrav < height; heightTrav++)
				{
					rectangles[widthTrav][heightTrav].DrawBox(paper, BoxType.Normal);
				}
			}

			for (var resultTrav = 0; resultTrav < resultBox.Count; resultTrav++)
			{
				resultBox[resultTrav].DrawBox(paper);
			}

			for (var widthTrav = 0; widthTrav < width; widthTrav++)
			{
				for (var heightTrav = 0; heightTrav < height; heightTrav++)
				{
					rectangles[widthTrav][heightTrav].DrawBox(paper, BoxType.Start);
					rectangles[widthTrav][heightTrav].DrawBox(paper, BoxType.End);
					rectangles[widthTrav][heightTrav].DrawBox(paper, BoxType.Wall);
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
					for (var widthTrav = 0; widthTrav < width; widthTrav++)
					{
						for (var heightTrav = 0; heightTrav < height; heightTrav++)
						{
							if (rectangles[widthTrav][heightTrav].BoxRect.IntersectsWith(new Rectangle(e.Location, new Size(1, 1))))
							{
								lastBoxType = rectangles[widthTrav][heightTrav].BoxType;
								lastBoxSelect = rectangles[widthTrav][heightTrav];
								switch (lastBoxType)
								{
									case BoxType.Normal:
									case BoxType.Wall:
										rectangles[widthTrav][heightTrav].SwitchBox();
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
					for (var widthTrav = 0; widthTrav < width; widthTrav++)
					{
						for (var heightTrav = 0; heightTrav < height; heightTrav++)
						{
							if (rectangles[widthTrav][heightTrav].BoxRect.IntersectsWith(new Rectangle(e.Location, new Size(1, 1))))
							{
								if (rectangles[widthTrav][heightTrav] == lastBoxSelect)
								{
									return;
								}
								else
								{
									switch (lastBoxType)
									{
										case BoxType.Normal:
										case BoxType.Wall:
											if (rectangles[widthTrav][heightTrav].BoxType == lastBoxType)
											{
												rectangles[widthTrav][heightTrav].SwitchBox();
												lastBoxSelect = rectangles[widthTrav][heightTrav];
												Invalidate();
											}
											break;
										case BoxType.Start:
											lastBoxSelect.SetNormalBox();
											lastBoxSelect = rectangles[widthTrav][heightTrav];
											lastBoxSelect.SetStartBox();
											Invalidate();
											break;
										case BoxType.End:
											lastBoxSelect.SetNormalBox();
											lastBoxSelect = rectangles[widthTrav][heightTrav];
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
				for (var widthTrav = 0; widthTrav < width; widthTrav++)
				{
					for (var heightTrav = 0; heightTrav < height; heightTrav++)
					{
						if (rectangles[widthTrav][heightTrav].BoxRect.IntersectsWith(new Rectangle(e.Location, new Size(1, 1))))
						{
							lastBoxType = rectangles[widthTrav][heightTrav].BoxType;
							lastBoxSelect = rectangles[widthTrav][heightTrav];
							switch (lastBoxType)
							{
								case BoxType.Normal:
								case BoxType.Wall:
									rectangles[widthTrav][heightTrav].SwitchBox();
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
			for (var widthTrav = 0; widthTrav < width; widthTrav++)
			{
				for (var heightTrav = 0; heightTrav < height; heightTrav++)
				{
					_ = rectangles[widthTrav][heightTrav].BoxType != BoxType.Wall
						? searchGrid.SetWalkableAt(new GridPos(widthTrav, heightTrav), true)
						: searchGrid.SetWalkableAt(new GridPos(widthTrav, heightTrav), false);

					if (rectangles[widthTrav][heightTrav].BoxType == BoxType.Start)
					{
						startPos.X = widthTrav;
						startPos.Y = heightTrav;
					}

					if (rectangles[widthTrav][heightTrav].BoxType == BoxType.End)
					{
						endPos.X = widthTrav;
						endPos.Y = heightTrav;
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

			for (var widthTrav = 0; widthTrav < jumpParam.SearchGrid.Width; widthTrav++)
			{
				for (var heightTrav = 0; heightTrav < jumpParam.SearchGrid.Height; heightTrav++)
				{
					if (jumpParam.SearchGrid.GetNodeAt(widthTrav, heightTrav) == null)
					{
						continue;
					}

					if (jumpParam.SearchGrid.GetNodeAt(widthTrav, heightTrav).IsOpened)
					{
						var resultBox = new ResultBox(widthTrav * 20, (heightTrav * 20) + 50, ResultBoxType.Opened);
						this.resultBox.Add(resultBox);
					}
					if (jumpParam.SearchGrid.GetNodeAt(widthTrav, heightTrav).IsClosed)
					{
						var resultBox = new ResultBox(widthTrav * 20, (heightTrav * 20) + 50, ResultBoxType.Closed);
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
			for (var widthTrav = 0; widthTrav < width; widthTrav++)
			{
				for (var heightTrav = 0; heightTrav < height; heightTrav++)
				{
					switch (rectangles[widthTrav][heightTrav].BoxType)
					{
						case BoxType.Normal:
						case BoxType.Start:
						case BoxType.End:
							break;
						case BoxType.Wall:
							rectangles[widthTrav][heightTrav].SetNormalBox();
							break;
					}
				}
			}
			Invalidate();
		}
	}
}
