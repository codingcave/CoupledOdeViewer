using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;

namespace Chimera
{
	public class AwpLoadSaveHandler
	{
		private const int V1 = 1;
		private const int V2 = 0;
		private const int V3 = 0;

		public AwpLoadSaveHandler ()
		{
		}

		public static void Save(string path, AWP awp) {
			Save (path, awp.N, awp.P, awp.Lambda, awp.Omega, awp.K, awp.Beta, awp.Y);
		}

		public static void Save(string path, int N, int P, double lambda, double omega, double K, double beta, Complex[] y0) {
			using(FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
				if(N != y0.Length) throw new ArgumentException("N must be equal to the length of y0", "N");
				List<byte> save = new List<byte> ();
				save.Add((byte)'c');
				save.Add((byte)'c');
				save.Add((byte)0);
				save.Add((byte)'A');
				save.Add((byte)'W');
				save.Add((byte)'P');
				save.Add((byte)0);
				save.Add((byte)'V');
				save.Add((byte)1);
				save.Add((byte)0);
				save.Add((byte)0);
				save.Add((byte)'c');
				save.Add((byte)'c');
				save.AddRange (BitConverter.GetBytes (N));
				save.AddRange (BitConverter.GetBytes (P));
				save.AddRange (BitConverter.GetBytes (lambda));
				save.AddRange (BitConverter.GetBytes (omega));
				save.AddRange (BitConverter.GetBytes (K));
				save.AddRange (BitConverter.GetBytes (beta));
				save.Add((byte)'c');
				save.Add((byte)'c');
				foreach (var item in y0) {
					save.AddRange (BitConverter.GetBytes (item.Real));
					save.AddRange (BitConverter.GetBytes (item.Imaginary));
				}
				fs.Write (save.ToArray (), 0, save.Count);
				fs.Flush ();
				fs.Close ();
			}
		}

		private static void Assert(byte a, byte b) {
			if(a != b) throw new ArgumentException("Assertion failed");
		}

		public static AWP Load(string path) { 
			byte[] file = File.ReadAllBytes(path);
			int ptr = 0;

			byte v1, v2, v3;
			// Header 
			// cc
			Assert(file[ptr++], (byte)'c');
			Assert(file[ptr++], (byte)'c');
			Assert(file[ptr++], 0);
			Assert(file[ptr++], (byte)'A');
			Assert(file[ptr++], (byte)'W');
			Assert(file[ptr++], (byte)'P');
			Assert(file[ptr++], 0);
			Assert(file[ptr++], (byte)'V');
			v1 = file [ptr++];
			v2 = file [ptr++];
			v3 = file [ptr++];

			if(!ChimeraVersion.CheckVersion(v1, v2, v3)) {
				throw new Exception("Version not supported");
			}

			Assert(file[ptr++], (byte)'c');
			Assert(file[ptr++], (byte)'c');


			int N = BitConverter.ToInt32(file, ptr);
			ptr += 4;
			int P = BitConverter.ToInt32(file, ptr);
			ptr += 4;

			double lambda = BitConverter.ToDouble (file, ptr);
			ptr += 8;
			double omega = BitConverter.ToDouble (file, ptr);
			ptr += 8;
			double K = BitConverter.ToDouble (file, ptr);
			ptr += 8;
			double beta = BitConverter.ToDouble (file, ptr);
			ptr += 8;

			Assert(file[ptr++], (byte)'c');
			Assert(file[ptr++], (byte)'c');

			Complex[] y0 = new Complex[N];
			double re, im;
			for (int i = 0; i < N; i++) {
				re = BitConverter.ToDouble (file, ptr);
				ptr += 8;
				im = BitConverter.ToDouble (file, ptr);
				ptr += 8;
				y0 [i] = new Complex (re, im);
			}
			if(ptr != file.Length) {
				throw new Exception("File could not be read.");
			}

			return new AWP (N, P, lambda, omega, K, beta, y0);
		}
	}
}

