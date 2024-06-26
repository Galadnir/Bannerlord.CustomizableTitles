using Bannerlord.TitleOverhaul.src.ConfigUI.VMs;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using System;
using TaleWorlds.Library;

namespace Bannerlord.TitlesForLords.src.ConfigUI.VMs.EditTitleConfigVMs.SimpleEditor {
	internal class SimpleEditTitlesVM : SettingsLayerBaseVM {

		static readonly Func<string, bool> TitleBeforeNameWarningCondition = stringValue => !stringValue.EndsWith(" ");
		const string TitleBeforeNameWarning = "Warning: Your title doesn't end with a space. A space is not automatically inserted after the title";

		static readonly Func<string, bool> TitleAfterNameWarningCondition = stringValue => !stringValue.StartsWith(" ");
		const string TitleAfterNameWarning = "Warning: Your title doesn't start with a space. A space is not automatically inserted before the title";

		readonly Func<bool> _isValid;

		internal override string PathDescriptor { get; }

		[DataSourceProperty]
		public bool IsEditEnabled { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameRulerMale { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameRulerMale { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameRulerFemale { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameRulerFemale { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier1Male { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier1Male { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier1Female { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier1Female { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier2Male { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier2Male { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier2Female { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier2Female { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier3Male { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier3Male { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier3Female { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier3Female { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier4Male { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier4Male { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier4Female { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier4Female { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier5Male { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier5Male { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier5Female { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier5Female { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier6Male { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier6Male { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameTier6Female { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameTier6Female { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameRulerChildMale { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameRulerChildMale { get; }

		[DataSourceProperty]
		public EditStringBarVM TitleBeforeNameRulerChildFemale { get; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterNameRulerChildFemale { get; }

		public SimpleEditTitlesVM(KingdomProperties properties, string pathDescriptor, ConfigUIBaseVM baseVM, Func<bool> isValid, bool isEditable) : base(baseVM) {
			PathDescriptor = pathDescriptor;
			_isValid = isValid;
			IsEditEnabled = isEditable;

			EnsureClanTiersExist(properties);

			(TitleBeforeNameRulerMale, TitleAfterNameRulerMale) = CreateStringBars(properties.RulerProperties.Male);
			(TitleBeforeNameRulerFemale, TitleAfterNameRulerFemale) = CreateStringBars(properties.RulerProperties.Female);

			(TitleBeforeNameTier1Male, TitleAfterNameTier1Male) = CreateStringBars(properties.ClanTiers[1].ClanMemberDefault.Male);
			(TitleBeforeNameTier1Female, TitleAfterNameTier1Female) = CreateStringBars(properties.ClanTiers[1].ClanMemberDefault.Female);

			(TitleBeforeNameTier2Male, TitleAfterNameTier2Male) = CreateStringBars(properties.ClanTiers[2].ClanMemberDefault.Male);
			(TitleBeforeNameTier2Female, TitleAfterNameTier2Female) = CreateStringBars(properties.ClanTiers[2].ClanMemberDefault.Female);

			(TitleBeforeNameTier3Male, TitleAfterNameTier3Male) = CreateStringBars(properties.ClanTiers[3].ClanMemberDefault.Male);
			(TitleBeforeNameTier3Female, TitleAfterNameTier3Female) = CreateStringBars(properties.ClanTiers[3].ClanMemberDefault.Female);

			(TitleBeforeNameTier4Male, TitleAfterNameTier4Male) = CreateStringBars(properties.ClanTiers[4].ClanMemberDefault.Male);
			(TitleBeforeNameTier4Female, TitleAfterNameTier4Female) = CreateStringBars(properties.ClanTiers[4].ClanMemberDefault.Female);

			(TitleBeforeNameTier5Male, TitleAfterNameTier5Male) = CreateStringBars(properties.ClanTiers[5].ClanMemberDefault.Male);
			(TitleBeforeNameTier5Female, TitleAfterNameTier5Female) = CreateStringBars(properties.ClanTiers[5].ClanMemberDefault.Female);

			(TitleBeforeNameTier6Male, TitleAfterNameTier6Male) = CreateStringBars(properties.ClanTiers[6].ClanMemberDefault.Male);
			(TitleBeforeNameTier6Female, TitleAfterNameTier6Female) = CreateStringBars(properties.ClanTiers[6].ClanMemberDefault.Female);

			(TitleBeforeNameRulerChildMale, TitleAfterNameRulerChildMale) = CreateStringBars(properties.ChildOfRulerProperties.Male);
			(TitleBeforeNameRulerChildFemale, TitleAfterNameRulerChildFemale) = CreateStringBars(properties.ChildOfRulerProperties.Female);
		}

		internal override bool IsValid() {
			return _isValid();
		}

		private void EnsureClanTiersExist(KingdomProperties properties) {
			for (int i = 1; i <= 6; i++) {
				if (!properties.ClanTiers.ContainsKey(i)) {
					properties.AddTierProperties(i, ClanProperties.CreateEmpty());
				}
			}
		}

		private (EditStringBarVM BeforeNameStringBar, EditStringBarVM AfterNameStringBar) CreateStringBars(TitleProperties properties) {
			return (CreateBeforeNameStringBar(properties.TitleBeforeName, stringValue => properties.TitleBeforeName = stringValue),
				CreateAfterNameStringBar(properties.TitleAfterName, stringValue => properties.TitleAfterName = stringValue));
		}

		private EditStringBarVM CreateBeforeNameStringBar(string currentValue, Action<string> onChange) {
			return new EditStringBarVM(IsEditEnabled, "Title (before name)", "The title of the character before his name. A space is not automatically inserted after. If 'Title (before name)' and 'Title (after name)' are both undefined for a character, no alterations to the game's output are made. NOTE: Potentially, the used titles can also come from values only visible in the \"Expert\"-Editor",
				currentValue, onChange, TitleBeforeNameWarningCondition, TitleBeforeNameWarning);
		}

		private EditStringBarVM CreateAfterNameStringBar(string currentValue, Action<string> onChange) {
			return new EditStringBarVM(IsEditEnabled, "Title (after name)", "The title of the character after his name. A space is not automatically inserted before. If 'Title (before name)' and 'Title (after name)' are both undefined for a character, no alterations to the game's output are made. NOTE: Potentially, the used titles can also come from values only visible in the \"Expert\"-Editor",
				currentValue, onChange, TitleAfterNameWarningCondition, TitleAfterNameWarning);
		}
	}
}
