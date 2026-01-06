using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
	private TextMeshProUGUI collectableText;
	
    // Start is called before the first frame update
    void Start()
    {
		collectableText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void UpdateCollectableText(PlayerInventory playerInventory)
    {
        collectableText.text = playerInventory.totalCollectables.ToString();
    }
}
