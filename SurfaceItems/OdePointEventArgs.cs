using System;
using System.Numerics;

namespace SurfaceItems
{
	public class OdePointEventArgs:EventArgs
	{
		public Complex Value { get; private set; }
		public int Index { get; private set;}

		public OdePointEventArgs (int index, Complex z)
		{
			Index = index;
			Value = z;
		}
	}
}

