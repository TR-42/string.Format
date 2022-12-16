using System;

namespace TR
{
	internal class MyStringFormatter
	{
		const char OPEN_BRACE = '{';
		const char CLOSE_BRACE = '}';
		const int INDEX_NO_VALUE = -1;

		internal static int CountFormatItemAndEscapingBrace(string format)
		{
			int numOfFormatItemAndEscapingBrace = 0;

			int indexOfFormatItemBeginBrace = INDEX_NO_VALUE;
			for (int i = 0; i < format.Length; i++)
			{
				if (format[i] == OPEN_BRACE)
				{
					// FormatItem開始位置の記録が初期位置なら、このBraceがFormatItemの開始を意味するかもしれない
					// 直前にOpenBraceが存在したなら、それはOpenBraceのエスケープ
					// FormatItem内にBraceは存在できないため、エラー
					if (indexOfFormatItemBeginBrace == INDEX_NO_VALUE)
						indexOfFormatItemBeginBrace = i;
					else if (indexOfFormatItemBeginBrace == (i - 1))
					{
						numOfFormatItemAndEscapingBrace++;
						indexOfFormatItemBeginBrace = INDEX_NO_VALUE;
					}
					else
						throw new FormatException("Brace cannot use in the Format item");
				}
				else if (format[i] == CLOSE_BRACE)
				{
					if (indexOfFormatItemBeginBrace == INDEX_NO_VALUE)
					{
						// FormatItemが開始していないのに終了しようとした
						// (Close Braceのエスケープではなかった)
						if (i == (format.Length - 1) || format[i + 1] != CLOSE_BRACE)
							throw new FormatException("Invalid Location Brace");

						i++;
						numOfFormatItemAndEscapingBrace++;
					}
					else
					{
						// このClose BraceはFormat Itemの終了を表すものなので、
						// カウンタのインクリメントと開始位置記録の初期化を行う
						numOfFormatItemAndEscapingBrace++;
						indexOfFormatItemBeginBrace = INDEX_NO_VALUE;
					}
				}
			}

			if (indexOfFormatItemBeginBrace != INDEX_NO_VALUE)
			{
				throw new FormatException("Brace not Closing");
			}

			return numOfFormatItemAndEscapingBrace;
		}

		internal static FormatItemSegment[] GetFormatItemSegments(string format, int numOfFormatItemAndEscapingBrace)
		{
			// `CountFormatItem`にて
			FormatItemSegment[] segments = new FormatItemSegment[numOfFormatItemAndEscapingBrace];
			int segmentIndex = 0;

			int indexOfFormatItemBeginBrace = INDEX_NO_VALUE;
			for (int i = 0; i < format.Length; i++)
			{
				if (format[i] == OPEN_BRACE)
				{
					// エスケープされたBrace
					if (format[i + 1] == OPEN_BRACE)
					{
						segments[segmentIndex++] = new FormatItemSegment(i, 2);
						i++;
						continue;
					}

					indexOfFormatItemBeginBrace = i;
				}
				else if (format[i] == CLOSE_BRACE)
				{
					// エスケープされたBrace
					if (indexOfFormatItemBeginBrace == INDEX_NO_VALUE)
					{
						segments[segmentIndex++] = new FormatItemSegment(i, 2);
						i++;
					}
					else
					{
						segments[segmentIndex++] = new FormatItemSegment(indexOfFormatItemBeginBrace, i - indexOfFormatItemBeginBrace);
						indexOfFormatItemBeginBrace = INDEX_NO_VALUE;
					}
				}
			}

			return segments;
		}

		readonly IFormatProvider FormatProvider;
		readonly string GivenFormatStr;
		readonly FormatItemSegment[] FormatItemSegmentArray;
		readonly FormatItemInfo[] FormatItemInfoArray;
		readonly int SumOfFormatItemSegmentLength;

		public MyStringFormatter(string format) : this(null, format) { }

		public MyStringFormatter(IFormatProvider formatProvider, string format)
		{
			int numOfFormatItemAndEscapingBrace = CountFormatItemAndEscapingBrace(format);

			GivenFormatStr = format;
			FormatProvider = formatProvider;
			FormatItemSegmentArray = GetFormatItemSegments(format, numOfFormatItemAndEscapingBrace);

			foreach (FormatItemSegment v in FormatItemSegmentArray)
				SumOfFormatItemSegmentLength += v.Length;
		}

		internal int GetOutputLength(string[] formatItemStrings)
		{
			int sumOfFormatItemStringLength = 0;
			foreach (string v in formatItemStrings)
				sumOfFormatItemStringLength += v.Length;

			return GivenFormatStr.Length - SumOfFormatItemSegmentLength + sumOfFormatItemStringLength;
		}

		internal string Concat(string[] formatItemStrings)
		{
			int outputLength = GetOutputLength(formatItemStrings);
			char[] output = new char[outputLength];

			int lastIndexOfFormatStr = 0;
			int lastIndexOfOutputArray = 0;
			for (int i = 0; i < FormatItemSegmentArray.Length; i++)
			{
				FormatItemSegment currentSeg = FormatItemSegmentArray[i];
				if (lastIndexOfFormatStr != currentSeg.StartIndex)
				{
					// 直前までは単なる文字列だった
					// => その文字列を単純にoutputにcopyする

					int length = currentSeg.StartIndex - lastIndexOfFormatStr;
					GivenFormatStr.CopyTo(lastIndexOfFormatStr, output, lastIndexOfOutputArray, length);
					lastIndexOfOutputArray += length;
				}

				string strToCopy = formatItemStrings[i];
				strToCopy.CopyTo(0, output, lastIndexOfOutputArray, strToCopy.Length);

				lastIndexOfFormatStr = currentSeg.StartIndex + currentSeg.Length;
				lastIndexOfOutputArray += strToCopy.Length;
			}

			FormatItemSegment lastSeg = FormatItemSegmentArray[FormatItemSegmentArray.Length - 1];
			int nextIndexOfLastSeg = lastSeg.StartIndex + lastSeg.Length;
			if (GivenFormatStr.Length != nextIndexOfLastSeg)
			{
				// 最後のSegmentの後にまだ文字が存在する
				int length = GivenFormatStr.Length - nextIndexOfLastSeg;
				GivenFormatStr.CopyTo(nextIndexOfLastSeg, output, lastIndexOfOutputArray, length);
			}

			return new string(output);
		}

		public string Format(object arg0)
		{
			if (FormatItemSegmentArray.Length <= 0)
				return GivenFormatStr;

			string[] formatItemStrings = new string[FormatItemInfoArray.Length];

			for (int i = 0; i < FormatItemInfoArray.Length; i++)
			{
				formatItemStrings[i] = FormatItemInfoArray[i].FormatWithFormatProvider(FormatProvider, arg0);
			}

			return Concat(formatItemStrings);
		}

		public string Format(object arg0, object arg1)
		{
			if (FormatItemSegmentArray.Length <= 0)
				return GivenFormatStr;

			string[] formatItemStrings = new string[FormatItemInfoArray.Length];

			for (int i = 0; i < FormatItemInfoArray.Length; i++)
			{
				formatItemStrings[i] = FormatItemInfoArray[i].FormatWithFormatProvider(FormatProvider, arg0, arg1);
			}

			return Concat(formatItemStrings);
		}

		public string Format(object arg0, object arg1, object arg2)
		{
			if (FormatItemSegmentArray.Length <= 0)
				return GivenFormatStr;

			string[] formatItemStrings = new string[FormatItemInfoArray.Length];

			for (int i = 0; i < FormatItemInfoArray.Length; i++)
			{
				formatItemStrings[i] = FormatItemInfoArray[i].FormatWithFormatProvider(FormatProvider, arg0, arg1, arg2);
			}

			return Concat(formatItemStrings);
		}

		public string Format(object[] args)
		{
			if (FormatItemSegmentArray.Length <= 0)
				return GivenFormatStr;

			string[] formatItemStrings = new string[FormatItemInfoArray.Length];

			for (int i = 0; i < FormatItemInfoArray.Length; i++)
			{
				formatItemStrings[i] = FormatItemInfoArray[i].FormatWithFormatProvider(FormatProvider, args);
			}

			return Concat(formatItemStrings);
		}
	}
}

