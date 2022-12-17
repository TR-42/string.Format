using System;
using System.Globalization;

namespace MyString.Format.Tests;

public class FormatItemInfo_ObjToStringTests
{
	[TestCase("D", 123, "123")]
	[TestCase("X", 123, "7B")]
	[TestCase("#,000.0", 1234.51, "1,234.5")]
	[TestCase("##,000.0##", 1234.51, "1,234.51")]
	public void Test_WithoutFormatProvider(string formatString, object obj, string expected)
		=> Assert.That(
			new TR.FormatItemInfo(0, 0, formatString).ObjToString(obj, null),
			Is.EqualTo(expected)
		);

	[TestCase("C", 123, "$123.00")]
	[TestCase("N", 12345, "12,345.000")]
	[TestCase("C", 1234.51, "$1,234.51")]
	[TestCase("P", 0.123451, "12.345%")]
	public void Test_WithFormatProvider(string formatString, object obj, string expected)
		=> Assert.That(
			new TR.FormatItemInfo(0, 0, formatString).ObjToString(obj, CultureInfo.CreateSpecificCulture("en-US")),
			Is.EqualTo(expected)
		);
}
