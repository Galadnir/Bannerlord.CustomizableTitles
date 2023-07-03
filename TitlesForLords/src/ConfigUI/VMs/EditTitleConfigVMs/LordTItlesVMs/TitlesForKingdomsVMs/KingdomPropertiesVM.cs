using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForKingdomsVMs {
	internal class KingdomPropertiesVM : SettingsLayerBaseVM {

		enum Command { None, EditDefault, EditRulerProperties, EditRulerSpouse, EditRulerChildren, EditRulingClan, EditRulesForTiers }

		Command _toNextScreen;
		Command _toNextScreenBeforeExecuteBack;

		readonly KingdomProperties _kingdomProperties;
		readonly TitleConfiguration _config;
		readonly bool _isEditable;
		readonly Func<bool> _isValid;
		readonly Func<KingdomProperties> _getPostResetProperties;
		readonly string _id;

		internal override string PathDescriptor { get; }

		[DataSourceProperty]
		public MBBindingList<ListButtonVM> Buttons { get; }

		internal KingdomPropertiesVM(string id, string pathDescriptor, KingdomProperties kingdomProperties, TitleConfiguration config, bool isEditable, Func<bool> isValid, Func<KingdomProperties> getPostResetProperties, ConfigUIBaseVM baseVM) : base(baseVM) {
			_kingdomProperties = kingdomProperties;
			_config = config;
			_isEditable = isEditable;
			_id = id;
			PathDescriptor = pathDescriptor;
			_isValid = isValid;
			_getPostResetProperties = getPostResetProperties;
			const string editDefaultHint = "Edit default properties. Overwrites undefined properties for the ruler, the ruling clan and all clan tiers";
			const string EditRulerPropertiesHint = "Define properties to apply to the ruler of this kingdom. Overwrites properties defined in 'Edit default', in 'Edit Ruling Clan' and in 'Edit clan tiers'";
			const string EditRulerSpousePropertiesHint = "Define properties to apply to the spouse of the ruler of this kingdom. Overwrites properties defined in 'Edit default', in 'Edit Ruling Clan' and in 'Edit clan tiers'";
			const string EditRulerChildrenPropertiesHint = "Define properties to apply to the children of the ruler of this kingdom. Overwrites properties defined in 'Edit default', in 'Edit Ruling Clan' and in 'Edit clan tiers'";
			const string EditRulingClanPropertiesHint = "Define properties to apply to the ruling clan of this kingdom. Overwrites properties defined in 'Edit default' and in 'Edit clan tiers', but these are overwritten by properties defined directly for the ruler.";
			const string EditClanTiersHint = "Define properties depending on the clan tier. Overwrites properties defined in 'Edit default', but is overwritten by all other properties defined here.";

			Buttons = new MBBindingList<ListButtonVM> {
				new ListButtonVM("Edit default", editDefaultHint, ExecuteEditDefault),
				new ListButtonVM("Edit ruler properties", EditRulerPropertiesHint, ExecuteEditRuler),
				new ListButtonVM("Edit ruler spouse properties", EditRulerSpousePropertiesHint, ExecuteEditRulerSpouse),
				new ListButtonVM("Edit ruler children properties", EditRulerChildrenPropertiesHint, ExecuteEditRulerChildren),
				new ListButtonVM("Edit ruling clan properties", EditRulingClanPropertiesHint, ExecuteEditRulingClan),
				new ListButtonVM("Edit clan tiers", EditClanTiersHint, ExecuteEditClanTiers)
			};
		}

		public void ExecuteEditDefault() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditClanPropertiesVM("default", _kingdomProperties.Default, _isEditable, this.IsValid, BaseVM);
			var movie = layer.LoadMovie("CTEditClanProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditDefault;
		}

		public void ExecuteEditRuler() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new RankMemberSelectionVM("ruler", _kingdomProperties.RulerProperties, _isEditable, this.IsValid, BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditRulerProperties;
		}

		public void ExecuteEditRulerSpouse() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new RankMemberSelectionVM("ruler spouse", _kingdomProperties.SpouseOfRulerProperties, _isEditable, this.IsValid, BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditRulerSpouse;
		}

		public void ExecuteEditRulerChildren() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new RankMemberSelectionVM("ruler children", _kingdomProperties.ChildOfRulerProperties, _isEditable, this.IsValid, BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditRulerChildren;
		}

		public void ExecuteEditRulingClan() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditClanPropertiesVM("ruling clan", _kingdomProperties.RulingClan, _isEditable, this.IsValid, BaseVM);
			var movie = layer.LoadMovie("CTEditClanProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditRulingClan;
		}

		public void ExecuteEditClanTiers() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new ClanTierListVM(_id, _config, _kingdomProperties, _isEditable, _getPostResetProperties, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditRulesForTiers;
		}
		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is KingdomPropertiesVM newInstance) {
				newInstance._toNextScreen = _toNextScreen;
				switch (_toNextScreen) {
					case Command.None:
						return;
					case Command.EditDefault:
						newInstance.ExecuteEditDefault();
						return;
					case Command.EditRulerProperties:
						newInstance.ExecuteEditRuler();
						return;
					case Command.EditRulerSpouse:
						newInstance.ExecuteEditRulerSpouse();
						return;
					case Command.EditRulerChildren:
						newInstance.ExecuteEditRulerChildren();
						return;
					case Command.EditRulingClan:
						newInstance.ExecuteEditRulingClan();
						return;
					case Command.EditRulesForTiers:
						newInstance.ExecuteEditClanTiers();
						return;
				}
			}
		}

		internal override void OnIsTopLayer() {
			_toNextScreen = Command.None;
		}

		internal override void OnAfterExecuteForwardOnNewTopScreen() {
			_toNextScreen = _toNextScreenBeforeExecuteBack;
		}

		internal override void OnAfterExecuteBackOnNewTopScreen() {
			_toNextScreenBeforeExecuteBack = _toNextScreen;
		}

		internal override bool IsValid() {
			return _isValid();
		}
	}
}
