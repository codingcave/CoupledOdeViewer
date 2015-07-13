using System;

namespace SurfaceItems
{
	public class LabelEventArgs:EventArgs
	{
		public string Label { get; private set; }

		public LabelEventArgs (string label)
		{
			this.Label = label;
		}
	}
}

