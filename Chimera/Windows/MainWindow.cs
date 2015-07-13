using System;
using Gtk;
using System.Collections.Generic;
using System.Numerics;
using Cairo;
using System.Threading;
using System.Configuration;
using SurfaceItems;
using Kekstoaster.Math;

namespace Chimera
{
	public partial class MainWindow: Gtk.Window
	{
		private const string WINDOW_TITLE = "Chimera";

		private Integrator _int;
		private bool _playing;
		private SurfaceList _surfaces;
		private PhasePortraitSurfaceItem _portrait;
		private double _maxTime;
		private double _h;
		private TriggeredTimer _timer;
		private double _timeValue;
		private ProgramMode _mode;

		private Complex[] modY = null;

		FileChooserDialog _fileOpenDialog;
		FileChooserDialog _fileSaveDialog;
		FileChooserDialog _filePlotDialog;

		SettingsChimera _settings;

		public MainWindow (): base (Gtk.WindowType.Toplevel)
		{
			// new Integrator
			_h = 0.01; //delta t
			_maxTime = 500;
			_timer = new TriggeredTimer (100);
			_timer.Elapsed += delegate(object sender, EventArgs e) {
				Gtk.Application.Invoke (delegate {
					_timeValue = _timeValue + _h*5;
					slideTime.Value = _timeValue;
					RepaintAll();
				});
			};		
			_settings = new SettingsChimera ();
			AWP awp = _settings.AWP;
			_int = new Integrator (_h, _maxTime, awp);
		
			_int.AWP.Changed += AwpChanged;
			_playing = false;
			_timeValue = 0;
			_int.EditMode = true;
			Build ();

			_surfaces = new SurfaceList(4, "j");
			Table tbl = _surfaces;

			this.contentColumns.Add (tbl);
			global::Gtk.Box.BoxChild wtbl = ((global::Gtk.Box.BoxChild)(this.contentColumns [tbl]));
			wtbl.Position = 0;

			_fileOpenDialog = new FileChooserDialog ("Anfangswert öffnen", this, FileChooserAction.Open);
			_fileOpenDialog.AddButton(Stock.Cancel, ResponseType.Cancel);
			_fileOpenDialog.AddButton(Stock.Open, ResponseType.Ok);

			_fileOpenDialog.Filter = new FileFilter();
			_fileOpenDialog.Filter.AddPattern("*.awp");

			_fileSaveDialog = new FileChooserDialog ("Anfangswert speichern", this, FileChooserAction.Save);
			_fileSaveDialog.AddButton(Stock.Cancel, ResponseType.Cancel);
			_fileSaveDialog.AddButton(Stock.Save, ResponseType.Ok);

			_fileSaveDialog.Filter = new FileFilter();
			_fileSaveDialog.Filter.AddPattern("*.awp");

			_filePlotDialog = new FileChooserDialog ("Grafiken exportieren", this, FileChooserAction.CreateFolder);
			_filePlotDialog.AddButton(Stock.Cancel, ResponseType.Cancel);
			_filePlotDialog.AddButton("Exportieren", ResponseType.Ok);

			ValueSurfaceItem vsi_real = new ValueSurfaceItem (1500, 500, 500, 100, "x<sub>j</sub>");
			vsi_real.ValueExtractor = new Func<Complex, double> (delegate(Complex z) {
				return z.Real;
			});
			//vsi_real.YLimUp = 1.2;
			//vsi_real.YLimDown = -1.2;
			ValueSurfaceItem vsi_imag = new ValueSurfaceItem (1500, 500, 500, 100, "y<sub>j</sub>");
			vsi_imag.ValueExtractor = new Func<Complex, double> (delegate(Complex z) {
				return z.Imaginary;
			});
			//vsi_imag.YLimUp = 1.2;
			//vsi_imag.YLimDown = -1.2;
			ValueSurfaceItem vsi_magn = new ValueSurfaceItem (1500, 500, 500, 100, "r<sub>j</sub>");
			//vsi_magn.YLimDown = 0;
			vsi_magn.ValueExtractor = new Func<Complex, double> (delegate(Complex z) {
				return z.Magnitude;
			});
			//vsi_magn.YLimUp = 1.2;
			//vsi_magn.YLimDown = -0.2;
			ValueSurfaceItem vsi_phas = new ValueSurfaceItem (1500, 500, 500, 100, "φ<sub>j</sub>");
			vsi_phas.ValueExtractor = new Func<Complex, double> (delegate(Complex z) {
				return z.Phase;
			});
			//vsi_phas.YLimUp = 3.2;
			//vsi_phas.YLimDown = -3.2;

			vsi_real.PointChanged += delegate(object sender, OdePointEventArgs e) {
				modY [e.Index] = new Complex (e.Value.Real, modY [e.Index].Imaginary);
				_int.AWP.Y = modY;
				RepaintAll ();
			};
			vsi_real.RedrawRequired += SurfaceRedrawRequired;
			vsi_imag.PointChanged += delegate(object sender, OdePointEventArgs e) {
				modY [e.Index] = new Complex (modY[e.Index].Real, e.Value.Real);
				_int.AWP.Y = modY;
				RepaintAll ();
			};
			vsi_imag.RedrawRequired += SurfaceRedrawRequired;
			vsi_magn.PointChanged += delegate(object sender, OdePointEventArgs e) {
				modY [e.Index] = Complex.FromPolarCoordinates(Math.Max(0, e.Value.Real), modY[e.Index].Phase);
				_int.AWP.Y = modY;
				RepaintAll ();
			};
			vsi_magn.RedrawRequired += SurfaceRedrawRequired;
			vsi_magn.AxisProperties.ZeroPosition = 0.92;
			vsi_magn.AxisProperties.AddValue(0, "0");
			vsi_phas.PointChanged += delegate(object sender, OdePointEventArgs e) {
				modY [e.Index] = Complex.FromPolarCoordinates(modY[e.Index].Magnitude, e.Value.Real);
				_int.AWP.Y = modY;
				RepaintAll ();
			};
			vsi_phas.RedrawRequired += SurfaceRedrawRequired;
			vsi_phas.AxisProperties = AxisProperties.CreateStatic (vsi_phas, -3.8, 3.8);
			vsi_phas.AxisProperties.MaxCount = 3;
			vsi_phas.AxisProperties.AddValue(Math.PI, "π");
			vsi_phas.AxisProperties.AddValue(0, "0");
			vsi_phas.AxisProperties.AddValue(-Math.PI, "-π");

			_surfaces.Add (vsi_real);
			_surfaces.Add (vsi_imag);
			_surfaces.Add (vsi_magn);
			_surfaces.Add (vsi_phas);

			_portrait = new PhasePortraitSurfaceItem (500, 300);
			_portrait.PointAdded += delegate(object sender, OdePointEventArgs e) {
				Complex[] n = new Complex[modY.Length + 1];
				Array.Copy (modY, n, modY.Length);
				n [modY.Length] = e.Value;

				modY = n;
				_int.AWP.Y = modY;
				numN.Value = n.Length;
				RepaintAll ();
			};
			_portrait.Clear ();
			_portrait.RedrawRequired += SurfaceRedrawRequired;

			this.boxPhase.Add (_portrait);
			global::Gtk.Box.BoxChild wp = ((global::Gtk.Box.BoxChild)(this.boxPhase [_portrait]));
			wp.Position = 0;

			ShowAll ();

			slideTime.SetIncrements (_h, Math.Max (_h, _maxTime / 100));
			slideTime.SetRange (0, _maxTime);

			slideBeta.Digits = 2;
			slideK.Digits = 1;
			slideLambda.Digits = 1;
			slideOmega.Digits = 1;

			slideBeta.Value = _int.AWP.Beta;
			slideK.Value = _int.AWP.K;
			slideLambda.Value = _int.AWP.Lambda;
			slideOmega.Value = _int.AWP.Omega;
			//slideTime.Value = 0;
			numN.Value = _int.AWP.N;
			numP.Value = _int.AWP.P;

			_int.CalculationStarted += HandleCalculationStarted;
			_int.CalculationFinished += (object sender, EventArgs e) => {
				Gtk.Application.Invoke (delegate {
					progressStatus.Visible = false;
					lblStatus.Text = "Berechnung abgeschlossen";
				});
			};
			_int.TimeRange += (object sender, TimeRangeEventArgs e) => {
			//	slideTime.SetRange (0, e.Time);
				RepaintAll();
				Gtk.Application.Invoke (delegate {
					progressStatus.Fraction = e.Time / _maxTime;
				});
			};

			
			_mode = ProgramMode.Investigation;
			ProgramMode = ProgramMode.Presentation;
			_int.EditMode = false;
		}

		void AwpChanged (object sender, EventArgs e)
		{
			if(slideBeta.Value != _int.AWP.Beta)
				slideBeta.Value = _int.AWP.Beta;
			if(slideK.Value != _int.AWP.K)
				slideK.Value = _int.AWP.K;
			if(slideLambda.Value != _int.AWP.Lambda)
				slideLambda.Value = _int.AWP.Lambda;
			if(slideOmega.Value != _int.AWP.Omega)
				slideOmega.Value = _int.AWP.Omega;
			if(numN.Value != _int.AWP.N)
				numN.Value = _int.AWP.N;
			if(numP.Value != _int.AWP.P)
				numP.Value = _int.AWP.P;
			RepaintAll ();

			foreach (ValueSurfaceItem s in _surfaces) {
				s.ResetAxis ();
			}
			_portrait.ResetAxis ();
		}

		void HandleCalculationStarted (object sender, EventArgs e)
		{
			Gtk.Application.Invoke (delegate {
				Playing = false;
				progressStatus.Fraction = 0.0;
				//if(progressStatus.Visible) {
					lblStatus.Text = "Berechnung gestartet";
				//}
				progressStatus.Visible = true;

			});
			
			//slideTime.Value = 0;
			//RepaintAll ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			_settings.AWP = _int.AWP;
			Application.Quit ();
			a.RetVal = true;
		}

		protected void OnNumNValueChanged (object sender, EventArgs e)
		{
			//Console.WriteLine ("N: {0}", numN.ValueAsInt);
			_int.AWP.N = numN.ValueAsInt;
			if(_mode == ProgramMode.Modification && modY.Length < numN.ValueAsInt) {
				Complex[] n = new Complex[modY.Length + 1];
				Array.Copy (modY, n, modY.Length);
				n [modY.Length] = modY[modY.Length - 1];

				modY = n;
				_int.AWP.Y = modY;
				RepaintAll ();
			}
		}

		protected void OnNumPValueChanged (object sender, EventArgs e)
		{
			//Console.WriteLine ("N: {0}", numP.ValueAsInt);
			_int.AWP.P = numP.ValueAsInt;
		}

		protected void OnSlideLambdaValueChanged (object sender, EventArgs e)
		{
			//Console.WriteLine ("Lambda: {0}", slideLambda.Value);
			_int.AWP.Lambda = slideLambda.Value;
		}

		protected void OnSlideOmegaValueChanged (object sender, EventArgs e)
		{
			//Console.WriteLine ("Omega: {0}", slideOmega.Value);
			_int.AWP.Omega = slideOmega.Value;
		}

		protected void OnSlideKValueChanged (object sender, EventArgs e)
		{
			//Console.WriteLine ("K: {0}", slideK.Value);
			_int.AWP.K = slideK.Value;
		}

		protected void OnSlideBetaValueChanged (object sender, EventArgs e)
		{
			//Console.WriteLine ("Beta: {0}", slideBeta.Value);
			_int.AWP.Beta = slideBeta.Value;
		}

		protected void OnSlideTimeValueChanged (object sender, EventArgs e)
		{
			//Console.WriteLine ("t: {0}", slideTime.Value);
			_timeValue = slideTime.Value;
			RepaintAll ();
		}

		protected void OnBtnPlayPauseClicked (object sender, EventArgs e)
		{
			Playing = !Playing;
		}

		public bool Playing {
			get {
				return _playing;
			}
			set {
				if(this._playing != value) {
					this._playing = value;
					mnExtrasPlayPause.Active = value;
					OnDrawPlayPauseExposeEvent (drawPlayPause, null);
					slideTime.Sensitive = !_playing;
					numN.Sensitive = !_playing;
					numP.Sensitive = !_playing;
					slideLambda.Sensitive = !_playing;
					slideOmega.Sensitive = !_playing;
					slideK.Sensitive = !_playing;
					slideBeta.Sensitive = !_playing;

					if(value) {
						_timer.Activate ();
					} else {
						Gtk.Application.Invoke (delegate {
							slideTime.Value = _timeValue;
						});
					}
				}
			}
		}

		public ProgramMode ProgramMode {
			get {
				return _mode;
			}
			set {
				if(_mode != value) {
					_mode = value;
					Console.WriteLine ("Change Mode: {0}", value);
					switch (value) {
					case ProgramMode.Presentation:
						mnModePresentation.Active = true;
						if (!_playing) {
							slideTime.Sensitive = true;
						}

						foreach (DisplayTypeSurfaceItem item in _surfaces) {
							item.Sensitive = false;
						}

						_portrait.Sensitive = false;

						progressStatus.Sensitive = true;
						btnPlayPause.Sensitive = true;
						mnExtrasPlayPause.Sensitive = true;
						_int.EditMode = false;
						this.Title = WINDOW_TITLE + " - Darstellungsmodus";
						break;
					case ProgramMode.Modification:
						mnModeModification.Active = true;
						slideTime.Sensitive = false;
						progressStatus.Sensitive = false;
						btnPlayPause.Sensitive = false;
						mnExtrasPlayPause.Sensitive = false;
						_int.EditMode = true;
						Playing = false;

						foreach (DisplayTypeSurfaceItem item in _surfaces) {
							item.Sensitive = true;
						}
						_portrait.Sensitive = true;

						modY = _int.TimeList [_timeValue];
						this.Title = WINDOW_TITLE + " - Bearbeitungsmodus";
						break;
					case ProgramMode.Investigation:
						Console.WriteLine ("Not Implemented");
						break;
					default:
						break;
					}
					RepaintAll ();
				}
			}
		}
	}

	public enum ProgramMode {
		Presentation,
		Modification,
		Investigation
	}
}