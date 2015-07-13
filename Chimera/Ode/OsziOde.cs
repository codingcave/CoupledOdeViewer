using System;
using System.Numerics;
using Kekstoaster.Math;

namespace Chimera
{
	public class OsziOde:Ode
	{
		private Complex _a;
		private Complex _b;

		public OsziOde (double a, double b)
		{
			this._a = new Complex(a, 0);
			this._b = new Complex(b, 0);
		}

		#region Ode implementation

		public Complex[] Calc (double t, Complex[] y)
		{
			Complex[] dy = new Complex[2];
			dy[0] = _a * y [0] + _b * y [1];
			dy[1] = -_a * y [1] - _b * y [0];
			return dy;
		}

		public Complex CalcComponent (int j, double t, Complex[] y)
		{
			Complex x;
			if(j == 0) {
				x = _a * y [0] + _b * y [1];
			} else {
				x = -_a * y [1] - _b * y [0];
			}
			return x;
		}

		public int N {
			get {
				return 2;
			}
		}

		#endregion
	}
}

