using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using DirectShowLib;



namespace RacerMateOne
{
	public class ReferenceClock : IReferenceClock
	{
		#region Private Members
		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceCounter(out ulong lpPerformanceCount);

		[DllImport("Kernel32.dll", SetLastError = true)]
		private static extern bool QueryPerformanceFrequency(out ulong lpFrequency);

		[DllImport("Kernel32.DLL", CharSet = CharSet.Auto)]
		private static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);

		[DllImport("Kernel32.DLL")]
		private static extern bool SetEvent(IntPtr hEvent);

		private bool _init = false;

		private ulong _lastTime;
		private ulong _freq;
		private long _currentTime;
		private double _playRate = 0.0;
		private const long TIME_FACTOR = 10000000;

		#endregion



		#region Constructors

		public ReferenceClock()
		{
			if (QueryPerformanceFrequency(out _freq) == false)
			{
				throw new Win32Exception(); // timer not supported
			}
			_playRate = 0d;
			_init = false;
		}
		#endregion

		#region Properties
		public double PlayRate
		{
			get
			{
				return _playRate;
			}

			set
			{
				if (value < 0)
					_playRate = 0;
				else
					_playRate = value;
			}
		}
		#endregion

		#region Get Private Time
		private long GetPrivateTime()
		{
			ulong time;
			if (_init == false)
			{
				_init = true;

				QueryPerformanceCounter(out time);
				_lastTime = time;
				_currentTime = 0;
			}
			else
			{
				QueryPerformanceCounter(out time);
				long timeDiff = (long)(time - _lastTime);
				_lastTime = time;
				double elapsedSeconds = (double)(timeDiff) / (double)_freq;
				elapsedSeconds = elapsedSeconds * PlayRate;
				_currentTime += (long)(elapsedSeconds * TIME_FACTOR);
			}
			return _currentTime;
		}
		#endregion

		#region IReferenceClock Members
		public int GetTime(out long pTime)
		{
			pTime = this.GetPrivateTime();
			return 0;
		}
		public int AdviseTime(long baseTime, long streamTime, System.IntPtr hEvent, out int pdwAdviseCookie)
		{
			pdwAdviseCookie = 0;
			long refTime = baseTime + streamTime;
			if (PlayRate == 0d)
			{
				SetEvent(hEvent);
				return 0;
			}
			while (true)
			{
				long time = this.GetPrivateTime();
				if (time >= refTime)
				{
					SetEvent(hEvent);
					break;
				}
				Thread.Sleep(2);
			}
			return 0;
		}
		public int Unadvise(int dwAdviseCookie)
		{
			// TODO:  Add RefrenceClock.Unadvise implementation
			return 0;
		}
		public int AdvisePeriodic(long startTime, long periodTime, System.IntPtr hSemaphore, out int pdwAdviseCookie)
		{
			// TODO:  Add RefrenceClock.AdvisePeriodic implementation
			pdwAdviseCookie = 0;
			return 0;
		}
		#endregion
	}

}
