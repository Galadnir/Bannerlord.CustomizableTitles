using Newtonsoft.Json;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	internal class IndividualCharacterProperties : ITitlePropertiesContainer {
		public TitleProperties Default { get; set; }
		public TitlesForTContainer<TitleProperties> SpecificClansContainer { get; }
		public TitlesForTContainer<TitleProperties> SpecificKingdomsContainer { get; }

		[JsonConstructor]
		public IndividualCharacterProperties(TitleProperties @default, TitlesForTContainer<TitleProperties> specificClansContainer, TitlesForTContainer<TitleProperties> specificKingdomsContainer) {
			this.Default = @default;
			this.SpecificClansContainer = specificClansContainer;
			this.SpecificKingdomsContainer = specificKingdomsContainer;
		}

		internal IndividualCharacterProperties(IndividualCharacterProperties properties) {
			Default = properties.Default != null ? new TitleProperties(properties.Default) : null;
			SpecificClansContainer = properties.SpecificClansContainer != null
				? new TitlesForTContainer<TitleProperties>(properties.SpecificClansContainer, titleProperties => new TitleProperties(titleProperties))
				: null;
			SpecificKingdomsContainer = properties.SpecificKingdomsContainer != null
				? new TitlesForTContainer<TitleProperties>(properties.SpecificKingdomsContainer, titleProperties => new TitleProperties(titleProperties))
				: null;
		}

		public bool TryGetTitlePropertiesFor(Hero hero, out TitleProperties titleProperties) {
			TitleProperties outProperties = null;
			TitleProperties clanProperties = null;
			TitleProperties kingdomProperties = null;
			TitleProperties defaultProperties = null;
			bool success = SpecificClansContainer?.TryGetTitlePropertiesFor(hero, (Hero lord) => lord.Clan?.Name.ToString(), out clanProperties, false) == true;
			success |= SpecificKingdomsContainer?.TryGetTitlePropertiesFor(hero, (Hero lord) => lord.Clan?.Kingdom?.Name.ToString(), out kingdomProperties, false) == true;
			success |= Default?.TryGetTitlePropertiesFor(hero, out defaultProperties) == true;
			if (success) {
				outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(clanProperties, kingdomProperties);
				outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(outProperties, defaultProperties);
			}
			titleProperties = outProperties ?? new TitleProperties();
			return success;
		}

		internal static IndividualCharacterProperties CreateEmpty() {
			TitleProperties createEmptyTitleProperties() => new TitleProperties();
			return new IndividualCharacterProperties(new TitleProperties(), TitlesForTContainer<TitleProperties>.CreateEmptyWithDefault(createEmptyTitleProperties), TitlesForTContainer<TitleProperties>.CreateEmptyWithDefault(createEmptyTitleProperties));
		}
	}
}
