using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePhone;

public class NarrativeHandler : MonoBehaviour {

    public static NarrativeHandler instance;
    // used to set values in editor
    public Contact You;
    public StoryBeat startingBeat;
    public bool active;

    public InteractableObject[] objects;
    public StoryBeat[] triggers;
    public MeshRenderer XREnvironment;
    public MeshRenderer fader;
    public TMPro.TextMeshPro[] faderDisplays;
    public Material nightMaterial;

    public bool debugMode;

    private Chat[] chats;
    private Contact[] contacts;
    private ChatApp chatApp;

    // Called on start of application
    private void Start() {
        instance = this;

        if (!active) {
            active = true;
        }

        chats = Resources.LoadAll<Chat>("Apps/ChatApp/Chats");
        foreach (Chat c in chats) {
            UpdatePassage(c);
        }
        contacts = Resources.LoadAll<Contact>("Apps/ChatApp/Contacts");
        foreach (Contact c in contacts) {
            c.readDelay = 0f;
            c.state = Contact.State.Offline;
        }

        foreach (InteractableObject io in objects)
        {
            if (PlayerPrefs.HasKey("scene"))
            {
                io.gameObject.SetActive(false);
            }
            else
            {
                io.gameObject.SetActive(PlayerPrefs.HasKey(io.name));
            }
        }

        if (PlayerPrefs.GetInt("scene") == 1)
        {
            XREnvironment.material = nightMaterial;
        }

        startingBeat.Activate();
    }

    private void Update()
    {
        foreach (Chat c in chats)
        {
            c.queue.Update();
        }

        foreach (Contact c in contacts) {
            c.UpdateReading();
        }

        if (debugMode)
        {
            if (UnityEngine.Input.GetKeyDown("1"))
            {
                Chat afia = GetComponent<EndingHandler>().Afia;
                afia.story = null;
                afia.queue.ResetQueue();
                PlayerPrefs.SetInt("Food", 0);
                PlayerPrefs.SetInt("Theme", 0);
                PlayerPrefs.SetInt("Music", 0);
                PlayerPrefs.SetInt("Drinks", 0);
                StartCoroutine(ProcessEndGame());
            }
        }
    }

    public void SwitchDebugMode ()
    {
        debugMode = !debugMode;
    }

    public void OnFinishedStory (StoryBeat beat)
    {
        StartCoroutine(WaitForQueue(beat));
    }

    public IEnumerator WaitForQueue(StoryBeat beat)
    {
        yield return new WaitForSeconds(.2f);

        // Wait for chat to finish
        while (!beat.chat.queue.isEmpty)
        {
            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(0f, 1.5f));

        // Start next chat
        beat.OnFinishedStory.Invoke();

        // Check for triggers
        for (int i = 0; i < triggers.Length; i++)
        {
            if (triggers[i] == beat)
            {
                objects[i].gameObject.SetActive(true);
            }
        }
    }

    public void OnInteractWithObject ()
    {
        bool finishedPreparing = true;
        foreach (InteractableObject io in objects)
        {
            if (io.selected < 0)
            {
                finishedPreparing = false;
            }
        }

        if (finishedPreparing)
        {
            StartCoroutine(ProcessEndGame());
        }
    }

    private IEnumerator ProcessEndGame()
    {
        yield return new WaitForSeconds(1f);

        //faderTimer.text = GameManager.GetTimeAsString((int)GameManager.time);
        Color c = fader.material.color;

        while (fader.material.color.a < 1)
        {
            c.a += Time.deltaTime;
            fader.material.color = c;
            faderDisplays[0].color = new Color(1, 1, 1, c.a);
            faderDisplays[1].color = new Color(1, 1, 1, c.a);
            yield return null;
        }

        StartCoroutine(UpdateEndGameTime());

        Phone phone = Transform.FindObjectOfType<Phone>();
        phone.GetNotified(2);

        XREnvironment.material = nightMaterial;
        PlayerPrefs.SetInt("scene", 1);
        //GameManager.time = 77880;
        //GameManager.day++;

        EndingHandler endingHandler = GetComponent<EndingHandler>();
        yield return endingHandler.SendMessages();

        foreach (InteractableObject io in objects)
        {
            io.gameObject.SetActive(false);
        }

        while (fader.material.color.a > 0)
        {
            c.a -= Time.deltaTime;
            fader.material.color = c;
            faderDisplays[0].color = new Color(1, 1, 1, c.a);
            faderDisplays[1].color = new Color(1, 1, 1, c.a);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        yield return endingHandler.ProcessEndGame();
    }

    private IEnumerator UpdateEndGameTime () {
        float startDay = GameManager.day;
        float timeToAdd = 77880 - GameManager.time + 86400;
        float t = 0f;
        while (t < 1 || GameManager.day == startDay)
        {
            //faderTimer.text = GameManager.GetTimeAsString((int)GameManager.time);
            //GameManager.time += 600 * Time.deltaTime;
            t += Time.deltaTime / 8f;
            GameManager.time += Mathf.Clamp01(InOutQuadBlend(t)) * timeToAdd;
            timeToAdd -= Mathf.Clamp01(InOutQuadBlend(t)) * timeToAdd;
            yield return null;
        }

        GameManager.time = 77880;
    }

    float InOutQuadBlend(float t)
    {
        if (t <= 0.5f)
            return 2.0f * t * t;
        t -= 0.5f;
        return 2.0f * t * (1.0f - t) + 0.5f;
    }

    // Send a new message and apply the players choice
    public void MakeDecision (int id, Chat chat) {
        chat.story.MakeDecision(id);

        ChatApp.ClearDialogueOptions(chat);
        ResetMessageScreen(false, true, false, false);

        UpdatePassage(chat);
    }

    // Load all messages and decisions from the current passage
    public void UpdatePassage (Chat chat) {
        if (chat.story) {
            foreach (string message in chat.story.GetMessages()) {
                SendMessage(chat, message);
            }

            // Reset all decisions
            ChatApp.ClearDialogueOptions(chat);

            Decision[] newDecisions = chat.story.GetDecisions();
            foreach (Decision d in newDecisions) {
                MessageOption option = ScriptableObject.CreateInstance<MessageOption>();
                option.name = chat.story.currentPassage.ToString();
                option.chat = chat;
                option.message = d.message;
                option.id = d.link;
                ChatApp.AddDialogueOption(option);
            }
        }
    }

    // Send message based on formatted string
    public void SendMessage(Chat chat, string formattedMessage) {
        SendMessage(chat, formattedMessage, false);
    }

    // Send message based on formatted string
    public void SendMessage (Chat chat, string formattedMessage, bool preChat) {
        if (formattedMessage.Length > 0)
        {
            string[] components = formattedMessage.Split(']');

            if (components.Length == 1)
            {
                SendMessage(chat, chat.contacts[0], components[0], preChat);
            }
            else
            {
                components[0] = components[0].Remove(0, 1);
                string path = "Apps/ChatApp/Contacts/" + components[0];
                Contact c = Resources.Load<Contact>(path);
                if (!c)
                {
                    Debug.LogWarning("Invalid contact name: " + components[0]);
                    c = chat.contacts[0];
                }
                SendMessage(chat, c, components[1], preChat);
            }
        }
    }

    // Send message
    public void SendMessage(Chat chat, Contact contact, string newMessage) {
        SendMessage(chat, contact, newMessage, false);
    }

    // Send message
    public void SendMessage (Chat chat, Contact contact, string newMessage, bool preChat)
    {
        /*// Check if message alread exists
        bool messageExists = false;
        string path = Application.persistentDataPath + "/Resources/ChatApp/Messages/" + chat.name + "/";

        if (!preChat) {
            if (System.IO.Directory.Exists(path)) {
                Message[] messages = SOSaver.LoadAll<Message>(path);
                for (int i = 0; i < messages.Length; i++) {
                    if (messages[i] && messages[i].message == newMessage) {
                        messageExists = true;
                    }
                }
            }
        }*/

        bool messageExists = false;

        // Create message from string
        if (!messageExists)
        {
            Message message = ScriptableObject.CreateInstance<Message>();
            message.name = newMessage.Length > 10 ? newMessage.Substring(0, 10) : newMessage;
            message.chat = chat;
            if (contact)
            {
                message.contact = contact;
            }
            else
            {
                message.contact = chat.contacts[0];
            }
            message.message = newMessage;
            if (!chat.IsPersonalChat() && contact != You)
            {
                message.text = "<#2ECC71>" + contact.name + "</color>" + '\n' + newMessage;
            } else
            {
                message.text = newMessage;
            }
            message.delay += Mathf.CeilToInt((float)newMessage.Length / (10f * message.contact.writingSpeed));

            // Send message to queue or to phone
            if (!preChat) {
                chat.queue.AddToQueue(message);
            } else {
                Send(message, true);
            }
        }
    }

    // Send message to the phone
    public void Send (Message message) {
        Send(message, false);
    }

    // Send message to the phone
    public void Send (Message message, bool preChat)
    {
        Phone phone = Transform.FindObjectOfType<Phone>();

        ChatApp.SaveMessage(message);

        if (!preChat) {
            foreach (Contact c in message.chat.contacts) {
                c.AddMessageToRead(message);
                StartCoroutine(NotifyContact(c));
            }

            if (message.contact != You) {
                ChatApp chat = FindObjectOfType<ChatApp>();
                if (chat) {
                    if (chat.activeChat != message.chat) {
                        phone.GetNotified(0);
                        message.chat.unreadMessages++;
                    }
                    else {
                        phone.GetNotified(1);
                    }
                }
                else {
                    phone.GetNotified(0);
                    message.chat.unreadMessages++;
                }

                StartCoroutine(GetOnline(message.contact));
            }
            else {
                phone.GetNotified(1);
            }

            ResetMessageScreen(true, false, true, false);
            if (!chatApp)
            {
                chatApp = FindObjectOfType<ChatApp>();
            }
            if (chatApp)
            {
                chatApp.UpdateContacts(message);
            }

            message.chat.CheckIfFinished();
        }
    }

    // Reset the chat app
    public void ResetMessageScreen (bool resetMessages, bool clearDecisions, bool resetDecisions, bool resetSendBox) {
        if (!chatApp) {
            chatApp = FindObjectOfType<ChatApp>();
        }

        if (chatApp && chatApp.messageScreenInstance) {
            if (resetMessages) chatApp.messageScreenInstance.ResetMessages(chatApp.activeChat);
            if (clearDecisions) chatApp.messageScreenInstance.ClearDecisions();
            if (resetDecisions) chatApp.messageScreenInstance.ResetDecisions(chatApp.activeChat);
            if (resetSendBox) chatApp.messageScreenInstance.sendBox.Reset(chatApp.activeChat);
        }
    }

    // Coroutine to make the contact come online after response time
    private IEnumerator NotifyContact (Contact contact)
    {
        contact.offlineTimer = Time.time + contact.activeTime;

        if (contact.state == Contact.State.Offline)
        {
            ChatApp chat = FindObjectOfType<ChatApp>();
            contact.state = Contact.State.Notified;
            yield return new WaitForSeconds(contact.responseTime);
            contact.state = Contact.State.Online;
            if (chat && chat.messageScreenInstance)
                chat.messageScreenInstance.SetStatus(chat.activeChat);

            while (contact.state == Contact.State.Online)
            {
                if (Time.time > contact.offlineTimer)
                {
                    contact.state = Contact.State.Offline;
                    if (chat && chat.messageScreenInstance)
                        chat.messageScreenInstance.SetStatus(chat.activeChat);
                }
                yield return null;
            }
        }
    }

    // Coroutine to make the contact come online
    private IEnumerator GetOnline(Contact contact)
    {
        contact.offlineTimer = Time.time + contact.activeTime;

        ChatApp chat = FindObjectOfType<ChatApp>();
        contact.state = Contact.State.Online;
        if (chat && chat.messageScreenInstance)
            chat.messageScreenInstance.SetStatus(chat.activeChat);

        while (contact.state == Contact.State.Online)
        {
            if (Time.time > contact.offlineTimer)
            {
                contact.state = Contact.State.Offline;
                if (chat && chat.messageScreenInstance)
                    chat.messageScreenInstance.SetStatus(chat.activeChat);
            }
            yield return null;
        }
    }
}
