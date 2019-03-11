using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Catte : MonoBehaviour
{
    private const int MAXPLAYER = 6;
    private const int MAXCARD = 6;

    // Replace with real data later
    private string userId = "1111";
    private string roomId = "1";
    private PlayerInfo playerInfo = new PlayerInfo();
    private bool inGame = false;
    private int row = -1;
    private int turn = -1;
    private string lastTopCard = "";

    public Sprite[] cards;
    public GameObject playerCard;
    public GameObject userInfo;
    public GameObject playerView;
    public GameObject[] playView;
    public GameObject[] otherCard;
    public GameObject[] playerInfos;
    public Button startButton;
    public Button foldButton;
    public Button playButton;
    public Canvas canvas;

    private Player player;
    private List<Player> otherPlayers;

    private List<int> usedIndex = new List<int>();
    public Dictionary<string, int> valueMap = new Dictionary<string, int>() {
        {"2", 2 },
        {"3", 3 },
        {"4", 4 },
        {"5", 5 },
        {"6", 6 },
        {"7", 7 },
        {"8", 8 },
        {"9", 9 },
        {"10", 10 },
        {"J", 11 },
        {"Q", 12 },
        {"K", 13 },
        {"A", 14 }
    };
    
    // Start is called before the first frame update
    void Awake()
    {
        
        playerInfo.userId = userId;
        playerInfo.userName = System.Guid.NewGuid().ToString();
        playerInfo.image = "";
        playerInfo.amount = 1000000;
        player = null;
        otherPlayers = new List<Player>();
        GameClient.Init();
        GameClient.OnConnectEvent += OnConnect;
        MessageHandler.Init(roomId, userId);
        MessageHandler.OnJoinEvent += OnJoin;
        MessageHandler.OnNewPlayerEvent += OnNewPlayer;
        MessageHandler.OnLeaveEvent += OnLeave;
        MessageHandler.OnCardsEvent += OnCards;
        MessageHandler.OnPlayEvent += OnPlay;
        MessageHandler.OnStartEvent += OnStart;
        MessageHandler.OnEliminatedEvent += OnEliminated;
        MessageHandler.OnResultEvent += OnResultEvent;
        startButton.gameObject.SetActive(false);
        startButton.onClick.AddListener(StartGame);
        playButton.gameObject.SetActive(false);
        playButton.onClick.AddListener(PlayCard);
        foldButton.gameObject.SetActive(false);
        foldButton.onClick.AddListener(FoldCard);
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MessageHandler.ProcessMessage();
    }

    public void OnApplicationQuit()
    {
        GameClient.OnConnectEvent -= OnConnect;
        MessageHandler.OnNewPlayerEvent -= OnNewPlayer;
        MessageHandler.OnLeaveEvent -= OnLeave;
        MessageHandler.OnJoinEvent -= OnJoin;
        MessageHandler.OnCardsEvent -= OnCards;
        MessageHandler.OnPlayEvent -= OnPlay;
        MessageHandler.OnStartEvent -= OnStart;
        MessageHandler.OnEliminatedEvent -= OnEliminated;
        MessageHandler.OnResultEvent -= OnResultEvent;
        GameClient.Disconnect();

        startButton.onClick.RemoveAllListeners();
        playButton.gameObject.SetActive(false);
        playButton.onClick.RemoveAllListeners();
        foldButton.onClick.RemoveAllListeners();
        foldButton.gameObject.SetActive(false);
    }

    public void RenderCards() {
        float xOffset = -1.3f * (player.cards.Count - 1) / 2;
        for (int i = 0; i < player.cards.Count; i++)
        {
            GameObject newCard = Instantiate(playerCard);
            newCard.transform.position = new Vector3(playerView.transform.position.x + xOffset, playerView.transform.position.y, playerView.transform.position.z);
            xOffset += 1.5f;
            Selectable selectable = newCard.GetComponent<Selectable>();
            selectable.faceup = true;
            selectable.lost = false;
            newCard.name = player.cards[i];
        }
        for (int i = 0; i < otherPlayers.Count; i++)
        {
            GameObject newCard = Instantiate(playerCard);
            newCard.transform.position = new Vector3(otherCard[otherPlayers[i].mappedIndex].transform.position.x, otherCard[otherPlayers[i].mappedIndex].transform.position.y, otherCard[otherPlayers[i].mappedIndex].transform.position.z);
            newCard.transform.localScale = new Vector3(newCard.transform.localScale.x * 0.6f, newCard.transform.localScale.y * 0.6f, newCard.transform.localScale.z);
        }
    }

    public void RenderPlays(PlayData p) {  
        if (p.index == player.index)
        {
            player.cards.Remove(p.card);
            player.numCard--;
            if (p.row < 4)
            {
                GameObject obj = GameObject.Find(p.card);
                Selectable selectable = obj.GetComponent<Selectable>();
                if (p.action == MessageHandler.PLAY)
                {
                    selectable.faceup = true;
                    if (lastTopCard != "")
                    {
                        GameObject last = GameObject.Find(lastTopCard);
                        Selectable sel = last.GetComponent<Selectable>();
                        sel.lost = true;
                    }
                    lastTopCard = p.card;
                }
                else
                {
                    selectable.faceup = false;
                    selectable.lost = true;
                }
                float xOffset = -0.7f + (0.4f * (5 - player.numCard));
                float zOffset = 0.1f + (0.1f * (5 - player.numCard));
                selectable.sortingOrder = 5 - player.numCard;
                selectable.targetPos = new Vector3(playView[0].transform.position.x + xOffset, playView[0].transform.position.y, playView[0].transform.position.z + zOffset);
                Debug.Log("XXXXX" + selectable.targetPos.ToString());
                selectable.targetScale = new Vector3(obj.transform.localScale.x * 0.8f, obj.transform.localScale.y * 0.8f, obj.transform.localScale.z);
                xOffset = -1.3f * (player.cards.Count - 1) / 2;
                for (int i = 0; i < player.cards.Count; i++)
                {
                    GameObject newCard = GameObject.Find(player.cards[i]);
                    selectable = newCard.GetComponent<Selectable>();
                    selectable.targetPos = new Vector3(playerView.transform.position.x + xOffset, playerView.transform.position.y, playerView.transform.position.z);
                    xOffset += 1.5f;
                }
            }
            if (p.row == 4)
            {
                GameObject obj = GameObject.Find(p.card);
                Selectable selectable = obj.GetComponent<Selectable>();
                selectable.targetPos = new Vector3(playerView.transform.position.x, playerView.transform.position.y, playerView.transform.position.z + 0.5f);
                selectable.sortingOrder = 5 - player.numCard;
                if (p.action == MessageHandler.PLAY)
                {
                    selectable.faceup = true;
                    if (lastTopCard != "")
                    {
                        GameObject last = GameObject.Find(lastTopCard);
                        Selectable sel = last.GetComponent<Selectable>();
                        sel.lost = true;
                    }
                    lastTopCard = p.card;
                }
                else
                {
                    selectable.faceup = true;
                    selectable.lost = true;
                }
                GameObject other = GameObject.Find(player.cards[0]);
                selectable = other.GetComponent<Selectable>();
                selectable.sortingOrder = 5 - player.numCard;
                selectable.targetPos = new Vector3(playerView.transform.position.x, playerView.transform.position.y, playerView.transform.position.z + 1.0f);
            }
            if (p.row == 5)
            {
                GameObject obj = GameObject.Find(p.card);
                Selectable selectable = obj.GetComponent<Selectable>();
                if (p.action == MessageHandler.PLAY)
                {
                    selectable.faceup = true;
                    if (lastTopCard != "")
                    {
                        GameObject last = GameObject.Find(lastTopCard);
                        Selectable sel = last.GetComponent<Selectable>();
                        sel.lost = true;
                    }
                    lastTopCard = p.card;
                }
                else
                {
                    selectable.faceup = true;
                    selectable.lost = true;
                }
                selectable.targetPos = new Vector3(playerView.transform.position.x - 0.8f, playerView.transform.position.y, playerView.transform.position.z + 0.5f);
            }
        }
        else
        {
            foreach (var player in otherPlayers) { 
                if (p.index == player.index) {
                    player.numCard--;
                    int index = player.mappedIndex;
                    GameObject newCard = Instantiate(playerCard);
                    newCard.name = p.card;
                    if (p.row < 4)
                    {
                        newCard.transform.position = new Vector3(otherCard[index].transform.position.x, otherCard[index].transform.position.y, otherCard[index].transform.position.z);
                        newCard.transform.localScale = new Vector3(newCard.transform.localScale.x * 0.6f, newCard.transform.localScale.y * 0.6f, newCard.transform.localScale.z);
                        Selectable selectable = newCard.GetComponent<Selectable>();
                        if (p.action == MessageHandler.PLAY)
                        {
                            selectable.lost = false;
                            selectable.faceup = true;
                            if (lastTopCard != "")
                            {
                                GameObject last = GameObject.Find(lastTopCard);
                                Selectable sel = last.GetComponent<Selectable>();
                                sel.lost = true;
                            }
                            lastTopCard = p.card;
                        }
                        else
                        {
                            selectable.lost = true;
                            selectable.faceup = false;
                        }
                        float xOffset = -0.7f + (0.4f * (5 - player.numCard));
                        float zOffset = 0.1f + (0.1f * (5 - player.numCard));
                        selectable.sortingOrder = 5 - player.numCard;
                        selectable.targetPos = new Vector3(playView[index].transform.position.x + xOffset, playView[index].transform.position.y, playView[index].transform.position.z + zOffset);
                        Debug.Log("XXXXX" + selectable.targetPos);
                        selectable.targetScale = new Vector3(newCard.transform.localScale.x * 1.34f, newCard.transform.localScale.y * 1.34f, newCard.transform.localScale.z);
                    }
                    if (p.row == 4)
                    {
                        newCard.transform.position = new Vector3(otherCard[index].transform.position.x, otherCard[index].transform.position.y, otherCard[index].transform.position.z + 1.0f);
                        newCard.transform.localScale = new Vector3(newCard.transform.localScale.x * 0.8f, newCard.transform.localScale.y * 0.8f, newCard.transform.localScale.z);
                        Selectable selectable = newCard.GetComponent<Selectable>();
                        if (p.action == MessageHandler.PLAY)
                        {
                            selectable.lost = false;
                            selectable.faceup = true;
                            if (lastTopCard != "")
                            {
                                GameObject last = GameObject.Find(lastTopCard);
                                Selectable sel = last.GetComponent<Selectable>();
                                sel.lost = true;
                            }
                        }
                        else
                        {
                            selectable.lost = true;
                            selectable.faceup = true;
                        }
                        selectable.targetPos = new Vector3(playView[index].transform.position.x - 2.0f, playView[index].transform.position.y, playView[index].transform.position.z + 1.0f);
                    }
                    if (p.row == 5)
                    {
                        newCard.transform.position = new Vector3(playView[index].transform.position.x - 1.0f, playView[index].transform.position.y, playView[index].transform.position.z + 0.5f);
                        newCard.transform.localScale = new Vector3(newCard.transform.localScale.x * 0.8f, newCard.transform.localScale.y * 0.8f, newCard.transform.localScale.z);
                        Selectable selectable = newCard.GetComponent<Selectable>();
                        if (p.action == MessageHandler.PLAY)
                        {
                            selectable.lost = false;
                            selectable.faceup = true;
                            if (lastTopCard != "")
                            {
                                GameObject last = GameObject.Find(lastTopCard);
                                Selectable sel = last.GetComponent<Selectable>();
                                sel.lost = true;
                            }
                        }
                        else
                        {
                            selectable.lost = true;
                            selectable.faceup = true;
                        }
                        selectable.targetPos = new Vector3(playView[index].transform.position.x - 1.3f, playView[index].transform.position.y, playView[index].transform.position.z + 0.5f);
                    }
                }
            }
        }
        if (p.newRow)
        {
            row = p.row + 1;
            lastTopCard = "";
        }
        else
        {
            row = p.row;
        }
        turn = p.nextTurn;
        string userId = FindPlayerIdByIndex(turn);
        if (userId != "")
        {
            Debug.Log(userId);
            GameObject obj = GameObject.Find(userId);
            UserStatus status = obj.GetComponent<UserStatus>();
            status.isActive = true;
        }
    }

    void RenderNewPlayer() {
        Player newPlayer = otherPlayers[otherPlayers.Count - 1];
        GameObject obj = Instantiate(userInfo);
        obj.name = newPlayer.playerInfo.userId;
        obj.transform.SetParent(canvas.transform, false);
        obj.transform.position = new Vector3(playerInfos[newPlayer.mappedIndex].transform.position.x, playerInfos[newPlayer.mappedIndex].transform.position.y, playerInfos[newPlayer.mappedIndex].transform.position.z);
        Text[] child = obj.GetComponentsInChildren<Text>();
        for (int i = 0; i < child.Length; i++)
        {
            if (child[i].name == "Username")
            {
                child[i].text = newPlayer.playerInfo.userId;
            }
            if (child[i].name == "Amount")
            {
                child[i].text = newPlayer.playerInfo.amount.ToString();
            }
        }
    }

    void RenderPlayers() {
        GameObject obj = Instantiate(userInfo);
        obj.name = player.playerInfo.userId;
        obj.transform.SetParent(canvas.transform, false);
        obj.transform.position = new Vector3(playerInfos[0].transform.position.x, playerInfos[0].transform.position.y, playerInfos[0].transform.position.z);
        Text[] child = obj.GetComponentsInChildren<Text>();
        for (int i = 0; i < child.Length; i++)
        {
            if (child[i].name == "Username")
            {
                child[i].text = player.playerInfo.userId;
            }
            if (child[i].name == "Amount")
            {
                child[i].text = player.playerInfo.amount.ToString();
            }
        }
        

        for (int i = 0; i < otherPlayers.Count; i++)
        {
            Debug.Log("xxxxx");
            GameObject otherObj = Instantiate(userInfo);
            UserStatus stt = otherObj.GetComponent<UserStatus>();
            otherObj.name = otherPlayers[i].playerInfo.userId;
            otherObj.transform.SetParent(canvas.transform, false);
            otherObj.transform.position = new Vector3(playerInfos[otherPlayers[i].mappedIndex].transform.position.x, playerInfos[otherPlayers[i].mappedIndex].transform.position.y, playerInfos[otherPlayers[i].mappedIndex].transform.position.z);
            Text[] otherChilds = otherObj.GetComponentsInChildren<Text>();
            for (int j = 0; j < otherChilds.Length; j++)
            {
                if (otherChilds[j].name == "Username")
                {
                    otherChilds[j].text = otherPlayers[i].playerInfo.userId;
                }
                if (otherChilds[j].name == "Amount")
                {
                    otherChilds[j].text = otherPlayers[i].playerInfo.amount.ToString();
                }
            }
        }
    }

    public void StartGame() {
        startButton.gameObject.SetActive(false);
        Debug.Log("StartGame");
        MessageHandler.Deal();
    }

    public void OnConnect()
    {
        Debug.Log("Server Connected");
        MessageHandler.JoinRoom(playerInfo);
    }

    public void OnJoin(List<Player> players) {
        otherPlayers.Clear();
        foreach(var p in players) {
          
            if (p.inGame == true)
            {
                //inGame = true;
            }
            if (p.playerInfo.userId == userId)
            {
                MessageHandler.SetIndex(p.index);
                usedIndex.Add(p.index);
                Debug.Log("JOINT");
                player = p;
                player.mappedIndex = 0;
            }
            else {
                Debug.Log("Have other player");
                otherPlayers.Add(p);
            }
        }

        for (int i = 0; i < otherPlayers.Count; i++)
        {
            usedIndex.Add(otherPlayers[i].index);
            otherPlayers[i].mappedIndex = MapIndex(player.index, otherPlayers[i].index);
        }

        if (!inGame && otherPlayers.Count != 0)
        {
            startButton.gameObject.SetActive(true);
        }
        RenderPlayers();
    }

    public void OnNewPlayer(Player newPlayer) {
        newPlayer.mappedIndex = MapIndex(player.index, newPlayer.index);
        otherPlayers.Add(newPlayer);
        usedIndex.Add(newPlayer.index);
        if (!inGame && otherPlayers.Count != 0)
        {
            startButton.gameObject.SetActive(true);
        }
        RenderNewPlayer();
    }

    public void OnLeave(int index)
    {
        string userId = FindPlayerIdByIndex(index);
        GameObject obj = GameObject.Find(userId);
        GameObject.Destroy(obj);
        usedIndex.Remove(index);
    }
    
    private int MapIndex(int playerIndex, int otherPlayerIndex)
    {
        Debug.Log(playerIndex.ToString() + " " + otherPlayerIndex.ToString());
        if (otherPlayerIndex < playerIndex)
        {
            return playerIndex - otherPlayerIndex;
        }
        else
        {
            return MAXPLAYER - otherPlayerIndex;
        }
    } 

    public void OnCards(List<string> cards) {
        player.cards = cards;
        player.numCard = MAXCARD;
        foreach(var p in otherPlayers) {
            p.numCard = 6;
        }
        RenderCards();
    }

    public void OnPlay(PlayData play)
    {
        Debug.Log("Received play " + play.ToString());
        RenderPlays(play);
    }

    public void OnStart(int index) {
        row = 0;
        turn = index;
        string userId = FindPlayerIdByIndex(turn);
        if (userId != "")
        {
            GameObject obj = GameObject.Find(userId);
            UserStatus status = obj.GetComponent<UserStatus>();
            status.isActive = true;
        }
        lastTopCard = "";
    }

    private string FindPlayerIdByIndex(int index)
    {
        if (player.index == index)
        {
            return player.playerInfo.userId;
        }
        for (int i = 0; i < otherPlayers.Count; i++)
        {
            if (otherPlayers[i].index == index)
            {
                return otherPlayers[i].playerInfo.userId;
            }
        }
        return "";
    }

    public void OnEliminated(List<int> disqualifiers) {
        for (int i = 0; i < disqualifiers.Count; i++)
        {
            if (disqualifiers[i] == player.index)
            {
                player.finalist = false;
            }
            else 
            {
                for (int j = 0; j < otherPlayers.Count; j++)
                {
                    if (disqualifiers[i] == otherPlayers[j].index)
                    {
                        otherPlayers[j].finalist = false;
                    }
                }
            }
        }
    }

    public void OnResultEvent() {
    
    }

    public int GetNext(int index)
    {
        usedIndex.Sort();
        int currIndex = usedIndex.IndexOf(index);
        return usedIndex[(currIndex + 1) % usedIndex.Count];
    }

    public void OnClickCard(string cardname) {
        Debug.Log("XXXX " + turn.ToString() + " " + player.index.ToString());
        if (turn != player.index)
        {
            return;
        }
        bool cardSelected = false;
        foreach (var p in player.cards) {
            GameObject obj = GameObject.Find(p);
            Selectable s = obj.GetComponent<Selectable>();
            if (p != cardname)
            {
                if (s.selected == true) {
                    s.selected = false;
                    s.deselected = true;
                }
            }
            else
            {
                if (s.selected == true)
                {
                    s.selected = false;
                    s.deselected = true;
                }
                else
                {
                    s.deselected = false;
                    s.selected = true;
                    cardSelected = true;
                }
            }
        }

        if (cardSelected == true) {
            Text play = playButton.GetComponentInChildren<Text>();
            Text fold = foldButton.GetComponentInChildren<Text>();
            if (row < 4)
            {
                play.text = "PLAY";
                fold.text = "FOLD";
                if (lastTopCard == "")
                {
                    playButton.gameObject.SetActive(true);
                    foldButton.gameObject.SetActive(false);
                }
                else
                {
                    foldButton.gameObject.SetActive(true);
                    if(largerCard(cardname, lastTopCard))
                    {
                        playButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        playButton.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                play.text = "SHOW";
                playButton.gameObject.SetActive(true);
                foldButton.gameObject.SetActive(false);
            }
        }
    }

    public void PlayCard() {
        foreach (var p in player.cards)
        {
            GameObject obj = GameObject.Find(p);
            Selectable s = obj.GetComponent<Selectable>();
            if (s.selected == true)
            {
                s.selected = false;
                Debug.Log("Select Card Play");
                MessageHandler.Play(obj.name);
            }
        }
        playButton.gameObject.SetActive(false);
        foldButton.gameObject.SetActive(false);
    }

    public void FoldCard()
    {
        foreach (var p in player.cards)
        {
            GameObject obj = GameObject.Find(p);
            Selectable s = obj.GetComponent<Selectable>();
            if (s.selected == true)
            {
                s.selected = false;
                MessageHandler.Fold(obj.name);
            }
        }
        playButton.gameObject.SetActive(false);
        foldButton.gameObject.SetActive(false);
    }

    public bool largerCard(string left, string right)
    {
        string leftSuit = left.Substring(left.Length - 1, 1);
        string leftValue = left.Substring(0, left.Length - 1);
        Debug.Log("YYYY" + leftSuit + " " + leftValue);
        string rightSuit = right.Substring(right.Length - 1, 1);
        string rightValue = right.Substring(0, right.Length - 1);
        Debug.Log("AAAAA" + rightSuit + " " + rightValue);
        if (leftSuit != rightSuit)
        {
            return false;
        }
        if (valueMap[leftValue] > valueMap[rightValue])
        {
            return true;
        }
        return false;
    }
}
