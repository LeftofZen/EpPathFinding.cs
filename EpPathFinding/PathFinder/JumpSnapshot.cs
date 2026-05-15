using System;
using System.Collections.Generic;
using System.Text;

namespace EpPathFinding.cs
{
	public class JumpSnapshot
	{
		public int X;
		public int Y;
		public int Px;
		public int Py;
		public int Dx;
		public int Dy;
		public int Stage;

		public JumpSnapshot()
		{
			X = 0;
			Y = 0;
			Px = 0;
			Py = 0;
			Dx = 0;
			Dy = 0;
			Stage = 0;
		}
	}
}
