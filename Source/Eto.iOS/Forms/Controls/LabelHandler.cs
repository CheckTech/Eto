using System;
using UIKit;
using Eto.Forms;
using Eto.iOS.Drawing;
using Eto.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class LabelHandler : IosView<UILabel, Label, Label.ICallback>, Label.IHandler
	{
		public LabelHandler()
		{
			Control = new UILabel();
		}

		public string Text
		{
			get { return Control.Text; }
			set
			{ 
				LayoutIfNeeded(() => Control.Text = value);
			}
		}

		public HorizontalAlign HorizontalAlign
		{
			get
			{
				switch (Control.TextAlignment)
				{
					case UITextAlignment.Center:
						return HorizontalAlign.Center;
					case UITextAlignment.Left:
						return HorizontalAlign.Left;
					case UITextAlignment.Right:
						return HorizontalAlign.Right;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				LayoutIfNeeded(() =>
				{
					switch (value)
					{
						case HorizontalAlign.Center:
							Control.TextAlignment = UITextAlignment.Center;
							break;
						case HorizontalAlign.Left:
							Control.TextAlignment = UITextAlignment.Left;
							break;
						case HorizontalAlign.Right:
							Control.TextAlignment = UITextAlignment.Right;
							break;
						default:
							throw new NotSupportedException();
					}
				});
			}
		}

		public VerticalAlign VerticalAlign
		{
			get;
			set;
		}

		public override Eto.Drawing.Font Font
		{
			get { return base.Font; }
			set
			{
				LayoutIfNeeded(() =>
				{
					base.Font = value;
					Control.Font = value.ToUI();
				});
			}
		}

		public WrapMode Wrap
		{
			get
			{ 
				switch (Control.LineBreakMode)
				{
					case UILineBreakMode.CharacterWrap:
						return WrapMode.Character;
					case UILineBreakMode.WordWrap:
						return WrapMode.Word;
					case UILineBreakMode.Clip:
					default:
						return WrapMode.None;
				}
			}
			set
			{
				LayoutIfNeeded(() =>
				{
					switch (value)
					{
						case WrapMode.Character:
							Control.LineBreakMode = UILineBreakMode.CharacterWrap;
							break;
						case WrapMode.Word:
							Control.LineBreakMode = UILineBreakMode.WordWrap;
							break;
						case WrapMode.None:
							Control.LineBreakMode = UILineBreakMode.Clip;
							break;
						default:
							throw new NotSupportedException();
					}
				});
			}
		}

		public Eto.Drawing.Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}
	}
}

