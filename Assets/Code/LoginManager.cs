using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LoginManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button joinButton;

    private void Start()
    {
        // Add a listener to the join button to handle the login
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void OnJoinButtonClicked()
    {
        string username = usernameInputField.text;

        if (username.Length > 1)
        {
            PhotonNetwork.NickName = username; // Set player's nickname
            PhotonNetwork.ConnectUsingSettings(); // Connect to Photon server
        }
        else
        {
            Debug.LogWarning("Username is empty! Please enter a valid username.");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No rooms available, creating a new room...");
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 10 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined a room!");
        PhotonNetwork.LoadLevel("top-down-Multiplayer"); 
    }
}
