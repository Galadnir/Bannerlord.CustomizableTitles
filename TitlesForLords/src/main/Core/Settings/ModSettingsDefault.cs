using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using System;
using System.Collections.Generic;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings {
	internal static class ModSettingsDefault {
		internal const ModVersion Version = ModSettings.CurrentVersion;
		internal const bool TrackNameAndClanChanges = true;
		internal const bool CopyConfigOnNameOrClanChange = false;
		internal const bool UpdateAllConfigsOnNameOrClanChange = false;
		internal const bool ApplyTitleConfigToPlayer = true;
		internal const bool ApplyTitleConfigToPlayerCompanions = false;
		internal const bool ApplyToPlayerCaravans = false;
		internal const TitleProperties GlobalDefault = null;
		internal static Dictionary<string, HashSet<string>> SubModuleToCultures = new Dictionary<string, HashSet<string>> {
			["Base Game"] = new HashSet<string> {
				"Vlandia",
				"Battania",
				"Khuzait",
				"Sturgia",
				"Aserai",
				"Empire"
			}
		};
		internal static Dictionary<string, HashSet<string>> SubModuleToKingdoms = new Dictionary<string, HashSet<string>> {
			["Base Game"] = new HashSet<string> {
				"Vlandia",
				"Battania",
				"Khuzait",
				"Sturgia",
				"Aserai",
				"Northern Empire",
				"Western Empire",
				"Southern Empire"
			}
		};
		internal static readonly List<TitleConfiguration> TitleConfigs = new List<TitleConfiguration> {
			new TitleConfiguration(Metadata, Options, LordTitles, NotableOwnedCaravans.CreateEmpty())
		};

		static Metadata Metadata => new Metadata("TitleOverhaulDefaultConfig", "Calradian Titles", "Base Game", new List<Tuple<string, string>>());
		static ConfigOptions Options => new ConfigOptions(true, true);
		static LordTitles LordTitles => new LordTitles(
			new RankMember(
				new TitleProperties(),
				new TitleProperties("Lord ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
				new TitleProperties("Lady ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
				),
			new TitlesForKingdoms(
				new KingdomProperties(
					new Dictionary<int, ClanProperties>(),
					ClanProperties.CreateEmpty(),
					new RankMember(
						new TitleProperties(),
						new TitleProperties("King ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
						new TitleProperties("Queen ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
						),
					RankMember.CreateEmpty(),
					RankMember.CreateEmpty(),
					ClanProperties.CreateEmpty()
					),
				new TitlesForTContainer<KingdomProperties>(
					new Dictionary<string, KingdomProperties> {
						["Vlandia"] = new KingdomProperties(
							new Dictionary<int, ClanProperties> {
								[6] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Grand Duke ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Grand Duchess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[5] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Grand Duke ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Grand Duchess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[4] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Duke ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Duchess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[3] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Count ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Countess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[2] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Baron ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Baroness ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[1] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Baron ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Baroness ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[0] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Baron ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Baroness ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									)
							},
							ClanProperties.CreateEmpty(),
							new RankMember(
								new TitleProperties(),
								new TitleProperties("King ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
								new TitleProperties("Queen ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
								),
							RankMember.CreateEmpty(),
							new RankMember(
								new TitleProperties(),
								new TitleProperties("Prince ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
								new TitleProperties("Princess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
								),
							ClanProperties.CreateEmpty()
							),
						["Battania"] = new KingdomProperties(
							new Dictionary<int, ClanProperties> {
								[6] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Petty King ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Petty Queen ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[5] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Petty King ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Petty Queen ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[4] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("High Chief ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("High Chieftess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[3] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Chief ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Chieftess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[2] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Chieftain ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Chieftainess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[1] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Chieftain ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Chieftainess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[0] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Chieftain ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Chieftainess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									)
							},
							ClanProperties.CreateEmpty(),
							new RankMember(
								new TitleProperties(),
								new TitleProperties("High King ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
								new TitleProperties("High Queen ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
								),
							RankMember.CreateEmpty(),
							RankMember.CreateEmpty(),
							ClanProperties.CreateEmpty()
							),
						["Empire"] = new KingdomProperties(
							new Dictionary<int, ClanProperties> {
								[6] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Consul ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Consul ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[5] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Consul ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Consul ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[4] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Magistrate ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Magistrate ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[3] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Proconsul ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Proconsul ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[2] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Patrician ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Patrician ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[1] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Patrician ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Patrician ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[0] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Patrician ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Patrician ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									)
							},
							ClanProperties.CreateEmpty(),
							new RankMember(
								new TitleProperties(),
								new TitleProperties("Emperor ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
								new TitleProperties("Empress ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
								),
							RankMember.CreateEmpty(),
							new RankMember(
								new TitleProperties(),
								new TitleProperties("Prince ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
								new TitleProperties("Princess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
								),
							ClanProperties.CreateEmpty()
							),
						["Sturgia"] = new KingdomProperties(
							new Dictionary<int, ClanProperties> {
								[6] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Prince ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Princess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[5] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Prince ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Princess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[4] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Boyar ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Boyar ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[3] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Castellan ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Castellan ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[2] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Chieftain ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Chieftainess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[1] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Chieftain ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Chieftainess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[0] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Chieftain ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Chieftainess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									)
							},
							ClanProperties.CreateEmpty(),
							new RankMember(
								new TitleProperties(),
								new TitleProperties("Grand Prince ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
								new TitleProperties("Grand Princess ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
								),
							RankMember.CreateEmpty(),
							RankMember.CreateEmpty(),
							ClanProperties.CreateEmpty()
							),
						["Khuzait"] = new KingdomProperties(
							new Dictionary<int, ClanProperties> {
								[6] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties(null, " Noyan", null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties(null, " Noyan", null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[5] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties(null, " Noyan", null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties(null, " Noyan", null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[4] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties(null, " Noyon", null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties(null, " Noyon", null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[3] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Cherbi ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Cherbi ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[2] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Baig ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Baig ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[1] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Baig ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Baig ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[0] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Baig ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Baig ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									)
							},
							new ClanProperties(
								new RankMember(
									new TitleProperties("", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()), // to overwrite default
									new TitleProperties(),
									new TitleProperties()
									),
								RankMember.CreateEmpty()),
							new RankMember(
								new TitleProperties(),
								new TitleProperties(null, " Khan", null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
								new TitleProperties(null, " Khatun", null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
								),
							RankMember.CreateEmpty(),
							new RankMember(
								new TitleProperties(),
								new TitleProperties(null, " Khanzadeh", null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
								new TitleProperties(null, " Khanzada", null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
								),
							ClanProperties.CreateEmpty()
							),
						["Aserai"] = new KingdomProperties(
							new Dictionary<int, ClanProperties> {
								[6] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Emir ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Emira ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[5] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Emir ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Emira ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[4] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Shaikh ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Shaykhah ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[3] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("bey ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("begum ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[2] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Agha ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Khanum ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[1] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Agha ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Khanum ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									),
								[0] = new ClanProperties(
									new RankMember(
										new TitleProperties(),
										new TitleProperties("Agha ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
										new TitleProperties("Khanum ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
										),
									RankMember.CreateEmpty()
									)
							},
							ClanProperties.CreateEmpty(),
							new RankMember(
								new TitleProperties(),
								new TitleProperties("Sultan ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
								new TitleProperties("Sultana ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
								),
							RankMember.CreateEmpty(),
							new RankMember(
								new TitleProperties(),
								new TitleProperties("Shahzade ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties()),
								new TitleProperties("Shehzadi ", null, null, null, null, null, null, null, new VillagerProperties(), new CaravanProperties())
								),
							ClanProperties.CreateEmpty()
							),
					},
					KingdomProperties.CreateEmpty()
					),
				TitlesForTContainer<KingdomProperties>.CreateEmptyWithoutDefault()
				),
			new TitlesForClans(new Dictionary<string, TitlesForTContainer<ClanProperties>>(), ClanProperties.CreateEmpty()),
			new TitlesForCharacters(new Dictionary<string, IndividualCharacterProperties>(), new TitleProperties())
			);
	}
}
