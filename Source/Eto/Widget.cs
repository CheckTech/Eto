using System;
using System.Collections.Generic;

#if DESKTOP
using System.Xaml;
#endif

namespace Eto
{
	public interface IWidget
	{
		Widget Widget { get; set; }

		void Initialize ();
	}

#if DESKTOP
	public class PropertyStore : IAttachedPropertyStore
	{
		IDictionary<AttachableMemberIdentifier, object> attachedProperties = new Dictionary<AttachableMemberIdentifier, object> ();

		public T Get<T> (AttachableMemberIdentifier member, T defaultValue)
		{
			object value;
			if (attachedProperties.TryGetValue (member, out value))
				return (T)value;
			else
				return defaultValue;
		}

		public T Get<T> (AttachableMemberIdentifier member)
		{
			object value;
			if (attachedProperties.TryGetValue (member, out value))
				return (T)value;
			else
				return default(T);
		}

		public void Set (AttachableMemberIdentifier member, object value)
		{
			attachedProperties[member] = value;
		}

		public bool Remove (AttachableMemberIdentifier member)
		{
			return attachedProperties.Remove (member);
		}


		void IAttachedPropertyStore.CopyPropertiesTo (KeyValuePair<AttachableMemberIdentifier, object>[] array, int index)
		{
			attachedProperties.CopyTo (array, index);
		}

		int IAttachedPropertyStore.PropertyCount { get { return attachedProperties.Count; } }

		bool IAttachedPropertyStore.RemoveProperty (AttachableMemberIdentifier member)
		{
			return attachedProperties.Remove (member);
		}

		void IAttachedPropertyStore.SetProperty (AttachableMemberIdentifier member, object value)
		{
			attachedProperties[member] = value;
		}

		bool IAttachedPropertyStore.TryGetProperty (AttachableMemberIdentifier member, out object value)
		{
			return attachedProperties.TryGetValue (member, out value);
		}
	}
#endif
	
	public abstract class Widget : IDisposable
	{
		BindingCollection bindings;
		
#if DESKTOP
		PropertyStore properties;

		public PropertyStore Properties
		{
			get
			{
				if (properties == null) properties = new PropertyStore ();
				return properties;
			}
		}
#endif
		public event EventHandler<EventArgs> Disposed;

		public Generator Generator { get; private set; }
		
		public BindingCollection Bindings {
			get {
				if (bindings == null) bindings = new BindingCollection (); 
				return bindings;
			}
		}

		public object Tag { get; set; }

		public IWidget Handler { get; internal protected set; }

		~Widget ()
		{
			//Console.WriteLine ("GC: {0}", this.GetType ().FullName);
			Dispose (false);
		}
		
		protected Widget (Generator generator, IWidget handler, bool initialize = true)
		{
			this.Generator = generator;
			this.Handler = handler;
			this.Handler.Widget = this; // tell the handler who we are
			if (initialize)
				Initialize ();
		}

		protected Widget (Generator generator, Type type, bool initialize = true)
		{
			this.Generator = generator;
			this.Handler = generator.CreateControl (type, this);
			if (initialize)
				Initialize ();
		}
		
		protected void Initialize ()
		{
			Handler.Initialize ();
		}
		
		#region IDisposable Members

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
		
		#endregion
		
		public virtual void Unbind ()
		{
			if (bindings != null) {
				bindings.Unbind();
				bindings = null;
			}
		}
		
		public virtual void UpdateBindings ()
		{
			if (bindings != null) {
				bindings.Update ();
			}
		}
		
		protected virtual void Dispose (bool disposing)
		{
			Unbind ();
			if (disposing) {
				if (Disposed != null)
					Disposed(this, EventArgs.Empty);
				var handler = this.Handler as IDisposable;
				if (handler != null)
					handler.Dispose ();
				this.Handler = null;
			}
		}		
		
	}
}

