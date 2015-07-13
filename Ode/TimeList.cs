using System;
using System.Collections.Generic;
using System.Collections;

namespace Kekstoaster.Math
{
	public class TimeList<T>: IDictionary<double, T>, ICollection, ICollection<KeyValuePair<double, T>>, IEnumerable<KeyValuePair<double, T>>, IEnumerable
	{
		//private List<KeyValuePair<double, T>> _list;
		private List<T[]> _values;
		private List<double[]> _times;
		private double _avg;
		private double _sum;
		private int _N;
		private ChooseClosest _choose;
		private int _capacity;
		private double _maxTime;
		//private double _first;
		private object _listLock = new object ();

		public TimeList (ChooseClosest choose = ChooseClosest.Below, int capacity = 10000)
		{
			//_list = new List<KeyValuePair<double, T>> ();
			_values = new List<T[]>();
			_times = new List<double[]>();
			_capacity = capacity;

			_avg = 0;
			_sum = 0;
			_N = 0;
			_maxTime = 0;
			_choose = choose;
		}

		public double MaxTime {
			get {
				return _maxTime;
			}
		}

		#region IDictionary implementation
		public void Add (double t, T value)
		{
			//Console.WriteLine ("tx: {0}", t);
			if(t < 0) throw new ArgumentException("t must be bigger 0 or positive.");
			lock (_listLock) {
				_sum += t;
				if (_N > 0) {
					_avg = 2 * _sum / (_N * (_N + 1));
				}
				_maxTime = System.Math.Max (t, _maxTime);
				int i = ClosedIndex (ref t, ChooseClosest.Above);			
				_N++;

				if (i + 1 == _N) {
					if (i / _capacity >= _times.Count) {
						_times.Add (new double[_capacity]);
						_values.Add (new T[_capacity]);
					}
					_times [i / _capacity] [i % _capacity] = t;
					_values [i / _capacity] [i % _capacity] = value;
				} else {
					throw new NotImplementedException ();
				}
			}

			//_list.Insert (i, new KeyValuePair<double, T> (t, value));
		}
		public bool ContainsKey (double key)
		{
			return _N > 0;
		}
		public bool Remove (double key)
		{
			if (_N > 0) {
				throw new NotImplementedException ();
				int i = ClosedIndex (ref key, _choose);


				//_list.RemoveAt (i);

				return true;
			} else {
				return false;
			}
		}
		public bool TryGetValue (double key, out T value)
		{
			if(_N > 0) {
				lock (_listLock) {
					int i = System.Math.Max (ClosedIndex (ref key, _choose), _N - 1);
					value = _values [i / _capacity] [i % _capacity];
				}
				return true;
			} else {
				value = default(T);
				return false;
			}
		}
		public T this [double index] {
			get {
				T r = default(T);
				lock (_listLock) {
					int i = ClosedIndex (ref index, _choose);
					r = _values [i / _capacity] [i % _capacity];
				}
				return r;
			}
			set {
				lock (_listLock) {
					int i = System.Math.Max (ClosedIndex (ref index, _choose), _N - 1);
					_values [i / _capacity] [i % _capacity] = value;
				}
			}
		}
		public double TimeOf(int index) {
			return _times [index / _capacity] [index % _capacity];
		}

		public ICollection<double> Keys {
			get {
				double[] keys = new double[_N];
				lock (_listLock) {
					for (int i = 0; i < _N; i++) {
						keys [i] = _times [i / _capacity] [i % _capacity];
					}
				}
				return keys;
			}
		}
		public ICollection<T> Values {
			get {
				throw new NotImplementedException ();
			}
		}
		#endregion
		#region ICollection implementation
		public void Add (KeyValuePair<double, T> item)
		{
			throw new NotImplementedException ();
		}
		public void Clear ()
		{
			lock (_listLock) {
				_values.Clear ();
				_times.Clear ();
				_avg = 0;
				_sum = 0;
				_N = 0;
				_maxTime = 0;
			}
		}
		public bool Contains (KeyValuePair<double, T> item)
		{
			throw new NotImplementedException ();
		}
		public void CopyTo (KeyValuePair<double, T>[] array, int arrayIndex)
		{
			throw new NotImplementedException ();
		}
		public bool Remove (KeyValuePair<double, T> item)
		{
			throw new NotImplementedException ();
		}
		public int Count {
			get {
				return _N;
			}
		}
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		#endregion

		#region IEnumerable implementation
		public IEnumerator<KeyValuePair<double, T>> GetEnumerator ()
		{
			throw new NotImplementedException ();
		}
		#endregion
		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			throw new NotImplementedException ();
		}
		#endregion

		public int Index(double t) {
			return ClosedIndex (ref t, _choose);
		}

		private int ClosedIndex(ref double t, ChooseClosest c) {
			//Console.Write("time: ");
			//Console.WriteLine (t);
			int listCount = _N;

			if(listCount == 0) {
				return 0;
			}

			int tryIndex;
			if(_avg > 0) {
				tryIndex = System.Math.Min((int)(t / _avg), listCount - 1);
			} else {
				tryIndex = 0;
			}

			double currentTime = _times[tryIndex / _capacity] [tryIndex % _capacity];
			//Console.Write ("CurrentTime: ");
			//Console.WriteLine (currentTime);
			if(currentTime == t) {
				return tryIndex;
			}

			switch (c) {
			case ChooseClosest.Above:
				if(currentTime < t) {
					while(currentTime < t && tryIndex < listCount) {
						tryIndex++;
						if (tryIndex < listCount) {
							currentTime = _times[tryIndex / _capacity] [tryIndex % _capacity];
						}
					}
					return tryIndex;
				} else {
					while(currentTime > t && tryIndex - 1 >= 0) {
						tryIndex--;
						currentTime = _times[tryIndex / _capacity] [tryIndex % _capacity];
					}
					return tryIndex + 1;
				}
			case ChooseClosest.Below:
				if(currentTime < t) {
					while(currentTime < t && tryIndex < listCount) {
						tryIndex++;
						if (tryIndex < listCount) {
							currentTime = _times[tryIndex / _capacity] [tryIndex % _capacity];
						}
					}
					return tryIndex - 1;
				} else {
					while(currentTime > t && tryIndex - 1 >= 0) {
						tryIndex--;
						currentTime = _times[tryIndex / _capacity] [tryIndex % _capacity];
					}
					return tryIndex;
				}
			case ChooseClosest.Distant:
				int otherIndex;
				double otherTime;
				if(listCount == 1) {
					return 0;
				}
				if (currentTime < t) {
					while (currentTime < t && tryIndex + 1 < listCount) {
						tryIndex++;
						currentTime = _times[tryIndex / _capacity] [tryIndex % _capacity];
					}
					if (currentTime < t && tryIndex == listCount - 1) { // If above last index, no distance needed, get last index
						return tryIndex;
					} else {
						otherIndex = tryIndex - 1;
					}
				} else {
					while(currentTime > t && tryIndex - 1 >= 0) {
						tryIndex--;
						currentTime = _times[tryIndex / _capacity] [tryIndex % _capacity];
					}
					if(currentTime > t && tryIndex == 0) {// If below frist index, no distance needed, get first index 0
						return 0;
					} else {
						otherIndex = tryIndex + 1;
					}
				}

				otherTime = _times[otherIndex / _capacity] [otherIndex % _capacity];
				if(System.Math.Abs(t - currentTime) < System.Math.Abs(t - otherTime)) {
					return tryIndex;
				} else {
					return otherIndex;
				}
			default:
				throw new ArgumentException("Method for choosing closest index unknown.");
			}
		}

		#region ICollection implementation

		public void CopyTo (Array array, int index)
		{
			throw new NotImplementedException ();
		}

		public bool IsSynchronized {
			get {
				return false;
			}
		}

		public object SyncRoot {
			get {
				throw new NotImplementedException ();
			}
		}

		#endregion

		public T Get(int index) {
			if (index < _N) {
				return _values [index / _capacity] [index % _capacity];
			} else {
				throw new IndexOutOfRangeException ();
			}
		}
	}

	public enum ChooseClosest {
		Distant,
		Below,
		Above
	}
}

