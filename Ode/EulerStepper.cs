using System;
using System.Numerics;

namespace Kekstoaster.Math
{
	public class EulerStepper:Stepper
	{
		private Complex _h;

		public EulerStepper (double h)
		{
			this._h = new Complex (h, 0);
		}

		#region Stepper implementation

		public Complex[] NextStep (Ode ode, double t, Complex[] y)
		{
			Complex[] n = ode.Calc (t, y);
			for(int i = 0; i < y.Length; i++) {
				n[i] = y[i] + n[i] * _h;
			}
			return n;
		}

		public Complex NextStepComponent (int j, Ode ode, double t, Complex[] y)
		{
			return y [j] + ode.CalcComponent (j, t, y) * _h;
		}

		public double Stepsize{ 
			get {
				return _h.Real;
			} 
		}

		#endregion
	}
}

