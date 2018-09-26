using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePlayer : MonoBehaviour {

    void Update()
    {
        if (GameManager.Instance !=null)
        if (GameManager.Instance.sceneReady)
        {
            NetworkManager.Instance.InstantiatePlayer();
            Destroy(this);
        }
    }
}
