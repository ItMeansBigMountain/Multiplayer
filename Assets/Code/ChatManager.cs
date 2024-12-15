using Photon.Chat;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // New Input System namespace

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    public TMP_InputField chatInput;
    public TextMeshProUGUI chatDisplay;
    public Button sendButton; // Add a UI button for mobile users

    private bool isTyping = false; // Track if the player is currently typing

    void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(PhotonNetwork.NickName));

        // Add listener for the send button
        sendButton.onClick.AddListener(SendMessage);

        // Listen to "Enter" key while typing
        chatInput.onEndEdit.AddListener(OnInputEndEdit);
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service(); // Ensure the client processes incoming messages
        }

        // Detect "Enter" key for toggling typing or sending message (PC only)
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                HandleEnterPress();
            }
        }
    }

    private void HandleEnterPress()
    {
        if (isTyping)
        {
            // If typing, send the message or stop typing if the input is empty
            if (!string.IsNullOrWhiteSpace(chatInput.text))
            {
                SendMessage();
            }
            else
            {
                StopTyping();
            }
        }
        else
        {
            // If not typing, start typing
            StartTyping();
        }
    }

    private void StartTyping()
    {
        isTyping = true;
        chatInput.ActivateInputField(); // Focus the input field
    }

    private void StopTyping()
    {
        isTyping = false;
        chatInput.text = ""; // Clear the input field
        chatInput.DeactivateInputField(); // Unfocus the input field
    }

    public void SendMessage()
    {
        string message = chatInput.text;
        if (!string.IsNullOrWhiteSpace(message))
        {
            chatClient.PublishMessage("GameRoom", $"{PhotonNetwork.NickName}: {message}");
            chatInput.text = ""; // Clear the input field after sending
        }

        StopTyping(); // Automatically stop typing after sending
    }

    private void OnInputEndEdit(string input)
    {
        // If "Enter" key is pressed while editing, send the message
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            HandleEnterPress();
        }
    }

    public void OnConnected()
    {
        chatClient.Subscribe("GameRoom"); // Join the chat channel
        Debug.Log("Connected to chat.");
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected from chat.");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            chatDisplay.text += $"\n{senders[i]}: {messages[i]}";
        }
    }

    // Interface methods that are not used but must be implemented
    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        Debug.Log($"[Chat Debug] {level}: {message}");
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log($"Private message from {sender}: {message}");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log($"Subscribed to channels: {string.Join(", ", channels)}");
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log($"Unsubscribed from channels: {string.Join(", ", channels)}");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"Status update from {user}: {status}");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"User {user} subscribed to channel {channel}");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"User {user} unsubscribed from channel {channel}");
    }

    public void OnChatStateChange(ChatState state) { }
}
