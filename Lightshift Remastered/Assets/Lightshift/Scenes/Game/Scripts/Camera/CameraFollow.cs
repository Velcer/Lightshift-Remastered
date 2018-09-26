using UnityEngine;
 using System.Collections;

public class CameraFollow : MonoBehaviour
{

    public static CameraFollow Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public GameObject Target;
    public Vector3 Offset;

    void LateUpdate()
    {
        if (Target)
            transform.position = Vector3.MoveTowards(transform.position, Target.transform.position + Offset, Time.deltaTime *  100);
    }
}