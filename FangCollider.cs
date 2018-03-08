using UnityEngine;
using System.Collections;

public class FangCollider : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        OverworldManager.instance.ICollided();
    }
}