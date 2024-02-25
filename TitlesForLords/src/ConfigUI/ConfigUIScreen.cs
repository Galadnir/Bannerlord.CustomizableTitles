using Bannerlord.TitleOverhaul.src.ConfigUI.VMs;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.ScreenSystem;

namespace Bannerlord.TitleOverhaul.src.ConfigUI {

	public class ConfigUIScreen : ScreenBase {

		readonly GauntletLayer _baseLayer;
		readonly IGauntletMovie _baseMovie;
		readonly ConfigUIBaseVM _baseVM;
		readonly ScreenBase _previousScreen;

		GauntletLayer _popUpLayer;
		EditableTextPopUpVM _popUpVM;
		IGauntletMovie _popUpMovie;

		internal ConfigUIScreen() {
			// TODO in in-game screen, pop-up that warns that potential unsaved changes due to name tracking are discarded
			ModSettings.Instance.Restore(); // in case there are unsaved changes from playing
			_baseLayer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			_baseVM = new ConfigUIBaseVM(this);
			_baseMovie = _baseLayer.LoadMovie("CTConfigUIBase", _baseVM);
			_previousScreen = ScreenManager.TopScreen;
			ActivateLayer(_baseLayer);
			ScreenManager.PopScreen();
			ScreenManager.PushScreen(this); // without replacing, if opened multiple times, this screen would also be open after closing the MCM screen
			_baseVM.LoadModSettingsLayer();
		}

		protected override void OnFrameTick(float dt) {
			_baseVM.OnFrameTick(_baseLayer);
			if (!(_popUpLayer is null)) {
				if (_popUpLayer.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.Escape)) {
					_popUpVM.ExecuteDiscard();
				}
				if (_popUpLayer.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.Enter)) {
					_popUpVM.ExecuteConfirm();
				}
			}
		}
		public void Close() { // all inner vm movies must be released before (although I don't know if they actually have to be released, I'm just doing it to be sure)
			UnloadLayer(_baseLayer, _baseMovie);
			DeactivateAllLayers();
			ScreenManager.PopScreen();
			ScreenManager.PushScreen(_previousScreen);
		}

		public void ActivateLayer(GauntletLayer layer) {
			layer.InputRestrictions.SetInputRestrictions();
			layer.IsFocusLayer = true;
			AddLayer(layer);
			ScreenManager.TrySetFocus(layer);
		}

		public void DeactivateLayer(GauntletLayer layer) {
			layer.InputRestrictions.ResetInputRestrictions();
			layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(layer);
			try {
				RemoveLayer(layer);
			} catch (NullReferenceException) { }
		}


		public void UnloadLayer(GauntletLayer layer, IGauntletMovie layerMovie) {
			DeactivateLayer(layer);
			layer.ReleaseMovie(layerMovie);
		}

		internal void FocusBaseLayer() {
			ScreenManager.TrySetFocus(_baseLayer);
		}

		internal void OpenPopUp(EditableTextPopUpVM vm) {
			_popUpLayer = new GauntletLayer(LayerPriority.InputPopUp);
			_popUpMovie = _popUpLayer.LoadMovie("CTEditableTextPopUp", vm);
			_popUpVM = vm;
			ActivateLayer(_popUpLayer);
		}

		internal void ClosePopUp() {
			if (_popUpMovie is null) {
				return;
			}
			UnloadLayer(_popUpLayer, _popUpMovie);
			_popUpLayer = null;
			_popUpMovie = null;
			_popUpVM = null;
		}
	}
}
