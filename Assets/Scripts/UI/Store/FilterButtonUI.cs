using RPG.Inventories;
using RPG.Shops;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
	public class FilterButtonUI : MonoBehaviour
	{
		[SerializeField] private ItemCategory category = ItemCategory.None;
		private Button button;
		private Store currentStore;

		private void Awake()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(SelectFilter);
		}

		public void SetStore(Store store)
		{
			currentStore = store;
		}

		public void RefreshUI()
		{
			button.interactable = currentStore.GetFilter() != category;
		}
		
		private void SelectFilter()
		{
			currentStore.SelectFilter(category);
		}
	}
}
