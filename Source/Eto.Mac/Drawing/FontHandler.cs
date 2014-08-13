using System;
using Eto.Drawing;
using MonoMac.CoreGraphics;

#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif


#if IOS

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using NSFont = MonoTouch.UIKit.UIFont;

namespace Eto.iOS.Drawing

#elif OSX
	
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Mac.Drawing
#endif
{
	public class FontHandler : WidgetHandler<NSFont, Font>, Font.IHandler
	{
		FontFamily family;
		FontTypeface typeface;
		FontStyle? style;
		FontDecoration decoration;
		NSDictionary _attributes;
		NSFontTraitMask? traits;

		public FontHandler()
		{
		}

		public FontHandler(NSFont font)
		{
			Control = font;
		}

		public FontHandler(NSFont font, FontStyle style)
		{
			Control = font;
			this.style = style;
		}

		public void Create(FontTypeface face, float size, FontDecoration decoration)
		{
			typeface = face;
			family = face.Family;
			Control = ((FontTypefaceHandler)face.Handler).CreateFont(size);
			this.decoration = decoration;
		}

		public void Create(SystemFont systemFont, float? fontSize, FontDecoration decoration)
		{
			switch (systemFont)
			{
				case SystemFont.Default:
					Control = NSFont.SystemFontOfSize((CGFloat)(fontSize ?? NSFont.SystemFontSize));
					break;
				case SystemFont.Bold:
					Control = NSFont.BoldSystemFontOfSize((CGFloat)(fontSize ?? NSFont.SystemFontSize));
					break;
				case SystemFont.Label:
#if IOS
					Control = NSFont.SystemFontOfSize(fontSize ?? NSFont.LabelFontSize);
#elif OSX
					Control = NSFont.LabelFontOfSize((CGFloat)(fontSize ?? NSFont.LabelFontSize + 2)); // labels get a size of 12 
#endif
					break;
#if OSX
				case SystemFont.TitleBar:
					Control = NSFont.TitleBarFontOfSize((CGFloat)(fontSize ?? NSFont.SystemFontSize));
					break;
				case SystemFont.ToolTip:
					Control = NSFont.ToolTipsFontOfSize((CGFloat)(fontSize ?? NSFont.SystemFontSize));
					break;
				case SystemFont.MenuBar:
					Control = NSFont.MenuBarFontOfSize((CGFloat)(fontSize ?? NSFont.SystemFontSize));
					break;
				case SystemFont.Menu:
					Control = NSFont.MenuFontOfSize((CGFloat)(fontSize ?? NSFont.SystemFontSize));
					break;
				case SystemFont.Message:
					Control = NSFont.MessageFontOfSize((CGFloat)(fontSize ?? NSFont.SystemFontSize));
					break;
				case SystemFont.Palette:
					Control = NSFont.PaletteFontOfSize((CGFloat)(fontSize ?? NSFont.SmallSystemFontSize));
					break;
				case SystemFont.StatusBar:
					Control = NSFont.SystemFontOfSize((CGFloat)(fontSize ?? NSFont.SystemFontSize));
					break;
#endif
				default:
					throw new NotSupportedException();
			}
			this.decoration = decoration;
		}

		public float LineHeight
		{
			get
			{
				return Control.LineHeight(); // LineHeight() is the extension method above
			}
		}

		public static NSFont CreateFont(FontFamilyHandler familyHandler, float size, NSFontTraitMask traits, int weight = 5)
		{
			var font = NSFontManager.SharedFontManager.FontWithFamily(familyHandler.MacName, traits, weight, size);
			if (font == null)
			{
				if (traits.HasFlag(NSFontTraitMask.Italic))
				{
					// fake italics by transforming the font
					const float kRotationForItalicText = 14.0f;
					var fontTransform = new NSAffineTransform();
					fontTransform.Scale(size);
					var italicTransform = new NSAffineTransform();
					italicTransform.TransformStruct = Matrix.FromSkew(0, kRotationForItalicText).ToCG();
					fontTransform.AppendTransform(italicTransform);
					traits &= ~NSFontTraitMask.Italic;
					font = NSFontManager.SharedFontManager.FontWithFamily(familyHandler.MacName, traits, 5, size);
					if (font != null)
					{
						font = NSFont.FromDescription(font.FontDescriptor, fontTransform);
					}
				}
			}
			return font;
		}

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			this.style = style;
			this.family = family;
			this.decoration = decoration;
#if OSX
			var familyHandler = (FontFamilyHandler)family.Handler;
			traits = style.ToNS() & familyHandler.TraitMask;
			var font = CreateFont(familyHandler, size, traits.Value);

			if (font == null || font.Handle == IntPtr.Zero)
				throw new ArgumentOutOfRangeException(string.Empty, string.Format("Could not allocate font with family {0}, traits {1}, size {2}", family.Name, traits, size));
#elif IOS
			string suffix = string.Empty;
			var familyHandler = (FontFamilyHandler)family.Handler;
			var font = familyHandler.CreateFont (size, style);
			/*
			var familyString = new StringBuilder();
			switch (family)
			{
			case FontFamily.Monospace: familyString.Append ("CourierNewPS"); suffix = "MT"; break; 
			default:
			case FontFamily.Sans: familyString.Append ("Helvetica"); italicString = "Oblique"; break; 
			case FontFamily.Serif: familyString.Append ("TimesNewRomanPS"); suffix = "MT"; break; 
			}
			bold = (style & FontStyle.Bold) != 0;
			italic = (style & FontStyle.Italic) != 0;
			
			if (bold || italic) familyString.Append ("-");
			if (bold) familyString.Append ("Bold");
			if (italic) familyString.Append (italicString);
			
			familyString.Append (suffix);
			var font = UIFont.FromName (familyString.ToString (), size);
			*/
			#endif
			Control = font;
		}

		public float Size
		{
			get { return (float)Control.PointSize; }
		}

		public string FamilyName
		{
			get { return Control.FamilyName; }
		}

		public FontFamily Family
		{
			get
			{
				if (family == null)
					family = new FontFamily(new FontFamilyHandler(Control.FamilyName));
				return family;
			}
		}

		public FontTypeface Typeface
		{
			get
			{
				if (typeface == null)
					typeface = ((FontFamilyHandler)Family.Handler).GetFace(Control, traits);
				return typeface;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				if (style == null)
#if OSX
					style = NSFontManager.SharedFontManager.TraitsOfFont(Control).ToEto();
#elif IOS
					style = Typeface.FontStyle;
#endif
				return style.Value;
			}
		}

		public FontDecoration FontDecoration
		{
			get { return decoration; }
		}

		public float Ascent
		{
			get { return (float)Control.Ascender; }
		}

		public float Descent
		{
			get { return (float)-Control.Descender; }
		}

		public float XHeight
		{
#if OSX
			get { return (float)Control.XHeight; }
#elif IOS
			get { return Control.xHeight; }
#endif
		}

		public float Leading
		{
			get { return (float)Control.Leading; }
		}

		public float Baseline
		{
			get { return Control.LineHeight() - Leading - Descent; }
		}

		public NSDictionary Attributes
		{
			get
			{ 
				if (_attributes == null)
					CreateAttributes();
				return _attributes ?? (_attributes = CreateAttributes());
			}
		}

		static readonly NSObject[] attributeKeys =
		{
#if OSX
			NSAttributedString.FontAttributeName,
			NSAttributedString.UnderlineStyleAttributeName,
			NSAttributedString.StrikethroughStyleAttributeName
#elif IOS
			UIStringAttributeKey.Font,
			UIStringAttributeKey.UnderlineStyle,
			UIStringAttributeKey.StrikethroughStyle
#endif
		};

		NSDictionary CreateAttributes()
		{
			return NSDictionary.FromObjectsAndKeys(
				new NSObject[]
				{
					Control,
					new NSNumber((int)(decoration.HasFlag(FontDecoration.Underline) ? NSUnderlineStyle.Single : NSUnderlineStyle.None)),
					NSNumber.FromBoolean(decoration.HasFlag(FontDecoration.Strikethrough))
				},
				attributeKeys
			);
		}
	}
}