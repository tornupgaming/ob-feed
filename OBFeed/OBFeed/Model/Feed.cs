using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OBFeed {
	public class Feed {
		
		public string Url { get; set; }

		public ObservableCollection<FeedItem> Items = new ObservableCollection<FeedItem>();

		/// <summary>
		/// Returns a list of items that contain the given term (either title or pubdate).
		/// It just uses a very very simple string.Contains() 
		/// </summary>
		/// <returns>The items by search term.</returns>
		/// <param name="term">Term that the item should contain.</param>
		public IEnumerable<FeedItem> GetItemsBySearchTerm(string term) {
			if (string.IsNullOrEmpty(term)) return Items;
			return Items.Where(item => item.Title.Contains(term) || item.PubDate.Contains(term));
		}
	}
}
