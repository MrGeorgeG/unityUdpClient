using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Unity.Collections;

public class NetworkMan : MonoBehaviour
{
    List<GameObject> GameList;
    public UdpClient udp;
    public GameObject PlayObject;

    // Start is called before the first frame update
    void Start()
    {
        udp = new UdpClient();
        
        udp.Connect("52.199.251.236", 12345);

        GameList = new List<GameObject>();

        Byte[] sendBytes = Encoding.ASCII.GetBytes("connect");
      
        udp.Send(sendBytes, sendBytes.Length);

        udp.BeginReceive(new AsyncCallback(OnReceived), udp);

        InvokeRepeating("HeartBeat", 1, 1);
    }

    void OnDestroy(){
        udp.Dispose();
    }


    public enum commands{
        NEW_CLIENT,
        UPDATE,
        NEW_PLAYERS,
        DESTROY_PLAYER
    };
    
    [Serializable]
    public class Message{
        public commands cmd;
    }
    
    [Serializable]
    public class Player{
        [Serializable]
        public struct receivedColor{
            public float R;
            public float G;
            public float B;
        }
        public string id;
        public receivedColor color;        
    }

    [Serializable]
    public class NewPlayer{
        public Player player;
    }

    [Serializable]
    public class GameState{
        public Player[] players;
    }

    [Serializable]
    public class DestroyCube
    {
        public NewPlayer[] players;
    }

    public Message latestMessage;
    public NewPlayer playerOne;
    public GameState lastestGameState;
    public GameState newGameState;
    public DestroyCube outPlayers;

    void OnReceived(IAsyncResult result){
        // this is what had been passed into BeginReceive as the second parameter:
        UdpClient socket = result.AsyncState as UdpClient;
        
        // points towards whoever had sent the message:
        IPEndPoint source = new IPEndPoint(0, 0);

        // get the actual message and fill out the source:
        byte[] message = socket.EndReceive(result, ref source);
        
        // do what you'd like with `message` here:
        string returnData = Encoding.ASCII.GetString(message);
        Debug.Log("Got this: " + returnData);
        
        latestMessage = JsonUtility.FromJson<Message>(returnData);
        try{
            switch(latestMessage.cmd){
                case commands.NEW_CLIENT:
                    playerOne = JsonUtility.FromJson<NewPlayer>(returnData);
                    Debug.Log(playerOne.player.id);

                    break;
                case commands.UPDATE:
                    lastestGameState = JsonUtility.FromJson<GameState>(returnData);
                    break;
                case commands.NEW_PLAYERS:
                    newGameState = JsonUtility.FromJson<GameState>(returnData);
                    break;

                case commands.DESTROY_PLAYER:
                    outPlayers = JsonUtility.FromJson<DestroyCube> (returnData);
                    Debug.Log(outPlayers.players);
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }
        catch (Exception e){
            Debug.Log(e.ToString());
        }
        
        // schedule the next receive operation once reading is done:
        socket.BeginReceive(new AsyncCallback(OnReceived), socket);
    }

    void SpawnPlayers(){

        foreach (Player NewCharacter in newGameState.players)
        {
            Vector3 VPoint = new Vector3(UnityEngine.Random.Range(-4, 4), UnityEngine.Random.Range(-4, 4), UnityEngine.Random.Range(0, 4));
            foreach (GameObject playerCube in GameObject.FindGameObjectsWithTag("Player"))
            {
                foreach (GameObject playerObjet in GameList)
                {
                    if (playerCube.GetComponent<PlayerCubeID>().UpdateID == NewCharacter.id)
                    {
                        return;
                    }

                }

            }

            GameObject CubePlayer = Instantiate(PlayObject, VPoint, Quaternion.identity);
            CubePlayer.GetComponent<PlayerCubeID>().UpdateID = NewCharacter.id;
            GameList.Add(CubePlayer);
        }


    }

    void UpdatePlayers(){

        if (lastestGameState.players.Length > GameList.Count)
        {
            foreach (Player Players in lastestGameState.players)
            {
                Vector3 VPoint = new Vector3(UnityEngine.Random.Range(-4, 4), UnityEngine.Random.Range(-4, 4), UnityEngine.Random.Range(0, 4));
                GameObject NewPlayer = Instantiate(PlayObject, VPoint, Quaternion.identity);
                NewPlayer.GetComponent<PlayerCubeID>().UpdateID = Players.id;
                GameList.Add(NewPlayer);
            }
        }

        foreach (Player CPlayer in lastestGameState.players)
        {
            foreach(GameObject CubeColor in GameList)
            {
                if (CPlayer.id.Equals(CubeColor.GetComponent<PlayerCubeID>().UpdateID))
                {
                    CubeColor.GetComponent<MeshRenderer>().material.color = new Color(CPlayer.color.R, CPlayer.color.G, CPlayer.color.B);

                }

            }

        }

    }

    void DestroyPlayers(){

    }

    void HeartBeat(){
        Byte[] sendBytes = Encoding.ASCII.GetBytes("heartbeat");
        udp.Send(sendBytes, sendBytes.Length);
    }

    void Update(){
        SpawnPlayers();
        UpdatePlayers();
        DestroyPlayers();
    }
}
