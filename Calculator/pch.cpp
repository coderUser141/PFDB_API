// pch.cpp: source file corresponding to the pre-compiled header

#include "pch.h"
#include "sqlite3.h"
#define DLLEXPORT __declspec(dllexport)



// When you are using pre-compiled headers, this source file is necessary for compilation to succeed.


namespace player_statistics_calculator {

	extern "C" DLLEXPORT unsigned long long test() {
		std::cout << "your mom\n"; return 6; 
	}

	/// <summary>
	/// Calculates the amount of credits earned at a given rank.
	/// </summary>
	/// <param name="rank">Desired rank to convert.</param>
	/// <returns>The credits obtained after hitting the given rank.</returns>
	extern "C" DLLEXPORT unsigned long long rankToCredits(unsigned long long rank) {
		//cannot be negative, cannot be bigger than unsigned long max
		if (((UINT64_MAX - 201) / 5) < rank || rank < 0) return UINT64_MAX; //avoid int overflow
		if (rank == 0)return 0;
		return 200 + 5 * rank;
	}

	/// <summary>
	/// Calculates the rank that achieves the given credits.
	/// </summary>
	/// <param name="credits">The credits to convert.</param>
	/// <returns>The rank that achieves the given rank.</returns>
	extern "C" DLLEXPORT unsigned long long creditsToRank(unsigned long long credits) {
		//must be a multiple of 5, cannot be negative, cannot be bigger than ulong max
		if (credits < 0 || credits % 5 != 0) return UINT64_MAX;
		return (credits - (unsigned long long)200) / (unsigned long long)5;
	}

	/// <summary>
	/// Calculates the sum of all of the credits earned between a range between a given start and end rank.
	/// </summary>
	/// <param name="startRank">The start rank.</param>
	/// <param name="endRank">The end rank.</param>
	/// <returns>Returns the sum of all the credits earned in the range defined by the start and end ranks.</returns>
	extern "C" DLLEXPORT unsigned long long rankToCreditsSummation(unsigned long long startRank, unsigned long long endRank) {
		//endRank must be greater than startRank, neither can be negative
		if (endRank < startRank) return UINT64_MAX;
		if (endRank == startRank)return 0;
		//to check if the intermediate calculation will exceed the uint64 limit
		if (startRank * (startRank - 1) > UINT64_MAX - (startRank * 2))return UINT64_MAX;
		if (endRank * (endRank - 1) > UINT64_MAX - (endRank * 2))return UINT64_MAX;
		//see https://www.desmos.com/calculator/4ywrmdkv6p
		unsigned long long startSum = 200 * startRank + 5 * ((startRank * startRank + startRank) / 2);
		unsigned long long endSum = 200 * endRank + 5 * ((endRank * endRank + endRank) / 2);

		return endSum - startSum;
	}

	/// <summary>
	/// Calculates the rank required to achieve the target experience.
	/// </summary>
	/// <param name="targetXP">Desired experience to convert.</param>
	/// <returns>The rank that will have the given experience.</returns>
	extern "C" DLLEXPORT unsigned long long experienceToRank(unsigned long long targetXP) {
		unsigned long long counter{ 0 };
		unsigned long long i{ 0 };
		//not really necessary, but should theoretically help with calculation time
		if (targetXP > 500500000) { i = 1000; counter = 499500000; }
		if (targetXP > 5000050000000) { i = 100000; counter = 4999950000000; }
		for (; i < targetXP; ++i) {
			//rank 1 has 1000xp, rank 2 has rank 1 + 2000 = 3000, rank 3 has rank 2 + rank 1 + 3000 = 6000
			//counter actually has the total xp for each rank here
			counter += i * 1000;
			//checks if the counter is just less than the target xp, but adding one more rank would cause it to go over the target xp
			if (counter + ((i + 1) * 1000) > targetXP && counter <= targetXP) {
				return i;
				break;
			}
		}
		return UINT64_MAX;
	}

	/// <summary>
	/// Calculates the experience gained from a startRank to a targetRank
	/// </summary>
	/// <param name="startRank">The rank to start from.</param>
	/// <param name="startRankProgress">The progress within the rank. Needs to be less than (startRank + 1)*1000.</param>
	/// <param name="targetRank">The rank to end at.</param>
	/// <returns>Returns the experience gained or needed to go from startRank to targetRank.</returns>
	extern "C" DLLEXPORT unsigned long long rankToExperienceGeneral(unsigned long long startRank, unsigned long long startRankProgress, unsigned long long targetRank) {
		unsigned long long requirement{ 0 };

		unsigned long long startSum = 1000 * ((startRank * startRank + startRank) / 2);
		unsigned long long endSum = 1000 * ((targetRank * targetRank + targetRank) / 2);
		//for (unsigned long long i = 0; i <= targetRank; i++)requirement += i * (ULONG)1000;
		//for (unsigned long long j = 0; j <= startRank; j++)requirement -= j * (ULONG)1000;
		//requirement -= startRankProgress;
		return endSum - startSum - startRankProgress;
	}

	/// <summary>
	/// Calculatest the experience gained from rank 0 to targetRank.
	/// </summary>
	/// <param name="targetRank">The target rank.</param>
	/// <returns>Returns the experience gained or needed to go from rank 0 to targetRank.</returns>
	extern "C" DLLEXPORT unsigned long long rankToExperienceDefault(unsigned long long targetRank) {
		return rankToExperienceGeneral(0, 0, targetRank);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="startRank"></param>
	/// <param name="endRank"></param>
	/// <returns></returns>
	extern "C" DLLEXPORT unsigned long long rankToRankXPRequirement(unsigned long long startRank, unsigned long long endRank) {
		unsigned long long startXP = rankToExperienceDefault(startRank);
		unsigned long long endXP = rankToExperienceDefault(endRank);
		return endXP - startXP;
	}

	extern "C" DLLEXPORT unsigned long long creditsForGunRankRequirement(unsigned long long startRank, unsigned long long targetRank) {
		unsigned long long delta = targetRank - startRank + 1;
		if (delta > (UINT64_MAX - 700) / 140)return UINT64_MAX;
		return 140 * delta + 700;
	}

	extern "C" DLLEXPORT unsigned creditsForAttachment(unsigned attachmentKillUnlock) {
		double x{ (double)attachmentKillUnlock };
		return (unsigned)(0.7 * x + 140) + 1;
	}

}



namespace weapon_statistics_calculator {

	extern "C" DLLEXPORT void hollowPoint() {

	}

	extern "C" DLLEXPORT void armorPiercing() {

	}

}

namespace sqlite_accessor {

	int callback(void* data, int argcount, char** argv, char** column_name) {
		std::cout << (char*)data << '\n';
		for (int i{ 0 }; i < argcount; ++i) {
			std::cout << column_name[i] << ": " << argv[i] << '\n';
		}
		std::cout << '\n';
		return 0;
	}

	void bla() {
		sqlite3* db;
		char* errMsg = static_cast<char*>(malloc(sizeof(char))); //i've been trying for the past hour trying to make this a unique_ptr. oh well, onto the to do list you go...
		*errMsg = 'y';
		int rc{ 0 };

		rc = sqlite3_open("attachments.db", &db);

		if (rc != SQLITE_OK) {
			//error
			std::cout << ":(\n";
			std::cout << "Error: " << errMsg << '\n';
			if (errMsg != nullptr)sqlite3_free(errMsg);
			sqlite3_close(db);
			return;
		}
		std::cout << "yay\n";
		char data[]{ "Callback function called." };
		const char* sql{ "SELECT * FROM group_attachments;" };

		sqlite3_exec(db, sql, callback, (void*)data, &errMsg);

		if (errMsg != nullptr)sqlite3_free(errMsg);
		sqlite3_close(db);
	}

	extern "C" DLLEXPORT void write() {
		bla();
	}

	extern "C" DLLEXPORT BSTR returnString(){
		return SysAllocString(L"hiiii");
	}

}
//how to pass vector from c++ to c#
//https://stackoverflow.com/questions/31417688/passing-a-vector-array-from-unmanaged-c-to-c-sharp

/*
attachment list:

Optics:
H&K Sight			20
Full Ring Sight		50
Half Ring Sight		90
Backup Sight		100
Steyr Sight			210
Double Open Sight	250
Izhmash Sight		315
Super Slim Sight	350
Diopter Sight		410
BUIS Sight			480
KEL-TEC Sight		540
Kalashnikov Sight	570
KAC Sight			600
1200M Sight			700
H&K Export Sight	785
Herstal Sight		820
Bundeswehr Sight	870
IWI Sight			1015
Quick-Release Sight	1105
MBUS Sight			1150
AAC Flip Up Sight	1275
Dual Aperture Sight	1400

Z-Point				10
EOTech XPS2			45
Delta Sight			60
MARS				70
EOTech 552			85
Mini Sight			120
Comp Aimpoint		130
PKA-S				225
Reflex Sight		285
Kobra Sight			330
Coyote Sight		380
Microdot Mini		490
Pilad 3				500
Kobra EKP Sight		625
Acro P-1 Sight		675
Barska Electro		750
Eotech M40			755
Kousaku Sight		777
Microdot SRS		870
OKP-7				900
UH-1 Sight			920
DDHB Reflex			1200
DCL 120				1320
Kousaku OLED		1554

C79					145
PK-A				165
M145				185
Malcolm 3X Scope	215
PSO-1 Scope			250
TA44 ACOG			295
ACOG Scope			430
VCOG 6X Scope		455
TA33 ACOG			650
Hensoldt 3X Sight	850
Hensoldt Z24		1300
Swarovski Scope		1355
TA11 ACOG			1750
PU-1 Scope			2000
FF 3X NV			2170
PSO-1M2				2250
TA01 ACOG			2500
Electra 5X			2600
Reflector Scope		2700
VCOG 8X Scope		3455
Susat Scope			4242
Global Offensive Scope	4750

OEG					1977
Maglite				1979
AMT-Terminator		2029
Handmade Sight		3000
Plague Insight		3825
Animu Sight			4500
Furro Sight			5000
Hand Sight			5250
Anti Sight			5600

Flash Hider			100
Compensator			120
Muzzle Brake		175
T-Brake				225
X-Ring				275
Halbek Device		765
Loudener			835
Muzzle Booster		1200

Suppressor			30
R2 Suppressor		50
ARS Suppressor		150
PBS-1 Suppressor	205
PBS-4 Suppressor	245
Muffler				600
Oil Filter			990

Flashlight			0
Red Laser			75
Green Laser			165
Blue Laser			255
Yellow Laser		450
Tri Laser			1070

Vertical Grip		45
Angled Grip			115
Potato Grip			180
Skeleton Grip		205
Folding Grip		265
Stubby Grip			355
Pistol Grip			580
//Sideways Grip		715
//Hera CQR Grip		870
Chainsaw Grip		950

Flashlight			0
Red Laser			20
Green Laser			110
Blue Laser			200
Yellow Laser		450
Tri Laser			1015
Ballistics Tracker	1500

Canted Iron Sight	310
Canted Delta Sight	405
Canted ACOG Scope	740
Canted Animu Sight	3700
Canted Furro Sight	3855

Armor Piercing		1000
Hollow Point		2000
Tracerless			2950

attachments grouped by weapons:

Ammo:

Flechette: KSG-12, Model 870, DBV12, Saiga-12, AA-12, USAS-12, Jury, Saiga-12U, Judge, Super Shorty, Sawed Off, Stevens DB, SPAS-12, M79 Thumper
Birdshot: KSG-12, Model 870, DBV12, Saiga-12, AA-12, USAS-12, Jury, Saiga-12U, Judge, Super Shorty, Sawed Off, Stevens DB, SPAS-12
Slugs: KSG-12, Model 870, DBV12, Saiga-12, AA-12, USAS-12, Saiga-12U, Judge, DT11, Super Shorty, Sawed Off, Stevens DB, SPAS-12
223 Remington: SCAR-L, AUG A1, M16A4, G36, M16A1, M16A3, Type 20, AUG A2, K2, FAMAS F1, AUG A3, L85A2, HK416, TAR-21, M231, C7A2, M4A1, G36K, M4, L22, SCAR PDW, K1A, C8A2, G36C, Arm Pistol, SL-8, Saiga-12, Saiga-12U, Colt LMG, AUG HBAR, MG36, L86 LSW, HAMR IAR, Stoner 96, ChainSAW, 
Armor Piercing NOT: Can Cannon, Gyrojet Carbine, KAC SRR, Jury, KSG-12, Model 870, DBV12, Saiga-12, AA-12, USAS-12, Saiga-12U, Judge, 1858 Carbine, Gyrojet Mark I, PP-2000, Obrez, Henry 45-70, NTW-20, WA2000, Steyr Scout, M107, SFG 50, FT300, Hecate II, K14, Dragunov SVDS, Mosin Nagant, TRG-42, AWM, BFG 50, AWS, Dragunov SVU, Model 700, Intervention, OTs-126
Hollow Point NOT: Gyrojet Carbine, Gyrojet Mark I, Obrez, KAC SRR, Jury, KSG-12, Model 870, DBV12, Saiga-12, AA-12, USAS-12, Saiga-12U, Judge, 1858 Carbine, Gyrojet Mark I, Henry 45-70, NTW-20, WA2000, Steyr Scout, M107, SFG 50, FT300, Hecate II, K14, M1903, Dragunov SVDS, Mosin Nagant, TRG-42, AWM, BFG 50, Dragunov SVU, Model 700, Intervention
Silent: G3A3, AG-3, SASS 308, AWS, Steyr Scout, K14, WA2000, Model 700, MG3KWS, FAL Para Shorty, FAL 50.00, FAL 50.63 Para, MC51, SA58 SPR, SCAR-H, M14, M21, MSG90, SCAR SSR, HK417, HK21E,  (+ all 7.62x51mm guns)
Super Armor Piercing: PP-2000, AK12, AN-94, AK74, Type 88, OTs-126, X95R, RPK12, RPK74, AK105, RPK12, RPK74
Extended Magazine: M17, M45A1, KG-99, TEC-9, AS VAL, Type 20, SR-3M, BWC9 A, TEC-9, Saiga-12U
Tracerless NOT: Gyrojet Carbine, Gyrojet Mark I, Super Shorty, KSG-12, DBV12, Model 870, Saiga-12, Saiga-12U, Stevens DB, E-Gun, AA-12, SPAS-12, DT11, USAS-12
Plus P: MP5K, UMP45, MAC 10, MP5, Colt SMG 633, L2A3, MP5SD, MP10, M3A1, MP5/10, UZI, AUG A3 PARA XS, K7, Kriss Vector, PP-19 Bizon, MP40, X95 SMG, Tommy Gun, RAMA 1130, BWC9 A, G17, M9, M1911A1, G21, M45A1, G40, KG-99, Hardballer, Izhevsk PB, Makarov PM, M2011, Alien, AF2011-A1, 93R, TEC-9, Micro UZI, ASMI, MP1911, Henry 45-70
Minishell: KSG-12, Model 870, SPAS-12, Super Shorty, GB-22
Minislugs: KSG-12, Model 870, SPAS-12, Super Shorty
.45 Super: UMP45, MAC10, M3A1, Kriss Vector, Tommy Gun, G21, G17, M1911A1, M45A1, AF2011-A1, MP1911
.45 ACP: FAL Para Shorty, FAL 50.63 Para, FAL 50.00, Five-0, G3A3, Micro UZI, SA58 SPR, L2A3, UZI, Five-0
9X18MM: Skorpion VZ.61, PP-19 Bizon
9X19MM: MP1911, Saiga-12U, Saiga-12, SCAR-L, AUG A1, SCAR PDW, AKU12, MAC10, AKS74U, Kriss Vector, Five-0
7.62X39MM: RPK12, AK12BR, AN-94, AUG A2, K2, Type 88, SCAR-H, AK12BR, RPK12, K1A, Saiga-12, Saiga-12U, AKS74U, VSS Vintorez
.410 Bore: DBV12, Obrez, Mosin Nagant, Henry 45-70, Beowulf ECR, Jury
9.6X53MM: Saiga-12, Saiga-12U, Dragunov SVU, Dragunov SVDS
.44 Special: Mateba 6, Redhawk 44, Desert Eagle L5
Rat shot: BFG 50, SFG 50, Skorpion VZ.61, Hecate II, M107, Zip 22
6.5 Grendel: M16A4, M16A3, C7A2, M4A1, M4, C8A2, Colt LMG
.25-45 Sharps: M16A4, M16A3, M4A1, M4
.300 BLK: AUG A3, X95R
.300 BLK.: G36, G36C, G36K, MG36, HK416
.366 Shot: AKM, SKS
5.56X45MM: AK103, STG-44, Beowulf ECR, Beowulf TCR, FAL 50.00, FAL 50.63 Para, FAL Para Shorty, SA58 SPR, Groza-1, AK105, X95R, K7, Steyr Scout, 
Rubber Pellets: Jury, Super Shorty, KSG-12, Model 870, Saiga-12, Saiga-12U, Stevens DB, AA-12, SPAS-12, Judge, Sawed Off
.308 WIN: Saiga-12, Saiga-12U, AWM
.308 WIN.: K14, MSG90
.40S&W: UMP45, MP5/10, AUG A3 PARA XS
.22LR: UZI, Kriss Vector, AUG HBAR
Depleted Uranium: MG3KWS, MK11
6.5MM: M14, M21, SCAR SSR
.338 Norma: AWM, TRG-42
10MM Auto: MP5K, GB-22, Tommy Gun
M903 SLAP, .17 Incinerator, .416 Barrett: Hecate II, BFG 50, SFG 50, M107
30RD Mag: P90, Type 88, Tommy Gun, G40
.357 Magnum: Desert Eagle L5, Henry 45-70
Dart: M1911A1, M45A1
.50 AE: GB-22, Automag III


Other:

Collapsible Stock: M16A4, M16A3, AK47, AK74, AKM, Beowulf ECR, G3A3, MK11, VSS Vintorez, Colt LMG
Full Stock: Dragunov SVDS, AS VAL, K2, HK416, Type 88, C7A2, MAC10, Colt MARS, Colt SMG 633, UZI, AKS74U, FAL Para Shorty, PP-19 Bizon, M4A1, M4, SCAR PDW, AKU12, SR-3M, FAL 50.63 Para, C8A2, HK51B, AG-3, HK417, SKS, Beowulf TCR, MG3KWS, KS-23M, KG-99, TEC-9, Micro UZI, Skorpion VZ.61, M79 Thumper, Saiga-12U
Remove Stock: BFG 50, SFG 50, Model 700, K14, M1903, Intervention, TRG-42, Dragunov SVDS, Mosin Nagant, FT300, Hecate II, SCAR-L, M16A4, M16A3, Type 20, AK47, HK416, AK74, AK103, AKM, Type 88, C7A2, MAC10, Colt MARS, Colt SMG 633, MP5/10, AKS74U, PPSH-41, Kriss Vector, PP-19 Bizon, Tommy Gun, Five-0, M4A1, M4, SCAR PDW, AK105, C8A2, Beowulf ECR, SCAR-H, Henry 45-70, HCAR, MK11, Beowulf TCR, SCAR SSR, Colt LMG, M60, HAMR IAR, M1918A2, Model 870, KS-23M, Saiga-12, Stevens DB, DT11, Intervention, ASMI, MP1911
Retract Stock: Dragunov SVDS, AK12, AN-94, AS VAL, SCAR-L, M16A4, G36, M16A3, Type 20, K2, AK47, AK74, AK103, AKM, Type 88, C7A2, G36C, MP7, Colt MARS, Colt SMG 633, M3A1, MP5/10, K7, AKS74U, Kriss Vector, PP-19 Bizon, MP40, G36K, M4A1, M4, SCAR PDW, AKU12, AK12C, K1A, SR-3M, FAL 50.63 Para, AK105, C8A2, Beowulf ECR, SCAR-H, AK12BR, G3A3, AG-3, HK417, MK11, SKS, VSS Vintorez, Beowulf TCR, SCAR SSR, Colt LMG, MG36, RPK12, HAMR IAR, MG3KWS, DBV12, Saiga-12, PP-2000, ASMI
Extend Stock: Type 20, UZI, FAL Para Shorty, MC51, HK51B
Burst Grouping: MP5K, MP5, MP5SD, Colt SMG 633, M16A3, MP5/10
Semi-Auto Conv.: AA-12, BWC9 A, USAS-12
Marksman Kit: Intervention, Model 700, AWS, AWM, TRG-42, Mosin Nagant, M1903, K14, Hecate II, Steyr Scout, Hecate II, NTW-20, FT300, Obrez, Can Cannon, Henry 45-70
Speedloader: Jury, KAC SRR, MP412 REX, Mateba 6, Redhawk 44, Judge, Executioner
Binary Trigger: Beowulf TCR, SL-8
Extended Tube: Super Shorty, Model 870
Wood Furniture: G3A3, FT300
Shoulder Stock: M1911A1, M45A1, GI M1, Grizzly, AF2011-A1
Extended Magazine: Model 700, Mosin Nagant, M107, Obrez, MP7, M9, Five seveN, Izhevsk PB, Makarov PM, Arm Pistol
12RD Magazine: Izhevsk PB, Makarov PM
Heavy Bolt: MG3KWS, MG42

Barrel:

Carbine Barrel: BFG 50, AK12, SCAR-L, AUG A1, M16A3, Type 20, AUG A2, K2, FAMAS F1, AK47, AUG A3, L85A2, HK416, AK74, AKM, AK103, Type 88, M231, C7A2, MAC10, M4A1, M4, AKU12, AK12C, AK105, C8A2, SCAR-H, Beowulf TCR, Colt LMG, RPK12, L86 LSW, HAMR IAR, M1918A2
Squad Barrel: SCAR-L, Type 20, K2, FAMAS F1, HK416, AK103, Type 88, G36C, Colt MARS, AKS74U, Tommy Gun, G36K, L22, SCAR PDW, AKU12, AK12C, Honey Badger, AK103, HK51B, HK417, Beowulf TCR, HAMR IAR
CQB Barrel: Intervention, M107, Hecate II, 
Heavy Barrel: BFG 50, TRG-42, Steyr Scout, FT300, Hecate II, Model 700, G50, GI M1, Grizzly
Light Barrel: BFG 50, TRG-42, K14, Steyr Scout, FT300, Hecate II, Model 700, Dragunov SVDS, M1903, Mosin Nagant, SASS 308
Front Post: Mosin Nagant, Obrez,
Obrez Barrel: NTW-20, PPSH-41
Osprey Suppressor: MP5K, UMP45, MP7, MAC10, Colt SMG 633, L2A3, MP10, M3A1, UZI, MP5/10, AUG A3 Para XS, PPSH-41, Kriss Vector, PP-19 Bizon, MP40, X95 SMG, Tommy Gun, RAMA 1130, BWC9 A, OTs-126, G17, M9, M1911A1, M17, G21, G23, M45A1, G40, KG-99, G50, Five seveN, Zip 22, GI M1, Hardballer, Makarov PM, GB-22, GSP, Grizzly, Alien, AF2011-A1, G18C, 93R, PP-2000, TEC-9, Micro UZI, Skorpion VZ.61, ASMI, MP1911, SASS 308
Long Barrel: MP5K, UMP45, MAC10, P90, Colt SMG 633, L2A3, MP5SD, MP10, MP5/10, UZI, AUG A3 Para XS, PPSH-41, Kriss Vector, RAMA 1130, MC51, 1858 Carbine, G17, M9, G21, G23, KG-99, Makarov PM, Gyrojet Mark I, Grizzly, G18C, TEC-9, Skorpion VZ.61
Short Barrel: L2A3, BWC9 A, OTs-126, 1858 Carbine, HK51B, HK417, SCAR SSR, MGV-176, G40, Hardballer, Gyrojet Mark I, TEC-9, MP1911
Assault Barrel: HCAR, MK11, SKS
Sporting Barrel: KS-23M, AA-12
Integral Suppressor: MC51, Type 20, DT11, FT300
Snubnose: MP412 REX, Mateba 6, 1858 New Army, Redhawk 44
Cowboy Barrel: MP412 REX, Mateba 6, 1858 New Army, Redhawk 44
Cut Down Barrel: M79 Thumper, GB-22

Underbarrel:

Romanian Grip: AN-94, G36, AK47, L85A2, AK74, AKM, AK103, Type 88, G11K2, G36C, MP5, Colt SMG 633, MP5/10, AKS74U, G36K, FAL 50.63 Para, AK105, Jury, KAC SRR, M14, Beowulf ECR, FAL 50.00, SKS, Dragunov SVU, MSG90, M21, MG36, L86 LSW, RPK, RPK74, Model 870, KS-23M, Saiga-12, AA-12, SPAS-12, Model 700, Mosin Nagant, Dragunov SVDS, M1903, WA2000
Folded Grip: AUG A1, MP7, SR-3M
Sideways Grip: AK12, AN-94, AS VAL, SCAR-L, AUG A1, M16A4, G36, M16A1, M16A3, Type 20, AUG A2, K2, FAMAS F1, AK47, AUG A3, L85A2, HK416, AK74, AKM, AK103, TAR-21, Type 88, M231, C7A2, STG-44, G11K2, MP5K, UMP45, G36C, MP7, MAC10, P90, Colt MARS, MP5, Colt SMG 633, L2A3, MP5SD, MP10, M3A1, MP5/10, UZI, AUG A3 Para XS, K7, AKS74U, PPSH-41, FAL Para Shorty, Kriss Vector, PP-19 Bizon, MP40, X95 SMG, RAMA 1130, BWC9 A, Five-0, M4A1, G36K, M4, L22, SCAR PDW, AKU12, Groza-1, OTs-126, AK12C, Honey Badger, K1A, SR-3M, Groza-4, MC51, FAL 50.63 Para, 1858 Carbine, AK105, Jury, KAC SRR, Gyrojet Carbine, C8A2, X95R, HK51B, Can Cannon, M14, Beowulf ECR, SCAR-H, AK12BR, G3A3, AG-3, HK417, Henry 45-70, FAL 50.00, HCAR, MK11, SKS, Dragunov SVU, VSS Vintorez, MSG90, M21, Beowulf TCR, SA58 SPR, SCAR SSR, Colt LMG, M60, AUG HBAR, MG36, RPK12, L86 LSW, RPK, HK21E, HAMR IAR, RPK74, MG3KWS, M1918A2, MGV-176, Stoner 96, KSG 12, Model 870, DBV12, KS-23M, Saiga-12, Stevens DB, E-Gun, AA-12, SPAS-12, DT11, USAS-12, Intervention, Model 700, AWS, BFG 50, AWM. TRG-42, Mosin Nagant, Dragunov SVDS, M1903, K14, Hecate II, FT300, M107, Steyr Scout, WA2000, NTW-20, MG42
Hera CQR Grip: AK12, AN-94, SCAR-L, AUG A1, M16A4, G36, M16A3, Type 20, AUG A2, K2, FAMAS F1, AK47, AUG A3, HK416, AK74, AKM, AK103, Type 88, M231, C7A2, STG-44, G11K2, G36C, MP7, MAC10, P90, MP5, MP5SD, MP5/10, AUG A3 Para XS, K7, AKS74U, PP-19 Bizon, RAMA 1130, Five-0, M4A1, G36K, M4, AKU12, Groza-1, OTs-126, AK12C, Honey Badger, K1A, SR-3M, Groza-4, FAL 50.63 Para, AK105, Gyrojet Carbine, C8A2, Can Cannon, Beowulf ECR, SCAR-H, AK12BR, G3A3, AG-3, HK417, FAL 50.00, MK11, SKS, Dragunov SVU, MSG90, Beowulf TCR, SA58 SPR, SCAR SSR, M60, AUG HBAR, MG36, RPK12, RPK, HAMR IAR, RPK74, MG3KWS, KSG 12, Saiga-12, E-Gun, AA-12, USAS-12, AWS, AWM, Dragunov SVDS, K14, WA2000, MG42

Optics:

Anti-Aircraft Irons: M60, MG3KWS, MG42
Carry Handle Sight: M16A4, HK416, C7A2, M4A1, Honey Badger, C8A2, M14, Beowulf ECR, MK11, M21, Beowulf TCR
PM II: SCAR-L, Type 20, SCAR PDW, Can Cannon, SCAR-H, SCAR SSR, HAMR IAR, AWS, AWM
Hensoldt Dual: G36, G36C, G36K, SL-8, MG36
Leupold M8-2X: 1858 Carbine, Jury, KAC SRR, Dragunov SVU, Intervention, Model 700, AWS, BFG 50, AWM, TRG-42, Mosin Nagant, Dragunov SVDS, K14, FT300, Hecate II, Steyr Scout, WA2000, NTW-20, Desert Eagle L5, Desert Eagle XIX, MP412 REX, 1858 New Army, Redhawk 44, Judge, Executioner, SFG 50
Leupold M8-6X: 1858 Carbine, Jury, KAC SRR, Gyrojet Carbine, Steyr Scout, GB-22, Desert Eagle XIX, Gyrojet Mark I, MP412 REX, 1858 New Army, Redhawk 44, Judge, Executioner, SFG 50
Malcolm 3X Scope: Jury, 1858 Carbine, KAC SRR, M14, Henry 45-70, AK12, AN-94, AS VAL, SCAR-L, G36, Type 20, AK47, AK74, AKM, AK103, Type 88, G36K, SCAR PDW, AKU12, AK12C, SR-3M, FAL 50.63 Para, AK105, HK51B, MK11, SL-8, VSS Vintorez, MSG90, M21, SA58 SPR, SCAR SSR, Colt LMG, AUG HBAR, MG36, RPK12, RPK, HK21E, HAMR IAR, RPK74, MGV-176, Stoner 96, DBV12, Saiga-12, Intervention, Model 700, AWS, AWM, M1903, K14, Hecate II, TRG-42, Mosin Nagant, FT300, Steyr Scout
Malcolm 6X Scope: Jury, 1858 Carbine, KAC SRR, M14, Beowulf ECR, Henry 45-70, SCAR-L, Type 20, SCAR SSR, HAMR IAR
Olympian Target Sight: FT300, Alien 

Shotgun Chokes:

Improved Choke: KSG 12, Model 870, DBV12, Saiga-12, Stevens DB, AA-12, SPAS-12, DT11, USAS-12, Judge, Super Shorty, Sawed Off, Saiga-12U
Duckbill Choke: KSG 12, Model 870, DBV12, Saiga-12, Stevens DB, AA-12, SPAS-12, DT11, Super Shorty, Sawed Off, Saiga-12U
Modified Choke: KSG 12, Model 870, Saiga-12, Stevens DB, SPAS-12, DT11, USAS-12, Judge, Super Shorty, Sawed Off, Saiga-12U
Paradox Choke: KSG 12, DBV12, KS-23M, Stevens DB, Stevens DB, AA-12, DT11, USAS-12, Super Shorty, Sawed Off
Diverter: Super Shorty, Model 870, Stevens DB, DT11, Super Shorty, Sawed Off
Full Choke: Model 870, Stevens DB, DT11, Super Shorty, Sawed Off




Optic: Extended Stock, Full Stock, Remove Stock: SPAS-12

Light Barrel: MG42
.358 Win.: SASS 308
Boom Stock: Sawed Off
Sponge, Buckshot, Double Slugs, .410 Beehive: M79 Thumper
Unfolded Leaf Sight: M79 Thumper
.45 Long Colt: Redhawk 44
KAC Stock: Redhawk 44
.38 Special: MP412 REX
Rubber Bullets: Skorpion VZ.61
Extended Stock: Skorpion VZ.61
Remove Limiter: Skorpion VZ.61
Strike-3: Micro UZI
Magazine Stock: PP-2000
Folded Stock: G18C
Double Dart: AF2011-A1
.32 S&W, GSP Rat Shot: GSP
Target Weight: GSP
.45 WINMAG: Automag III
Police Barrel, Police Stock, .440 Cor-Bon: Desert Eagle XIX
Sporting Barrel: GB-22
Pro Mag: Zip 22
G Switch: G50
22RD Mag: G23
26RD Mag: G21
Ratshot: M1911A1
Engravings: M1911A1
20X100M, 14.5MM: NTW-20
.376 Steyr: Steyr Scout
Olympic Foregrip: FT300
KOM 10X42: K14
.32 ACP: Model 700
Hi-Power 8-32: Model 700
.375 Cheytac: Intervention
Siege Sight, Olympian Target Sight, Combat Barrel, Sawed Off Barrel, Olympian Stock, Sporting Stock, #000 Buckshot, 3½ Shell, Bolo Round: DT11
Pump Action: SPAS-12
20RD Drum: AA-12
Shortened Barrel, Lengthened Barrel: Saiga-12
Shrapnel, Volna-R, Barrikada, Harpoon, 4-Gauge: KS-23M
KSG 25 Barrel: KSG 12
Fold Stock: MGV-176
Closed Bolt Conv.: AUG HBAR
.20 Tactical: Colt LMG
Marksman Barrel: SL-8
.300 Whisper: SL-8
.500 Phantom: MK11
HCAR Open Bolt: HCAR
Exotic Slugs: Jury
Type 37 Conv.: M3A1
Vikhr Suppressor: SR-3M
.380 ACP: RAMA 1130
50RD Drum, M1919 Conv.: Tommy Gun
35RD Box Mag: PPSH-41
.41 AE: UZI
Sionics Suppressor: MAC10
LSW Barrel: C7A2
Wire Stock: M231
CTAR Barrel: TAR-21
.366 SP: AKM
.366 TKM Polymer: SKS
Remove Irons: Type 20
Colt Retro Scope: M16A1
60RD Drum: Type 20
Model 700: .32 ACP
.20 Tactical: Colt LMG
5.45X39MM: Groza-1
7.62X25MM: PP-19 Bizon
12.7X55MM: AK12BR
7.62X51MM: SCAR PDW
7.62X51MM Subsonic: AWS
7.5X55M, .300 WINMAG: WA2000
9X39MM: AK12C
.30-06: DBV12
Duplex: TAR-21
Triplex: X95 SMG
Flashlight Off: MP10
SVU Backup Sights, SVU-A Conv.: Dragunov SVU 
TRG-22 Kit, TRG-S Kit, Sako Backup: TRG-42
10/3.5MM: Dragunov SVDS
Unertl 8X Scope, Air Service Barrel, Pedersen Device, Air Service Mag: M1903
Heavy Can, Golf Ball, Cannon Ball, Tennis Balls, Bloxy Cola, .223 Blanks, 7.62x39 Blanks, .50 Beowulf: Can Cannon
Obrez Stock: Obrez
Burst Conv.: BWC9 A
Reduced Mag: BWC9 A
Box Grip: Tommy Gun
FAMAS G2: FAMAS F1
.357 Sig: Kriss Vector
.30-30: Henry 45-70
5.56X30MM MINSAS: X95 SMG
Remove Suppressor: Honey Badger

Other notes: 
canted iron, furro, and animu are not on ft300
no canteds or irons or suppressors on ntw20
full ring sight not on k2 or k1a or k7
some irons are missing from stg-44
rama-1130 has some missing special sights
1858 carbine, jury does not have suppressors, also check which muzzle attachments are gone
groza-4, aws, e-gun, coilgun, gyrojet carbine, kac srr, izhevsk pb and k7 have no barrel attachments
can cannon doesnt have eotech m40
m14, m21 missing some special sights, canted sights and only has carry handle sight as its only iron sight
henry 45-70 is missing oil filter
chainSAW is missing grips, and has no optics nor canted sights
stevens db and sawed off have only muffler for suppressor
e-gun, coilgun have no iron sights, and only has oeg, handmade sight, and plague insight for special sights
coilgun has default primary scopes
dt11 has no suppressors except for integral
secondaries have no canted sights by default
zip 22 has PSO-1 Scope, PSO-1M2 Scope, Swarovski Scope, PM II, Sagittarius 40X
makarov pm does not have vcog 6x scope or hensoldt 3x sight
gyrojet mark I has no barrel except for long and short barrels
gsp has no irons, doesn't have pk-a, and only has amt terminator, maglite, and plague insight for special sights
af2011-a1 has no oil filter
pp-2000, super shorty, asmi, m79 thumper, saiga-12u, sawed off and micro uzi have primary default scopes and primary default special sights for some godforsaken reason
skorpion has no iron sights, vcog 6x scope, ta33 acog, hensoldt 3x sight, ta11 acog and only has amt terminator, maglite and plague insight
asmi has canted sights and grips in other category
arm pistol has c79, pk-a, m145, ta44 acog, acog scope, hensoldt z24, ta01 acog, electra 5x, susat scope for scopes, and has primary default special sights
revolvers have no suppressors by default
super shorty, mp412 rex and m79 thumper have some more scopes than usual
m79 thumper doesnt have suppressors and doesnt have flash hider, muzzle booster, or halbek device
obrez and sfg 50 has default primary sniper scopes and default primary special sights
secondary other category has default canted, except for m79 thumper
sass 308 has default secondary scopes and special sights

pistol default scopes: C79, PK-A, M145, TA44 ACOG, ACOG Scope, VCOG 6X Scope, TA33 ACOG, Hensoldt 3X Sight, TA11 ACOG, TA01 ACOG
pistol default special sights: Maglite, AMT-Terminator, Plague Insight, Animu Sight, Furro Sight, Hand Sight, Anti Sight

sniper default:
Leupold M8-2X
C79
PK-A
M145
Malcolm 3X Scope
TA44 ACOG
PSO-1 Scope
ACOG Scope
VCOG 6X Scope
TA33 ACOG
Hensoldt 3X Sight
Hensoldt Z24
Swarovski Scope
TA11 ACOG
PU-1 Scope
FF 3X NV
PSO-1M2 Scope
TA01 ACOG
Electra 5X
Reflector Scope
VCOG 8X Scope
Susat Scope
Global Offensive Scope
HI-Power 8-32
Klassik LM
Leupold M8-6X
Leupold Mark 4
NXS 8-32
KOM 10X42
PM II
NXS 5.5-22
Sidewinder ED
Sagittarius 40X

model 700 does not have pso-1 or pso-1m2
intervention has NXS 5.5-22 by default so it does not have it as attachment
bfg 50 and sfg 50 both have nxs 8-32 as default, doesnt have as attachment
hecate doesn't have pu-1 or sidewinder ed
ft300 doesn't have pso-1 or pso-1m2
wa2000 doesn't have malcolm 3x scope, and also has klassik lm as default (no klassik lim attachment)
ntw-20 does not have malcolm 3x scope or hensoldt 3x sight or pso-1m2 or pso-1

note for future: pso-1 switches between 400 and 250 on some snipers

Family-Specific Attachments:
.36 Caliber: 1858 Carbine, 1858 New Army
Automatic: Gyrojet Carbine, Gyrojet Mark I
Extended Barrel: Desert Eagle L5, Desert Eagle XIX
G Stock: G17, G21, G23, G40, G50, G18C
Raffica Stock: M9, 93R
Suppressor..?: AS VAL, VSS Vintorez
Taurus Barrel, Taurus Stock: Judge, Executioner
8X50MMR: Obrez, Mosin Nagant
Full-Auto Conv.: Saiga-12, Saiga-12U
.50 BMG: Saiga-12, Saiga-12U
.458 SOCOM: Beowulf ECR, Beowulf TCR
12MM Rocket: Gyrojet Carbine, Gyrojet Mark I
33RD Mag: G17, G18C
Crowd Control Setup, Home Defense Setup, Heavy Discs, Light Discs, Pennies: E-Gun, Coilgun
M855: M16A1, M231
BARS Barrel: AK103, AK105, AK12
.20 Tact.: C7A2, C8A2
Snake Shot: 1858 New Army, 1858 Carbine
PU-1 Scope, Front Post: Mosin Nagant, Obrez
*/