using System;

namespace SurfaceItems
{
	public class MouseEventArgs:EventArgs
	{
		public MouseButton Button { get; private set; }
		public int X { get; private set; }
		public int Y { get; private set; }

		public MouseEventArgs (int x, int y, MouseButton btn)
		{
			X = x;
			Y = y;
			Button = btn;
		}

		public MouseEventArgs (int x, int y, uint btn): this(x, y, (MouseButton)btn) {

		}
	}
}

