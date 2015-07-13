using System;
using Gtk;
using Cairo;
using Kekstoaster.Math;
using System.Numerics;

namespace SurfaceItems
{
	public class DisplayTypeSurfaceItem:PlainSurfaceItem
	{
		DisplayMode _mode;
		public event EventHandler<DisplayModeEventArgs> DisplayModeChanged;		
		protected AxisProperties _axis;

		public DisplayTypeSurfaceItem (int maxWidth, int maxHeight, int widthRequest = 0, int heightRequest = 0, string label = null):base(maxWidth, maxHeight, widthRequest, heightRequest, label)
		{
			Console.WriteLine ("DisplayTypeSurfaceItem");
			_axis = AxisProperties.CreateMaximum (this);
		}		

		public DisplayMode DisplayMode {
			get {
				return this._mode;
			}
			set {
				if(_mode != value) {
					_mode = value;

					if(DisplayModeChanged != null) {
						DisplayModeChanged (this, new DisplayModeEventArgs (value));
					}
					OnRedrawRequired ();
				}
			}
		}

		public int N {
			get;
			set;
		}

		public override void Draw (TimeList<Complex[]> list, double t)
		{
			Draw (list, t, _mode);
		}

		public virtual void Draw(TimeList<Complex[]> list, double t, DisplayMode mode) {
			base.Draw(list, t);
		}

		public AxisProperties AxisProperties {
			get {
				return _axis;
			}
			set {
				this._axis = value;
				OnRedrawRequired ();
			}
		}

		public void ResetAxis() {
			_axis.Reset ();
		}
	}
}

