/*!
@file GridBox.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief GridBox Interface
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

An Interface for the GridBox Class.

*/
using System;
using System.Drawing;

namespace EpPathFinding.cs
{
	internal enum BoxType { Start, End, Wall, Normal };

	internal class GridBox : IDisposable
	{
		public int X, Y, Width, Height;
		public SolidBrush Brush;
		public Rectangle BoxRect;
		public BoxType BoxType;

		public GridBox(int x, int y, BoxType boxType)
		{
			X = x;
			Y = y;
			BoxType = boxType;

			switch (boxType)
			{
				case BoxType.Normal:
					Brush = new SolidBrush(Color.WhiteSmoke);
					break;
				case BoxType.End:
					Brush = new SolidBrush(Color.Red);
					break;
				case BoxType.Start:
					Brush = new SolidBrush(Color.Green);
					break;
				case BoxType.Wall:
					Brush = new SolidBrush(Color.Gray);
					break;
			}

			Width = 18;
			Height = 18;
			BoxRect = new Rectangle(x, y, Width, Height);
		}

		public void DrawBox(Graphics graphics, BoxType boxType)
		{
			if (boxType == BoxType)
			{
				BoxRect.X = X;
				BoxRect.Y = Y;
				graphics.FillRectangle(Brush, BoxRect);
			}
		}

		public void SwitchBox()
		{
			switch (BoxType)
			{
				case BoxType.Normal:
					Brush?.Dispose();
					Brush = new SolidBrush(Color.Gray);
					BoxType = BoxType.Wall;
					break;
				case BoxType.Wall:
					Brush?.Dispose();
					Brush = new SolidBrush(Color.WhiteSmoke);
					BoxType = BoxType.Normal;
					break;
			}
		}

		public void SetNormalBox()
		{
			Brush?.Dispose();
			Brush = new SolidBrush(Color.WhiteSmoke);
			BoxType = BoxType.Normal;
		}

		public void SetStartBox()
		{
			Brush?.Dispose();
			Brush = new SolidBrush(Color.Green);
			BoxType = BoxType.Start;
		}

		public void SetEndBox()
		{
			Brush?.Dispose();
			Brush = new SolidBrush(Color.Red);
			BoxType = BoxType.End;
		}

		public void Dispose()
			=> Brush?.Dispose();
	}
}
