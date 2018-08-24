using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Direct2D.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<sw.FontFace, FontTypeface>, FontTypeface.IHandler
    {
		public sw.Font Font { get; private set; }

		public FontTypefaceHandler(sw.Font font)
		{
			Font = font;
			Control = new sw.FontFace(font);
		}

		public FontStyle FontStyle => Font.ToEtoStyle();

		public string Name => Font.FaceNames.GetString(0);

		public string LocalizedName
		{
			get
			{
				int index;
				if (!Font.FaceNames.FindLocaleName(CultureInfo.CurrentUICulture.Name, out index))
					Font.FaceNames.FindLocaleName("en-us", out index);
				return Font.FaceNames.GetString(index);
			}
		}

	}
}
