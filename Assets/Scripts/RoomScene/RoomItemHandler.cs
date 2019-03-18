using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemHandler : MonoBehaviour
{
    public RoomInfo roomInfo;
    public GameObject playerItem;
    public Text roomId;
    public Text roomName;
    public Text roomAmount;
    public Button button;
    public GameObject playerList;
    public Sprite[] statusSprite;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnApplicationQuit()
    {
        button.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        if (roomInfo != null)
        {
            roomId.text = roomInfo.roomid;
            roomName.text = "Ai ma biet";
            roomAmount.text = roomInfo.amount.ToString();
            int i = 0;
            while (i < roomInfo.maxplayer)
            {
                GameObject obj = Instantiate(playerItem);
                obj.name = i.ToString();
                obj.transform.SetParent(playerList.transform, false);
                Image image = obj.GetComponent<Image>();
                if (i < roomInfo.numplayer)
                {
                    image.sprite = statusSprite[0];
                }
                else
                {
                    image.sprite = statusSprite[1];   
                }
                i++;
            }
        }
    }

    void OnClick()
    {
        Debug.Log("Click" + this.name);
        int index = int.Parse(this.name);
        RoomHandler handler = FindObjectOfType<RoomHandler>();
        handler.OnRoomChoose(index);
    }
}
