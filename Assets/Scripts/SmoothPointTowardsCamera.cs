using UnityEngine;

public class SmoothPointTowardsCamera : MonoBehaviour
{
    public bool useWorldParentOffset;
    public Vector3 worldParentOffset;

    public float smooth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion target = Quaternion.LookRotation(-(Camera.main.transform.position - transform.position), Vector3.up);
        if (smooth > 100f)
        {
            transform.rotation = target;
        }
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

        if (useWorldParentOffset)
        {
            transform.position = transform.parent.position + worldParentOffset;
        }
    }

    private void OnEnable()
    {
        Quaternion target = Quaternion.LookRotation(-(Camera.main.transform.position - transform.position), Vector3.up);
        transform.rotation = target;
    }
}
