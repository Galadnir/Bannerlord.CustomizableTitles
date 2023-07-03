using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.Tests.tests.Core.Settings;
using Newtonsoft.Json;

namespace Bannerlord.TitlesForLords.Tests.src.tests.Core.Settings.TitleConfig.TitleConfigElements {
	[TestClass]
	public class CaravanPropertiesTests {

		static readonly CaravanProperties FullProperties = JsonConvert.DeserializeObject<CaravanProperties>(JsonObjectsForTests.FullCaravanProperties().ToString())!;

		static readonly CaravanProperties PartialProperties = JsonConvert.DeserializeObject<CaravanProperties>(JsonObjectsForTests.PartialCaravanProperties().ToString())!;

		[TestMethod]
		public void TestSerialization() {
			string fullPropertiesSerialized = JsonConvert.SerializeObject(FullProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			string partialPropertiesSerialized = JsonConvert.SerializeObject(PartialProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.FullCaravanProperties().ToString(), fullPropertiesSerialized);
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.PartialCaravanProperties().ToString(), partialPropertiesSerialized);
		}

		[TestMethod]
		public void TestMerging() {
			Assert.IsNull(CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(null, null));
			Assert.IsFalse(Object.ReferenceEquals(CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(PartialProperties, null), PartialProperties));
			Assert.AreEqual(PartialProperties, CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(PartialProperties, null));
			Assert.IsFalse(Object.ReferenceEquals(CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(null, FullProperties), FullProperties));
			Assert.AreEqual(FullProperties, CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(null, FullProperties));

			CaravanProperties mergedProperties = CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(PartialProperties, FullProperties);
			Assert.AreEqual(FullProperties.CaravanPrefixOnCampaignMap, mergedProperties.CaravanPrefixOnCampaignMap);
			Assert.AreEqual(PartialProperties.CaravanPostfixOnCampaignMap, mergedProperties.CaravanPostfixOnCampaignMap);
			Assert.AreEqual(FullProperties.ShowNameOfOwner, mergedProperties.ShowNameOfOwner);
		}
	}
}
