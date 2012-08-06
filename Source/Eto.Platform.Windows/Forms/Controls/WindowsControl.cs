using System;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	public interface IWindowsControl
	{
		bool InternalVisible { get; }

		SWF.DockStyle DockStyle { get; }
	}

	public abstract class WindowsControl<T, W> : WidgetHandler<T, W>, IControl, IWindowsControl
		where T: System.Windows.Forms.Control
		where W: Control
	{
		bool internalVisible = true;
		Font font;
		Cursor cursor;
		string tooltip;

		public override void Initialize ()
		{
			Control.TabIndex = 100;
			this.Control.Margin = SWF.Padding.Empty;
		}
		
		public virtual SWF.DockStyle DockStyle {
			get {
				return SWF.DockStyle.Fill;
			}
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Eto.Forms.Control.KeyDownEvent: 
				Control.KeyDown += new SWF.KeyEventHandler (Control_KeyDown);
				Control.KeyPress += new System.Windows.Forms.KeyPressEventHandler (Control_KeyPress);
				break;
				
			case Eto.Forms.Control.TextChangedEvent:
				Control.TextChanged += Control_TextChanged;
				break;
			case Eto.Forms.Control.SizeChangedEvent:
				Control.SizeChanged += Control_SizeChanged;
				break;
			case Eto.Forms.Control.MouseDoubleClickEvent:
				Control.MouseDoubleClick += Control_DoubleClick;
				break;
			case Eto.Forms.Control.MouseEnterEvent:
				Control.MouseEnter += HandleControlMouseEnter;
				break;
			case Eto.Forms.Control.MouseLeaveEvent:
				Control.MouseLeave += HandleControlMouseLeave;
				break;
			case Eto.Forms.Control.MouseDownEvent:
				Control.MouseDown += Control_MouseDown;
				break;
			case Eto.Forms.Control.MouseUpEvent:
				Control.MouseUp += Control_MouseUp;
				break;
			case Eto.Forms.Control.MouseMoveEvent:
				Control.MouseMove += Control_MouseMove;
				break;
			case Eto.Forms.Control.GotFocusEvent:
				Control.GotFocus += delegate {
					Widget.OnGotFocus (EventArgs.Empty); 
				};
				break;
			case Eto.Forms.Control.LostFocusEvent:
				Control.LostFocus += delegate {
					Widget.OnLostFocus (EventArgs.Empty);
				};
				break;
			}
		}

		void HandleControlMouseLeave (object sender, EventArgs e)
		{
			Widget.OnMouseLeave (new MouseEventArgs (MouseButtons.None, KeyMap.Convert (SWF.Control.ModifierKeys), Point.Empty));
		}

		void HandleControlMouseEnter (object sender, EventArgs e)
		{
			Widget.OnMouseEnter (new MouseEventArgs (MouseButtons.None, KeyMap.Convert (SWF.Control.ModifierKeys), Point.Empty));
		}

		void Control_DoubleClick (object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Widget.OnMouseDoubleClick (GetMouseEvent (e));
		}

		MouseEventArgs GetMouseEvent (System.Windows.Forms.MouseEventArgs e)
		{
			Point point = new Point (e.X, e.Y);
			MouseButtons buttons = MouseButtons.None;
			if ((e.Button & SWF.MouseButtons.Left) != 0)
				buttons |= MouseButtons.Primary;
			if ((e.Button & SWF.MouseButtons.Right) != 0)
				buttons |= MouseButtons.Alternate;
			if ((e.Button & SWF.MouseButtons.Middle) != 0)
				buttons |= MouseButtons.Middle;
			Key modifiers = KeyMap.Convert (SWF.Control.ModifierKeys);
			
			return new MouseEventArgs (buttons, modifiers, point);
		}

		void Control_MouseUp (Object sender, SWF.MouseEventArgs e)
		{
			Widget.OnMouseUp (GetMouseEvent (e));
		}

		void Control_MouseMove (Object sender, SWF.MouseEventArgs e)
		{
			Widget.OnMouseMove (GetMouseEvent (e));
		}

		void Control_MouseDown (object sender, SWF.MouseEventArgs e)
		{
			Widget.OnMouseDown (GetMouseEvent (e));
		}

		public virtual string Text {
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public virtual Size Size {
			get { return Generator.Convert (Control.Size); }
			set {
				this.Control.AutoSize = value.Width == -1 || value.Height == -1;
				Control.Size = Generator.Convert (value);
			}
		}

		public virtual Size ClientSize {
			get	{ return new Size (Control.ClientSize.Width, Control.ClientSize.Height); }
			set {
                this.Control.AutoSize = value.Width == -1 || value.Height == -1;
				Control.ClientSize = Generator.Convert (value);
			}
		}

		public bool Enabled {
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		
		public Cursor Cursor {
			get { return cursor; }
			set {
				cursor = value;
				if (cursor != null)
					this.Control.Cursor = cursor.ControlObject as SWF.Cursor;
				else
					this.Control.Cursor = null;
			}
		}
		
		public string ToolTip {
			get { return tooltip; }
			set {
				tooltip = value;
				SetToolTip ();
			}
		}

		public void Invalidate ()
		{
			Control.Invalidate (true);
		}

		public void Invalidate (Rectangle rect)
		{
			Control.Invalidate (Generator.Convert (rect), true);
		}

		public virtual Color BackgroundColor {
			get { return Generator.Convert (Control.BackColor); }
			set { Control.BackColor = Generator.Convert (value); }
		}

		public Graphics CreateGraphics ()
		{
			return new Graphics (Widget.Generator, new GraphicsHandler (Control.CreateGraphics ()));
		}

		public virtual void SuspendLayout ()
		{
			Control.SuspendLayout ();
		}

		public virtual void ResumeLayout ()
		{
			Control.ResumeLayout ();
		}

		public void Focus ()
		{
			if (!Control.Visible)
				Control.TabIndex = 0;
			Control.Focus ();
		}

		public bool HasFocus {
			get { return Control.Focused; }
		}

		bool IWindowsControl.InternalVisible {
			get { return internalVisible; }
		}

		public bool Visible {
			get { return Control.Visible; }
			set {
				internalVisible = value;
				Control.Visible = value;
			}
		}

		public virtual void SetLayout (Layout layout)
		{
		}

		public virtual void SetParentLayout (Layout layout)
		{
		}

		public virtual void SetParent (Control parent)
		{
		}

		void Control_SizeChanged (object sender, EventArgs e)
		{
			Widget.OnSizeChanged (e);
		}

		public virtual void OnPreLoad (EventArgs e)
		{
		}
		
		public virtual void OnLoad (EventArgs e)
		{
		}

		public virtual void OnLoadComplete (EventArgs e)
		{
			SetToolTip ();
		}
		
		void SetToolTip ()
		{
			if (this.Widget.ParentWindow != null) {
				var parent = this.Widget.ParentWindow.Handler as IWindowHandler;
				if (parent != null)
					parent.ToolTips.SetToolTip (Control, tooltip);
			}
		}
		
		Key key;
		bool handled;
		char keyChar;
		bool charPressed;

		void Control_KeyDown (object sender, System.Windows.Forms.KeyEventArgs e)
		{
			charPressed = false;
			handled = true;
			key = KeyMap.Convert (e.KeyCode) | KeyMap.Convert (e.Modifiers);

			if (key != Key.None) {
				KeyPressEventArgs kpea = new KeyPressEventArgs (key);
				Widget.OnKeyDown (kpea);
				e.Handled = kpea.Handled;
				handled = kpea.Handled;
			} else
				handled = false;
			if (!handled && charPressed) {
				// this is when something in the event causes messages to be processed for some reason (e.g. show dialog box)
				// we want the char event to come after the dialog is closed, and handled is set to true!
				KeyPressEventArgs kpea = new KeyPressEventArgs (key, keyChar);
				Widget.OnKeyDown (kpea);
				e.Handled = kpea.Handled;
			}
		}

		void Control_KeyPress (object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			charPressed = true;
			keyChar = e.KeyChar;
			if (!handled) {
				KeyPressEventArgs kpea = new KeyPressEventArgs (key, keyChar);
				Widget.OnKeyDown (kpea);
				e.Handled = kpea.Handled;
			} else
				e.Handled = true;
		}

		void Control_TextChanged (object sender, EventArgs e)
		{
			Widget.OnTextChanged (e);
		}
		
		public Font Font {
			get { return font; }
			set {
				font = value;
				if (font != null)
					this.Control.Font = font.ControlObject as System.Drawing.Font;
				else
					this.Control.Font = null;
			}
		}
		
		public virtual void MapPlatformAction (string systemAction, BaseAction action)
		{
		}
	}
}
