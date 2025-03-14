using UnityEngine;

public class DestroyParentless : MonoBehaviour
{
    public float timer = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                Destroy(gameObject);
        }
    }
}
