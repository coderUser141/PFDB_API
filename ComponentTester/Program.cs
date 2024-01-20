
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using WeaponClasses;
using System.Data.SQLite;

public class ComponentTester
{
	public static void Main(string[] args)
	{

        Console.WriteLine(PINVOKE.experienceToRank(PINVOKE.rankToExperienceDefault(213) + PINVOKE.rankToExperienceDefault(173)));

        const string workingDirectory = "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API";
		ImmutableList<string> buildoutputdirectories = (new List<string>() { "all build options v5", "all build options v6", "all build options v7" } ).ToImmutableList();
		List<List<string>> listofstr = new List<List<string>>()
		{
			new List<string>()
			{
				"ak12","an94","as-val","scar-l","aug-a1","m16a4","g36","m16a1","m16a3","type-20","aug-a2","k2","famas-f1",
		"ak47","aug-a3","l85a2","hk416","ak74","akm","ak103","tar-21","type-88","m231","c7a2","stg-44","g11k2"
			},new List<string>()
			{
				"mp5k","ump45","g36c","mp7","mac10","p90","colt-mars","mp5","colt-smg-633","l2a3","mp5sd","mp10","m3a1",
		"mp510","uzi","aug-a3-para-xs","k7","aks74u","ppsh-41","fal-para-shorty","kriss-vector","pp-19-bizon","mp40",
		"x95-smg","tommy-gun","rama-1130","bwc9-a","five-0"
			},new List<string>()
			{
				"colt-lmg","m60","aug-hbar","mg36","rpk12","l86-lsw","rpk","hk21e","hamr-iar","rpk74","mg3kws","mgv-176","stoner-96","mg42"
			}, new List<string>()
			{
				"intervention","model-700","dragunov-svu","aws","bfg-50","awm","trg-42","mosin-nagant","dragunov-svds",
		"m1903","k14","hecate-ii","ft300","m107","steyr-scout","wa2000","ntw-20"
			}, new List<string>()
			{
				"m14","beowulf-ecr","scar-h","ak12br","g3a3","ag-3","hk417","henry-45-70","fal-5000"
			}, new List<string>()
			{
				"m4a1","g36k","m4","l22","scar-pdw","aku12","groza-1","ots-126","ak12c","honey-badger","k1a","sr-3m","groza-4",
		"mc51","fal-5063-para","1858-carbine","ak105","jury","kac-srr","gyrojet-carbine","c8a2","x95r","hk51b",
		"can-cannon"
			}, new List<string>()
			{
				"mk11","sks","sl-8","vss-vintorez","msg90","m21","beowulf-tcr","sa58-spr","scar-ssr"
			}, new List<string>()
			{
				"ksg-12","model-870","dbv12","ks-23m","saiga-12","stevens-db","e-gun","aa-12","spas-12","dt11","usas-12"
			}, new List<string>()
			{
				"g17","m9","m1911a1","desert-eagle-l5","g21","g23","m45a1","g40","kg-99","g50","five-seven","zip-22","gi-m1",
		"hardballer","izhevsk-pb","makarov-pm","gb-22","desert-eagle-xix","automag-iii","gyrojet-mark-i","gsp",
		"grizzly","m2011","alien","af2011-a1"
			}, new List<string>()
			{
				"g18c","93r","pp-2000","tec-9","micro-uzi","skorpion-vz61","asmi","mp1911","arm-pistol"
			}, new List<string>()
			{
				"mp412-rex","mateba-6","1858-new-army","redhawk-44","judge","executioner"
			}, new List<string>()
			{
				"super-shorty","sfg-50","m79-thumper","advanced-coilgun","sawed-off","saiga-12u","obrez","sass-308"
			}, new List<string>()
			{
				"m67-frag", "mk-2-frag", "m24-stick", "m26-frag", "m560-mini", "v40-mini", "roly-hg"
			}, new List<string>()
			{
				"dynamite-3", "dynamite", "rgd-5-he","semtex", "pb-grenade", "bundle-charge"
			},
			new List<string>()
			{
				"t-13-impact","rgn-udzs","rgo-udzs"
			}, new List<string>()
			{
				"cleaver", "tanzanite-blade", "war-fan", "nata-hatchet",
		"mekleth", "karambit", "krampus-kukri", "trench-knife", "knife", "tactical-spatula", "hunting-knife",
		"tanto", "entrencher", "ritual-sickle", "kama", "key", "ice-pick", "machete", "tomahawk", "pocket-knife",
		"havoc-blade", "cutter", "jason", "bridal-brandisher", "darkheart", "streiter", "balisong", "kommando",
		"linked-sword", "classic-knife", "jkey"
			}, new List<string>()
			{
				"zircon-trident", "nordic-war-axe", "world-buster", "noobslayer", "hattori", "chosen-one", "reaper",
		"zero-cutter", "naginata", "training-bayonet", "longsword", "fire-axe", "harvester", "zweihander"
			}, new List<string>()
			{
				"asp-baton", "toy-gun", "maglite-club", "crowbar", "mjolnir", "keyboard", "fumelee", "candy-cane",
		"bare-fists", "tanzanite-pick", "stick-grenade", "bloxy", "holiday-tea", "trench-mace", "clonker",
		"nightstick", "stun-gun", "uchiwa", "fixer", "brass-knuckle", "cricket-bat", "frying-pan", "arm-cannon",
		"starlis-funpost"
			}, new List<string>()
			{
				"sledge-hammer", "hockey-stick", "sweeper", "baseball-bat",
				"paddle", "cursed-shinai", "banjo", "stylis-brush", "kanabo", "stopper", "the-axe", "void-staff",
		"morning-star", "present", "crane"
			}
		};
		List<List<string>> listofstr2 = new List<List<string>>()
		{
			new List<string>()
			{
				"ak12","an94","as val","scar-l","aug a1","m16a4","g36","m16a1","m16a3","type 20","aug a2","k2","famas f1",
				"ak47","aug a3","l85a2","hk416","ak74","akm","ak103","tar-21","type 88","m231","c7a2","stg-44","g11k2"
			},new List<string>()
			{
				"mp5k","ump45","g36c","mp7","mac10","p90","colt mars","mp5","colt smg 633","l2a3","mp5sd","mp10","m3a1",
				"mp5/10","uzi","aug a3 para xs","k7","aks74u","ppsh-41","fal para shorty","kriss vector","pp-19 bizon","mp40",
				"x95 smg","tommy gun","rama 1130"//,"bwc9 a","five-0"
			},new List<string>()
			{
				"colt lmg","m60","aug hbar","mg36","rpk12","l86 lsw","rpk","hk21e","hamr iar","rpk74","mg3kws"//,"mgv-176","stoner 96","mg42"
			}, new List<string>()
			{
				"intervention","model 700","dragunov svu","aws","bfg 50","awm","trg-42","mosin nagant","dragunov svds",
				"m1903","k14","hecate ii","ft300","m107","steyr scout","wa2000","ntw-20"
			}, new List<string>()
			{
				"m14","beowulf ecr","scar-h","ak12br","g3a3","ag-3","hk417","henry 45-70","fal 50.00"
			}, new List<string>()
			{
				"m4a1","g36k","m4","l22","scar-pdw","aku12","groza-1","ots-126","ak12c","honey badger","k1a","sr-3m","groza-4",
				"mc51","fal 50.63 para","1858 carbine","ak105","jury","kac srr","gyrojet carbine","c8a2","x95r","hk51b",
				"can cannon"
			}, new List<string>()
			{
				"mk11","sks","sl-8","vss vintorez","msg90","m21","beowulf tcr","sa58 spr","scar ssr"
			}, new List<string>()
			{
				"ksg-12","model 870","dbv12","ks-23m","saiga-12","stevens db","e-gun","aa-12","spas-12","dt11","usas-12"
			}, new List<string>()
			{
				"g17","m9","m1911a1","desert eagle l5","g21","g23","m45a1","g40","kg-99","g50","five seven","zip 22","gi m1",
				"hardballer","izhevsk pb","makarov pm","gb-22","desert eagle xix","automag iii","gyrojet mark i","gsp",
				"grizzly","m2011","alien","af2011-a1"
			}, new List<string>()
			{
				"g18c","93r","pp-2000","tec-9","micro uzi","skorpion vz.61","asmi","mp1911","arm pistol"
			}, new List<string>()
			{
				"mp412 rex","mateba 6","1858 new army","redhawk 44","judge","executioner"
			}, new List<string>()
			{
				"super shorty","sfg 50","m79 thumper","advanced coilgun","sawed off","saiga-12u","obrez","sass 308"
			}, new List<string>()
			{
				"m67 frag", "mk-2 frag", "m24-stick", "m26-frag", "m560-mini", "v40-mini", "roly-hg"
			}, new List<string>()
			{
				"dynamite-3", "dynamite", "rgd-5 he","semtex", "pb grenade", "bundle charge"
			},
			new List<string>()
			{
				"t-13 impact","rgn udzs","rgo udzs"
			}, new List<string>()
			{
				"cleaver", "tanzanite blade", "war fan", "nata hatchet",
				"mekleth", "karambit", "krampus kukri", "trench knife", "knife", "tactical spatula", "hunting knife",
				"tanto", "entrencher", "ritual sickle", "kama", "key", "ice pick", "machete", "tomahawk", "pocket knife",
				"havoc blade", "cutter", "jason", "bridal brandisher", "darkheart", "streiter", "balisong", "kommando",
				"linked sword", "classic knife", "jade key"
			}, new List<string>()
			{
				"zircon trident", "nordic war axe", "world buster", "noobslayer", "hattori", "chosen one", "reaper",
				"zero cutter", "naginata", "training bayonet", "longsword", "fire axe", "harvester", "zweihander"
			}, new List<string>()
			{
				"asp baton", "toy gun", "maglite club", "crowbar", "mjolnir", "keyboard", "fumelee", "candy cane",
				"bare fists", "tanzanite pick", "stick grenade", "bloxy", "holiday tea", "trench mace", "clonker",
				"nightstick", "stun gun", "uchiwa", "fixer", "brass knuckle", "cricket bat", "frying pan", "arm cannon",
				"starlis funpost"
			}, new List<string>()
            {
                "sledge hammer", "hockey stick", "sweeper", "baseball bat",
                "paddle", "cursed shinai", "banjo", "stylis brush", "kanabo", "stopper", "the axe", "void staff",
				"morning star", "present", "crane"
			}
		};
		/*
		foreach(List<string> t in listofstr2)
		{
			foreach(string s in t)
			{
				using(SQLiteConnection conn = new SQLiteConnection(@$"Data Source={workingDirectory}\weapon_database.db;Version=3;FailIfMissing=True;"))
				{
					using(SQLiteCommand command = conn.CreateCommand())
					{
						conn.Open();
						command.CommandText = "INSERT INTO version800 (weapon_name,category,category_number) VALUES ('" + s + "','',0); ";
						using(SQLiteDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								//do nothing because it's inserting
							}
						}
						conn.Close();
					}
				}
			}
		}
		/*
		foreach (string t in buildoutputdirectories)
		{
			for(int g = 0; g < listofstr.Count; ++g)
			{
				for (int h = 0; h < listofstr[g].Count; ++h) {
					try {
						File.Copy($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}1.png.txt", $"{workingDirectory}\\{t}\\{g}_{h}_1.txt");
						File.Copy($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}2.png.txt", $"{workingDirectory}\\{t}\\{g}_{h}_2.txt");
                        File.Move($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}1.png.txt", $"{workingDirectory}\\{t}\\{listofstr2[g][h]}_1.txt");
                        File.Move($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}2.png.txt", $"{workingDirectory}\\{t}\\{listofstr2[g][h]}_2.txt");

                    }
					catch
					{
						try
						{
							File.Copy($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}.png.txt", $"{workingDirectory}\\{t}\\{g}_{h}_0.txt");
                            File.Move($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}.png.txt", $"{workingDirectory}\\{t}\\{listofstr2[g][h]}_0.txt");
                        }
						catch
						{
							Console.WriteLine("file not found");
						}
					}
				}
			}
		}*/
			// { "all build outputs v5", "all build outputs v5" };


		string r = "888.999   55.9";
		Console.WriteLine("gg");
		List<string> list = new List<string>(FileParse.findAllStatisticsInFile("C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\text.txt",true));
		Match match = Proofread.regex(list[5], @"\d+\.\d+");
		MatchCollection matches = Proofread.regexes(r, @"\d+\.\d+");
		if (match.Success)
		{
			Console.WriteLine($"{match.Value} was extracted from {list[5]}");
		}
		else
		{
			Console.WriteLine("g");
		}
		foreach(Match g in matches)
		{
			Console.WriteLine($"{g.Value} was extracted from {r})");
		}
	}
}
