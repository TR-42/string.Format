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

		[TestCase("{-1}")]
		[TestCase("{-214738648}")]
		[TestCase("{0xFF}")]
		public void ArgumentIndexInvalidCharTest(string format)
			=> Tester<FormatException>(format, "You must use only digit in the `ArgumentIndex` segment");

		[TestCase("{0,}")]
		[TestCase("{10,:}")]
		public void AlignmentEmptyTest(string format)
			=> Tester<FormatException>(format, "Alignment must have one value (You must put a number after a comma)");

		static void Tester<T>(string format, string message) where T : Exception
			=> Assert.That(
				() => new TR.FormatItemInfo(format, new(0, format.Length)),
				Throws.InstanceOf<T>().And.Message.EqualTo(message)
			);
	}
}
