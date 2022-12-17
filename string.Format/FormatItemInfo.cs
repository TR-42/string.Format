using System;
using System.Reflection;

namespace TR
{
	internal class FormatItemInfo
	{
		const char ALIGNMENT_SEPARATE_CHAR = ',';
		const char FORMAT_STRING_SEPARATE_CHAR = ':';

		const int ARG_INDEX_MEANING_OF_ESCAPE_OPEN_BRACE = -1;
		const int ARG_INDEX_MEANING_OF_ESCAPE_CLOSE_BRACE = -2;
		const char OPEN_BRACE = '{';
		const char CLOSE_BRACE = '}';
		const string OPEN_BRACE_STR = "{";
		const string CLOSE_BRACE_STR = "}";

		public readonly int ArgumentIndex;
		public readonly int Alignment;
		public readonly string FormatString;

		public FormatItemInfo(int ArgumentIndex, int Alignment, string FormatString)
		{
			this.ArgumentIndex = ArgumentIndex;
			this.Alignment = Alignment;
			this.FormatString = FormatString;
		}

		public FormatItemInfo(string format, FormatItemSegment segment)
		{
			if (segment.Length == 2)
			{
				char char1 = format[segment.StartIndex];
				char char2 = format[segment.StartIndex + 1];

				if (char1 == OPEN_BRACE && char2 == OPEN_BRACE)
					this.ArgumentIndex = ARG_INDEX_MEANING_OF_ESCAPE_OPEN_BRACE;
				else if (char1 == CLOSE_BRACE && char2 == CLOSE_BRACE)
					this.ArgumentIndex = ARG_INDEX_MEANING_OF_ESCAPE_CLOSE_BRACE;
				else
					throw new ArgumentException("the string is not escaping a brace");
			}

			bool isIndexAlreadyParsed = false;
			bool isAlignmentAlreadyParsed = false;

			// 始まりと終わりの括弧は除いて解析する
			int startIndex = segment.StartIndex + 1;
			int closeBraceIndex = segment.StartIndex + segment.Length - 1;

			int currentComponentStartIndex = startIndex;

			for (int i = startIndex; i < closeBraceIndex; i++)
			{
				char currentChar = format[i];
				if (!isIndexAlreadyParsed)
				{
					if (currentChar == ALIGNMENT_SEPARATE_CHAR || currentChar == FORMAT_STRING_SEPARATE_CHAR)
					{
						isIndexAlreadyParsed = true;
						this.ArgumentIndex = int.Parse(format.Substring(currentComponentStartIndex, i - currentComponentStartIndex));
						currentComponentStartIndex = i + 1;

						if (currentChar == FORMAT_STRING_SEPARATE_CHAR)
						{
							isAlignmentAlreadyParsed = true;
							break;
						}
					}
					else if (!char.IsDigit(currentChar))
						throw new FormatException("You must use only digit in the `ArgumentIndex` segment");
				}
				else if (currentChar == FORMAT_STRING_SEPARATE_CHAR)
				{
					isAlignmentAlreadyParsed = true;
					if (currentComponentStartIndex == i)
						throw new FormatException("Alignment must have one value (You must put a number after a comma)");

					this.Alignment = int.Parse(format.Substring(currentComponentStartIndex, i - currentComponentStartIndex));
					currentComponentStartIndex = i + 1;
					break;
				}
			}

			if (currentComponentStartIndex != closeBraceIndex)
			{
				string unparsedString = format.Substring(currentComponentStartIndex, closeBraceIndex - currentComponentStartIndex);

				if (!isIndexAlreadyParsed)
				{
					this.ArgumentIndex = int.Parse(unparsedString);
				}
				else if (!isAlignmentAlreadyParsed)
				{
					this.Alignment = int.Parse(unparsedString);
				}
				else
				{
					this.FormatString = unparsedString;
				}
			}
			else if (!isAlignmentAlreadyParsed)
				throw new FormatException("Alignment must have one value (You must put a number after a comma)");
		}

		private string ApplyAlignment(string str)
		{
			int absAlignment = Math.Abs(this.Alignment);

			return this.Alignment < 0
				? str.PadRight(absAlignment)
				: str.PadLeft(absAlignment);
		}

		// Alignmentはまだ適用しない
		internal string ObjToString(object obj, IFormatProvider formatProvider)
		{
			if (obj == null)
				return string.Empty;

			if (IsAssignableTo(obj, typeof(ICustomFormatter)))
			{
				ICustomFormatter customFormatter = (ICustomFormatter)obj;

				string result = customFormatter.Format(this.FormatString, obj, formatProvider);

				if (result != null)
					return result;
			}

			if (IsAssignableTo(obj, typeof(IFormattable)))
			{
				IFormattable formattable = (IFormattable)obj;

				string result = formattable.ToString(this.FormatString, formatProvider);

				if (result != null)
					return result;
			}

			string toStringResult = obj.ToString();

			return toStringResult == null ? string.Empty : toStringResult;
		}

		static bool IsAssignableTo(object obj, Type typeToAssignTo)
		{
#if NETSTANDARD2_0_OR_GREATER || NETFRAMEWORK
			return typeToAssignTo.IsAssignableFrom(obj.GetType());
#else
			return typeToAssignTo.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
#endif
		}

		#region Format
		public string Format(object arg0)
		{
			return FormatWithFormatProvider(null, arg0);
		}
		public string Format(object arg0, object arg1)
		{
			return FormatWithFormatProvider(null, arg0, arg1);
		}
		public string Format(object arg0, object arg1, object arg2)
		{
			return FormatWithFormatProvider(null, arg0, arg1, arg2);
		}
		public string Format(object[] args)
		{
			return FormatWithFormatProvider(null, args);
		}

		public string FormatWithFormatProvider(IFormatProvider formatProvider, object arg0)
		{
			switch (this.ArgumentIndex)
			{
				case ARG_INDEX_MEANING_OF_ESCAPE_OPEN_BRACE:
					return OPEN_BRACE_STR;
				case ARG_INDEX_MEANING_OF_ESCAPE_CLOSE_BRACE:
					return CLOSE_BRACE_STR;

				case 0:
					return ApplyAlignment(ObjToString(arg0, formatProvider));

				default:
					throw new IndexOutOfRangeException("The specified Index is out of range of the given arguments");
			}
		}
		public string FormatWithFormatProvider(IFormatProvider formatProvider, object arg0, object arg1)
		{
			switch (this.ArgumentIndex)
			{
				case ARG_INDEX_MEANING_OF_ESCAPE_OPEN_BRACE:
					return OPEN_BRACE_STR;
				case ARG_INDEX_MEANING_OF_ESCAPE_CLOSE_BRACE:
					return CLOSE_BRACE_STR;

				case 0:
					return ApplyAlignment(ObjToString(arg0, formatProvider));
				case 1:
					return ApplyAlignment(ObjToString(arg1, formatProvider));

				default:
					throw new IndexOutOfRangeException("The specified Index is out of range of the given arguments");
			}
		}
		public string FormatWithFormatProvider(IFormatProvider formatProvider, object arg0, object arg1, object arg2)
		{
			switch (this.ArgumentIndex)
			{
				case ARG_INDEX_MEANING_OF_ESCAPE_OPEN_BRACE:
					return OPEN_BRACE_STR;
				case ARG_INDEX_MEANING_OF_ESCAPE_CLOSE_BRACE:
					return CLOSE_BRACE_STR;

				case 0:
					return ApplyAlignment(ObjToString(arg0, formatProvider));
				case 1:
					return ApplyAlignment(ObjToString(arg1, formatProvider));
				case 2:
					return ApplyAlignment(ObjToString(arg2, formatProvider));

				default:
					throw new IndexOutOfRangeException("The specified Index is out of range of the given arguments");
			}
		}

		public string FormatWithFormatProvider(IFormatProvider formatProvider, object[] args)
		{
			switch (this.ArgumentIndex)
			{
				case ARG_INDEX_MEANING_OF_ESCAPE_OPEN_BRACE:
					return OPEN_BRACE_STR;
				case ARG_INDEX_MEANING_OF_ESCAPE_CLOSE_BRACE:
					return CLOSE_BRACE_STR;

				default:
					if (args.Length <= this.ArgumentIndex)
						throw new IndexOutOfRangeException("The specified Index is out of range of the given arguments");

					return ApplyAlignment(ObjToString(args[this.ArgumentIndex], formatProvider));
			}
		}
		#endregion

		#region ToString(), Equals() and GetHashCode()
		public override string ToString()
		{
			return "FormatItemInfo{" + ArgumentIndex + ',' + Alignment + ':' + FormatString + '}';
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			return this.Equals((FormatItemInfo)obj);
		}

		public bool Equals(FormatItemInfo v)
		{
			return (this.ArgumentIndex == v.ArgumentIndex)
				&& (this.Alignment == v.Alignment)
				&& (this.FormatString == v.FormatString);
		}

		public override int GetHashCode()
		{
			int formatStringHashCode = this.FormatString == null ? 0 : this.FormatString.GetHashCode();

			return this.ArgumentIndex.GetHashCode() ^ this.Alignment.GetHashCode() ^ formatStringHashCode;
		}
		#endregion
	}
}
