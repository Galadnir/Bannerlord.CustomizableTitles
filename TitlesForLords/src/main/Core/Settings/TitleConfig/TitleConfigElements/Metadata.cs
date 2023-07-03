using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements {
	internal class Metadata : IEquatable<Metadata> {

		readonly string _uid;
		readonly List<Tuple<string, string>> _automaticRenames;

		public string Uid {
			get => _uid;
		} // to avoid readonly on property for JsonSerializer
		public string Name { get; set; }
		public string SubModule { get; set; }
		public IReadOnlyList<Tuple<string, string>> AutomaticRenames {
			get => _automaticRenames;
		}

		[JsonConstructor]
		public Metadata(string uid, string name, string subModule, IReadOnlyList<Tuple<string, string>> AutomaticRenames) {
			this._uid = uid;
			this.Name = name;
			this.SubModule = subModule;
			this._automaticRenames = AutomaticRenames != null ? AutomaticRenames.Select(x => new Tuple<string, string>(x.Item1, x.Item2)).ToList() : new List<Tuple<string, string>>();
		}

		internal Metadata(Metadata metadata, string newUid) {
			_uid = newUid;
			Name = metadata.Name;
			SubModule = metadata.SubModule;
			_automaticRenames = metadata.AutomaticRenames.Select(x => new Tuple<string, string>(x.Item1, x.Item2)).ToList();
		}

		internal void AddRename(string oldName, string newName) {
			_automaticRenames.Add(new Tuple<string, string>(oldName, newName));
		}

		public override bool Equals(object obj) {
			return this.Equals(obj as Metadata);
		}

		public bool Equals(Metadata other) {
			return !(other is null) &&
				   this.Uid == other.Uid;
		}

		public override int GetHashCode() {
			return -1737426059 + EqualityComparer<string>.Default.GetHashCode(this.Uid);
		}

		public static bool operator ==(Metadata left, Metadata right) {
			return EqualityComparer<Metadata>.Default.Equals(left, right);
		}

		public static bool operator !=(Metadata left, Metadata right) {
			return !(left == right);
		}
	}
}
