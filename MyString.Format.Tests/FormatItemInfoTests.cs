using System;

namespace MyString.Format.Tests
{
	public class FormatItemInfoTests
	{
		[TestCase("{0}", 0, 0, null)]
		[TestCase("{1}", 1, 0, null)]
		[TestCase("{2147483647}", int.MaxValue, 0, null)]

		[TestCase("{0,0}", 0, 0, null)]
		[TestCase("{1,3}", 1, 3, null)]
		[TestCase("{2147483647,2147483647}", int.MaxValue, int.MaxValue, null)]

		[TestCase("{0:}", 0, 0, null)]
		[TestCase("{1:abc d}", 1, 0, "abc d")]
		[TestCase("{2147483647: : \n}", int.MaxValue, 0, " : \n")]

		[TestCase("{0,0:abc}", 0, 0, "abc")]
		[TestCase("{1,3:}", 1, 3, null)]
		[TestCase("{2147483647,2147483647:,:,}", int.MaxValue, int.MaxValue, ",:,")]
		public void NormalCaseTest(string format, int expectArgumentIndex, int expectAlignment, string expectFormatString)
			=> Assert.That(
				new TR.FormatItemInfo(format, new(0, format.Length)),
				Is.EqualTo(new TR.FormatItemInfo(expectArgumentIndex, expectAlignment, expectFormatString))
			);
	}
}
