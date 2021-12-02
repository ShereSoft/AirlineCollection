# AirlineCollection
Provides convenient access to airline info including 2-letter codes, 3-letter codes and airline names based on IATA and ICAO. No library dependencies. No external calls.

[![](https://img.shields.io/nuget/v/AirlineCollection.svg)](https://www.nuget.org/packages/AirlineCollection/)
[![](https://img.shields.io/nuget/dt/AirlineCollection)](https://www.nuget.org/packages/AirlineCollection/)

* Light-weight
* Device-agnostic country data (Alternative to the .NET Framework RegionInfo data)
* Preloaded airline information wtih IATA 2-digit code, ICAO 3-digit code, and names.
* Airline code lookup and normalization
* Convenient static data collection with methods
* Fully customizable instance data collection (to add new data and to remove old data)
* IEnumerable implementation (can be used with LINQ)
* DI-friendly interface
* Unit Tested
* No external library dependencies
* No external calls
<br />

Name|Format|Description|Example|Notes|
----|------|-----------|-------|-----|
Iata2LetterCode|Alphanumeric(2)|IATA 2-Letter Code|"AA"|Commonly used for flight numbers; Can be recycled from a defunct airline|
Icao3LetterCode|Alphanumeric(3)|ICAO 3-Letter Code|"AAL"|Occasionally used for flight numbers|
Prefix|Numeric(3)|IATA-assigned air waybill prefix|"001"||
IataName|(string)|Name registered at IATA|"American Airlines Inc."|Not recommended for general use|
IcaoName|(string)|Name registered at ICAO|"American Airlines"|Not recommended for general use|
CallSign|alphanumeric|Identification of an aircraft in air-ground communications|"American"||
Name|(string)|General name|"American Airlines"|The most commonly used name|

<br />

## STATIC MEMBERS
Read-only data collection 

### .Contains(code)
```csharp
string code = "AA";
bool valid = AirlineCollection.Contains(code);  // True
```

### .Contains(code)
```csharp
string lowerCased = "aa";
bool valid = AirlineCollection.Contains(lowerCased);  // True
```

### .Contains(code)
```csharp
int numericCode = 1;
bool valid = AirlineCollection.Contains(numericCode);  // True
```

### .Normalize(code)
```csharp
string iata2DigitCode = "aa";
string code = AirlineCollection.Normalize(iata2DigitCode);  // "AA"
```

### .Normalize(code)
```csharp
string icao3DigitCode = "aal";
string code = AirlineCollection.Normalize(icao3DigitCode);  // "AAL"
```

### .Normalize(code)
```csharp
int numericCode = 1;
string code = AirlineCollection.Normalize(numericCode);  // "001"
```

### .Normalize(code)
```csharp
string invalidCode = "xyz";
string code = AirlineCollection.Normalize(invalidCode);  // null
```

### .Normalize(code)
```csharp
int numericCode = 99999;
string code = AirlineCollection.Normalize(numericCode);  // null
```

### .GetAirline(code)
```csharp
string code = "AA";
var country = AirlineCollection.GetAirline(code);  // { "Iata2LetterCode":"AA", "Icao3LetterCode":"USA", "Prefix":"001", "IataName":"American Airlines Inc.", "IcaoName":"American Airlines", "Callsign":"American Airlines", "Name":"American Airlines" }
```

### .GetCountry(code)
```csharp
string invalidCode = "XYZ";
var airline = CountryCollection.GetAirline(code);  // null
```
<br />
 
## INSTANCE MEMBERS
Customizable data collection (All instance members can be mocked in unit testing by implementing IAirlineCollection)

### [code]
```csharp
string code = "AA";
var airline = new AirlineCollection()[code];  // { "Iata2LetterCode":"AA", "Icao3LetterCode":"USA", "Prefix":"001", "IataName":"American Airlines Inc.", "IcaoName":"American Airlines", "Callsign":"American Airlines", "Name":"American Airlines" }
```

### .Add(iata2LetterCode, icao3LetterCode, prefix)
### .Add(iata2LetterCode, icao3LetterCode, prefix, iataName, icaoName, callSign, name)
```csharp
var airlines = new AirlineCollection();
airlines.Add("XX", "XXX", "999");
airlines.Add("ZZ", "ZZZ", "000", "ZZ Airline", "ZZ Air LLC", "ZZA", "ZZ Air");
```

### .Remove(code)
```csharp
var code = "AA";
var airlines = new AirlineCollection();
airlines.Remove(code);
```
<br />
 
## EXTENSION METHODS

### .Contains(code)
```csharp
string code = "AA";
bool valid = new AirlineCollection().Contains(code);  // extension method
```

### .Normalize(code)
```csharp
string iata2DigitCode = "aa";
string code = new AirlineCollection().Normalize(iata2DigitCode);  // extension method
```

### .GetAirline(code)
```csharp
string code = "AA";
var country = AirlineCollection.GetAirline(code);  // extension method
```
