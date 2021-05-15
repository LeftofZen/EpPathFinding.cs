/*!
@file GridPos.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief Grid Position Interface
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

An Interface for the Grid Position Struct.

*/
using System;

namespace EpPathFinding.cs
{
	public class GridPos : IEquatable<GridPos>
	{
		public int X;
		public int Y;

		public GridPos()
			=> Set(0, 0);

		public GridPos(int x, int y)
			=> Set(x, y);

		public GridPos(GridPos gp)
			=> Set(gp.X, gp.Y);

		public void Set(int x, int y)
		{
			X = x;
			Y = y;
		}
		public GridPos SetAndReturn(int x, int y)
		{
			X = x;
			Y = y;
			return this;
		}

		public GridPos Translate(int x, int Y)
			=> new GridPos(X + x, Y + Y);

		public override int GetHashCode()
			=> X ^ Y;

		public override bool Equals(object obj)
			=> (obj is GridPos pos) && X == pos.X && Y == pos.Y;

		public bool Equals(GridPos pos)
			=> pos != null && (X == pos.X) && (Y == pos.Y);

		public static bool operator ==(GridPos a, GridPos b)
			=> ReferenceEquals(a, b) || (!(a is null) && !(b is null) && a.X == b.X && a.Y == b.Y);

		public static bool operator !=(GridPos a, GridPos b)
			=> !(a == b);

		public override string ToString()
			=> $"(X={X}, Y={Y})";
	}
}
