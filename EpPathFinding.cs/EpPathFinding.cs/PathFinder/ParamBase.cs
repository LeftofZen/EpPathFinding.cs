/*!
@file JumpPointFinder.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief Param Base Interface
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

namespace EpPathFinding.cs
{
	public delegate float HeuristicDelegate(int iDx, int iDy);

	public abstract class ParamBase
	{
		protected ParamBase(BaseGrid grid, GridPos startPos, GridPos endPos, DiagonalMovement diagonalMovement, HeuristicMode mode) : this(grid, diagonalMovement, mode)
		{
			startNode = searchGrid.GetNodeAt(startPos.X, startPos.Y);
			endNode = searchGrid.GetNodeAt(endPos.X, endPos.Y);

			if (startNode == null)
			{
				startNode = new Node(startPos.X, startPos.Y, true);
			}

			if (endNode == null)
			{
				endNode = new Node(endPos.X, endPos.Y, true);
			}
		}

		protected ParamBase(BaseGrid grid, DiagonalMovement diagonalMovement, HeuristicMode mode)
		{
			SetHeuristic(mode);

			searchGrid = grid;
			DiagonalMovement = diagonalMovement;
			startNode = null;
			endNode = null;
		}

		protected ParamBase(ParamBase param)
		{
			searchGrid = param.searchGrid;
			DiagonalMovement = param.DiagonalMovement;
			startNode = param.startNode;
			endNode = param.endNode;
		}

		internal abstract void ResetInternal(GridPos startPos, GridPos endPos, BaseGrid searchGrid = null);

		public void Reset(GridPos startPos, GridPos endPos, BaseGrid _searchGrid = null)
		{
			ResetInternal(startPos, endPos, _searchGrid);
			startNode = null;
			endNode = null;

			if (_searchGrid != null)
			{
				searchGrid = _searchGrid;
			}

			searchGrid.Reset();
			startNode = searchGrid.GetNodeAt(startPos.X, startPos.Y);
			endNode = searchGrid.GetNodeAt(endPos.X, endPos.Y);

			if (startNode == null)
			{
				startNode = new Node(startPos.X, startPos.Y, true);
			}

			if (endNode == null)
			{
				endNode = new Node(endPos.X, endPos.Y, true);
			}
		}

		public DiagonalMovement DiagonalMovement;

		public HeuristicDelegate HeuristicFunc
			=> heuristic;

		public BaseGrid SearchGrid
			=> searchGrid;

		public Node StartNode
			=> startNode;

		public Node EndNode
			=> endNode;

		public void SetHeuristic(HeuristicMode mode)
			=> heuristic = mode switch
			{
				HeuristicMode.Manhattan => new HeuristicDelegate(Heuristic.Manhattan),
				HeuristicMode.Euclidean => new HeuristicDelegate(Heuristic.Euclidean),
				HeuristicMode.Chebyshev => new HeuristicDelegate(Heuristic.Chebyshev),
				_ => new HeuristicDelegate(Heuristic.Euclidean),
			};

		protected BaseGrid searchGrid;
		protected Node startNode;
		protected Node endNode;
		protected HeuristicDelegate heuristic;
	}
}
