using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kekstoaster.Math
{
	public interface OdeSolver
	{
		TimeList<Complex[]> Solve(Ode ode, Complex[] y0);
		TimeList<Complex[]> Solve(Ode ode, Complex[] y0, double t);
		void SolveToFile(Ode ode, Complex[] y0, string fileName);
		void SolveToFile(Ode ode, Complex[] y0, double t, string fileName);
	}
}

