using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathUI : MonoBehaviour
{
	private TextMeshProUGUI deathText;
	
    // Start is called before the first frame update
    void Start()
    {
		deathText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void UpdateDeathText(PlayerInventory playerInventory)
    {
        deathText.text = playerInventory.totalDeaths.ToString();
    }
}
