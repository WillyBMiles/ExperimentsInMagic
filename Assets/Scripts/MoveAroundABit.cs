using UnityEngine;

public class MoveAroundABit : MonoBehaviour
{
    public float howMuch;
    Vector3 startingPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = startingPosition + Random.insideUnitSphere * howMuch;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);
    }
}
