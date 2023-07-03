using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Bannerlord.TitlesForLords.Tests.tests.Core.Settings;
using Newtonsoft.Json;

namespace Bannerlord.TitlesForLords.Tests.src.tests.Core.Settings {
	[TestClass]
	public class ModSettingsTests {

		[TestCleanup]
		public void Cleanup() {
			File.Delete(ModSettings.SavefileLocation);
		}

		[TestMethod]
		public void SaveAndRestoreTest() {
			ModSettings Instance = ModSettings.Instance;
			Instance.Save();
			Assert.IsTrue(File.Exists(ModSettings.SavefileLocation));
			string firstSave = File.ReadAllText(ModSettings.SavefileLocation);
			Instance.Restore();
			Instance.Save();
			string secondSave = File.ReadAllText(ModSettings.SavefileLocation);
			JsonObjectsForTests.AssertJsonEquality(firstSave, secondSave);
		}

		[TestMethod]
		public void LoadJsonConfigFilesTest() {
			ModSettings Instance = ModSettings.Instance;
			var titleConfig = new TitleConfiguration(new Metadata("1stUid", "1stName", "firstJson", null),
				new ConfigOptions(false, true), null, null);
			string firstSubModuleName = "1stSubModule";
			string secondSubModuleName = "2ndSubModule";
			var kingdoms = new HashSet<string> { "a kingdom" };
			var cultures = new HashSet<string> { "a culture" };
			var firstJsonConfigFile = new JsonConfigFile(firstSubModuleName, titleConfig, new HashSet<string>(), kingdoms);
			var secondJsonConfigFile = new JsonConfigFile(secondSubModuleName, new TitleConfiguration(titleConfig, "2ndUid"), cultures, new HashSet<string>());
			string firstJsonPath = $"{ModSettings.ConfigJsonsBasePath}/1stJson/{ModSettings.ConfigJsonName}";
			string firstJsonAlternatePath = $"{ModSettings.ConfigJsonsBasePath}/alternate1stJson/{ModSettings.ConfigJsonName}"; // check only one is added due to same uid
			string secondJsonPath = $"{ModSettings.ConfigJsonsBasePath}/2stJson/{ModSettings.ConfigJsonName}";
			string nonParseableFilePath = $"{ModSettings.ConfigJsonsBasePath}/Unparseable/{ModSettings.ConfigJsonName}";
			new FileInfo(firstJsonPath).Directory!.Create();
			new FileInfo(firstJsonAlternatePath).Directory!.Create();
			new FileInfo(secondJsonPath).Directory!.Create();
			new FileInfo(nonParseableFilePath).Directory!.Create();
			File.WriteAllText(firstJsonPath, JsonConvert.SerializeObject(firstJsonConfigFile));
			File.WriteAllText(firstJsonAlternatePath, JsonConvert.SerializeObject(firstJsonConfigFile));
			File.WriteAllText(secondJsonPath, JsonConvert.SerializeObject(secondJsonConfigFile));
			File.WriteAllText(nonParseableFilePath, "gibberish");

			var previousCountTitleConfigs = Instance.TitleConfigs.Count;
			var previosCountCultures = Instance.SubModuleToCultures.Count;
			var previousCountKingdoms = Instance.SubModuleToKingdoms.Count;
			Instance.LoadAndSaveNewJsonConfigFiles();
			Assert.AreEqual(previousCountTitleConfigs + 2, Instance.TitleConfigs.Count);
			Assert.AreEqual(previosCountCultures + 1, Instance.SubModuleToCultures.Count);
			Assert.AreEqual(previousCountKingdoms + 1, Instance.SubModuleToKingdoms.Count);

			Assert.IsTrue(Instance.TitleConfigs.Contains(firstJsonConfigFile.TitleConfig));
			Assert.IsTrue(Instance.TitleConfigs.Contains(secondJsonConfigFile.TitleConfig));

			Assert.IsTrue(Instance.SubModuleToKingdoms.ContainsKey(firstSubModuleName));
			Assert.IsTrue(kingdoms.All(kingdom => Instance.SubModuleToKingdoms[firstSubModuleName].Contains(kingdom)));

			Assert.IsTrue(Instance.SubModuleToCultures.ContainsKey(secondSubModuleName));
			Assert.IsTrue(cultures.All(culture => Instance.SubModuleToCultures[secondSubModuleName].Contains(culture)));

			Assert.IsTrue(!File.Exists(firstJsonPath));
			Assert.IsTrue(File.Exists(firstJsonPath.Replace(ModSettings.ConfigJsonName, ModSettings.SuccessfullyLoadedConfigJsonName)));
			Assert.IsTrue(!File.Exists(firstJsonAlternatePath));
			Assert.IsTrue(File.Exists(firstJsonAlternatePath.Replace(ModSettings.ConfigJsonName, ModSettings.SuccessfullyLoadedConfigJsonName)));
			Assert.IsTrue(!File.Exists(secondJsonPath));
			Assert.IsTrue(File.Exists(secondJsonPath.Replace(ModSettings.ConfigJsonName, ModSettings.SuccessfullyLoadedConfigJsonName)));
			Assert.IsTrue(File.Exists(nonParseableFilePath));
			Assert.IsTrue(File.Exists(nonParseableFilePath.Replace(ModSettings.ConfigJsonName, ModSettings.FailedToLoadConfigJsonName)));

			File.Delete(firstJsonPath.Replace(ModSettings.ConfigJsonName, ModSettings.SuccessfullyLoadedConfigJsonName));
			File.Delete(firstJsonAlternatePath.Replace(ModSettings.ConfigJsonName, ModSettings.SuccessfullyLoadedConfigJsonName));
			File.Delete(secondJsonPath.Replace(ModSettings.ConfigJsonName, ModSettings.SuccessfullyLoadedConfigJsonName));
			File.Delete(nonParseableFilePath);
			File.Delete(nonParseableFilePath.Replace(ModSettings.ConfigJsonName, ModSettings.FailedToLoadConfigJsonName));
		}
	}
}
