using ShereSoft;
using System;
using System.Linq;

/// <summary>
/// Provides a set of static (Shared in Visual Basic) methods for querying a AirlineCollection instance
/// </summary>
public static class AirlineCollectionExtensions
{
    /// <summary>
    /// Returns a matching airline by code
    /// </summary>
    /// <param name="airlines">AirlineCollection instance</param>
    /// <param name="code">The value to find in the CountryCollection.</param>
    /// <returns>The matching country in the CountryCollection.</returns>
    public static ReadOnlyAirlineInfo GetAirline(this AirlineCollection airlines, string code)
    {
        return airlines[code];
    }

    /// <summary>
    /// Returns a matching airline by numeric prefix
    /// </summary>
    /// <param name="airlines">AirlineCollection instance</param>
    /// <param name="code">The value to find in the CountryCollection.</param>
    /// <returns>The matching country in the CountryCollection.</returns>
    public static ReadOnlyAirlineInfo GetAirline(this AirlineCollection airlines, int code)
    {
        return GetAirline(airlines, code.ToString().PadLeft(3, '0'));
    }

    /// <summary>
    /// Determines whether AirlineCollection contains a specified code
    /// </summary>
    /// <param name="airlines">AirlineCollection instance</param>
    /// <param name="code">The value to check in the CountryCollection.</param>
    /// <returns>true if the CountryCollection contains a country that has the specified code; otherwise, false.</returns>
    public static bool Contains(this AirlineCollection airlines, string code)
    {
        return GetAirline(airlines, code) != null;
    }

    /// <summary>
    /// Determines whether AirlineCollection contains a specified code
    /// </summary>
    /// <param name="airlines">AirlineCollection instance</param>
    /// <param name="code">The value to check in the CountryCollection.</param>
    /// <returns>true if the CountryCollection contains a country that has the specified code; otherwise, false.</returns>
    public static bool Contains(this AirlineCollection airlines, int code)
    {
        return GetAirline(airlines, code) != null;
    }

    /// <summary>
    /// Returns a normalized value from the matching airline code.
    /// </summary>
    /// <param name="airlines">AirlineCollection instance</param>
    /// <param name="code">The value to normalize</param>
    /// <returns>The normalized code in the AirlineCollection.</returns>
#if NET40
    public static string Normalize(this AirlineCollection airlines, string code)
#else
    public static string Normalize(this AirlineCollection airlines, string code)
#endif
    {
        var ci = GetAirline(airlines, code);  // null checked here

        if (ci != null)
        {
            if (code.All(Char.IsNumber))
            {
                return ci.Prefix;
            }
            else if (code.Length == 2)
            {
                return ci.Iata2LetterCode;
            }
            else if (code.Length == 3)
            {
                return ci.Icao3LetterCode;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a normalized value from the matching airline code.
    /// </summary>
    /// <param name="airlines">AirlineCollection instance</param>
    /// <param name="code">The value to normalize</param>
    /// <returns>The normalized code in the AirlineCollection.</returns>
    public static string Normalize(this AirlineCollection airlines, int code)
    {
        var ci = GetAirline(airlines, code);

        return ci != null ? ci.Prefix : null;
    }
}
