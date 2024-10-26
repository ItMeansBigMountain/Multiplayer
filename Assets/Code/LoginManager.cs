using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System.Collections;

public class LoginManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button joinButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    private void Start()
    {
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void OnJoinButtonClicked()
    {
        string username = usernameInputField.text;

        if (username.Length > 1)
        {
            PhotonNetwork.NickName = username;
            PhotonNetwork.ConnectUsingSettings();
            StartTypingFeedback("Connecting...");
        }
        else
        {
            StartTypingFeedback("Please enter a valid username.");
            Debug.LogWarning("Username is empty! Please enter a valid username.");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        StartTypingFeedback("Connected to Master Server. Joining Lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
        StartTypingFeedback("Joined Lobby! Looking for a room...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No rooms available, creating a new room...");
        StartTypingFeedback("No rooms available, creating a new room...");
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 10 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined a room!");
        StartTypingFeedback("Joined room! Loading game...");
        PhotonNetwork.LoadLevel("top-down-Multiplayer");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected from Photon with reason {0}", cause);
        StartTypingFeedback($"Disconnected: {cause}");
    }

    // Coroutine for the typing effect
    private IEnumerator TypeText(string message)
    {
        feedbackText.text = ""; // Clear current text

        foreach (char letter in message)
        {
            feedbackText.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(0.05f); // Adjust speed as desired
        }
    }

    // Method to start typing feedback
    private void StartTypingFeedback(string message)
    {
        StopAllCoroutines(); // Stop any ongoing typing effect
        StartCoroutine(TypeText(message)); // Start the new typing effect
    }
}
