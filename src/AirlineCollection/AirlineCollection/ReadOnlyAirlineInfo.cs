using System;
using System.Linq;

namespace ShereSoft
{
    /// <summary>
    /// Initializes a new instance of the ReadOnlyAirlineInfo class 
    /// </summary>
    public class ReadOnlyAirlineInfo
    {
        /// <summary>
        /// IATA Alpha 2 Code (read only)
        /// </summary>
        public string Iata2LetterCode { get; }

        /// <summary>
        /// ICAO Alpha 3 Code (read only)
        /// </summary>
        public string Icao3LetterCode { get; }

        /// <summary>
        /// Prefix (read only)
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// IATA Name (read only)
        /// </summary>
        public string IataName { get; }

        /// <summary>
        /// ICAO Name (read only)
        /// </summary>
        public string IcaoName { get; }

        /// <summary>
        /// Call Sign (read only)
        /// </summary>
        public string Callsign { get; }

        /// <summary>
        /// General name
        /// </summary>
        public string Name { get; set; }

        internal ReadOnlyAirlineInfo(string iata2LetterCode, string prefix, string icao3LetterCode, string iataEnglishName, string icaoEnglishName, string callsign, string name)
        {
            if (iata2LetterCode == null)
            {
                throw new ArgumentNullException(nameof(iata2LetterCode));
            }

            if (icao3LetterCode == null)
            {
                throw new ArgumentNullException(nameof(icao3LetterCode));
            }

            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            if (iataEnglishName == null)
            {
                throw new ArgumentNullException(nameof(iataEnglishName));
            }

            if (icaoEnglishName == null)
            {
                throw new ArgumentNullException(nameof(icaoEnglishName));
            }

            iata2LetterCode = iata2LetterCode.Trim();
            icao3LetterCode = icao3LetterCode.Trim();
            prefix = prefix.Trim();

            if (iata2LetterCode.Length != 2 || !iata2LetterCode.All(Char.IsLetterOrDigit))
            {
                throw new FormatException($"{nameof(iata2LetterCode)} must be a 2-digit alphanumeric code.");
            }

            if (icao3LetterCode.Length != 3 || !icao3LetterCode.All(Char.IsLetterOrDigit))
            {
                throw new FormatException($"{nameof(icao3LetterCode)} must be a 3-digit alphanumeric code.");
            }

            if (prefix.Length != 3 || !prefix.All(Char.IsNumber))
            {
                throw new FormatException($"{nameof(prefix)} must be all numeric characters.");
            }

            Iata2LetterCode = iata2LetterCode;
            Icao3LetterCode = icao3LetterCode;
            Prefix = prefix;
            IataName = iataEnglishName.Trim();
            IcaoName = icaoEnglishName.Trim();
            Name = name?.Trim();
            Callsign = callsign?.Trim();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ReadOnlyAirlineInfo ai)
            {
                return ai.Iata2LetterCode == Iata2LetterCode && ai.Icao3LetterCode == Icao3LetterCode && ai.Prefix == Prefix;
            }
            else if (obj is string c)
            {
                return String.Equals(c, Iata2LetterCode, StringComparison.OrdinalIgnoreCase) || String.Equals(c, Icao3LetterCode, StringComparison.OrdinalIgnoreCase) || String.Equals(c.TrimStart('0'), Prefix.TrimStart('0'));
            }
            else if (obj is int p)
            {
                return p == int.Parse(Prefix);
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this airline info.
        /// </summary>
        /// <returns>A hash code for the current airline info.</returns>
        public override int GetHashCode()
        {
            return Iata2LetterCode.GetHashCode() ^ Icao3LetterCode.GetHashCode() ^ Prefix.GetHashCode();
        }
    }
}
