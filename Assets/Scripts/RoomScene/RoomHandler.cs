using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomHandler : MonoBehaviour
{
    public GameObject roomItem;
    public GameObject createRoomDialog;
    public GameObject noMoneyDialog;
    public GameObject rewardDialog;
    public List<RoomInfo> rooms;
    public Button signOut;
    public Button createRoom;
    public Button quickJoin;
    public Button clearText;
    public Button searchRoom;
    public Button refresh;
    public Button closeCreateRoom;
    public Button closeInform;
    public Button submitCreateRoom;
    public Button closeReward;

    public static Dictionary<long, List<long>> listAmount = new Dictionary<long, List<long>> {
        {30000, new List<long> {3000, 5000, 10000}},
        {80000, new List<long> {5000, 10000, 20000, 50000}},
        {800000, new List<long> {100000, 200000, 500000}},
        {30000000, new List<long> {1000000, 2000000, 5000000, 10000000}},
        {300000000, new List<long> {5000000, 10000000, 20000000, 50000000, 100000000, 200000000}}
    };

    // Start is called before the first frame update
    void Start()
    {
        createRoomDialog.SetActive(false);
        noMoneyDialog.SetActive(false);
        rewardDialog.SetActive(false);
        signOut.onClick.AddListener(OnSignOut);
        createRoom.onClick.AddListener(OnCreateRoom);
        quickJoin.onClick.AddListener(OnQuickJoin);
        clearText.onClick.AddListener(OnClearText);
        searchRoom.onClick.AddListener(OnSearchRoom);
        refresh.onClick.AddListener(OnRefresh);
        closeInform.onClick.AddListener(() => { noMoneyDialog.SetActive(false); });
        closeCreateRoom.onClick.AddListener(() => { createRoomDialog.SetActive(false); });
        closeReward.onClick.AddListener(() => { rewardDialog.SetActive(false); });
        submitCreateRoom.onClick.AddListener(OnSubmitRoom);
        if (GameData.currentPlayer == null)
        {
            StartCoroutine(ServiceClient.CheckIn(OnCheckInFinish));
            StartCoroutine(ServiceClient.GetUserInfo(OnFinish));
            StartCoroutine(ServiceClient.GetRooms(OnRoomFound));
        }
        else
        {
            ShowUserInfo();
            StartCoroutine(ServiceClient.GetRooms(OnRoomFound));
        }
    }

    private void OnDisable()
    {
        signOut.onClick.RemoveAllListeners();
        createRoom.onClick.RemoveAllListeners();
        quickJoin.onClick.RemoveAllListeners();
        clearText.onClick.RemoveAllListeners();
        searchRoom.onClick.RemoveAllListeners();
        refresh.onClick.RemoveAllListeners();
        closeInform.onClick.RemoveAllListeners();
        closeCreateRoom.onClick.RemoveAllListeners();
        submitCreateRoom.onClick.RemoveAllListeners();
    }

    private void OnApplicationQuit()
    {
        signOut.onClick.RemoveAllListeners();
        createRoom.onClick.RemoveAllListeners();
        quickJoin.onClick.RemoveAllListeners();
        clearText.onClick.RemoveAllListeners();
        searchRoom.onClick.RemoveAllListeners();
        refresh.onClick.RemoveAllListeners();
        closeInform.onClick.RemoveAllListeners();
        closeCreateRoom.onClick.RemoveAllListeners();
        submitCreateRoom.onClick.RemoveAllListeners();
    }

    void OnCheckInFinish(bool hasError, long reward)
    {
        if(hasError == true || reward == 0)
        {
            return;
        }
        // TODO: Inform reward
        rewardDialog.SetActive(true);
        var amount = rewardDialog.transform.Find("Amount").gameObject;
        if (amount != null)
        {
            Text rewardText = amount.GetComponent<Text>();
            rewardText.text = Converter.ConvertToMoneyBasic(reward);
        }
    }

    void OnFinish(PlayerInfo info)
    {
        if (info == null)
        {
            return;
        }
        GameData.currentPlayer = info;
        Debug.Log("Callback");
        Text username = GameObject.Find("Username").GetComponent<Text>();
        username.text = info.userName;
        Text amount = GameObject.Find("Amount").GetComponent<Text>();
        amount.text = Converter.ConvertToMoney(info.amount);
        if (info.image == "")
        {
            info.image = "https://i.pinimg.com/originals/9e/1d/d6/9e1dd6458c89b03c506b384f537423d9.jpg";
        }
        StartCoroutine(ServiceClient.GetImageTexture(info.image, OnImageLoaded));
    }

    void ShowUserInfo()
    {
        Text username = GameObject.Find("Username").GetComponent<Text>();
        username.text = GameData.currentPlayer.userName;
        Text amount = GameObject.Find("Amount").GetComponent<Text>();
        amount.text = Converter.ConvertToMoney(GameData.currentPlayer.amount);
        RawImage userImage = GameObject.Find("UserImage").GetComponent<RawImage>();
        userImage.texture = GameData.currentPlayerImage;
    }

    void OnImageLoaded(Texture2D image)
    {
        if (image == null)
        {
            return;
        }
        Debug.Log("Image loaded");
        RawImage userImage = GameObject.Find("UserImage").GetComponent<RawImage>();
        userImage.texture = image;
        GameData.currentPlayerImage = image;
    }

    void OnRoomFound(List<RoomInfo> infos)
    {
        if (infos == null)
        {
            //Error happen, do something
            return;
        }
        rooms = infos;
        GameObject roomList = GameObject.Find("RoomLayout");
        int childCount = roomList.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(roomList.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < infos.Count; i++)
        {
            GameObject obj = Instantiate(roomItem);
            obj.name = infos[i].roomid;
            obj.transform.SetParent(roomList.transform, false);
            RoomItemHandler handler = obj.GetComponent<RoomItemHandler>();
            handler.roomInfo = infos[i];
        }
    }

    void OnRoomFound(RoomInfo room)
    {
        if (room == null)
        {
            //Error happen, do something
            return;
        }
        GameData.currentRoom = room;
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void OnRoomChoose(string roomid)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].roomid == roomid)
            {
                GameData.currentRoom = rooms[i];
                break;
            }
        }
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void OnSignOut()
    {
        PlayerPrefHandler.Delete(PlayerPrefHandler.TOKEN);
        SceneManager.LoadSceneAsync("LoginScene");
    }

    public void OnCreateRoom() {
        // Show create room dialog
        long amount = 0;
        foreach (var item in listAmount)
        {
            if(item.Key <= GameData.currentPlayer.amount)
            {
                amount = item.Key;
            }
            else
            {
                break;
            }
        }
        if (amount == 0)
        {
            // Show Dialog that user don't have enough money to create room
            noMoneyDialog.SetActive(true);
        }
        else
        {
            // Create Dialog
            createRoomDialog.SetActive(true);
            CreateRoomConfig config = createRoomDialog.GetComponent<CreateRoomConfig>();
            // Set Dialog Slider with listAmount[amount]
            config.SetSliderValue(listAmount[amount]);
        }
    }

    public void OnQuickJoin() {
        // Start coroutine quick find room
        StartCoroutine(ServiceClient.QuickJoin(OnRoomFound));
    }

    public void OnClearText()
    {
        InputField input = GameObject.Find("Search").GetComponent<InputField>();
        input.text = "";
    }

    public void OnSearchRoom()
    {
        InputField input = GameObject.Find("Search").GetComponent<InputField>();
        if (input.text == "")
        {
            return;
        }
        else
        {
            GameObject roomList = GameObject.Find("RoomLayout");
            int childCount = roomList.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                if (roomList.transform.GetChild(i).name != input.text)
                {
                    Destroy(roomList.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    public void OnSubmitRoom()
    {
        createRoomDialog.SetActive(false);
        long amount = 0;
        int number = 0;
        CreateRoomConfig config = createRoomDialog.GetComponent<CreateRoomConfig>();
        config.GetConfig(ref amount, ref number);
        StartCoroutine(ServiceClient.CreateRoom(amount, number, OnRoomFound));
    }

    public void OnRefresh() {
        StartCoroutine(ServiceClient.GetRooms(OnRoomFound));
    }
}
