using System;
using System.Numerics;

namespace Chimera
{
	public class AWP:ICloneable
	{
		private int _N;
		private int _P;
		private double _lambda;
		private double _omega;
		private double _K;
		private double _beta;
		private Complex[] _y0;

		public event EventHandler Changed;

		public AWP (int N, int P, double lambda, double omega, double K, double beta, Complex[] y0) {
			this._N = N;
			this._P = P;
			this._lambda = lambda;
			this._omega = omega;
			this._K = K;
			this._beta = beta;
			this._y0 = y0;
		}	

		#region ICloneable implementation
		object ICloneable.Clone ()
		{
			return this.Clone ();
		}

		public AWP Clone() {
			return new AWP (_N, _P, _lambda, _omega, _K, _beta, (Complex[])_y0.Clone ());
		}

		#endregion

		public void Use(AWP awp) {
			this._N = awp._N;
			this._P = awp._P;
			this._lambda = awp._lambda;
			this._omega = awp._omega;
			this._K = awp._K;
			this._beta = awp._beta;
			this._y0 = (Complex[])awp._y0.Clone ();
			if(Changed != null) {
				Changed (this, EventArgs.Empty);
			}
		}

		public int N {
			get {
				return _N;
			}
			set {
				if (value > 1 && value != _N) {
					Complex[] n = new Complex[value];
					if(value < _N) {
						Array.Copy (_y0, n, value);
					} else {
						Array.Copy (_y0, n, _N);
						for(int i = _N; i < value; i++) {
							n [i] = _y0 [_N - 1];
						}
					}
					_N = value;
					Y = n;
				}

			}
		}

		public double Lambda {
			get {
				return _lambda;
			}
			set {
				if(value != _lambda && value >= 0) {
					_lambda = value;
					if(Changed != null) {
						Changed (this, EventArgs.Empty);
					}
				}
			}
		}

		public double Omega {
			get {
				return _omega;
			}
			set {
				if (value != _omega && value >= 0) {
					_omega = value;
					if(Changed != null) {
						Changed (this, EventArgs.Empty);
					}
				}
			}
		}

		public int P {
			get {
				return _P;
			}
			set {
				if(value != _P && value >= 0) {
					_P = value;
					if(Changed != null) {
						Changed (this, EventArgs.Empty);
					}
				}
			}
		}

		public double K {
			get {
				return _K;
			}
			set {
				if(value != _K && value >= 0) {
					_K = value;
					if(Changed != null) {
						Changed (this, EventArgs.Empty);
					}
				}
			}
		}

		public double Beta {
			get {
				return _beta;
			}
			set {
				if(value != _beta && value >= 0) {
					_beta = value;
					if(Changed != null) {
						Changed (this, EventArgs.Empty);
					}
				}
			}
		}

		public Complex[] Y {
			get {
				return _y0;
			}
			set {
				_y0 = value;
				_N = value.Length;
				if(Changed != null) {
					Changed (this, EventArgs.Empty);
				}
			}
		}
	}
}