using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.Tests.tests.Core.Settings;
using Newtonsoft.Json;

namespace Bannerlord.TitlesForLords.Tests.src.tests.Core.Settings.TitleConfig.TitleConfigElements {

	[TestClass]
	public class TitlePropertiesTests {

		static readonly TitleProperties FullProperties = JsonConvert.DeserializeObject<TitleProperties>(JsonObjectsForTests.FullTitleProperties().ToString())!;
		static readonly TitleProperties PartialProperties = JsonConvert.DeserializeObject<TitleProperties>(JsonObjectsForTests.PartialTitleProperties().ToString())!;

		[TestMethod]
		public void TestSerialization() {
			string fullPropertiesSerialized = JsonConvert.SerializeObject(FullProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			string partialPropertiesSerialized = JsonConvert.SerializeObject(PartialProperties, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.FullTitleProperties().ToString(), fullPropertiesSerialized);
			JsonObjectsForTests.AssertJsonEquality(JsonObjectsForTests.PartialTitleProperties().ToString(), partialPropertiesSerialized);
		}

		[TestMethod]
		public void TestMerging() {
			Assert.IsNull(TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(null, null));
			Assert.IsFalse(Object.ReferenceEquals(TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(PartialProperties, null), PartialProperties));
			Assert.AreEqual(PartialProperties, TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(PartialProperties, null));
			Assert.IsFalse(Object.ReferenceEquals(TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(null, FullProperties), FullProperties));
			Assert.AreEqual(FullProperties, TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(null, FullProperties));

			var mergedProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(PartialProperties, FullProperties);
			Assert.AreEqual(PartialProperties.TitleBeforeName, mergedProperties.TitleBeforeName);
			Assert.AreEqual(FullProperties.TitleAfterName, mergedProperties.TitleAfterName);
			Assert.AreEqual(FullProperties.PartyPrefixOnCampaignMap, mergedProperties.PartyPrefixOnCampaignMap);
			Assert.AreEqual(PartialProperties.PartyPostfixOnCampaignMap, mergedProperties.PartyPostfixOnCampaignMap);
			Assert.AreEqual(PartialProperties.ArmyPrefixOnCampaignMap, mergedProperties.ArmyPrefixOnCampaignMap);
			Assert.AreEqual(FullProperties.ArmyPostfixOnCampaignMap, mergedProperties.ArmyPostfixOnCampaignMap);
			Assert.AreEqual(FullProperties.OnlyApplyToClanLeader, mergedProperties.OnlyApplyToClanLeader);
			Assert.AreEqual(FullProperties.ApplyToMercenaries, mergedProperties.ApplyToMercenaries);
			Assert.AreEqual(FullProperties.OwnedVillagers, mergedProperties.OwnedVillagers);
			Assert.AreEqual(FullProperties.OwnedCaravans, mergedProperties.OwnedCaravans);
		}
	}
}
