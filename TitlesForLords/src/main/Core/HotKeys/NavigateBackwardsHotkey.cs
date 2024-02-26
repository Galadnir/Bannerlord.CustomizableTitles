using Bannerlord.ButterLib.HotKeys;

namespace Bannerlord.TitlesForLords.src.main.Core {
	internal class NavigateBackwardsHotkey : HotKeyBase {

		protected override string DisplayName { get; }
		protected override string Description { get; }
		protected override TaleWorlds.InputSystem.InputKey DefaultKey { get; }
		protected override string Category { get; }

		public NavigateBackwardsHotkey() : base("Customizable_Titles_--_Navigate_Backwards") {
			IsEnabled = false;

			DisplayName = "Customizable Titles Config Menu -- Navigate Backwards";
			Description = "Navigate to the previous page in the config menu for \"Customizable Titles\"";
			DefaultKey = TaleWorlds.InputSystem.InputKey.X1MouseButton;
			Category = HotKeyManager.Categories[HotKeyCategory.MenuShortcut];
		}
	}
}
