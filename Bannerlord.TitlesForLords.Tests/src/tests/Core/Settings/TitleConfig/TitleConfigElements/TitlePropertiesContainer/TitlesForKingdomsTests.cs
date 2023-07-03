using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.Tests.tests.Core.Settings;
using Newtonsoft.Json;

namespace Bannerlord.TitlesForLords.Tests.src.tests.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	[TestClass]
	public class TitlesForKingdomsTests {

		static readonly TitlesForKingdoms FullProperties = JsonConvert.DeserializeObject<TitlesForKingdoms>(JsonObjectsForTests.FullTitlesForKingdomsProperties().ToString())!;
		static readonly TitlesForKingdoms EmptyProperties = JsonConvert.DeserializeObject<TitlesForKingdoms>(JsonObjectsForTests.EmptyProperties().ToString())!;

		[TestMethod]
		public void TestSerialization() {
			JsonConvert.SerializeObject(FullProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			JsonConvert.SerializeObject(EmptyProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			// only test if it throws, empty dictionaries are added as keys during serialization, therefore I can't compare via object.ToJsonString()
		}
	}
}
