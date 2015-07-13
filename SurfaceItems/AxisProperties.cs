using System;
using Cairo;
using System.Collections.Generic;

namespace SurfaceItems
{
	public class AxisProperties
	{
		private DisplayTypeSurfaceItem _surface;
		private double _ylimUp;
		private double _ylimDown;
		private List<KeyValuePair<double, string>> _specialValues;
		private AxisDrawType _type;
		private double _delay;
		private int _minCount = 1;
		private int _maxCount = 5;

		private double _absMin;
		private double _absMax;

		private double _lastMin;
		private double _lastMax;

		private KeyValuePair<double, string> _zero;
		private double _zeroPosition;

		const double totalMinimum = 1.2;

		private AxisProperties (DisplayTypeSurfaceItem surface, AxisDrawType type)
		{
			this._type = type;
			this._specialValues = new List<KeyValuePair<double, string>> ();
			this._surface = surface;
			_zero = new KeyValuePair<double, string>(0, "0");
			_zeroPosition = 0.5;
		}

		public static AxisProperties CreateStatic(DisplayTypeSurfaceItem surface, double min, double max) {
			AxisProperties n = new AxisProperties (surface, AxisDrawType.Static);
			n._ylimDown = min;
			n._ylimUp = max;
			return n;
		}

		public static AxisProperties CreateCurrent(DisplayTypeSurfaceItem surface) {
			return new AxisProperties (surface, AxisDrawType.Current);
		}

		public static AxisProperties CreateMaximum(DisplayTypeSurfaceItem surface) {
			return new AxisProperties (surface, AxisDrawType.Maximum);
		}

		public static AxisProperties CreateDelayedMaximum(DisplayTypeSurfaceItem surface, double delayTime) {
			AxisProperties n = new AxisProperties (surface, AxisDrawType.DelayedMaximum);
			n._delay = delayTime;
			return n;
		}

		public void AddValue(double val, string description) {
			_specialValues.Add (new KeyValuePair<double, string> (val, description));
		}

		public void Text(Context context,  string txt, HorizontalTextAlignment horizontal = HorizontalTextAlignment.Center, VerticalTextAlignment vertical = VerticalTextAlignment.Middle) {
			context.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Normal);
			//context.SetFontSize (_surface.Height / 10);
			context.SetFontSize (14);
			var extends = context.TextExtents (txt);
			double x, y;
			x = -(((double)horizontal) / 2 * extends.Width + extends.XBearing);
			y = -(((double)vertical) / 2 * extends.Height + extends.YBearing);
			context.RelMoveTo (x, y);
			context.ShowText (txt);
		}

		public KeyValuePair<double, string>[] AxisSteps(double min, double max) {
			_absMax = Math.Max (max, _absMax);
			_absMin = Math.Min (min, _absMin);

			switch (_type) {
			case AxisDrawType.Static:
				min = _ylimDown;
				max = _ylimUp;
				break;
			case AxisDrawType.Maximum:
			case AxisDrawType.DelayedMaximum:
				min = _absMin;
				max = _absMax;
				break;
			default:
				// use passed min/max
				break;
			}
			_lastMin = min;
			_lastMax = max;
			min = YLimDown;
			max = YLimUp;

			KeyValuePair<double, string>[] steps = new KeyValuePair<double, string>[_maxCount];
			bool[] isFilled = new bool[_maxCount];

			double dist = (max - min) / (_maxCount - 1);

			foreach (var sval in _specialValues) {
				if(sval.Key < max && sval.Key > min) {
					double s = min - dist / 2;
					for(int si = 0; si < _maxCount; si++) {
						if(sval.Key > s && sval.Key < s + dist && !isFilled [si]) {
							steps [si] = sval;
							isFilled [si] = true;
						}
						s += dist;
					}
				}
			}

			double fixDist = (double)((int)(dist * 2)) / 2;
			double m = (int)min;
			for (int f = 0; f < isFilled.Length; f++) {
				if(!isFilled[f]) {
					steps [f] = new KeyValuePair<double, string>(m, m.ToString());
				}
				m += fixDist;
			}

			return steps;
		}

		public double YLimUp {
			get {
				double max = _lastMax;
				double min = _lastMin;
				switch (_type) {
				case AxisDrawType.Static:
					return _ylimUp;
				case AxisDrawType.Maximum:
				case AxisDrawType.DelayedMaximum:
					max = _absMax;
					min = _absMin;
					goto default;
				default:
					double high = max > 0 ? max / _zeroPosition : 0;
					double low = min < 0 ? min / Math.Abs (1 - _zeroPosition) : 0;
					if(high >= low) {
						return Math.Max(max, 2 * totalMinimum * _zeroPosition);
					} else {
						return Math.Max(Math.Abs(min * _zeroPosition / (1 - _zeroPosition)), 2 * totalMinimum * _zeroPosition);
					}
				}
			}
			set {
				if(_ylimUp != value && value > _ylimDown) {
					_ylimUp = value;
				}
			}
		}

		public double YLimDown {
			get { 
				double max = _lastMax;
				double min = _lastMin;
				switch (_type) {
				case AxisDrawType.Static:
					return _ylimDown;
				case AxisDrawType.Maximum:
				case AxisDrawType.DelayedMaximum:
					max = _absMax;
					min = _absMin;
					goto default;
				default:
					double high = max > 0 ? max / _zeroPosition : 0;
					double low = min < 0 ? min / Math.Abs (1 - _zeroPosition) : 0;
					if(low >= high) {
						return Math.Min(min, -2 * totalMinimum * (1 - _zeroPosition));
					} else {
						return Math.Min(-Math.Abs(max * (1 - _zeroPosition) / _zeroPosition), -2 * totalMinimum * (1 - _zeroPosition));
					}
				}
			}
			set {
				if(_ylimDown != value && value < _ylimUp) {
					_ylimDown = value;
				}
			}
		}

		public AxisDrawType AxisType {
			get {
				return _type;
			}
			set {
				_type = value;
			}
		}

		public double ZeroPosition {
			get {
				return _zeroPosition;
			}
			set {
				if(_zeroPosition != value && value >= 0 && value <= 1.0) {
					_zeroPosition = value;
				}
			}
		}

		public void Reset() {
			_lastMax = double.NegativeInfinity;
			_lastMin = double.PositiveInfinity;
			_absMax = double.NegativeInfinity;
			_absMin = double.PositiveInfinity;
		}

		public int MinCount {
			get {
				return _minCount;
			}
			set {
				_minCount = value;
			}
		}

		public int MaxCount {
			get {
				return _maxCount;
			}
			set {
				_maxCount = value;
			}
		}

		public enum AxisDrawType {
			Static,
			Current,
			Maximum,
			DelayedMaximum
		}
	}

	public enum HorizontalTextAlignment {
		Left = 0,
		Center = 1,
		Right = 2
	}

	public enum VerticalTextAlignment {
		Top = 0,
		Middle = 1,
		Bottom = 2
	}
}

