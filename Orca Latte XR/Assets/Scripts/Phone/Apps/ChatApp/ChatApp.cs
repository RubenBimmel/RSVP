using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GamePhone {
	public class ChatApp : App {

        public static List<Chat> chats;

        public MessageScreen messageScreen;

        [HideInInspector]
        public Chat activeChat;
        [HideInInspector]
        public MessageScreen messageScreenInstance;
        public Grid contactGrid;

        private GameObject contactScreen;
        private List<Division> chatButtons;

        // Called on initialisation
        void Awake() {
            contactScreen = transform.Find("ContactScreen").gameObject;

            chats = new List<Chat>();
            chatButtons = new List<Division>();

            contactGrid = contactScreen.GetComponentInChildren<Grid>();
            InstantiateContacts();
        }

        // Gets executed when the app is opened
        public override void Open() {
            base.Open();
            OpenContactScreen();
        }

        // Gets executed when back button is pressed
        public override bool GoBack() {
            if (messageScreenInstance) {
                OpenContactScreen();
                activeChat = null;
                return true;
            }
            return base.GoBack();
        }

        // Opens the conversation with a contact
        public void OpenMessageScreen(Chat chat) {
            activeChat = chat;
            chat.unreadMessages = 0;
            messageScreenInstance = Instantiate(messageScreen, transform);
            contactScreen.SetActive(false);

            messageScreenInstance.ResetMessages(activeChat);
            messageScreenInstance.ResetDecisions(activeChat);
        }

        // Opens the list of all contacts
        public void OpenContactScreen() {
            if (messageScreenInstance) {
                Destroy(messageScreenInstance.gameObject);
            }
            contactScreen.SetActive(true);
        }

        // Create new list of button for each contact
        public void InstantiateContacts() {
            chats = Resources.LoadAll<Chat>("Apps/ChatApp/Chats").ToList();

            foreach (Chat c in chats) {
                Button newButton = Instantiate(Resources.Load<Button>("Apps/ChatApp/Prefabs/ChatButton"), contactGrid.transform);
                newButton.OnClick.AddListener(delegate { OpenMessageScreen(c); });

                newButton.GetComponent<UIContacts>().SetChat(c);
                newButton.name = c.name;

                newButton.GetComponent<UIContacts>().lastMessage = c.LastMessage();

                contactGrid.cells.Add(newButton);

                chatButtons.Add(newButton);
            }
            contactGrid.ResetGrid();
        }

        // Add a new message to the persistent data folder
        public static void SaveMessage(Message m) {
            string path = Application.persistentDataPath + "/Resources/ChatApp/Messages/" + m.chat.name;
            if (!System.IO.Directory.Exists(path)) {
                System.IO.Directory.CreateDirectory(path);
            }

            int count = System.IO.Directory.GetFiles(path).Length;
            string countName = "";
            if (count < 10) countName = "000" + count;
            else if (count < 100) countName = "00" + count;
            else if (count < 1000) countName = "0" + count;

            SOSaver.Save(path  + "/ " + m.chat.name + "_" + countName + ".dat", m);
        }

        // Store new dialogue options
        public static void AddDialogueOption(MessageOption option) {
            string path = Application.persistentDataPath + "/Resources/ChatApp/Options/" + option.chat.name;
            if (!System.IO.Directory.Exists(path)) {
                System.IO.Directory.CreateDirectory(path);
            }
            int count = System.IO.Directory.GetFiles(path).Length;
            SOSaver.Save(path  + "/ " + option.chat.name + "_" + count + ".dat", option);
        }

        // Removes all dialogue options
        public static void ClearDialogueOptions(Chat chat) {
            string path = Application.persistentDataPath + "/Resources/ChatApp/Options/" + chat.name;
            if (System.IO.Directory.Exists(path)) {
                System.IO.Directory.Delete(path, true);
            }
        }

        // Sort contact list
        public void UpdateContacts (Message lastMessage)
        {
            Division buttonToUpdate = null;
            foreach (Division button in chatButtons)
            {
                if (button.name == lastMessage.chat.name)
                {
                    buttonToUpdate = button;
                }
            }

            // Update last message
            buttonToUpdate.GetComponent<UIContacts>().lastMessage = lastMessage;
            
            // Sort Contacts

            if (buttonToUpdate)
            {
                chatButtons.Remove(buttonToUpdate);
                chatButtons.Insert(0, buttonToUpdate);

                chats.Remove(lastMessage.chat);
                chats.Insert(0, lastMessage.chat);

                contactGrid.cells = chatButtons;
                contactGrid.ResetGrid();
            }
        }

        public void UpdateLastMessages()
        {
            foreach (Division button in chatButtons)
            {
                UIContacts uic = button.GetComponent<UIContacts>();
                uic.lastMessage = uic.chat.LastMessage();
            }
        }
    }
}
