using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.Tests.tests.Core.Settings;
using Newtonsoft.Json;

namespace Bannerlord.TitlesForLords.Tests.src.tests.Core.Settings.TitleConfig.TitleConfigElements {
	[TestClass]
	public class NotableOwnedCaravansTests {

		static readonly NotableOwnedCaravans FullProperties = JsonConvert.DeserializeObject<NotableOwnedCaravans>(JsonObjectsForTests.FullNotableOwnedCaravansProperties().ToString())!;
		static readonly NotableOwnedCaravans EmptyProperties = JsonConvert.DeserializeObject<NotableOwnedCaravans>(JsonObjectsForTests.EmptyProperties().ToString())!;

		[TestMethod]
		public void TestSerialization() {
			JsonConvert.SerializeObject(FullProperties, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			JsonConvert.SerializeObject(EmptyProperties, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			// only test if it throws, empty dictionaries are added as keys during serialization, therefore I can't compare via object.ToJsonString()
		}
	}
}
