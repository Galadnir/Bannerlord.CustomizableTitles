using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Newtonsoft.Json.Linq;

namespace Bannerlord.TitlesForLords.Tests.tests.Core.Settings {
	internal static class JsonObjectsForTests {

		internal static JObject FullVillagerProperties() {
			return new JObject {
				[nameof(VillagerProperties.VillagerPrefixOnCampaignMap)] = "villager prefix",
				[nameof(VillagerProperties.VillagerPostfixOnCampaignMap)] = "villager postfix",
				[nameof(VillagerProperties.VillagerInfixOnCampaignMap)] = "villager infix",
				[nameof(VillagerProperties.ShowOwnerOfVillagers)] = true,
				[nameof(VillagerProperties.ShowHomeSettlementOfVillagers)] = true,
				[nameof(VillagerProperties.ShowHomeSettlementBeforeOwner)] = false
			};
		}

		internal static JObject PartialVillagerProperties() {
			return new JObject {
				[nameof(VillagerProperties.VillagerPrefixOnCampaignMap)] = "another villager prefix",
				[nameof(VillagerProperties.ShowOwnerOfVillagers)] = true,
				[nameof(VillagerProperties.ShowHomeSettlementBeforeOwner)] = false
			};
		}

		internal static JObject FullCaravanProperties() {
			return new JObject {
				[nameof(CaravanProperties.CaravanPrefixOnCampaignMap)] = "caravan prefix",
				[nameof(CaravanProperties.CaravanPostfixOnCampaignMap)] = "caravan postfix",
				[nameof(CaravanProperties.ShowNameOfOwner)] = true
			};
		}

		internal static JObject PartialCaravanProperties() {
			return new JObject {
				[nameof(CaravanProperties.CaravanPostfixOnCampaignMap)] = "another caravan postfix"
			};
		}

		internal static JObject FullTitleProperties() {
			return new JObject {
				[nameof(TitleProperties.TitleBeforeName)] = "before name",
				[nameof(TitleProperties.TitleAfterName)] = "after name",
				[nameof(TitleProperties.PartyPrefixOnCampaignMap)] = "",
				[nameof(TitleProperties.PartyPostfixOnCampaignMap)] = "'s party",
				[nameof(TitleProperties.ArmyPrefixOnCampaignMap)] = "army Prefix",
				[nameof(TitleProperties.ArmyPostfixOnCampaignMap)] = "army Postfix",
				[nameof(TitleProperties.OnlyApplyToClanLeader)] = false,
				[nameof(TitleProperties.ApplyToMercenaries)] = false,
				[nameof(TitleProperties.OwnedVillagers)] = FullVillagerProperties(),
				[nameof(TitleProperties.OwnedCaravans)] = FullCaravanProperties()
			};
		}

		internal static JObject PartialTitleProperties() {
			return new JObject {
				[nameof(TitleProperties.TitleBeforeName)] = "another title",
				[nameof(TitleProperties.PartyPostfixOnCampaignMap)] = "another postfix",
				[nameof(TitleProperties.ArmyPrefixOnCampaignMap)] = "another prefix"
			};
		}

		internal static JObject FullRankMemberProperties() {
			return new JObject {
				[nameof(RankMember.Default)] = FullTitleProperties(),
				[nameof(RankMember.Male)] = PartialTitleProperties(),
				[nameof(RankMember.Female)] = EmptyProperties()
			};
		}

		internal static JObject EmptyProperties() {
			return new JObject();
		}

		internal static JObject FullClanProperties() {
			return new JObject {
				[nameof(ClanProperties.ClanMemberDefault)] = FullRankMemberProperties(),
				[nameof(ClanProperties.ClanLeader)] = EmptyProperties(),
			};
		}

		internal static JObject FullKingdomProperties() {
			return new JObject {
				[nameof(KingdomProperties.ClanTiers)] = new JObject {
					["0"] = FullClanProperties(),
					["-1"] = EmptyProperties(),
					["-50"] = FullClanProperties(),
					["1"] = FullClanProperties(),
					["60"] = EmptyProperties()
				},
				[nameof(KingdomProperties.Default)] = FullClanProperties(),
				[nameof(KingdomProperties.RulingClan)] = EmptyProperties(),
				[nameof(KingdomProperties.RulerProperties)] = FullRankMemberProperties()
			};
		}

		internal static JObject FullTitlesForKingdomsProperties() {
			return new JObject {
				[nameof(TitlesForKingdoms.Default)] = new JObject(),
				[nameof(TitlesForKingdoms.CultureProperties)] = new JObject {
					[nameof(TitlesForTContainer<KingdomProperties>.Properties)] = new JObject {
						["Battania"] = FullKingdomProperties(),
						["Empire"] = EmptyProperties()
					}
				},
				[nameof(TitlesForKingdoms.ExplicitKingdomProperties)] = new JObject {
					[nameof(TitlesForTContainer<KingdomProperties>.Properties)] = new JObject {
						["Vlandia"] = FullKingdomProperties(),
						["Khuzait"] = EmptyProperties()
					}
				}
			};
		}

		internal static JObject FullTitlesForClansProperties() {
			return new JObject {
				[nameof(TitlesForClans.Default)] = FullClanProperties(),
				[nameof(TitlesForClans.Clans)] = new JObject {
					["clan"] = new JObject {
						[nameof(TitlesForTContainer<ClanProperties>.Default)] = FullClanProperties(),
						[nameof(TitlesForTContainer<ClanProperties>.Properties)] = new JObject {
							["Battania"] = EmptyProperties(),
							["Vlandia"] = FullClanProperties()
						}
					},
					["anotherClan"] = new JObject {
						[nameof(TitlesForTContainer<ClanProperties>.Default)] = EmptyProperties(),
						[nameof(TitlesForTContainer<ClanProperties>.Properties)] = new JObject {
							["Khuzait"] = EmptyProperties()
						}
					},
					["lastClan"] = new JObject {
						[nameof(TitlesForTContainer<ClanProperties>.Properties)] = new JObject {
							["Sturgia"] = FullClanProperties()
						}
					}
				}
			};
		}

		internal static JObject FullIndividualCharacterProperties() {
			return new JObject {
				[nameof(IndividualCharacterProperties.Default)] = PartialTitleProperties(),
				[nameof(IndividualCharacterProperties.SpecificClansContainer)] = new JObject {
					[nameof(TitlesForTContainer<TitleProperties>.Properties)] = new JObject {
						["clan1"] = FullTitleProperties(),
						["anotherClan"] = EmptyProperties(),
						["lastClan"] = PartialTitleProperties()
					}
				},
				[nameof(TitlesForTContainer<TitleProperties>)] = new JObject {
					[nameof(TitlesForTContainer<TitleProperties>.Properties)] = new JObject {
						["Khuzait"] = PartialTitleProperties(),
						["Vlandia"] = FullTitleProperties(),
						["Northern Empire"] = EmptyProperties()
					}
				}
			};
		}

		internal static JObject FullTitlesForCharactersProperties() {
			return new JObject {
				[nameof(TitlesForCharacters.Default)] = EmptyProperties(),
				[nameof(TitlesForCharacters.Characters)] = new JObject {
					["firstCharacter"] = FullIndividualCharacterProperties(),
					["secondCharacter"] = EmptyProperties()
				}
			};
		}

		internal static JObject FullLordTitlesProperties() {
			return new JObject {
				[nameof(LordTitles.Default)] = FullRankMemberProperties(),
				[nameof(LordTitles.TitlesForKingdoms)] = FullTitlesForKingdomsProperties(),
				[nameof(LordTitles.TitlesForClans)] = FullTitlesForClansProperties(),
				[nameof(LordTitles.TitlesForCharacters)] = FullTitlesForCharactersProperties()
			};
		}

		internal static JObject FullNotableOwnedCaravansProperties() {
			return new JObject {
				[nameof(NotableOwnedCaravans.Default)] = FullCaravanProperties(),
				[nameof(NotableOwnedCaravans.NotableProperties)] = new JObject {
					["notable1"] = FullCaravanProperties(),
					["notable2"] = EmptyProperties()
				},
				[nameof(NotableOwnedCaravans.KingdomProperties)] = new JObject {
					["Battania"] = FullCaravanProperties(),
					["Vlandia"] = EmptyProperties()
				},
				[nameof(NotableOwnedCaravans.CultureProperties)] = new JObject {
					["Battania"] = FullCaravanProperties(),
					["Vlandia"] = EmptyProperties()
				}
			};
		}

		internal static JObject RequiredOnlyTitleConfigurationProperties() {
			return new JObject {
				[nameof(TitleConfiguration.Metadata)] = new JObject() {
					[nameof(Metadata.Uid)] = "requiredUid",
					[nameof(Metadata.Name)] = "name",
					[nameof(Metadata.SubModule)] = "SubModule"
				},
				[nameof(TitleConfiguration.Options)] = new JObject() {
					[nameof(ConfigOptions.IsActive)] = true,
					[nameof(ConfigOptions.IsDefault)] = false
				}
			};
		}

		internal static JObject FullTitleConfigurationProperties() {
			var json = JObject.Parse(RequiredOnlyTitleConfigurationProperties().ToString());
			json![nameof(TitleConfiguration.Metadata)]![nameof(Metadata.Uid)] = "fullUid";
			var options = json[nameof(TitleConfiguration.Options)];
			options![nameof(ConfigOptions.IsDefault)] = true;
			json[nameof(TitleConfiguration.TitlesForLords)] = FullLordTitlesProperties();
			json[nameof(TitleConfiguration.NotableCaravans)] = FullNotableOwnedCaravansProperties();
			return json;
		}

		internal static void AssertJsonEquality(string expected, string actual) {
			var o1 = JObject.Parse(expected);
			var o2 = JObject.Parse(actual);
			Assert.IsTrue(JToken.DeepEquals(o1, o2), $"expected: {o1}\nactual: {o2}");
		}
	}
}