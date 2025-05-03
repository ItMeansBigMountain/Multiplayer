using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LoginManager : MonoBehaviourPunCallbacks
{
    [Header("Login")]
    public TMP_InputField usernameInputField;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("Room Browser")]
    public GameObject roomListItemPrefab;
    public Transform roomListContainer;
    public Button NickNameCheckInButton;

    [Header("Map Selection")]
    public TMP_Dropdown mapDropdown;

    [Header("Create Room")]
    public TMP_InputField createRoomInput;
    public Button createRoomButton;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    void Start()
    {

        createRoomButton.onClick.AddListener(CreateRoom);
        NickNameCheckInButton.onClick.AddListener(RefreshRooms);
    }

    IEnumerator ConnectAfterDisconnect()
    {
        while (PhotonNetwork.IsConnected)
            yield return null;

        PhotonNetwork.ConnectUsingSettings();
        Log("Reconnecting to Photon...");
    }

    void RefreshRooms()
    {
        string username = usernameInputField.text;

        if (string.IsNullOrEmpty(username))
            username = "Player" + Random.Range(1000, 9999); // fallback nickname

        PhotonNetwork.NickName = username;

        if (username.EndsWith(".tzbd"))
            Log("ADMIN PRIV UNLOCKED");

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // restart connection fully
            StartCoroutine(ConnectAfterDisconnect());
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            Log($"Connecting as {username}...");
        }
    }


    void OnConnectClicked()
    {
        string username = usernameInputField.text;
        if (!string.IsNullOrEmpty(username))
        {
            PhotonNetwork.NickName = username;
            PhotonNetwork.ConnectUsingSettings();
            Log("Connecting...");
        }
        else Log("Please enter a nickname.");
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to Network Vendor.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Log("Connected to Application");
        ClearRoomList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
                cachedRoomList.Remove(room.Name);
            else
                cachedRoomList[room.Name] = room;
        }

        UpdateRoomListUI();
    }

    void UpdateRoomListUI()
    {
        ClearRoomList();

        foreach (var room in cachedRoomList.Values)
        {
            GameObject item = Instantiate(roomListItemPrefab, roomListContainer);
            var text = item.GetComponentInChildren<TextMeshProUGUI>();
            var button = item.GetComponentInChildren<Button>();

            text.text = $"{room.Name} ({room.PlayerCount}/{room.MaxPlayers})";
            button.onClick.AddListener(() =>
            {
                Log($"Joining room: {room.Name}");
                PhotonNetwork.JoinRoom(room.Name);
            });
        }
    }

    void ClearRoomList()
    {
        foreach (Transform child in roomListContainer)
            Destroy(child.gameObject);
    }

    string GetSelectedMapName()
    {
        switch (mapDropdown.value)
        {
            case 0: return "top-down-Multiplayer-Arena";
            case 1: return "top-down-Multiplayer-Spiral";
            case 2: return "top-down-Multiplayer-Gorilla";
            default: return "top-down-Multiplayer-Arena";
        }
    }


    void CreateRoom()
    {
        string roomName = createRoomInput.text;
        if (string.IsNullOrEmpty(roomName))
        {
            Log("Room name cannot be empty.");
            return;
        }

        string selectedMap = GetSelectedMapName();

        RoomOptions options = new RoomOptions { MaxPlayers = 100, IsVisible = true, IsOpen = true };
        PhotonNetwork.CreateRoom(roomName, options);
        Log($"Creating room '{roomName}' with map: {selectedMap}");

        // Save selected map to use on join
        PlayerPrefs.SetString("selectedMap", selectedMap);
    }



    public override void OnJoinedRoom()
    {
        string selectedMap = PlayerPrefs.GetString("selectedMap", "top-down-Multiplayer-Arena");
        Log($"Room joined. Loading map: {selectedMap}...");
        PhotonNetwork.LoadLevel(selectedMap);
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Log($"Disconnected: {cause}");
    }

    void Log(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(msg));
    }

    IEnumerator TypeText(string msg)
    {
        feedbackText.text = "";
        foreach (char c in msg)
        {
            feedbackText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }
}
