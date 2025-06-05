CREATE TABLE "version1011" (
	"weapon_name"	TEXT NOT NULL UNIQUE,
	"current_state"	TEXT,
	"category"	TEXT NOT NULL,
	"category_number"	INTEGER NOT NULL,
	"category_shorthand"	TEXT,
	"previous_category"	TEXT,
	"previous_category_number"	INTEGER,
	"previous_category_shorthand"	TEXT,
	"versionRankTiebreaker"	INTEGER,
	PRIMARY KEY("weapon_name")
);