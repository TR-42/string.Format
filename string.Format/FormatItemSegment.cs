using System;

namespace TR
{
	internal class FormatItemSegment
	{
		public readonly int StartIndex;
		public readonly int Length;

		public FormatItemSegment(int StartIndex, int Length)
		{
			if (StartIndex < 0)
				throw new ArgumentOutOfRangeException("StartIndex", "must be positive");
			if (Length <= 3)
				throw new ArgumentOutOfRangeException("Length", "must be more than 3 (Open Brace, Index Number, and Close Brace)");
			if ((int.MaxValue - Length) < StartIndex)
				throw new OverflowException("`StartIndex + Length` will overflow with given values");

			this.StartIndex = StartIndex;
			this.Length = Length;
		}
	}
}
