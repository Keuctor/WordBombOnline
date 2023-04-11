using System;
using System.Collections;
using ilasm.WordBomb.Initialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ilasm.WordBomb.Chat
{
    public class ChatController : MonoBehaviour
    {
        public GameObject ChatObject;
        public ChatMessageView MessageViewTemplate;
        public ChatMessageView LocalMessageViewTemplate;
        public Transform MessageContent;


        public TMP_InputField Input;
        public ScrollRect ScrollRect;

        public GameObject WarningObjectTemplate;


        public TMP_Text ChatNewMessageCount;
        public GameObject NewMessagesCircle;

        private int newMessages;
        private bool cantSend;
        private GameObject warningObject;

        private ChatManager chatManager;

        private void Start()
        {
            chatManager = FindObjectOfType<ChatManager>();
        }

        public void Toggle()
        {
            ChatObject.gameObject.SetActive(!ChatObject.activeSelf);
            if (ChatObject.activeSelf)
            {
                if (cantSend)
                {
                    if (warningObject != null)
                    {
                        Destroy(warningObject);
                    }
                    cantSend = false;
                }
                PopupManager.Active = true;
                newMessages = 0;
                NewMessagesCircle.gameObject.SetActive(false);
                Input.ActivateInputField();
                Input.onValueChanged.AddListener(OnTextValueChanged);
            }
            else
            {
                PopupManager.Active = false;
                Input.onValueChanged.RemoveAllListeners();
            }
        }

        private void OnTextValueChanged(string text)
        {
            SoundManager.PlayAudio(Sounds.Typing, false, 0.03f);
        }

        public void SendMessage()
        {
            if (string.IsNullOrEmpty(Input.text))
                return;

            if (cantSend)
            {
                if (warningObject == null)
                {
                    warningObject = Instantiate(WarningObjectTemplate, MessageContent);
                    Canvas.ForceUpdateCanvases();
                    ScrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
                    ScrollRect.content.GetComponent<ContentSizeFitter>().SetLayoutVertical();
                    ScrollRect.verticalNormalizedPosition = 0;
                }
                return;
            }

            chatManager.SendChatMessage(GameSetup.LocalPlayerId, Input.text);
            Input.text = "";
            Input.ForceLabelUpdate();
            Input.ActivateInputField();
            StartCoroutine(TimeoutUser());
        }

        public IEnumerator TimeoutUser()
        {
            cantSend = true;
            yield return new WaitForSecondsRealtime(2);
            cantSend = false;
            if (warningObject != null)
            {
                Destroy(warningObject);
                warningObject = null;
            }
        }

        private void Update()
        {
            if (ChatObject.activeSelf)
            {
                if (Input.IsActive())
                {
                    if (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter))
                    {
                        SendMessage();
                    }
                }

                if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                    Toggle();
            }
        }

        public void OnReceivedMessage(int playerId, string message)
        {
            var isValid = UserValidator.CheckPlayerMessageIsValid(message, out string validatedMessage);
            if (!isValid)
                return;

            var player = MatchmakingService.CurrentRoom.Players.Find(p => p.Id == playerId);
            ChatMessageView createView;
            if (playerId == GameSetup.LocalPlayerId)
            {
                createView = LocalMessageViewTemplate;
            }
            else
            {
                createView = MessageViewTemplate;
            }

            var view = Instantiate(createView, MessageContent);
            view.Icon.sprite = AvatarManager.GetAvatar(player.AvatarId);
            view.NameText.text = player.UserName;
            view.Text.text = validatedMessage;
            if (MessageContent.transform.childCount > 10)
            {
                Destroy(MessageContent.transform.GetChild(0).gameObject);
            }

            Canvas.ForceUpdateCanvases();
            ScrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            ScrollRect.content.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            ScrollRect.verticalNormalizedPosition = 0;
            if (!ChatObject.activeSelf)
            {
                newMessages++;
                this.ChatNewMessageCount.text = newMessages.ToString();
                NewMessagesCircle.gameObject.SetActive(true);
                SoundManager.PlayAudio(Sounds.NewMessage, false, 0.07f);
            }
        }
    }
}