using System;

namespace TR
{
	public class MyString
	{
		public static string Format(string format, object arg0)
		{
			return new MyStringFormatter(format).Format(arg0);
		}

		public static string Format(string format, params object[] args)
		{
			return new MyStringFormatter(format).Format(args);
		}

		public static string Format(IFormatProvider provider, string format, object arg0)
		{
			return new MyStringFormatter(provider, format).Format(arg0);
		}

		public static string Format(IFormatProvider provider, string format, params object[] args)
		{
			return new MyStringFormatter(provider, format).Format(args);
		}

		public static string Format(string format, object arg0, object arg1)
		{
			return new MyStringFormatter(format).Format(arg0, arg1);
		}

		public static string Format(IFormatProvider provider, string format, object arg0, object arg1)
		{
			return new MyStringFormatter(provider, format).Format(arg0, arg1);
		}

		public static string Format(string format, object arg0, object arg1, object arg2)
		{
			return new MyStringFormatter(format).Format(arg0, arg1, arg2);
		}

		public static string Format(IFormatProvider provider, string format, object arg0, object arg1, object arg2)
		{
			return new MyStringFormatter(provider, format).Format(arg0, arg1, arg2);
		}
	}
}
