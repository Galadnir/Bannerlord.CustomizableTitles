using Newtonsoft.Json;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements {
	internal class ConfigOptions {

		readonly bool _isDefault;

		public bool IsDefault {
			get => _isDefault;
		} // if this property is readonly it is not used by JsonSerializer
		public bool IsActive { get; set; }

		[JsonConstructor]
		public ConfigOptions(bool isDefault, bool isActive) {
			this._isDefault = isDefault;
			this.IsActive = isActive;
		}

		internal ConfigOptions(ConfigOptions options, bool isDefault = false) {
			_isDefault = isDefault;
			IsActive = options.IsActive;
		}
	}
}