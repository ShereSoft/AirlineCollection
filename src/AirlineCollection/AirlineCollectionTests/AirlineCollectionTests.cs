using System;
using System.Linq;
using Xunit;

namespace AirlineCollectionTests
{
    public class AirlineCollectionTests
    {
		[Fact]
		void Test_GetAirline_WithValid2LetterCode_ReturnsAirline()
		{
			Assert.NotNull(AirlineCollection.GetAirline("AA"));
		}

		[Fact]
		void Test_GetAirline_WithValid3LetterCode_ReturnsAirline()
		{
			Assert.NotNull(AirlineCollection.GetAirline("AAL"));
		}

		[Fact]
		void Test_GetAirline_WithValidPrefix_ReturnsAirline()
		{
			Assert.NotNull(AirlineCollection.GetAirline("001"));
		}

		[Fact]
		void Test_GetAirline_WithInvalid2LetterCode_ReturnsNull()
		{
			Assert.Null(AirlineCollection.GetAirline("ZZ"));
		}

		[Fact]
		void Test_GetAirline_WithInvalid3LetterCode_ReturnsNull()
		{
			Assert.Null(AirlineCollection.GetAirline("ZZZ"));
		}

		[Fact]
		void Test_GetAirline_WithInvalidPrefix_ReturnsNull()
		{
			Assert.Null(AirlineCollection.GetAirline("000"));
		}

		[Fact]
		void Test_Contains_WithValid2LetterCode_ReturnsTrue()
		{
			Assert.True(AirlineCollection.Contains("AA"));
		}

		[Fact]
		void Test_Contains_WithValid3LetterCode_ReturnsTrue()
		{
			Assert.True(AirlineCollection.Contains("AAL"));
		}

		[Fact]
		void Test_Contains_WithValidPrefix_ReturnsTrue()
		{
			Assert.True(AirlineCollection.Contains("001"));
		}

		[Fact]
		void Test_Contains_WithInvalid2LetterCode_ReturnsFalse()
		{
			Assert.False(AirlineCollection.Contains("ZZ"));
		}

		[Fact]
		void Test_Contains_WithInvalid3LetterCode_ReturnsFalse()
		{
			Assert.False(AirlineCollection.Contains("ZZZ"));
		}

		[Fact]
		void Test_Contains_WithInvalidPrefix_ReturnsFalse()
		{
			Assert.False(AirlineCollection.Contains("000"));
		}

		[Fact]
		void Test_Normalize_WithValid2LetterCode_ReturnsNormalized2LetterCode()
		{
			Assert.Equal("AA", AirlineCollection.Normalize("aa"));
		}

		[Fact]
		void Test_Normalize_WithValid3LetterCode_ReturnsNormalized3LetterCode()
		{
			Assert.Equal("AAL", AirlineCollection.Normalize("aal"));
		}

		[Fact]
		void Test_Normalize_WithValidPrefix_ReturnsNormalizedPrefix()
		{
			Assert.Equal("001", AirlineCollection.Normalize("1"));
		}

		[Fact]
		void Test_Normalize_WithValidNumericPrefix_ReturnsNormalizedPrefix()
		{
			Assert.Equal("001", AirlineCollection.Normalize(1));
		}

		[Fact]
		void Test_Normalize_WithInalid2LetterCode_ReturnsNull()
		{
			Assert.Null(AirlineCollection.Normalize("zz"));
		}

		[Fact]
		void Test_Normalize_WithInvalid3LetterCode_ReturnsNull()
		{
			Assert.Null(AirlineCollection.Normalize("zzz"));
		}

		[Fact]
		void Test_Normalize_WithInvalidPrefix_ReturnsNull()
		{
			Assert.Null(AirlineCollection.Normalize("0"));
		}

		[Fact]
		void Test_Normalize_WithInvalidNumericPrefix_ReturnsNull()
		{
			Assert.Null(AirlineCollection.Normalize(0));
		}

		[Fact]
		void Test_Values_ReturnsAllData()
		{
			Assert.NotNull(AirlineCollection.Values);
			Assert.True(AirlineCollection.Values.Count() > 0);
		}

		[Fact]
		void Test_Instance_CanEnumerate()
		{
			foreach (var airline in new AirlineCollection())
			{
				Assert.NotNull(airline);
			}

			Assert.True(new AirlineCollection().Any());
		}

		[Fact]
		void Test_Instance_WithValid2LetterCode_ReturnsAirline()
		{
			Assert.NotNull(new AirlineCollection()["AA"]);
		}

		[Fact]
		void Test_Instance_WithValid3LetterCode_ReturnsAirline()
		{
			Assert.NotNull(new AirlineCollection()["AAL"]);
		}

		[Fact]
		void Test_Instance_WithValidPrefix_ReturnsAirline()
		{
			Assert.NotNull(new AirlineCollection()["001"]);
		}

		[Fact]
		void Test_Instance_WithInvalid2LetterCode_ReturnsNull()
		{
			Assert.Null(new AirlineCollection()["zz"]);
		}

		[Fact]
		void Test_Instance_WithInvalid3LetterCode_ReturnsNull()
		{
			Assert.Null(new AirlineCollection()["zzz"]);
		}

		[Fact]
		void Test_Instance_WithInvalidPrefix_ReturnsNull()
		{
			Assert.Null(new AirlineCollection()["000"]);
		}

		[Fact]
		void Test_Instance_CanAddNewAirline()
		{
			var airlines = new AirlineCollection();
			airlines.Add("ZZ", "ZZZ", "000");

			Assert.NotNull(airlines["000"]);
		}

		[Fact]
		void Test_Instance_CanAddNewAirlineWithCompleteData()
		{
			var airlines = new AirlineCollection();
			airlines.Add("ZZ", "ZZZ", "000", "ZZ Airline", "ZZ Air LLC", "ZZA", "ZZ Air");

			var newAirline = airlines["ZZ"];
			Assert.NotNull(newAirline);
			Assert.Equal("ZZ", newAirline.Iata2LetterCode);
			Assert.Equal("ZZZ", newAirline.Icao3LetterCode);
			Assert.Equal("000", newAirline.Prefix);
			Assert.Equal("ZZ Airline", newAirline.IataName);
			Assert.Equal("ZZ Air LLC", newAirline.IcaoName);
			Assert.Equal("ZZA", newAirline.Callsign);
			Assert.Equal("ZZ Air", newAirline.Name);
		}

		[Fact]
		void Test_Instance_CanRemoveExistingAirline()
		{
			var airlines = new AirlineCollection();

			Assert.NotNull(airlines["AA"]);

			airlines.Remove("AA");

			Assert.Null(airlines["AA"]);
		}

		[Fact]
		void Test_Instance_GetAirline_WithValid2LetterCode_ReturnsAirline()
		{
			Assert.NotNull(new AirlineCollection().GetAirline("AA"));
		}

		[Fact]
		void Test_Instance_GetAirline_WithValid3LetterCode_ReturnsAirline()
		{
			Assert.NotNull(new AirlineCollection().GetAirline("AAL"));
		}

		[Fact]
		void Test_Instance_GetAirline_WithValidPrefix_ReturnsAirline()
		{
			Assert.NotNull(new AirlineCollection().GetAirline("001"));
		}

		[Fact]
		void Test_Instance_GetAirline_WithInvalid2LetterCode_ReturnsNull()
		{
			Assert.Null(new AirlineCollection().GetAirline("ZZ"));
		}

		[Fact]
		void Test_Instance_GetAirline_WithInvalid3LetterCode_ReturnsNull()
		{
			Assert.Null(new AirlineCollection().GetAirline("ZZZ"));
		}

		[Fact]
		void Test_Instance_GetAirline_WithInvalidPrefix_ReturnsNull()
		{
			Assert.Null(new AirlineCollection().GetAirline("000"));
		}

		[Fact]
		void Test_Instance_Contains_WithValid2LetterCode_ReturnsTrue()
		{
			Assert.True(new AirlineCollection().Contains("AA"));
		}

		[Fact]
		void Test_Instance_Contains_WithValid3LetterCode_ReturnsTrue()
		{
			Assert.True(new AirlineCollection().Contains("AAL"));
		}

		[Fact]
		void Test_Instance_Contains_WithValidPrefix_ReturnsTrue()
		{
			Assert.True(new AirlineCollection().Contains("001"));
		}

		[Fact]
		void Test_Instance_Contains_WithInvalid2LetterCode_ReturnsFalse()
		{
			Assert.False(new AirlineCollection().Contains("ZZ"));
		}

		[Fact]
		void Test_Instance_Contains_WithInvalid3LetterCode_ReturnsFalse()
		{
			Assert.False(new AirlineCollection().Contains("ZZZ"));
		}

		[Fact]
		void Test_Instance_Contains_WithInvalidPrefix_ReturnsFalse()
		{
			Assert.False(new AirlineCollection().Contains("000"));
		}

		[Fact]
		void Test_Instance_Normalize_WithValid2LetterCode_ReturnsNormalized2LetterCode()
		{
			Assert.Equal("AA", new AirlineCollection().Normalize("aa"));
		}

		[Fact]
		void Test_Instance_Normalize_WithValid3LetterCode_ReturnsNormalized3LetterCode()
		{
			Assert.Equal("AAL", new AirlineCollection().Normalize("aal"));
		}

		[Fact]
		void Test_Instance_Normalize_WithValidPrefix_ReturnsNormalizedPrefix()
		{
			Assert.Equal("001", new AirlineCollection().Normalize("1"));
		}

		[Fact]
		void Test_Instance_Normalize_WithValidNumericPrefix_ReturnsNormalizedPrefix()
		{
			Assert.Equal("001", new AirlineCollection().Normalize(1));
		}

		[Fact]
		void Test_Instance_Normalize_WithInalid2LetterCode_ReturnsNull()
		{
			Assert.Null(new AirlineCollection().Normalize("zz"));
		}

		[Fact]
		void Test_Instance_Normalize_WithInvalid3LetterCode_ReturnsNull()
		{
			Assert.Null(new AirlineCollection().Normalize("zzz"));
		}

		[Fact]
		void Test_Instance_Normalize_WithInvalidPrefix_ReturnsNull()
		{
			Assert.Null(new AirlineCollection().Normalize("0"));
		}

		[Fact]
		void Test_Instance_Normalize_WithInvalidNumericPrefix_ReturnsNull()
		{
			Assert.Null(new AirlineCollection().Normalize(0));
		}
	}
}
