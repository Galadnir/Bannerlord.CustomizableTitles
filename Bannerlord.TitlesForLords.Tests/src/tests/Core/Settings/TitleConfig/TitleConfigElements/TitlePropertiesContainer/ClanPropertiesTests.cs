using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.Tests.tests.Core.Settings;
using Newtonsoft.Json;

namespace Bannerlord.TitlesForLords.Tests.src.tests.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	[TestClass]
	public class ClanPropertiesTests {

		static readonly ClanProperties FullProperties = JsonConvert.DeserializeObject<ClanProperties>(JsonObjectsForTests.FullClanProperties().ToString())!;

		static readonly ClanProperties EmptyProperties = JsonConvert.DeserializeObject<ClanProperties>(JsonObjectsForTests.EmptyProperties().ToString())!;

		[TestMethod]
		public void TestSerialization() {
			string fullPropertiesSerialized = JsonConvert.SerializeObject(FullProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			string emptyPropertiesSerialized = JsonConvert.SerializeObject(EmptyProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.FullClanProperties().ToString(), fullPropertiesSerialized);
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.EmptyProperties().ToString(), emptyPropertiesSerialized);
		}
	}
}
