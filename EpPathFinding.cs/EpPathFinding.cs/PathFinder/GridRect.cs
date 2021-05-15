/*!
@file GridRect.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief GridRect Interface
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

An Interface for the GridRect Struct.

*/

using System;

namespace EpPathFinding.cs
{
	public class GridRect : IEquatable<GridRect>
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public GridRect()
			=> Set(0, 0, 0, 0);

		public GridRect(int left, int top, int right, int bottom)
			=> Set(left, top, right, bottom);

		public GridRect(GridRect gr)
			=> Set(gr.Left, gr.Top, gr.Right, gr.Bottom);

		public void Set(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public override int GetHashCode()
			=> Left ^ Top ^ Right ^ Bottom;

		public override bool Equals(object obj)
			=> obj is GridRect rect && Left == rect.Left && Top == rect.Top && Bottom == rect.Bottom && Right == rect.Right;

		public bool Equals(GridRect rect)
			=> rect != null
			&& (Left == rect.Left) && (Top == rect.Top) && (Right == rect.Right) && (Bottom == rect.Bottom);

		public static bool operator ==(GridRect a, GridRect b)
			=> ReferenceEquals(a, b) || (!(a is null) && !(b is null) && a.Left == b.Left && a.Top == b.Top && a.Bottom == b.Bottom && a.Right == b.Right);

		public static bool operator !=(GridRect a, GridRect b)
			=> !(a == b);

		public override string ToString()
			=> $"(Top={Top}, Left={Left}, Bottom={Bottom} Right={Right})";
	}
}