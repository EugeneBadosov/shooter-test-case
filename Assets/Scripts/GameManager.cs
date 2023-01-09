using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static int heroPoint = 0, enemyPoint = 0;
    public Text enPoint, hePoint;
    public GameObject winPanel, losePanel;

    void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        enPoint.text = enemyPoint.ToString();
        hePoint.text = heroPoint.ToString();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        
    }

    public void HeroWin()
    {
        heroPoint++;
        hePoint.text = heroPoint.ToString();
        Time.timeScale = 0;
        winPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnemyWin()
    {
        enemyPoint++;
        enPoint.text = enemyPoint.ToString();
        Time.timeScale = 0;
        losePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Time.timeScale = 1;
        heroPoint = 0;
        enemyPoint = 0;
        SceneManager.LoadScene("MainMenu");
    }

}
