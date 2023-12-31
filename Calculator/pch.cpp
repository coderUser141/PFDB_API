// pch.cpp: source file corresponding to the pre-compiled header

#include "pch.h"
#define DLLEXPORT __declspec(dllexport)


// When you are using pre-compiled headers, this source file is necessary for compilation to succeed.

extern "C" DLLEXPORT int test() {
	std::cout << "your mom" << std::endl; return 6;
}

/// <summary>
/// Calculates the amount of credits earned at a given rank.
/// </summary>
/// <param name="rank">Desired rank to convert.</param>
/// <returns>The credits obtained after hitting the given rank.</returns>
extern "C" DLLEXPORT unsigned long long rankToCredits(unsigned long long rank) {
	//cannot be negative, cannot be bigger than unsigned long max
	if (((UINT64_MAX - 201) / 5 ) < rank || rank < 0) return UINT64_MAX; //avoid int overflow
	if (rank == 0)return 0;
	return 200 + 5*rank;
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
	unsigned long long startSum = 200 * startRank + 5 * ((startRank * startRank + startRank)/2);
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

extern "C" DLLEXPORT void hollowPoint() {

}

extern "C" DLLEXPORT void armorPiercing() {

}

void bla() {
	std::cout << "bla" << std::endl;
}

extern "C" DLLEXPORT void write() {
	bla();
}

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
Sideways Grip		715
Hera CQR Grip		870
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
Flechette: KSG-12, Model 870, DBV12, Saiga-12, AA-12, USAS-12, Jury, Saiga-12U, Judge, Super Shorty, Sawed Off, Stevens DB, SPAS-12, M79 Thumper
Birdshot: KSG-12, Model 870, DBV12, Saiga-12, AA-12, USAS-12, Jury, Saiga-12U, Judge, Super Shorty, Sawed Off, Stevens DB, SPAS-12
Slugs: KSG-12, Model 870, DBV12, Saiga-12, AA-12, USAS-12, Saiga-12U, Judge, DT11, Super Shorty, Sawed Off, Stevens DB, SPAS-12
223 Remington: SCAR-L, AUG A1, M16A4, G36, M16A1, M16A3, Type 20, AUG A2, K2, FAMAS F1, AUG A3, L85A2, HK416, TAR-21, M231, C7A2, M4A1, G36K, M4, L22, SCAR PDW, K1A, C8A2, G36C, Arm Pistol, SL-8, Saiga-12, Saiga-12U, Colt LMG, AUG HBAR, MG36, L86 LSW, HAMR IAR, Stoner 96, ChainSAW, 
Armor Piercing NOT: Can Cannon, Gyrojet Carbine, KAC SRR, Jury, KSG-12, Model 870, DBV12, Saiga-12, AA-12, USAS-12, Saiga-12U, Judge, 1858 Carbine, Gyrojet Mark I, PP-2000, Obrez, Henry 45-70, NTW-20, WA2000, Steyr Scout, M107, SFG 50, FT300, Hecate II, K14, Dragunov SVDS, Mosin Nagant, TRG-42, AWM, BFG 50, AWS, Dragunov SVU, Model 700, Intervention
Hollow Point NOT: Gyrojet Carbine, Gyrojet Mark I, Obrez, KAC SRR, Jury, KSG-12, Model 870, DBV12, Saiga-12, AA-12, USAS-12, Saiga-12U, Judge, 1858 Carbine, Gyrojet Mark I, Henry 45-70, NTW-20, WA2000, Steyr Scout, M107, SFG 50, FT300, Hecate II, K14, M1903, Dragunov SVDS, Mosin Nagant, TRG-42, AWM, BFG 50, Dragunov SVU, Model 700, Intervention
Silent: G3A3, AG-3, AWS, Steyr Scout, K14, WA2000
Super Armor Piercing: PP-2000, AK12, AN-94, AK74, Type 88, OTs-126, X95R, RPK12, RPK74, 
Extended Magazine: M17, M45A1, KG-99, TEC-9, AS VAL, Type 20, SR-3M, BWC9 A, Model 700, Mosin Nagant, M107, Obrez
Tracerless NOT: Gyrojet Carbine, Gyrojet Mark I, Super Shorty, KSG-12, DBV12, Model 870, Saiga-12, Saiga-12U, Stevens DB, E-Gun, AA-12, SPAS-12, DT11, USAS-12
Plus P: MP5K, UMP45, MAC 10, MP5, Colt SMG 633, L2A3, MP5SD, MP10, M3A1, MP5/10, UZI, AUG A3 PARA XS, K7, Kriss Vector, PP-19 Bizon, MP40, X95 SMG, Tommy Gun, RAMA 1130, BWC9 A, G17, M9, M1911A1, G21, M45A1, G40, KG-99, Hardballer, Izhevsk PB, Makarov PM, M2011, Alien, AF2011-A1, 93R, TEC-9, Micro UZI, ASMI, MP1911, Henry 45-70
Minishell: KSG-12, Model 870, SPAS-12, Super Shorty
Minislugs: KSG-12, Model 870, SPAS-12, Super Shorty
.45 Super: UMP45, MAC10, M3A1, Kriss Vector, Tommy Gun, G21, G17, M1911A1, M45A1, AF2011-A1,, MP1911
.45 ACP: FAL Para Shorty, FAL 50.63 Para, FAL 50.00, Five-0, G3A3, Micro UZI, SA58 SPR, L2A3, UZI
9X18MM: Skorpion VZ.61, PP-19 Bizon
9X19MM: MP1911, Saiga-12U, Saiga-12, SCAR-L, AUG A1, SCAR PDW, AKU12, MAC10, AKS74U, Kriss Vector
7.62X39MM: RPK12, AK12BR, AN-94, AUG A2, K2, Type 88, SCAR-H, AK12BR, RPK12, K1A, Saiga-12, Saiga-12U, AKS74U, VSS Vintorez
.410 Bore: DBV12, Obrez, Mosin Nagant, Henry 45-70, Beowulf ECR, Jury
9.6X53MM: Saiga-12, Saiga-12U, Dragunov SVU, Dragunov SVDS
.44 Special: Mateba 6, Redhawk 44, 
Rat shot: BFG 50, SFG 50, Skorpion VZ.61, Hecate II, M107
6.5 Grendel: M16A4, M16A3, C7A2, M4A1, M4, C8A2, Colt LMG
.25-45 Sharps: M16A4, M16A3, M4A1, M4
.300 BLK: AUG A3, X95R
.300 BLK.: G36, G36C, G36K, MG36, HK416
.366 Shot: AKM, SKS
5.56X45MM: AK103, STG-44, Beowulf ECR, Beowulf TCR, FAL 50.00, FAL 50.63 Para, FAL Para Shorty, SA58 SPR, Groza-1, AK105, X95R, K7, Steyr Scout, 
Rubber Pellets: Jury, Super Shorty, KSG-12, Model 870, Saiga-12, Saiga-12U, Stevens DB, AA-12, SPAS-12
.308 WIN: Saiga-12, Saiga-12U, AWM
.308 WIN.: K14, MSG90
.40S&W: UMP45, MP5/10, AUG A3 PARA XS
.22LR: UZI, Kriss Vector, AUG HBAR
Depleted Uranium: MG3KWS, MK11
6.5MM: M14, M21, SCAR SSR
.338 Norma: AWM, TRG-42
10MM Auto: MP5K, GB-22, Tommy Gun
Burst Grouping: MP5K, MP5, MP5SD, Colt SMG 633
Semi-Auto Conv.: AA-12, BWC9 A
Carbine Barrel: BFG 50
Squad Barrel:
CQB Barrel: Intervention, M107, Hecate II, 
Full Stock: Dragunov SVDS,
Remove Stock: BFG 50, SFG 50, Model 700, K14, M1903, Intervention, TRG-42, Dragunov SVDS, Mosin Nagant, FT300, Hecate II
Retract Stock: Dragunov SVDS
Heavy Barrel: BFG 50, TRG-42, Steyr Scout, FT300, Hecate II, Model 700
Light Barrel: BFG 50, TRG-42, K14, Steyr Scout, FT300, Hecate II, Model 700, Dragunov SVDS, M1903
Front Post: Mosin Nagant, Obrez,
Marksman Kit: Intervention, Model 700, AWS, AWM, TRG-42, Mosin Nagant, M1903, K14, Hecate II, Steyr Scout, Hecate II, NTW-20, FT300, Obrez
M903 SLAP, .17 Incinerator, .416 Barrett: Hecate II, BFG 50, SFG 50, M107
Obrez Barrel: NTW-20, PPSH-41


Model 700: .32 ACP
.20 Tactical: Colt LMG
5.45X39MM: Groza-1
7.62X25MM
12.7X55MM: AK12BR
7.62X51MM: SCAR PDW
9X39MM: AK12C
.30-06: DBV12
Duplex: TAR-21
Triplex: X95 SMG
Flashlight Off: MP10
SVU Backup Sights, SVU-A Conv.: Dragunov SVU 
TRG-22 Kit, TRG-S Kit, Sako Backup: TRG-42
10/3.5MM: Dragunov SVDS
Unertl 8X Scope, Air Service Barrel, Pedersen Device, Air Service Mag: M1903
Obrez Stock: Obrez

Other notes: 
canted iron, furro, and animu are not on ft300
no canteds on ntw20

Family-Specific Attachments:
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
*/