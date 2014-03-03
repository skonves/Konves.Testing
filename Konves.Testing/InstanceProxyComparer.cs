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

using System.Collections;
using System.Linq;
using System.Reflection;

namespace Konves.Testing
{
	/// <summary>
	/// Provides functionality to compare proxied and non-proxied objects.
	/// </summary>
	public class InstanceProxyComparer : IComparer
	{
		public InstanceProxyComparer()
			: this(null) { }

		/// <summary>
		/// Initializes a new <see cref="InstanceProxyComparer"/> which compares to proxied or non-proxies objects based on the specified properties.
		/// </summary>
		/// <param name="properties">The properties to compare.</param>
		public InstanceProxyComparer(params string[] properties)
		{
			m_properties = properties;
		}

		int IComparer.Compare(object x, object y)
		{
			{
				string[] properties = m_properties;

				if (properties == null || properties.Length == 0)
					properties = (y as InstanceProxy).Type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(p => p.Name).ToArray();

				if (x is InstanceProxy && y is InstanceProxy)
				{
					foreach (string name in properties)
					{
						object a = (x as InstanceProxy).GetValue(name, null);
						object b = (y as InstanceProxy).GetValue(name, null);

						if (!(a == b || (a != null && b != null && a.Equals(b))))
							return 1;
					}
					return 0;
				}
				else if (x is InstanceProxy)
				{
					foreach (string name in properties)
					{
						object a = (x as InstanceProxy).GetValue(name, null);
						object b = y.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(y, null);

						if (!(a == b || (a != null && b != null && a.Equals(b))))
							return 1;
					}

					return 0;
				}
				else if (y is InstanceProxy)
				{
					return (this as IComparer).Compare(y, x);
				}
				else
				{
					return x.Equals(y) ? 0 : 1;
				}
			}
		}

		readonly string[] m_properties;
	}
}
