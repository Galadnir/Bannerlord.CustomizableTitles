using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings {
	internal struct JsonConfigFile {

		public string SubModule { get; set; }
		public TitleConfiguration TitleConfig { get; set; }
		public HashSet<string> Cultures { get; set; }
		public HashSet<string> Kingdoms { get; set; }

		[JsonConstructor]
		public JsonConfigFile(string subModule, TitleConfiguration titleConfig, HashSet<string> cultures, HashSet<string> kingdoms) {
			this.SubModule = subModule;
			this.TitleConfig = titleConfig;
			this.Cultures = cultures;
			this.Kingdoms = kingdoms;
		}
	}
}
