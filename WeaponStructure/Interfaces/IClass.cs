namespace PFDB.WeaponUtility
{
	public interface IClass
	{
		bool NeedsRevision { get; }
		Classes ClassType { get; }
		ICategoryCollection CategoryCollection { get; }
	}
}
