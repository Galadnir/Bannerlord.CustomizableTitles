using Bannerlord.ButterLib.HotKeys;

namespace Bannerlord.TitlesForLords.src.main.Core.HotKeys {

	internal class PopUpConfirmHotkey : HotKeyBase {
		protected override string DisplayName { get; }
		protected override string Description { get; }
		protected override TaleWorlds.InputSystem.InputKey DefaultKey { get; }
		protected override string Category { get; }

		public PopUpConfirmHotkey() : base("Customizable_Titles_--_PopUp_Confirm") {
			IsEnabled = false;

			DisplayName = "Customizable Titles Config Menu -- Confirm Pop-Up input";
			Description = "Confirm a Pop-Up input in the config menu for \"Customizable Titles\"";
			DefaultKey = TaleWorlds.InputSystem.InputKey.Enter;
			Category = HotKeyManager.Categories[HotKeyCategory.MenuShortcut];
		}
	}

}
