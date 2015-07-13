using System;

namespace SurfaceItems
{
	[Serializable]
	public sealed class DisplayModeEventArgs : EventArgs
	{
		public DisplayMode DisplayMode { get; private set; }

		public DisplayModeEventArgs (DisplayMode mode)
		{
			this.DisplayMode = mode;
		}
	}
}

