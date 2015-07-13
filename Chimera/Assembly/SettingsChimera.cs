using System;
using System.Configuration;
using System.IO;

namespace Chimera
{
	public class SettingsChimera:IDisposable
	{
		Configuration _cfg;

		public SettingsChimera ()
		{
			_cfg = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.PerUserRoaming);
		}

		public AWP AWP {
			get  {
				string filename = Path.Combine (Path.GetDirectoryName (_cfg.FilePath), "last.awp");
				if(File.Exists(filename)) {
					try {
						AWP awp = AwpLoadSaveHandler.Load (filename);
						return awp;
					} catch (Exception ex) {
						File.Delete (filename);
						return null;
					}
				}
				return null;
			}
			set {
				string dirName = Path.GetDirectoryName (_cfg.FilePath);
				string filename = Path.Combine (dirName, "last.awp");
				if(!Directory.Exists(dirName)) {
					Directory.CreateDirectory (dirName);
				}
				AwpLoadSaveHandler.Save (filename, value);
			}
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

