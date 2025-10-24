using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Get the start and end movement points for platforms
public class Keypath : MonoBehaviour {

    public Transform getKeypath(int keypathIndex) {
        return transform.GetChild(keypathIndex);
    }

    public int getNextKeypathIndex(int currentKeypathIndex) {
        int nextKeypathIndex = currentKeypathIndex + 1;

        if (nextKeypathIndex == transform.childCount) {
            nextKeypathIndex = 0;
        }

        return nextKeypathIndex;
    }

}