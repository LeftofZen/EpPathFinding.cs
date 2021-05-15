using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpPathFinding.cs;
using NUnit.Framework;

namespace EpPathFindingTest
{
	public class UtilTest
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void GetDiagonalMovement()
		{
			Assert.AreEqual(DiagonalMovement.Always, Util.GetDiagonalMovement(true, true));
			Assert.AreEqual(DiagonalMovement.IfAtLeastOneWalkable, Util.GetDiagonalMovement(true, false));
			Assert.AreEqual(DiagonalMovement.OnlyWhenNoObstacles, Util.GetDiagonalMovement(false, true));
			Assert.AreEqual(DiagonalMovement.Never, Util.GetDiagonalMovement(false, false));
		}
	}
}
