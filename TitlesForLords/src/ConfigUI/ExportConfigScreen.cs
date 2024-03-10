using Bannerlord.TitleOverhaul.src.ConfigUI.ExportConfigVMs;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.ScreenSystem;

namespace Bannerlord.TitleOverhaul.src.ConfigUI {
	internal class ExportConfigScreen {

		readonly ExportConfigVM _vm;

		internal GauntletLayer SelectConfigLayer { get; }
		internal GauntletLayer InputModNameAndUIDLayer { get; private set; }
		internal GauntletLayer AddKingdomsAndCulturesLayer { get; }
		IGauntletMovie SelectConfigMovie { get; }
		IGauntletMovie InputModNameAndUIDMovie { get; set; }
		IGauntletMovie AddKingdomsAndCulturesMovie { get; }

		internal ExportConfigScreen() {
			ModSettings.Instance.Restore(); // in case there are unsaved changes from playing
			_vm = new ExportConfigVM(this);
			this.SelectConfigLayer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			this.AddKingdomsAndCulturesLayer = new GauntletLayer(LayerPriority.AddKingdomsAndCulturesLayer, "GauntletLayer", true);
			this.SelectConfigMovie = this.SelectConfigLayer.LoadMovie("CTSelectModConfig", _vm);
			this.AddKingdomsAndCulturesMovie = this.AddKingdomsAndCulturesLayer.LoadMovie("CTExportKingdomsAndCultures", _vm);
			ActivateLayer(this.SelectConfigLayer);
		}

		internal void Close() {
			SelectConfigLayer.ReleaseMovie(SelectConfigMovie);
			AddKingdomsAndCulturesLayer.ReleaseMovie(AddKingdomsAndCulturesMovie);
			ScreenManager.TopScreen.RemoveLayer(SelectConfigLayer);
			ScreenManager.TopScreen.RemoveLayer(AddKingdomsAndCulturesLayer);
		}

		internal void ActivateLayer(GauntletLayer layer) {
			layer.InputRestrictions.SetInputRestrictions();
			layer.IsFocusLayer = true;
			ScreenManager.TopScreen.AddLayer(layer);
			ScreenManager.TrySetFocus(layer);
		}

		internal void CreateSelectedConfigPopUp() {
			this.InputModNameAndUIDLayer = new GauntletLayer(LayerPriority.InputModNameAndUIDLayer);
			this.InputModNameAndUIDMovie = this.InputModNameAndUIDLayer.LoadMovie("CTModNameAndUIDPopUp", _vm);
		}

		internal void TearDownSelectedConfigPopUp() {
			InputModNameAndUIDLayer.ReleaseMovie(InputModNameAndUIDMovie);
			ScreenManager.TopScreen.RemoveLayer(InputModNameAndUIDLayer);
		}
	}
}
