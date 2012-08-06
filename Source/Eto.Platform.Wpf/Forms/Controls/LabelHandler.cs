﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class LabelHandler : WpfControl<System.Windows.Controls.Label, Label>, ILabel
	{
		System.Windows.Controls.TextBlock text;

		public LabelHandler ()
		{
			Control = new System.Windows.Controls.Label ();
			text = new System.Windows.Controls.TextBlock ();
			Control.Content = text;
			HorizontalAlign = HorizontalAlign.Left;
			VerticalAlign = VerticalAlign.Top;
		}

		public HorizontalAlign HorizontalAlign
		{
			get { 
				switch (Control.HorizontalContentAlignment) {
					case System.Windows.HorizontalAlignment.Left:
						return Eto.Forms.HorizontalAlign.Left;
					case System.Windows.HorizontalAlignment.Right:
						return Eto.Forms.HorizontalAlign.Right;
					case System.Windows.HorizontalAlignment.Center:
						return Eto.Forms.HorizontalAlign.Center;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
					case Eto.Forms.HorizontalAlign.Center:
						Control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
						break;
					case Eto.Forms.HorizontalAlign.Left:
						Control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
						break;
					case Eto.Forms.HorizontalAlign.Right:
						Control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public VerticalAlign VerticalAlign
		{
			get
			{
				switch (Control.VerticalContentAlignment) {
					case System.Windows.VerticalAlignment.Top:
						return Eto.Forms.VerticalAlign.Top;
					case System.Windows.VerticalAlignment.Bottom:
						return Eto.Forms.VerticalAlign.Bottom;
					case System.Windows.VerticalAlignment.Center:
						return Eto.Forms.VerticalAlign.Middle;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
					case Eto.Forms.VerticalAlign.Top:
						Control.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
						break;
					case Eto.Forms.VerticalAlign.Bottom:
						Control.VerticalContentAlignment = System.Windows.VerticalAlignment.Bottom;
						break;
					case Eto.Forms.VerticalAlign.Middle:
						Control.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}

		public WrapMode Wrap
		{
			get
			{
				switch (text.TextWrapping) {
					case System.Windows.TextWrapping.NoWrap:
						return WrapMode.None;
					case System.Windows.TextWrapping.Wrap:
						return WrapMode.Word;
					case System.Windows.TextWrapping.WrapWithOverflow:
						return WrapMode.Character;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
					case WrapMode.Word:
						text.TextWrapping = System.Windows.TextWrapping.Wrap;
						break;
					case WrapMode.Character:
						text.TextWrapping = System.Windows.TextWrapping.WrapWithOverflow;
						break;
					case WrapMode.None:
						text.TextWrapping = System.Windows.TextWrapping.NoWrap;
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}

		public Color TextColor
		{
			get
			{
				var b = (System.Windows.Media.SolidColorBrush)text.Foreground;
				return Generator.Convert (b.Color);
			}
			set
			{
				text.Foreground = new System.Windows.Media.SolidColorBrush (Generator.Convert (value));
			}
		}

		public string Text
		{
			get { return text.Text; }
			set { text.Text = value; }
		}
	}
}
