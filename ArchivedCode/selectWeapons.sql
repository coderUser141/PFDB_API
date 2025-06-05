DROP TABLE IF EXISTS cumulativeChanges;
CREATE TABLE cumulativeChanges (
	"weaponName" INTEGER,
	"category" TEXT,
	"categoryNumber" INTEGER,
	"categoryShorthand" TEXT,
	"versionRankTiebreaker" TEXT,
	"rank" TEXT
);
INSERT INTO cumulativeChanges ("weaponName","category","categoryNumber","categoryShorthand","versionRankTiebreaker", "rank")
SELECT "weaponName","category","categoryNumber","categoryShorthand","versionRankTiebreaker", "rank"
FROM version800;

DELETE FROM cumulativeChanges
WHERE cumulativeChanges.weaponName IN (
	SELECT cumulativeChanges.weaponName FROM cumulativeChanges
	INNER JOIN version801
	ON (cumulativeChanges.weaponName = version801.weaponName AND cumulativeChanges.categoryNumber = version801.categoryNumber)
	WHERE cumulativeChanges.weaponName IS NOT NULL AND version801.weaponName IS NOT NULL
);

INSERT INTO cumulativeChanges ("weaponName","category","categoryNumber","categoryShorthand","versionRankTiebreaker", "rank")
SELECT "weaponName","category","categoryNumber","categoryShorthand","versionRankTiebreaker", "rank"
FROM version801;

DELETE FROM cumulativeChanges
WHERE cumulativeChanges.weaponName IN (
	SELECT cumulativeChanges.weaponName FROM cumulativeChanges
	INNER JOIN version802
	ON (cumulativeChanges.weaponName = version802.weaponName AND cumulativeChanges.categoryNumber = version802.categoryNumber)
	WHERE cumulativeChanges.weaponName IS NOT NULL AND version802.weaponName IS NOT NULL
);

INSERT INTO cumulativeChanges ("weaponName","category","categoryNumber","categoryShorthand","versionRankTiebreaker", "rank")
SELECT "weaponName","category","categoryNumber","categoryShorthand","versionRankTiebreaker", "rank"
FROM version802;

DELETE FROM cumulativeChanges
WHERE cumulativeChanges.weaponName IN (
	SELECT cumulativeChanges.weaponName FROM cumulativeChanges
	INNER JOIN version902
	ON (cumulativeChanges.weaponName = version902.weaponName AND cumulativeChanges.categoryNumber = version902.categoryNumber)
	WHERE cumulativeChanges.weaponName IS NOT NULL AND version902.weaponName IS NOT NULL
);

INSERT INTO cumulativeChanges ("weaponName","category","categoryNumber","categoryShorthand","versionRankTiebreaker", "rank")
SELECT "weaponName","category","categoryNumber","categoryShorthand","versionRankTiebreaker", "rank"
FROM version902;

DELETE FROM cumulativeChanges
WHERE cumulativeChanges.weaponName IN (
	SELECT cumulativeChanges.weaponName FROM cumulativeChanges
	INNER JOIN version903
	ON (cumulativeChanges.weaponName = version903.weaponName AND cumulativeChanges.categoryNumber = version903.categoryNumber)
	WHERE cumulativeChanges.weaponName IS NOT NULL AND version903.weaponName IS NOT NULL
);

INSERT INTO cumulativeChanges ("weaponName","category","categoryNumber","categoryShorthand","versionRankTiebreaker", "rank")
SELECT "weaponName","category","categoryNumber","categoryShorthand","versionRankTiebreaker", "rank"
FROM version903;

SELECT "weaponName", "category", "categoryNumber", "categoryShorthand", "rank", "versionRankTiebreaker" FROM cumulativeChanges ORDER BY "categoryNumber", "rank" + 0, "versionRankTiebreaker" + 0;