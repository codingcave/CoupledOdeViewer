using System;

namespace Chimera
{
	public struct ChimeraVersion
	{
		// TODO get from assembly info
		private const byte MAYOR = 1;
		private const byte MINOR = 0;
		private const byte FIX = 1;

		private byte _mayor;
		private byte _minor;
		private byte _fix;

		public ChimeraVersion (byte mayor, byte minor, byte fix)
		{
			if(!CheckVersion(mayor, minor, fix)) {
				throw new ArgumentException("Required version exceeds current program version");
			}
			this._mayor = mayor;
			this._minor = minor;
			this._fix = fix;
		}

		public static bool CheckVersion(byte mayor, byte minor, byte fix) {
			if(mayor == MAYOR) {
				if(minor == MINOR) {
					if(fix <= FIX) {
						return true;
					} else {
						return false;
					}
				} else {
					if(minor < MINOR) {
						return true;
					} else {
						return false;
					}
				}
			} else {
				if(mayor < MAYOR) {
					return true;
				} else {
					return false;
				}
			}
		}

		public byte Mayor {
			get{
				return _mayor;
			}
		}

		public byte Minor {
			get {
				return _minor;
			}
		}

		public byte Fix {
			get {
				return _fix;
			}
		}
	}
}

