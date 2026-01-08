using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
	[Header("Audio Settings")]
    [SerializeField] private float fadeDuration = 1.0f;
	private AudioSource audioSource;
	
	private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
	
    private void OnTriggerEnter(Collider other){
		PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
		
		if (playerInventory != null){
			playerInventory.IncrementCollected();
			
			StartCoroutine(FadeOutAndDisable());
		}
	}
	
	private IEnumerator FadeOutAndDisable()
    {
        // Disable collider
        GetComponent<Collider>().enabled = false;

        // Disable the Visuals so it looks "collected"
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null) rend.enabled = false;

        // Fade volume
        float startVolume = audioSource.volume;
        float currentTime = 0;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeDuration);
            yield return null;
        }
		
        audioSource.Stop();
        audioSource.volume = startVolume;
        gameObject.SetActive(false);
    }
}
