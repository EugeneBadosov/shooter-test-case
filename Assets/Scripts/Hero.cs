using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    private Rigidbody2D rb;
    public float heroSpeed;
    public float rotationSpeed;
    public GameObject bullet;
    public GameObject shoot;
    public Camera cam;
    public Image reloadImage;
    public Image HPImage;
    public GameObject HPBorder;
    public GameObject blood;
    public float MaxHP;

    private float reloadTime = 2f;
    private float reloadTimeNow = 0;
    private float HP;
    private GameManager gm;
    private Vector2 moveDirection = Vector2.zero;

    void Start()
    {
        HPBorder.SetActive(false);
        HP = MaxHP;
        HPImage.fillAmount = 0;
        gm = cam.GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        MoveLogic();
        FireLogic();
    }
    private void MoveLogic()
    {
        float vert = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");
        float mouseX = Input.GetAxis("Mouse X");
        moveDirection = new Vector2(hor * heroSpeed * Time.deltaTime, vert * heroSpeed * Time.deltaTime);
        //moveDirection = transform.TransformDirection(moveDirection);
        rb.transform.Translate(moveDirection);
        rb.transform.Rotate(0, 0, -1 * mouseX * rotationSpeed * Time.deltaTime);
    }

    private void Damage(int dmg)
    {
        HP -= dmg;
        HPBorder.SetActive(true);
        HPImage.fillAmount = HP / MaxHP;
        if (HP > 5)
        {
            HPImage.color = new Color(-HP / 5f + 2, 1, 0);
        }
        else
        {
            HPImage.color = new Color(1, HP/4f - 0.25f, 0);
        }
        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gm.EnemyWin();
    }

    private void FireLogic()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && reloadTimeNow <= 0)
        {
            Instantiate(bullet, shoot.transform.position, transform.rotation);
            reloadTimeNow = reloadTime;
            reloadImage.fillAmount = 1;
        }

        if (reloadTimeNow > 0)
        {
            reloadTimeNow -= Time.deltaTime;
            reloadImage.fillAmount = reloadTimeNow / reloadTime;
        }
    }
    private void Blooding(Collision2D col)
    {
        float rangeX = Random.Range(col.gameObject.GetComponent<Rigidbody2D>().velocity.x - 20f, col.gameObject.GetComponent<Rigidbody2D>().velocity.x + 20f);
        float rangeY = Random.Range(col.gameObject.GetComponent<Rigidbody2D>().velocity.y - 20f, col.gameObject.GetComponent<Rigidbody2D>().velocity.y + 20f);
        Instantiate(blood, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = new Vector2(rangeX, rangeY);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy's Bullet")
        {
            for (int i = 0; i <= Random.Range(4, 8); i++)
            {
                Blooding(collision);
            }
            Destroy(collision.gameObject);
            Damage(1);
            GetComponent<Rigidbody2D>().angularVelocity = 0;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
