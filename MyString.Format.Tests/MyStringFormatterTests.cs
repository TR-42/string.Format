using System;
using System.Collections.Generic;
using System.Linq;

using TR;

namespace MyString.Format.Tests;

public class MyStringFormatterTests
{
	#region CountFormatItemAndEscapingBrace
	[TestCase("", ExpectedResult = 0)]
	[TestCase("    ", ExpectedResult = 0)]
	[TestCase("{0}", ExpectedResult = 1)]
	[TestCase("{{", ExpectedResult = 1)]
	[TestCase("}}", ExpectedResult = 1)]
	[TestCase(" {0}", ExpectedResult = 1)]
	[TestCase(" {{", ExpectedResult = 1)]
	[TestCase(" }}", ExpectedResult = 1)]
	[TestCase("{0} ", ExpectedResult = 1)]
	[TestCase("{{ ", ExpectedResult = 1)]
	[TestCase("}} ", ExpectedResult = 1)]
	[TestCase("{{{0}", ExpectedResult = 2)]
	[TestCase("}}{0}", ExpectedResult = 2)]
	[TestCase("{0}{{", ExpectedResult = 2)]
	[TestCase("{0}}}", ExpectedResult = 2)]
	[TestCase("{0}{1}", ExpectedResult = 2)]
	[TestCase("{0} {1}", ExpectedResult = 2)]
	[TestCase(" {0} {1} ", ExpectedResult = 2)]
	[TestCase("{{ {0} {1} }}", ExpectedResult = 4)]
	public int CountFormatItemAndEscapingBrace_NormalCaseTest(string format)
		=> TR.MyStringFormatter.CountFormatItemAndEscapingBrace(format);

	[TestCase("{ {")]
	[TestCase("{ {}")]
	[TestCase(" { {")]
	[TestCase(" { {}")]
	public void CountFormatItemAndEscapingBrace_NestedBraceTest(string format)
		=> CountFormatItemAndEscapingBrace_ExceptionTest(format, "Brace cannot use in the Format item");

	[TestCase("}")]
	[TestCase(" }")]
	[TestCase(" {}}")]
	[TestCase(" {} }")]
	public void CountFormatItemAndEscapingBrace_InvalidLocationBraceTest(string format)
		=> CountFormatItemAndEscapingBrace_ExceptionTest(format, "Invalid Location Brace");

	[TestCase("{")]
	[TestCase("{{{")]
	[TestCase(" {")]
	[TestCase("{}{")]
	[TestCase("{} {")]
	public void CountFormatItemAndEscapingBrace_BraceNotClosingTest(string format)
		=> CountFormatItemAndEscapingBrace_ExceptionTest(format, "Brace not Closing");

	static void CountFormatItemAndEscapingBrace_ExceptionTest(string format, string expectedMessage)
		=> Assert.That(
			() => MyStringFormatter.CountFormatItemAndEscapingBrace(format),
			Throws.InstanceOf<FormatException>().And.Message.EqualTo(expectedMessage)
		);
	#endregion

	#region GetFormatItemSegments
	static IEnumerable<TestCaseData> GetFormatItemSegments_NormalTestSource()
	{
		static TestCaseData gen(string format, params (int index, int length)[] segments)
			=> new TestCaseData(format)
			.Returns(segments.Select(
				v => new FormatItemSegment(v.index, v.length)
			));

		yield return gen("");
		yield return gen(" ");

		yield return gen("{{", (0, 2));
		yield return gen("}}", (0, 2));
		yield return gen("{0}", (0, 3));
		yield return gen("{0,1:2.34}", (0, 10));

		yield return gen(" {{", (1, 2));
		yield return gen(" }}", (1, 2));
		yield return gen(" {0}", (1, 3));
		yield return gen(" {0,1:2.34}", (1, 10));

		yield return gen("{{{0}", (0, 2), (2, 3));
		yield return gen("}}{0}", (0, 2), (2, 3));
		yield return gen("{0}{{", (0, 3), (3, 2));
		yield return gen("{0}}}", (0, 3), (3, 2));
		yield return gen("{0}{12}", (0, 3), (3, 4));

		yield return gen("{{{0}}}", (0, 2), (2, 3), (5, 2));
		yield return gen("}}{0}{{", (0, 2), (2, 3), (5, 2));
	}

	[TestCaseSource(nameof(GetFormatItemSegments_NormalTestSource))]
	public Array GetFormatItemSegments_NormalTest(string format)
	{
		int itemCount = MyStringFormatter.CountFormatItemAndEscapingBrace(format);

		return MyStringFormatter.GetFormatItemSegments(format, itemCount);
	}
	#endregion
}
