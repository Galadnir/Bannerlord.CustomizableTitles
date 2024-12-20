﻿using Bannerlord.TitleOverhaul.src.ConfigUI.VMs;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.src.main.Core;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.ScreenSystem;

namespace Bannerlord.TitleOverhaul.src.ConfigUI {

	public class ConfigUIScreen {

		readonly GauntletLayer _baseLayer;
		readonly IGauntletMovie _baseMovie;
		readonly ConfigUIBaseVM _baseVM;
		readonly ScreenBase _activeScreen;

		GauntletLayer _popUpLayer;
		EditableTextPopUpVM _popUpVM;
		IGauntletMovie _popUpMovie;

		GauntletLayer _inquiryPopUpLayer;
		InquiryPopUpVM _inquiryPopUpVM;
		IGauntletMovie _inquiryPopUpMovie;

		internal ConfigUIScreen() {
			ModSettings.Instance.Restore(); // in case there are unsaved changes from playing

			_baseLayer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			_baseVM = new ConfigUIBaseVM(this);
			_baseMovie = _baseLayer.LoadMovie("CTConfigUIBase", _baseVM);
			_activeScreen = ScreenManager.TopScreen;
			ActivateLayer(_baseLayer);

			_baseVM.LoadModSettingsLayer();
		}

		public void Close() { // all inner vm movies must be released before (although I don't know if they actually have to be released, I'm just doing it to be sure)
			ClosePopUp();
			UnloadLayer(_baseLayer, _baseMovie);
		}

		public void ActivateLayer(GauntletLayer layer) {
			layer.InputRestrictions.SetInputRestrictions();
			layer.IsFocusLayer = true;
			_activeScreen.AddLayer(layer);
			ScreenManager.TrySetFocus(layer);
		}

		public void DeactivateLayer(GauntletLayer layer) {
			layer.InputRestrictions.ResetInputRestrictions();
			layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(layer);
			try {
				_activeScreen.RemoveLayer(layer);
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
			_popUpVM?.ExecuteDiscard(); // it should not be possible that OpenPopUp is called while a popUp is already open, but with this there will only be one open popUp in case of bugs also, which is easier to handle
			_inquiryPopUpVM?.ExecuteDeny();
			_popUpLayer = new GauntletLayer(LayerPriority.InputPopUp);
			_popUpMovie = _popUpLayer.LoadMovie("CTEditableTextPopUp", vm);
			_popUpVM = vm;
			TitlesForLordsSubModule.NavigateBackwardsHotkey.IsEnabled = false;
			TitlesForLordsSubModule.NavigateForwardsHotkey.IsEnabled = false;

			var PopUpConfirmHotkey = TitlesForLordsSubModule.PopUpConfirmHotkey;
			PopUpConfirmHotkey.IsEnabled = true;
			PopUpConfirmHotkey.IsDownAndReleasedEvent += _popUpVM.ExecuteConfirm;

			var PopUpDiscardHotkey = TitlesForLordsSubModule.PopUpDiscardHotkey;
			PopUpDiscardHotkey.IsEnabled = true;
			PopUpDiscardHotkey.IsDownAndReleasedEvent += _popUpVM.ExecuteDiscard;

			ActivateLayer(_popUpLayer);
		}

		internal void ClosePopUp() {
			if (_popUpMovie is null) {
				return;
			}

			TitlesForLordsSubModule.NavigateBackwardsHotkey.IsEnabled = true;
			TitlesForLordsSubModule.NavigateForwardsHotkey.IsEnabled = true;

			var PopUpConfirmHotkey = TitlesForLordsSubModule.PopUpConfirmHotkey;
			PopUpConfirmHotkey.IsEnabled = false;
			PopUpConfirmHotkey.IsDownAndReleasedEvent -= _popUpVM.ExecuteConfirm;

			var PopUpDiscardHotkey = TitlesForLordsSubModule.PopUpDiscardHotkey;
			PopUpDiscardHotkey.IsEnabled = false;
			PopUpDiscardHotkey.IsDownAndReleasedEvent -= _popUpVM.ExecuteDiscard;

			UnloadLayer(_popUpLayer, _popUpMovie);
			_popUpLayer = null;
			_popUpMovie = null;
			_popUpVM = null;
		}

		internal void OpenInquiryPopUp(InquiryPopUpVM vm) {
			_popUpVM?.ExecuteDiscard(); // it should not be possible that OpenPopUp is called while a popUp is already open, but with this there will only be one open popUp in case of bugs also, which is easier to handle
			_inquiryPopUpVM?.ExecuteDeny();
			_inquiryPopUpLayer = new GauntletLayer(LayerPriority.InquiryPopUp);
			_inquiryPopUpMovie = _inquiryPopUpLayer.LoadMovie("CTInquiryPopUp", vm);
			_inquiryPopUpVM = vm;
			TitlesForLordsSubModule.NavigateBackwardsHotkey.IsEnabled = false;
			TitlesForLordsSubModule.NavigateForwardsHotkey.IsEnabled = false;

			ActivateLayer(_inquiryPopUpLayer);
		}

		internal void CloseInquiryPopUp() {
			if (_inquiryPopUpMovie is null) {
				return;
			}
			TitlesForLordsSubModule.NavigateBackwardsHotkey.IsEnabled = true;
			TitlesForLordsSubModule.NavigateForwardsHotkey.IsEnabled = true;

			UnloadLayer(_inquiryPopUpLayer, _inquiryPopUpMovie);
			_inquiryPopUpLayer = null;
			_inquiryPopUpMovie = null;
			_inquiryPopUpVM = null;
		}
	}
}
