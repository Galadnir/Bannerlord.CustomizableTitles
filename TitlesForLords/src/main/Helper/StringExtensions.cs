using System.Collections.Generic;

namespace Bannerlord.TitlesForLords.src.main.Helper {
	internal static class StringExtensions {

		public static string RemovePrefix(this string str, string prefix) {
			if (str is null) {
				return null;
			}
			if (prefix is null) {

				return str;
			}
			return str.StartsWith(prefix) ? str.Substring(prefix.Length) : str;
		}

		public static string RemovePostfix(this string str, string postfix) {
			if (str == null) {
				return null;
			}
			if (postfix is null) {
				return str;
			}
			return str.EndsWith(postfix) ? str.Substring(0, str.Length - postfix.Length) : str;
		}
	}
}
