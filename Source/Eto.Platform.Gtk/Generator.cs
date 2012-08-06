using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using Eto.Platform.GtkSharp.Drawing;
using System.Threading;

namespace Eto.Platform.GtkSharp
{
	public class Generator : Eto.Generator
	{ 	
		public const string GeneratorID = "gtk";
		
		public override string ID {
			get {
				return GeneratorID;
			}
		}
		
		public Generator ()
		{
			if (Eto.Misc.Platform.IsWindows && Environment.Is64BitProcess)
				throw new NotSupportedException("Please compile/run GTK in x86 mode (32-bit) on windows");
			Gtk.Application.Init();
			
			Gdk.Threads.Enter ();
		}
		
		public static Gdk.Color Convert (Color color)
		{
			return new Gdk.Color ((byte)(color.R * byte.MaxValue), (byte)(color.G * byte.MaxValue), (byte)(color.B * byte.MaxValue));
		}
		
		public static Cairo.Color ConvertC (Color color)
		{
			return new Cairo.Color ((double)color.R, (double)color.G, (double)color.B, (double)color.A);
		}

		public static Cairo.Rectangle ConvertC (Rectangle rectangle)
		{
			return new Cairo.Rectangle (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public static Rectangle ConvertC (Cairo.Rectangle rectangle)
		{
			return new Rectangle ((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
		}
		
		public static Color Convert (Gdk.Color color)
		{
			return new Color ((float)color.Red / ushort.MaxValue, (float)color.Green / ushort.MaxValue, (float)color.Blue / ushort.MaxValue);
		}

		public static Gdk.Size Convert (Size size)
		{
			return new Gdk.Size (size.Width, size.Height);
		}

		public static Size Convert (Gdk.Size size)
		{
			return new Size (size.Width, size.Height);
		}

		public static Size Convert (Gtk.Requisition req)
		{
			return new Size (req.Width, req.Height);
		}

		public static Gdk.Point Convert (Point point)
		{
			return new Gdk.Point (point.X, point.Y);
		}

		public static Point Convert (Gdk.Point point)
		{
			return new Point (point.X, point.Y);
		}

		public static Gdk.Rectangle Convert (Rectangle rect)
		{
			return new Gdk.Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Rectangle Convert (Gdk.Rectangle rect)
		{
			return new Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static DialogResult Convert (Gtk.ResponseType result)
		{
			DialogResult ret = DialogResult.None;
			if (result == Gtk.ResponseType.Ok)
				ret = DialogResult.Ok;
			else if (result == Gtk.ResponseType.Cancel)
				ret = DialogResult.Cancel;
			else if (result == Gtk.ResponseType.Yes)
				ret = DialogResult.Yes;
			else if (result == Gtk.ResponseType.No)
				ret = DialogResult.No;
			else if (result == Gtk.ResponseType.None)
				ret = DialogResult.None;
			else if (result == Gtk.ResponseType.Accept)
				ret = DialogResult.Ignore;
			else if (result == Gtk.ResponseType.Reject)
				ret = DialogResult.Abort;
			else
				ret = DialogResult.None;

			return ret;
		}

		public static string Convert (ImageFormat format)
		{
			switch (format) {
			case ImageFormat.Jpeg:
				return "jpeg";
			case ImageFormat.Bitmap:
				return "bmp";
			case ImageFormat.Gif:
				return "gif";
			case ImageFormat.Tiff:
				return "tiff";
			case ImageFormat.Png:
				return "png";
			default:
				throw new Exception ("Invalid format specified");
			}
		}
		
		public static Gdk.CursorType Convert(CursorType cursor)
		{
			switch (cursor) {
			case CursorType.Arrow: return Gdk.CursorType.Arrow;
			case CursorType.Crosshair: return Gdk.CursorType.Crosshair;
			case CursorType.Default: return Gdk.CursorType.Arrow;
			case CursorType.HorizontalSplit: return Gdk.CursorType.SbHDoubleArrow;
			case CursorType.VerticalSplit: return Gdk.CursorType.SbVDoubleArrow;
			case CursorType.IBeam: return Gdk.CursorType.Xterm;
			case CursorType.Move: return Gdk.CursorType.Fleur;
			case CursorType.Pointer: return Gdk.CursorType.Hand2;
			default:
				throw new NotSupportedException();
			}
		}
	}
}
