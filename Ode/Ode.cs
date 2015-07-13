using System;
using System.Numerics;

namespace Kekstoaster.Math
{
	public interface Ode
	{
		Complex[] Calc (double t, Complex[] y);
		Complex CalcComponent (int j, double t, Complex[] y);
		int N { get;}
	}
}

