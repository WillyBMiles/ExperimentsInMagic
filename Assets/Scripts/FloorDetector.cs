using UnityEngine;

public class FloorDetector : MonoBehaviour
{
    public bool detected = false;
    public float radius;

    private void Update()
    {
        detected = Physics.CheckSphere(transform.position, radius, LayerMask.GetMask("Ground"));
    }
}
