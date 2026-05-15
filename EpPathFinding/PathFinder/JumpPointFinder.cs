/*!
@file JumpPointFinder.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief Jump Point Search Algorithm Interface
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
	public static class JumpPointFinder
	{
		public static List<GridPos> GetFullPath(List<GridPos> routeFound)
		{
			if (routeFound == null)
			{
				return null;
			}

			var consecutiveGridList = new List<GridPos>();
			if (routeFound.Count > 1)
			{
				consecutiveGridList.Add(new GridPos(routeFound[0]));
			}

			for (var routeTrav = 0; routeTrav < routeFound.Count - 1; routeTrav++)
			{
				var fromGrid = new GridPos(routeFound[routeTrav]);
				var toGrid = routeFound[routeTrav + 1];
				var dX = toGrid.X - fromGrid.X;
				var dY = toGrid.Y - fromGrid.Y;

				var nDX = 0;
				var nDY = 0;
				if (dX != 0)
				{
					nDX = dX / Math.Abs(dX);
				}
				if (dY != 0)
				{
					nDY = dY / Math.Abs(dY);
				}

				while (fromGrid != toGrid)
				{
					fromGrid.X += nDX;
					fromGrid.Y += nDY;
					consecutiveGridList.Add(new GridPos(fromGrid));
				}
			}
			return consecutiveGridList;
		}

		public static List<GridPos> FindPath(JumpPointParam jpParam)
		{
			var tOpenList = jpParam.OpenList;
			var tStartNode = jpParam.StartNode;
			var tEndNode = jpParam.EndNode;
			Node tNode;
			var revertEndNodeWalkable = false;

			// set the `g` and `f` value of the start node to be 0
			tStartNode.StartToCurNodeLen = 0;
			tStartNode.HeuristicStartToEndLen = 0;

			// push the start node into the open list
			_ = tOpenList.Add(tStartNode);
			tStartNode.IsOpened = true;

			if (jpParam.CurEndNodeUnWalkableTreatment == EndNodeUnWalkableTreatment.Allow && !jpParam.SearchGrid.IsWalkableAt(tEndNode.X, tEndNode.Y))
			{
				_ = jpParam.SearchGrid.SetWalkableAt(tEndNode.X, tEndNode.Y, true);
				revertEndNodeWalkable = true;
			}

			// while the open list is not empty
			while (tOpenList.Count > 0)
			{
				// pop the position of node which has the minimum `f` value.
				tNode = tOpenList.DeleteMin();
				tNode.IsClosed = true;

				if (tNode.Equals(tEndNode))
				{
					if (revertEndNodeWalkable)
					{
						_ = jpParam.SearchGrid.SetWalkableAt(tEndNode.X, tEndNode.Y, false);
					}
					return Node.Backtrace(tNode); // rebuilding path
				}

				IdentifySuccessors(jpParam, tNode);
			}

			if (revertEndNodeWalkable)
			{
				_ = jpParam.SearchGrid.SetWalkableAt(tEndNode.X, tEndNode.Y, false);
			}

			// fail to find the path
			return new List<GridPos>();
		}

		private static void IdentifySuccessors(JumpPointParam jpParam, Node node)
		{
			var tHeuristic = jpParam.HeuristicFunc;
			var tOpenList = jpParam.OpenList;
			var tEndX = jpParam.EndNode.X;
			var tEndY = jpParam.EndNode.Y;
			GridPos tNeighbor;
			GridPos tJumpPoint;
			Node tJumpNode;

			var tNeighbors = FindNeighbors(jpParam, node);
			for (var i = 0; i < tNeighbors.Count; i++)
			{
				tNeighbor = tNeighbors[i];
				tJumpPoint = jpParam.CurIterationType == IterationType.Recursive
					? Jump(jpParam, tNeighbor.X, tNeighbor.Y, node.X, node.Y)
					: JumpLoop(jpParam, tNeighbor.X, tNeighbor.Y, node.X, node.Y);

				if (tJumpPoint != null)
				{
					tJumpNode = jpParam.SearchGrid.GetNodeAt(tJumpPoint.X, tJumpPoint.Y);
					if (tJumpNode == null && jpParam.EndNode.X == tJumpPoint.X && jpParam.EndNode.Y == tJumpPoint.Y)
					{
						tJumpNode = jpParam.SearchGrid.GetNodeAt(tJumpPoint);
					}

					if (tJumpNode.IsClosed)
					{
						continue;
					}

					// include distance, as parent may not be immediately adjacent:
					var tCurNodeToJumpNodeLen = tHeuristic(Math.Abs(tJumpPoint.X - node.X), Math.Abs(tJumpPoint.Y - node.Y));
					var tStartToJumpNodeLen = node.StartToCurNodeLen + tCurNodeToJumpNodeLen; // next `startToCurNodeLen` value

					if (!tJumpNode.IsOpened || tStartToJumpNodeLen < tJumpNode.StartToCurNodeLen)
					{
						tJumpNode.StartToCurNodeLen = tStartToJumpNodeLen;
						tJumpNode.HeuristicCurNodeToEndLen ??= tHeuristic(Math.Abs(tJumpPoint.X - tEndX), Math.Abs(tJumpPoint.Y - tEndY));
						tJumpNode.HeuristicStartToEndLen = tJumpNode.StartToCurNodeLen + tJumpNode.HeuristicCurNodeToEndLen.Value;
						tJumpNode.Parent = node;

						if (!tJumpNode.IsOpened)
						{
							_ = tOpenList.Add(tJumpNode);
							tJumpNode.IsOpened = true;
						}
					}
				}
			}
		}

		private static GridPos JumpLoop(JumpPointParam jpParam, int x, int y, int px, int py)
		{
			var stack = new Stack<JumpSnapshot>();

			var currentSnapshot = new JumpSnapshot
			{
				X = x,
				Y = y,
				Px = px,
				Py = py,
				Stage = 0
			};

			stack.Push(currentSnapshot);
			GridPos retVal = null;

			while (stack.Count != 0)
			{
				currentSnapshot = stack.Pop();
				JumpSnapshot newSnapshot;
				switch (currentSnapshot.Stage)
				{
					case 0:
						if (!jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y))
						{
							retVal = null;
							continue;
						}
						else if (jpParam.SearchGrid.GetNodeAt(currentSnapshot.X, currentSnapshot.Y).Equals(jpParam.EndNode))
						{
							retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
							continue;
						}

						currentSnapshot.Dx = currentSnapshot.X - currentSnapshot.Px;
						currentSnapshot.Dy = currentSnapshot.Y - currentSnapshot.Py;
						if (jpParam.DiagonalMovement == DiagonalMovement.Always || jpParam.DiagonalMovement == DiagonalMovement.IfAtLeastOneWalkable)
						{
							// check for forced neighbors
							// along the diagonal
							if (currentSnapshot.Dx != 0 && currentSnapshot.Dy != 0)
							{
								if ((jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X - currentSnapshot.Dx, currentSnapshot.Y + currentSnapshot.Dy) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X - currentSnapshot.Dx, currentSnapshot.Y)) ||
									(jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y - currentSnapshot.Dy) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y - currentSnapshot.Dy)))
								{
									retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
									continue;
								}
							}
							// horizontally/vertically
							else
							{
								if (currentSnapshot.Dx != 0)
								{
									// moving along x
									if ((jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y + 1) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y + 1)) ||
										(jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y - 1) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y - 1)))
									{
										retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
										continue;
									}
								}
								else
								{
									if ((jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + 1, currentSnapshot.Y + currentSnapshot.Dy) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + 1, currentSnapshot.Y)) ||
										(jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X - 1, currentSnapshot.Y + currentSnapshot.Dy) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X - 1, currentSnapshot.Y)))
									{
										retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
										continue;
									}
								}
							}
							// when moving diagonally, must check for vertical/horizontal jump points
							if (currentSnapshot.Dx != 0 && currentSnapshot.Dy != 0)
							{
								currentSnapshot.Stage = 1;
								stack.Push(currentSnapshot);

								newSnapshot = new JumpSnapshot
								{
									X = currentSnapshot.X + currentSnapshot.Dx,
									Y = currentSnapshot.Y,
									Px = currentSnapshot.X,
									Py = currentSnapshot.Y,
									Stage = 0
								};
								stack.Push(newSnapshot);
								continue;
							}

							// moving diagonally, must make sure one of the vertical/horizontal
							// neighbors is open to allow the path

							// moving diagonally, must make sure one of the vertical/horizontal
							// neighbors is open to allow the path
							if (jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y) || jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y + currentSnapshot.Dy))
							{
								newSnapshot = new JumpSnapshot
								{
									X = currentSnapshot.X + currentSnapshot.Dx,
									Y = currentSnapshot.Y + currentSnapshot.Dy,
									Px = currentSnapshot.X,
									Py = currentSnapshot.Y,
									Stage = 0
								};
								stack.Push(newSnapshot);
								continue;
							}
							else if (jpParam.DiagonalMovement == DiagonalMovement.Always)
							{
								newSnapshot = new JumpSnapshot
								{
									X = currentSnapshot.X + currentSnapshot.Dx,
									Y = currentSnapshot.Y + currentSnapshot.Dy,
									Px = currentSnapshot.X,
									Py = currentSnapshot.Y,
									Stage = 0
								};
								stack.Push(newSnapshot);
								continue;
							}
						}
						else if (jpParam.DiagonalMovement == DiagonalMovement.OnlyWhenNoObstacles)
						{
							// check for forced neighbors
							// along the diagonal
							if (currentSnapshot.Dx != 0 && currentSnapshot.Dy != 0)
							{
								if ((jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y + currentSnapshot.Dy) && jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y + currentSnapshot.Dy) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y)) ||
									(jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y + currentSnapshot.Dy) && jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y + currentSnapshot.Dy)))
								{
									retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
									continue;
								}
							}
							// horizontally/vertically
							else
							{
								if (currentSnapshot.Dx != 0)
								{
									// moving along x
									if ((jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y + 1) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X - currentSnapshot.Dx, currentSnapshot.Y + 1)) ||
										(jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y - 1) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X - currentSnapshot.Dx, currentSnapshot.Y - 1)))
									{
										retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
										continue;
									}
								}
								else
								{
									if ((jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + 1, currentSnapshot.Y) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + 1, currentSnapshot.Y - currentSnapshot.Dy)) ||
										(jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X - 1, currentSnapshot.Y) && !jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X - 1, currentSnapshot.Y - currentSnapshot.Dy)))
									{
										retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
										continue;
									}
								}
							}

							// when moving diagonally, must check for vertical/horizontal jump points
							if (currentSnapshot.Dx != 0 && currentSnapshot.Dy != 0)
							{
								currentSnapshot.Stage = 3;
								stack.Push(currentSnapshot);

								newSnapshot = new JumpSnapshot
								{
									X = currentSnapshot.X + currentSnapshot.Dx,
									Y = currentSnapshot.Y,
									Px = currentSnapshot.X,
									Py = currentSnapshot.Y,
									Stage = 0
								};
								stack.Push(newSnapshot);
								continue;
							}

							// moving diagonally, must make sure both of the vertical/horizontal
							// neighbors is open to allow the path
							if (jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y) && jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y + currentSnapshot.Dy))
							{
								newSnapshot = new JumpSnapshot
								{
									X = currentSnapshot.X + currentSnapshot.Dx,
									Y = currentSnapshot.Y + currentSnapshot.Dy,
									Px = currentSnapshot.X,
									Py = currentSnapshot.Y,
									Stage = 0
								};
								stack.Push(newSnapshot);
								continue;
							}
						}
						else // if(jpParam.DiagonalMovement == DiagonalMovement.Never)
						{
							if (currentSnapshot.Dx != 0)
							{
								// moving along x
								if (!jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y))
								{
									retVal = new GridPos(x, y);
									continue;
								}
							}
							else
							{
								if (!jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y + currentSnapshot.Dy))
								{
									retVal = new GridPos(x, y);
									continue;
								}
							}

							//  must check for perpendicular jump points
							if (currentSnapshot.Dx != 0)
							{
								currentSnapshot.Stage = 5;
								stack.Push(currentSnapshot);

								newSnapshot = new JumpSnapshot
								{
									X = currentSnapshot.X,
									Y = currentSnapshot.Y + 1,
									Px = currentSnapshot.X,
									Py = currentSnapshot.Y,
									Stage = 0
								};
								stack.Push(newSnapshot);
								continue;
							}
							else // tDy != 0
							{
								currentSnapshot.Stage = 6;
								stack.Push(currentSnapshot);

								newSnapshot = new JumpSnapshot
								{
									X = currentSnapshot.X + 1,
									Y = currentSnapshot.Y,
									Px = currentSnapshot.X,
									Py = currentSnapshot.Y,
									Stage = 0
								};
								stack.Push(newSnapshot);
								continue;
							}
						}
						retVal = null;
						break;
					case 1:
						if (retVal != null)
						{
							retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
							continue;
						}

						currentSnapshot.Stage = 2;
						stack.Push(currentSnapshot);

						newSnapshot = new JumpSnapshot
						{
							X = currentSnapshot.X,
							Y = currentSnapshot.Y + currentSnapshot.Dy,
							Px = currentSnapshot.X,
							Py = currentSnapshot.Y,
							Stage = 0
						};
						stack.Push(newSnapshot);
						break;
					case 2:
						if (retVal != null)
						{
							retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
							continue;
						}

						// moving diagonally, must make sure one of the vertical/horizontal
						// neighbors is open to allow the path
						if (jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y) || jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y + currentSnapshot.Dy))
						{
							newSnapshot = new JumpSnapshot
							{
								X = currentSnapshot.X + currentSnapshot.Dx,
								Y = currentSnapshot.Y + currentSnapshot.Dy,
								Px = currentSnapshot.X,
								Py = currentSnapshot.Y,
								Stage = 0
							};
							stack.Push(newSnapshot);
							continue;
						}
						else if (jpParam.DiagonalMovement == DiagonalMovement.Always)
						{
							newSnapshot = new JumpSnapshot
							{
								X = currentSnapshot.X + currentSnapshot.Dx,
								Y = currentSnapshot.Y + currentSnapshot.Dy,
								Px = currentSnapshot.X,
								Py = currentSnapshot.Y,
								Stage = 0
							};
							stack.Push(newSnapshot);
							continue;
						}
						retVal = null;
						break;
					case 3:
						if (retVal != null)
						{
							retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
							continue;
						}

						currentSnapshot.Stage = 4;
						stack.Push(currentSnapshot);

						newSnapshot = new JumpSnapshot
						{
							X = currentSnapshot.X,
							Y = currentSnapshot.Y + currentSnapshot.Dy,
							Px = currentSnapshot.X,
							Py = currentSnapshot.Y,
							Stage = 0
						};
						stack.Push(newSnapshot);
						break;
					case 4:
						if (retVal != null)
						{
							retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
							continue;
						}

						// moving diagonally, must make sure both of the vertical/horizontal
						// neighbors is open to allow the path
						if (jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y) && jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y + currentSnapshot.Dy))
						{
							newSnapshot = new JumpSnapshot
							{
								X = currentSnapshot.X + currentSnapshot.Dx,
								Y = currentSnapshot.Y + currentSnapshot.Dy,
								Px = currentSnapshot.X,
								Py = currentSnapshot.Y,
								Stage = 0
							};
							stack.Push(newSnapshot);
							continue;
						}
						retVal = null;
						break;
					case 5:
						if (retVal != null)
						{
							retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
							continue;
						}
						currentSnapshot.Stage = 7;
						stack.Push(currentSnapshot);

						newSnapshot = new JumpSnapshot
						{
							X = currentSnapshot.X,
							Y = currentSnapshot.Y - 1,
							Px = currentSnapshot.X,
							Py = currentSnapshot.Y,
							Stage = 0
						};
						stack.Push(newSnapshot);
						break;
					case 6:
						if (retVal != null)
						{
							retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
							continue;
						}
						currentSnapshot.Stage = 7;
						stack.Push(currentSnapshot);

						newSnapshot = new JumpSnapshot
						{
							X = currentSnapshot.X - 1,
							Y = currentSnapshot.Y,
							Px = currentSnapshot.X,
							Py = currentSnapshot.Y,
							Stage = 0
						};
						stack.Push(newSnapshot);
						break;
					case 7:
						if (retVal != null)
						{
							retVal = new GridPos(currentSnapshot.X, currentSnapshot.Y);
							continue;
						}
						// keep going
						if (jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X + currentSnapshot.Dx, currentSnapshot.Y) && jpParam.SearchGrid.IsWalkableAt(currentSnapshot.X, currentSnapshot.Y + currentSnapshot.Dy))
						{
							newSnapshot = new JumpSnapshot
							{
								X = currentSnapshot.X + currentSnapshot.Dx,
								Y = currentSnapshot.Y + currentSnapshot.Dy,
								Px = currentSnapshot.X,
								Py = currentSnapshot.Y,
								Stage = 0
							};
							stack.Push(newSnapshot);
							continue;
						}
						retVal = null;
						break;
				}
			}

			return retVal;
		}
		private static GridPos Jump(JumpPointParam jpParam, int x, int y, int px, int py)
		{
			if (!jpParam.SearchGrid.IsWalkableAt(x, y))
			{
				return null;
			}
			else if (jpParam.SearchGrid.GetNodeAt(x, y).Equals(jpParam.EndNode))
			{
				return new GridPos(x, y);
			}

			var tDx = x - px;
			var tDy = y - py;
			if (jpParam.DiagonalMovement == DiagonalMovement.Always || jpParam.DiagonalMovement == DiagonalMovement.IfAtLeastOneWalkable)
			{
				// check for forced neighbors
				// along the diagonal
				if (tDx != 0 && tDy != 0)
				{
					if ((jpParam.SearchGrid.IsWalkableAt(x - tDx, y + tDy) && !jpParam.SearchGrid.IsWalkableAt(x - tDx, y)) ||
						(jpParam.SearchGrid.IsWalkableAt(x + tDx, y - tDy) && !jpParam.SearchGrid.IsWalkableAt(x, y - tDy)))
					{
						return new GridPos(x, y);
					}
				}
				// horizontally/vertically
				else
				{
					if (tDx != 0)
					{
						// moving along x
						if ((jpParam.SearchGrid.IsWalkableAt(x + tDx, y + 1) && !jpParam.SearchGrid.IsWalkableAt(x, y + 1)) ||
							(jpParam.SearchGrid.IsWalkableAt(x + tDx, y - 1) && !jpParam.SearchGrid.IsWalkableAt(x, y - 1)))
						{
							return new GridPos(x, y);
						}
					}
					else
					{
						if ((jpParam.SearchGrid.IsWalkableAt(x + 1, y + tDy) && !jpParam.SearchGrid.IsWalkableAt(x + 1, y)) ||
							(jpParam.SearchGrid.IsWalkableAt(x - 1, y + tDy) && !jpParam.SearchGrid.IsWalkableAt(x - 1, y)))
						{
							return new GridPos(x, y);
						}
					}
				}
				// when moving diagonally, must check for vertical/horizontal jump points
				if (tDx != 0 && tDy != 0)
				{
					if (Jump(jpParam, x + tDx, y, x, y) != null)
					{
						return new GridPos(x, y);
					}
					if (Jump(jpParam, x, y + tDy, x, y) != null)
					{
						return new GridPos(x, y);
					}
				}

				// moving diagonally, must make sure one of the vertical/horizontal
				// neighbors is open to allow the path
				return jpParam.SearchGrid.IsWalkableAt(x + tDx, y) || jpParam.SearchGrid.IsWalkableAt(x, y + tDy)
					? Jump(jpParam, x + tDx, y + tDy, x, y)
					: jpParam.DiagonalMovement == DiagonalMovement.Always ? Jump(jpParam, x + tDx, y + tDy, x, y) : null;
			}
			else if (jpParam.DiagonalMovement == DiagonalMovement.OnlyWhenNoObstacles)
			{
				// check for forced neighbors
				// along the diagonal
				if (tDx != 0 && tDy != 0)
				{
					if (jpParam.SearchGrid.IsWalkableAt(x + tDx, y + tDy) && (!jpParam.SearchGrid.IsWalkableAt(x, y + tDy) || !jpParam.SearchGrid.IsWalkableAt(x + tDx, y)))
					{
						return new GridPos(x, y);
					}
				}
				// horizontally/vertically
				else
				{
					if (tDx != 0)
					{
						// moving along x
						if ((jpParam.SearchGrid.IsWalkableAt(x, y + 1) && !jpParam.SearchGrid.IsWalkableAt(x - tDx, y + 1)) ||
							(jpParam.SearchGrid.IsWalkableAt(x, y - 1) && !jpParam.SearchGrid.IsWalkableAt(x - tDx, y - 1)))
						{
							return new GridPos(x, y);
						}
					}
					else
					{
						if ((jpParam.SearchGrid.IsWalkableAt(x + 1, y) && !jpParam.SearchGrid.IsWalkableAt(x + 1, y - tDy)) ||
							(jpParam.SearchGrid.IsWalkableAt(x - 1, y) && !jpParam.SearchGrid.IsWalkableAt(x - 1, y - tDy)))
						{
							return new GridPos(x, y);
						}
					}
				}

				// when moving diagonally, must check for vertical/horizontal jump points
				if (tDx != 0 && tDy != 0)
				{
					if (Jump(jpParam, x + tDx, y, x, y) != null)
					{
						return new GridPos(x, y);
					}

					if (Jump(jpParam, x, y + tDy, x, y) != null)
					{
						return new GridPos(x, y);
					}
				}

				// moving diagonally, must make sure both of the vertical/horizontal
				// neighbors is open to allow the path
				return jpParam.SearchGrid.IsWalkableAt(x + tDx, y) && jpParam.SearchGrid.IsWalkableAt(x, y + tDy)
					? Jump(jpParam, x + tDx, y + tDy, x, y)
					: null;
			}
			else // if(jpParam.DiagonalMovement == DiagonalMovement.Never)
			{
				if (tDx != 0)
				{
					// moving along x
					if (!jpParam.SearchGrid.IsWalkableAt(x + tDx, y))
					{
						return new GridPos(x, y);
					}
				}
				else
				{
					if (!jpParam.SearchGrid.IsWalkableAt(x, y + tDy))
					{
						return new GridPos(x, y);
					}
				}

				//  must check for perpendicular jump points
				if (tDx != 0)
				{
					if (Jump(jpParam, x, y + 1, x, y) != null)
					{
						return new GridPos(x, y);
					}

					if (Jump(jpParam, x, y - 1, x, y) != null)
					{
						return new GridPos(x, y);
					}
				}
				else // tDy != 0
				{
					if (Jump(jpParam, x + 1, y, x, y) != null)
					{
						return new GridPos(x, y);
					}

					if (Jump(jpParam, x - 1, y, x, y) != null)
					{
						return new GridPos(x, y);
					}
				}

				// keep going
				return jpParam.SearchGrid.IsWalkableAt(x + tDx, y) && jpParam.SearchGrid.IsWalkableAt(x, y + tDy)
					? Jump(jpParam, x + tDx, y + tDy, x, y)
					: null;
			}
		}

		private static List<GridPos> FindNeighbors(JumpPointParam jpParam, Node node)
		{
			var tParent = (Node)node.Parent;
			//var diagonalMovement = Util.GetDiagonalMovement(jpParam.CrossCorner, jpParam.CrossAdjacentPoint);
			var tX = node.X;
			var tY = node.Y;
			int tPx, tPy, tDx, tDy;
			var tNeighbors = new List<GridPos>();
			List<Node> tNeighborNodes;
			Node tNeighborNode;

			// directed pruning: can ignore most neighbors, unless forced.
			if (tParent != null)
			{
				tPx = tParent.X;
				tPy = tParent.Y;
				// get the normalized direction of travel
				tDx = (tX - tPx) / Math.Max(Math.Abs(tX - tPx), 1);
				tDy = (tY - tPy) / Math.Max(Math.Abs(tY - tPy), 1);

				if (jpParam.DiagonalMovement == DiagonalMovement.Always || jpParam.DiagonalMovement == DiagonalMovement.IfAtLeastOneWalkable)
				{
					// search diagonally
					if (tDx != 0 && tDy != 0)
					{
						if (jpParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
						{
							tNeighbors.Add(new GridPos(tX, tY + tDy));
						}
						if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
						{
							tNeighbors.Add(new GridPos(tX + tDx, tY));
						}

						if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY + tDy))
						{
							if (jpParam.SearchGrid.IsWalkableAt(tX, tY + tDy) || jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
							{
								tNeighbors.Add(new GridPos(tX + tDx, tY + tDy));
							}
							else if (jpParam.DiagonalMovement == DiagonalMovement.Always)
							{
								tNeighbors.Add(new GridPos(tX + tDx, tY + tDy));
							}
						}

						if (jpParam.SearchGrid.IsWalkableAt(tX - tDx, tY + tDy) && !jpParam.SearchGrid.IsWalkableAt(tX - tDx, tY))
						{
							if (jpParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
							{
								tNeighbors.Add(new GridPos(tX - tDx, tY + tDy));
							}
							else if (jpParam.DiagonalMovement == DiagonalMovement.Always)
							{
								tNeighbors.Add(new GridPos(tX - tDx, tY + tDy));
							}
						}

						if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY - tDy) && !jpParam.SearchGrid.IsWalkableAt(tX, tY - tDy))
						{
							if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
							{
								tNeighbors.Add(new GridPos(tX + tDx, tY - tDy));
							}
							else if (jpParam.DiagonalMovement == DiagonalMovement.Always)
							{
								tNeighbors.Add(new GridPos(tX + tDx, tY - tDy));
							}
						}
					}
					// search horizontally/vertically
					else
					{
						if (tDx != 0)
						{
							if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
							{
								tNeighbors.Add(new GridPos(tX + tDx, tY));

								if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY + 1) && !jpParam.SearchGrid.IsWalkableAt(tX, tY + 1))
								{
									tNeighbors.Add(new GridPos(tX + tDx, tY + 1));
								}
								if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY - 1) && !jpParam.SearchGrid.IsWalkableAt(tX, tY - 1))
								{
									tNeighbors.Add(new GridPos(tX + tDx, tY - 1));
								}
							}
							else if (jpParam.DiagonalMovement == DiagonalMovement.Always)
							{
								if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY + 1) && !jpParam.SearchGrid.IsWalkableAt(tX, tY + 1))
								{
									tNeighbors.Add(new GridPos(tX + tDx, tY + 1));
								}
								if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY - 1) && !jpParam.SearchGrid.IsWalkableAt(tX, tY - 1))
								{
									tNeighbors.Add(new GridPos(tX + tDx, tY - 1));
								}
							}
						}
						else
						{
							if (jpParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
							{
								tNeighbors.Add(new GridPos(tX, tY + tDy));

								if (jpParam.SearchGrid.IsWalkableAt(tX + 1, tY + tDy) && !jpParam.SearchGrid.IsWalkableAt(tX + 1, tY))
								{
									tNeighbors.Add(new GridPos(tX + 1, tY + tDy));
								}
								if (jpParam.SearchGrid.IsWalkableAt(tX - 1, tY + tDy) && !jpParam.SearchGrid.IsWalkableAt(tX - 1, tY))
								{
									tNeighbors.Add(new GridPos(tX - 1, tY + tDy));
								}
							}
							else if (jpParam.DiagonalMovement == DiagonalMovement.Always)
							{
								if (jpParam.SearchGrid.IsWalkableAt(tX + 1, tY + tDy) && !jpParam.SearchGrid.IsWalkableAt(tX + 1, tY))
								{
									tNeighbors.Add(new GridPos(tX + 1, tY + tDy));
								}
								if (jpParam.SearchGrid.IsWalkableAt(tX - 1, tY + tDy) && !jpParam.SearchGrid.IsWalkableAt(tX - 1, tY))
								{
									tNeighbors.Add(new GridPos(tX - 1, tY + tDy));
								}
							}
						}
					}
				}
				else if (jpParam.DiagonalMovement == DiagonalMovement.OnlyWhenNoObstacles)
				{
					// search diagonally
					if (tDx != 0 && tDy != 0)
					{
						if (jpParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
						{
							tNeighbors.Add(new GridPos(tX, tY + tDy));
						}
						if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
						{
							tNeighbors.Add(new GridPos(tX + tDx, tY));
						}

						if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY + tDy))
						{
							if (jpParam.SearchGrid.IsWalkableAt(tX, tY + tDy) && jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
							{
								tNeighbors.Add(new GridPos(tX + tDx, tY + tDy));
							}
						}

						if (jpParam.SearchGrid.IsWalkableAt(tX - tDx, tY + tDy))
						{
							if (jpParam.SearchGrid.IsWalkableAt(tX, tY + tDy) && jpParam.SearchGrid.IsWalkableAt(tX - tDx, tY))
							{
								tNeighbors.Add(new GridPos(tX - tDx, tY + tDy));
							}
						}

						if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY - tDy))
						{
							if (jpParam.SearchGrid.IsWalkableAt(tX, tY - tDy) && jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
							{
								tNeighbors.Add(new GridPos(tX + tDx, tY - tDy));
							}
						}
					}
					// search horizontally/vertically
					else
					{
						if (tDx != 0)
						{
							if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
							{
								tNeighbors.Add(new GridPos(tX + tDx, tY));

								if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY + 1) && jpParam.SearchGrid.IsWalkableAt(tX, tY + 1))
								{
									tNeighbors.Add(new GridPos(tX + tDx, tY + 1));
								}
								if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY - 1) && jpParam.SearchGrid.IsWalkableAt(tX, tY - 1))
								{
									tNeighbors.Add(new GridPos(tX + tDx, tY - 1));
								}
							}
							if (jpParam.SearchGrid.IsWalkableAt(tX, tY + 1))
							{
								tNeighbors.Add(new GridPos(tX, tY + 1));
							}

							if (jpParam.SearchGrid.IsWalkableAt(tX, tY - 1))
							{
								tNeighbors.Add(new GridPos(tX, tY - 1));
							}
						}
						else
						{
							if (jpParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
							{
								tNeighbors.Add(new GridPos(tX, tY + tDy));

								if (jpParam.SearchGrid.IsWalkableAt(tX + 1, tY + tDy) && jpParam.SearchGrid.IsWalkableAt(tX + 1, tY))
								{
									tNeighbors.Add(new GridPos(tX + 1, tY + tDy));
								}
								if (jpParam.SearchGrid.IsWalkableAt(tX - 1, tY + tDy) && jpParam.SearchGrid.IsWalkableAt(tX - 1, tY))
								{
									tNeighbors.Add(new GridPos(tX - 1, tY + tDy));
								}
							}
							if (jpParam.SearchGrid.IsWalkableAt(tX + 1, tY))
							{
								tNeighbors.Add(new GridPos(tX + 1, tY));
							}

							if (jpParam.SearchGrid.IsWalkableAt(tX - 1, tY))
							{
								tNeighbors.Add(new GridPos(tX - 1, tY));
							}
						}
					}
				}
				else // if(jpParam.DiagonalMovement == DiagonalMovement.Never)
				{
					if (tDx != 0)
					{
						if (jpParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
						{
							tNeighbors.Add(new GridPos(tX + tDx, tY));
						}
						if (jpParam.SearchGrid.IsWalkableAt(tX, tY + 1))
						{
							tNeighbors.Add(new GridPos(tX, tY + 1));
						}
						if (jpParam.SearchGrid.IsWalkableAt(tX, tY - 1))
						{
							tNeighbors.Add(new GridPos(tX, tY - 1));
						}
					}
					else // if (tDy != 0)
					{
						if (jpParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
						{
							tNeighbors.Add(new GridPos(tX, tY + tDy));
						}
						if (jpParam.SearchGrid.IsWalkableAt(tX + 1, tY))
						{
							tNeighbors.Add(new GridPos(tX + 1, tY));
						}
						if (jpParam.SearchGrid.IsWalkableAt(tX - 1, tY))
						{
							tNeighbors.Add(new GridPos(tX - 1, tY));
						}
					}
				}
			}
			// return all neighbors
			else
			{
				tNeighborNodes = jpParam.SearchGrid.GetNeighbors(node, jpParam.DiagonalMovement);
				for (var i = 0; i < tNeighborNodes.Count; i++)
				{
					tNeighborNode = tNeighborNodes[i];
					tNeighbors.Add(new GridPos(tNeighborNode.X, tNeighborNode.Y));
				}
			}

			return tNeighbors;
		}
	}
}
