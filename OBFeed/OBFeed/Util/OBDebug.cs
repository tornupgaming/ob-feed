using System;
using System.Text;

namespace OBFeed {
	public static class OBDebug {

		/// <summary>
		/// It ain't efficient but it's a *DEBUG* logger. 
		/// It's designed to be able to just spam whatever you want in the parameters
		/// without having to worry about which logging function does what.
		/// </summary>
		/// <param name="items">Items to list.</param>
		[System.Diagnostics.Conditional("DEBUG")]
		public static void Log(params object[] items) {
			var sb = new StringBuilder();
			sb.AppendLine ("=-=-=-=");

			// Essentially just iterate over the items 
			foreach (var item in items) {

				// If it's an exception - do some fancy output
				var ex = item as Exception;
				if (ex != null) {
					sb.AppendLine (ex.Message);
					if (ex.InnerException != null) {
						sb.AppendLine ("Inner ex: " + ex.InnerException.Message);
					}
					continue;
				}

				// We could also check for business logic specific objects but really
				// just override your object's ToString if you're that bothered.

				// Otherwise perform a ToString()
				sb.AppendLine(item.ToString());

			}

			sb.AppendLine("=-=-=-=");
			System.Diagnostics.Debug.WriteLine(sb.ToString().Trim());
		}
	}
}

