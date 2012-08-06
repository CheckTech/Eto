using System;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;

#if DESKTOP
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	public partial interface IContainer : IControl
	{
		Size ClientSize { get; set; }

		object ContainerObject { get; }

		void SetLayout (Layout layout);
	}
	
#if DESKTOP
	[ContentProperty("Layout")]
#endif
	public partial class Container : Control
	{
		IContainer handler;
		Layout layout;

		public IEnumerable<Control> Controls {
			get { 
				if (Layout != null)
					return Layout.Controls;
				else
					return Enumerable.Empty<Control> ();
			}
		}
		
		public IEnumerable<Control> Children {
			get {
				if (Layout != null) {
					foreach (var control in Layout.Controls) {
						yield return control;
						var container = control as Container;
						if (container != null) {
							foreach (var child in container.Children)
								yield return child;
						}
					}
				}
			}
		}
		
		public override void OnPreLoad (EventArgs e)
		{
			base.OnPreLoad (e);
			
			if (Layout != null)
				Layout.OnPreLoad (e);
			
			foreach (Control control in Controls) {
				control.OnPreLoad (e);
			}
		}
		
		public override void OnLoad (EventArgs e)
		{
			foreach (Control control in Controls) {
				control.OnLoad (e);
			}
			
			base.OnLoad (e);
			
			if (Layout != null)
				Layout.OnLoad (e);
		}

		public override void OnLoadComplete (EventArgs e)
		{
			foreach (Control control in Controls) {
				control.OnLoadComplete (e);
			}
			
			base.OnLoadComplete (e);
			
			if (Layout != null)
				Layout.OnLoadComplete (e);
		}
		
		protected Container (Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
			handler = (IContainer)base.Handler;
		}
		
		public object ContainerObject {
			get { return handler.ContainerObject; }
		}
		
		public Layout Layout {
			get { return layout; }
			set {
				layout = value;
				layout.Container = this;
				SetInnerLayout ();
			}
		}

		public void SetInnerLayout ()
		{
			var innerLayout = layout.InnerLayout;
			if (innerLayout != null) {
				innerLayout.Container = this;
				handler.SetLayout (innerLayout);
				if (Loaded) {
					layout.OnPreLoad (EventArgs.Empty);
					layout.OnLoad (EventArgs.Empty);
					layout.OnLoadComplete (EventArgs.Empty);
				}
			}
		}
		
		public Size ClientSize {
			get { return handler.ClientSize; }
			set { handler.ClientSize = value; }
		}
		
		public override void Unbind ()
		{
			base.Unbind ();
			foreach (var control in Controls) {
				control.Unbind ();
			}
		}
		
		public override void UpdateBindings ()
		{
			base.UpdateBindings ();
			foreach (var control in Controls) {
				control.UpdateBindings ();
			}
		}
	}
}
