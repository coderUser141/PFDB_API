using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.ParsingUtility
{
	public static class ParsingUtilityClass
	{
		public static IEnumerable<SearchTargets> GetSearchTargetsForWeapon(WeaponType weaponType)
		{
			List<SearchTargets> searchTargets = new List<SearchTargets>();

			switch(weaponType)
			{
				case WeaponType.Grenade:
					{
						searchTargets.AddRange(Enum.GetValues<SearchTargets>().ToList().Where(x => x >= SearchTargets.BlastRadius && x <= SearchTargets.StoredCapacity));
						break;
					}
				case WeaponType.Melee:
					{
						searchTargets.AddRange(Enum.GetValues<SearchTargets>().ToList().Where(x => x >= SearchTargets.FrontStabDamage && x <= SearchTargets.Walkspeed));
						searchTargets.AddRange(Enum.GetValues<SearchTargets>().ToList().Where(x => x >= SearchTargets.HeadMultiplier && x <= SearchTargets.LimbMultiplier));

						break;
					}
				default:
					{
						searchTargets.AddRange(Enum.GetValues<SearchTargets>().ToList().Where(x => x >= SearchTargets.Rank && x <= SearchTargets.FireModes));
						break;
					}
			}

			return searchTargets;
		}
	}
}
