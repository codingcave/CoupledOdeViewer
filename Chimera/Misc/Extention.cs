using System;

namespace Chimera
{
	public static class Extention
	{
		public static string F(this double x) {
			return x.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}

