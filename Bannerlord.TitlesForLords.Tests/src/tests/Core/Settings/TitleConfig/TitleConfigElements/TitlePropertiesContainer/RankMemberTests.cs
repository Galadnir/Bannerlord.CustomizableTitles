using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.Tests.tests.Core.Settings;
using Newtonsoft.Json;

namespace Bannerlord.TitlesForLords.Tests.src.tests.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	[TestClass]
	public class RankMemberTests {

		static readonly RankMember FullProperties = JsonConvert.DeserializeObject<RankMember>(JsonObjectsForTests.FullRankMemberProperties().ToString())!;
		static readonly RankMember EmptyProperties = JsonConvert.DeserializeObject<RankMember>(JsonObjectsForTests.EmptyProperties().ToString())!;

		[TestMethod]
		public void TestSerialization() {
			string fullPropertiesSerialized = JsonConvert.SerializeObject(FullProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			string emptyPropertiesSerialized = JsonConvert.SerializeObject(EmptyProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.FullRankMemberProperties().ToString(), fullPropertiesSerialized);
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.EmptyProperties().ToString(), emptyPropertiesSerialized);
		}
	}
}
