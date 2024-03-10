using Bannerlord.ButterLib.HotKeys;
using Bannerlord.TitleOverhaul.src.ConfigUI;
using Bannerlord.TitlesForLords.src.main.Core.HotKeys;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using MCM.Abstractions.FluentBuilder;
using MCM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.TitlesForLords.src.main.Core {
	internal class TitlesForLordsSubModule : MBSubModuleBase {

		public static bool IsOptionsMenuOpen;

		public const string HotkeyCategoryName = "Customizable Titles -- Config Menu Hotkeys";

		internal static HotKeyManager HotKeyManager;
		internal static NavigateBackwardsHotkey NavigateBackwardsHotkey;
		internal static NavigateForwardsHotkey NavigateForwardsHotkey;
		internal static PopUpConfirmHotkey PopUpConfirmHotkey;
		internal static PopUpDiscardHotkey PopUpDiscardHotkey;

		private const string _failedToLoadText = "Unfortunately, attempting to load my UI here simply loads a black screen. Please access the options either through the the options menu while in a campaign or through \"Main Menu => Mod Options\"";

		private bool firstModuleScreen = true;

		protected override void OnSubModuleLoad() {
			var harmony = new HarmonyLib.Harmony("de.galadnir.Bannerlord.TitleOverhaul");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot() {
			if (firstModuleScreen) { // module screen is set as root again after exiting a campaign
				foreach (string name in ModSettings.Instance.LoadAndSaveNewJsonConfigFiles()) {
					InformationManager.DisplayMessage(new InformationMessage($"Loaded TitleOverhaul Config: {name}"));
				}
				CreateHotkeys();

				firstModuleScreen = false;
			}

			var builder = BaseSettingsBuilder.Create("Customizable_Titles_Settings", "Customizable Titles")
				.SetFormat("none")
				.SetFolderName(string.Empty)
				.SetSubFolder(string.Empty)
				.CreateGroup("", groupBuilder => groupBuilder
					.AddButton("Customizable_Titles_List_Current_Kingdoms_And_Cultures", "List current Kingdoms and Cultures", new ProxyRef<Action>(() =>
					() => {
						if (Campaign.Current is null) {
							InformationManager.DisplayMessage(new InformationMessage("Error, you must be in a campaign and then access this menu through \"Escape => Options => Mod Options => Customizable Titles\""));
						} else {
							var kingdoms = Campaign.Current.MobileParties.Select(party => party.LeaderHero?.Clan?.Kingdom?.Name.ToString()).Distinct().ToList();
							kingdoms.Remove(null);
							kingdoms.Sort();
							var cultures = Campaign.Current.MobileParties.Select(party => party.LeaderHero?.Clan?.Kingdom?.Culture?.Name.ToString()).Distinct().ToList();
							cultures.Remove(null);
							cultures.Sort();
							InformationManager.DisplayMessage(new InformationMessage($"Kingdoms: {string.Join(", ", kingdoms)}"));
							InformationManager.DisplayMessage(new InformationMessage($"Cultures: {string.Join(", ", cultures)}"));
							ModSettings.Instance.RegisterLastListedKingdomsAndCultures(new HashSet<string>(kingdoms), new HashSet<string>(cultures));
						}
					}, _ => { })
					, "Create lists",
					boolBuilder => boolBuilder
					.SetHintText("List all kingdoms and cultures present in your current campaign")
					.SetRequireRestart(false)
					.SetOrder(3))
					.AddButton("Customizalbe_Titles_Export_Config_UI", "Export Configuration", new ProxyRef<Action>(() =>
					() => {
						if (IsOptionsMenuOpen && Campaign.Current is null) {
							InformationManager.DisplayMessage(new InformationMessage(_failedToLoadText));
							return;
						}
						if (!(Campaign.Current is null)) {
							InformationManager.DisplayMessage(new InformationMessage("Opening this menu reloads the current settings which can discard potential changes caused by tracking name changes. As such, this menu is only accessible through the game's main menu."));
							return;
						}
						var screen = new ExportConfigScreen();
					}, _ => { }),
					"Open Export Menu",
					boolBuilder => boolBuilder
					.SetHintText("Export a title configuration to your desktop to include in your mod")
					.SetRequireRestart(false)
					.SetOrder(2))
					.AddButton("Activate_Customizable_Titles_UI", "Open Customizable Titles settings menu", new ProxyRef<Action>(() =>
					() => {
						if (IsOptionsMenuOpen && Campaign.Current is null) {
							InformationManager.DisplayMessage(new InformationMessage(_failedToLoadText));
							return;
						}
						if (!(Campaign.Current is null) && ModSettings.Instance.TrackAllNameChanges) {
							InformationManager.ShowInquiry(new InquiryData(
								"Warning",
								"Warning, if you have \"Track Name Changes\" enabled, then editing your configurations while in a campaign can cause inconsistencies, because unsaved changes are discarded upon entering this menu. This should only happen rarely though. Enter anyway?\n\n" +
								 "Note: This warning only appears if you have \"Track Name Changes\" enabled.",
								true, true, "Yes", "No",
								() => new ConfigUIScreen(),
								() => { }
								));
						} else {
							var screen = new ConfigUIScreen();
						}
					}, _ => { }),
					"Open Config Menu",
					boolBuilder => boolBuilder
					.SetHintText("opens the settings menu for Customizable Titles")
					.SetRequireRestart(false)
					.SetOrder(1)));

			var globalSettings = builder.BuildAsGlobal();
			globalSettings.Register();
		}

		private void CreateHotkeys() {
			HotKeyManager = HotKeyManager.Create("Customizable Titles");
			NavigateBackwardsHotkey = HotKeyManager.Add<NavigateBackwardsHotkey>();
			NavigateForwardsHotkey = HotKeyManager.Add<NavigateForwardsHotkey>();
			PopUpConfirmHotkey = HotKeyManager.Add<PopUpConfirmHotkey>();
			PopUpDiscardHotkey = HotKeyManager.Add<PopUpDiscardHotkey>();
			HotKeyManager.Build();
		}
	}
}
