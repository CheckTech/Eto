﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Eto
{
	static partial class Win32
	{
		static Lazy<bool> perMonitorDpiSupported = new Lazy<bool>(() => MethodExists("shcore.dll", "SetProcessDpiAwareness"));
		static Lazy<bool> monitorDpiSupported = new Lazy<bool>(() => MethodExists("shcore.dll", "GetDpiForMonitor"));

		public static bool PerMonitorDpiSupported
		{
			get { return perMonitorDpiSupported.Value; }
		}

		public static bool MonitorDpiSupported
		{
			get { return monitorDpiSupported.Value; }
		}

		public enum PROCESS_DPI_AWARENESS : uint
		{
			UNAWARE = 0,
			SYSTEM_DPI_AWARE = 1,
			PER_MONITOR_DPI_AWARE = 2
		}

		public enum MONITOR : uint
		{
			DEFAULTTONULL = 0,
			DEFAULTTOPRIMARY = 1,
			DEFAULTTONEAREST = 2
		}

		/// <summary>
		/// Monitor Display Type
		/// </summary>
		public enum MDT : uint
		{
			EFFECTIVE_DPI = 0,
			ANGULAR_DPI = 1,
			RAW_DPI = 2
		}

		public static uint GetWindowDpi(IntPtr hwnd)
		{
			// use system DPI if per-monitor DPI is not supported.
			if (!PerMonitorDpiSupported)
				return 1;

			var monitor = MonitorFromWindow(hwnd, MONITOR.DEFAULTTONEAREST);
			uint dpiX; uint dpiY;
			GetDpiForMonitor(monitor, MDT.EFFECTIVE_DPI, out dpiX, out dpiY);

			return dpiX;
		}

		public static uint GetDpi(this System.Windows.Forms.Screen screen)
		{
			if (!MonitorDpiSupported)
			{
				// fallback to slow method if we can't get the dpi from the system
				using (var form = new System.Windows.Forms.Form { Bounds = screen.Bounds })
				using (var graphics = form.CreateGraphics())
				{
					return (uint)graphics.DpiY;
				}
			}

			var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
			var mon = MonitorFromPoint(pnt, MONITOR.DEFAULTTONEAREST);
			uint dpiX, dpiY;
			GetDpiForMonitor(mon, MDT.EFFECTIVE_DPI, out dpiX, out dpiY);
			return dpiX;
		}

		[DllImport("User32.dll")]
		public static extern IntPtr MonitorFromPoint(System.Drawing.Point pt, MONITOR dwFlags);


		[DllImport("user32.dll")]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MONITOR flags);

		[DllImport("shcore.dll")]
		public static extern uint GetDpiForMonitor(IntPtr hmonitor, MDT dpiType, out uint dpiX, out uint dpiY);

		[DllImport("shcore.dll")]
		public static extern uint SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);
	}
}
