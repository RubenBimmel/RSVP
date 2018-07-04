using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int startTime;
    public int startDay;
    public static float time;
    public static int day;
    //private GameEvent[] events;

    // Use this for initialization
    void Awake() {
        if (!PlayerPrefs.HasKey("gameTime")) {
            time = startTime;
            day = startDay;
            PlayerPrefs.SetFloat("gameTime", time);
            PlayerPrefs.SetInt("gameDay", day);
            ResetNarrative();
        }
        else {
            time = PlayerPrefs.GetFloat("gameTime");
            day = PlayerPrefs.GetInt("gameDay");
        }

        // Set all chat story states
        GamePhone.Chat[] chats = Resources.LoadAll<GamePhone.Chat>("Apps/ChatApp/Chats");
        foreach (GamePhone.Chat c in chats) {
            if (c.queue != null) {
                c.queue.ResetQueue();
            }
            else {
                c.queue = new GamePhone.MessageQueue();
                c.queue.ResetQueue();
            }

            if (c.story)
            {
                if (PlayerPrefs.HasKey(c.name))
                {
                    c.story.currentPassage = PlayerPrefs.GetInt(c.name);
                }
                else
                {
                    c.story.currentPassage = c.story.startPassage;
                }
            }
        }
    }

    void OnApplicationQuit() {
        // Save current game time
        PlayerPrefs.SetFloat("gameTime", time);
        PlayerPrefs.SetInt("gameDay", day);

        // Save current chat states
        GamePhone.Chat[] chats = Resources.LoadAll<GamePhone.Chat>("Apps/ChatApp/Chats");
        foreach (GamePhone.Chat c in chats) {
            if (c.story)
            {
                PlayerPrefs.SetInt(c.name, c.story.currentPassage);
            } else
            {
                PlayerPrefs.DeleteKey(c.name);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        UpdateTime();
    }

    private void UpdateTime() {
        time += Time.deltaTime;
        if (time >= 86400) {
            time -= 86400;
            day++;
        }
    }

    public static string GetTimeAsString(int timeInSeconds) {
        int time = timeInSeconds / 60;
        int min = time % 60;
        int hour = (time - min) / 60;
        string divider = (min < 10) ? ":0" : ":";
        return hour + divider + min;
    }

    public void ResetGame() {
        Debug.Log("Resetting Game");
        if (System.IO.Directory.Exists(Application.persistentDataPath + "/Resources")) {
            System.IO.Directory.Delete(Application.persistentDataPath + "/Resources", true);
        }
        PlayerPrefs.DeleteAll();
        time = startTime;
        day = startDay;
        PlayerPrefs.SetFloat("gameTime", time);
        PlayerPrefs.SetInt("gameDay", day);
        ResetNarrative();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void ResetNarrative() {
        GamePhone.Contact[] contacts = Resources.LoadAll<GamePhone.Contact>("Apps/ChatApp/Contacts");
        foreach (GamePhone.Contact c in contacts)
        {
            c.state = GamePhone.Contact.State.Offline;
            c.offlineTimer = 0;
            c.readDelay = 0;
        }

        GamePhone.Chat[] chats = Resources.LoadAll<GamePhone.Chat>("Apps/ChatApp/Chats");
        foreach (GamePhone.Chat c in chats) {
            /*if (c.story) {
                c.story.ResetStory();

                PlayerPrefs.SetInt(c.name, c.story.startPassage);
                GamePhone.ChatApp.ClearDialogueOptions(c);
            }*/

            c.story = null;

            c.queue.ResetQueue();
            c.LoadPreConversation();
            c.unreadMessages = 0;
        }

        NarrativeHandler.instance.active = false;
    }
}
