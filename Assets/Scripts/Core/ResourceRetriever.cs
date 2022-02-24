using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
	public static class ResourceRetriever<T> where T : ScriptableObject, IHasItemID
	{
		private static Dictionary<string, T> _itemLookupCache;
		
		public static T GetFromID(string itemID)
		{
			if (_itemLookupCache == null)
			{
				_itemLookupCache = new Dictionary<string, T>();
				var itemList = Resources.LoadAll<T>("");
				foreach (var item in itemList)
				{
					if (_itemLookupCache.ContainsKey(item.GetItemID()))
					{
						Debug.LogError($"Looks like there's a duplicate  ID for objects: {_itemLookupCache[item.GetItemID()]} and {item}");
						continue;
					}

					_itemLookupCache[item.GetItemID()] = item;
				}
			}

			if (itemID == null || !_itemLookupCache.ContainsKey(itemID)) return null;
			return _itemLookupCache[itemID];
		}
	}
}
