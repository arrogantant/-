using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public Playermove Player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartBtn;

    private void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    public void NextStage()
    {
        if(stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE" + (stageIndex + 1);
        }
        else
        {
            //게임끝
            Time.timeScale = 0;
            Debug.Log("게임끝");

            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Game Clear!";
            RestartBtn.SetActive(true);
            
        }
       

        totalPoint += stagePoint;
        stagePoint = 0;


    }
    public void HealthDown()
    {
        if(health > 1) { 
            health--;
            UIhealth[health].color = new Color(1, 1, 1, 0.4f);
        }
        else
        {
            //죽을때
            Player.OnDie();
            //유아이
            Debug.Log("주금");
            RestartBtn.SetActive(true);
            UIhealth[0].color = new Color(1, 1, 1, 0.4f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {

            //체력운지

            if (health > 1)

                //플레이어 리셋        
                PlayerReposition();
            
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        Player.transform.position = new Vector3(0, 0, -1);
        Player.VelocityZero();
    }

   public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        
    }
}
