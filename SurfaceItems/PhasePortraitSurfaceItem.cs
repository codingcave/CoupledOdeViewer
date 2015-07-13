using System;
using Gtk;
using Cairo;
using System.Numerics;
using System.Collections.Generic;
using Kekstoaster.Math;
using SurfaceItems;

namespace SurfaceItems
{
	public class PhasePortraitSurfaceItem:DisplayTypeSurfaceItem
	{
		List<Color> _phaseColors;
		Random _rnd;
		double _lastTime;

		public PhasePortraitSurfaceItem (int maxSize, int sizeRequest = 100):base(maxSize, maxSize, sizeRequest, sizeRequest)
		{
			_phaseColors = new List<Color> ();
			_rnd = new Random ();

			this.MouseClick += HandleMouseClick;
			_lastTime = 0;
		}

		void HandleMouseClick (object sender, MouseEventArgs e)
		{
			double re = (double)(2 * e.X - Width) / (Width - 10);
			double im = (double)(2 * e.Y - Height) / (Height - 10);
			Complex z = new Complex (re, im);
			OnPointAdded(new OdePointEventArgs (N, z));
		}

		private void DrawAxes(Context context, double min, double max) {
			context.Rectangle(0, 0, Width, Height);
			context.Color = Background;
			context.Fill();

			context.LineWidth = 1;
			context.SetSourceRGB(0.0, 0.0, 0.0);

			context.MoveTo (0, Height / 2);
			context.LineTo (Width, Height / 2);
			context.StrokePreserve();

			context.MoveTo (Width / 2, 0);
			context.LineTo (Width / 2, Height);
			context.StrokePreserve();

			KeyValuePair<double, string>[] ax = _axis.AxisSteps (min, max);
			double y;

			foreach (var a in ax) {
				y = a.Key / _axis.YLimUp * Height / 2 + Height / 2;
				context.MoveTo (Width / 2 - 6, y);
				context.LineTo (Width / 2 + 6, y);
				context.StrokePreserve();

				context.MoveTo (y, Height / 2 - 6);
				context.LineTo (y, Height / 2 + 6);
				context.StrokePreserve();

				if (a.Key != 0) {
					context.MoveTo (Width / 2 - 10, Height - (y - 1));
					_axis.Text (context, a.Value, HorizontalTextAlignment.Right);

					context.MoveTo (y - 1, Height / 2 + 10);
					_axis.Text (context, a.Value, HorizontalTextAlignment.Center, VerticalTextAlignment.Top);
				} else {
					//context.MoveTo (Width / 2 + 5, Height / 2 + 5);
					//_axis.Text (context, a.Value, HorizontalTextAlignment.Left, VerticalTextAlignment.Top);
				}
			}
		}

		private void DrawPoint(Context context, Color c, Complex z) {
			context.SetSourceRGB (c.R, c.G, c.B);

			int x = (int)(0.5 * (Width - 10) * z.Real / _axis.YLimUp + 0.5 * Width);
			int y = (int)(0.5 * (Height - 10) * z.Imaginary / _axis.YLimUp + 0.5 * Height);

			context.MoveTo (x, y);
			context.Arc(x, y, 2, 0, 2 * Math.PI);
			context.StrokePreserve();
			context.Fill();
		}

		public override void Draw (TimeList<Complex[]> list, double t)
		{				
			Complex[] points = list [t];
			_lastTime = t;
			Draw (points);
		}

		public void Draw (Complex[] points)
		{
			N = points.Length;
			AdjustColors ();

			double min = double.PositiveInfinity, max = double.NegativeInfinity;
			for (int i = 0; i < points.Length; i++) {						
				min = Math.Min (min, Math.Min (points [i].Real, points [i].Imaginary));
				max = Math.Max (max, Math.Max (points [i].Real, points [i].Imaginary));
			}

			using (Context context = new Context(Surface)) {
				Clear (context);
				DrawAxes (context, min, max);
				for (int i = 0; i < points.Length; i++) {						
					DrawPoint (context, _phaseColors [i], points [i]);
				}
			}

			DrawExpose (null, null);
		}

		private void AdjustColors() {
			if(N != _phaseColors.Count) {
				while(N > _phaseColors.Count) {
					_phaseColors.Add(new Color (_rnd.NextDouble (), _rnd.NextDouble (), _rnd.NextDouble ()));
				}
				while(N < _phaseColors.Count) {
					_phaseColors.RemoveAt (N);
				}
			}
		}
	}
}

