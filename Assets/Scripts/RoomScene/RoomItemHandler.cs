using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemHandler : MonoBehaviour
{
    public RoomInfo roomInfo
    {
        set
        {
            roomId.text = value.roomid;
            roomName.text = "Ai ma biet";
            roomAmount.text = Converter.ConvertToMoney(value.amount);
            int i = 0;
            while (i < value.maxplayer)
            {
                GameObject obj = Instantiate(playerItem);
                obj.name = i.ToString();
                obj.transform.SetParent(playerList.transform, false);
                Image image = obj.GetComponent<Image>();
                if (i < value.numplayer)
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
    }

    void OnClick()
    {
        Debug.Log("Click" + this.name);
        RoomHandler handler = FindObjectOfType<RoomHandler>();
        handler.OnRoomChoose(this.name);
    }
}
