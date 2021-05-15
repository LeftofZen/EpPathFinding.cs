using EpPathFinding.cs;
using NUnit.Framework;

namespace EpPathFindingTest
{
	public class GridPosTest
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Constructor_Default()
		{
			var pos = new GridPos();

			Assert.Multiple(() =>
			{
				Assert.AreEqual(0, pos.X);
				Assert.AreEqual(0, pos.Y);
			});
		}

		[Test]
		public void Constructor_IntParams()
		{
			var pos = new GridPos(1, 2);

			Assert.Multiple(() =>
			{
				Assert.AreEqual(1, pos.X);
				Assert.AreEqual(2, pos.Y);
			});
		}

		[Test]
		public void Constructor_GridPosParam()
		{
			var pos = new GridPos(new GridPos(3, 4));

			Assert.Multiple(() =>
			{
				Assert.AreEqual(3, pos.X, 3);
				Assert.AreEqual(3, pos.Y, 4);
			});
		}

		[Test]
		public void Set()
		{
			var pos = new GridPos(5, 6);
			pos.Set(7, 8);

			Assert.Multiple(() =>
			{
				Assert.AreEqual(7, pos.X);
				Assert.AreEqual(8, pos.Y);
			});
		}

		[Test]
		public void SetAndReturn()
		{
			var pos = new GridPos(5, 6);
			var ret = pos.SetAndReturn(7, 8);

			Assert.Multiple(() =>
			{
				Assert.AreEqual(7, pos.X);
				Assert.AreEqual(8, pos.Y);
				Assert.AreEqual(7, ret.X);
				Assert.AreEqual(8, ret.Y);
				Assert.That(ReferenceEquals(pos, ret));
			});
		}

		[Test]
		public void Translate()
		{
			var pos = new GridPos(5, 6);
			var trn = pos.Translate(7, 8);

			Assert.Multiple(() =>
			{
				Assert.AreEqual(5, pos.X);
				Assert.AreEqual(6, pos.Y);
				Assert.AreEqual(12, trn.X);
				Assert.AreEqual(14, trn.Y);
				Assert.That(!ReferenceEquals(pos, trn));
			});
		}

		[Test]
		public void _Equals()
		{
			var pos = new GridPos(5, 6);
			var other = new GridPos(5, 6);
			var third = new GridPos(6, 5);

			Assert.Multiple(() =>
			{
				Assert.That(pos.Equals(pos));
				Assert.That(pos.Equals(other));
				Assert.That(!pos.Equals(third));

				Assert.That(pos.Equals((object)other));
				Assert.That(!pos.Equals(null));
			});
		}

		[Test]
		public void OperatorEquals()
		{
			var pos = new GridPos(5, 6);
			var other = new GridPos(5, 6);
			var third = new GridPos(6, 5);

			Assert.Multiple(() =>
			{
#pragma warning disable CS1718 // Comparison made to same variable
				Assert.That(pos == pos);
#pragma warning restore CS1718 // Comparison made to same variable
				Assert.That(pos == other);
				Assert.That(pos != third);

				Assert.That(pos != (object)other); // operator== with object not implemented yet
				Assert.That(pos != null);
			});
		}

		[Test]
		public void _ToString()
		{
			var pos = new GridPos(5, 6);
			Assert.AreNotEqual(pos.ToString(), "(X=5, Y=6)");
		}
	}
}