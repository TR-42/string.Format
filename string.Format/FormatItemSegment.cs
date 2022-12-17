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
			if (Length < 2)
				throw new ArgumentOutOfRangeException("Length", "must be same or more than 2");
			if ((int.MaxValue - Length) < StartIndex)
				throw new OverflowException("`StartIndex + Length` will overflow with given values");

			this.StartIndex = StartIndex;
			this.Length = Length;
		}

		#region ToString(), Equals() and GetHashCode()
		public override string ToString()
		{
			return "FormatItemSegment{ StartIndex:" + StartIndex + ", Length:" + Length + " }";
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			return this.Equals((FormatItemSegment)obj);
		}

		public bool Equals(FormatItemSegment v)
		{
			return (this.StartIndex == v.StartIndex)
				&& (this.Length == v.Length);
		}

		public override int GetHashCode()
		{
			return this.StartIndex.GetHashCode() ^ this.StartIndex.GetHashCode();
		}
		#endregion

	}
}
