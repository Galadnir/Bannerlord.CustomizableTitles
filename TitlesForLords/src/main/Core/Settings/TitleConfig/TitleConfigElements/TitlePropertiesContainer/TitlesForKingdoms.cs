using Newtonsoft.Json;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	internal class TitlesForKingdoms : ITitlePropertiesContainer {

		public KingdomProperties Default { get; set; }
		public TitlesForTContainer<KingdomProperties> CultureProperties { get; set; }
		public TitlesForTContainer<KingdomProperties> ExplicitKingdomProperties { get; set; }

		[JsonConstructor]
		public TitlesForKingdoms(KingdomProperties @default, TitlesForTContainer<KingdomProperties> cultureProperties, TitlesForTContainer<KingdomProperties> explicitKingdomProperties) {
			this.Default = @default;
			this.CultureProperties = cultureProperties;
			this.ExplicitKingdomProperties = explicitKingdomProperties;
		}

		internal TitlesForKingdoms(TitlesForKingdoms titlesForKingdoms) {
			Default = titlesForKingdoms.Default != null ? new KingdomProperties(titlesForKingdoms.Default) : null;
			CultureProperties = titlesForKingdoms.CultureProperties != null ?
				new TitlesForTContainer<KingdomProperties>(titlesForKingdoms.CultureProperties, properties => new KingdomProperties(properties)) : null;
			ExplicitKingdomProperties = titlesForKingdoms.ExplicitKingdomProperties != null ?
				new TitlesForTContainer<KingdomProperties>(titlesForKingdoms.ExplicitKingdomProperties, properties => new KingdomProperties(properties)) : null;
		}

		private TitlesForKingdoms() {
			Default = KingdomProperties.CreateEmpty();
			CultureProperties = TitlesForTContainer<KingdomProperties>.CreateEmptyWithoutDefault();
			ExplicitKingdomProperties = TitlesForTContainer<KingdomProperties>.CreateEmptyWithoutDefault();
		}

		public bool TryGetTitlePropertiesFor(Hero hero, out TitleProperties titleProperties) {
			TitleProperties culture = null;
			TitleProperties kingdom = null;
			TitleProperties outProperties = null;
			bool success = CultureProperties?.TryGetTitlePropertiesFor(hero, (Hero lord) => lord.Clan.Kingdom.Culture.Name.ToString(), out culture, false) == true;
			success |= ExplicitKingdomProperties?.TryGetTitlePropertiesFor(hero, (Hero lord) => lord.Clan.Kingdom.Name.ToString(), out kingdom, false) == true;
			if (success) {
				outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(kingdom, culture);
				if (Default?.TryGetTitlePropertiesFor(hero, out TitleProperties defaultProperties) == true) {
					outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(outProperties, defaultProperties);
				}
			}
			titleProperties = outProperties ?? new TitleProperties();
			return success;
		}

		internal bool WouldUpdateDueToKingdomNameChange(string oldName, string newName) {
			return (ExplicitKingdomProperties?.Properties?.ContainsKey(oldName) == true) && (ExplicitKingdomProperties?.Properties?.ContainsKey(newName) == false);
		}

		internal void UpdateDueToKingdomNameChange(string oldName, string newName) {
			if (WouldUpdateDueToKingdomNameChange(oldName, newName)) {
				ExplicitKingdomProperties.AddProperty(newName, ExplicitKingdomProperties.Properties[oldName]);
			}
		}

		internal static TitlesForKingdoms CreateEmpty() {
			return new TitlesForKingdoms();
		}
	}
}
