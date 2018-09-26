using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerConnector : MonoBehaviour {

	void Start () => PlayerIONetwork.net.RequestGameServer();
	
}
