﻿/*!
@file GridLine.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief GridLine Interface
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

An Interface for the GridLine Class.

*/
using System.Drawing;

namespace EpPathFinding.cs
{
	internal class GridLine
	{
		public int FromX, FromY, ToX, ToY;
		public Pen Pen;

		public GridLine(GridBox from, GridBox to)
		{
			FromX = from.BoxRect.X + 9;
			FromY = from.BoxRect.Y + 9;
			ToX = to.BoxRect.X + 9;
			ToY = to.BoxRect.Y + 9;
			Pen = new Pen(Color.Yellow)
			{
				Width = 2
			};
		}

		public void DrawLine(Graphics graphics)
			=> graphics.DrawLine(Pen, FromX, FromY, ToX, ToY);

		public void Dispose() => Pen?.Dispose();
	}
}
