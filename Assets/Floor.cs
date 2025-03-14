using UnityEngine;

public class Floor : MonoBehaviour
{
    public static float FloorHeight { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FloorHeight = transform.position.y;
    }
}
