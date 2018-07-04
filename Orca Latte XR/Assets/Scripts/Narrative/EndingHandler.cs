using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePhone;

public class EndingHandler : MonoBehaviour {
    public Chat Colleagues;
    public TextAsset[] ColleagueEndings;
    public Chat Boss;
    public TextAsset[] BossEndings;
    public Chat Migrant;
    public TextAsset[] MigrantsEndings;
    public Chat Afia;
    public TextAsset[] AfiaEndings;
    public Chat Peter;
    public TextAsset[] PeterEndings;

    public IEnumerator SendMessages() {
        // Colleagues
        if (PlayerPrefs.GetInt("Theme") == 0) yield return SendMessages(Colleagues, ColleagueEndings[7], false);
        else yield return SendMessages(Colleagues, ColleagueEndings[9], false);

        if (PlayerPrefs.GetInt("Food") == 0) yield return SendMessages(Colleagues, ColleagueEndings[5], false);
        else yield return SendMessages(Colleagues, ColleagueEndings[3], false);

        if (PlayerPrefs.GetInt("Drinks") == 0) yield return SendMessages(Colleagues, ColleagueEndings[1], false);
        else yield return SendMessages(Colleagues, ColleagueEndings[0], false);

        if (PlayerPrefs.GetInt("Music") == 0) yield return SendMessages(Colleagues, ColleagueEndings[8], false);
        else yield return SendMessages(Colleagues, ColleagueEndings[2], false);

        int likes = 0;
        if (PlayerPrefs.GetInt("Theme") == 0) likes++;
        if (PlayerPrefs.GetInt("Food") == 1) likes++;
        if (PlayerPrefs.GetInt("Drinks") == 0) likes++;
        if (PlayerPrefs.GetInt("Music") == 0) likes++;

        if (likes > 2) yield return SendMessages(Colleagues, ColleagueEndings[6], false);
        else yield return SendMessages(Colleagues, ColleagueEndings[4], false);

        yield return WaitForChatToFinish(Colleagues);

        // Boss
        if (PlayerPrefs.GetInt("Drinks") == 0) yield return SendMessages(Boss, BossEndings[0], false);
        else yield return SendMessages(Boss, BossEndings[1], false);

        yield return WaitForChatToFinish(Boss);

        // Migrants
        if (PlayerPrefs.GetInt("Theme") == 0) yield return SendMessages(Migrant, MigrantsEndings[8], false);
        else yield return SendMessages(Migrant, MigrantsEndings[9], false);

        if (PlayerPrefs.GetInt("Food") == 0) yield return SendMessages(Migrant, MigrantsEndings[4], false);
        else yield return SendMessages(Migrant, MigrantsEndings[3], false);

        if (PlayerPrefs.GetInt("Drinks") == 0) yield return SendMessages(Migrant, MigrantsEndings[1], false);
        else yield return SendMessages(Migrant, MigrantsEndings[0], false);

        if (PlayerPrefs.GetInt("Music") == 0) yield return SendMessages(Migrant, MigrantsEndings[5], false);
        else yield return SendMessages(Migrant, MigrantsEndings[2], false);

        likes = 0;
        if (PlayerPrefs.GetInt("Theme") == 1) likes++;
        if (PlayerPrefs.GetInt("Food") == 0) likes++;
        if (PlayerPrefs.GetInt("Drinks") == 1) likes++;
        if (PlayerPrefs.GetInt("Music") == 1) likes++;

        if (likes > 2) yield return SendMessages(Migrant, MigrantsEndings[7], false);
        else yield return SendMessages(Migrant, MigrantsEndings[6], false);

        yield return WaitForChatToFinish(Migrant);

        // Afia
        if (likes > 2) yield return SendMessages(Afia, AfiaEndings[1], false);
        else           yield return SendMessages(Afia, AfiaEndings[0], false);

        yield return WaitForChatToFinish(Afia);

        ChatApp chatApp = FindObjectOfType<ChatApp>();
        if (chatApp)
        {
            chatApp.UpdateLastMessages();
        }
    }

    public IEnumerator ProcessEndGame() {
        // Migrant likes
        int likes = 0;
        if (PlayerPrefs.GetInt("Theme") == 1) likes++;
        if (PlayerPrefs.GetInt("Food") == 0) likes++;
        if (PlayerPrefs.GetInt("Drinks") == 1) likes++;
        if (PlayerPrefs.GetInt("Music") == 1) likes++;

        //Afia
        if (likes <= 2)
        {
            Afia.SetStory(AfiaEndings[2]);
            yield return WaitForChatToFinish(Afia);
        }

        // Peter
        Peter.SetStory(PeterEndings[6]);
        yield return WaitForChatToFinish(Peter);

        if (PlayerPrefs.GetInt("Food") == 0)   yield return SendMessages(Peter, PeterEndings[4], true);
        else                                   yield return SendMessages(Peter, PeterEndings[3], true);

        if (PlayerPrefs.GetInt("Drinks") == 0) yield return SendMessages(Peter, PeterEndings[1], true);
        else                                   yield return SendMessages(Peter, PeterEndings[0], true);

        if (PlayerPrefs.GetInt("Music") == 0)  yield return SendMessages(Peter, PeterEndings[5], true);
        else                                   yield return SendMessages(Peter, PeterEndings[2], true);

        yield return WaitForChatToFinish(Peter);

        if (PlayerPrefs.GetInt("Theme") == 0)  Peter.SetStory(PeterEndings[7]);
        else                                   Peter.SetStory(PeterEndings[8]);
        yield return WaitForChatToFinish(Peter);
    }

    public IEnumerator WaitForChatToFinish (Chat chat)
    {
        bool finished = false;
        while (!finished)
        {
            yield return new WaitForSeconds(.2f);

            if (chat.story) finished = chat.story.finished && chat.queue.isEmpty;
            else            finished = chat.queue.isEmpty;
        }
    }

    public IEnumerator SendMessages (Chat chat, TextAsset asset, bool realTime)
    {
        string conversation = System.Text.Encoding.Default.GetString(asset.bytes);
        string[] messages = conversation.Split('\n');

        foreach (string m in messages)
        {
            NarrativeHandler.instance.SendMessage(chat, m, !realTime);
            if (!realTime)
            {
                chat.unreadMessages++;
            }
        }

        yield return WaitForChatToFinish(chat);
    }
}
