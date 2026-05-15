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
				Assert.That(pos.X, Is.EqualTo(0));
				Assert.That(pos.Y, Is.EqualTo(0));
			});
		}

		[Test]
		public void Constructor_IntParams()
		{
			var pos = new GridPos(1, 2);

			Assert.Multiple(() =>
			{
				Assert.That(pos.X, Is.EqualTo(1));
				Assert.That(pos.Y, Is.EqualTo(2));
			});
		}

		[Test]
		public void Constructor_GridPosParam()
		{
			var pos = new GridPos(new GridPos(3, 4));

			Assert.Multiple(() =>
			{
				Assert.That(pos.X, Is.EqualTo(3));
				Assert.That(pos.Y, Is.EqualTo(4));
			});
		}

		[Test]
		public void Set()
		{
			var pos = new GridPos(5, 6);
			pos.Set(7, 8);

			Assert.Multiple(() =>
			{
				Assert.That(pos.X, Is.EqualTo(7));
				Assert.That(pos.Y, Is.EqualTo(8));
			});
		}

		[Test]
		public void SetAndReturn()
		{
			var pos = new GridPos(5, 6);
			var ret = pos.SetAndReturn(7, 8);

			Assert.Multiple(() =>
			{
				Assert.That(pos.X, Is.EqualTo(7));
				Assert.That(pos.Y, Is.EqualTo(8));
				Assert.That(ret.X, Is.EqualTo(7));
				Assert.That(ret.Y, Is.EqualTo(8));
				Assert.That(ret, Is.SameAs(pos));
			});
		}

		[Test]
		public void Translate()
		{
			var pos = new GridPos(5, 6);
			var trn = pos.Translate(7, 8);

			Assert.Multiple(() =>
			{
				Assert.That(pos.X, Is.EqualTo(5));
				Assert.That(pos.Y, Is.EqualTo(6));
				Assert.That(trn.X, Is.EqualTo(12));
				Assert.That(trn.Y, Is.EqualTo(14));
				Assert.That(trn, Is.Not.SameAs(pos));
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
				Assert.That(pos.Equals(pos), Is.True);
				Assert.That(pos.Equals(other), Is.True);
				Assert.That(pos.Equals(third), Is.False);

				Assert.That(pos.Equals((object)other), Is.True);
				Assert.That(pos.Equals(null), Is.False);
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
				Assert.That(pos == pos, Is.True);
#pragma warning restore CS1718 // Comparison made to same variable
				Assert.That(pos == other, Is.True);
				Assert.That(pos != third, Is.True);

				Assert.That(pos != (object)other, Is.True); // operator== with object not implemented yet
				Assert.That(pos != null, Is.True);
			});
		}

		[Test]
		public void _ToString()
		{
			var pos = new GridPos(5, 6);
			Assert.That(pos.ToString(), Is.EqualTo("(X=5, Y=6)"));
		}
	}
}