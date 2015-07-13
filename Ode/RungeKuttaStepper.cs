using System;
using System.Numerics;

namespace Kekstoaster.Math
{
	public class RungeKuttaStepper:Stepper
	{
		private Complex _h;

		public RungeKuttaStepper(double h)
		{
			this._h = new Complex (h, 0);
		}

		private Complex[] K1 (double t, Complex[] y) {
			return null;
		}

		private Complex[] K2 (double t, Complex[] y) {
			return null;
		}

		private Complex[] K3 (double t, Complex[] y) {
			return null;
		}

		private Complex[] K4 (double t, Complex[] y) {
			return null;
		}

		#region Stepper implementation
		public Complex[] NextStep (Ode ode, double t, Complex[] y)
		{
			Complex[] ya = new Complex[y.Length];
			Complex[] yb = new Complex[y.Length];
			Complex[] yc = new Complex[y.Length];
			Complex[] yr = new Complex[y.Length];
			Complex[] dya, dyb, dyc;

			Complex[] dy = ode.Calc (t, y);

			for (int i = 0; i < y.Length; i++) {
				ya [i] = y [i] + .5 * _h * dy [i];
			}
			dya = ode.Calc (t + .5 * _h.Real, ya);

			for (int i = 0; i < y.Length; i++) {
				yb [i] = y [i] + .5 * _h * dya [i];
			}
			dyb = ode.Calc (t + .5 * _h.Real, yb);

			for (int i = 0; i < y.Length; i++) {
				yc [i] = y [i] + _h * dyb [i];
			}
			dyc = ode.Calc (t + _h.Real, yc);

			double _1_6 = 1.0 / 6.0;
			for (int i = 0; i < y.Length; i++) {
				yr [i] = y [i] + _1_6 * _h * (dy [i] + 2 * (dya[i] + dyb[i]) + dyc[i]);
			}

			return yr;
		}

		public Complex NextStepComponent (int j, Ode ode, double t, Complex[] y)
		{
			Complex[] ya = new Complex[y.Length];
			Complex[] yb = new Complex[y.Length];
			Complex[] yc = new Complex[y.Length];
			Complex[] dya, dyb, dyc;

			Complex[] dy = ode.Calc (t, y);

			for (int i = 0; i < y.Length; i++) {
				ya [i] = y [i] + .5 * _h * dy [i];
			}
			dya = ode.Calc (t + .5 * _h.Real, ya);

			for (int i = 0; i < y.Length; i++) {
				yb [i] = y [i] + .5 * _h * dya [i];
			}
			dyb = ode.Calc (t + .5 * _h.Real, yb);

			for (int i = 0; i < y.Length; i++) {
				yc [i] = y [i] + _h * dyb [i];
			}
			dyc = ode.Calc (t + _h.Real, yc);


			return y [j] + _h * (dy [j] + 2 * (dya[j] + dyb[j]) + dyc[j]) / 6;

		}
		public double Stepsize {
			get {
				return _h.Real;
			}
		}
		#endregion
	}
}

