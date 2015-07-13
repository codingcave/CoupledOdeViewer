using System;

namespace Chimera
{
	public partial class AboutDialog : Gtk.Dialog
	{
		public AboutDialog ()
		{
			this.Build ();

			GtkLabel1.HeightRequest = 40;
			GtkLabel2.HeightRequest = 40;
			GtkLabel3.HeightRequest = 40;
		}
	}
}

