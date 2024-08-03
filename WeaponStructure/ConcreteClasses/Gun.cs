using PFDB.ConversionUtility;
using PFDB.WeaponUtility;
using System.Reflection.Metadata.Ecma335;

namespace PFDB.WeaponStructure
{
	public sealed class Gun : Weapon {

		private IConversionCollection _conversionCollection;

		public IConversionCollection ConversionCollection => _conversionCollection;


		public Gun(string name, IConversionCollection conversionCollection, Categories category) : base(name, conversionCollection,category)
		{
			_conversionCollection = conversionCollection;
			//Console.WriteLine(_conversionCollection.Conversions.First().StatisticCollection.Statistics.First().WeaponID.Version.VersionNumber);
		}

		public Gun(string name, string? description, IConversionCollection conversionCollection, Categories category) : base(name, description, conversionCollection, category)
		{
			_conversionCollection = conversionCollection;
		}
	}
}
