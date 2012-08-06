using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms.Cells
{
	public class ImageViewCellHandler : SingleCellHandler<Gtk.CellRendererPixbuf, ImageViewCell>, IImageViewCell
	{
		class Renderer : Gtk.CellRendererPixbuf
		{
			public ImageViewCellHandler Handler { get; set; }

			[GLib.Property("item")]
			public object Item { get; set; }

			[GLib.Property("row")]
			public int Row { get; set; }

			public override void GetSize (Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize (widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}

			protected override void Render (Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				if (Handler.FormattingEnabled)
					Handler.Format(new GtkGridCellFormatEventArgs<Renderer> (this, Handler.Column.Widget, Item, Row));
				base.Render (window, widget, background_area, cell_area, expose_area, flags);
			}
		}


		public ImageViewCellHandler ()
		{
			Control = new Renderer { Handler = this };
		}

		protected override void BindCell (ref int dataIndex)
		{
			Column.Control.ClearAttributes (Control);
			SetColumnMap (dataIndex);
			Column.Control.AddAttribute (Control, "pixbuf", dataIndex++);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
		}
		
		public override void SetValue (object dataItem, object value)
		{
			// can't set
		}
		
		protected override GLib.Value GetValueInternal (object item, int column, int row)
		{
			if (Widget.Binding != null) {
				var ret = Widget.Binding.GetValue (item);
				var image = ret as Image;
				if (image != null)
					return new GLib.Value(((IGtkPixbuf)image.Handler).GetPixbuf (new Size (16, 16)));
			}
			return new GLib.Value((Gdk.Pixbuf)null);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case GridView.EndCellEditEvent:
				// no editing here
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
	}
}

