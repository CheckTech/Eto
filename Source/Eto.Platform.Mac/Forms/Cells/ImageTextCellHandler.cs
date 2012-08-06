using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Drawing;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class ImageTextCellHandler : CellHandler<NSTextFieldCell, ImageTextCell>, IImageTextCell
	{
		public class EtoCell : MacImageListItemCell, IMacControl
		{
			public object Handler { get; set; }
			
			public EtoCell ()
			{
			}
			
			public EtoCell (IntPtr handle) : base(handle)
			{
			}
			
			[Export("copyWithZone:")]
			NSObject CopyWithZone (IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, MacCommon.selCopyWithZone.Handle, zone);
				return new EtoCell (ptr) { Handler = this.Handler };
			}
		}
		
		public ImageTextCellHandler ()
		{
			Control = new EtoCell { Handler = this };
		}

		public override void SetBackgroundColor (NSCell cell, Color color)
		{
			var c = cell as EtoCell;
			c.BackgroundColor = Generator.ConvertNS (color);
			c.DrawsBackground = color != Color.Transparent;
		}

		public override Color GetBackgroundColor (NSCell cell)
		{
			var c = cell as EtoCell;
			return Generator.Convert (c.BackgroundColor);
		}

		public override void SetForegroundColor (NSCell cell, Color color)
		{
			var c = cell as EtoCell;
			c.TextColor = Generator.ConvertNS (color);
		}

		public override Color GetForegroundColor (NSCell cell)
		{
			var c = cell as EtoCell;
			return Generator.Convert (c.TextColor);
		}

		public override NSObject GetObjectValue (object dataItem)
		{
			var result = new MacImageData();
			if (Widget.TextBinding != null) {
				result.Text = (NSString)Convert.ToString (Widget.TextBinding.GetValue (dataItem));
			}
			if (Widget.ImageBinding != null) {
				var image = Widget.ImageBinding.GetValue (dataItem) as Image;
				result.Image = image != null ? ((IImageSource)image.Handler).GetImage () : null;
			}
			else result.Image = new NSImage();
			return result;
		}
		
		public override void SetObjectValue (object dataItem, NSObject val)
		{
			if (Widget.TextBinding != null) {
				var str = val as NSString;
				if (str != null)
					Widget.TextBinding.SetValue (dataItem, (string)str);
				else
					Widget.TextBinding.SetValue (dataItem, null);
			}
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize)
		{
			var val = value as MacImageData;
			if (val == null) return 0;
			
			var font = Control.Font ?? NSFont.SystemFontOfSize (NSFont.SystemFontSize);
			var str = val.Text;
			var attrs = NSDictionary.FromObjectAndKey (font, NSAttributedString.FontAttributeName);
			
			var size = str.StringSize (attrs).Width + 4 + 16 + MacImageListItemCell.ImagePadding * 2; // for border + image
			return size;
			
		}
	}
}

