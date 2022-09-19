# WeatherFetch
Command line tool for fetching Ambientweather.net weather data from your Personal Weather Station (PWS) and storing it in a local sqlite database.

### Adding your device and keys to Settings.json
1. Obtain an API Key and Application Key for AmbientWeather.net. Refer to https://help.ambientweather.net/help/api/ for information on obtaining this information.
2. Obtain your Weather Station's Device MAC Address, displayed in the "Devices" page (https://ambientweather.net/devices) in AmbientWeather.net
3. Open the Settings.json file in a text editor such as Notepad and add your MAC Address Value, API Key and Application Key.

You can download the console application with an empty SQLite database file from https://github.com/josemoliver/WeatherFetch/releases
Otherwise, you can create a new SQLite Database and Build using the following steps:

### Create a new SQLite Database
1. Download and install an application such as SQLite Browser https://sqlitebrowser.org/
2. Create a new database named WeatherStation.sqlite
3. Execute the WeatherStation.sqlite.sql script to create the database schema

### Build the Solution
1. Open WeatherFetch.csproj in Microsot Visual Studio
2. Build solution

## Command Line Usage

```dotnet WeatherFetch.dll -days=NUM ```
  
where NUM is the number of days past from the time the application was executed to retrieve weather station data from AmbientWeather.net

.Net Core 2.1 min is required (https://dotnet.microsoft.com/download/dotnet-core)
