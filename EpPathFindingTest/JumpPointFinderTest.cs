using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpPathFinding.cs;
using NUnit.Framework;

namespace EpPathFindingTest
{
	public class JumpPointFinderTest
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void FindPath_Trivial()
		{
			var grid = new StaticGrid(3, 3);
			//grid.SetWalkableAll(true);
			var param = new JumpPointParam(grid, new GridPos(0, 0), new GridPos(2, 2));
			var path = JumpPointFinder.FindPath(param);

			Assert.Contains(new GridPos(0, 0), path);
			Assert.Contains(new GridPos(2, 2), path);
		}
	}
}
