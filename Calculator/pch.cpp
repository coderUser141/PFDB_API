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





*/