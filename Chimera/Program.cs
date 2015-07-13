using System;
using Gtk;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace Chimera
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}

		static void ToFile(Dictionary<double, Complex[]> list) {
			StringBuilder sb = new StringBuilder();
			using(StreamWriter fh = new StreamWriter("test.txt")) {
				foreach (var item in list) {
					sb.Clear ();
					sb.Append (item.Key.ToString(System.Globalization.CultureInfo.InvariantCulture));

					foreach (var z in item.Value) {
						sb.Append(" ");
						sb.Append (z.Real.ToString(System.Globalization.CultureInfo.InvariantCulture));
					}

					fh.WriteLine (sb.ToString());
				}
			}
		}

		static void ItemToFile(Dictionary<double, Complex[]> list, double t) {
			double max = 0;
			foreach (var item in list.Keys) {
				if(item < t)
					max = Math.Max (max, item);
			}
			Complex[] last = list [max];
			//Complex[] first = list [-1];
			using(StreamWriter fh = new StreamWriter("test.txt")) {
				for(int i = 0; i < last.Length; i++) {
					fh.WriteLine ("{0} {1} {2}", i, last[i].Imaginary.F(), last[i].Magnitude.F());
				}

			}
		}
	}
}
