using Bannerlord.TitleOverhaul.src.ConfigUI;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using MCM.Abstractions.FluentBuilder;
using MCM.Common;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.TitlesForLords.src.main.Core {
	internal class TitlesForLordsSubModule : MBSubModuleBase {

		public static bool IsOptionsMenuOpen;

		private static string _failedToLoadText = "Unfortunately, attempting to load my UI here simply loads a black screen. Please access the options either through the the options menu while in a campaign or through \"Main Menu => Mod Options\"";


		bool firstModuleScreen = true;

		protected override void OnSubModuleLoad() {
			var harmony = new HarmonyLib.Harmony("de.galadnir.Bannerlord.TitleOverhaul");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot() {
			if (firstModuleScreen) { // module screen is set as root again after exiting a campaign
				foreach (string name in ModSettings.Instance.LoadAndSaveNewJsonConfigFiles()) {
					InformationManager.DisplayMessage(new InformationMessage($"Loaded TitleOverhaul Config: {name}"));
				}
				firstModuleScreen = false;
			}
			var builder = BaseSettingsBuilder.Create("Customizable_Titles_Settings", "Customizable Titles")
				.SetFormat("none")
				.SetFolderName(string.Empty)
				.SetSubFolder(string.Empty)
				.CreateGroup("", groupBuilder => groupBuilder
					.AddBool("Customizalbe_Titles_Export_Config_UI", "Export Configuration", new ProxyRef<bool>(() => false, o => {
						if (IsOptionsMenuOpen && Campaign.Current is null) {
							InformationManager.DisplayMessage(new InformationMessage(_failedToLoadText));
							return;
						}
						var screen = new ExportConfigScreen();
					}),
					boolBuilder => boolBuilder
					.SetHintText("Export a title configuration to your desktop to include in your mod")
					.SetRequireRestart(false)
					.SetOrder(2))
					.AddBool("Activate_Customizable_Titles_UI", "Open Customizable Titles settings menu", new ProxyRef<bool>(() => false, o => {
						if (IsOptionsMenuOpen && Campaign.Current is null) {
							InformationManager.DisplayMessage(new InformationMessage(_failedToLoadText));
							return;
						}
						var screen = new ConfigUIScreen();
					}),
					boolBuilder => boolBuilder
					.SetHintText("opens the settings menu for Customizable Titles")
					.SetRequireRestart(false)
					.SetOrder(1)));

			var globalSettings = builder.BuildAsGlobal();
			globalSettings.Register();
		}
	}
}
