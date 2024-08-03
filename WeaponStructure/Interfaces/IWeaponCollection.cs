
namespace PFDB.WeaponUtility
{
	public interface IWeaponCollection : IPFDBCollection
	{
		IEnumerable<IWeapon> Weapons { get; }

		void Add(IWeapon weapon);
		void Add(IWeaponCollection weapons);
		void AddRange(IEnumerable<IWeapon> weapons);
	}
}
