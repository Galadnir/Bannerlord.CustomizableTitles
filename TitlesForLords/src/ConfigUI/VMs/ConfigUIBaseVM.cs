using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs {
	public class ConfigUIBaseVM : ViewModel {

		readonly struct LayerData {  //this exists to always reclear, otherwise navigating backwards or forwards only leaves black screen

			internal GauntletLayer Layer { get; }
			internal VMData VMData { get; }

			internal LayerData(GauntletLayer layer, IGauntletMovie movie, SettingsLayerBaseVM vm) {
				Layer = layer;
				VMData = new VMData(vm, movie);
			}

			public LayerData(GauntletLayer layer, VMData vmData) {
				Layer = layer;
				this.VMData = vmData;
			}
		}

		readonly struct VMData {

			internal SettingsLayerBaseVM VM { get; }
			internal IGauntletMovie Movie { get; }

			internal VMData(SettingsLayerBaseVM vm, IGauntletMovie movie) {
				VM = vm;
				Movie = movie;
			}
		}

		public const string MovieName = "CTModSettings";

		internal ConfigUIScreen Screen { get; }

		bool _isBackEnabled;
		bool _isForwardEnabled;
		string _currentPath;

		[DataSourceProperty]
		public bool IsBackEnabled {
			get => _isBackEnabled;
			set {
				if (value != _isBackEnabled) {
					_isBackEnabled = value;
					OnPropertyChangedWithValue(value, nameof(IsBackEnabled));
				}
			}
		}
		[DataSourceProperty]
		public bool IsForwardEnabled {
			get => _isForwardEnabled;
			set {
				if (value != _isForwardEnabled) {
					_isForwardEnabled = value;
					OnPropertyChangedWithValue(value, nameof(IsForwardEnabled));
				}
			}
		}
		[DataSourceProperty]
		public HintViewModel BackHint { get; } = new HintViewModel(new TextObject("Go to previous page"));
		[DataSourceProperty]
		public HintViewModel ForwardHint { get; } = new HintViewModel(new TextObject("Go forward"));
		[DataSourceProperty]
		public string CurrentPath {
			get => _currentPath;
			set {
				if (value != _currentPath) {
					_currentPath = value;
					OnPropertyChangedWithValue(value, nameof(CurrentPath));
				}
			}
		}

		internal IDictionary<string, Dictionary<string, string>> KeyRenames { get; } // mapping IDs of DefaultListBaseVM implementations to current entry keys mapping to the original key

		readonly Stack<LayerData> _configUILayers; //each layer only gets one movie
		readonly Stack<VMData> _forwardHistory;


		internal ConfigUIBaseVM(ConfigUIScreen screen) {
			this.Screen = screen;
			CurrentPath = "";
			_configUILayers = new Stack<LayerData>();
			_forwardHistory = new Stack<VMData>();
			KeyRenames = new Dictionary<string, Dictionary<string, string>>();
			UpdateNavigationPossibilities();
		}

		internal void LoadModSettingsLayer() {
			var modSettingsLayer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var modSettingsVM = new ModSettingsVM(this);
			var modSettingsMovie = modSettingsLayer.LoadMovie(MovieName, modSettingsVM);
			PushLayerAndMovie(modSettingsLayer, modSettingsMovie, modSettingsVM, true);
		}

		public void ExecuteCancel() {
			ModSettings.Instance.Restore(); // Discard changes
			CloseConfigUI();
		}

		public void ExecuteDone() {
			Save();
			CloseConfigUI();
		}

		public void ExecuteSave() {
			Save();
			InformationManager.DisplayMessage(new InformationMessage("saved successfully"));
			foreach (var specificKeyRenames in KeyRenames.Values) {
				specificKeyRenames.Clear();
			}
			foreach (var layerData in _configUILayers) {
				layerData.VMData.VM.OnAfterSave();
			}
			foreach (var vmData in _forwardHistory) {
				vmData.VM.OnAfterSave();
			}
		}

		public void ExecuteReset() {
			ModSettings.Instance.Restore();
			while (!_configUILayers.Peek().VMData.VM.IsValid()) {
				ExecuteBack();
			}
			_forwardHistory.Clear();
			foreach (var specificKeyRenames in KeyRenames.Values) {
				specificKeyRenames.Clear();
			}
			var configLayersInReverse = new Stack<LayerData>(); // stack iterator is LIFO
			while (!_configUILayers.IsEmpty()) {
				var layerData = _configUILayers.Pop();
				Screen.DeactivateLayer(layerData.Layer);
				configLayersInReverse.Push(layerData);
			}
			LoadModSettingsLayer();
			foreach (var layerData in configLayersInReverse) {
				layerData.VMData.VM.RestoreNextScreenPostResetAndTransferAdditionalState(_configUILayers.Peek().VMData.VM);
			}
			UpdateNavigationPossibilities();
			InformationManager.DisplayMessage(new InformationMessage("reset settings"));
		}

		public void ExecuteBack() {
			if (!_isBackEnabled) {
				return;
			}
			var layerData = _configUILayers.Pop();
			this.Screen.DeactivateLayer(layerData.Layer);
			_forwardHistory.Push(layerData.VMData);
			_configUILayers.Peek().VMData.VM.OnAfterExecuteBackOnNewTopScreen();
			ActivateTopLayer();
		}

		public void ExecuteForward() {
			if (!_isForwardEnabled) {
				return;
			}
			SettingsLayerBaseVM previousTopLayer = _configUILayers.Peek().VMData.VM;
			var vmData = _forwardHistory.Pop();
			var newLayer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var newMovie = newLayer.LoadMovie(vmData.Movie.MovieName, vmData.VM);
			PushLayerAndMovie(newLayer, newMovie, vmData.VM, false);
			previousTopLayer.OnAfterExecuteForwardOnNewTopScreen();
		}

		internal void OnFrameTick(GauntletLayer baseLayer) {
			OnFrameTickForLayer(baseLayer);
			if (_configUILayers.IsEmpty()) {
				return;
			}
			var layerData = _configUILayers.Peek();
			OnFrameTickForLayer(layerData.Layer);
		}

		internal void OnFrameTickForLayer(GauntletLayer layer) {
			if (layer.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.Escape) || layer.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.X1MouseButton)) {
				ExecuteBack();
			}
			if (layer.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.X2MouseButton)) {
				ExecuteForward();
			}
		}

		internal void CloseConfigUI() {
			UnloadAllLayers();
			this.Screen.Close();
		}

		internal void PushLayerAndMovie(GauntletLayer layer, IGauntletMovie movie, SettingsLayerBaseVM vm, bool clearForwardHistory = true) {
			if (clearForwardHistory) {
				_forwardHistory.Clear();
			}
			if (!_configUILayers.IsEmpty()) {
				var layerData = _configUILayers.Peek();
				this.Screen.DeactivateLayer(layerData.Layer);
			}
			PushAndActivate(new LayerData(layer, movie, vm));
		}

		internal void DisableForwardInHistory() {
			_forwardHistory.Clear();
			UpdateNavigationPossibilities();
		}

		internal void UpdateCurrentPath() {
			CurrentPath = string.Join("/", _configUILayers.Select(layerData => layerData.VMData.VM.PathDescriptor)
				.Where(pathDescriptor => pathDescriptor != string.Empty).Reverse());
		}

		private void PushAndActivate(LayerData layerData) {
			_configUILayers.Push(layerData);
			this.Screen.ActivateLayer(layerData.Layer);
			layerData.VMData.VM.OnIsTopLayer();
			UpdateNavigationPossibilities();
			UpdateCurrentPath();
		}

		private void ActivateTopLayer() {
			if (_configUILayers.IsEmpty()) {
				return;
			}
			var layerData = _configUILayers.Pop();
			var newLayer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var newMovie = newLayer.LoadMovie(layerData.VMData.Movie.MovieName, layerData.VMData.VM);
			PushAndActivate(new LayerData(newLayer, newMovie, layerData.VMData.VM));
		}

		private void UnloadAllLayers() {
			foreach (var layerData in _configUILayers) {
				layerData.Layer.ReleaseMovie(layerData.VMData.Movie);
			}
			_configUILayers.Clear();
			_forwardHistory.Clear();
		}

		private void UpdateNavigationPossibilities() {
			IsBackEnabled = _configUILayers.Count > 1;
			IsForwardEnabled = _forwardHistory.Count > 0;
		}

		private void Save() {
			ModSettings.Instance.Save();
		}
	}
}
