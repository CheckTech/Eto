using Eto.Drawing;

#if OSX
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Drawing
#elif IOS
using CoreGraphics;

namespace Eto.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="SolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : BrushHandler, SolidBrush.IHandler
	{
		public override void Apply (object control, GraphicsHandler graphics)
		{
			graphics.Control.SetFillColor ((CGColor)control);
		}

		public Color GetColor (SolidBrush widget)
		{
			return ((CGColor)widget.ControlObject).ToEtoColor ();
		}

		public void SetColor (SolidBrush widget, Color color)
		{
			widget.ControlObject = color.ToCGColor ();
		}

		object SolidBrush.IHandler.Create (Color color)
		{
			return color.ToCGColor ();
		}
	}
}

