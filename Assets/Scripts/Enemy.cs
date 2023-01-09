using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float moveSpeed;
    public float rotationSpeed;
    public GameObject shoot;
    public GameObject bullet;
    public GameObject HPBorder;
    public Image reloadImage;
    public Image HPImage;
    public Camera cam;
    public float MaxHP;
    public GameObject blood;

    private Vector3 target;
    private Vector2 rebountTarget;
    private Transform me;
    private NavMeshAgent agent;
    private Ray2D aim = new Ray2D();
    private Ray2D sideAim = new Ray2D();
    private RaycastHit2D sideHit;
    private float angle = 0;
    private float reload = 2f;
    private float reloadNow = 0;
    private float HP;
    private bool OnTarget = false;
    private bool OnReboundTarget = false;
    private GameManager gm;

    void Start()
    {
        HP = MaxHP;
        HPImage.fillAmount = 0;
        gm = cam.GetComponent<GameManager>();
        me = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        target = transform.position;
        HPBorder.SetActive(false);
    }


    void Update()
    {
        AimLogic();
        SetTargetPosition();
        SetAgentPosition();
        SetAgentRotate(player.position);
        SideAimLogic();
        Reload();
    }

    /// <summary>
    /// �����������
    /// </summary>
    void Reload()
    {
        if (reloadNow > 0)
        {
            reloadNow -= Time.deltaTime;
            reloadImage.fillAmount = reloadNow / reload;
        }
    }

    /// <summary>
    /// ����� ��������� ���������
    /// </summary>
    void SideAimLogic()
    {
        sideAim.origin = transform.position;                                            //�������� ������ ����������� aim
        sideAim.direction = aim.direction;
        sideAim.direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));            //��������������� ���� ��������
        angle = angle + Mathf.PI / 180;                                                 //���������� ����
        ShootLogic(sideAim);                                                            //����������� ���������
    }

    /// <summary>
    /// ������ �������� ����������
    /// </summary>
    void SetAgentRotate(Vector2 target)
    {
        if (!(OnReboundTarget || OnTarget))
        {
            LookAt2D(target, false);
        }
    }

    /// <summary>
    /// ��������� ������� �����������
    /// </summary>
    void SetTargetPosition()
    {
        if (reloadNow <= 0)
        {
            if (OnTarget || OnReboundTarget)                                                //���� �� �� �������
            {
                target = transform.position;                                                //�� ������������
            }
            else
            {
                target = player.position;                                                   //����� �����
            }
        }
    }

    /// <summary>
    /// �����������
    /// </summary>
    void SetAgentPosition()
    {
        agent.SetDestination(new Vector3(target.x, target.y, 0));
    }

    /// <summary>
    /// ������ ������������
    /// </summary>
    void AimLogic()
    {
        aim.origin = transform.position;                                                //������ ���
        aim.direction = shoot.transform.up;
        ShootLogic(aim);                                                                //��������� ���������
    }

    /// <summary>
    /// ����� ���������
    /// </summary>
    /// <param name="aim">��� �������</param>
    void ShootLogic(Ray2D aim)
    {
        RaycastHit2D hit = Physics2D.Raycast(aim.origin, aim.direction, 6f);            //����� ������������ ���� � ���������
        if (hit && !OnReboundTarget)                                                                        //���� ���� ���������
        {
            if (hit.collider.gameObject.tag == "Player")                                //���� ���������� �������� �����
            {
                OnTarget = true;                                                        //������ �������������
                Fire();                                                  //�������
            }
            else if (hit.collider.gameObject.tag == "Wall")                             //���� ���������� �������� �����
            {
                OnTarget = false;                                                       //���������� ������������� � ������
                Ray2D rebound = new Ray2D(hit.point, Vector2.Reflect(aim.direction, hit.normal));       //������ ��� �� �����

                RaycastHit2D hitBound = Physics2D.Raycast(rebound.origin, rebound.direction, 5f);       //���� ������������
                if (hitBound && hitBound.collider.gameObject.tag == "Player")                           //���� ������� �������� � ������
                {
                    rebountTarget = hit.point;
                    OnReboundTarget = true;                                                                   //��������
                    LookAt2D(rebountTarget, true);
                }
            }
        }
        else if (OnReboundTarget)
        {
            LookAt2D(rebountTarget, true);
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="aim">������</param>
    private void Fire()
    {
        if (reloadNow <= 0)
        {
                Instantiate(bullet, shoot.transform.position, transform.rotation);
                reloadNow = reload;
                reloadImage.fillAmount = 1;
                target = new Vector2(Random.Range(-11f, 18f), Random.Range(-10f, 19f));
                OnTarget = false;
                OnReboundTarget = false;
        }
        else
        {
            OnTarget = false;
            OnReboundTarget = false;
        }
    }

    /// <summary>
    /// ��������� ����
    /// </summary>
    /// <param name="collision">�������� ����</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
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

    private void Blooding(Collision2D col)
    {
        float rangeX = Random.Range(col.gameObject.GetComponent<Rigidbody2D>().velocity.x - 20f, col.gameObject.GetComponent<Rigidbody2D>().velocity.x + 20f);
        float rangeY = Random.Range(col.gameObject.GetComponent<Rigidbody2D>().velocity.y - 20f, col.gameObject.GetComponent<Rigidbody2D>().velocity.y + 20f);
        Instantiate(blood, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = new Vector2(rangeX, rangeY);
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
            HPImage.color = new Color(1, HP / 4f - 0.25f, 0);
        }
        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gm.HeroWin();
    }

    /// <summary>
    /// ��������� �������2 �� ����������� � ���������
    /// </summary>
    /// <param name="point">���������� ������</param>
    /// <returns></returns>
    private Vector2 WorldToLocal(Vector2 point)
    {
        point = new Vector2(point.x - transform.position.x, point.y - transform.position.y);
        return point;
    }

    /// <summary>
    /// ������� ����� ���������
    /// </summary>
    /// <param name="point">������ ������</param>
    /// <returns></returns>
    private float AngleDif(Vector2 point)
    {
        point = WorldToLocal(point);
        float angle = -Vector2.SignedAngle(point, Vector2.up);
        if(angle < 0)
        {
            return 360 + angle;
        }
        return angle;
    }

    /// <summary>
    /// ��������� ������� ���������
    /// </summary>
    /// <param name="point">����� �������</param>
    /// <param name="fire">����� �� ��������</param>
    private void LookAt2D(Vector2 point, bool fire)
    {
        float signedAngle = AngleDif(point);
        float rotate = Mathf.Lerp(me.eulerAngles.z, signedAngle, rotationSpeed * Time.deltaTime);
        me.Rotate(0, 0, rotate - me.eulerAngles.z);
        if (fire && Mathf.Abs(signedAngle - me.eulerAngles.z) < 0.2)
        {
            Fire();
        }
    }
}
