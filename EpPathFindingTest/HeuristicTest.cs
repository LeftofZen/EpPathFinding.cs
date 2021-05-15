using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpPathFinding.cs;
using NUnit.Framework;

namespace EpPathFindingTest
{
	public class HeuristicTest
	{
		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public void Manhattan()
		{
			Assert.Multiple(() =>
			{
				Assert.AreEqual(4, Heuristic.Manhattan(1, 3));
				Assert.AreEqual(4, Heuristic.Manhattan(0, 4));
				Assert.AreEqual(4, Heuristic.Manhattan(2, 2));
				Assert.AreEqual(16, Heuristic.Manhattan(-7, 9));
				Assert.AreEqual(0, Heuristic.Manhattan(0, 0));
			});
		}

		[Test]
		public void Euclidean()
		{
			Assert.Multiple(() =>
			{
				Assert.AreEqual(3.1622777f, Heuristic.Euclidean(1, 3));
				Assert.AreEqual(4, Heuristic.Euclidean(0, 4));
				Assert.AreEqual(2.82842708f, Heuristic.Euclidean(2, 2));
				Assert.AreEqual(11.4017544f, Heuristic.Euclidean(-7, 9));
				Assert.AreEqual(0, Heuristic.Euclidean(0, 0));
			});
		}

		[Test]
		public void EuclideanSquared()
		{
			Assert.Multiple(() =>
			{
				Assert.AreEqual(10, Heuristic.EuclideanSquared(1, 3));
				Assert.AreEqual(16, Heuristic.EuclideanSquared(0, 4));
				Assert.AreEqual(8, Heuristic.EuclideanSquared(2, 2));
				Assert.AreEqual(130, Heuristic.EuclideanSquared(-7, 9));
				Assert.AreEqual(0, Heuristic.EuclideanSquared(0, 0));
			});
		}

		[Test]
		public void Chebyshev()
		{
			Assert.Multiple(() =>
			{
				Assert.AreEqual(3, Heuristic.Chebyshev(1, 3));
				Assert.AreEqual(4, Heuristic.Chebyshev(0, 4));
				Assert.AreEqual(2, Heuristic.Chebyshev(2, 2));
				Assert.AreEqual(9, Heuristic.Chebyshev(-7, 9));
				Assert.AreEqual(9, Heuristic.Chebyshev(-9, 7));
				Assert.AreEqual(0, Heuristic.Chebyshev(0, 0));
			});
		}
	}
}
