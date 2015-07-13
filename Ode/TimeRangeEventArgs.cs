using System;

namespace Kekstoaster.Math
{
	[Serializable]
	public sealed class TimeRangeEventArgs : EventArgs
	{
		public TimeRangeEventArgs (double t)
		{
			Time = t;
		}

		public double Time { get; private set;}

	}
}

