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
				Assert.That(Heuristic.Manhattan(1, 3), Is.EqualTo(4));
				Assert.That(Heuristic.Manhattan(0, 4), Is.EqualTo(4));
				Assert.That(Heuristic.Manhattan(2, 2), Is.EqualTo(4));
				Assert.That(Heuristic.Manhattan(-7, 9), Is.EqualTo(16));
				Assert.That(Heuristic.Manhattan(0, 0), Is.EqualTo(0));
			});
		}

		[Test]
		public void Euclidean()
		{
			Assert.Multiple(() =>
			{
				Assert.That(Heuristic.Euclidean(1, 3), Is.EqualTo(3.1622777f).Within(0.000001f));
				Assert.That(Heuristic.Euclidean(0, 4), Is.EqualTo(4f).Within(0.000001f));
				Assert.That(Heuristic.Euclidean(2, 2), Is.EqualTo(2.82842708f).Within(0.000001f));
				Assert.That(Heuristic.Euclidean(-7, 9), Is.EqualTo(11.4017544f).Within(0.000001f));
				Assert.That(Heuristic.Euclidean(0, 0), Is.EqualTo(0f).Within(0.000001f));
			});
		}

		[Test]
		public void EuclideanSquared()
		{
			Assert.Multiple(() =>
			{
				Assert.That(Heuristic.EuclideanSquared(1, 3), Is.EqualTo(10));
				Assert.That(Heuristic.EuclideanSquared(0, 4), Is.EqualTo(16));
				Assert.That(Heuristic.EuclideanSquared(2, 2), Is.EqualTo(8));
				Assert.That(Heuristic.EuclideanSquared(-7, 9), Is.EqualTo(130));
				Assert.That(Heuristic.EuclideanSquared(0, 0), Is.EqualTo(0));
			});
		}

		[Test]
		public void Chebyshev()
		{
			Assert.Multiple(() =>
			{
				Assert.That(Heuristic.Chebyshev(1, 3), Is.EqualTo(3));
				Assert.That(Heuristic.Chebyshev(0, 4), Is.EqualTo(4));
				Assert.That(Heuristic.Chebyshev(2, 2), Is.EqualTo(2));
				Assert.That(Heuristic.Chebyshev(-7, 9), Is.EqualTo(9));
				Assert.That(Heuristic.Chebyshev(-9, 7), Is.EqualTo(9));
				Assert.That(Heuristic.Chebyshev(0, 0), Is.EqualTo(0));
			});
		}
	}
}
