using Bannerlord.TitleOverhaul.src.ConfigUI.ExportConfigVMs;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.ScreenSystem;

namespace Bannerlord.TitleOverhaul.src.ConfigUI {
	internal class ExportConfigScreen : ScreenBase {

		readonly ScreenBase _previousScreen;
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
			this.SelectConfigLayer = new GauntletLayer(0, "GauntletLayer", true);
			this.AddKingdomsAndCulturesLayer = new GauntletLayer(20, "GauntletLayer", true);
			_previousScreen = ScreenManager.TopScreen;
			this.SelectConfigMovie = this.SelectConfigLayer.LoadMovie("CTSelectModConfig", _vm);
			this.AddKingdomsAndCulturesMovie = this.AddKingdomsAndCulturesLayer.LoadMovie("CTExportKingdomsAndCultures", _vm);
			ActivateLayer(this.SelectConfigLayer);
			ScreenManager.PopScreen();
			ScreenManager.PushScreen(this); // without replacing, if opened multiple times, this screen would also be open after closing the MCM screen
		}

		internal void Close() {
			SelectConfigLayer.ReleaseMovie(SelectConfigMovie);
			AddKingdomsAndCulturesLayer.ReleaseMovie(AddKingdomsAndCulturesMovie);
			RemoveLayer(SelectConfigLayer);
			RemoveLayer(AddKingdomsAndCulturesLayer);
			ScreenManager.PopScreen();
			ScreenManager.PushScreen(_previousScreen);
		}

		internal void ActivateLayer(GauntletLayer layer) {
			layer.InputRestrictions.SetInputRestrictions();
			layer.IsFocusLayer = true;
			AddLayer(layer);
			ScreenManager.TrySetFocus(layer);
		}

		internal void CreateSelectedConfigPopUp() {
			this.InputModNameAndUIDLayer = new GauntletLayer(10);
			this.InputModNameAndUIDMovie = this.InputModNameAndUIDLayer.LoadMovie("CTModNameAndUIDPopUp", _vm);
		}

		internal void TearDownSelectedConfigPopUp() {
			InputModNameAndUIDLayer.ReleaseMovie(InputModNameAndUIDMovie);
			RemoveLayer(InputModNameAndUIDLayer);
		}
	}
}
