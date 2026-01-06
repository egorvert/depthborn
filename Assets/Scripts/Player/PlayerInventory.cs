using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public int totalCollectables { get; private set; }
	public int totalDeaths {get; private set; }
	
	public UnityEvent<PlayerInventory> OnCollected;
	public UnityEvent<PlayerInventory> OnDeath;
	
	public void IncrementCollected() {
		totalCollectables++;
		OnCollected.Invoke(this);
	}
	
	public void IncrementDeath(){
		totalDeaths++;
		OnDeath.Invoke(this);
	}
}
