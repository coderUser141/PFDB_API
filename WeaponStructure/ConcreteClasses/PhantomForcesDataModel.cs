﻿using PFDB.PythonFactoryUtility;
using PFDB.WeaponUtility;
using PFDB.PythonFactory;
using PFDB.PythonExecutionUtility;
using PFDB.Conversion;
using PFDB.ConversionUtility;
using PFDB.Proofreading;
using PFDB.Logging;
using PFDB.PythonExecution;
using PFDB.SQLite;

namespace PFDB.WeaponStructure
{
	public class PhantomForcesDataModel : IPhantomForcesDataModel  
	{

		private IClassCollection _classCollection;
		public IClassCollection ClassCollection { get { return _classCollection; } }

		public static IWeaponCollection GetWeaponCollection(IPythonExecutionFactoryOutput factory)
		{
			if (factory.IsDefaultConversion) //default conversion, which means we have only ONE conversion per weapon
			{
				IWeaponCollection weaponCollection = new WeaponCollection();
				foreach (IPythonExecutor pythonExecutor in factory.PythonExecutors)
				{
					IWeapon weapon; 

					switch (pythonExecutor.Input.WeaponType)
					{
						case WeaponType.Primary:
						case WeaponType.Secondary:
							{
								IConversionCollection conversionCollection = new ConversionCollection(new DefaultConversion(pythonExecutor));
								weapon = new Gun("placeholder name", conversionCollection, pythonExecutor.Input.WeaponID.Category);
								break;
							}
						case WeaponType.Grenade:
							{
								weapon = new Grenade("placeholder name", new DefaultConversion(pythonExecutor), pythonExecutor.Input.WeaponID.Category);
								
								break;
							}
						case WeaponType.Melee:
							{
								weapon = new Melee("placeholder name", new DefaultConversion(pythonExecutor), pythonExecutor.Input.WeaponID.Category);
								break;
							}
						default:
							{
								PFDBLogger.LogFatal("Invalid weapon type.");
								throw new Exception("Invalid weapon type.");
							}
					}
					weaponCollection.Add(weapon);
				}
				return weaponCollection;
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public static IWeaponCollection GetWeaponCollection(PhantomForcesVersion phantomForcesVersion)
		{
			IWeaponCollection weaponCollection = new WeaponCollection();
			//if (WeaponTable.WeaponIDCache[phantomForcesVersion].Any() == false)WeaponTable.GetWeaponIdentifications(phantomForcesVersion);

			foreach(Categories category in Enum.GetValues(typeof(Categories)))
			{
				for(int j = 0; j < 40; ++j)
				{
					IDefaultConversion defaultConversion;
					var keyvalue = WeaponTable.WeaponIDCache.ToDictionary();
					var tuple = keyvalue[phantomForcesVersion].First(x => x.weaponNumber == j && x.categoryNumber == (int)category);
					try
					{
						string filename = @$"{Directory.GetCurrentDirectory()}\{PythonExecutor.OutputFolderName}\{phantomForcesVersion.VersionNumber}\{(int)category}_{j}.png.pfdb";


						defaultConversion = new DefaultConversion(filename, tuple.weaponID);
					}
					catch(Exception e)
					{
						PFDBLogger.LogWarning(e.Message);
						continue;
					}
					IWeapon weapon;
					switch(WeaponUtilityClass.GetWeaponType(category))
					{
						case WeaponType.Primary:
						case WeaponType.Secondary:
							{
								IConversionCollection conversionCollection = new ConversionCollection(defaultConversion);
								weapon = new Gun(WeaponTable., conversionCollection, category);
								break;
							}
						case WeaponType.Grenade:
							{
								weapon = new Grenade("placeholder name", defaultConversion, category);
								break;
							}
						case WeaponType.Melee:
							{
								weapon = new Melee("placeholder name", defaultConversion, category);
								break;
							}
						default:
							{
								PFDBLogger.LogFatal("Invalid weapon type.");
								throw new Exception("Invalid weapon type.");
							}
					}
					weaponCollection.Add(weapon);
				}
			}
			return weaponCollection;
		}

		public PhantomForcesDataModel(IWeaponCollection weaponCollection)
		{
			//ICategory


			List<ICategory> categories = new List<ICategory>();
			foreach(Categories t in Enum.GetValues(typeof(Categories)))
			{
				categories.Add(
					new Category(
						t, new WeaponCollection(
							weaponCollection.Weapons.Where(x => x.Category == t)
						)
					)
				);
			}

			ICategoryCollection categoryCollection = new CategoryCollection();
			categoryCollection.AddRange(categories);


			//ICategory assaultRifles = new Category()
			List<IClass> classes = new List<IClass>();
			foreach(Classes c in Enum.GetValues(typeof(Classes)))
			{
				ICategoryCollection temp = new CategoryCollection();
				switch (c)
				{
					case Classes.Assault:
						{
							temp.AddRange(
								categoryCollection.Categories.Where(x => new List<Categories>() { Categories.AssaultRifles, Categories.BattleRifles, Categories.Carbines, Categories.Shotguns }.Contains(x.CategoryType))
							);
							break;
						}
					case Classes.Scout:
						{
							temp.AddRange(
								categoryCollection.Categories.Where(x => new List<Categories>() { Categories.PersonalDefenseWeapons, Categories.DesignatedMarksmanRifles, Categories.Carbines, Categories.Shotguns}.Contains(x.CategoryType))
								);
							break;
						}
					case Classes.Support:
						{
							temp.AddRange(
								categoryCollection.Categories.Where(x => new List<Categories>() { Categories.LightMachineGuns, Categories.BattleRifles, Categories.Carbines, Categories.Shotguns}.Contains(x.CategoryType))
								);
							break;
						}
					case Classes.Recon:
						{
							temp.AddRange(
								categoryCollection.Categories.Where(x => new List<Categories>() { Categories.SniperRifles, Categories.BattleRifles, Categories.Carbines, Categories.DesignatedMarksmanRifles}.Contains(x.CategoryType))
								);
							break;
						}
				}
				temp.AddRange(categoryCollection.Categories.Where(x => x.CategoryType >= Categories.Pistols && x.CategoryType <= Categories.TwoHandBluntMelees));
				classes.Add(new Class(c, temp));
			}
			_classCollection = new ClassCollection(classes);
			return;
		}
	}
}