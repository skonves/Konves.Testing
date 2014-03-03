/*
 *		Productively licensed under the GSD License!
 * 
 *			http://blog.stevekonves.com/2011/11/gsd/
 * 
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Konves.Testing
{
	/// <summary>
	/// Verifies conditions in unit tests using true/false propositions about proxied and non-proxied objects.
	/// </summary>
	public static class ProxyAssert
	{
		/// <summary>
		/// Verifies that an object proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see> is equal to another object by comparing all public and non-public properties. The assertion fails if the objects are not equal.
		/// </summary>
		/// <param name="expected">The first object to compare.  This is the obejct the unit test expects and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
		/// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException"><paramref name="expected"/> is not equal to <paramref name="actual"/>.</exception>
		public static void AreEqual(InstanceProxy expected, object actual)
		{
			AreEqual(expected, actual, null);
		}

		/// <summary>
		/// Verifies that an object proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see> is equal to another object by comparing the specified public and non-public properties. The assertion fails if the objects are not equal.
		/// </summary>
		/// <param name="expected">The first object to compare.  This is the obejct the unit test expects and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
		/// <param name="properties">The properties to compare.  If the array is <c>null</c> or does not contain any elements, all public and non-public properties are compared.</param>
		/// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException"><paramref name="expected"/> is not equal to <paramref name="actual"/>.</exception>
		public static void AreEqual(InstanceProxy expected, object actual, params string[] properties)
		{
			if (properties == null || properties.Length == 0)
				properties = actual.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(p => p.Name).ToArray();

			foreach (string name in properties)
			{
				object a = expected.GetValue(name, null);
				object b = actual.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(actual,null);

				if (NullSafeEquals(a, b))
					throw new AssertFailedException(string.Format("ProxyAssert.AreEqual failed at property '{0}'. Expected:<{1}>. Actual:<{2}>.", name, a, b));
			}
		}

		/// <summary>
		/// Verifies that two <see cref="Konves.Testing.InstanceProxy">InstanceProxies</see> are equal by comparing all public and non-public properties. The assertion fails if the objects are not equal.
		/// </summary>
		/// <param name="expected">The first object to compare.  This is the obejct the unit test expects and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <param name="actual">The second object to compare. This is the object the unit test produced and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException"><paramref name="expected"/> is not equal to <paramref name="actual"/>.</exception>
		public static void AreEqual(InstanceProxy expected, InstanceProxy actual)
		{
			AreEqual(expected, actual, null);
		}

		/// <summary>
		/// Verifies that two <see cref="Konves.Testing.InstanceProxy">InstanceProxies</see> are equal by comparing the specified public and non-public properties. The assertion fails if the objects are not equal.
		/// </summary>
		/// <param name="expected">The first object to compare.  This is the obejct the unit test expects and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <param name="actual">The second object to compare. This is the object the unit test produced and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <param name="properties">The properties to compare.  If the array is <c>null</c> or does not contain any elements, all public and non-public properties are compared.</param>
		/// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException"><paramref name="expected"/> is not equal to <paramref name="actual"/>.</exception>
		public static void AreEqual(InstanceProxy expected, InstanceProxy actual, params string[] properties)
		{
			if (properties == null || properties.Length == 0)
				properties = actual.Type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(p => p.Name).ToArray();

			foreach (string name in properties)
			{
				object a = expected.GetValue(name, null);
				object b = actual.GetValue(name, null);

				if (!NullSafeEquals(a, b))
					throw new AssertFailedException(string.Format("ProxyAssert.AreEqual failed at property '{0}'. Expected:<{1}>. Actual:<{2}>.", name, a, b));
			}
		}

		/// <summary>
		/// Verifies that an object proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see> is not equal to another object by comparing all public and non-public properties. The assertion fails if the objects are equal.
		/// </summary>
		/// <param name="notExpected">The first object to compare.  This is the obejct the unit test expects not to match <paramref name="actual"/> and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
		/// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException"><paramref name="notExpected"/> is equal to <paramref name="actual"/>.</exception>
		public static void AreNotEqual(InstanceProxy notExpected, object actual)
		{
			AreNotEqual(notExpected, actual, null);
		}

		/// <summary>
		/// Verifies that an object proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see> is not equal to another object by comparing the specified public and non-public properties. The assertion fails if the objects are equal.
		/// </summary>
		/// <param name="notExpected">The expected object value proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
		/// <param name="properties">The properties to compare.  If the array is <c>null</c> or does not contain any elements, all public and non-public properties are compared.</param>
		/// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException"><paramref name="notExpected"/> is equal to <paramref name="actual"/>.</exception>
		public static void AreNotEqual(InstanceProxy notExpected, object actual, params string[] properties)
		{
			if (properties == null || properties.Length == 0)
				properties = actual.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(p => p.Name).ToArray();

			foreach (string name in properties)
			{
				object a = notExpected.GetValue(name, null);
				object b = actual.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(actual, null);

				if (!NullSafeEquals(a, b))
					throw new AssertFailedException(string.Format("ProxyAssert.AreNotEqual failed at property '{0}'. Expected any value except:<{1}>. Actual:<{2}>.", name, a, b));
			}
		}

		/// <summary>
		/// Verifies that two <see cref="Konves.Testing.InstanceProxy">InstanceProxies</see> are not equal by comparing all public and non-public properties. The assertion fails if the objects are equal.
		/// </summary>
		/// <param name="expected">The first object to compare.  This is the obejct the unit test expects not to match <paramref name="actual"/> and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <param name="actual">The second object to compare. This is the object the unit test produced and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException"><paramref name="notExpected"/> is equal to <paramref name="actual"/>.</exception>
		public static void AreNotEqual(InstanceProxy expected, InstanceProxy actual)
		{
			AreNotEqual(expected, actual, null);
		}

		/// <summary>
		/// Verifies that two <see cref="Konves.Testing.InstanceProxy">InstanceProxies</see> are not equal by comparing the specified public and non-public properties. The assertion fails if the objects are equal.
		/// </summary>
		/// <param name="expected">The first object to compare.  This is the obejct the unit test expects not to match <paramref name="actual"/> and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <param name="actual">The second object to compare. This is the object the unit test produced and is proxied by an <see cref="Konves.Testing.InstanceProxy">InstanceProxy</see>.</param>
		/// <param name="properties">The properties to compare.  If the array is <c>null</c> or does not contain any elements, all public and non-public properties are compared.</param>
		/// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException"><paramref name="notExpected"/> is equal to <paramref name="actual"/>.</exception>
		public static void AreNotEqual(InstanceProxy expected, InstanceProxy actual, params string[] properties)
		{
			if (properties == null || properties.Length == 0)
				properties = actual.Type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(p => p.Name).ToArray();

			foreach (string name in properties)
			{
				object a = expected.GetValue(name, null);
				object b = actual.GetValue(name, null);

				if (NullSafeEquals(a, b))
					throw new AssertFailedException(string.Format("ProxyAssert.AreNotEqual failed at property '{0}'. Expected any value except:<{1}>. Actual:<{2}>.", name, a, b));
			}
		}

		private static bool NullSafeEquals(object a, object b)
		{
			return a == b || (a != null && b != null && a.Equals(b));
		}
	}
}
