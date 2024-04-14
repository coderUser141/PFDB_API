DROP TABLE IF EXISTS cumulativeChanges;
CREATE TEMP TABLE cumulativeChanges (
	"weapon_name" INTEGER,
	"category" TEXT,
	"category_number" INTEGER,
	"category_shorthand" TEXT,
	"versionRankTiebreaker" TEXT,
	"rank" TEXT
);
INSERT INTO cumulativeChanges ("weapon_name","category","category_number","category_shorthand","versionRankTiebreaker", "rank")
SELECT "weapon_name","category","category_number","category_shorthand","versionRankTiebreaker", "rank"
FROM version800;


INSERT INTO cumulativeChanges ("weapon_name","category","category_number","category_shorthand","versionRankTiebreaker", "rank")
SELECT "weapon_name","category","category_number","category_shorthand","versionRankTiebreaker", "rank"
FROM version801;

SELECT "weapon_name", "category", "category_number", "category_shorthand", "rank", "versionRankTiebreaker" FROM cumulativeChanges ORDER BY "category_number", "rank" + 0, "versionRankTiebreaker" + 0;