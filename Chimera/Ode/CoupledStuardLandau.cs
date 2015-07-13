using System;
using System.Threading;
using System.Numerics;
using Kekstoaster.Math;

namespace Chimera
{
	public class CoupledStuardLandau:Ode
	{
		private double _lambda;
		private double _omega;
		private int _N;
		private int _P;
		private Complex _sigma;
		private static Complex i = Complex.ImaginaryOne;
		private Complex _s_2P;

		public Complex StuardLandau(Complex z) {
			return (_lambda + i * _omega - Complex.Abs (z)) * z;
		}

		public CoupledStuardLandau (double lambda, double omega, int N, int P, double K, double beta)
		{
			this._lambda = lambda;
			this._omega = omega;
			this._N = N;
			this._P = P;
			this._sigma = Complex.FromPolarCoordinates (K, beta);
			this._s_2P = _sigma / new Complex(2 * P, 0);
		}

		#region Ode implementation

		public Complex[] Calc (double t, Complex[] y)
		{
			Complex[] n = new Complex[_N];
			for (int i = 0; i < _N; i++) {
				n [i] = CalcComponent (i, t, y);
			}

			return n;
		}

		public Complex CalcComponent (int j, double t, Complex[] y)
		{
			double couple = 0;
			for(int k = j - _P; k <= j + _P; k++) {
				couple += y [(k + _N) % _N].Real - y [j].Real;
			}

			return StuardLandau (y [j]) + _s_2P * new Complex(couple, 0);
		}

		public int N {
			get {
				return this._N;
			}
		}

		#endregion
	}
}

