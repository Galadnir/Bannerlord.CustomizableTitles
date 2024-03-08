using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.Tests.tests.Core.Settings;
using Newtonsoft.Json;

namespace Bannerlord.TitlesForLords.Tests.src.tests.Core.Settings.TitleConfig.TitleConfigElements {
	[TestClass]
	public class VillagerPropertiesTests {

		static readonly VillagerProperties FullProperties = JsonConvert.DeserializeObject<VillagerProperties>(JsonObjectsForTests.FullVillagerProperties().ToString())!;

		static readonly VillagerProperties PartialProperties = JsonConvert.DeserializeObject<VillagerProperties>(JsonObjectsForTests.PartialVillagerProperties().ToString())!;

		[TestMethod]
		public void TestSerialization() {
			string fullPropertiesSerialized = JsonConvert.SerializeObject(FullProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			string partialPropertiesSerialized = JsonConvert.SerializeObject(PartialProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.FullVillagerProperties().ToString(), fullPropertiesSerialized);
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.PartialVillagerProperties().ToString(), partialPropertiesSerialized);
		}

		[TestMethod]
		public void TestMerging() {
			Assert.IsNull(VillagerProperties.MergeLowerPriorityVillagerPropertiesIntoHigher(null, null));
			Assert.IsFalse(Object.ReferenceEquals(VillagerProperties.MergeLowerPriorityVillagerPropertiesIntoHigher(PartialProperties, null), PartialProperties));
			Assert.AreEqual(PartialProperties, VillagerProperties.MergeLowerPriorityVillagerPropertiesIntoHigher(PartialProperties, null));
			Assert.IsFalse(Object.ReferenceEquals(VillagerProperties.MergeLowerPriorityVillagerPropertiesIntoHigher(null, FullProperties), FullProperties));
			Assert.AreEqual(FullProperties, VillagerProperties.MergeLowerPriorityVillagerPropertiesIntoHigher(null, FullProperties));

			VillagerProperties mergedProperties = VillagerProperties.MergeLowerPriorityVillagerPropertiesIntoHigher(PartialProperties, FullProperties);
			Assert.AreEqual(PartialProperties.VillagerPrefixOnCampaignMap, mergedProperties.VillagerPrefixOnCampaignMap);
			Assert.AreEqual(FullProperties.VillagerPostfixOnCampaignMap, mergedProperties.VillagerPostfixOnCampaignMap);
			Assert.AreEqual(FullProperties.VillagerInfixOnCampaignMap, mergedProperties.VillagerInfixOnCampaignMap);
			Assert.AreEqual(PartialProperties.ShowOwnerOfVillagers, mergedProperties.ShowOwnerOfVillagers);
			Assert.AreEqual(PartialProperties.ShowHomeSettlementBeforeOwner, mergedProperties.ShowHomeSettlementBeforeOwner);
			Assert.AreEqual(FullProperties.ShowHomeSettlementOfVillagers, mergedProperties.ShowHomeSettlementOfVillagers);
		}
	}
}
