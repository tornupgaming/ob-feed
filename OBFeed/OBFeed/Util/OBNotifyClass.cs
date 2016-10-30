using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace OBFeed {
	public class OBNotifyClass : INotifyPropertyChanged {

		#region Notify Event

		public event PropertyChangedEventHandler PropertyChanged;
		protected void Notify([CallerMemberName] string propertyName = "") {
			if (PropertyChanged != null) {
				// Invoke the property change event on the UI thread - as 99% of the time these
				// property changes will be listened to by UI widgets/items
				Device.BeginInvokeOnMainThread(() => {
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				});
			}
		}

		#endregion

		#region Binding

		/// <summary>
		/// Binds to the PropertyChanged event and passes back the object to the provided Action<T>
		/// </summary>
		/// <param name="propertyName">Property name to listen out for.</param>
		/// <param name="onPropertyChanged">Action to perform when that property changes.</param>
		/// <typeparam name="T">The type of the property.</typeparam>
		public void Bind<T>(string propertyName, Action<T> onPropertyChanged) {

			// Ignore idiocy
			if (string.IsNullOrEmpty(propertyName)) return;
			if (onPropertyChanged == null) return;

			// Hook into the property change event
			PropertyChanged += (sender, e) => {

				// Check for the right property
				if (e.PropertyName != propertyName) return;

				// Grab the property & fire action
				FireActionWithNamedProperty(propertyName, onPropertyChanged);
			};

			// Any bound action will want to know the state of the property at the time of the binding
			// so let's fire the action now with the current property value
			FireActionWithNamedProperty(propertyName, onPropertyChanged);
		}

		// Refactored out for code duplication's sake
		void FireActionWithNamedProperty<T>(string propertyName, Action<T> onPropertyChanged) {
			try {
				onPropertyChanged((T)GetType().GetRuntimeProperty(propertyName).GetValue(this));
			} catch (Exception ex) {
				OBDebug.Log("You dun goofed. Either your property name wasn't correct, or the specified type was incorrect.", propertyName, typeof(T), ex);
				throw ex;
			}
		}

		#endregion
	}
}
