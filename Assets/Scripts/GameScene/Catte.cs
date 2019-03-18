using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Catte : MonoBehaviour
{
    private const int MAXPLAYER = 6;
    private const int MAXCARD = 6;

    // Replace with real data later
    private PlayerInfo playerInfo;
    private RoomInfo roomInfo;
    private bool inGame = false;
    private int row = -1;
    private int turn = -1;
    private string lastTopCard = "";

    public Sprite[] cards;
    public GameObject playerCard;
    public GameObject userInfo;
    public GameObject playerView;
    public GameObject eliminatedIndicator;
    public GameObject otherIndicator;
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
        playerInfo = GameData.currentPlayer;
        roomInfo = GameData.currentRoom;
        Text room = GameObject.Find("Room Id").GetComponent<Text>();
        room.text = roomInfo.roomid;
        Text amount = GameObject.Find("Amount Value").GetComponent<Text>();
        amount.text = Converter.ConvertToMoney(roomInfo.amount);
        player = null;
        otherPlayers = new List<Player>();

        GameClient.OnConnectEvent += OnConnect;
        if (roomInfo.host == "")
        {
            roomInfo.host = Setting.GetHost();
        }
        GameClient.Init(roomInfo.host);

        MessageHandler.OnJoinEvent += OnJoin;
        MessageHandler.OnNewPlayerEvent += OnNewPlayer;
        MessageHandler.OnLeaveEvent += OnLeave;
        MessageHandler.OnCardsEvent += OnCards;
        MessageHandler.OnPlayEvent += OnPlay;
        MessageHandler.OnStartEvent += OnStart;
        MessageHandler.OnEliminatedEvent += OnEliminated;
        MessageHandler.OnErrorEvent += OnError;
        MessageHandler.OnResultEvent += OnResult;
        MessageHandler.Init(roomInfo.roomid, playerInfo.userId);

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
        MessageHandler.OnErrorEvent -= OnError;
        MessageHandler.OnResultEvent -= OnResult;
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
            CardController cardController = newCard.GetComponent<CardController>();
            cardController.faceup = true;
            cardController.lost = false;
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
        // Update the current row's topcard status
        if (p.action == MessageHandler.PLAY)
        {
            if (lastTopCard != "")
            {
                GameObject last = GameObject.Find(lastTopCard);
                CardController lastCard = last.GetComponent<CardController>();
                lastCard.lost = true;
            }
            lastTopCard = p.card;
        }

        // Current player's play
        if (p.index == player.index)
        {
            player.cards.Remove(p.card);
            player.numCard--;

            GameObject obj = GameObject.Find(p.card);
            CardController cardController = obj.GetComponent<CardController>();

            int pos = 5 - player.numCard;
            float xOffset = -0.7f + (0.4f * pos);

            Vector3 targetPos = Vector3.zero;
            Vector3 targetScale = Vector3.zero;
            int order = 1;
            if (p.row < 4)
            {
                order = pos + 1;
                targetPos = new Vector3(playView[0].transform.position.x + xOffset, playView[0].transform.position.y, playView[0].transform.position.z);
                targetScale = new Vector3(obj.transform.localScale.x * 0.8f, obj.transform.localScale.y * 0.8f, obj.transform.localScale.z);
            }
            else if (p.row == 4)
            {
                order = 10;
                targetPos = new Vector3(playerView.transform.position.x, playerView.transform.position.y, playerView.transform.position.z);
            }
            else
            {
                order = 9;
                targetPos = new Vector3(playerView.transform.position.x - 0.6f, playerView.transform.position.y, playerView.transform.position.z);
            }
            cardController.Action(p.action, p.row, order, targetPos, targetScale);

            if (p.row < 4)
            {
                xOffset = -1.3f * (player.cards.Count - 1) / 2;
                for (int i = 0; i < player.cards.Count; i++)
                {
                    GameObject otherCard = GameObject.Find(player.cards[i]);
                    cardController = otherCard.GetComponent<CardController>();
                    cardController.targetPos = new Vector3(playerView.transform.position.x + xOffset, playerView.transform.position.y, playerView.transform.position.z);
                    xOffset += 1.5f;
                }
            }
            if (p.row == 4)
            {
                GameObject otherCard = GameObject.Find(player.cards[0]);
                cardController = otherCard.GetComponent<CardController>();
                cardController.targetPos = new Vector3(playerView.transform.position.x, playerView.transform.position.y, playerView.transform.position.z);
                cardController.sortingOrder = 1;
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
                    CardController cardController = newCard.GetComponent<CardController>();

                    if (p.row < 4)
                    {
                        newCard.transform.position = new Vector3(otherCard[index].transform.position.x, otherCard[index].transform.position.y, otherCard[index].transform.position.z);
                        newCard.transform.localScale = new Vector3(newCard.transform.localScale.x * 0.6f, newCard.transform.localScale.y * 0.6f, newCard.transform.localScale.z);
                    }
                    else if (p.row == 4)
                    {
                        newCard.transform.position = new Vector3(otherCard[index].transform.position.x, otherCard[index].transform.position.y, otherCard[index].transform.position.z);
                        newCard.transform.localScale = new Vector3(newCard.transform.localScale.x * 0.6f, newCard.transform.localScale.y * 0.6f, newCard.transform.localScale.z);
                    }
                    else
                    {
                        newCard.transform.position = new Vector3(playView[index].transform.position.x, playView[index].transform.position.y, playView[index].transform.position.z);
                        newCard.transform.localScale = new Vector3(newCard.transform.localScale.x, newCard.transform.localScale.y, newCard.transform.localScale.z);
                    }

                    Vector3 targetPos = Vector3.zero;
                    Vector3 targetScale = Vector3.zero;
                    int order = 0;
                    if (p.row < 4)
                    {
                        int pos = 5 - player.numCard;
                        float xOffset = -0.7f + (0.4f * pos);
                        order = pos + 1;
                        targetPos = new Vector3(playView[index].transform.position.x + xOffset, playView[index].transform.position.y, playView[index].transform.position.z);
                        targetScale = new Vector3(newCard.transform.localScale.x * 1.34f, newCard.transform.localScale.y * 1.34f, newCard.transform.localScale.z);
                    }
                    else if (p.row == 4)
                    {
                        order = 10;
                        targetPos = new Vector3(playView[index].transform.position.x, playView[index].transform.position.y, playView[index].transform.position.z);
                        targetScale = new Vector3(newCard.transform.localScale.x * 1.67f, newCard.transform.localScale.y * 1.67f, newCard.transform.localScale.z);
                    }
                    else
                    {
                        order = 9;
                        targetPos = new Vector3(playView[index].transform.position.x - 0.5f, playView[index].transform.position.y, playView[index].transform.position.z);
                    }
                    cardController.Action(p.action, p.row, order, targetPos, targetScale);
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
            GameObject obj = GameObject.Find(userId);
            UserController status = obj.GetComponent<UserController>();
            status.isActive = true;
        }
        if ((p.row == 4 && p.newRow == true && turn == player.index) || (p.row == 5 && turn == player.index))
        {
            StartCoroutine(PlayLastCard());
            
        }
        
    }

    void RenderNewPlayer() {
        Player newPlayer = otherPlayers[otherPlayers.Count - 1];
        GameObject obj = Instantiate(userInfo);
        UserController userStatus = obj.GetComponent<UserController>();
        obj.transform.SetParent(canvas.transform, false);
        obj.transform.position = new Vector3(playerInfos[newPlayer.mappedIndex].transform.position.x, playerInfos[newPlayer.mappedIndex].transform.position.y, playerInfos[newPlayer.mappedIndex].transform.position.z);
        userStatus.SetInfo(newPlayer.playerInfo);
        obj.name = newPlayer.playerInfo.userId;
        StartCoroutine(ServiceClient.GetImageTexture(newPlayer.playerInfo.image, userStatus.SetTexture));
    }

    void RenderPlayers() {
        GameObject obj = Instantiate(userInfo);
        UserController userStatus = obj.GetComponent<UserController>();
        Debug.Log(player.playerInfo.userName);
        obj.name = player.playerInfo.userId;
        obj.transform.SetParent(canvas.transform, false);
        obj.transform.position = new Vector3(playerInfos[0].transform.position.x, playerInfos[0].transform.position.y, playerInfos[0].transform.position.z);
        userStatus.SetInfo(player.playerInfo);
        userStatus.SetTexture(GameData.currentPlayerImage);

        for (int i = 0; i < otherPlayers.Count; i++)
        {
            GameObject otherObj = Instantiate(userInfo);
            UserController stt = otherObj.GetComponent<UserController>();
            
            otherObj.transform.SetParent(canvas.transform, false);
            otherObj.transform.position = new Vector3(playerInfos[otherPlayers[i].mappedIndex].transform.position.x, playerInfos[otherPlayers[i].mappedIndex].transform.position.y, playerInfos[otherPlayers[i].mappedIndex].transform.position.z);
            otherObj.name = otherPlayers[i].playerInfo.userId;
            stt.SetInfo(otherPlayers[i].playerInfo);
            StartCoroutine(ServiceClient.GetImageTexture(otherPlayers[i].playerInfo.image, stt.SetTexture));
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
            if (p.playerInfo.userId == playerInfo.userId)
            {
                MessageHandler.SetIndex(p.index);
                usedIndex.Add(p.index);
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
        if (index == player.index)
        {
            SceneManager.LoadSceneAsync("RoomScene");
        }
        else
        {
            for (int i = 0; i < otherPlayers.Count; i++)
            {
                if (otherPlayers[i].index == index)
                {
                    otherPlayers.RemoveAt(i);
                    GameObject obj = GameObject.Find(otherPlayers[i].playerInfo.userId);
                    GameObject.Destroy(obj);
                    usedIndex.Remove(index);
                    break;
                }
            }
        }
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
            UserController status = obj.GetComponent<UserController>();
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
        ShowEliminated();
    }

    void ShowEliminated()
    {
        if (player.finalist == false)
        {
            GameObject obj = Instantiate(eliminatedIndicator);
            obj.transform.SetParent(canvas.transform, false);
            obj.transform.position = playView[0].transform.position;

        }
        for (int i = 0; i < otherPlayers.Count; i++)
        {
            if (otherPlayers[i].finalist == false)
            {
                GameObject obj = Instantiate(eliminatedIndicator);
                obj.transform.SetParent(canvas.transform, false);
                obj.transform.position = playView[otherPlayers[i].mappedIndex].transform.position;
            }
        }
    }

    public void OnResult(List<ResultMsg> results) {
        StartCoroutine(ShowResultAndClean(results));
    }

    IEnumerator ShowResultAndClean(List<ResultMsg> results)
    {
        yield return new WaitForSeconds(2);
        for (int i = 0; i < results.Count; i++)
        {
            if (player.index == results[i].index)
            {
                if (results[i].change > 0)
                {
                    GameObject obj = Instantiate(otherIndicator);
                    obj.transform.SetParent(canvas.transform, false);
                    obj.transform.position = playerInfos[0].transform.position;
                    IndicatorController controller = obj.GetComponent<IndicatorController>();
                    controller.isAmount = false;
                    controller.isWinner = true;
                    controller.text = "NHAT";
                }
                else
                {
                    GameObject obj = Instantiate(otherIndicator);
                    obj.transform.SetParent(canvas.transform, false);
                    obj.transform.position = playerInfos[0].transform.position;
                    IndicatorController controller = obj.GetComponent<IndicatorController>();
                    controller.isAmount = false;
                    controller.isWinner = false;
                    controller.text = "BET";
                }
            }
            else
            {
                for (int j = 0; j < otherPlayers.Count; j++)
                {
                    if (otherPlayers[j].index == results[i].index)
                    {
                        GameObject obj = Instantiate(otherIndicator);
                        obj.transform.SetParent(canvas.transform, false);
                        obj.transform.position = playerInfos[otherPlayers[j].mappedIndex].transform.position;
                        IndicatorController controller = obj.GetComponent<IndicatorController>();
                        if (results[i].change > 0)
                        {
                            controller.isAmount = false;
                            controller.isWinner = true;
                            controller.text = "NHAT";
                        }
                        else
                        {
                            controller.isAmount = false;
                            controller.isWinner = false;
                            controller.text = "BET";
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(2);
        for (int i = 0; i < results.Count; i++)
        {
            if(player.index == results[i].index)
            {
                GameObject obj = Instantiate(otherIndicator);
                obj.transform.SetParent(canvas.transform, false);
                obj.transform.position = new Vector3(playerInfos[0].transform.position.x + 0.5f, playerInfos[0].transform.position.y + 0.5f, playerInfos[0].transform.position.z);
                IndicatorController controller = obj.GetComponent<IndicatorController>();
                controller.isAmount = true;
                if(results[i].change > 0)
                {
                    controller.isWinner = true;
                }
                else
                {
                    controller.isWinner = false;
                }
                controller.text = Converter.ConvertToMoney(results[i].change);
            }
            else
            {
                for (int j = 0; j < otherPlayers.Count; j++)
                {
                    if (otherPlayers[j].index == results[i].index)
                    {
                        GameObject obj = Instantiate(otherIndicator);
                        obj.transform.SetParent(canvas.transform, false);
                        float xOffset = 1.0f;
                        float yOffset = 1.0f;
                        if (otherPlayers[j].mappedIndex > 3)
                        {
                            xOffset = -1.0f;
                        }
                        obj.transform.position = new Vector3(playerInfos[otherPlayers[j].mappedIndex].transform.position.x + 0.5f, playerInfos[otherPlayers[j].mappedIndex].transform.position.y + 0.5f, playerInfos[otherPlayers[j].mappedIndex].transform.position.z);
                        IndicatorController controller = obj.GetComponent<IndicatorController>();
                        controller.isAmount = true;
                        if (results[i].change > 0)
                        {
                            controller.isWinner = true;
                        }
                        else
                        {
                            controller.isWinner = false;
                        }
                        controller.text = Converter.ConvertToMoney(results[i].change);
                    }
                }
            }
        }

        yield return new WaitForSeconds(2);
        var cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (var card in cards)
        {
            Destroy(card);
        }
        var indicators = GameObject.FindGameObjectsWithTag("Indicator");
        foreach (var indicator in indicators)
        {
            Destroy(indicator);
        }
    }

    public void OnError(int error)
    {

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
            CardController s = obj.GetComponent<CardController>();
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
            CardController s = obj.GetComponent<CardController>();
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
            CardController s = obj.GetComponent<CardController>();
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
        string rightSuit = right.Substring(right.Length - 1, 1);
        string rightValue = right.Substring(0, right.Length - 1);
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

    IEnumerator PlayLastCard()
    {
        yield return new WaitForSeconds(1);
        if (lastTopCard == "")
        {
            MessageHandler.Play(player.cards[0]);
        }
        else {
            if (largerCard(player.cards[0], lastTopCard))
            {
                MessageHandler.Play(player.cards[0]);
            }
            else
            {
                MessageHandler.Fold(player.cards[0]);
            }
        }
    }
    
}
