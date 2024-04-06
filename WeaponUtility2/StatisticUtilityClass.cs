using PFDB.WeaponUtility;

namespace PFDB
{
	namespace StatisticUtility
	{
		public static class StatisticUtilityClass
		{
			public static IEnumerable<StatisticOptions> GetSearchTargetsForWeapon(WeaponType weaponType)
			{
				List<StatisticOptions> searchTargets = new List<StatisticOptions>();

				switch (weaponType)
				{
					case WeaponType.Grenade:
						{
							searchTargets.AddRange(Enum.GetValues<StatisticOptions>().ToList().Where(x => x >= StatisticOptions.BlastRadius && x <= StatisticOptions.StoredCapacity));
							break;
						}
					case WeaponType.Melee:
						{
							searchTargets.AddRange(Enum.GetValues<StatisticOptions>().ToList().Where(x => x >= StatisticOptions.FrontStabDamage && x <= StatisticOptions.Walkspeed));
							searchTargets.AddRange(Enum.GetValues<StatisticOptions>().ToList().Where(x => x >= StatisticOptions.HeadMultiplier && x <= StatisticOptions.LimbMultiplier));

							break;
						}
					default:
						{
							searchTargets.AddRange(Enum.GetValues<StatisticOptions>().ToList().Where(x => x >= StatisticOptions.Rank && x <= StatisticOptions.FireModes));
							break;
						}
				}

				return searchTargets;
			}
		}
	}
}
