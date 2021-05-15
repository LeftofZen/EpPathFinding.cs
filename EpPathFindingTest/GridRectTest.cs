using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpPathFinding.cs;
using NUnit.Framework;

namespace EpPathFindingTest
{
	public class GridRectTest
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Constructor_Default()
		{
			var rect = new GridRect();

			Assert.Multiple(() =>
			{
				Assert.AreEqual(0, rect.Left);
				Assert.AreEqual(0, rect.Top);
				Assert.AreEqual(0, rect.Right);
				Assert.AreEqual(0, rect.Bottom);
			});
		}

		[Test]
		public void Constructor_IntParams()
		{
			var rect = new GridRect(1, 2, 3, 4);

			Assert.Multiple(() =>
			{
				Assert.AreEqual(1, rect.Left);
				Assert.AreEqual(2, rect.Top);
				Assert.AreEqual(3, rect.Right);
				Assert.AreEqual(4, rect.Bottom);
			});
		}

		[Test]
		public void Constructor_GridPosParam()
		{
			var rect = new GridRect(new GridRect(1, 2, 3, 4));

			Assert.Multiple(() =>
			{
				Assert.AreEqual(1, rect.Left);
				Assert.AreEqual(2, rect.Top);
				Assert.AreEqual(3, rect.Right);
				Assert.AreEqual(4, rect.Bottom);
			});
		}

		[Test]
		public void Set()
		{
			var rect = new GridRect(1, 2, 3, 4);
			rect.Set(5, 6, 7, 8);

			Assert.Multiple(() =>
			{
				Assert.AreEqual(5, rect.Left);
				Assert.AreEqual(6, rect.Top);
				Assert.AreEqual(7, rect.Right);
				Assert.AreEqual(8, rect.Bottom);
			});
		}

		[Test]
		public void _Equals()
		{
			var rect = new GridRect(1, 2, 3, 4);
			var other = new GridRect(1, 2, 3, 4);
			var third = new GridRect(5, 6, 7, 8);

			Assert.Multiple(() =>
			{
				Assert.That(rect.Equals(rect));
				Assert.That(rect.Equals(other));
				Assert.That(!rect.Equals(third));

				Assert.That(rect.Equals((object)other));
				Assert.That(!rect.Equals(null));
			});
		}

		[Test]
		public void OperatorEquals()
		{
			var rect = new GridRect(1, 2, 3, 4);
			var other = new GridRect(1, 2, 3, 4);
			var third = new GridRect(5, 6, 7, 8);

			Assert.Multiple(() =>
			{

#pragma warning disable CS1718 // Comparison made to same variable
				Assert.That(rect == rect);
#pragma warning restore CS1718 // Comparison made to same variable
				Assert.That(rect == other);
				Assert.That(rect != third);

				Assert.That(rect != (object)other); // operator== with object not implemented yet
				Assert.That(rect != null);

			});
		}

		[Test]
		public void _ToString()
		{
			var pos = new GridRect(5, 6, 7, 8);
			Assert.AreNotEqual(pos.ToString(), "(Left=5, Top=6, Bottom=7, Right=8)");
		}
	}
}
