using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.Tests.tests.Core.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bannerlord.TitlesForLords.Tests.src.tests.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	[TestClass]
	public class KingdomPropertiesTests {

		static readonly KingdomProperties FullProperties = JsonConvert.DeserializeObject<KingdomProperties>(JsonObjectsForTests.FullKingdomProperties().ToString())!;
		static readonly KingdomProperties EmptyProperties = JsonConvert.DeserializeObject<KingdomProperties>(JsonObjectsForTests.EmptyProperties().ToString())!;

		[TestMethod]
		public void TestSerialization() {
			string fullPropertiesSerialized = JsonConvert.SerializeObject(FullProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			string emptyPropertiesSerialized = JsonConvert.SerializeObject(EmptyProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.FullKingdomProperties().ToString(), fullPropertiesSerialized);
			JsonObjectsForTests.AssertJsonEquality(new JObject() {
				[nameof(KingdomProperties.ClanTiers)] = new JObject()
			}.ToString(), emptyPropertiesSerialized);
		}
	}
}
