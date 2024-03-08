using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs {
	internal class EditTitlePropertiesVMWidget : ViewModel {

		[DataSourceProperty]
		public MainTitlePropertiesVM MainTitleProperties { get; }
		[DataSourceProperty]
		public VillagerPropertiesVM VillagerProperties { get; }
		[DataSourceProperty]
		public CaravanPropertiesVM CaravanProperties { get; }

		public EditTitlePropertiesVMWidget(MainTitlePropertiesVM mainTitleProperties, VillagerPropertiesVM villagerProperties, CaravanPropertiesVM caravanProperties) {
			this.MainTitleProperties = mainTitleProperties;
			this.VillagerProperties = villagerProperties;
			this.CaravanProperties = caravanProperties;
		}

		internal static EditTitlePropertiesVMWidget CreateFor(TitleProperties properties, bool isEditEnabled) {
			return new EditTitlePropertiesVMWidget(
				new MainTitlePropertiesVM(properties, isEditEnabled),
				new VillagerPropertiesVM(properties.OwnedVillagers, isEditEnabled),
				new CaravanPropertiesVM(properties.OwnedCaravans, isEditEnabled));
		}
	}
}
