using PFDB.Conversion;
using PFDB.ConversionUtility;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;

namespace PFDB.WeaponStructure
{
	public sealed class Grenade: Weapon
	{
		public Grenade(string name, IDefaultConversion defaultConversion, Categories category) : base(name, new ConversionCollection(defaultConversion), category)
		{
			
		}
		public Grenade(string name, string? description, IDefaultConversion defaultConversion, Categories category) : base(name, description, new ConversionCollection(defaultConversion), category)
		{
			
		}
	}

	

	
}
