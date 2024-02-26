using Bannerlord.ButterLib.HotKeys;

namespace Bannerlord.TitlesForLords.src.main.Core.HotKeys {
	internal class NavigateForwardsHotkey : HotKeyBase {

		protected override string DisplayName { get; }
		protected override string Description { get; }
		protected override TaleWorlds.InputSystem.InputKey DefaultKey { get; }
		protected override string Category { get; }

		public NavigateForwardsHotkey() : base("Customizable_Titles_--_Navigate_Forwards") {
			IsEnabled = false;

			DisplayName = "Customizable Titles Config Menu -- Navigate Forwards";
			Description = "Navigate to the previous page in the config menu for \"Customizable Titles\"";
			DefaultKey = TaleWorlds.InputSystem.InputKey.X2MouseButton;
			Category = HotKeyManager.Categories[HotKeyCategory.MenuShortcut];
		}
	}
}
