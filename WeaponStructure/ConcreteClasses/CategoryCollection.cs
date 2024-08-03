using PFDB.WeaponUtility;

namespace PFDB.WeaponStructure
{
	public class CategoryCollection : List<ICategory>, ICategoryCollection
	{

		public IEnumerable<ICategory> Categories => this;

		public bool CollectionNeedsRevision
		{
			get
			{
				return this.Any(x => x.NeedsRevision);
			}
		}

		public CategoryCollection()
		{
			
		}

		public CategoryCollection(IEnumerable<ICategory> categories)
		{
			this.AddRange(categories);
		}

		public new void Add(ICategory category)
		{
			//todo: add checks
			base.Add(category);
		}

		public new void AddRange(IEnumerable<ICategory> categories)
		{
			//todo: add checks
			base.AddRange(categories);
		}

		public void Add(ICategoryCollection categoryCollection)
		{
			//todo: add checks
			base.AddRange(categoryCollection.Categories);
		}
	}
}
