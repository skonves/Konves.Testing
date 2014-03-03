using System;
using System.Linq;
using System.Reflection;

namespace Konves.Testing
{
	/// <summary>
	/// Provides functionality for unit-testing instances of non-public classes and classes with non-public members.
	/// </summary>
	public sealed class InstanceProxy
	{
		private InstanceProxy(Type type, params object[] args)
		{
			m_type = type;

			Type[] types = args.Select(a => a == null ? typeof(object) : a.GetType()).ToArray();

			ConstructorInfo ci = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, types, null);

			if (ci == null)
				ci = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(c => c.GetParameters().Length == args.Length);

			m_instance = ci.Invoke(args);
		}

		private InstanceProxy(Type type, object instance)
		{
			m_type = type;
			m_instance = instance;
		}

		/// <summary>
		/// Gets a proxy for the specified object instance.
		/// </summary>
		/// <param name="instance">The instance of the object to proxy.</param>
		public static InstanceProxy For(object instance)
		{
			return new InstanceProxy(instance.GetType(), instance);
		}

		/// <summary>
		/// Gets a proxy for an instance of the specified <see cref="Konves.Testing.TypeProxy">TypeProxy</see> constructed using the specified arguments.
		/// </summary>
		/// <param name="typeProxy">The <see cref="Konves.Testing.TypeProxy">TypeProxy</see> of the object to construct.</param>
		/// <param name="args">The arguments to pass to the constructor.</param>
		public static InstanceProxy For(TypeProxy typeProxy, params object[] args)
		{
			return new InstanceProxy(typeProxy.Type, args);
		}

		/// <summary>
		/// Gets a proxy for an instance of the specified <see cref="System.Type">Type</see> constructed using the specified arguments.
		/// </summary>
		/// <param name="type">The type the object to construct.</param>
		/// <param name="args">The arguments to pass to the constructor.</param>
		public static InstanceProxy For(Type type, params object[] args)
		{
			return new InstanceProxy(type, args);
		}

		/// <summary>
		/// Gets a proxy for an instance of the specified assembly and class constructed using the specified arguments.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <param name="className">Name of the class to proxy.</param>
		/// <param name="args">The arguments to pass to the constructor.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Type could not be found in assembly.</exception>
		/// <exception cref="System.ArgumentException">Assembly could not be found.</exception>
		public static InstanceProxy For(string assemblyName, string className, params object[] args)
		{
			Assembly assy;
			Type type = null;
			try
			{
				assy = Assembly.Load(assemblyName);

				try
				{
					type = assy.GetType(className, true);
				}
				catch (Exception ex)
				{
					throw new ArgumentException(string.Format("Type '{0}' could not be found in assembly '{1}'.", className, assemblyName), "className", ex);
				}
			}
			catch (Exception ex)
			{
				throw new ArgumentException(string.Format("Assembly '{0}' could not be found.", assemblyName), "assemblyName", ex);
			}

			return new InstanceProxy(type, args);
		}

		/// <summary>
		/// Gets a proxy for an instance of a generic type constructed using the specified arguments.
		/// </summary>
		/// <typeparam name="T">Type of the proxied class.</typeparam>
		/// <param name="args">The arguments to pass to the constructor.</param>
		public static InstanceProxy For<T>(params object[] args)
		{
			return new InstanceProxy(typeof(T), args);
		}

		/// <summary>
		/// Invokes the specified named method using the supplied arguments.
		/// </summary>
		/// <param name="methodName">The name of the method.</param>
		/// <param name="args">The arguments or <c>null</c> if the method does not take any arguments.</param>
		/// <returns>
		/// The value returned by the method.
		/// </returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="methodName"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">Method does not exist on type.</exception>
		public object Invoke(string methodName, params object[] args)
		{
			if (methodName == null)
				throw new ArgumentNullException("methodName", "methodName is null.");

			MethodInfo method = m_type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (method == null)
				throw new ArgumentException(string.Format("Method '{0}' does not exist on type '{1}'.", methodName, m_type.FullName), "methodName");

			return method.Invoke(m_instance, args);
		}

		/// <summary>
		/// Gets the value of the specified named property.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <returns>
		/// The value returned by the property.
		/// </returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">Property does not exist on type.</exception>
		public object GetValue(string propertyName)
		{
			return GetValue(propertyName, null);
		}

		/// <summary>
		/// Gets the value of the specified named property.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="index">The index arguments or <c>null</c> if the property does not take any index arguments.</param>
		/// <returns>
		/// The value returned by the property.
		/// </returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">Property does not exist on type.</exception>
		public object GetValue(string propertyName, params object[] index)
		{
			if (propertyName == null)
				throw new ArgumentNullException("propertyName", "propertyName is null.");

			PropertyInfo property = m_type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (property == null)
				throw new ArgumentException(string.Format("Property '{0}' does not exist on type '{1}'.", propertyName, m_type.FullName), "propertyName");

			return property.GetValue(m_instance, index);
		}

		/// <summary>
		/// Sets the value of the specified named property.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">Property does not exist on type.</exception>
		public void SetValue(string propertyName)
		{
			SetValue(propertyName, null);
		}

		/// <summary>
		/// Sets the value of the specified named property.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="value">The value.</param>
		/// <param name="index">The index arguments or <c>null</c> if the property does not take any index arguments.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">Property does not exist on type.</exception>
		public void SetValue(string propertyName, object value, params object[] index)
		{
			if (propertyName == null)
				throw new ArgumentNullException("propertyName", "propertyName is null.");

			PropertyInfo property = m_type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (property == null)
				throw new ArgumentException(string.Format("Property '{0}' does not exist on type '{1}'.", propertyName, m_type.FullName), "propertyName");

			property.SetValue(m_instance, value, index);
		}

		/// <summary>
		/// Gets the type of the proxied object.
		/// </summary>
		public Type Type { get { return m_type; } }

		/// <summary>
		/// Gets the non-proxied instance of the proxied object.
		/// </summary>
		public object Instance { get { return m_instance; } }

		readonly Type m_type;
		readonly object m_instance;
	}
}
