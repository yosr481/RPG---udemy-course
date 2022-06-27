using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Attributes
{
	public class FPSDisplay : MonoBehaviour
	{
		private void Update()
		{
			float fps = 1 / Time.unscaledDeltaTime;
			GetComponent<Text>().text = $"FPS: {fps:000}";
		}
	}
}
