using System;

namespace MyString.Format.Tests
{
public class FormatItemSegmentTests
{
	[TestCase(0, 16)]
	[TestCase(0, 2)]
	[TestCase(0, int.MaxValue)]
	[TestCase(int.MaxValue - 3, 3)]
	public void NormalCaseTest(int StartIndex, int Length)
		=> Assert.That(
			() => new TR.FormatItemSegment(StartIndex, Length),
			Throws.Nothing
		);

	[TestCase(-1, 16)]
	[TestCase(int.MinValue, 16)]
	public void StartIndexOutOfRangeTests(int StartIndex, int Length)
		=> Tester<ArgumentOutOfRangeException>(StartIndex, Length, "must be positive (Parameter 'StartIndex')");

	[TestCase(0, -1)]
	[TestCase(0, 0)]
	[TestCase(0, 1)]
	public void LengthOutOfRangeTests(int StartIndex, int Length)
		=> Tester<ArgumentOutOfRangeException>(StartIndex, Length, "must be same or more than 2 (Parameter 'Length')");

	[TestCase(1, int.MaxValue)]
	[TestCase(int.MaxValue - 2, 3)]
	[TestCase(int.MaxValue, int.MaxValue)]
	public void OverflowTests(int StartIndex, int Length)
		=> Tester<OverflowException>(StartIndex, Length, "`StartIndex + Length` will overflow with given values");

	static void Tester<T>(int StartIndex, int Length, string message) where T : Exception
		=> Assert.That(
			() => new TR.FormatItemSegment(StartIndex, Length),
			Throws.InstanceOf<T>().And.Message.EqualTo(message)
		);
}
}
