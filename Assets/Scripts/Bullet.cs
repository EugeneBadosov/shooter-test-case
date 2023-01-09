using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = transform.up * speed;
    }

    void Update()
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Vector2 l = rb.velocity;
            Vector2 n = collision.contacts[0].normal;
            Vector2 r;

            r = Vector2.Reflect(l, n);

            rb.velocity = r;
            rb.transform.LookAt2D((Vector2)rb.transform.position + r);

        }
    }

}

static class Extensions
{
    #region LookAt2D
    public static void LookAt2D(this Transform me, Vector3 target, Vector3? eye = null)
    {
        float signedAngle = Vector2.SignedAngle(eye ?? me.up, target - me.position);

        if (Mathf.Abs(signedAngle) >= 1e-3f)
        {
            var angles = me.eulerAngles;
            angles.z += signedAngle;
            me.eulerAngles = angles;
        }
    }
    public static void LookAt2D(this Transform me, Transform target, Vector3? eye = null)
    {
        me.LookAt2D(target.position, eye);
    }
    public static void LookAt2D(this Transform me, GameObject target, Vector3? eye = null)
    {
        me.LookAt2D(target.transform.position, eye);
    }
    #endregion
}
