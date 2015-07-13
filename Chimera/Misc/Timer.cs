using System;
using System.Threading;

namespace Chimera
{
	public class TriggeredTimer:IDisposable
	{
		private int _millisec;
		private AutoResetEvent _wait;
		private bool _waiting;
		private bool _disposed;
		private int _reschedule;
		private object _threadLock = new object();

		public event EventHandler Elapsed;

		public TriggeredTimer (int interval)
		{
			this._millisec = interval;
			_wait = new AutoResetEvent (false);
			_reschedule = 0;
			_disposed = false;
			_waiting = true;

			Thread timer = new Thread (new ThreadStart (Run));
			timer.IsBackground = true;
			timer.Start ();
		}

		private void Run() {

			while(!_disposed) {
				_wait.WaitOne ();
				Thread.Sleep (_millisec);
				// _reschedule
				OnElapsed ();
			}
		}

		public int Interval {
			get {
				return _millisec;
			}
			set {
				if(_millisec != value) {
					lock (_threadLock) {
						if(value > _millisec) {
							_reschedule = value - _millisec - _reschedule;
							if(_reschedule < 0) {
								OnElapsed ();
							}
						} else {
							OnElapsed ();
						}
					}
				}
			}
		}

		protected void OnElapsed() {
			_reschedule = 0;
			if(Elapsed != null) {
				Elapsed (this, EventArgs.Empty);
			}
		}

		public void Activate() {
			if(_waiting) {
				_wait.Set ();
			}
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			_disposed = true;
		}

		#endregion
	}
}

