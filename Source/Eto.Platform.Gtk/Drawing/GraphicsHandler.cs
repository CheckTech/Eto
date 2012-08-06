using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class RegionHandler : Region
	{

		public RegionHandler ()
		{
		}

		public override object ControlObject {
			get { return null; }
		}

		public override void Exclude (Rectangle rect)
		{
		}

		public override void Reset ()
		{
		}

		public override void Set (Rectangle rect)
		{
		}
	}

	public class GraphicsHandler : WidgetHandler<Cairo.Context, Graphics>, IGraphics
	{
		Gtk.Widget widget;
		Gdk.Drawable drawable;
		Image image;
		Cairo.ImageSurface surface;

		public GraphicsHandler ()
		{
		}
		
		public GraphicsHandler (Gtk.Widget widget, Gdk.Drawable drawable, Gdk.GC gc)
		{
			this.widget = widget;
			this.drawable = drawable;
			this.Control = Gdk.CairoHelper.Create (drawable);
		}

		public bool Antialias {
			get { return Control.Antialias != Cairo.Antialias.None; }
			set {
				if (value)
					Control.Antialias = Cairo.Antialias.Default;
				else
					Control.Antialias = Cairo.Antialias.None;
			}
		}

		public void CreateFromImage (Bitmap image)
		{
			this.image = image;
			var handler = (BitmapHandler)image.Handler;

			if (handler.Alpha)
				surface = new Cairo.ImageSurface (Cairo.Format.Argb32, image.Size.Width, image.Size.Height);
			else
				surface = new Cairo.ImageSurface (Cairo.Format.Rgb24, image.Size.Width, image.Size.Height);
			Control = new Cairo.Context (surface);
			Control.Save ();
			Control.Rectangle (0, 0, image.Size.Width, image.Size.Height);
			Gdk.CairoHelper.SetSourcePixbuf (Control, handler.Control, 0, 0);
			Control.Operator = Cairo.Operator.Source;
			Control.Fill ();
			Control.Restore ();
		}
		
		public void Flush ()
		{
			if (image != null) {
				var handler = (BitmapHandler)image.Handler;
				Gdk.Pixbuf pb = (Gdk.Pixbuf)image.ControlObject;
				if (pb != null) {

					surface.Flush ();
					var bd = handler.Lock ();
					unsafe {
						byte* srcrow = (byte*)surface.DataPtr;
						byte* destrow = (byte*)bd.Data;
						for (int y=0; y<image.Size.Height; y++) {
							uint* src = (uint*)srcrow;
							uint* dest = (uint*)destrow;
							for (int x=0; x<image.Size.Width; x++) {
								*dest = bd.TranslateArgbToData(*src);
								dest++;
								src++;
							}
							destrow += bd.ScanWidth;
							srcrow += surface.Stride;
						}
					}
					handler.Unlock (bd);
				}
			}
			if (Control != null)
			{
				((IDisposable)Control).Dispose();
				if (surface != null) {
					this.Control = new Cairo.Context (surface);
				}
				else if (drawable != null) {
					this.Control = Gdk.CairoHelper.Create (drawable);
				}
			}
		}

		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
			Control.Save ();
			Control.Color = Generator.ConvertC (color);
			if (startx != endx || starty != endy) {
				// to draw a line, it must move..
				Control.MoveTo (startx + 0.5, starty + 0.5);
				Control.LineTo (endx + 0.5, endy + 0.5);
				Control.LineCap = Cairo.LineCap.Square;
				Control.LineWidth = 1.0;
				Control.Stroke ();
			} else {
				// to draw one pixel, we must fill it
				Control.Rectangle (startx, starty, 1, 1);
				Control.Fill ();
			}
			Control.Restore ();
		}

		public void DrawRectangle (Color color, int x, int y, int width, int height)
		{
			Control.Save ();
			Control.Color = Generator.ConvertC (color);
			Control.Rectangle (x + 0.5, y + 0.5, width - 1, height - 1);
			Control.LineWidth = 1.0;
			Control.Stroke ();
			Control.Restore ();
		}

		public void FillRectangle (Color color, int x, int y, int width, int height)
		{
			Control.Save ();
			Control.Color = Generator.ConvertC (color);
			Control.Rectangle (x, y, width, height);
			Control.Fill ();
			Control.Restore ();
		}
		
		public void FillPath (Color color, GraphicsPath path)
		{
			Control.Save ();
			Control.Color = Generator.ConvertC (color);
			var pathHandler = path.Handler as GraphicsPathHandler;
			pathHandler.Apply (this);
			Control.Fill ();
			Control.Restore ();
		}

		public void DrawPath (Color color, GraphicsPath path)
		{
			Control.Save ();
			Control.Color = Generator.ConvertC (color);
			var pathHandler = path.Handler as GraphicsPathHandler;
			pathHandler.Apply (this);
			Control.LineCap = Cairo.LineCap.Square;
			Control.LineWidth = 1.0;
			Control.Stroke ();
			Control.Restore ();
		}
		
		public void DrawImage (Image image, int x, int y)
		{
			((IImageHandler)image.Handler).DrawImage (this, x, y);
		}

		public void DrawImage (Image image, int x, int y, int width, int height)
		{
			((IImageHandler)image.Handler).DrawImage (this, x, y, width, height);
		}

		public void DrawImage (Image image, Rectangle source, Rectangle destination)
		{
			((IImageHandler)image.Handler).DrawImage (this, source, destination);
		}

		public void DrawIcon (Icon icon, int x, int y, int width, int height)
		{
			var iconHandler = ((IconHandler)icon.Handler);
			var pixbuf = iconHandler.Pixbuf;
			Control.Save ();
			Gdk.CairoHelper.SetSourcePixbuf(Control, pixbuf, 0, 0);
			if (width != pixbuf.Width || height != pixbuf.Height) {
				Control.Scale ((double)width / (double)pixbuf.Width, (double)height / (double)pixbuf.Height);
			}
			Control.Rectangle (x, y, width, height);
			Control.Fill ();
			Control.Restore ();
		}
		
		public Region ClipRegion {
			get { return null; }
			set {
				
			}
		}

		public void DrawText (Font font, Color color, int x, int y, string text)
		{
			if (widget != null) {
				using (var layout = new Pango.Layout (widget.PangoContext)) {
					layout.FontDescription = (Pango.FontDescription)font.ControlObject;
					layout.SetText (text);
					Control.Save ();
					Control.Color = Generator.ConvertC (color);
					Control.MoveTo (x, y);
					Pango.CairoHelper.LayoutPath (Control, layout);
					Control.Fill ();
					Control.Restore ();
				}
			}
		}

		public SizeF MeasureString (Font font, string text)
		{
			if (widget != null) {

				Pango.Layout layout = new Pango.Layout (widget.PangoContext);
				layout.FontDescription = (Pango.FontDescription)font.ControlObject;
				layout.SetText (text);
				int width, height;
				layout.GetPixelSize (out width, out height);
				layout.Dispose ();
				return new SizeF (width, height);
			}
			return new SizeF ();
		}

		protected override void Dispose (bool disposing)
		{
			if (image != null)
				Flush ();
			
			base.Dispose (disposing);
		}
	}
}
