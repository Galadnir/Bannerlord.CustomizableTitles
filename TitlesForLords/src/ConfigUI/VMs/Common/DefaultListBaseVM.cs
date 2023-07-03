using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common {
	internal abstract class DefaultListBaseVM : SettingsLayerBaseVM {

		string _nextScreenOpenedKeyBeforeExecuteBack;
		bool _nextScreenOpenedButtonBeforeExecuteBack;

		protected readonly SettingsLayerBaseVM _parent;
		protected readonly TitleConfiguration _config;

		IDictionary<string, string> KeyRenames => BaseVM.KeyRenames[ID];


		protected abstract string ID { get; }
		protected abstract string EntryHint { get; }
		protected abstract IEnumerable<string> EntriesOriginKeys { get; }

		protected string NextScreenOpenedWithOriginalKey { get; private set; }
		protected bool NextScreenOpenedViaAdditionalButton { get; set; }

		[DataSourceProperty]
		public bool IsEditEnabled { get; }
		[DataSourceProperty]
		public MBBindingList<DefaultListEntryVM> Entries { get; }

		protected DefaultListBaseVM(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM, bool createEntriesImmediately = true, bool createKeyRenamesDictImmediately = true) : base(baseVM) {
			_config = config;
			_parent = parent;
			IsEditEnabled = isEditable;
			Entries = new MBBindingList<DefaultListEntryVM>();
			if (createKeyRenamesDictImmediately) {
				CreateKeyRenamesDict();
			}
			if (createEntriesImmediately) {
				CreateStartingEntries();
			}

		}

		public abstract void ExecuteCreateNewEntry();

		public virtual void ExecuteAdditionalButtonPressed() {
		}
		protected abstract bool Rename(string oldKey, string newKey);
		protected abstract bool Copy(string entryKey, string newKey);
		protected abstract void Delete(string entryKey);
		protected abstract void ExecuteSelectEntry(string entryKey, string originalKey);

		internal void ExecuteRename(string newKey, DefaultListEntryVM calledBy) {
			if (Rename(calledBy.EntryKey, newKey)) {
				string originalKey = KeyRenames[calledBy.EntryKey];
				KeyRenames.Remove(calledBy.EntryKey);
				calledBy.EntryKey = newKey;
				KeyRenames[calledBy.EntryKey] = originalKey;
				OnPropertyChanged(nameof(Entries));
				BaseVM.DisableForwardInHistory();
			}
		}

		internal void ExecuteCopy(string entryKey, string newKey) {
			if (Copy(entryKey, newKey)) {
				AddToEntries(newKey);
			}
		}

		internal void ExecuteDelete(string entryKey) {
			Delete(entryKey);
			Entries.Remove(Entries.Single(entry => entry.EntryKey == entryKey));
			OnPropertyChanged(nameof(Entries));
			KeyRenames.Remove(entryKey);
			BaseVM.DisableForwardInHistory();
		}

		internal void Select(string entryKey, string originalKey) {
			ExecuteSelectEntry(entryKey, originalKey);
			NextScreenOpenedWithOriginalKey = originalKey;
			NextScreenOpenedViaAdditionalButton = false;
		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is DefaultListBaseVM newInstance) {
				newInstance.NextScreenOpenedWithOriginalKey = NextScreenOpenedWithOriginalKey;
				newInstance.NextScreenOpenedViaAdditionalButton = NextScreenOpenedViaAdditionalButton;
				if (!(NextScreenOpenedWithOriginalKey is null)) {
					newInstance.Select(NextScreenOpenedWithOriginalKey, NextScreenOpenedWithOriginalKey);
				} else if (NextScreenOpenedViaAdditionalButton) {
					newInstance.ExecuteAdditionalButtonPressed();
				}
			}
		}

		internal override void OnIsTopLayer() {
			NextScreenOpenedWithOriginalKey = null;
			NextScreenOpenedViaAdditionalButton = false;
		}

		internal override void OnAfterExecuteForwardOnNewTopScreen() {
			NextScreenOpenedWithOriginalKey = _nextScreenOpenedKeyBeforeExecuteBack;
			NextScreenOpenedViaAdditionalButton = _nextScreenOpenedButtonBeforeExecuteBack;
		}

		internal override void OnAfterExecuteBackOnNewTopScreen() {
			_nextScreenOpenedKeyBeforeExecuteBack = NextScreenOpenedWithOriginalKey;
			_nextScreenOpenedButtonBeforeExecuteBack = NextScreenOpenedViaAdditionalButton;
		}

		internal override void OnAfterSave() {
			var childOriginEntry = Entries.FirstOrDefault(entry => entry.OriginalKey == NextScreenOpenedWithOriginalKey);
			foreach (var entry in Entries) {
				entry.OnAfterSave();
			}
			if (!(childOriginEntry is null)) {
				NextScreenOpenedWithOriginalKey = childOriginEntry.OriginalKey;
			}
			AddKeyMappings();
		}

		internal override bool IsValid() {
			return _parent.IsValid();
		}

		protected void AddToEntries(string newKey) {
			Entries.Add(new DefaultListEntryVM(IsEditEnabled, newKey, newKey, EntryHint, this));
			KeyRenames[newKey] = newKey;
			OnPropertyChanged(nameof(Entries));
		}

		protected void CreateStartingEntries() {
			CreateEntries();
			AddKeyMappings();
		}

		protected void CreateKeyRenamesDict() {
			if (!BaseVM.KeyRenames.ContainsKey(ID)) {
				BaseVM.KeyRenames[ID] = new Dictionary<string, string>();
			}
		}

		private void CreateEntries() {
			foreach (var entryOriginKey in EntriesOriginKeys) {
				string originalKey = KeyRenames.ContainsKey(entryOriginKey) ? KeyRenames[entryOriginKey] : entryOriginKey;
				Entries.Add(new DefaultListEntryVM(IsEditEnabled, entryOriginKey, originalKey, EntryHint, this));
			}
			Entries.Sort(Comparer<DefaultListEntryVM>.Create(
				(entry1, entry2) => entry1.EntryKey.CompareTo(entry2.EntryKey)
			));
		}

		private void AddKeyMappings() {
			foreach (var entry in Entries) {
				KeyRenames[entry.EntryKey] = entry.OriginalKey;
			}
		}
	}
}
