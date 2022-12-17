using TR;

namespace MyString.Format.Tests;

public class MyString_FormatTests
{
	[TestCase("", 123, ExpectedResult = "")]
	[TestCase("{{", 123, ExpectedResult = "{")]
	[TestCase("}}", 123, ExpectedResult = "}")]

	[TestCase("{0}", 123, ExpectedResult = "123")]
	[TestCase("{{{0}}}", 123, ExpectedResult = "{123}")]

	[TestCase(" {0} ", 123, ExpectedResult = " 123 ")]
	[TestCase("{{ {0} }}", 123, ExpectedResult = "{ 123 }")]
	[TestCase("{0,5}", 123, ExpectedResult = "  123")]
	[TestCase("{0,5:X}", 123, ExpectedResult = "   7B")]
	public string FormatTest_OneArg(string format, object arg0)
		=> new MyStringFormatter(format).Format(arg0);

	[TestCase("{0} {1}", 123, 1.23, ExpectedResult = "123 1.23")]
	[TestCase("{0,4}{1,5}", 123, 1.23, ExpectedResult = " 123 1.23")]
	[TestCase("{0,4:X}{1,10:P}", 123, 1.23, ExpectedResult = "  7B  123.000%")]
	public string FormatTest_TwoArg(string format, object arg0, object arg1)
		=> new MyStringFormatter(format).Format(arg0, arg1);

	[TestCase("{0} {1} {2}", 123, 1.23, 4.56, ExpectedResult = "123 1.23 4.56")]
	public string FormatTest_ThreeArg(string format, object arg0, object arg1, object arg2)
		=> new MyStringFormatter(format).Format(arg0, arg1, arg2);

	[TestCase("{0} {1} {2} {3}", 123, 1.23, 4.56, 7.89, ExpectedResult = "123 1.23 4.56 7.89")]
	public string FormatTest_FourArg(string format, object arg0, object arg1, object arg2, object arg3)
		=> new MyStringFormatter(format).Format(new object[] { arg0, arg1, arg2, arg3 });
}
