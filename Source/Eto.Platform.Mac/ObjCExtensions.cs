using System;
#if DESKTOP
using MonoMac.ObjCRuntime;
#elif MOBILE
using MonoTouch.ObjCRuntime;
#endif
using System.Runtime.InteropServices;

namespace Eto.Platform.Mac
{
	public static class ObjCExtensions
	{
		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getClassMethod (IntPtr cls, IntPtr sel);
		
		public static IntPtr GetMethod (this Class cls, IntPtr selector)
		{
			return class_getClassMethod (cls.Handle, selector);
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern bool class_addMethod (IntPtr cls, IntPtr sel, Delegate method, string argTypes);
		
		public static bool AddMethod (this Class cls, IntPtr selector, Delegate method, string arguments)
		{
			return class_addMethod (cls.Handle, selector, method, arguments);
		}
		
		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern bool method_exchangeImplementations (IntPtr method1, IntPtr method2);
		
		public static void ExchangeMethod (this Class cls, IntPtr selMethod1, IntPtr selMethod2)
		{
			var method1 = class_getClassMethod (cls.Handle, selMethod1);
			var method2 = GetMethod (cls, selMethod2);
			method_exchangeImplementations (method1, method2);
		}
		
		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr object_getClass (IntPtr obj);
		
		public static Class GetClass (IntPtr cls)
		{
			return new Class (object_getClass (cls));
		}
		
		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr objc_getMetaClass (string metaClassName);

		public static Class GetMetaClass (string metaClassName)
		{
			return new Class (objc_getMetaClass (metaClassName));
		}
	}
}

