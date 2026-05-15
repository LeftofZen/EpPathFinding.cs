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
				Assert.That(rect.Left, Is.EqualTo(0));
				Assert.That(rect.Top, Is.EqualTo(0));
				Assert.That(rect.Right, Is.EqualTo(0));
				Assert.That(rect.Bottom, Is.EqualTo(0));
			});
		}

		[Test]
		public void Constructor_IntParams()
		{
			var rect = new GridRect(1, 2, 3, 4);

			Assert.Multiple(() =>
			{
				Assert.That(rect.Left, Is.EqualTo(1));
				Assert.That(rect.Top, Is.EqualTo(2));
				Assert.That(rect.Right, Is.EqualTo(3));
				Assert.That(rect.Bottom, Is.EqualTo(4));
			});
		}

		[Test]
		public void Constructor_GridPosParam()
		{
			var rect = new GridRect(new GridRect(1, 2, 3, 4));

			Assert.Multiple(() =>
			{
				Assert.That(rect.Left, Is.EqualTo(1));
				Assert.That(rect.Top, Is.EqualTo(2));
				Assert.That(rect.Right, Is.EqualTo(3));
				Assert.That(rect.Bottom, Is.EqualTo(4));
			});
		}

		[Test]
		public void Set()
		{
			var rect = new GridRect(1, 2, 3, 4);
			rect.Set(5, 6, 7, 8);

			Assert.Multiple(() =>
			{
				Assert.That(rect.Left, Is.EqualTo(5));
				Assert.That(rect.Top, Is.EqualTo(6));
				Assert.That(rect.Right, Is.EqualTo(7));
				Assert.That(rect.Bottom, Is.EqualTo(8));
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
				Assert.That(rect.Equals(rect), Is.True);
				Assert.That(rect.Equals(other), Is.True);
				Assert.That(rect.Equals(third), Is.False);

				Assert.That(rect.Equals((object)other), Is.True);
				Assert.That(rect.Equals(null), Is.False);
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
				Assert.That(rect == rect, Is.True);
#pragma warning restore CS1718 // Comparison made to same variable
				Assert.That(rect == other, Is.True);
				Assert.That(rect != third, Is.True);

				Assert.That(rect != (object)other, Is.True); // operator== with object not implemented yet
				Assert.That(rect != null, Is.True);

			});
		}

		[Test]
		public void _ToString()
		{
			var pos = new GridRect(5, 6, 7, 8);
			Assert.That(pos.ToString(), Is.EqualTo("(Top=6, Left=5, Bottom=8 Right=7)"));
		}
	}
}
