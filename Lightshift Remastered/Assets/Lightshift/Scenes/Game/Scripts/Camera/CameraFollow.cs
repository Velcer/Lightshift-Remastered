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

    //void LateUpdate()
    //{
    //    if (Target)
    //        transform.position = Vector3.MoveTowards(transform.position, Target.transform.position + Offset, Time.deltaTime *  100);
    //}
    void Update()
    {
        if (Target)
        {
            Rigidbody playerStats = Target.GetComponent<Rigidbody>();
            Vector3 speedOffset = new Vector3(playerStats.velocity.x * 0.05f, playerStats.velocity.y * 0.05f, 0);
            transform.position = Target.transform.position + Offset - speedOffset;
            //transform.position = new Vector3(pos.x * 0.25f + targetPos.x * 0.75f, pos.y * 0.25f + targetPos.y * 0.75f, pos.z * 0.25f + targetPos.z * 0.75f);
        }
    }
}