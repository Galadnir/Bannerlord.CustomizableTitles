using Bannerlord.ButterLib.HotKeys;

namespace Bannerlord.TitlesForLords.src.main.Core {
	internal class ForceExitCustomizationUiHotkey : HotKeyBase {

		protected override string DisplayName { get; }
		protected override string Description { get; }
		protected override TaleWorlds.InputSystem.InputKey DefaultKey { get; }
		protected override string Category { get; }

		public ForceExitCustomizationUiHotkey() : base("Customizable_Titles_--_Force_Exit_Customization_Ui_Hotkey") {
			IsEnabled = false;

			DisplayName = "Customizable Titles Config Menu -- Exit UI";
			Description = "Exit the configuration menu for Customizable Titles";
			DefaultKey = TaleWorlds.InputSystem.InputKey.Escape;
			Category = HotKeyManager.Categories[HotKeyCategory.MenuShortcut];
		}
	}
}