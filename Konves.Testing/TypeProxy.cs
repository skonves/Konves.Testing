using System;
using System.Reflection;

namespace Konves.Testing
{
	/// <summary>
	/// Provides functionality for unit-testing non-public classes and classes with non-public members.
	/// </summary>
	public sealed class TypeProxy
	{
		private TypeProxy(Type type)
		{
			m_type = type;
		}

		/// <summary>
		/// Creates a proxy for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		public static TypeProxy For(Type type)
		{
			return new TypeProxy(type);
		}

		/// <summary>
		/// Creates a proxy for the specified assembly and class name.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <param name="className">Name of the class.</param>
		/// <exception cref="System.ArgumentException">Type could not be found in assembly.</exception>
		/// <exception cref="System.ArgumentException">Assembly could not be found.</exception>
		public static TypeProxy For(string assemblyName, string className)
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

			return new TypeProxy(type);
		}

		/// <summary>
		/// Creates a proxy for a generic type.
		/// </summary>
		/// <typeparam name="T">Type of the proxied class.</typeparam>
		public static TypeProxy For<T>()
		{
			return new TypeProxy(typeof(T));
		}

		/// <summary>
		/// Invokes the specified named static method using the supplied arguments.
		/// </summary>
		/// <param name="methodName">The name of the static method.</param>
		/// <param name="args">The arguments or <c>null</c> if the static method does not take any arguments.</param>
		/// <returns>
		/// The value returned by the static method.
		/// </returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="methodName"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">Static method does not exist on type.</exception>
		public object Invoke(string methodName, params object[] args)
		{
			if (methodName == null)
				throw new ArgumentNullException("methodName", "methodName is null.");

			MethodInfo method = m_type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (method == null)
				throw new ArgumentException(string.Format("Static method '{0}' does not exist on type '{1}'.", methodName, m_type.FullName), "methodName");

			return method.Invoke(null, args);
		}

		/// <summary>
		/// Gets the value of the specified named static property.
		/// </summary>
		/// <param name="propertyName">The name of the static property.</param>
		/// <returns>
		/// The value returned by the property.
		/// </returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">Static property does not exist on type.</exception>
		public object GetValue(string propertyName)
		{
			return GetValue(propertyName, null);
		}

		/// <summary>
		/// Gets the value of the specified named static property.
		/// </summary>
		/// <param name="propertyName">The name of the static property.</param>
		/// <param name="index">The index arguments or <c>null</c> if the static property does not take any index arguments.</param>
		/// <returns>
		/// The value returned by the static property.
		/// </returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">Static property does not exist on type.</exception>
		public object GetValue(string propertyName, params object[] index)
		{
			if (propertyName == null)
				throw new ArgumentNullException("propertyName", "propertyName is null.");

			PropertyInfo property = m_type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (property == null)
				throw new ArgumentException(string.Format("Static property '{0}' does not exist on type '{1}'.", propertyName, m_type.FullName), "propertyName");

			return property.GetValue(null, index);
		}

		/// <summary>
		/// Sets the value of the specified named static property.
		/// </summary>
		/// <param name="propertyName">The name of the static property.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">Static property does not exist on type.</exception>
		public void SetValue(string propertyName, object value)
		{
			SetValue(propertyName, value, null);
		}

		/// <summary>
		/// Sets the value of the specified named static property.
		/// </summary>
		/// <param name="propertyName">The name of the static property.</param>
		/// <param name="value">The value.</param>
		/// <param name="index">The index arguments or <c>null</c> if the static property does not take any index arguments.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">Static property does not exist on type.</exception>
		public void SetValue(string propertyName, object value, params object[] index)
		{
			if (propertyName == null)
				throw new ArgumentNullException("propertyName", "propertyName is null.");

			PropertyInfo property = m_type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (property == null)
				throw new ArgumentException(string.Format("Static property '{0}' does not exist on type '{1}'.", propertyName, m_type.FullName), "propertyName");

			property.SetValue(null, value, index);
		}

		/// <summary>
		/// Gets the proxied type.
		/// </summary>
		public Type Type { get { return m_type; } }

		readonly Type m_type;
	}
}
