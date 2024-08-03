namespace PFDB.WeaponUtility
{
	public interface ICategory
	{
		bool NeedsRevision { get; }
		Categories CategoryType { get; }
		IWeaponCollection WeaponCollection { get; }
	}
}
