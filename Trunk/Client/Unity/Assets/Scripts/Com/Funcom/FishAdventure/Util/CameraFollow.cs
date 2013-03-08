using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float followDistance = 10;

    private void Update()
    {
        iTween.MoveUpdate(gameObject, new Vector3(target.position.x, target.position.y, target.position.z - followDistance), .8f);
    }
}