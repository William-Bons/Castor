CREATE TABLE "movebook" (
	"id"	INTEGER NOT NULL UNIQUE,
	"card_id"	INTEGER NOT NULL,
	"fio"	TEXT NOT NULL,
	"birthdate"	DATE NOT NULL,
	"datein"	DATE NOT NULL,
	"dateout"	DATE NOT NULL,
	"ordered"	NUMERIC,
	"dsin"	TEXT NOT NULL,
	"dsout"	TEXT,
	"outto"	NUMERIC,
	"city"	NUMERIC,
	"firt"	NUMERIC,
	"second"	NUMERIC,
	"early"	NUMERIC,
	"unvoluntary"	NUMERIC,
	"date_lastout"	DATE,
	"agein"	INT GENERATED ALWAYS AS (CAST(strftime('%Y', "datein") AS INTEGER) - CAST(strftime('%Y', "birthdate") AS INTEGER) - (strftime('%m-%d', "datein") < strftime('%m-%d', "birthdate"))) VIRTUAL,
	"ageout"	INT GENERATED ALWAYS AS (CAST(strftime('%Y', "dateout") AS INTEGER) - CAST(strftime('%Y', "birthdate") AS INTEGER) - (strftime('%m-%d', "dateout") < strftime('%m-%d', "birthdate"))) VIRTUAL,
	"Ai"	int GENERATED ALWAYS AS (iif("dsin" == "21" OR "dsin" == "22" OR "dsin" == "23" OR "dsin" == "25" OR "dsin" == "30" OR "dsin" == "31" OR "dsin" == "32" OR "dsin" == "00" OR "dsin" == "01", 1, 0)) VIRTUAL,
	"Bi"	int GENERATED ALWAYS AS (iif("dsin" == "20", 1, 0)) VIRTUAL,
	"Ci"	int GENERATED ALWAYS AS (iif("dsin" == "70" OR "dsin" == "71" OR "dsin" == "72", 1, 0)) VIRTUAL,
	"Di"	int GENERATED ALWAYS AS (iif("dsin" == "02" OR "dsin" == "03" OR "dsin" == "04" OR "dsin" == "05" OR "dsin" == "06" OR "dsin" == "07" OR "dsin" == "50" OR "dsin" == "60" OR "dsin" == "61" OR "dsin" == "62" OR "dsin" == "90" OR "dsin" == "91" OR "dsin" == "40" OR "dsin" == "41" OR "dsin" == "42" OR "dsin" == "43" OR "dsin" == "45", 1, 0)) VIRTUAL,
	"Ei"	int GENERATED ALWAYS AS (iif("dsin" == "10" OR "dsin" == "15" OR "dsin" == "19", 1, 0)) VIRTUAL, contr_in TEXT GENERATED ALWAYS as (
iif(Ai==0 and Bi==0 and Ci==0 and Di==0 and Ei=0, "Error: diagnosis is not classified", NULL)
), Ao int GENERATED ALWAYS as (
iif(dsout == "21" or dsout=="22" or dsout=="23" or dsout =="25" or dsout=="30" or dsout =="31" or dsout =="32" or dsout=="00" or dsout=="01",1,0)
), Bo int GENERATED ALWAYS as (
iif(dsout == "20",1,0)
), Co int GENERATED ALWAYS as (
iif(dsout == "70" or dsout =="71" or dsout=="72",1,0)
), 'Do' int GENERATED ALWAYS as (
iif(dsout == "02" or dsout=="03" or dsout =="04" or dsout =="05" or dsout =="06" or dsout =="07" or dsout=="50" or dsout =="60" or dsout=="61" or dsout =="62" 
	or dsout=="90" or dsout =="91" or dsout =="40" or dsout =="41" or dsout =="42" or dsout=="43" or dsout =="45",1,0)
), Eo int GENERATED ALWAYS as (
iif(dsout == "10" or dsout =="15" or dsout =="19",1,0)
), contr_out TEXT GENERATED ALWAYS as (
iif(Ao==0 and Bo==0 and Co==0 and 'Do'==0 and Eo=0, "Error: diagnosis is not classified", NULL)
),
	PRIMARY KEY("id" AUTOINCREMENT)
)