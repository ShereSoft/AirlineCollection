using ShereSoft;
using System.Collections.Generic;

/// <summary>
/// Represents a collection of airlines
/// </summary>
public interface IAirlineCollection : IEnumerable<ReadOnlyAirlineInfo>
{
    /// <summary>
    /// Returns the matching airline by code; null if not found
    /// </summary>
    /// <param name="code">The value to find in the AirlineCollection.</param>
    /// <returns>The matching airline in the AirlineCollection.</returns>
    ReadOnlyAirlineInfo this[string code] { get; }

    /// <summary>
    /// Add a new airline
    /// </summary>
    /// <param name="iata2LetterCode">The IATA 2-letter code</param>
    /// <param name="icao3LetterCode">The ICAO 3-letter code</param>
    /// <param name="prefix">The airline prefix</param>
    void Add(string iata2LetterCode, string icao3LetterCode, string prefix);

    /// <summary>
    /// Add a new airline
    /// </summary>
    /// <param name="iata2LetterCode">The IATA 2-letter code</param>
    /// <param name="icao3LetterCode">The ICAO 3-letter code</param>
    /// <param name="prefix">The airline prefix</param>
    /// <param name="iataName">The IATA airline name</param>
    /// <param name="icaoName">The ICAO airline name</param>
    /// <param name="callSign">The call sign</param>
    /// <param name="name">General name</param>
    void Add(string iata2LetterCode, string icao3LetterCode, string prefix, string iataName, string icaoName, string callSign, string name);

    /// <summary>
    /// Removes the existing airline by code
    /// </summary>
    /// <param name="code">The airline code to remove from the current instance</param>
    void Remove(string code);
}