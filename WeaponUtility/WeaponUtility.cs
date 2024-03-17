using System;
using System.Text.RegularExpressions;

namespace PFDB
{
    namespace WeaponUtility
    {
        /// <summary>
        /// Specifies the weapon type.
        /// </summary>
        public enum WeaponType
		{
			/// <summary>
			/// Primary Gun.
			/// </summary>
			Primary = 1,
			/// <summary>
			/// Secondary Gun.
			/// </summary>
			Secondary,
			/// <summary>
			/// Grenade.
			/// </summary>
			Grenade,
			/// <summary>
			/// Melee.
			/// </summary>
			Melee
		}

		/// <summary>
		/// Specifies the version of Phantom Forces.
		/// </summary>
		public sealed class PhantomForcesVersion
		{
			private string _versionString;

			/// <summary>
			/// User-friendly version number.
			/// </summary>
			public string VersionString
			{
				get { return _versionString; }
			}

			/// <summary>
			/// Integer version of <see cref="VersionString"/>.
			/// </summary>
			public int VersionNumber
			{
				get
				{
					string tempstr = _versionString;
					while (tempstr.Contains('.'))
					{
						try
						{
							tempstr = tempstr.Remove(tempstr.LastIndexOf('.'), 1);
						}
						catch (ArgumentOutOfRangeException)
						{
							break;
						}
					}
					return Convert.ToInt32(tempstr);
				}
			}

			/// <summary>
			/// Equality operator.
			/// </summary>
			/// <param name="first">First object to compare.</param>
			/// <param name="second">Second object to compare.</param>
			/// <returns>True if the versions are equal, false if unequal.</returns>
			public static bool operator==(PhantomForcesVersion first, PhantomForcesVersion second)
			{
				return first.VersionNumber == second.VersionNumber;
			}

			/// <summary>
			/// Non-equality operator.
			/// </summary>
			/// <param name="first">First object to compare.</param>
			/// <param name="second">Second object to compare.</param>
			/// <returns>True if the versions are unequal, false if equal.</returns>
			public static bool operator !=(PhantomForcesVersion first, PhantomForcesVersion second)
			{
				return first.VersionNumber != second.VersionNumber;
			}

			/// <summary>
			/// Greater-than operator.
			/// </summary>
			/// <param name="first">First object to compare.</param>
			/// <param name="second">Second object to compare.</param>
			/// <returns>True if the first object is greater than the second object.</returns>
			public static bool operator >(PhantomForcesVersion first, PhantomForcesVersion second)
			{
				return first.VersionNumber > second.VersionNumber;
			}

			/// <summary>
			/// Less-than operator.
			/// </summary>
			/// <param name="first">First object to compare.</param>
			/// <param name="second">Second object to compare.</param>
			/// <returns>True if the first object is less than the second object.</returns>
			public static bool operator <(PhantomForcesVersion first, PhantomForcesVersion second)
			{
				return first.VersionNumber < second.VersionNumber;
			}

			/// <summary>
			/// Greater-than-or-equal-to operator.
			/// </summary>
			/// <param name="first">First object to compare.</param>
			/// <param name="second">Second object to compare.</param>
			/// <returns>True if the first object is greater than or equal to the second object.</returns>
			public static bool operator >=(PhantomForcesVersion first, PhantomForcesVersion second)
			{
				return first.VersionNumber >= second.VersionNumber;
			}

			/// <summary>
			/// Less-than-or-equal-to operator.
			/// </summary>
			/// <param name="first">First object to compare.</param>
			/// <param name="second">Second object to compare.</param>
			/// <returns>True if the first object is less than or equal to the second object.</returns>
			public static bool operator <=(PhantomForcesVersion first, PhantomForcesVersion second)
			{
				return first.VersionNumber <= second.VersionNumber;
			}


			/// <summary>
			/// Constructor using major, minor and revision versions of Phantom Forces.
			/// For example, in the version "8.0.1":
			/// <list type="bullet">
			/// <item>8 = Major version</item>
			/// <item>0 = Minor version</item>
			/// <item>1 = Revision</item>
			/// </list>
			/// </summary>
			/// <param name="majorVersion"></param>
			/// <param name="minorVersion"></param>
			/// <param name="revision"></param>
			public PhantomForcesVersion(int majorVersion, int minorVersion, int revision)
			{
				_versionString = $"{majorVersion}.{minorVersion}.{revision}";
			}

			/// <summary>
			/// Constructor using the in-game version number.
			/// </summary>
			/// <param name="versionString">The in-game version number. For example: "8.0.1g" or "8.0.1" will both work for this parameter.</param>
			public PhantomForcesVersion(string versionString)
			{
                //alternate: ^(\d+\.?\d+\.?\d+)(\D+)
				//matches "1010" as well, but can also match "10.10" :C

                Regex regexpart1 = new Regex(@"^(\d+\.\d+\.\d+)(\D+)$");
				Match matches = regexpart1.Match(versionString);
				if(matches.Groups.Count > 1)
				{
					_versionString = matches.Groups[1].Value;
				}
				else
				{
					_versionString = versionString;
				}
			}

			/// <summary>
			/// Determines whether the specified object equals the current object.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			/// <exception cref="NotImplementedException"></exception>
			public override bool Equals(object? obj)
			{
				if(obj != null)
				{
					if(obj is PhantomForcesVersion objc)
					{
						if(objc.VersionNumber == this.VersionNumber)
						{
							return true;
						}
					}
				}
				return false;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>The default hash return.</returns>
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}

		/// <summary>
		/// A dedicated utility class for weapon types in Phantom Forces.
		/// </summary>
		public static class WeaponUtilityClass
		{
			/// <summary>
			/// Converts an integer to the corresponding <see cref="WeaponType"/> category.
			/// </summary>
			/// <param name="categoryNumber">The category number. This value cannot exceed 19.</param>
			/// <returns></returns>
			/// <exception cref="ArgumentException"></exception>
			public static WeaponType GetWeaponType(int categoryNumber)
			{
				if (categoryNumber > 19) throw new ArgumentException("The parameter cannot exceed 19.", nameof(categoryNumber));
				WeaponType weaponType = 0;
				if (categoryNumber < 8)
				{
					weaponType = WeaponType.Primary;
				}
				else if (categoryNumber > 8 && categoryNumber < 12)
				{
					weaponType = WeaponType.Secondary;
				}
				else if (categoryNumber > 11 && categoryNumber < 15)
				{
					weaponType = WeaponType.Grenade;
				}
				else
				{
					weaponType = WeaponType.Melee;
				}
				return weaponType;
			}
		}
	}
}
