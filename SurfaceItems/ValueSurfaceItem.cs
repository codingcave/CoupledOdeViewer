using System;
using Gtk;
using Cairo;
using System.Numerics;
using System.Collections.Generic;
using Kekstoaster.Math;

namespace SurfaceItems
{
	[System.ComponentModel.ToolboxItem(true)]
	public class ValueSurfaceItem:DisplayTypeSurfaceItem
	{
		private Func<Complex, double> _valueExtractor;

		public ValueSurfaceItem (int maxWidth, int maxHeight, int widthRequest = 0, int heightRequest = 0, string label = null):base(maxWidth, maxHeight, widthRequest, heightRequest, label)
		{
			//Console.WriteLine ("ValueSurfaceItem");
			//double ylim, Func<Complex, double> valueExtractor, DrawingArea draw, int maxHeight, int maxWidth, string label, DisplayMode mode = DisplayMode.Snapshot):base(draw, maxHeight, maxWidth, label, mode
			/*
			this._ylim = ylim;
			this._valueExtractor = valueExtractor;
			*/
			this.MouseClick += HandleMouseClick;
		}

		void HandleMouseClick (object sender, MouseEventArgs e)
		{
			int index = PointIndex (e.X);
			if(index != -1) {
				OnPointChanged(new OdePointEventArgs (index, new Complex (PointValue (e.Y), 0)));
			}
		}

		public Func<Complex, double> ValueExtractor {
			get { return _valueExtractor; }
			set { _valueExtractor = value; }
		}

		public int PointIndex(int x) {
			int i = (int)((x - 50) * N / (Width - 60));
			if(i < 0 || i >= N) {
				return -1;
			}
			return i;
		}

		public double PointValue(double y) {
			double v = _axis.YLimUp - y *  (_axis.YLimUp - _axis.YLimDown) / Height;
			//Console.WriteLine ("Value: {0} - {1}", y, v);
			return v;
		}

		private void DrawAxes(Context context, double min, double max) {
			context.Rectangle(0, 0, Width, Height);
			context.SetSourceRGB(Background.R, Background.G, Background.B);
			context.Fill();

			int y = ValueToY (0);
			context.LineWidth = 1;
			context.SetSourceRGB(0.0, 0.0, 0.0);

			context.MoveTo (44, y);
			context.LineTo (Width, y);
			context.StrokePreserve();

			context.MoveTo (50, 0);
			context.LineTo (50, Height);
			context.StrokePreserve();

			KeyValuePair<double, string>[] ax = _axis.AxisSteps (min, max);

			foreach (var a in ax) {
				y = ValueToY (a.Key);
				context.MoveTo (44, y);
				context.LineTo (56, y);
				context.StrokePreserve();
				context.MoveTo (40, y - 1);
				_axis.Text (context, a.Value, HorizontalTextAlignment.Right);
			}
		}

		private void DrawPoint(Context context, int j, double val) {
			double xl = 0.5 * (double)(Width - 60) / N;
			int x = (int)((Width - 60) * ((double)j / N) + 50 + xl);
			int y = ValueToY (val);
			context.MoveTo (x, y);
			context.Arc(x, y, 2, 0, 2 * Math.PI);
			context.StrokePreserve();
			context.Fill();
		}

		private int ValueToY(double val) {
			return (int)(Height / (_axis.YLimUp - _axis.YLimDown) * (_axis.YLimUp - val));
		}

		private void DrawHistArea(Context context, double xl, double yl, int i, int j, double value) {
			Color c = HistColor (value);
			context.SetSourceRGB (c.R, c.G, c.B);
			context.MoveTo ((int)(j * xl + 50), (int)(i * yl));
			context.LineTo ((int)((j + 1) * xl + 50), (int)(i * yl));
			context.LineTo ((int)((j + 1) * xl + 50), (int)((i + 1) * yl));
			context.LineTo ((int)(j * xl + 50), (int)((i + 1) * yl));
			context.LineTo ((int)(j * xl + 50), (int)(i * yl));
			context.StrokePreserve ();
			context.MoveTo ((int)((j + .5) * xl + 50), (int)((i + .5) * yl));
			context.Fill ();
		}

		public void DrawHist(TimeList<Complex[]> list, double t) {				
			using (Context context = new Context(Surface)) {

				context.Rectangle(0, 0, Width, Height);
				context.Color = Background;
				context.Fill();

				int index = list.Index (t);
				int times = 30;

				double xl = (double)(Width - 60) / N;
				double yl = (double)Height / times;

				Complex[] points;
				Color c;
				int ii;
				for (int i = index; i >= Math.Max(0, index - times); i--) {
					points = list.Get (i);
					N = points.Length;
					ii = index - i;
					for (int j = 0; j < points.Length; j++) {
						DrawHistArea (context, xl, yl, ii, j, _valueExtractor(points [j]));
					}
				}
			}

			DrawExpose (null, null);
		}

		private Color HistColor(double x) {
			//Console.WriteLine (x);
			double g = 0;
			double r = 1;
			double b = 1;
			if(x > 0) {
				r = 1 - Math.Abs(x / _axis.YLimUp);
			}
			if(x < 0) {
				b = 1 - Math.Abs(x / _axis.YLimDown);
			}

			return new Color (r, g, b);
		}

		public void DrawSingle(Complex[] points) {		
			N = points.Length;
			using (Context context = new Context(Surface)) {
				double min = double.PositiveInfinity, max = double.NegativeInfinity, val;
				for (int i = 0; i < points.Length; i++) {
					val = _valueExtractor (points [i]) * 1.2;
					if(val < min) {
						min = val;
					}
					if(val > max) {
						max = val;
					}
				}
				DrawAxes (context, min, max);
				context.SetSourceRGB (0.0, 0.0, 1.0);

				for (int i = 0; i < points.Length; i++) {
					DrawPoint (context, i, _valueExtractor(points [i]));
				}	
			}

			DrawExpose (null, null);
		}

		public override void Draw (TimeList<Complex[]> list, double t, DisplayMode mode)
		{
			Complex[] points = list [t];

			switch (mode) {
				case DisplayMode.Snapshot:				

				DrawSingle (points);
				break;
				case DisplayMode.History:
				DrawHist (list, t);
				break;
				default:
				break;
			}	
		}
	}
}

