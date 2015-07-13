using System;

namespace SurfaceItems
{
	public class ResizeEventArgs:EventArgs
	{
		public int Width { get; private set; }
		public int Height { get; private set; }

		public ResizeEventArgs (int height, int width)
		{
			Width = width;
			Height = height;
		}
	}
}

