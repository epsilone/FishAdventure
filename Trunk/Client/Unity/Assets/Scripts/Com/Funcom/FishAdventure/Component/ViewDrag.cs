using UnityEngine;
using System.Collections;

public class ViewDrag : MonoBehaviour {
    private Vector3 hit_position = Vector3.zero;
    private Vector3 current_position = Vector3.zero;

    private void Start() {

    }

    private void Update() {
        if (Input.touchCount == 2)
            return;

        if (Input.GetMouseButtonDown(0)) {
            hit_position = Input.mousePosition;

        }

        if (Input.GetMouseButton(0)) {
            current_position = Input.mousePosition;
            LeftMouseDrag();
        }

    }




    void LeftMouseDrag() {
        Vector3 position = hit_position - current_position;
        Vector3 invert = new Vector3(0, position.x, 0);
        transform.Rotate(invert);
        hit_position = current_position;
    }
}