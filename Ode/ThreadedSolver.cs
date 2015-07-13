using System;
using System.Threading;
using System.Collections.Generic;
using System.Numerics;

namespace Kekstoaster.Math
{
	public class ThreadedSolver:OdeSolver
	{
		private Stepper _step;
		// Threading
		private int _threadCount;
		private AutoResetEvent[] _resets;
		private object syncLock = new object();

		public ThreadedSolver (Stepper stepper, int threadCount)
		{
			this._step = stepper;
			this._threadCount = threadCount;

			this._threadCount = System.Math.Max(1, threadCount);
			if(this._threadCount > 1) {
				_resets = new AutoResetEvent[this._threadCount];
				for (int i = 0; i < this._threadCount; i++) {
					_resets [i] = new AutoResetEvent (false);
				}
			}
		}

		#region OdeSolver implementation

		public TimeList<Complex[]> Solve (Ode ode, Complex[] y0)
		{
			throw new NotImplementedException ();
		}

		public TimeList<Complex[]> Solve (Ode ode, Complex[] y0, double t)
		{
			if(_threadCount > 1) {
				return CalcThreaded (ode, y0, t);
			} else {
				return CalcSingle (ode, y0, t);
			}
		}

		public void SolveToFile (Ode ode, Complex[] y0, string fileName)
		{
			throw new NotImplementedException ();
		}

		public void SolveToFile (Ode ode, Complex[] y0, double t, string fileName)
		{
			throw new NotImplementedException ();
		}

		#endregion

		private TimeList<Complex[]> CalcThreaded(Ode ode, Complex[] y0, double t_max) {
			int ptr = 0;
			int N = ode.N;
			double t = 0;
			TimeList<Complex[]> solve = new TimeList<Complex[]> ();
			Complex[] y = new Complex[N];
			Complex[] lastY = y0;

			for (int thread = 0; thread < _threadCount; thread++) {
				ThreadPool.QueueUserWorkItem (new WaitCallback (delegate(object data) {
					int c;
					while (t < t_max) {
						while ((c = Interlocked.Increment(ref ptr)) <= N) {
							y[c - 1] = _step.NextStepComponent(c - 1, ode, t, lastY);
						}
						lock(syncLock) {
							((AutoResetEvent)data).Set();
							Monitor.Wait(syncLock);
						}
					}

				}), _resets [thread]);
			}

			for (t = 0; t < t_max; t += _step.Stepsize) {
				WaitHandle.WaitAll (_resets);

				lock(syncLock) {
					ptr = 0;
					solve.Add(t, y);
					lastY = y;
					y = new Complex[N];
					Monitor.PulseAll (syncLock);
				}
			}

			return solve;
		}

		private TimeList<Complex[]> CalcSingle(Ode ode, Complex[] y0, double t_max) {
			TimeList<Complex[]> solve = new TimeList<Complex[]> ();
			Complex[] y = y0;

			for(double t = 0; t < t_max; t += _step.Stepsize) {
				y = _step.NextStep (ode, t, y);
				solve.Add (t, y);
			}

			return solve;
		}
	}
}

