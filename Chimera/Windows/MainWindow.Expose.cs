using System;
using System.Numerics;
using Gtk;
using Cairo;
using SurfaceItems;

namespace Chimera
{
	public partial class MainWindow
	{
		void OnSizeAllocated (object o, SizeAllocatedArgs args)
		{
			RepaintAll ();
		}

		protected void OnDrawPlayPauseExposeEvent (object o, ExposeEventArgs args)
		{
			using(Context context = Gdk.CairoHelper.Create (((DrawingArea)o).GdkWindow))
			{
				context.Rectangle(0, 0, 16, 16);
				context.SetSourceRGB(.8, .8, .8);
				context.Fill();

				context.LineWidth = 1;

				if(_playing) {
					if (_mode == ProgramMode.Presentation) {
						context.SetSourceRGB (1.0, 0.0, 0.0);
					} else {
						context.SetSourceRGB (0.4, 0.4, 0.4);
					}

					context.MoveTo (2, 2);
					context.LineTo (2, 13);
					context.LineTo (13, 13);
					context.LineTo (13, 2);
					context.LineTo (2, 2);
					context.StrokePreserve();
					context.MoveTo (7, 7);
					context.Fill();
				} else {
					if (_mode == ProgramMode.Presentation) {
						context.SetSourceRGB (0.0, 1.0, 0.0);
					} else {
						context.SetSourceRGB (0.4, 0.4, 0.4);
					}

					context.MoveTo (3, 1);
					context.LineTo (3, 14);
					context.LineTo (12, 8);
					context.LineTo (12, 7);
					context.LineTo (3, 1);
					context.StrokePreserve();
					context.MoveTo (7, 7);
					context.Fill();
				}
			}
		}		

		void SurfaceRedrawRequired (object sender, EventArgs e)
		{
			Gtk.Application.Invoke (delegate {		
				if (this._mode == ProgramMode.Modification) {
					((SurfaceItems.DisplayTypeSurfaceItem)sender).Draw(_int.TimeList, _timeValue, SurfaceItems.DisplayMode.Snapshot);
				} else {
					((SurfaceItems.DisplayTypeSurfaceItem)sender).Draw(_int.TimeList, _timeValue);
				}
			});
		}


		public void RepaintAll() {
			Gtk.Application.Invoke (delegate {		
				if (this._mode == ProgramMode.Modification) {
					foreach (SurfaceItems.ValueSurfaceItem item in _surfaces) {
						item.DrawSingle(modY);
					}
					_portrait.Draw(modY);
				} else {
					foreach (SurfaceItems.DisplayTypeSurfaceItem item in _surfaces) {
						item.Draw(_int.TimeList, _timeValue);
					}
					_portrait.Draw(_int.TimeList, _timeValue);
				}
				if(_playing) {
					_timer.Activate();
				}
				//OnDrawPhasePortraitExposeEvent (drawPhasePortrait, null);
			});
		}
	}
}