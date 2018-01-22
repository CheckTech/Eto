using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Eto.Forms;


#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Schema;
#if NETSTANDARD1_3
using cm = System.ComponentModel;
#else
using cm = Portable.Xaml.ComponentModel;
#endif

#if NET40
using EtoTypeConverter = System.ComponentModel.TypeConverter;
using EtoTypeConverterAttribute = System.ComponentModel.TypeConverterAttribute;
#else
using EtoTypeConverter = Eto.TypeConverter;
using EtoTypeConverterAttribute = Eto.TypeConverterAttribute;
using Portable.Xaml.ComponentModel;
#endif
#else
using System.Xaml;
using System.Xaml.Schema;
using cm = System.ComponentModel;
using EtoTypeConverter = Eto.TypeConverter;
using EtoTypeConverterAttribute = Eto.TypeConverterAttribute;
#endif

namespace Eto.Serialization.Xaml
{
	#if NET40
	static class TypeExtensions
	{
		public static Type GetTypeInfo(this Type type)
		{
			return type;
		}

		public static T GetCustomAttribute<T>(this Type type, bool inherit = true)
		{
			return type.GetCustomAttributes(typeof(T), inherit).OfType<T>().FirstOrDefault();
		}
	}
	#endif

	#if PORTABLE || NET45
	class TypeConverterConverter : IXamlTypeConverter
	{
		readonly EtoTypeConverter etoConverter;

		public TypeConverterConverter(EtoTypeConverter etoConverter)
		{
			this.etoConverter = etoConverter;
		}

		public bool CanConvertFrom(object context, Type sourceType)
		{
			return etoConverter.CanConvertFrom(sourceType);
		}

		public bool CanConvertTo(object context, Type destinationType)
		{
			return etoConverter.CanConvertTo(destinationType);
		}

		public object ConvertFrom(object context, System.Globalization.CultureInfo culture, object value)
		{
			return etoConverter.ConvertFrom(null, culture, value);
		}

		public object ConvertTo(object context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			return etoConverter.ConvertTo(null, culture, value, destinationType);
		}
	}

	class EtoValueConverter : XamlValueConverter<IXamlTypeConverter>
	{
		public EtoValueConverter(Type converterType, XamlType targetType)
			: base(converterType, targetType)
		{
		}

		protected override IXamlTypeConverter CreateInstance()
		{
			var etoConverter = Activator.CreateInstance(ConverterType) as EtoTypeConverter;
			return new TypeConverterConverter(etoConverter);
		}
	}
	#endif

	class EtoDesignerType : EtoXamlType
	{
		public string TypeName { get; set; }

		public string Namespace { get; set; }

		public EtoDesignerType(Type underlyingType, XamlSchemaContext schemaContext)
			: base(underlyingType, schemaContext)
		{
		}

		class DesignerInvoker : XamlTypeInvoker
		{
			public EtoDesignerType DesignerType { get; private set; }

			public DesignerInvoker(EtoDesignerType type)
				: base(type)
			{
				DesignerType = type;
			}

			public override object CreateInstance(object[] arguments)
			{
				var instance = base.CreateInstance(arguments);
				var ctl = instance as DesignerMarkupExtension;
				if (ctl != null)
				{
					ctl.Text = "[" + DesignerType.TypeName + "]";
					ctl.ToolTip = DesignerType.Namespace;
				}
				
				return instance;
			}
		}

		protected override XamlTypeInvoker LookupInvoker()
		{
			return new DesignerInvoker(this);
		}
	}

	class EtoXamlType : XamlType
	{
		public EtoXamlType(Type underlyingType, XamlSchemaContext schemaContext)
			: base(underlyingType, schemaContext)
		{
		}

		T GetCustomAttribute<T>(bool inherit = true)
			where T: Attribute
		{
			return UnderlyingType.GetTypeInfo().GetCustomAttribute<T>(inherit);
		}

		#if PORTABLE || NET45
		XamlValueConverter<IXamlTypeConverter> typeConverter;
		bool gotTypeConverter;

		protected override XamlValueConverter<IXamlTypeConverter> LookupXamlTypeConverter()
		{
			if (gotTypeConverter)
				return typeConverter;

			gotTypeConverter = true;

			// convert from Eto.TypeConverter to Portable.Xaml.ComponentModel.TypeConverter
			var typeConverterAttrib = GetCustomAttribute<EtoTypeConverterAttribute>();

			if (typeConverterAttrib == null
			    && UnderlyingType.GetTypeInfo().IsGenericType
			    && UnderlyingType.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				typeConverterAttrib = Nullable.GetUnderlyingType(UnderlyingType).GetTypeInfo().GetCustomAttribute<EtoTypeConverterAttribute>();
			}

			if (typeConverterAttrib != null)
			{
				var converterType = Type.GetType(typeConverterAttrib.ConverterTypeName);
				if (converterType != null)
					typeConverter = new EtoValueConverter(converterType, this);
			}
			if (typeof(MulticastDelegate).GetTypeInfo().IsAssignableFrom(UnderlyingType.GetTypeInfo()))
			{
				var context = SchemaContext as EtoXamlSchemaContext;
				if (context.DesignMode)
				{
					return null;
				}
			}

			if (typeConverter == null)
				typeConverter = base.LookupXamlTypeConverter();
			return typeConverter;
		}

		#endif

		class EmptyXamlMember : XamlMember
		{
			public EmptyXamlMember(EventInfo eventInfo, XamlSchemaContext context)
				: base(eventInfo, context)
			{

			}

			class EmptyConverter : IXamlTypeConverter
			{
				public bool CanConvertFrom(object context, Type sourceType) => true;

				public bool CanConvertTo(object context, Type destinationType) => false;

				public object ConvertFrom(object context, CultureInfo culture, object value) => null;

				public object ConvertTo(object context, CultureInfo culture, object value, Type destinationType) => null;
			}

			protected override XamlValueConverter<IXamlTypeConverter> LookupXamlTypeConverter()
			{
				return new XamlValueConverter<IXamlTypeConverter>(typeof(EmptyConverter), Type);
			}
		}


		protected override XamlMember LookupMember(string name, bool skipReadOnlyCheck)
		{
			var member = base.LookupMember(name, skipReadOnlyCheck);
			var context = SchemaContext as EtoXamlSchemaContext;
			if (member != null && member.IsEvent)
			{
				if (context != null && context.DesignMode)
				{
					// in design mode, ignore wiring up events
					return new EmptyXamlMember(member.UnderlyingMember as EventInfo, context);
				}
			}
			return member;
		}

		protected override bool LookupIsAmbient()
		{
			if (this.UnderlyingType != null && UnderlyingType == typeof(PropertyStore))
				return true;
			return base.LookupIsAmbient();
		}

		bool gotContentProperty;
		XamlMember contentProperty;

		protected override XamlMember LookupContentProperty()
		{
			if (gotContentProperty)
				return contentProperty;
			gotContentProperty = true;
			var contentAttribute = GetCustomAttribute<ContentPropertyAttribute>();
			if (contentAttribute == null || contentAttribute.Name == null)
				contentProperty = base.LookupContentProperty();
			else
				contentProperty = GetMember(contentAttribute.Name);
			return contentProperty;
		}

		XamlMember nameAliasedProperty;

		protected override XamlMember LookupAliasedProperty(XamlDirective directive)
		{
			if (directive == XamlLanguage.Name)
			{
				if (nameAliasedProperty != null)
					return nameAliasedProperty;

				var nameAttribute = GetCustomAttribute<RuntimeNamePropertyAttribute>();
				if (nameAttribute != null && nameAttribute.Name != null)
				{
					nameAliasedProperty = GetMember(nameAttribute.Name);
					return nameAliasedProperty;
				}

			}
			return base.LookupAliasedProperty(directive);
		}
	}
}