/*!
@file JumpPointFinder.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief Jump Point Search Parameter Interface
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

using C5;

namespace EpPathFinding.cs
{
	public enum IterationType
	{
		Loop,
		Recursive,
	};

	public enum EndNodeUnWalkableTreatment
	{
		Allow,
		Disallow
	};

	public class JumpPointParam : ParamBase
	{
		public JumpPointParam(BaseGrid grid, GridPos startPos, GridPos endPos, EndNodeUnWalkableTreatment allowEndNodeUnWalkable = EndNodeUnWalkableTreatment.Allow, DiagonalMovement diagonalMovement = DiagonalMovement.Always, HeuristicMode mode = HeuristicMode.Euclidean)
			: base(grid, startPos, endPos, diagonalMovement, mode)
		{
			CurEndNodeUnWalkableTreatment = allowEndNodeUnWalkable;
			OpenList = new IntervalHeap<Node>();

			CurIterationType = IterationType.Loop;
		}

		public JumpPointParam(BaseGrid grid, EndNodeUnWalkableTreatment allowEndNodeUnWalkable = EndNodeUnWalkableTreatment.Allow, DiagonalMovement diagonalMovement = DiagonalMovement.Always, HeuristicMode mode = HeuristicMode.Euclidean)
			: base(grid, diagonalMovement, mode)
		{
			CurEndNodeUnWalkableTreatment = allowEndNodeUnWalkable;

			OpenList = new IntervalHeap<Node>();
			CurIterationType = IterationType.Loop;
		}

		public JumpPointParam(JumpPointParam b) : base(b)
		{
			heuristic = b.heuristic;
			CurEndNodeUnWalkableTreatment = b.CurEndNodeUnWalkableTreatment;

			OpenList = new IntervalHeap<Node>();
			OpenList.AddAll(b.OpenList);

			CurIterationType = b.CurIterationType;
		}

		internal override void ResetInternal(GridPos startPos, GridPos endPos, BaseGrid searchGrid = null)
			=> OpenList = new IntervalHeap<Node>();//openList.Clear();

		public EndNodeUnWalkableTreatment CurEndNodeUnWalkableTreatment
		{
			get;
			set;
		}
		public IterationType CurIterationType
		{
			get;
			set;
		}

		public IntervalHeap<Node> OpenList;
	}
}
