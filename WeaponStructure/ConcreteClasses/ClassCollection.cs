using PFDB.WeaponUtility;
using System.Collections.Generic;

namespace PFDB.WeaponStructure
{
	public class ClassCollection : List<IClass>, IClassCollection
	{
		public bool CollectionNeedsRevision
		{
			get
			{
				return this.Any(x => x.NeedsRevision);
			}
		}

		public IEnumerable<IClass> Classes => this;

		public ClassCollection()
		{
			
		}

		public ClassCollection(IEnumerable<IClass> classes)
		{
			this.AddRange(classes);
		}

		public new void Add(IClass classItem)
		{
			//todo: add checks
			base.Add(classItem);
		}

		public new void AddRange(IEnumerable<IClass> classes)
		{
			//todo: add checks
			base.AddRange(classes);
		}

		public void Add(IClassCollection classes)
		{
			//todo: add checks
			base.AddRange(classes.Classes);
		}
	}
}
