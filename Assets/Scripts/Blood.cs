using UnityEngine;
using System.Threading;

public class Blood : MonoBehaviour
{
    private float size;
    private float time = 0;

    void Start()
    {
        size = Random.Range(0.1f, 0.5f);
        transform.localScale = new Vector3(size, size, 1);
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= 0.5f) GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
    }
}
