using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;
using MonoMac.Foundation;
using System.Collections.Generic;
namespace Eto.Platform.Mac.Forms.Controls
{
	public abstract class MacControl<T, W> : MacView<T, W>
		where T: NSControl
		where W: Control
	{
		Font font;
		
		public override bool Enabled {
			get {
				return Control.Enabled;
			}
			set {
				Control.Enabled = value;
			}
		}
		
		public virtual Font Font {
			get {
				return font;
			}
			set {
				font = value;
				if (font != null)
					Control.Font = font.ControlObject as NSFont;
				else
					Control.Font = null;
				Control.SizeToFit ();
				LayoutIfNeeded();
			}
		}
	}
}

