using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomHandler : MonoBehaviour
{
    public GameObject roomItem;
    public List<RoomInfo> rooms;
    public Button signOut;
    public Button createRoom;
    public Button quickJoin;
    public Button clearText;
    public Button searchRoom;
    public Button refresh;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ServiceClient.CheckIn(OnCheckInFinish));
        signOut.onClick.AddListener(OnSignOut);
        createRoom.onClick.AddListener(OnCreateRoom);
        quickJoin.onClick.AddListener(OnQuickJoin);
        clearText.onClick.AddListener(OnClearText);
        searchRoom.onClick.AddListener(OnSearchRoom);
        refresh.onClick.AddListener(OnRefresh);
    }

    private void OnApplicationQuit()
    {
        signOut.onClick.RemoveAllListeners();
        createRoom.onClick.RemoveAllListeners();
        quickJoin.onClick.RemoveAllListeners();
        clearText.onClick.RemoveAllListeners();
        searchRoom.onClick.RemoveAllListeners();
        refresh.onClick.RemoveAllListeners();
    }

    void OnCheckInFinish(bool hasError, long reward)
    {
        if(hasError == true)
        {
            return;
        }
        // TODO: Inform reward
        StartCoroutine(ServiceClient.GetUserInfo(OnFinish));
        StartCoroutine(ServiceClient.GetRooms(OnRoomFound));
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

    public void OnRoomChoose(int room)
    {
        GameData.currentRoom = rooms[room];
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void OnSignOut()
    {
        PlayerPrefHandler.Delete(PlayerPrefHandler.TOKEN);
        SceneManager.LoadSceneAsync("LoginScene");
    }

    public void OnCreateRoom() {
        // Show create room dialog
    }

    public void OnQuickJoin() {
        // Start coroutine quick find room
        StartCoroutine(ServiceClient.QuickJoin(OnRoomFound))
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

    public void OnRefresh() {
        StartCoroutine(ServiceClient.GetRooms(OnRoomFound));
    }
}
