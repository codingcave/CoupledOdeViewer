using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using Kekstoaster.Math;

namespace Chimera
{
	public class Plotter
	{
		public event EventHandler SaveDone;

		public Plotter ()
		{
		}

		public void CreateAll(string path, TimeList<Complex[]> list, double t) {
			if(!Directory.Exists(path)) {
				Directory.CreateDirectory (path);
				//throw new ArgumentException("Directory already exists");
			}

			Complex[] points = list[t];
			int index = list.Index (t);
			int N = list.Get (index).Length;

			string name = Path.GetFileName (path);
			using(StreamWriter w = new StreamWriter(Path.Combine(path, name + "_single.dat"))) {
				for (int i = 0; i < N; i++) {
					w.WriteLine ("{0} {1} {2} {3} {4}", i, points [i].Real.F(), points [i].Imaginary.F(), points [i].Magnitude.F(), points [i].Phase.F());
				}
				w.Flush ();
				w.Close ();
			}


			double times = 10;
			int firstTime;
			if(t - times <= 0) {
				firstTime = 0;
			} else {
				firstTime = list.Index (t - times);
			}

			double[] cmX = new double[N];
			double[] cmY = new double[N];

			using(StreamWriter w = new StreamWriter(Path.Combine(path, name + "_hist.dat"))) {
				for (int ti = firstTime; ti <= index; ti++) {
					points = list.Get (ti);
					w.Write (points[0].Real.F());
					for (int j = 1; j < N; j++) {
						w.Write(" ");
						w.Write (points[j].Real.F());
						cmX [j] += points [j].Real;
					}
					w.WriteLine ();

					w.Write (points[0].Imaginary.F());
					for (int j = 1; j < N; j++) {
						w.Write(" ");
						w.Write (points[j].Imaginary.F());
						cmY [j] += points [j].Imaginary;
					}
					w.WriteLine ();

					w.Write (points[0].Magnitude.F());
					for (int j = 1; j < N; j++) {
						w.Write(" ");
						w.Write (points[j].Magnitude.F());
					}
					w.WriteLine ();
				}
				w.Flush ();
				w.Close ();
			}

			for (int i = 0; i < N; i++) {
				cmX [i] = cmX [i] / (index - firstTime);
				cmY [i] = cmY [i] / (index - firstTime);
			}

			using(StreamWriter w = new StreamWriter(Path.Combine(path, name + "_cm.dat"))) {			
				for (int j = 0; j < N; j++) {
					w.WriteLine ("{0} {1}", j, Math.Sqrt (cmX [j] * cmX [j] + cmY [j] * cmY [j]).F());						
				}

				w.Flush ();
				w.Close ();
			}

			using(StreamWriter w = new StreamWriter(Path.Combine(path, name + ".plt"))) {
				w.WriteLine ("set terminal png");
				w.WriteLine ("set termopt enhanced");
				w.WriteLine ("set autoscale");

				w.WriteLine ("set output \"x.png\"");
				w.WriteLine ("set title \"Plot 1\"");
				w.WriteLine ("set xlabel \"j\"");
				w.WriteLine ("set ylabel \"x_j\"");
				w.WriteLine ("plot \"" + name + "_single.dat\" using 1:2 notitle");

				w.WriteLine ("set output \"y.png\"");
				w.WriteLine ("set title \"Plot 2\"");
				w.WriteLine ("set xlabel \"j\"");
				w.WriteLine ("set ylabel \"y_j\"");
				w.WriteLine ("plot \"" + name + "_single.dat\" using 1:3 notitle");

				w.WriteLine ("set output \"xy.png\"");
				w.WriteLine ("set title \"Plot 3\"");
				w.WriteLine ("set xlabel \"j\"");
				w.WriteLine ("set ylabel \"y_j, x_j\"");
				w.WriteLine ("plot \"" + name + "_single.dat\" using 1:2 title \"x\", \"" + name + "_single.dat\" using 1:3 title \"y\"");

				w.WriteLine ("set output \"r.png\"");
				w.WriteLine ("set title \"Plot 3\"");
				w.WriteLine ("set xlabel \"j\"");
				w.WriteLine ("set ylabel \"r_j\"");
				w.WriteLine ("plot \"" + name + "_single.dat\" using 1:4 notitle");

				w.WriteLine ("set output \"cm.png\"");
				w.WriteLine ("set title \"Massenschwerpunkt\"");
				w.WriteLine ("set xlabel \"j\"");
				w.WriteLine ("set ylabel \"r_{c.m.}\"");
				w.WriteLine ("plot \"" + name + "_cm.dat\" using 1:2 notitle");

				w.WriteLine ("set output \"Phasenraum.png\"");
				w.WriteLine ("set title \"Phasenraum\"");
				w.WriteLine ("set xlabel \"Realteil\"");
				w.WriteLine ("set ylabel \"Imaginärteil\"");
				w.WriteLine ("plot \"" + name + "_single.dat\" using 2:3 notitle");

				w.WriteLine ("set terminal png");
				w.WriteLine ("set autoscale fix");
				w.WriteLine ("set tics out nomirror");
				w.WriteLine ("set pm3d map interpolate 0,0");
				w.WriteLine ("set xlabel \"j\"");
				w.WriteLine ("set ylabel \"t\"");

				w.WriteLine ("set output \"hist_x.png\"");
				w.WriteLine ("set title \"x\"");
				w.WriteLine ("splot '" + name + "_hist.dat' every :3 matrix using 1:($2/" + ((t - list.TimeOf(firstTime)) * 30).F() + "):3 notitle");

				w.WriteLine ("set output \"hist_y.png\"");
				w.WriteLine ("set title \"y\"");
				w.WriteLine ("splot '" + name + "_hist.dat' every :3::1 matrix using 1:(($2 - 1)/" + ((t - list.TimeOf(firstTime)) * 30).F() + "):3 notitle");

				w.WriteLine ("set output \"hist_r.png\"");
				w.WriteLine ("set title \"r\"");
				w.WriteLine ("splot '" + name + "_hist.dat' every :3::2 matrix using 1:(($2 - 2)/" + ((t - list.TimeOf(firstTime)) * 30).F() + "):3 notitle");

				w.Flush ();
				w.Close ();
			}

			System.Diagnostics.Process proc = new System.Diagnostics.Process ();
			proc.EnableRaisingEvents = false;	
			proc.StartInfo.CreateNoWindow = true;
			proc.StartInfo.WorkingDirectory = path;
			proc.StartInfo.FileName = "gnuplot";
			proc.StartInfo.Arguments = Path.Combine(path, name + ".plt");
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.Start ();
			proc.WaitForExit ();
			string output = proc.StandardOutput.ReadToEnd ();
			proc.Close ();

		}
	}
}

