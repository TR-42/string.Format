using System;

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
	}
}
