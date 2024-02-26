
using Bannerlord.ButterLib.HotKeys;

namespace Bannerlord.TitlesForLords.src.main.Core.HotKeys {
	internal class PopUpDiscardHotkey : HotKeyBase {
		protected override string DisplayName { get; }
		protected override string Description { get; }
		protected override TaleWorlds.InputSystem.InputKey DefaultKey { get; }
		protected override string Category { get; }

		public PopUpDiscardHotkey() : base("Customizable_Titles_--_PopUp_Discard") {
			IsEnabled = false;

			DisplayName = "Customizable Titles Config Menu -- Discard Pop-Up";
			Description = "Discard a Pop-Up in the config menu for \"Customizable Titles\"";
			DefaultKey = TaleWorlds.InputSystem.InputKey.Escape;
			Category = HotKeyManager.Categories[HotKeyCategory.MenuShortcut];
		}
	}
}
