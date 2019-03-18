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

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ServiceClient.CheckIn(OnCheckInFinish));
        
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
        rooms = infos;
        GameObject roomList = GameObject.Find("RoomLayout");
        for (int i = 0; i < infos.Count; i++)
        {
            GameObject obj = Instantiate(roomItem);
            obj.name = i.ToString();
            obj.transform.SetParent(roomList.transform, false);
            RoomItemHandler handler = obj.GetComponent<RoomItemHandler>();
            handler.roomInfo = infos[i];
        }
    }

    public void OnRoomChoose(int room)
    {
        GameData.currentRoom = rooms[room];
        SceneManager.LoadSceneAsync("GameScene");
    }
}
