using System;
using System.Numerics;
using Gtk;

namespace Chimera
{
	public partial class MainWindow
	{
		protected void OnMnExtrasOptionActivated(object sender, EventArgs e) {
			OptionsDialog optWin = new OptionsDialog ();
			optWin.Run ();
			optWin.Destroy ();
		}

		protected void OnMnFileNewActivated (object sender, EventArgs e)
		{
			//Console.WriteLine ("File New");
			//_surface.Clear ();
			_int.AWP.Y = new Complex[_int.AWP.N];
		}

		protected void OnMnFileOpenActivated (object sender, EventArgs e)
		{
			//Console.WriteLine ("File Open");
			ResponseType RetVal = (ResponseType)_fileOpenDialog.Run();
			if(RetVal == ResponseType.Ok) {
				//Console.WriteLine (_fileOpenDialog.Filename);

				try {
					AWP awp = AwpLoadSaveHandler.Load (_fileOpenDialog.Filename);
					//_surface.Clear ();
					_int.AWP.Use(awp);
					lblStatus.Text = "AWP '" + System.IO.Path.GetFileName(_fileOpenDialog.Filename) + "' geladen.";
				} catch (Exception ex) {
					Console.WriteLine (ex.Message);
				}



			}
			_fileOpenDialog.Hide ();
		}


		protected void OnMnFileSaveActivated (object sender, EventArgs e)
		{
			//Console.WriteLine ("File Save");
			ResponseType RetVal = (ResponseType)_fileSaveDialog.Run();
			if(RetVal == ResponseType.Ok) {
				//Console.WriteLine (_fileSaveDialog.Filename);
				string filename;
				if(!_fileSaveDialog.Filename.EndsWith(".awp")) {
					filename = _fileSaveDialog.Filename + ".awp";
				} else {
					filename = _fileSaveDialog.Filename;
				}

				if(System.IO.File.Exists(filename)) {
					MessageDialog m = new MessageDialog (_fileSaveDialog, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, "Die Datei '" + System.IO.Path.GetFileName(filename) + "' existiert bereits.\nSind Sie sicher, dass Sie die Datei überschreiben möchten?");
					if((ResponseType)m.Run() == ResponseType.Yes) {
						AwpLoadSaveHandler.Save (filename, numN.ValueAsInt, numP.ValueAsInt, slideLambda.Value, slideOmega.Value, slideK.Value, slideBeta.Value, _int.AWP.Y);
						lblStatus.Text = "AWP '" + System.IO.Path.GetFileName(filename) + "' gespeichert.";
					}
					m.Destroy ();
				} else {
					AwpLoadSaveHandler.Save (filename, numN.ValueAsInt, numP.ValueAsInt, slideLambda.Value, slideOmega.Value, slideK.Value, slideBeta.Value, _int.AWP.Y);
					lblStatus.Text = "AWP '" + System.IO.Path.GetFileName(filename) + "' gespeichert.";
				}

			}
			_fileSaveDialog.Hide ();
		}

		protected void OnMnFilePlotActivated (object sender, EventArgs e)
		{
			//Console.WriteLine ("File Plot");

			if((ResponseType)_filePlotDialog.Run() == ResponseType.Ok) {
				Console.WriteLine (_filePlotDialog.Filename);
				(new Plotter ()).CreateAll (_filePlotDialog.Filename, _int.TimeList, _timeValue);
			}
			_filePlotDialog.Hide ();
		}

		protected void OnMnFileCloseActivated (object sender, EventArgs e)
		{
			//Console.WriteLine ("File Close");
			Application.Quit ();
		}

		protected void OnPresentationActionChanged (object o, ChangedArgs args)
		{
			//Console.WriteLine ("Pres change");
			if (((RadioAction)o).Active) {
				ProgramMode = ProgramMode.Presentation;
			}
		}

		protected void OnMnModeModificationChanged (object o, ChangedArgs args)
		{
			//Console.WriteLine ("Mod change");
			if (((RadioAction)o).Active) {
				ProgramMode = ProgramMode.Modification;
			}
		}

		protected void OnInvestigationActionChanged (object o, ChangedArgs args)
		{
			//Console.WriteLine ("Inv change");
			if (((RadioAction)o).Active) {
				ProgramMode = ProgramMode.Investigation;
			}
		}

		protected void OnHistorieActionChanged (object o, ChangedArgs args)
		{
			if (((RadioAction)o).Active) {
				/*
				if (_surface.DisplayMode != DisplayMode.History) {
					_surface.DisplayMode = DisplayMode.History;
					RepaintAll ();
				}
				*/
			}
		}

		protected void OnZeitpunktActionChanged (object o, ChangedArgs args)
		{
			if (((RadioAction)o).Active) {
				/*
				if (_surface.DisplayMode != DisplayMode.Snapshot) {
					_surface.DisplayMode = DisplayMode.Snapshot;
					RepaintAll ();
				}
				*/
			}
		}

		protected void OnMnAwpRandomActivated (object sender, EventArgs e)
		{
			//Console.WriteLine ("awp rand");
			_int.RandomAwp ();
			//_surface.Clear ();
			RepaintAll ();
		}

		protected void OnMnAwp2StepsActivated (object sender, EventArgs e)
		{
			//Console.WriteLine ("awp 2 steps");
			_int.TwoStepsAwp ();
			//_surface.Clear ();
			RepaintAll ();
		}

		protected void OnMnAwpCurrentActivated(object sender, EventArgs e)
		{
			_int.AWP.Y = _int.TimeList [_timeValue];
			//_surface.Clear ();
			RepaintAll ();
		}

		protected void OnMnExtrasPlayActivated(object sender, EventArgs e)
		{
			Playing = mnExtrasPlayPause.Active;
		}

		protected void OnMnExtrasClearPhaseActivated (object sender, EventArgs e)
		{
			_portrait.Clear ();
			//RepaintAll ();
		}

		protected void OnMnHelpAboutActivated (object sender, EventArgs e)
		{
			AboutDialog a = new AboutDialog ();
			a.Run ();
			a.Destroy ();
		}
	}
}