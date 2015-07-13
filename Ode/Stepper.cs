using System;
using System.Numerics;

namespace Kekstoaster.Math
{
	public interface Stepper
	{
		Complex[] NextStep (Ode ode, double t, Complex[] y);
		Complex NextStepComponent (int j, Ode ode, double t, Complex[] y);

		double Stepsize{ get; }
	}
}

