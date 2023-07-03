using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Bannerlord.TitlesForLords.Tests.tests.Core.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bannerlord.TitlesForLords.Tests.src.tests.Core.Settings.TitleConfig.TitleConfigElements {
	[TestClass]
	public class TitleConfigurationTests {

		static readonly TitleConfiguration FullProperties = JsonConvert.DeserializeObject<TitleConfiguration>(JsonObjectsForTests.FullTitleConfigurationProperties().ToString())!;
		static readonly TitleConfiguration RequiredOnlyDefaultProperties = JsonConvert.DeserializeObject<TitleConfiguration>(JsonObjectsForTests.RequiredOnlyTitleConfigurationProperties().ToString())!;

		[TestMethod]
		public void TestSerialization() {
			JsonConvert.SerializeObject(FullProperties, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			JsonConvert.SerializeObject(RequiredOnlyDefaultProperties, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			// only test if it throws, empty dictionaries are added as keys during serialization, therefore I can't compare via object.ToJsonString()
		}

		[TestMethod]
		public void TestCopyConstructor() {
			string newUid = "newUid";
			var copied = new TitleConfiguration(FullProperties, newUid, FullProperties.Options.IsDefault);
			var copiedAsJson = JObject.Parse(JsonConvert.SerializeObject(copied, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
			var fullPropertiesAsJsonString = JsonConvert.SerializeObject(FullProperties, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			Assert.AreEqual(newUid, copiedAsJson![nameof(TitleConfiguration.Metadata)]![nameof(Metadata.Uid)]!.ToString());
			copiedAsJson[nameof(TitleConfiguration.Metadata)]![nameof(Metadata.Uid)] = FullProperties.Metadata.Uid;
			JsonObjectsForTests.AssertJsonEquality(fullPropertiesAsJsonString, copiedAsJson.ToString());
		}
	}
}
