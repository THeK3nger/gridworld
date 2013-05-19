using UnityEngine;
using System.Collections;

public class DiagGuiUpdater : MonoBehaviour {

    GameObject score;
    GameObject life;
    BotAttributes botAtt;
    GameObject timerText;

    public AudioClip playerDeath;
    public AudioClip timeIsrunningOut;

    public float timer = 90;
    public GameObject enemy;
    private bool timeLow = false;

	// Use this for initialization
	void Awake () {
        score = GameObject.Find("Score");
        life = GameObject.Find("Life");
        timerText = GameObject.Find("Timer");
        score.guiText.material.color = Color.red;
        life.guiText.material.color = Color.red;
        timerText.guiText.material.color = Color.red;
	}
	
	// Update is called once per frame
	void Update () {
        GameObject bot = GameObject.Find("BotDiag(Clone)");
        if (bot == null) return;
        if (botAtt == null)
        {
            botAtt = bot.GetComponent<BotAttributes>();
            return;
        }
        timer -= Time.deltaTime;
        if (timer > 0)
        {
            if (timer < 15 && !timeLow)
            {
                AudioSource.PlayClipAtPoint(timeIsrunningOut, transform.position);
                audio.pitch = 1.2f;
                timeLow = true;
            }
            score.guiText.text = "SCORE: " + botAtt.goldCarrying;
            life.guiText.text = "LIFE: " + botAtt.life;
            timerText.guiText.text = "TIME: " + Mathf.Round(timer*10)/10;
        }
        else
        {
            GameObject gameover = GameObject.Find("GAMEOVER");
            gameover.guiText.text = "TIME OUT!";
            AudioSource.PlayClipAtPoint(playerDeath, transform.position);
            audio.Stop();
            Destroy(bot);
        }
	}

}
