using System;
using System.Numerics;
using System.Threading;
using Kekstoaster.Math;

namespace Chimera
{
	public class Integrator
	{
		private AWP _awp;

		private bool _changed;

		private CoupledStuardLandau _ode;
		private TimeList<Complex[]> _list;
		private Stepper _stepper;
		private object _listLock = new object();
		private ManualResetEvent _editPause;

		private bool _calculating;
				
		private double _maxTime;
		private double _triggerRange;
		private double _lastTime;
		private double _h;
		private Random _rnd = new Random ();

		private bool _editMode;

		public event EventHandler CalculationStarted;
		public event EventHandler CalculationFinished;
		public event EventHandler<TimeRangeEventArgs> TimeRange;

		public Integrator (double h, double maxTime, AWP awp = null)
		{
			_editPause = new ManualResetEvent (true);
			_h = h;
			_stepper = new RungeKuttaStepper (_h);
			//_stepper = new EulerStepper(_h);
			_calculating = false;

			_maxTime = maxTime;
			int capacity = (int)(maxTime / h) + 5;
			_list = new TimeList<Complex[]> (ChooseClosest.Distant, capacity);
			_triggerRange = 10;

			if (awp != null) {
				_awp = awp.Clone ();
			} else {
				Complex[] y0 = new Complex[100];

				for (int i = 0; i < y0.Length; i++) {
					//y0 [i] = new Complex(1, rnd.NextDouble() * 2 - 1);
					//y0 [i] = new Complex (1, i < y0.Length / 2 ? -1 : 1);
					y0 [i] = Complex.FromPolarCoordinates(1, _rnd.NextDouble() * 2 * Math.PI);
					//y0 [i] = Complex.FromPolarCoordinates(1, i < y0.Length / 2 ? Math.PI / 2 : Math.PI * 3 / 2);
				}
				_awp = new AWP (100, 4, 1, 2, 14, 0, y0);
			}
			_awp.Changed+= delegate(object sender, EventArgs e) {
				_changed = true;
				Recalc ();
			};
			_changed = true;
			Recalc ();
		}

		public bool EditMode {
			get {
				return this._editMode;
			}
			set {
				if (this._editMode != value) {
					this._editMode = value;
					if (value) {
						_editPause.Reset ();
					} else {
						_editPause.Set ();
						Recalc ();
					}
				}
			}
		}

		public AWP AWP {
			get {
				return _awp;
			}
		}

		public void RandomAwp() {
			for (int i = 0; i < _awp.Y.Length; i++) {
				_awp.Y [i] = Complex.FromPolarCoordinates(_rnd.NextDouble(), _rnd.NextDouble() * 2 * Math.PI);
			}
			_changed = true;
			Recalc ();
		}

		public void TwoStepsAwp() {
			for (int i = 0; i < _awp.Y.Length; i++) {
				_awp.Y [i] = new Complex(i < _awp.Y.Length / 2 ? 0.2 : -0.2, i < _awp.Y.Length / 2 ? -1 : 1);
			}
			_changed = true;
			Recalc ();
		}

		public TimeList<Complex[]> TimeList {
			get {
				return _list;
			}
		}

		private void Recalc() {
			if (!_editMode && _changed) {
				_ode = new CoupledStuardLandau (_awp.Lambda, _awp.Omega, _awp.N, _awp.P, _awp.K, _awp.Beta);
				_calculating = false;
				lock (_listLock) {
					_calculating = true;
					_lastTime = 0;
					_list.Clear ();
					_list.Add (0, _awp.Y);
					System.Threading.Tasks.Task.Run (new Action (Calculate));
				}
			}
		}

		private void Calculate() {
			lock (_listLock) {
				Console.WriteLine ("Calculation started");
				if(CalculationStarted != null) {
					CalculationStarted(this, EventArgs.Empty);
				}

				Complex[] y = _awp.Y;
				_changed = false;

				DateTime start = DateTime.Now;
				//TimeList<Complex[]> solve = new TimeList<Complex[]> ();

				for(double t = _h; t <= _maxTime && _calculating; t += _h) {
					y = _stepper.NextStep (_ode, t, y);
					_list.Add (t, y);
					if(_lastTime + _triggerRange <= t) {
						_lastTime = t;
						if(TimeRange != null) {
							TimeRange (this, new TimeRangeEventArgs (t));
						}
					}
					_editPause.WaitOne ();
				}
				DateTime end = DateTime.Now;
				if(TimeRange != null && _calculating) {
					TimeRange (this, new TimeRangeEventArgs (_list.MaxTime));
				}
				if(_calculating) {
					Console.WriteLine ("Calculation done after {0}s.",(end - start).TotalSeconds);
					_calculating = false;
					//_list = solve;
					if(CalculationFinished != null) {
						CalculationFinished(this, EventArgs.Empty);
					}
				} else {
					Console.WriteLine ("Calculation aborted after {0}s.",(end - start).TotalSeconds);
				}
			}
		}
	}
}

