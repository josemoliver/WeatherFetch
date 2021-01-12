BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Settings" (
	"Setting"	TEXT,
	"Value"	TEXT
);
CREATE TABLE IF NOT EXISTS "ReadingsLog" (
	"DateUTC"	TEXT,
	"ReadingsCount"	INTEGER,
	"DateRetrieved"	TEXT
);
CREATE TABLE IF NOT EXISTS "Readings" (
	"dateutc"	TEXT,
	"winddir"	REAL,
	"windspeedmph"	REAL,
	"windgustmph"	REAL,
	"maxdailygust"	REAL,
	"tempf"	REAL,
	"hourlyrainin"	REAL,
	"eventrainin"	REAL,
	"dailyrainin"	REAL,
	"weeklyrainin"	REAL,
	"monthlyrainin"	REAL,
	"totalrainin"	REAL,
	"baromrelin"	REAL,
	"baromabsin"	REAL,
	"humidity"	REAL,
	"tempinf"	REAL,
	"humidityin"	REAL,
	"uv"	REAL,
	"solarradiation"	REAL,
	"feelsLike"	REAL,
	"dewPoint"	TEXT,
	"lastRain"	TEXT,
	"date"	TEXT,
	"datelocal"	TEXT,
	PRIMARY KEY("dateutc")
);
CREATE VIEW WeatherReport AS
select 
strftime('%m/%d/%Y',datetime(ROUND(dateutc)/1000,'unixepoch','-4 Hours')) as Reading_Date, 
CASE 
        WHEN CAST(strftime('%H', datetime(ROUND(dateutc)/1000,'unixepoch','-4 Hours')) AS INTEGER) = 12 
                THEN strftime('%H:%M', datetime(ROUND(dateutc)/1000,'unixepoch','-4 Hours')) || ' PM'   
        WHEN CAST(strftime('%H', datetime(ROUND(dateutc)/1000,'unixepoch','-4 Hours')) AS INTEGER) > 12 
                THEN strftime('%H:%M', datetime(ROUND(dateutc)/1000,'unixepoch','-4 Hours'), '-12 Hours') || ' PM'
		WHEN CAST(strftime('%H', datetime(ROUND(dateutc)/1000,'unixepoch','-4 Hours')) AS INTEGER) = 0
				THEN strftime('12:%M AM', datetime(ROUND(dateutc)/1000,'unixepoch','-4 Hours'))
        ELSE strftime('%H:%M', datetime(ROUND(dateutc)/1000,'unixepoch','-4 Hours')) || ' AM' 
END 
       AS Reading_Time,
CASE
	WHEN tempf = 0.0
		THEN '--'
	ELSE
		cast(round(((tempf-32)*5/9)) as int) || ' °C'
END 
	AS Temperature, 
CASE
	WHEN cast(humidity as int)=0 THEN '--'
	ELSE cast(humidity as int) || ' %' 
END 
	AS Humidity, 
round((baromrelin/0.029529983071445),2) || ' hPa' as Pressure 
FROM Readings ORDER BY Readings.dateutc DESC;
CREATE VIEW weatherhistory AS
select * from WeatherReport WHERE Temperature<>'--' OR humidity<>'--';
COMMIT;
