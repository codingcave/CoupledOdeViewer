using System;
using Gtk;
using Cairo;
using System.Numerics;
using Kekstoaster.Math;

namespace SurfaceItems
{
	public class PlainSurfaceItem
	{
		private ImageSurface _surface;
		private Color _background;
		private int _width;
		private int _height;
		private EventBox _evtBox;
		private DrawingArea _draw;
		private MouseButton _button;
		private DisplayMode _mode;
		private bool _sensitive;
		private string _label;

		public event EventHandler<ResizeEventArgs> Resized;
		public event EventHandler RedrawRequired;
		public event EventHandler<OdePointEventArgs> PointChanged;
		public event EventHandler<OdePointEventArgs> PointAdded;
		public event EventHandler<MouseEventArgs> MouseClick;
		public event EventHandler<LabelEventArgs> LabelChanged;

		public PlainSurfaceItem(int maxWidth, int maxHeight, int widthRequest = 0, int heightRequest = 0, string label = null) {
			//Console.WriteLine ("PlainSurfaceItem");
			_label = label;
			this._evtBox = new EventBox ();
			this._draw = new global::Gtk.DrawingArea ();
			if(widthRequest > 0) {
				_draw.WidthRequest = widthRequest;
			}
			if(heightRequest > 0) {
				_draw.HeightRequest = heightRequest;
			}

			this._evtBox.Add (this._draw);

			maxWidth = Math.Max (maxWidth, widthRequest);
			maxHeight = Math.Max (maxHeight, heightRequest);
			this._height = Math.Max(_draw.Allocation.Height, heightRequest);
			this._width = Math.Max(_draw.Allocation.Width, widthRequest);
			this._mode = DisplayMode.Snapshot;
			this._surface = new ImageSurface(Format.Argb32, maxWidth, maxHeight);
			this._button = MouseButton.None;
			this._background = new Color (1.0, 1.0, 1.0);

			_draw.ExposeEvent += DrawExpose;
			_evtBox.ButtonPressEvent += DrawButtonPressEvent;
			_evtBox.ButtonReleaseEvent += DrawButtonReleaseEvent;
			_evtBox.MotionNotifyEvent += DrawMotionNotifyEvent;
		}

		protected void OnRedrawRequired() {
			if(RedrawRequired != null) {
				RedrawRequired (this, EventArgs.Empty);
			}
		}

		protected void OnPointChanged(OdePointEventArgs args) {
			if(PointChanged != null) {
				PointChanged (this, args);
			}
		}

		protected void OnPointAdded(OdePointEventArgs args) {
			if(PointAdded != null) {
				PointAdded (this, args);
			}
		}		

		public string Name {
			get {
				return _evtBox.Name;
			}
			set {
				this._evtBox.Name = value;
			}
		}

		public string Label {
			get {
				return this._label;
			}
			set {
				if(_label != value) {
					_label = value;

					if(LabelChanged != null) {
						LabelChanged (this, new LabelEventArgs (value));
					}
				}
			}
		}

		protected void DrawExpose(object o, ExposeEventArgs args) {
			var size = _draw.Allocation;
			if (_height != size.Height || _width != size.Width) {
				_height = size.Height;
				_width = size.Width;

				if(Resized != null) {
					Resized (this, new ResizeEventArgs(_height, _width));
				}
			}

			using(Context context = Gdk.CairoHelper.Create (_draw.GdkWindow))
			{
				context.Rectangle(0, 0, Width, Height);
				context.SetSource(_surface);
				context.Fill ();
			}
		}

		void DrawMotionNotifyEvent (object o, MotionNotifyEventArgs args)
		{
			if (this.Sensitive && MouseClick != null && _button != MouseButton.None) {
				MouseClick (this, new MouseEventArgs ((int)args.Event.X, (int)args.Event.Y, _button));
			}
		}		

		void DrawButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (this.Sensitive) {
				MouseButton newBtn = MouseButton.None;
				switch (args.Event.Button) {
					case 1:
						newBtn = MouseButton.Left;
						break;
					case 2:
						newBtn = _button | MouseButton.Middle;
						break;
					case 3:
						newBtn = _button | MouseButton.Right;
						break;
					default:
						break;
				}
				_button = _button | newBtn;

				if (MouseClick != null && newBtn != MouseButton.None) {
					MouseClick (this, new MouseEventArgs ((int)args.Event.X, (int)args.Event.Y, newBtn));
				}
			}
		}

		public bool Sensitive {
			get {
				return this._sensitive;
			}
			set {
				this._sensitive = value;
			}
		}

		public static implicit operator EventBox(PlainSurfaceItem item) {
			return item._evtBox;
		}

		void DrawButtonReleaseEvent (object o, ButtonReleaseEventArgs args) {
			switch (args.Event.Button) {
				case 1:
				_button = (_button | MouseButton.Left) ^ MouseButton.Left;
				break;
				case 2:
				_button = (_button | MouseButton.Middle) ^ MouseButton.Middle;
				break;
				case 3:
				_button = (_button | MouseButton.Right) ^ MouseButton.Right;
				break;
				default:
				break;
			}
		}

		public int Height {
			get { return this._height; }
			private set { 
				if(this._height != value) {
					this._height = value;

					if(Resized != null) {
						Resized (this, new ResizeEventArgs(_height, _width));
					}
				}
			}
		}

		public int Width {
			get { return this._width; }
			private set { 
				if(this._width != value) {
					this._width = value;

					if(Resized != null) {
						Resized (this, new ResizeEventArgs(_height, _width));
					}
				}
			}
		}

		public ImageSurface Surface {
			get {
				return _surface;
			}
		}

		public Color Background {
			get { return _background; }
		}

		public void Clear() {
			using (Context context = new Context(_surface)) {
				Clear (context);
			}

			OnRedrawRequired ();
		}	

		protected void Clear (Context context) {
			context.Rectangle(0, 0, Width, Height);
			context.Color = _background;
			context.Fill();
		}

		public virtual void Draw(TimeList<Complex[]> list, double t) {
			Clear ();
			DrawExpose (_draw, null);
		}
	}
}

