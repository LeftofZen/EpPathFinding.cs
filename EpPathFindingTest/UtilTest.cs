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
			Assert.That(Util.GetDiagonalMovement(true, true), Is.EqualTo(DiagonalMovement.Always));
			Assert.That(Util.GetDiagonalMovement(true, false), Is.EqualTo(DiagonalMovement.IfAtLeastOneWalkable));
			Assert.That(Util.GetDiagonalMovement(false, true), Is.EqualTo(DiagonalMovement.OnlyWhenNoObstacles));
			Assert.That(Util.GetDiagonalMovement(false, false), Is.EqualTo(DiagonalMovement.Never));
		}
	}
}
