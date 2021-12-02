using ShereSoft;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides the class for a pre-loaded, read-only airline collection or a customizable airline list instance.
/// </summary>
public class AirlineCollection : IAirlineCollection
{
    static readonly Dictionary<string, int> HashDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    static readonly Dictionary<int, ReadOnlyAirlineInfo> Dict = new Dictionary<int, ReadOnlyAirlineInfo>();

    internal readonly Dictionary<string, int> _hashDict;
    internal readonly Dictionary<int, ReadOnlyAirlineInfo> _dict;

    /// <summary>
    ///  Gets a collection containing all the airlines
    /// </summary>
    public static IEnumerable<ReadOnlyAirlineInfo> Values => Dict.Values;

    static AirlineCollection()
    {
        for (int i = 0; i < Data.Length; i++)
        {
            var values = Data[i];
            var airline = new ReadOnlyAirlineInfo(values[0], values[1], values[2], values[3], values[4], values[6], values[5]);
            var hashcode = airline.GetHashCode();

            HashDict.Add(airline.Iata2LetterCode, hashcode);
            HashDict.Add(airline.Icao3LetterCode, hashcode);
            HashDict.Add(airline.Prefix, hashcode);
            Dict.Add(hashcode, airline);
        }
    }

    /// <summary>
    /// Initializes a new instance of the AirlineCollection class that contains airlines copied from the static AirlineCollection object with add/remove capability
    /// </summary>
    public AirlineCollection()
    {
        _hashDict = new Dictionary<string, int>(HashDict, StringComparer.OrdinalIgnoreCase);
        _dict = new Dictionary<int, ReadOnlyAirlineInfo>(Dict);
    }

    /// <summary>
    /// Returns the matching airline by code; null if not found
    /// </summary>
    /// <param name="code">The value to find in the AirlineCollection.</param>
    /// <returns>The matching airline in the AirlineCollection.</returns>
    public ReadOnlyAirlineInfo this[string code]
    {
        get => GetAirline(_hashDict, _dict, code);
    }

    /// <summary>
    /// Add a new airline
    /// </summary>
    /// <param name="iata2LetterCode">The IATA 2-letter code</param>
    /// <param name="icao3LetterCode">The ICAO 3-letter code</param>
    /// <param name="prefix">The airline prefix</param>
    public void Add(string iata2LetterCode, string icao3LetterCode, string prefix)
    {
        Add(iata2LetterCode, icao3LetterCode, prefix, "", "", null, null);
    }

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
    public void Add(string iata2LetterCode, string icao3LetterCode, string prefix, string iataName, string icaoName, string callSign, string name)
    {
        var ci = new ReadOnlyAirlineInfo(iata2LetterCode, prefix, icao3LetterCode, iataName, icaoName, callSign, name);  // null checked here

        if (_hashDict.ContainsKey(iata2LetterCode) || _hashDict.ContainsKey(icao3LetterCode) || _hashDict.ContainsKey(prefix))
        {
            throw new ArgumentException("Airline already exists.");
        }

        _hashDict.Add(ci.Iata2LetterCode, ci.GetHashCode());
        _hashDict.Add(ci.Icao3LetterCode, ci.GetHashCode());
        _hashDict.Add(ci.Prefix, ci.GetHashCode());
        _dict.Add(ci.GetHashCode(), ci);
    }

    /// <summary>
    /// Removes the existing airline by code
    /// </summary>
    /// <param name="code">The airline code to remove from the current instance</param>
    public void Remove(string code)
    {
        var al = GetAirline(_hashDict, _dict, code);

        if (al != null)
        {
            _hashDict.Remove(al.Iata2LetterCode);
            _hashDict.Remove(al.Icao3LetterCode);
            _hashDict.Remove(al.Prefix);
            _dict.Remove(al.GetHashCode());
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<ReadOnlyAirlineInfo> GetEnumerator()
    {
        return ((IEnumerable<ReadOnlyAirlineInfo>)_dict.Values).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dict.Values.GetEnumerator();
    }

    /// <summary>
    /// Returns a matching airline by code
    /// </summary>
    /// <param name="code">The value to find in the AirlineCollection.</param>
    /// <returns>The matching airline in the AirlineCollection.</returns>
    public static ReadOnlyAirlineInfo GetAirline(string code)
    {
        return GetAirline(HashDict, Dict, code);
    }

    /// <summary>
    /// Returns a matching airline by numeric prefix
    /// </summary>
    /// <param name="code">The value to find in the AirlineCollection.</param>
    /// <returns>The matching airline in the AirlineCollection.</returns>
    public static ReadOnlyAirlineInfo GetAirline(int code)
    {
        return GetAirline(code.ToString().PadLeft(3, '0'));
    }

    /// <summary>
    /// Determines whether AirlineCollection contains a specified code
    /// </summary>
    /// <param name="code">The value to check in the AirlineCollection.</param>
    /// <returns>true if the AirlineCollection contains an airline that has the specified code; otherwise, false.</returns>
    public static bool Contains(string code)
    {
        return GetAirline(code) != null;
    }

    /// <summary>
    /// Determines whether AirlineCollection contains a specified code
    /// </summary>
    /// <param name="code">The value to check in the AirlineCollection.</param>
    /// <returns>true if the AirlineCollection contains an airline that has the specified code; otherwise, false.</returns>
    public static bool Contains(int code)
    {
        return GetAirline(code) != null;
    }

    /// <summary>
    /// Returns a normalized value from the matching airline code.
    /// </summary>
    /// <param name="code">The value to normalize</param>
    /// <returns>The normalized code in the AirlineCollection.</returns>
#if NET40
    public static string Normalize(string code)
#else
    public static string Normalize(string code)
#endif
    {
        var ci = GetAirline(code);  // null checked here

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
    /// <param name="code">The value to normalize</param>
    /// <returns>The normalized code in the AirlineCollection.</returns>
    public static string Normalize(int code)
    {
        return Normalize(code.ToString().PadLeft(3, '0'));
    }

    static ReadOnlyAirlineInfo GetAirline(Dictionary<string, int> hashDict, Dictionary<int, ReadOnlyAirlineInfo> dict, string code)
    {
        if (code == null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        if (!hashDict.TryGetValue(code, out var hash))
        {
            if (code.Length < 3 && code.All(Char.IsNumber))
            {
                if (!hashDict.TryGetValue(code.PadLeft(3, '0'), out hash))
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        return dict[hash];
    }

    static string[][] Data = new[]
    {
        new[] { "AA", "001", "AAL", "American Airlines Inc.", "American Airlines", "American Airlines", "American" },
        new[] { "B0", "002", "DJT", "DreamJet SAS t/a La Compagnie", "DreamJet SAS d/b/a La Compagnie", "La Compagnie", "Dreamjet" },
        new[] { "BV", "004", "BPA", "Blue Panorama Airlines S.p.A.", "Blue Panorama Airlines S.p.A.", "Blue Panorama Airlines", "Blue Panorama" },
        new[] { "DL", "006", "DAL", "Delta Air Lines, Inc.", "Delta Air Lines, Inc.", "Delta Air Lines", "Delta" },
        new[] { "MV", "012", "MAR", "Air Mediterranean S.A.", "Air Mediterranean S.A.", "Air Mediterranean", "Hellasmed" },
        new[] { "AC", "014", "ACA", "Air Canada", "Air Canada", "Air Canada", "Air Canada" },
        new[] { "UA", "016", "UAL", "United Airlines, Inc.", "United Airlines, Inc.", "United Airlines", "United" },
        new[] { "HO", "018", "DKH", "Juneyao Airlines Co,. Ltd.", "Juneyao Airlines Co. Ltd.", "Juneyao Air", "Air Juneyao" },
        new[] { "OT", "024", "CDO", "Tchadia Airlines", "Tchadia Airlines", "Tchadia Airline", "Tchadia" },
        new[] { "AS", "027", "ASA", "Alaska Airlines Inc.", "Alaska Airlines, Inc.", "Alaska Airlines", "Alaska" },
        new[] { "VY", "030", "VLG", "Vueling Airlines S.A.", "Vueling Airlines, S.A.", "Vueling", "Vueling" },
        new[] { "KP", "032", "SKK", "Compagnie Aerienne ASKY dba ASKY", "ASKY", "ASKY", "Asky Airline" },
        new[] { "Y4", "036", "VOI", "Concesionaria Vuela Compania De Aviacion SA de CV (Volaris)", "Concesionaria Vuela Compania De, SA de CV (Volaris)", "Volaris", "Volaris" },
        new[] { "ZF", "037", "AZV", "AZUR air Limited Liability Company", "AZUR air Limited Liability Company", "Azur Air", "Azur Air" },
        new[] { "AR", "044", "ARG", "Aerolineas Argentinas S.A.", "Aerolineas Argentinas", "Aerolíneas Argentinas", "Argentina" },
        new[] { "LA", "045", "LAN", "LATAM Airlines Group S.A. dba LATAM Airlines Group", "LATAM Airlines Chile", "LATAM Chile", "LAN" },
        new[] { "TP", "047", "TAP", "TAP Portugal", "TAP Portugal", "TAP Air Portugal", "Air Portugal" },
        new[] { "OA", "050", "OAL", "Olympic Air", "Olympic Air", "Olympic Air", "Olympic" },
        new[] { "EI", "053", "EIN", "Aer Lingus Limited", "Aer Lingus Limited", "Aer Lingus", "Shamrock" },
        new[] { "2D", "054", "EAL", "Eastern Airlines, LLC", "Eastern Airlines, LLC", "Eastern Airlines, LLC", "Eastern" },
        new[] { "AZ", "055", "ITY", "Alitalia Societa Aerea Italiana S.p.A", "Italia Transporto Aereo S.p.A. d/b/a ITA S.p.A", "Alitalia", "Itarrow" },
        new[] { "AF", "057", "AFR", "Air France", "Air France", "Air France", "Airfrans" },
        new[] { "I2", "060", "IBS", "Compania Operadora de Corto y Medio Radio Iberia Express, S.A.U", "Compania Operadora de Corto y Medio Radio Iberia Express, S.A.U", "Iberia Express", "Iberexpres" },
        new[] { "HM", "061", "SEY", "Air Seychelles Limited", "Air Seychelles Limited", "Air Seychelles", "Seychelles" },
        new[] { "5Q", "062", "HES", "Holiday Europe Ltd.", "Holiday Europe Ltd.", "Holiday Europe", "Holiday Europe" },
        new[] { "SB", "063", "ACI", "Air Caledonie International", "Air Caledonie International", "Aircalin", "Air Calin" },
        new[] { "OK", "064", "CSA", "Czech Airlines a.s,. CSA", "Czech Airlines A.S. , CSA", "Czech Airlines", "CSA-Lines" },
        new[] { "SV", "065", "SVA", "Saudi Arabian Airlines Corporation", "Saudi Arabian Airlines", "Saudia", "Saudia" },
        new[] { "TM", "068", "LAM", "LAM - Linhas Aereas de Mocambique", "LAM", "LAM Mozambique Airlines", "Mozambique" },
        new[] { "IK", "069", "AKL", "Air Kiribati Limited dba Air Kiribati", "Air Kiribati Limited", "Air Kiribati", "Air Kiribati" },
        new[] { "RB", "070", "SYR", "Syrian Arab Airlines", "Syrian Arab Airlines", "Syrian Air", "Syrianair" },
        new[] { "ET", "071", "EMT", "ETHIOPIAN AIRLINES", "Emetebe.com.ec Air Taxi Charter Service", "Ethiopian Airlines", "Emetebe" },
        new[] { "GF", "072", "GFA", "Gulf Air B.S.C. (c)", "Gulf Air Company G.S.C.", "Gulf Air", "Gulf Air" },
        new[] { "KL", "074", "KLM", "KLM", "KLM Royal Dutch Airlines (Koninklijke Luchtvaart Maatschappij N.V.)", "KLM", "KLM" },
        new[] { "IB", "075", "IBE", "Iberia Lineas Aereas de Espana S.A. Operadora", "Iberia", "IBERIA", "Iberia" },
        new[] { "ME", "076", "MEA", "Middle East Airlines AirLiban", "Middle East Airlines", "Middle East Airlines", "Cedar Jet" },
        new[] { "MS", "077", "MSR", "Egyptair dba Egyptair Airlines", "Egyptair", "Egyptair", "Egyptair" },
        new[] { "CY", "078", "CYP", "Charlie Airlines Limited dba Cyprus Airways", "Charlie Airways t/a Cyprus Airways", "Cyprus Airways", "CYPRUS" },
        new[] { "PR", "079", "PAL", "Philippine Airlines, Inc.", "Philippine Airlines, Inc.", "Philippine Airlines", "Philippine" },
        new[] { "LO", "080", "LOT", "LOT Polish Airlines", "LOT", "LOT Polish Airlines", "Lot" },
        new[] { "QF", "081", "QFA", "Qantas Airways Ltd.", "Qantas Airways Ltd.", "Qantas", "Qantas" },
        new[] { "SN", "082", "BEL", "Brussels Airlines", "Brussels Airlines N.V.", "Brussels Airlines", "Beeline" },
        new[] { "SA", "083", "CIG", "South African Airways SOC LTD dba South african Airways", "Air Company Sirius-Aero Ltd", "South African Airways", "Sirius Aero" },
        new[] { "PA", "084", "ABQ", "M/S Airblue Limited", "M/S Airblue (PVT) Ltd", "Airblue", "Pakblue" },
        new[] { "NZ", "086", "ANZ", "Air New Zealand Limited", "Air New Zealand Limited", "Air New Zealand", "New Zealand" },
        new[] { "9C", "089", "CQH", "Spring Airlines Limited Corporation", "Spring Airlines Limited Corporation", "Spring Airlines", "Air Spring" },
        new[] { "I5", "091", "IAD", "AirAsia (India) Limited AIRASIA", "Air Asia (India) Ltd.", "AirAsia India", "Red Knight" },
        new[] { "MQ", "093", "ENY", "Envoy Air Inc.", "Envoy Air, Inc.", "Envoy Air", "Envoy" },
        new[] { "IR", "096", "IRA", "Iran Air", "Iran Air", "Iran Air", "Iranair" },
        new[] { "AI", "098", "AIC", "Air India Limited dba Air India", "Air India Limited", "Air India", "Air India" },
        new[] { "EN", "101", "DLA", "AIR DOLOMITI S.p.A. LINEE AEREE REGIONALI EUROPEE", "Air Dolomiti S.p.A. Linee Aeree Regionali Europee", "Air Dolomiti", "Dolomiti" },
        new[] { "EW", "104", "EWG", "Eurowings GmbH", "Eurowings GmbH", "Eurowings", "Eurowings" },
        new[] { "AY", "105", "FIN", "Finnair Oyj", "Finnair Oyj", "Finnair", "Finnair" },
        new[] { "BW", "106", "BWA", "Caribbean Airlines Limited dba Caribbean Airlines Ltd.", "Caribbean Airlines Limited", "Caribbean Airlines", "Caribbean Airlines" },
        new[] { "FI", "108", "ICE", "Icelandair ehf.", "Icelandair", "Icelandair", "Iceair" },
        new[] { "LY", "114", "ELY", "El Al Israel Airlines Ltd. dba EL AL", "EL AL Israel Airlines Ltd. d/b/a El Al", "EL AL", "El Al" },
        new[] { "JU", "115", "ASL", "JSC for Air Traffic-Air Serbia Belgrade t/a Air Serbia a.d Beograd", "JSC for Air Traffic-Air SERBIA Belgrade t/a Air Serbia a.d. Beograd", "Air Serbia", "Air Serbia" },
        new[] { "MG", "116", "EZA", "EZNIS AIRWAYS LLC", "Eznis Airways LLC", "Eznis Airways", "EZNIS" },
        new[] { "SK", "117", "SAS", "Scandinavian Airlines System (SAS)", "Scandinavian Airlines System (SAS)", "Scandinavian Airlines", "Scandinavian" },
        new[] { "DT", "118", "DTA", "TAAG - Linhas Aereas de Angola (Angola Airlines)", "TAAG", "TAAG Angola Airlines", "DTA" },
        new[] { "JS", "120", "KOR", "Air Koryo", "Air Koryo", "Air Koryo", "Air Koryo" },
        new[] { "ON", "123", "RON", "Nauru Air Corporation dba Nauru Airlines", "Nauru Air Corporation t/a Nauru Airlines", "Nauru Airlines", "AIR NAURU" },
        new[] { "AH", "124", "DAH", "Air Algerie", "Air Algerie", "Air Algérie", "Air Algerie" },
        new[] { "BA", "125", "BAW", "British Airways p.l.c.", "British Airways P.L.C.", "British Airways", "Speedbird" },
        new[] { "GA", "126", "GIA", "PT. Garuda Indonesia (PERSERO) Tbk dba Garuda Indonesia", "Garuda Indonesia", "Garuda Indonesia", "Indonesia" },
        new[] { "G3", "127", "GLO", "GOL Linhas Aereas S.A.", "GOL Linhas Aereas Inteligentes", "Gol Linhas Aéreas Inteligentes", "Gol Transporte" },
        new[] { "UO", "128", "HKE", "Hong Kong Express Airways Limited", "Hong Kong Express Airways Ltd", "HK Express", "Hongkong Shuttle" },
        new[] { "5F", "130", "FIA", "Fly One S.R.L.", "FlyOne Airlines S.R.L.", "FLYONE", "Fia Airlines" },
        new[] { "JL", "131", "JAL", "Japan Airlines Co., Ltd.", "Japan Airlines International Co. Ltd.", "Japan Airlines", "Japanair" },
        new[] { "LR", "133", "LRC", "Avianca Costa Rica, S.A. dba Avianca Costa Rica, S.A.", "Avianca Costa Rica S.A.", "Avianca Costa Rica", "LACSA" },
        new[] { "AV", "134", "AVA", "Aerovias del Continente Americano S.A. AVIANCA", "Avianca", "Avianca", "Avianca" },
        new[] { "VT", "135", "VTA", "Air Tahiti", "Air Tahiti", "Air Tahiti", "Air Tahiti" },
        new[] { "CU", "136", "CUB", "Cubana de Aviacion S.A.", "Cubana de Aviacion, S.A.", "Cubana de Aviación", "Cubana" },
        new[] { "AM", "139", "AMX", "Aeromexico", "Aeromexico Aerovias de Mexico S.A. de C.V.", "Aeroméxico", "Aeromexico" },
        new[] { "FZ", "141", "FDB", "Flydubai dba Dubai Aviation Corporation \"FLYDUBAI\"", "Dubai Aviation Corporation d/b/a flydubai", "Flydubai", "Sky Dubai" },
        new[] { "KF", "142", "ABB", "Air Belgium SA", "Air Belgium SA", "Air Belgium S.A.", "Air Belgium" },
        new[] { "XK", "146", "CCM", "Air Corsica", "Air Corsica", "Air Corsica", "Corsica" },
        new[] { "AT", "147", "RAM", "Royal Air Maroc", "Royal Air Maroc", "Royal Air Maroc", "Royalair Maroc" },
        new[] { "LN", "148", "LAA", "Libyan Airlines", "Libyan Airlines", "Libyan Airlines", "Libair" },
        new[] { "LG", "149", "LGL", "Luxair", "Luxair", "Luxair", "Luxair" },
        new[] { "UG", "150", "TUX", "Tunisair Express", "Tunisair Express", "Tunisair Express", "Tunexpress" },
        new[] { "R5", "151", "JAV", "Jordan Aviation dba Jordan Aviation Airlines", "Jordan Enterprise for Air Navigation and Aviation", "Jordan Aviation", "Jordan Aviation" },
        new[] { "2I", "156", "CSB", "Star Up S.A. dba Star Peru", "21 Air LLC", "21 Air", "Cargo South" },
        new[] { "QR", "157", "QTR", "Qatar Airways (Q.C.S.C.)", "Qatar Airways Group Q.C.S.C.", "Qatar Airways", "Qatari" },
        new[] { "CX", "160", "CPA", "Cathay Pacific Airways Ltd.", "Cathay Pacific Airways Ltd.", "Cathay Pacific", "Cathay" },
        new[] { "MN", "161", "CAW", "Comair Ltd.", "Comair Ltd.", "Comair (South Africa)", "Commercial" },
        new[] { "OL", "162", "PAO", "Polynesian Limited dba Samoa Airways", "Samoa Airways", "Samoa Airways", "Polynesian" },
        new[] { "UM", "168", "AZW", "Air Zimbabwe (Pvt) Ltd.", "Air Zimbabwe (PVT) Ltd.", "Air Zimbabwe", "Air Zimbabwe" },
        new[] { "HR", "169", "HHN", "Hahn Air Lines GmbH", "Hahn Air Lines", "Hahn Air", "Rooster" },
        new[] { "HA", "173", "HAL", "Hawaiian Airlines, Inc.", "Hawaiian Airlines", "Hawaiian Airlines", "Hawaiian" },
        new[] { "EK", "176", "UAE", "Emirates", "Emirates", "Emirates (airline)", "Emirates" },
        new[] { "OR", "178", "TFL", "TUI Airlines Nederland B.V.", "TUI Airlines Nederlands B.V. t/a TUI fly Netherlands", "TUI fly Netherlands", "Orange" },
        new[] { "KE", "180", "KAL", "Korean Air Lines Co. Ltd.", "Korean Air", "Korean Air", "Koreanair" },
        new[] { "XR", "187", "CXI", "Touristic Aviation Services Ltd dba Corendon Airlines Europe", "Touristic Aviation Services Ltd. t/a Corendon Airlines Europe", "Corendon Airlines Europe", "Touristic" },
        new[] { "K6", "188", "KHV", "Cambodia Angkor Air t/a Cambodia Angkor Air Co., Ltd.", "Cambodia Angkor Air Ltd.", "Cambodia Angkor Air", "Cambodia Air" },
        new[] { "TY", "190", "TPC", "Air Caledonie", "Air Caledonie", "Air Calédonie", "Aircal" },
        new[] { "PY", "192", "SLM", "Surinaamse Luchtvaart Maatschappij N.V dba Surinam Airways", "Surinam Airways Ltd.", "Surinam Airways", "Surinam" },
        new[] { "IE", "193", "SOL", "Solomon Airlines Limited", "Solomon Airlines", "Solomon Airlines", "Solomon" },
        new[] { "FV", "195", "SDM", "Rossiya Airlines JSC", "Rossiya Airlines JSC", "Rossiya Airlines", "Rossiya" },
        new[] { "TC", "197", "ATC", "Air Tanzania Company Limited", "Air Tanzania Company Limited", "Air Tanzania", "Tanzania" },
        new[] { "TU", "199", "TAR", "Tunisair", "Tunisair", "Tunisair", "Tunair" },
        new[] { "SD", "200", "SUD", "Sudan Airways Co. Ltd.", "Sudan Airways Co. Ltd.", "Sudan Airways", "Sudanair" },
        new[] { "TA", "202", "TAK", "TACA International Airlines S.A.", "Transafrican Air Limited", "TACA", "Transafrican" },
        new[] { "5J", "203", "CEB", "Cebu Air, Inc dba Cebu Pacific Air", "Cebu Air, Inc. d/b/a Cebu Pacific Air", "Cebu Pacific", "Cebu Air" },
        new[] { "NH", "205", "ANA", "All Nippon Airways Co. Ltd.", "All Nippon Airways Co. Ltd.", "All Nippon Airways", "All Nippon" },
        new[] { "AG", "209", "ARU", "Arubaanse Luchtvaart Maatschappij NV dba Aruba Airlines", "Arubaanse Luchtvaart Maatschappij N.V dba Aruba Airlines", "Aruba Airlines", "Aruba" },
        new[] { "2P", "211", "GAP", "Air Philippines Corporation dba PAL Express and Airphil Express", "Air Philippines Corporation d/b/a PAL Express and Airphil Express", "PAL Express", "Airphil" },
        new[] { "PK", "214", "PIA", "Pakistan International Airlines Corporation Limited", "Pakistan International Airlines", "Pakistan International Airlines", "Pakistan" },
        new[] { "N4", "216", "NWS", "LLC \"Nord Wind\"", "LLC \"Nord Wind\"", "Nordwind Airlines", "Nordland" },
        new[] { "TG", "217", "THA", "Thai Airways International Public Company Ltd. dba Thai", "Thai Airways International Public Company Ltd.", "Thai Airways", "Thai" },
        new[] { "NF", "218", "AVN", "Air Vanuatu (Operations) Limited dba Air Vanuatu", "Air Vanuatu (Operations) Limited", "Air Vanuatu", "Air Van" },
        new[] { "YN", "219", "CRQ", "Air Creebec (1994) Inc.", "Air Creebec (1994), Inc.", "Air Creebec", "Cree" },
        new[] { "LH", "220", "DLH", "Deutsche Lufthansa AG", "Deutsche Lufthansa AG", "Lufthansa", "Lufthansa" },
        new[] { "UK", "228", "VTI", "TATA SIA AIRLINES LTD dba VISTARA", "TATA SIA Airlines Limited t/a Vistara", "Vistara", "Vistara" },
        new[] { "KU", "229", "KAC", "Kuwait Airways", "Kuwait Airways", "Kuwait Airways", "Kuwaiti" },
        new[] { "CM", "230", "CMP", "Compania Panamena de Aviacion, S.A. (COPA)", "Copa Airlines, Inc", "Copa Airlines", "COPA" },
        new[] { "MH", "232", "MAS", "Malaysia Airlines Berhad dba Malaysia Airlines", "Malaysia Airlines Berhad d/b/a Malaysia Airlines", "Malaysia Airlines", "Malaysian" },
        new[] { "TK", "235", "THY", "Turkish Airlines Inc.", "Turkish Airlines, Inc.", "Turkish Airlines", "Turkish" },
        new[] { "QB", "237", "QSM", "Qeshm Air", "Qeshm Air", "Qeshm Air", "Qeshm Air" },
        new[] { "IZ", "238", "AIZ", "Arkia Israeli Airlines Ltd", "Arkia Israeli Airlines Ltd.", "Arkia", "Arkia" },
        new[] { "MK", "239", "MAU", "Air Mauritius Ltd", "Air Mauritius", "Air Mauritius", "Airmauritius" },
        new[] { "GU", "240", "GUG", "AVIATECA, S.A.", "AVIATECA, S.A.", "Avianca Guatemala", "AVIATECA" },
        new[] { "EL", "241", "ELB", "Ellinair S.A", "Ellinair S.A.", "Ellinair", "ELLINAIR HELLAS" },
        new[] { "TN", "244", "THT", "Air Tahiti Nui", "Air Tahiti Nui", "Air Tahiti Nui", "Tahiti Airlines" },
        new[] { "HY", "250", "UZB", "Uzbekistan Airways", "Uzbekistan Havo Yullary", "Uzbekistan Airways", "Uzbek" },
        new[] { "FG", "255", "AFG", "Ariana Afghan Airlines dba Ariana Afghan Airlines", "Ariana Afghan Airlines", "Ariana Afghan Airlines", "Ariana" },
        new[] { "OS", "257", "AUA", "Austrian Airlines AG dba Austrian", "Austrian Airlines AG d/b/a/ Austrian", "Austrian Airlines", "Austrian" },
        new[] { "MD", "258", "MDG", "Air Madagascar", "Air Madagascar", "Air Madagascar", "Air Madagascar" },
        new[] { "FJ", "260", "FJI", "Air Pacific Limited t/a Fiji Airway", "Air Pacific Ltd. t/a Fiji Airways", "Fiji Airways", "Fiji" },
        new[] { "U6", "262", "SVR", "Joint Stock Company \"Ural Airlines\"", "Ural Airlines", "Ural Airlines", "Sverdlovsk Air" },
        new[] { "GP", "275", "RIV", "APG Airlines", "APG Airlines", "APG Airlines", "Riviera" },
        new[] { "J7", "277", "ABS", "Afrijet Business Service dba Afrijet", "Afrijet Business Services d/b/a Afrijet", "Afrijet Business Service", "AFRIJET" },
        new[] { "RO", "281", "ROT", "COMPANIA NATIONALA DE TRANSPORTURI AERIENE ROMANE TAROM S.A.", "Compnia Nationala de Transporturi Aeriene Romane TAROM S.A.", "TAROM", "Tarom" },
        new[] { "RA", "285", "RNA", "Nepal Airlines Corporation", "Nepal Airlines Corporation t/a Nepal Airlines", "Nepal Airlines", "Royal Nepal" },
        new[] { "4N", "287", "ANT", "Air North Charter and Training Ltd.", "Air North Charter and Training Ltd. t/a Air North", "Air North", "Air North" },
        new[] { "OM", "289", "MGL", "MIAT Mongolian Airlines", "MIAT", "MIAT Mongolian Airlines", "Mongol Air" },
        new[] { "WG", "292", "SWG", "Sunwing Airlines Inc.", "Sunwing Airlines Inc", "Sunwing Airlines", "SUNWING" },
        new[] { "WM", "295", "WIA", "Windward Islands Airways Int'l N.V. dba WINAIR", "Windward Islands Airways International N.V.", "Winair", "Windward" },
        new[] { "CI", "297", "CAL", "China Airlines Ltd.", "China Airlines", "China Airlines", "Dynasty" },
        new[] { "UT", "298", "UTA", "UTair Aviation", "Utair Aviation Joint-Stock Company", "Utair", "Tjumavi" },
        new[] { "5S", "301", "GAK", "Global Air Transport", "Global Aviation Services Group", "Global Aviation and Services Group", "AVIAGROUP" },
        new[] { "ZW", "303", "AWI", "Air Wisconsin Airlines Corporation (AWAC)", "Air Wisconsin Airlines Corporation (AWAC)", "Air Wisconsin", "Wisconsin" },
        new[] { "C2", "304", "CEL", "CEIBA Intercontinental", "Ceiba Intercontinental", "CEIBA Intercontinental", "Ceiba Line" },
        new[] { "V0", "308", "VCV", "CONVIASA", "Conviasa", "Conviasa", "CONVIASA" },
        new[] { "6E", "312", "IGO", "Interglobe Aviation Ltd. dba Indigo", "Interglobe Aviation Ltd. d/b/a Indigo", "IndiGo", "Ifly" },
        new[] { "5N", "316", "AUL", "Joint Stock Company Smartavia Airlines", "Joint Stock Company Smartavia Airlines", "Smartavia", "Dvina" },
        new[] { "SC", "324", "CDG", "Shandong Airlines", "Shandong Airlines", "Shandong Airlines", "Shandong" },
        new[] { "NP", "325", "NIA", "Nile Air", "Nile Air", "Nile Air", "Nile Bird" },
        new[] { "D8", "329", "IBK", "Norwegian Air International LTD.", "Norwegian Air International Ltd.", "Norwegian Air International", "Nortrans" },
        new[] { "S4", "331", "RZO", "SATA  Internacional - Azores Airlines, S.A.", "SATA International Servicos e Transportes Aereos, S.A. t/a Azores Airlines", "Azores Airlines", "Air Azores" },
        new[] { "VB", "333", "VIV", "Aeroenlaces Nacionales S.A. de C.V.", "Aeroenlaces Nacionales, S.A. de C.V. t/a viva aerobus", "VivaAerobús", "AEROENLACES" },
        new[] { "SY", "337", "SCX", "MN Airlines LLC", "MN Airlines, LLC d/b/a Sun Country Airlines", "Sun Country Airlines", "Sun Country" },
        new[] { "0V", "338", "VFC", "Vietnam Air Service Company (VASCO)", "Vietnam Air Service Company", "Vietnam Air Services Company", "Vasco Air" },
        new[] { "NU", "353", "JTA", "Japan Transocean Air Co. Ltd.", "Japan Transocean Air Co. Ltd.", "Japan Transocean Air", "Jai Ocean" },
        new[] { "J4", "367", "BDR", "Badr Airlines", "Badr Airlines", "Badr Airlines", "Badr Air" },
        new[] { "N3", "370", "VOS", "Vuela El Salvador S.A. de C.V.", "Veula El Salvador S.A. de C.V.", "Volaris El Salvador", "Jetsal" },
        new[] { "AP", "374", "LAV", "Alba Star,S.A. dba Alba Star", "Alba Star, S.A. d/b/a Alba Star.es", "AlbaStar", "Albastar" },
        new[] { "KX", "378", "CAY", "Cayman Airways Limited", "Cayman Airways Limited", "Cayman Airways", "Cayman" },
        new[] { "SM", "381", "MSC", "Air Cairo", "Air Cairo", "Air Cairo", "AIR CAIRO" },
        new[] { "RQ", "384", "KMF", "Kam Air dba Kam Air", "Kam Air", "Kam Air", "Kamgar" },
        new[] { "A6", "389", "OTC", "Air Travel Co. Ltd.", "Air Travel Co., Ltd.", "Air Travel (airline)", "Air Travel" },
        new[] { "A3", "390", "AEE", "Aegean Airlines", "Aegean Airlines, S.A.", "Aegean Airlines", "Aegean" },
        new[] { "BF", "396", "FBU", "French Bee dba French Bee", "French bee", "French Bee", "French Bee" },
        new[] { "SZ", "413", "SMR", "Aircompany Somon Air LLC dba Somon Air", "Aircompany Somon Air LLC", "Somon Air", "Somon Air" },
        new[] { "N8", "416", "NCR", "National Air Cargo Group, Inc. dba National Airlines", "National Air Cargo Group, Inc. d/b/a National Airlines", "National Airlines (N8)", "National Cargo" },
        new[] { "S7", "421", "SBI", "JSC Siberia Airlines dba S7 airlines", "JSC Siberia Airlines d/b/a S7 Airlines", "S7 Airlines", "Siberian Airlines" },
        new[] { "F9", "422", "FFT", "Frontier Airlines, Inc.", "Frontier Airlines, Inc.", "Frontier Airlines", "Frontier Flight" },
        new[] { "TX", "427", "FWI", "Air Caraibes", "Air Caraibes", "Air Caraïbes", "French West" },
        new[] { "9E", "430", "EDV", "Endeavor Air", "Endeavor Air, Inc.", "Endeavor Air", "Endeavor" },
        new[] { "P6", "433", "PSC", "Privilege Style S.A.", "9736140 Canada Inc t/a Pascan", "Pascan Aviation", "Pascan" },
        new[] { "RM", "437", "NGT", "Aircompany Armenia LLC", "Aircompany Armenia LLC", "Aircompany Armenia", "Nika" },
        new[] { "ZP", "445", "AZP", "Compania de Aviacion Paraguaya S.A dba Paranair", "Compañía de Aviación Paraguaya S.A. d/b/a Paranair", "Paranair", "Guarani" },
        new[] { "3M", "449", "SIL", "Silver Airways Corp", "Silver Airways Corp.", "Silver Airways", "Silver Wings" },
        new[] { "PD", "451", "POE", "Porter Airlines Inc.", "Porter Airlines Inc.", "Porter Airlines", "PORTER" },
        new[] { "3O", "452", "MAC", "Air Arabia Maroc", "Air Arabia Maroc", "Air Arabia Maroc", "Arabia Maroc" },
        new[] { "YX", "453", "RPA", "Republic Airways Inc", "Republic Airline, Inc.", "Republic Airways", "Brickyard" },
        new[] { "Z2", "457", "APG", "Philippines AirAsia, INC. dba AirAsia", "Philippines AirAsia Inc. t/a AirAsia Philippines", "Philippines AirAsia", "Cool Red" },
        new[] { "WB", "459", "RWD", "RwandAir  Limited", "RwandAir Limited", "RwandAir", "Rwandair" },
        new[] { "EB", "460", "PLM", "WAMOS AIR, S.A.", "Wamos Air, S.A.", "Wamos Air", "Pullman" },
        new[] { "7W", "461", "WRC", "Wind Rose Aviation Company", "Wind Rose Aviation Company Ltd t/a WINDROSE Airlines", "Windrose Airlines", "Wind Rose" },
        new[] { "XL", "462", "LNE", "Aerolane Lineas Aerea Nacional dba Latam Airlines Ecuador", "Aerolane", "LATAM Airlines Ecuador", "LAN Ecuador" },
        new[] { "KC", "465", "KZR", "JSC AIR ASTANA", "Air Astana", "Air Astana", "Astanaline" },
        new[] { "3H", "466", "AIE", "Air Inuit Ltd/Ltee", "Air Inuit Ltd/Ltee.", "Air Inuit", "Inuit" },
        new[] { "0B", "475", "BLA", "Blue Air Aviation S.A.", "Blue Air Aviation S.A.", "Blue Air", "Blue Air" },
        new[] { "Y7", "476", "TYA", "Joint-stock company NordStar Airlines dba NORDSTAR", "Joint-stock company NordStar Airlines d/b/a NordStar", "NordStar", "Taimyr" },
        new[] { "ZH", "479", "CSZ", "Shenzhen Airlines", "Shenzhen Airlines", "Shenzhen Airlines", "Shenzhen Air" },
        new[] { "QX", "481", "QXE", "Horizon Air Industries, Inc.", "Horizon Air Industries, Inc.", "Horizon Air", "Horizon Air" },
        new[] { "J9", "486", "JZR", "Jazeera Airways", "Jazeera Airways", "Jazeera Airways", "Jazeera" },
        new[] { "NK", "487", "NKS", "Spirit Airlines", "Spirit Airlines, Inc.", "Spirit Airlines", "Spirit Wings" },
        new[] { "B9", "491", "IRB", "Iran Airtour Airline", "Iran Airtour Airline", "Iran Airtour", "IRAN AIRTOUR" },
        new[] { "Y5", "509", "GMR", "Golden Myanmar Airlines Public Co. Ltd dbaGolden Myanmar Airlines Public Co.Ltd", "Golden Myanmar Airlines Public Co., Ltd", "Golden Myanmar Airlines", "Golden Myanmar" },
        new[] { "YK", "511", "AVJ", "Avia Traffic Company LLC dba Avia Traffic Company LLC", "Avia Traffic Company LLC", "Avia Traffic Company", "Atomic" },
        new[] { "RJ", "512", "RJA", "Alia - The Royal Jordanian Airlines dba Royal Jordanian", "Alia", "Royal Jordanian", "Jordanian" },
        new[] { "G9", "514", "ABY", "Air Arabia dba Air Arabia", "Air Arabia PJSC", "Air Arabia", "Arabia" },
        new[] { "5T", "518", "AKT", "Canadian North Inc.", "Canadian North Inc.", "Canadian North", "Arctic" },
        new[] { "B7", "525", "UIA", "UNI Airways Corporation", "UNI Airways Corporation", "Uni Air", "Glory" },
        new[] { "WN", "526", "SWA", "Southwest Airlines Co.", "Southwest Airlines Co.", "Southwest Airlines", "Southwest" },
        new[] { "3W", "529", "MWI", "Malawi Airlines", "Malawian Airlines", "Malawi Airlines", "MALAWIAN" },
        new[] { "YV", "533", "ASH", "Mesa Airlines, Inc.", "Mesa Airlines, Inc.", "Mesa Airlines", "Air Shuttle" },
        new[] { "W5", "537", "IRM", "Mahan Air", "Mahan Air", "Mahan Air", "Mahan Air" },
        new[] { "5H", "540", "FFV", "Five Forty Aviation Ltd dba Five Forty Aviation Ltd", "Five Fourty Aviation Limited", "Fly540", "Swift Tango" },
        new[] { "T5", "542", "TUA", "Turkmenistan Airlines dba Turkmenistan Airlines", "Turkmenistan Airlines", "Turkmenistan Airlines", "Turkmenistan" },
        new[] { "LP", "544", "LPE", "LATAM Airlines Peru S.A.", "Lan Peru, S.A. d/b/a LATAM Airlines Peru", "LATAM Perú", "Lanperu" },
        new[] { "8U", "546", "AAW", "Afriqiyah Airways", "Afriqiyah Airways", "Afriqiyah Airways", "Afriqiyah" },
        new[] { "2K", "547", "GLG", "AVIANCA-Ecuador dba AVIANCA", "Avianca Ecuador d/b/a Avianca", "Avianca Ecuador", "Galapagos" },
        new[] { "BL", "550", "PIC", "Pacific Airlines / Pacific Airlines Aviation JSC", "Pacific Airlines Co. Ltd.", "Pacific Airlines", "Pacific Express" },
        new[] { "YU", "551", "MMZ", "EuroAtlantic Airways", "EuroAtlantic Airways Transportes Aereos, S.A.", "EuroAtlantic Airways", "Euroatlantic" },
        new[] { "SU", "555", "AFL", "PJSC Aeroflot", "PJSC \"Aeroflot\"", "Aeroflot", "Aeroflot" },
        new[] { "5O", "558", "FPO", "ASL AIRLINES FRANCE", "ASL Airlines France S.A.", "ASL Airlines France", "French Post" },
        new[] { "XQ", "564", "SXS", "SunExpress", "SunExpress", "SunExpress", "Sunexpress" },
        new[] { "PS", "566", "AUI", "Private Stock Company \"Ukraine International Airlines\" dba UIA", "Private Stock Company \"Ukraine International Airlines\"", "Ukraine International Airlines", "Ukraine International" },
        new[] { "9D", "568", "NMG", "Genghis Khan Airlines Co., Ltd", "Genghis Khan Airlines Co., Ltd.", "Genghis Khan Airlines", "Tianjiao Air" },
        new[] { "9U", "572", "MLD", "Air Moldova", "Air Moldova", "Air Moldova", "Air Moldova" },
        new[] { "G7", "573", "GJS", "GoJet Airlines LLC", "GoJet Airlines LLC", "GoJet Airlines", "Lindbergh" },
        new[] { "AD", "577", "AZU", "Azul Linhas Aereas Brasileiras", "Azul Linhas Aereas Brasileiras", "Azul Brazilian Airlines", "Azul" },
        new[] { "5B", "590", "BSX", "Bassaka Air Limited dba Bassaka Air", "Bassaka Air Limited d/b/a Bassaka Air", "Bassaka Air", "Bassaka" },
        new[] { "XY", "593", "KNE", "Flynas Company Closed Joint Stock owned by (NAS Holding) JSC", "Flynas Company Closed Joint Stock owned by (NAS Holding) JSC", "Flynas", "Nas Express" },
        new[] { "DD", "596", "NOK", "Nok Airlines Public Company Limited dba Nok Air", "Nok Airlines Public Co., Ltd. d/b/a Nok Air", "Nok Air", "Nok Air" },
        new[] { "HZ", "598", "SHU", "Joint-Stock Company Aurora Airlines", "Joint- Stock Company \"Aurora Airlines\"", "Aurora (airline)", "Aurora" },
        new[] { "8M", "599", "MMA", "Myanmar Airways International Company Limited", "Myanmar Airways International Company Ltd.", "Myanmar Airways International", "Myanmar" },
        new[] { "UL", "603", "ALK", "SriLankan Airlines Limited", "SriLankan Airlines Limited", "SriLankan Airlines", "Srilankan" },
        new[] { "H2", "605", "SKU", "Sky Airline S.A.", "Sky Airline S.A.", "Sky Airline", "Aerosky" },
        new[] { "A9", "606", "TGZ", "Georgian Airways", "Georgian Airways", "Georgian Airways", "Tamazi" },
        new[] { "EY", "607", "ETD", "Etihad Airways dba Etihad", "Etihad Airways d/b/a Etihad", "Etihad Airways", "Etihad" },
        new[] { "IT", "608", "TTW", "Tigerair Taiwan Co. Ltd", "Tiger Airways Taiwan Co. Ltd.", "Tigerair Taiwan", "Smart Cat" },
        new[] { "TB", "612", "JAF", "TUI Airlines Belgium N.V dba TUI fly", "TUI Airlines Belgium NV t/a TUI fly Belgium", "TUI fly Belgium", "Beauty" },
        new[] { "6A", "616", "AMW", "Armenia Airways Air Company CJSC", "Armenia Airways Air company CJSC", "Armenia Airways", "ARMENIA" },
        new[] { "X3", "617", "TUI", "TUIfly GmbH", "TUIfly GmbH", "TUI fly Deutschland", "Tuifly" },
        new[] { "SQ", "618", "SIA", "Singapore Airlines Limited", "Singapore Airlines Limited", "Singapore Airlines", "Singapore" },
        new[] { "Q6", "621", "VOC", "Vuela Aviacion S.A Volaris Costa Rica", "Vuela Aviacion S.A. (Volaris Costa Rica)", "Volaris Costa Rica", "Costa Rican" },
        new[] { "MO", "622", "CAV", "Calm Air International Ltd.", "Calm Air International Ltd.", "Calm Air", "Calm Air" },
        new[] { "FB", "623", "LZB", "Bulgaria Air JSC", "Bulgaria Air", "Bulgaria Air", "Flying Bulgaria" },
        new[] { "PC", "624", "PGT", "Pegasus Hava Tasimaciligi A.S.", "Pegasus Hava Tasimaciligi A.S.", "Pegasus Airlines", "Sunturk" },
        new[] { "CG", "626", "TOK", "PNG Air Limited", "PNG Air Limited.", "PNG Air", "Balus" },
        new[] { "QV", "627", "LAO", "Lao Airlines", "Lao Airlines", "Lao Airlines", "Lao" },
        new[] { "B2", "628", "BRU", "Belavia - Belarusian Airlines dba Belavia", "Belavia", "Belavia", "Belavia" },
        new[] { "MI", "629", "SLK", "SilkAir (SINGAPORE) Pte. Ltd.", "SilkAir (SINGAPORE) Pte. Ltd.", "SilkAir", "Silkair" },
        new[] { "GL", "631", "GRL", "Air Greenland A/S", "Air Greenland A/S", "Air Greenland", "Greenland" },
        new[] { "JV", "632", "BLS", "Perimeter Aviation LP dba Bearskin Airlines", "Perimeter Aviation LP d/b/a Bearskin Airlines", "Bearskin Airlines", "Bearskin" },
        new[] { "9M", "634", "GLR", "Central Mountain Air Ltd.", "Central Mountain Air Ltd.", "Central Mountain Air", "Glacier" },
        new[] { "IY", "635", "IYE", "Yemenia - Yemen Airways", "Yemenia", "Yemenia", "Yemeni" },
        new[] { "BP", "636", "BOT", "Air Botswana", "Air Botswana Corporation", "Air Botswana", "Botswana" },
        new[] { "B8", "637", "ERT", "Eritrean Airlines s.c. dba Eritrean Airlines s.c.", "Eritrean Airlines", "Eritrean Airlines", "Eritrean" },
        new[] { "PJ", "638", "SPM", "Air Saint  Pierre", "Air Saint Pierre", "Air Saint-Pierre", "SAINT-PIERRE" },
        new[] { "8Y", "639", "AAV", "Astro Air International Inc. dba PAN PACIFIC AIRLINES", "Astro Air International Inc. d/b/a Pan Pacific Airlines", "Pan Pacific Airlines", "Astro-Phil" },
        new[] { "FA", "640", "SFR", "Safair Operations (Proprietary) Ltd dba Safair", "Safair Operations (Proprietary) Ltd. dba Safair", "Safair", "Cargo" },
        new[] { "5D", "642", "SLI", "Aerolitoral S.A. de C.V.", "Aerolitoral, S.A. de C.V., d/b/a Aerom�xico Connect", "Aeroméxico Connect", "Costera" },
        new[] { "KM", "643", "AMC", "Air Malta p.l.c.", "Air Malta p.l.c.", "Air Malta", "Air Malta" },
        new[] { "TS", "649", "TSC", "Air Transat", "Air Transat", "Air Transat", "Transat" },
        new[] { "DV", "655", "VSV", "JSC Aircompany SCAT", "PLL Scat Aircompany", "SCAT Airlines", "Vlasta" },
        new[] { "PX", "656", "ANG", "Air Niugini Pty Limited dba Air Niugini", "Air Niugini Pty Limited d/b/a Air Niugini", "Air Niugini", "Niugini" },
        new[] { "BT", "657", "BTI", "Air Baltic Corporation A/S", "Air Baltic Corporation S/A", "airBaltic", "Airbaltic" },
        new[] { "GY", "661", "CGZ", "Colorful GuiZhou Airlines Co., Ltd", "Colorful GuiZhou Airlines Co., Ltd", "Colorful Guizhou Airlines", "Colorful" },
        new[] { "PU", "663", "PUE", "Plus Ultra Lineas Aereas, S. A.", "Plus Ultra Lineas Aereas, S.A.", "Plus Ultra Líneas Aéreas", "Spanish" },
        new[] { "YC", "664", "LLM", "Joint-Stock Company \"Yamal Airlines", "Yamal Airlines", "Yamal Airlines", "Yamal" },
        new[] { "FU", "666", "FZA", "Fuzhou Airlines Co., Ltd", "Fuzhou Airlines Co., Ltd.", "Fuzhou Airlines", "Strait Air" },
        new[] { "MJ", "669", "MYW", "Myway Airlines Co.,LTD", "MyWay Airlines Co., Ltd.", "MyWay Airlines", "My Sky" },
        new[] { "BI", "672", "RBA", "Royal Brunei Airlines Sdn. Bhd. dba Royal Brunei Airlines", "Royal Brunei Airlines Sdn. Bhd.", "Royal Brunei Airlines", "Brunei" },
        new[] { "NX", "675", "AMU", "Air Macau Company Limited", "Air Macau Company Limited", "Air Macau", "Air Macau" },
        new[] { "LM", "682", "LOG", "Loganair Limited", "Loganair Limited", "Loganair", "Logan" },
        new[] { "CL", "683", "CLH", "Lufthansa CityLine GmbH", "Lufthansa CityLine", "Lufthansa CityLine", "Hansaline" },
        new[] { "NI", "685", "PGA", "Portugalia - Companhia Portuguesa de Transportes Aereos SA", "Portugalia", "PGA-Portugalia Airlines", "Portugalia" },
        new[] { "WX", "689", "BCY", "Cityjet", "Cityjet", "CityJet", "City-Ireland" },
        new[] { "PZ", "692", "LAP", "Transportes Aereos del Mercosur S.A dba LATAM Airlines Paraguay", "TAM", "LATAM Airlines Paraguay", "Paraguaya" },
        new[] { "YW", "694", "ANE", "Air Nostrum", "Air Nostrum Lineas Aereas del Mediterraneo, S.A.", "Air Nostrum", "Nostru Air" },
        new[] { "BR", "695", "EVA", "EVA Airways Corporation", "EVA Airways Corporation", "EVA Air", "Eva" },
        new[] { "VR", "696", "TCV", "Transportes Aereos de Cabo Verde dba TACV Cabo Verde Airlines", "Transportes Aereos de Cabo Verde (TACV) d/b/a Cabo Verde Airlines", "Cabo Verde Airlines", "Caboverbe" },
        new[] { "WF", "701", "WIF", "Wideroe's Flyveselskap A.S.", "Wideroe's Flyveselskap A/S", "Widerøe", "Wideroe" },
        //new[] { "NO", "703", "NAB", "Neos S.P.A.", "Niger Air Cargo", "Neos", "Niger Cargo" },
        new[] { "KQ", "706", "KQA", "Kenya Airways PLC", "Kenya Airways", "Kenya Airways", "Kenya" },
        new[] { "P4", "710", "APK", "Air Peace Limited", "Air Peace Limited", "Air Peace", "Peace Bird" },
        new[] { "LX", "724", "SWR", "SWISS International Air Lines Ltd dba SWISS", "Swiss International Airlines Ltd. d/b/a Swiss", "Swiss International Air Lines", "Swiss" },
        new[] { "GT", "730", "CGH", "Air Guilin Co. Ltd.", "Air Guilin Co., Ltd.", "Air Guilin", "Welkin" },
        new[] { "MF", "731", "CXA", "Xiamen Airlines", "Xiamen Airlines", "XiamenAir", "Xiamen Air" },
        new[] { "ZD", "732", "EWR", "EWA Air", "EWA Air", "Ewa Air", "Mayotte Air" },
        new[] { "SP", "737", "SAT", "SATA (Air Acores)", "SATA", "SATA Air Açores", "SATA" },
        new[] { "VN", "738", "HVN", "Vietnam Airlines JSC dba Vietnam Airlines JSC", "Vietnam Airlines Corporation", "Vietnam Airlines", "Vietnam Airlines" },
        new[] { "9V", "742", "ROI", "Avior Airlines, C.A. dba Avior Airlines", "Aviones de Oriente, C.A. (AVIOR)", "Avior Airlines", "Avior" },
        new[] { "3N", "746", "URG", "Air Urga", "Air Urga", "Air Urga", "Urga" },
        new[] { "4Z", "749", "LNK", "SA Airlink (PTY) LTD dba South African Airlink", "Airlink (Pty) Ltd.", "Airlink", "Link" },
        new[] { "RL", "750", "ABG", "JSC Royal Flight Airlines", "CJSC Royal Flight Airlines", "Royal Flight (airline)", "Royal Flight" },
        new[] { "3V", "756", "TAY", "ASL Airlines Belgium", "ASL Airlines Belgium", "ASL Airlines Belgium", "Quality" },
        new[] { "F7", "757", "RSY", "LTD I Fly", "The Airline iFly", "I-Fly", "Russian Sky" },
        new[] { "VU", "759", "VAG", "Vietravel Airlines Co., Ltd.", "Vietravel Airlines Co., Ltd.", "Vietravel Airlines", "Vietravel Air" },
        new[] { "UU", "760", "REU", "Air Austral", "Air Austral", "Air Austral", "Reunion" },
        new[] { "QU", "761", "UTN", "Azur Air Ukraine Airlines LLC", "Azur Air Ukraine Airlines LLC", "Azur Air Ukraine", "UT-Ukraine" },
        new[] { "DI", "762", "NRS", "Norwegian Air UK ltd", "Norwegian Air UK Ltd.", "Norwegian Air UK", "Rednose" },
        new[] { "RC", "767", "FLI", "Atlantic Airways, Faroe Islands, P/F", "Atlantic Airways Faroe Islands", "Atlantic Airways", "Faroeline" },
        new[] { "H9", "769", "HIM", "Himalaya Airlines Pvt. Ltd.", "Himalaya Airlines Pvt. Ltd.", "Himalaya Airlines", "Himalaya" },
        new[] { "EO", "770", "KAR", "LLC \"Aircompany \"Ikar\" dba Pegas Fly", "Ikar, LLC t/a Pegas Fly", "Pegas Fly", "Krasjet" },
        new[] { "J2", "771", "AHY", "Azerbaijan Hava Yollary", "Azerbaijan Airlines CJSC", "Azerbaijan Airlines", "Azal" },
        new[] { "FM", "774", "CSH", "Shanghai Airlines Co. Ltd.", "Shanghai Airlines Co. Ltd.", "Shanghai Airlines", "Shanghai Air" },
        new[] { "SG", "775", "SEJ", "SpiceJet Ltd.", "SpiceJet Ltd", "SpiceJet", "Spicejet" },
        new[] { "MU", "781", "CES", "China Eastern Airlines", "China Eastern Airlines", "China Eastern", "China Eastern" },
        new[] { "QL", "782", "LER", "Linea Aerea De Servicio Ejecutivo Regional, C.A. dba Laser", "Linea Aerea de Servicio Ejecutivo Regional, C.A. d/b/a Laser", "LASER Airlines", "Laser" },
        new[] { "E9", "783", "EVE", "Evelop Airlines S.L.", "Evelop Airlines S.L. d/b/a iberojet", "Iberojet (airline)", "Evelop" },
        new[] { "CZ", "784", "CSN", "China Southern Airlines", "China Southern Airlines", "China Southern Airlines", "China Southern" },
        new[] { "B3", "786", "BTN", "Bhutan Airlines dba Tashi Air Pvt Ltd.", "Tashi Air Private Ltd. t/a Bhutan Airlines", "Bhutan Airlines", "Bhutan Air" },
        new[] { "KB", "787", "DRK", "Druk Air Corporation Ltd.", "Druk Air Corporation Ltd.", "Druk Air", "Royal Bhutan" },
        new[] { "VA", "795", "VOZ", "Virgin Australia International Airlines Pty Ltd", "Virgin Australia International Airlines Pty Ltd", "Virgin Australia", "Velocity" },
        new[] { "BJ", "796", "LBT", "Nouvelair Tunisie", "Nouvelair Tunisie", "Nouvelair", "Nouvelair" },
        new[] { "QS", "797", "TVS", "Smartwings, a.s.", "Smartwings, a.s.", "Smartwings", "Skytravel" },
        new[] { "AE", "803", "MDA", "Mandarin Airlines Ltd.", "Mandarin Airlines Ltd.", "Mandarin Airlines", "Mandarin" },
        new[] { "AK", "807", "AXM", "AirAsia Berhad dba AirAsia", "Air Asia Sen Bhd. t/a Air Asia", "AirAsia", "Red Cap" },
        new[] { "EU", "811", "UEA", "Chengdu Airlines", "Chengdu Airlines", "Chengdu Airlines", "United Eagle" },
        new[] { "EP", "815", "IRC", "Iran Aseman Airlines", "Iran Aseman Airline", "Iran Aseman Airlines", "Aseman" },
        new[] { "RS", "820", "ASV", "Air Seoul, Inc", "Air Seoul, Inc.", "Air Seoul", "Air Seoul" },
        new[] { "KN", "822", "CUA", "China United Airlines", "China United Airlines", "China United Airlines", "Lianhang" },
        new[] { "GS", "826", "GCR", "TianJin Airlines Co. Ltd", "Tianjin Airlines Co. Ltd", "Tianjin Airlines", "Bo Hai" },
        new[] { "PG", "829", "BKP", "Bangkok Airways Public Co., Ltd.", "Bangkok Airways Co. Ltd.", "Bangkok Airways", "Bangkok Air" },
        new[] { "OU", "831", "CTN", "Croatia Airlines", "Croatia Airlines", "Croatia Airlines", "Croatia" },
        new[] { "KY", "833", "KNA", "Kunming Airlines Co., Ltd.", "Kunming Airlines Co., Ltd.", "Kunming Airlines", "Kunming Air" },
        new[] { "NS", "836", "HBH", "Hebei Airlines Co., Ltd.", "Hebei Airlines Co., Ltd.", "Hebei Airlines", "Hebei Air" },
        new[] { "WS", "838", "WJA", "WestJet", "Westjet", "WestJet", "Westjet" },
        new[] { "C5", "841", "UCA", "Champlain Enterprises Inc. dba Commutair", "Champlain Enterprises, Inc.", "CommutAir", "Commutair" },
        new[] { "D7", "843", "XAX", "Airasia X Berhad dba Airasia X", "Airasia X Berhad d/b/a Airasia X", "AirAsia X", "Xanadu" },
        new[] { "E5", "844", "RBG", "Air Arabia Egypt", "Air Arabia Egypt", "Air Arabia Egypt", "Arabia Egypt" },
        new[] { "P5", "845", "RPB", "Aero Republica S.A.", "AeroRep�blica, S.A. t/a Copa Airlines Colombia", "Aero Republica", "Aerorepublica" },
        new[] { "PN", "847", "CHB", "West Air Co., Ltd.", "China West Air Ltd.", "West Air (China)", "West China" },
        new[] { "R3", "849", "SYL", "Joint Stock Company Aircompany \"Yakutia\"", "JSC Airline Yakutia", "Yakutia Airlines", "Air Yakutia" },
        new[] { "HX", "851", "CRK", "Hong Kong Airlines Limited", "Hong Kong Airlines Limited", "Hong Kong Airlines", "Bauhinia" },
        new[] { "TZ", "852", "TDS", "Tsaradia dba Tsaradia", "Tsaradia", "Tsaradia", "Tsaradia" },
        new[] { "9H", "856", "CGN", "Changan Airlines Limited Company", "Changan Airlines Limited Company t/a Air Changan", "Air Changan", "Chang an" },
        new[] { "8L", "859", "LKE", "Lucky Air Co. Ltd.", "Lucky Air Co. Ltd", "Lucky Air", "Lucky Air" },
        new[] { "MR", "861", "MML", "Hunnu Air LLC", "Hunnu Air", "Hunnu Air", "Trans Mongolia" },
        new[] { "BK", "866", "OKA", "Okay Airways Company Limited", "Okay Airways Company Limited", "Okay Airways", "Okayjet" },
        new[] { "LT", "867", "SNG", "LongJiang Airlines Co.,Ltd.", "LongJiang Airlines Co., Ltd.", "LJ Air", "Snow Eagle" },
        new[] { "Y8", "871", "YZR", "Suparna Airlines Co., Ltd.", "Suparna Airlines Co., Ltd.", "Suparna Airlines", "Yangtze River" },
        new[] { "GX", "872", "CBG", "Guangxi Beibu Gulf Airlines Co.,Ltd", "Guangxi Beibu Gulf Airlines Co., Ltd.", "GX Airlines", "Green City" },
        new[] { "H4", "874", "HYS", "Seven Four Eight Air Services(K)Limited", "HiSky Europe", "HiSky", "Sky Europe" },
        new[] { "3U", "876", "CSC", "Sichuan Airlines Co. Ltd.", "Sichuan Airlines Co. Ltd.", "Sichuan Airlines", "Sichuan" },
        new[] { "OQ", "878", "CQN", "Chongqing Airlines Co. Ltd", "Chong Qing Airlines Co., Ltd", "Chongqing Airlines", "Chong Qing" },
        new[] { "G8", "879", "GOW", "Go Airlines (India) Limited", "Go Airlines (India) Pvt. Ltd.", "Go First", "goair" },
        new[] { "HU", "880", "CHH", "Hainan Airlines Holding Company Limited", "Hainan Airlines Holding Company Limited", "Hainan Airlines", "Hainan" },
        new[] { "DE", "881", "CFG", "Condor Flugdienst GmbH", "Condor Flugdienst GmbH", "Condor (airline)", "Condor" },
        new[] { "UQ", "886", "CUH", "Urumqi Airlines Co. Ltd.", "Urumqi Airlines Co. Ltd.", "Urumqi Air", "Loulan" },
        new[] { "GJ", "891", "CDC", "Zhejiang Loong Airlines Co., Ltd", "Zhejiang Loong Airlines Co., Ltd.", "Loong Air", "Loong Air" },
        new[] { "DZ", "893", "EPA", "Donghai Airlines Co., Ltd", "Shenzhen Donghai Airlines Co. Ltd.", "Donghai Airlines", "Donghai Air" },
        new[] { "CN", "895", "GDC", "Grand China Air Co. , Ltd.", "Grand China Air Co., Ltd.", "Grand China Air", "Grand China" },
        new[] { "JD", "898", "CBJ", "Beijing Capital Airlines Co. Ltd.", "Beijing Capital Airlines Co., Ltd", "Beijing Capital Airlines", "Capital Jet" },
        new[] { "ZL", "899", "RXA", "REGIONAL EXPRESS PTY LIMITED", "Australiawide Airlines Ltd. d/b/a Regional Express", "Rex Airlines", "Rex" },
        new[] { "FD", "900", "AIQ", "Thai AirAsia Co., Ltd.", "Thai AirAsia Co. Ltd.", "Thai AirAsia", "Thai Asia" },
        new[] { "AQ", "902", "JYH", "9 Air Co Ltd", "Nine Air Co., Ltd.", "9 Air", "Trans Jade" },
        new[] { "4D", "903", "ASD", "Egypt Air Holding Company dba Air Sinai", "Egypt Air Holding Company d/b/a Air Sinai", "Air Sinai", "Air Sinai" },
        new[] { "WE", "909", "THD", "Thai Smile Airways Company Limited", "Thai Smile Airways Company Limited", "Thai Smile", "Thai Smile" },
        new[] { "WY", "910", "OMA", "Oman Air (S.A.O.C)", "Oman Air", "Oman Air", "Oman Air" },
        new[] { "5U", "911", "TGU", "Transportes Aereos Guatemaltecos S.A.", "Transportes A�reos Guatemaltecos", "Transportes Aéreos Guatemaltecos", "Chapin" },
        new[] { "QW", "912", "QDA", "Qingdao Airlines Co., Ltd", "Qingdao Airlines Co., Ltd.", "Qingdao Airlines", "Sky Legend" },
        new[] { "SS", "923", "CRL", "Corsair t/a Corsair International", "Corse Air International", "Corsair International", "Corsair" },
        new[] { "GR", "924", "AUR", "Aurigny Air Services Limited", "Aurigny Air Services Ltd.", "Aurigny", "Ayline" },
        new[] { "QH", "926", "BAV", "Bamboo Airways Joint Stock Company dba Bamboo Airways Joint Stock Comp", "Bamboo Airways Joint Company Limited", "Bamboo Airways", "Bamboo" },
        new[] { "UZ", "928", "BRQ", "Buraq Air dba Buraq Air", "Buraq Air Transport (BRQ)", "Buraq Air", "Buraqair" },
        new[] { "JR", "929", "JOY", "Joy Air", "Joy Air Co., Ltd.", "Joy Air", "Joy Air" },
        new[] { "OB", "930", "BOV", "Boliviana de Aviacion (BoA)", "Boliviana de Aviacion (BoA)", "Boliviana de Aviación", "Boliviana" },
        new[] { "VS", "932", "VIR", "Virgin Atlantic Airways Limited", "Virgin Atlantic Airways Limited", "Virgin Atlantic", "Virgin" },
        new[] { "TL", "935", "ANO", "Capiteq Limited dba Airnorth", "Airnorth Regional", "Airnorth", "Topend" },
        new[] { "XJ", "940", "TAX", "Thai Airasia X Company Limited", "Thai Airasia X Company Limited", "Thai AirAsia X", "Express Wing" },
        new[] { "VW", "942", "TAO", "Transportes Aeromar, S.A. de C.V.", "Transportes Aeromar, S.A. de C.V.", "Aeromar", "Trans-Aeromar" },
        new[] { "LS", "949", "EXS", "Jet2.com Limited", "Jet2.com Limited", "Jet2.com", "Channex" },
        new[] { "6B", "951", "BLX", "TUIfly Nordic AB", "TUIfly Nordic AB", "TUI fly Nordic", "Bluescan" },
        new[] { "JJ", "957", "TAM", "TAM Linhas Aereas S.A. dba LATAM Airlines Brasil", "TAM Linhas Aereas S.A. d/b/a LATAM Airlines Brasil", "LATAM Brasil", "TAM" },
        new[] { "LQ", "961", "MKR", "Lanmei Airlines (Cambodia) Co.,Ltd", "Lanmei Airlines (Cambodia) Co., Ltd.", "Lanmei Airlines", "Air Lanme" },
        new[] { "PB", "967", "PVL", "PAL Airlines LTD. dba Provincial Airlines/PAL Airline", "PAL Airlines Ltd.", "PAL Airlines", "Provincial" },
        new[] { "ZA", "969", "SWM", "Sky Angkor Airlines dba Sky Angkor Airlines Co., Ltd", "Sky Angkor Airlines Co., Ltd. d/b/a Sky Angkor Airlines", "Sky Angkor Airlines", "Sky Angkor" },
        new[] { "LU", "972", "LXP", "Transporte Aereo S.A. dba LATAM Airlines Chile", "Transporte Aereo S.A. d/b/a LATAM Airlines Chile", "LATAM Express", "Lanex" },
        new[] { "QZ", "975", "AWQ", "PT. Indonesia AirAsia dba AirAsia X Indonesia", "PT. Indonesia AirAsia", "Indonesia AirAsia", "Wagon Air" },
        new[] { "SJ", "977", "SJY", "PT. Sriwijaya Air dba Sriwijaya Air", "Sriwijaya Air PT", "Sriwijaya Air", "Sriwijaya" },
        new[] { "VJ", "978", "VJC", "Vietjet Aviation Joint Stock Compan dba Vietjet Aviation JSC", "Vietjet Aviation Joint Stock Company", "VietJet Air", "Vietjetair" },
        new[] { "HV", "979", "TRA", "Transavia Airlines", "Transavia", "Transavia", "Transavia" },
        new[] { "BX", "982", "ABL", "Air Busan", "Air Busan", "Air Busan", "Air Busan" },
        new[] { "QK", "983", "JZA", "Jazz Aviation LP", "Jazz Aviation LP", "Jazz (airline)", "Jazz" },
        new[] { "Q2", "986", "DQA", "Island Aviation Services Ltd.", "Island Aviation Services Ltd.", "Maldivian (airline)", "Island Aviation" },
        new[] { "G5", "987", "HXA", "China Express Airlines", "China Express Airlines Co., Ltd", "China Express Airlines", "China Express" },
        new[] { "OZ", "988", "AAR", "Asiana Airlines Inc.", "Asiana Airlines, Inc.", "Asiana Airlines", "Asiana" },
        new[] { "RY", "989", "CJX", "Jiangxi Air Company Limited dba Jiangxi Air", "Jiangxi Air Company Limited d/b/a Jiangxi Air", "Jiangxi Air", "Air Crane" },
        new[] { "D3", "991", "DAO", "Daallo Airlines", "Daallo Airlines", "Daallo Airlines", "Dalo Airlines" },
        new[] { "WO", "995", "WSW", "Swoop Inc.", "Swoop, Inc", "Swoop (airline)", "Swoop" },
        new[] { "UX", "996", "AEA", "Air Europa Lineas Aereas, S.A.", "Air Europa Lineas Aereas, S.A.", "Air Europa", "Europa" },
        new[] { "BG", "997", "BBC", "Biman Bangladesh Airlines Limited", "Biman Bangladesh Airlines", "Biman Bangladesh Airlines", "Bangladesh" },

        new[] { "KH", "687", "AAH", "Aeko Kula, LLC dba Aloha Air Cargo", "Aeko Kula, Inc d/b/a Aloha Air Cargo - United States of America", "Aloha Air Cargo", "Aloha" },
        new[] { "CC", "318", "ABD", "Air Atlanta Icelandic", "Air Atlanta Icelandic - Iceland", "Air Atlanta Icelandic", "Atlanta" },
        new[] { "RU", "580", "ABW", "AirBridgeCargo Airlines LLC", "AirBridge Cargo Airlines Limited - Russian Federation", "AirBridgeCargo Airlines", "AirBridge Cargo" },
        new[] { "GB", "832", "ABX", "ABX Air, Inc.", "ABX Air, Inc. - United States of America", "ABX Air", "Abex" },
        new[] { "JK", "543", "ACL", "Aerolinea Del Caribe S.A. dba Aercaribe S.A.", "Aercaribe Aerolina del Caribe, S.A. - Colombia", "AerCaribe S.A.", "Admire" },
        new[] { "8V", "485", "ACP", "Astral Aviation Limited", "Astral Aviation Ltd. - Kenya", "Astral Aviation", "Astral Cargo" },
        new[] { "KO", "341", "AER", "Alaska Central Express", "Alaska Central Express, Inc. - United States of America", "Alaska Central Express", "Ace Air" },
        new[] { "LD", "288", "AHK", "AHK Air Hong Kong Limited dba AHK Air Hong Kong Limited", "AHK Air Hong Kong Limited - Hong Kong", "Air Hong Kong", "Air Hong Kong" },
        new[] { "S8", "442", "AHW", "Sky Capital Airlines Ltd. dba Sky Air", "Sky Capital Airlines Ltd. - Bangladesh", "SkyAir", "Sky Capital" },
        new[] { "KJ", "994", "AIH", "Air Incheon Co, Ltd", "Air Incheon Co., Ltd. - Korea, Republic of", "Air Incheon", "Air Incheon" },
        new[] { "4W", "574", "AJK", "Allied Air Ltd.", "Allied Air Limited - Nigeria", "Allied Air", "Bambi" },
        new[] { "M6", "810", "AJT", "Amerijet International Inc.", "Amerijet International, Inc. - United States of America", "Amerijet International", "Amerijet" },
        //new[] { "RN", "557", "ARN", "AERONEXUS CORPORATE (PTY) LTD dba Aeronexus Corporate", "Aeronexus Corporate Pty Ltd - South Africa", "", "Aeronex" },
        new[] { "F5", "227", "ATG", "Aerotranscargo SRL", "Aerotranscargo SRL - Moldova, Republic of", "Aerotranscargo", "Moldcargo" },
        new[] { "8C", "813", "ATN", "Air Transport International Inc.", "Air Transport International LLC - United States of America", "Air Transport International", "Air Transport" },
        new[] { "ZT", "858", "AWC", "Titan Airways Limited", "Titan Airways Ltd. - United Kingdom", "Titan Airways", "Zap" },
        new[] { "IX", "236", "AXB", "Air India Charters Limited", "Air India Charters Limited - India", "Air India Express", "Express India" },
        //new[] { "AX", "247", "AXY", "Air X Charter Ltd", "Air X Charter Limited - Malta", "", "Legend" },
        new[] { "7L", "501", "AZG", "Silk Way West Airlines LLC", "Silk Way West Airlines - Azerbaijan", "Silk Way West Airlines", "Silk West" },
        //new[] { "ZP", "445", "AZQ", "Compania de Aviacion Paraguaya S.A dba Paranair", "Silk Way Airlines - Azerbaijan", "Paranair S.A.", "Silk Line" },
        new[] { "ZR", "410", "AZS", "Aviacon Zitotrans Air Company JSC", "Aviacon Zitotrans Air Company JSC - Russian Federation", "Aviacon Zitotrans", "Zitotrans" },
        //new[] { "AJ", "179", "AZY", "Aztec Worldwide Airlines Inc.", "Aztec Worldwide Airlines, Inc. - United States of America", "", "Aztec World" },
        new[] { "BO", "290", "BBD", "Blafugl ehf. dba Bluebird Cargo Ltd", "Bl�fugl ehf. d/b/a bluebird NORDIC - Iceland", "Bluebird Nordic", "Blue Cargo" },
        //new[] { "BZ", "620", "BBG", "Blue Dart Aviation Ltd.", "Blue Bird Airways - Greece", "Blue Dart Aviation", "Cadia Bird" },
        new[] { "QY", "615", "BCS", "European Air Transport Leipzig", "European Air Transport Leipzig GmbH - Germany", "European Air Transport Leipzig", "EuroTrans" },
        //new[] { "E6", "417", "BCT", "Bringer Air Cargo Taxi Aereo Ltda", "Bringer Air Cargo Taxi Aereo Ltda. - Brazil", "", "" },
        new[] { "BZ", "620", "BDA", "Blue Dart Aviation Ltd.", "Blue Dart Aviation Ltd - India", "Blue Dart Aviation", "Blue Dart" },
        new[] { "BH", "256", "BML", "Bismillah Airlines Limited", "Bismillah Airlines Ltd. - Bangladesh", "Bismillah Airlines", "Bismillah" },
        //new[] { "2L", "901", "BOL", "Transportes Aereos Bolivianos", "Transportes Aereos Bolivianos (TAB) - Bolivia", "Transportes Aéreos Bolivianos", "Bol" },
        new[] { "3S", "278", "BOX", "Aerologic GmbH", "AeroLogic GmbH - Germany", "AeroLogic", "German Cargo" },
        //new[] { "8E", "766", "BRG", "Easy Fly Express Limited.", "Bering Air, Inc. - United States of America", "Bering Air", "Bering Air" },
        new[] { "KW", "321", "BSC", "LLC Air company AeroStan dba LLC Air company AeroStan", "AeroStan Aircompany - Kyrgyzstan", "", "Big Shot" },
        //new[] { "4B", "210", "BTQ", "Aviastar-TU Co.  Aviacompany", "Boutique Air, Inc. - United States of America", "", "Boutique" },
        new[] { "XC", "395", "CAI", "Turistik Hava Tasimacilik A.S. (Corendon Airlines)", "Turistik Hava Tasimacilik A.S. - Corendon Airlines - Turkey", "Corendon Airlines", "Corendon" },
        new[] { "CA", "999", "CAO", "Air China Cargo", "Air China Cargo Co., Ltd. - China", "Air China Cargo", "Airchina Freight" },
        //new[] { "CA", "999", "CCA", "Air China Cargo", "Air China Limited - China", "", "Air China" },
        new[] { "X7", "744", "CHG", "Challenge Airlines (BE) S.A.", "Challenge Airlines (BE) S.A. - Belgium", "Challenge Airlines", "Challenge" },
        //new[] { "CF", "804", "CIL", "China Postal Airlines Ltd.", "CIAF Leasing - Egypt", "", "CIAF Leasing" },
        new[] { "W8", "489", "CJT", "Cargojet Airways Ltd.", "Cargojet Airways Ltd. - Canada", "Cargojet", "Cargojet" },
        new[] { "CK", "112", "CKK", "China Cargo Airlines Ltd.", "China Cargo Airlines Ltd. - China", "China Cargo Airlines", "China King" },
        new[] { "K4", "272", "CKS", "Kalitta Air, LLC", "Kalitta Air LLC - United States of America", "Kalitta Air", "Connie" },
        new[] { "P3", "560", "CLU", "Cargologicair Ltd", "Cargologicair Ltd. - United Kingdom", "CargoLogicAir", "Firebird" },
        new[] { "CV", "172", "CLX", "Cargolux Airlines International S.A", "Cargolux Airlines International, S.A. - Luxembourg", "Cargolux", "Cargolux" },
        new[] { "CD", "503", "CND", "Corendon Dutch Airlines B.V.", "Corendon Dutch Airlines - Netherlands", "Corendon Dutch Airlines", "Dutch Corendon" },
        //new[] { "CO", "966", "CNW", "North-Western Cargo International Airlines Co., Ltd.", "North-Western Cargo International Airlines Co., Ltd. - China", "", "Tang" },
        //new[] { "7C", "575", "COY", "Coyne Airways Ltd", "Coyne Airways Ltd. - United Kingdom", "Coyne Airways", "Coyne Air" },
        new[] { "EF", "497", "CPR", "Aerolinea del Caribe  Peru S.A.C.", "Aerolinea del Caribe - Peru S.A.C. - Peru", "AerCaribe", "Caribe-Peru" },
        //new[] { "C8", "356", "CRA", "Cargolux Italia S.p.A.", "Cronos Airlines - Equatorial Guinea", "", "Cronos" },
        new[] { "II", "175", "CSQ", "IBC Airways, Inc.", "IBC Airways, Inc. - United States of America", "IBC Airways", "Chasqui" },
        //new[] { "O3", "921", "CSS", "SF Airlines Company Limited", "Shun Fung Airlines Company Limited - China", "", "Shun Feng" },
        //new[] { "CQ", "976", "CSV", "Charterlines, Inc.", "Coastal Travels Limited - Tanzania, United Republic of", "", "Coastel Travel" },
        new[] { "HT", "877", "CTJ", "Tianjin Air Cargo Co., Ltd.", "Tianjin Air Cargo Co., Ltd. - China", "Tianjin Air Cargo", "Tianjin Cargo" },
        new[] { "CF", "804", "CYZ", "China Postal Airlines Ltd.", "China Postal Airlines Ltd. - China", "China Postal Airlines", "China Posta" },
        new[] { "D5", "992", "DAE", "DHL Aero Expreso S.A.", "DHL Aero Expreso, S.A. - Panama", "DHL Aero Expreso", "YELLOW" },
        new[] { "D0", "936", "DHK", "DHL Air Limited", "DHL Air Limited - United Kingdom", "DHL Air UK", "World Express" },
        new[] { "ES", "155", "DHX", "DHL Aviation EEMEA B.S.C.(c)", "DHL Aviation EEMEA B.S.C(c) - Bahrain", "DHL International Aviation ME", "Dilmun" },
        //new[] { "6R", "873", "DRU", "Aerotransporte de Carga Union, S.A. dba Aerounion", "Open Joint Stock Company \"ALROSA\" (Mirny Air Enterprise) - Russian Federation", "", "Mirny" },
        new[] { "WK", "945", "EDW", "Edelweiss Air AG", "Edelweiss Air AG - Switzerland", "Edelweiss Air", "Edelweiss" },
        new[] { "8E", "766", "EFX", "Easy Fly Express Limited.", "Easy Fly Express Limited - Bangladesh", "Easy Fly Express", "Easy Express" },
        //new[] { "E7", "355", "EKA", "Estafeta Carga Aerea, S.A. de C.V", "Equaflight Service - Congo", "", "Equaflight" },
        new[] { "RF", "358", "EOK", "Erofey Limited Liability Company", "Aero K Airlines - Korea, Democratic People's Republic of", "Aero K", "Aerokorea" },
        //new[] { "RF", "358", "ERF", "Erofey Limited Liability Company", "Erofey LLC t/a E-Cargo Airlines - Russian Federation", "", "Gulliver" },
        new[] { "E7", "355", "ESF", "Estafeta Carga Aerea, S.A. de C.V", "Estafeta Carga Aerea, S.A. de C.V. - Mexico", "Estafeta Carga Aérea", "ESTAFETA" },
        //new[] { "ES", "155", "ETR", "DHL Aviation EEMEA B.S.C.(c)", "Estelar Latinoamerica C.A. d/b/a Estelar - Venezuela", "", "Estel" },
        new[] { "8D", "319", "EXV", "FITS Aviation (Pvt) Ltd", "FITS Aviation (Pvt) Ltd - Sri Lanka", "FitsAir", "Expoavia" },
        //new[] { "IF", "611", "FBA", "Gulf & Caribbean Cargo, Inc", "Fly Baghdad - Iraq", "", "Iraq Express" },
        //new[] { "N7", "264", "FCM", "My Jet Xpress Airlines Sdn.Bhd. dba My Jet Xpress Airlines", "Nordic Regional Airlines Oy - Finland", "", "Finncomm" },
        new[] { "FX", "023", "FDX", "FedEx", "Federal Express Corporation d/b/a FedEx - United States of America", "FedEx", "Express" },
        //new[] { "6L", "342", "GCL", "CargoLogic Germany GmbH", "CargoLogic Germany GmbH - Germany", "", "Saxonian" },
        //new[] { "D4", "330", "GEL", "Airline GEO SKY LLC", "Airline Geo Sky LLC - Georgia", "", "Sky Georgia" },
        new[] { "ZQ", "944", "GER", "German Airways GmbH & Co.KG", "German Airways GmbH & Co.KG - Germany", "German Airways", "German Eagle" },
        new[] { "GW", "059", "GJT", "GetJet Airlines", "GetJet Airlines - Lithuania", "GetJet Airlines", "Getjet" },
        new[] { "5Y", "369", "GTI", "Atlas Air, Inc.", "Atlas Air, Inc. - United States of America", "Atlas Air", "Giant" },
        //new[] { "3S", "278", "GUY", "Aerologic GmbH", "CAIRE d/b/a Air Antilles - Guyana", "", "Green Bird" },
        //new[] { "HA", "164", "HAL", "Al Haya aviation co.", "Hawaiian Airlines - United States of America", "", "Hawaiian" },
        //new[] { "HT", "877", "HAT", "Tianjin Air Cargo Co., Ltd.", "Air Horizont - Malta", "", "Sky Runner" },
        new[] { "5K", "026", "HFY", "Hi Fly Transportes Aereos, S.A.", "Springjet, S.A. t/a HiFly - Portugal", "Hi Fly", "Sky Flyer" },
        //new[] { "RH", "828", "HKC", "Hong Kong Air Cargo Carrier Limited", "Hong Kong Air Cargo Co., Ltd. - China", "", "Mascot" },
        //new[] { "I9", "959", "HLF", "China Central Airlines Co., Ltd.", "Central Airlines Co., Ltd. - China", "", "Homeland" },
        //new[] { "HA", "164", "HYA", "Al Haya aviation co.", "Al-Haya Aviation - Iraq", "", "Alhaya" },
        new[] { "YG", "860", "HYT", "YTO Cargo Airlines Co. Ltd.", "YTO Cargo Airlines Co., Ltd. - China", "YTO Cargo Airlines", "Quick Air" },
        new[] { "5C", "700", "ICL", "C.A.L. Cargo Airlines Ltd.", "Cavei Avir Lemitanim t/a Cargo Air Lines - Israel", "CAL Cargo Airlines", "Cal" },
        new[] { "C8", "356", "ICV", "Cargolux Italia S.p.A.", "Cargolux Italia S.p.A. - Italy", "Cargolux Italia", "Cargolux Italia" },
        new[] { "TE", "022", "IGA", "Sky Taxi sp. z.o.o", "SkyTaxi sp. z.o.o. - Poland", "SkyTaxi", "iguana" },
        new[] { "XM", "927", "IMX", "Zimex Aviation AG", "Zimex Aviation Limited - Switzerland", "Zimex Aviation", "Zimex" },
        new[] { "JY", "420", "IWY", "National Air Charters, Inc.", "InterCaribbean Airways - Turks And Caicos Islands", "InterCaribbean Airways", "Islandways" },
        new[] { "JA", "973", "JAT", "JETSMART SpA", "JetSMART SpA - Chile", "JetSmart", "Rocksmart" },
        //new[] { "7C", "575", "JJA", "Coyne Airways Ltd", "Jeju Air Co. Ltd. - Korea, Republic of", "Jeju Air", "JEJU AIR" },
        new[] { "L3", "947", "JOS", "DHL de Guatemala S.A.", "DHL de Guatemala, S.A. - Guatemala", "DHL de Guatemala", "" },
        //new[] { "IM", "707", "JPJ", "LLP Air company Jupiter Jet", "Aircompany Jupiter Jet LLP - Kazakhstan", "", "Jupiterjet" },
        new[] { "3K", "375", "JSA", "Jetstar Asia Airways Pte Ltd", "Jetstar Asia Airways Pte Ltd - Singapore", "Jetstar Asia Airways", "Jetstar Asia" },
        //new[] { "JY", "420", "JYA", "National Air Charters, Inc.", "National Air Charters, Inc. - United States of America", "", "Sun Biz" },
        new[] { "FK", "758", "KFA", "Kelowna Flightcraft Air Charter Ltd dba KF Cargo", "Kelowna Flightcraft Air Charter Ltd. - Canada", "KF Cargo", "Flightcraft" },
        //new[] { "KO", "341", "KMA", "Alaska Central Express", "Komiaviatrans Open Joint Stock Company - Russian Federation", "", "Komi Avia" },
        new[] { "8K", "119", "KMI", "K-Mile Air Co. Ltd dba K-Mile Air", "K-Mile Air Co. Ltd. - Thailand", "K-Mile Air", "KAY-MILE AIR" },
        //new[] { "WW", "313", "KXP", "M Jets International Sdn Bhd dba Kargo Xpress", "M Jets International Sdn Bhd t/a Kargo Xpress - Malaysia", "", "Xpress Kargo" },
        //new[] { "GG", "576", "KYE", "Sky Lease I, Inc.", "Sky Lease 1, Inc. - United States of America", "", "Sky Cube" },
        new[] { "GO", "444", "KZU", "ULS Airlines Cargo", "ULS Havayollair Kargo Tasimacilik A.S. - Turkey", "ULS Airlines Cargo", "UNIVERSAL CARGO" },
        new[] { "L7", "985", "LAE", "Linea Aerea Carguera de Colombia S. dba LATAM Cargo Colombia", "Lineas Aereas Carguera de Colombia S.A. d/b/a LATAM Cargo Colombia - Colombia", "LATAM Cargo Colombia", "Lanco" },
        //new[] { "4L", "174", "LAU", "Lineas Aereas Suramericanas SA \"LAS S.A.\"", "Lineas Aereas Suramericanas SA t/a LAS, S.A. - Colombia", "", "Suramericano" },
        new[] { "UC", "145", "LCO", "Lan Cargo S.A. dba LATAM Cargo Chile", "LAN Cargo S.A. d/b/a LATAM Cargo Chile - Chile", "LATAM Cargo Chile", "LAN CARGO" },
        //new[] { "6T", "388", "LGT", "Longtail Aviation International Limited", "Longtail Aviation International Ltd - Bermuda", "", "Longtail" },
        new[] { "GI", "908", "LHA", "China Central Longhao Airlines Co. Ltd", "China Central Airlines Co., Ltd. - China", "Longhao Airlines", "Air Canton" },
        //new[] { "CD", "503", "LLR", "Corendon Dutch Airlines B.V.", "Alliance Air - India", "", "Allied" },
        new[] { "M3", "549", "LTG", "ABSA  -  Aerolinhas Brasileiras S.A dba LATAM Cargo Brasil", "ABSA - Aerolinhas Brasileiras S.A. d/b/a LATAM Cargo Brasil - Brazil", "LATAM Cargo Brasil", "Tamcargo" },
        new[] { "L2", "344", "LYC", "Lynden Air Cargo, LLC", "Lynden Air Cargo, LLC - United States of America", "Lynden Air Cargo", "Lynden" },
        new[] { "M7", "865", "MAA", "Aerotransportes Mas de Carga S.A. de C.V. dba Masair", "Aerotransportes Mas de Carga, S.A. de C.V. t/a MasAir - Mexico", "Mas Air", "MAS Carga" },
        new[] { "T2", "137", "MCS", "MCS Aerocarga de Mexico, S.A. de CV", "TUM AeroCarga de Mexico, S.A. de C.V. - Mexico", "Mercury Air Cargo", "Carmex" },
        //new[] { "4X", "805", "MEC", "Mercury Air Cargo, Inc.", "Mercury Air Cargo, Inc. d/b/a Air Mercury - United States of America", "", "Mercair" },
        new[] { "P9", "046", "MGE", "Aero Micronesia Inc. dba Asia Pacific Airlines", "Aero Micronesia Inc d/b/a Asia Pacific Airlines - Guam", "Asia Pacific Airlines", "Magellan" },
        new[] { "M2", "592", "MHV", "Sunrise Airlines Inc.", "MHS Aviation GmbH - Germany", "MHS Aviation", "Snowcap" },
        new[] { "DB", "094", "MLT", "MALETH-AERO AOC LIMITED", "Maleth-Aero AOC Ltd - Malta", "Maleth-Aero", "Maleth" },
        //new[] { "X8", "658", "MMX", "Airmax S.A.", "Airmax S.A. - Peru", "", "Perumax" },
        new[] { "MB", "716", "MNB", "MNG Havayollari Tasimacilik A.S.", "MNG Airlines - Turkey", "MNG Airlines", "Black Sea" },
        new[] { "MP", "129", "MPH", "Martinair Holland N.V.", "Martinair Holland N.V. - Netherlands", "Martinair", "Martinair" },
        new[] { "M4", "408", "MSA", "Poste Air Cargo s.r.l. dba Poste Air Cargo", "Poste Air Cargo S.r.l. - Italy", "Poste Air Cargo", "Mistral Wings" },
        //new[] { "7N", "007", "MTD", "MetroJets, LLC", "MetroJets, LLC - United States of America", "", "Matador" },
        //new[] { "WD", "352", "MWM", "Limited Liability Company Aviation Company Eleron", "Modern Transporte Aereo De Carga S.A. d/b/a Modern Logistics - Brazil", "", "Modernair" },
        new[] { "6M", "387", "MXM", "Maximus Airlines Limited Liability Company", "Maximus Airlines - Ukraine", "Maximus Air", "Maxlines" },
        //new[] { "M2", "592", "MXS", "Sunrise Airlines Inc.", "Sunrise Airlines, Inc. d/b/a Millon Express - United States of America", "", "Millon Express" },
        new[] { "2Y", "585", "MYU", "PT. My Indo Airlines", "PT. My Indo Airlines - Indonesia", "My Indo Airlines", "Indo" },
        new[] { "NC", "345", "NAC", "Northern Air Cargo, LLC.", "Northern Air Cargo, LLC. - United States of America", "Northern Air Cargo", "Yukon" },
        new[] { "KZ", "933", "NCA", "Nippon Cargo Airlines", "Nippon Cargo Airlines - Japan", "Nippon Cargo Airlines", "Nippon Cargo" },
        new[] { "N7", "264", "NEP", "My Jet Xpress Airlines Sdn.Bhd. dba My Jet Xpress Airlines", "My Jet Xpress Airlines Sdn.Bhd. dba My Jet Xpress - Malaysia", "My Jet Xpress Airlines", "Warisan" },
        //new[] { "S5", "917", "NKP", "Limited Liability Company \"Abakan A", "Abakan Air LLC - Russian Federation", "", "Abakan Air" },
        new[] { "NO", "703", "NOS", "Neos S.P.A.", "Neos S.p.A. - Italy", "Neos", "Moonflower" },
        new[] { "N9", "326", "NVR", "Nova Airlines AB", "Nova Airlines AB - Sweden", "Novair", "Navigator" },
        //new[] { "2L", "901", "OAW", "Transportes Aereos Bolivianos", "Helvetic Airways AG - Switzerland", "Helvetic Airways", "Helvetic" },
        //new[] { "4S", "644", "OLC", "Solar Cargo, C.A.", "Solar Cargo, C.A. - Venezuela", "", "Solarcargo" },
        new[] { "OV", "960", "OMS", "Salam Air (S.A.O.C)", "SalamAir - Oman", "Salam Air", "Mazoon" },
        new[] { "PO", "403", "PAC", "Polar Air Cargo Worldwide, Inc.", "Polar Air Cargo Worldwide, Inc. - United States of America", "Polar Air Cargo", "Polar" },
        //new[] { "PE", "934", "PCF", "Pacific Air Express Australia Pty L", "Pacific Air Express Australia (Pty) Ltd. - Australia", "", "Pacific" },
        //new[] { "PE", "934", "PEV", "Pacific Air Express Australia Pty L", "Altenrhein Luftfahrt GmbH t/a People�s - Austria", "", "Peoples" },
        //new[] { "CM", "230", "PHP", "Compania Panamena de Aviacion, S.A. (COPA)", "PSI Air 2007, Inc. - Philippines", "", "Sky Power" },
        //new[] { "0J", "254", "PJZ", "Premium Jet AG", "Premium Jet AG - Switzerland", "", "Primejet" },
        //new[] { "P9", "046", "PXC", "Aero Micronesia Inc. dba Asia Pacific Airlines", "Palau Express Airlines Corporation - Palau", "", "Palau Express" },
        //new[] { "C4", "340", "QAI", "Conquest Air, Inc.", "Conquest Air, Inc. - United States of America", "", "Chickpea" },
        new[] { "TH", "539", "RMY", "Raya Airways Sdn. Bhd. dba Raya Airways", "Raya Airways Sdn. Bhd. d/b/a Raya Airways - Malaysia", "Raya Airways", "Raya Express" },
        new[] { "7T", "144", "RTM", "Aero Express del Ecuador  - Trans A", "Aero Express Del Ecuador Trans AM - Ecuador", "DHL Ecuador", "Aero Transam" },
        new[] { "9T", "556", "RUN", "ACT Havayollari A.S.", "ACT Havayollari A.S. (ACT Airlines) - Turkey", "Air ACT", "Cargo Turk" },
        //new[] { "VZ", "510", "RVP", "Air Express S.A. dba Airclass Lineas Aereas", "Sevenair Air Services - Portugal", "", "Sevair" },
        new[] { "U3", "728", "SAY", "Air Company \"Sky Gates Airlines\" Limited Liability Company", "Air Company \"Sky Gates Airlines\" LLC - Russian Federation", "Sky Gates Airlines", "Sky Path" },
        new[] { "4E", "242", "SBO", "Stabo Air Limited", "Stabo Air Limited - Zambia", "Stabo Air", "Stabair" },
        //new[] { "S8", "442", "SDA", "Sky Capital Airlines Ltd. dba Sky Air", "Sounds Air Travel & Tourism Ltd. - New Zealand", "", "" },
        //new[] { "N9", "326", "SHA", "Nova Airlines AB", "Shree Airlines Pvt. Ltd. - Nepal", "", "Shree-Air" },
        new[] { "9S", "099", "SOO", "Southern Air Inc.", "Southern Air, Inc. - United States of America", "Southern Air", "Southern Air" },
        new[] { "PQ", "010", "SQP", "LLC \"SKYUP AIRLINES\"", "SkyUp Airlines - Ukraine", "SkyUp", "Skyup" },
        //new[] { "SX", "906", "SRK", "Skywagon Corporation", "Skywork SA - Switzerland", "", "Skyfox" },
        new[] { "DJ", "763", "SRR", "Star Air A/S", "Star Air A/S - Denmark", "Star Air", "Whitestar" },
        new[] { "WT", "221", "SWT", "Swiftair, S.A.", "Swiftair, S.A. - Spain", "Swiftair", "Swift" },
        //new[] { "ZF", "037", "SXY", "AZUR air Limited Liability Company", "Safari Express Cargo Limited - Kenya", "", "Safari Express" },
        //new[] { "YD", "259", "SYG", "Synergy Aviation Ltd", "Synergy Aviation Ltd. - United Kingdom", "", "Synergy" },
        //new[] { "TA", "202", "TAK", "TACA International Airlines S.A.", "Transafrican Air Limited - Kenya", "", "Transafrican" },
        new[] { "T7", "382", "TIW", "Transcarga Int'l Airways, C.A.", "Transcarga International Airways, C.A. - Venezuela", "Transcarga", "TIACA" },
        //new[] { "T7", "382", "TJT", "Transcarga Int'l Airways, C.A.", "Twin Jet - France", "", "Twinjet" },
        new[] { "6R", "873", "TNO", "Aerotransporte de Carga Union, S.A. dba Aerounion", "Aerotransporte de Carga Union, S.A. de C.V. - Mexico", "AeroUnion", "Aerounion" },
        new[] { "BY", "754", "TOM", "TUI Airways Limited dba TUI", "TUI Airways Limited - United Kingdom", "TUI Airways", "Tomjet" },
        new[] { "QT", "729", "TPA", "Tampa Cargo S.A.S", "Transportes Aereos Mercantiles Panamericanos S.A., (TAMPA) t/a Avianca Cargo - Colombia", "Avianca Cargo", "Tampa" },
        //new[] { "4B", "210", "TUP", "Aviastar-TU Co.  Aviacompany", "Aviastar-Tu Co. Ltd - Russian Federation", "", "Tupolevair" },
        //new[] { "VZ", "510", "TVJ", "Air Express S.A. dba Airclass Lineas Aereas", "Thai Vietjet Air Joint Stock Co., Ltd. - Thailand", "", "Thaiviet Jet" },
        //new[] { "T8", "357", "TVR", "CA Terra Avia SRL", "CA Terra Avia S.r.l. - Moldova, Republic of", "", "Terraavia" },
        //new[] { "TI", "768", "TWI", "Tailwind Hava Yollari A.S.", "Tailwind Hava Yollari A.S. - Turkey", "", "Tailwind" },
        new[] { "V2", "552", "TWN", "Avialeasing Aviation Company", "Avialeasing Aviation Company - Uzbekistan", "Avialeasing", "Twinarrow" },
        //new[] { "U7", "399", "UCG", "Uniworld Air Cargo Corp.", "Uniworld Commercial Group, S.A. (Uniworld Air Cargo) - Panama", "", "Uniworld" },
        new[] { "5X", "406", "UPS", "UPS", "United Parcel Service Company (UPS) - United States of America", "UPS", "UPS" },
        new[] { "UW", "699", "UTP", "Uni-Top Airlines Co.,Ltd", "Uni-Top Airlines - China", "Uni-Top Airlines", "Uni-Top" },
        new[] { "VI", "412", "VDA", "Volga-Dnepr Airline Joint Stock", "Volga-Dnepr Airline Joint Stock - Russian Federation", "Volga-Dnepr", "Volga" },
        new[] { "V4", "946", "VEC", "Vensecar Internacional C.A.", "Vensecar Internacional, C.A. - Venezuela", "Vensecar Internacional", "Vecar" },
        //new[] { "V4", "946", "VES", "Vensecar Internacional C.A.", "Vieques Air Link, Inc. - Puerto Rico", "", "Vieques" },
        new[] { "DK", "630", "VKG", "Sunclass Airlines Aps", "SunClass Airlines ApS - Denmark", "Sunclass Airlines", "Viking" },
        new[] { "WD", "352", "VVA", "Limited Liability Company Aviation Company Eleron", "LLC Aviation Company Eleron - Ukraine", "Eleron Airlines", "Load Shark" },
        //new[] { "4W", "574", "WAV", "Allied Air Ltd.", "Warbelow's Air Ventures, Inc. - United States of America", "", "Warbelow" },
        new[] { "3G", "380", "WCM", "World Cargo Airline Sdn Bhd. dba World Cargo Airlines", "World Cargo Airlines Sdn. Bhd. - Malaysia", "World Cargo Airline", "World Cargo" },
        new[] { "KD", "904", "WGN", "Western Global Airlines, Inc.", "Western Global Airlines, Inc. - United States of America", "Western Global Airlines", "Western Global" },
        //new[] { "8V", "485", "WRF", "Astral Aviation Limited", "Wright Air Service, Inc. - United States of America", "", "Wright Flyer" },
        new[] { "7A", "336", "XRC", "Express Air Cargo", "Express Air Cargo - Tunisia", "Express Air Cargo", "TUNISIA CARGO" },
        //new[] { "ZK", "686", "ZAV", "Limited Liability Company \"Aircompa ZetAvia\" dba \"Aircompany ZetAvia\"", "Zetavia Limited, Aircompany - Ukraine", "", "Zetavia" },
    };
}