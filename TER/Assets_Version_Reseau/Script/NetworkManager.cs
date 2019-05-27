using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public struct msg {
	public int id;
	public int x;
	public int y;
	public int cx;
	public int cy;
	public char direction;
};

public struct msgsend_pos {
	public int x;
	public int y;
	public int cx;
	public int cy;
	public char direction;
}

public struct test2 {
	public int x;
};

public struct msg_codeid {
	public int id;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct msg_grille {
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
	public char[] grille;
};

public struct msg_newplayer {
	public int id;
		
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	public char[] pseudo;

	public int x;
	public int y;
	public int hp;
	public int nbBombe;
	public int puissance;

}



public class PlayerInfo {

	private int id;
	private string pseudo;
	private int hp;
	private PlayerTest realView;

	public PlayerInfo(msg_newplayer data, PlayerTest rv) {
		id = data.id;
		pseudo = new string(data.pseudo);
		hp = data.hp;
		realView = rv;
	}

	public int getId() {
		return id;
	}

	public PlayerTest getRealView() {
		return realView;
	}

	public void changePos(int x, int y) {
	   //realView.transform.position = new Vector3(x, y, realView.transform.position.z);
	   realView.changePosition(x, y);
	}
	
	public void movePos(int x, int y) {
	   //realView.transform.position = new Vector3(x, y, realView.transform.position.z);
	   realView.movePosition(x, y);
	}
}


public class NetworkManager : MonoBehaviour
{
	private TcpClient client;
	const int READ_BUFFER_SIZE = 2048; // on maximize 
	private byte[] readBuffer = new byte[READ_BUFFER_SIZE];

	private bool isConnected = false;


	// Pour la gestion du protocole
	private int codeID;
	private bool isReadingStruct = false;

	/* Pour l'instant on gère le jeu aussi dans ce code */
	private char[] grille;
	// Liste des joueurs (pour l'instant on le laisse ici mais on devrait séparer)
	private ArrayList players = new ArrayList();

	private ArrayList pipe = new ArrayList();
	private ArrayList pipemsg = new ArrayList();
	private msg[] pipemsg2;


	// 
	GameObject PManager;


 
    // Start is called before the first frame update
    void Start() {
		PManager = GameObject.FindWithTag("PlayerManager");

        try {
			client = new TcpClient("93.113.206.174", 9000);
			client.GetStream().BeginRead(readBuffer, 0, System.Runtime.InteropServices.Marshal.SizeOf(typeof(msg_codeid)), new AsyncCallback(DoRead), null);
			isConnected = true;

			Debug.Log("Connection réussite");
		} catch(Exception e){
			Debug.Log("Erreur de connection");
		}
    }


	//https://stackoverflow.com/questions/3278827/how-to-convert-a-structure-to-a-byte-array-in-c

	public static T FromBytes<T>(byte[] arr) where T : struct {
    	T str = default(T);
		GCHandle h = default(GCHandle);
		try {
			h = GCHandle.Alloc(arr, GCHandleType.Pinned);
			str = Marshal.PtrToStructure<T>(h.AddrOfPinnedObject());
		}
		finally {
			if (h.IsAllocated) {
				h.Free();
			}
		}

   		 return str;
	}

	public static byte[] GetBytes<T>(T str) {
		int size = Marshal.SizeOf(str);
		byte[] arr = new byte[size];
		GCHandle h = default(GCHandle);

		try	{
			h = GCHandle.Alloc(arr, GCHandleType.Pinned);
			Marshal.StructureToPtr<T>(str, h.AddrOfPinnedObject(), false);
		}
		finally	{
			if (h.IsAllocated) {
				h.Free();
			}
		}

		return arr;
	}
	
	void DoRead(IAsyncResult ar)
	{ 
			// On lit les messages
			int BytesRead;

			try
			{
				BytesRead = client.GetStream().EndRead(ar);

				// Si la taille est = 0 alors c'est que le serveur nous a déco?
				if(BytesRead < 1) {
					Debug.Log("Bytes read < 0 ");
					return;
				}

				Debug.Log("On m'envoie un message");

				// Gestion de la structure du protocole

				// Pour savoir on s'attends à cb de byte après
				int sizeOfNext = 0;

				// Si on attends un codeid, alors on traite
				if(!isReadingStruct) {
					msg_codeid data = FromBytes<msg_codeid>(readBuffer);
					codeID = data.id;
					Debug.Log("C'est un code id ! : "+codeID);
					isReadingStruct = true;

					// on met la taille des bytes qu'on s'attends après
					switch(codeID){
						case 1: sizeOfNext =  System.Runtime.InteropServices.Marshal.SizeOf(typeof(msg)); break;
						case 2: sizeOfNext =  System.Runtime.InteropServices.Marshal.SizeOf(typeof(test2)); break;
						case 3: sizeOfNext = System.Runtime.InteropServices.Marshal.SizeOf(typeof(msg_grille)); break;
						case 4: sizeOfNext = System.Runtime.InteropServices.Marshal.SizeOf(typeof(msg_newplayer)); break;
						default: break;
					}

				} else {
					Debug.Log("C'est une structure");
					isReadingStruct = false;
					// on traite les différents code id
					switch(codeID) {
						case 1:
						//	Debug.Log("Position");
							//msg data = FromBytes<msg>(readBuffer);
							Position(FromBytes<msg>(readBuffer));
							break;
						case 2:
						//	Debug.Log("Test");
							test2 data1 = FromBytes<test2>(readBuffer);
						//	Debug.Log("Voila "+data1.x);
							break;
						case 3:
					//		Debug.Log("Grille");
							//msg_grille g = FromBytes<msg_grille>(readBuffer);
							NewGrille(FromBytes<msg_grille>(readBuffer));
							break;
						case 4:
							NewPlayer(FromBytes<msg_newplayer>(readBuffer));
							break;
						default: break;
					}
					sizeOfNext = System.Runtime.InteropServices.Marshal.SizeOf(typeof(msg_codeid));
				}

				/* ouverture sur le udp pour les performances par exemple? */
 				client.GetStream().BeginRead(readBuffer, 0,sizeOfNext, new AsyncCallback(DoRead), null);

			} 
			catch(Exception e) {
				// Disconnected
			//	isConnected = false;
				Debug.Log("erreur catched"+e);
			}
	}
	

	void SendStruct<T>(T data) {
		if(!isConnected)
			return;

		//Debug.Log("J'envoie !");
		// On transforme en byte
		byte[] dataByte = GetBytes<T>(data);
		
		// On récupère stream
		StreamWriter writer = new StreamWriter(client.GetStream());
		writer.Write(System.Text.Encoding.UTF8.GetString(dataByte).ToCharArray(), 0, dataByte.Length);
		writer.Flush();
	}


	void Position(msg data) {
	//	SendStruct<msg>(data);
		/*foreach(PlayerInfo p in players) {
			if(p.getId() == data.id) {
				p.changePos(data.x, data.y);
			}
		}
		*/
		pipemsg.Add(data);
	}

	void NewGrille(msg_grille data) {
		grille = data.grille;
	}

	void NewPlayer(msg_newplayer data) {
		Debug.Log("Un nouveau joueur se connecte "+data.id+" avec pour pseudo : "+new string(data.pseudo));
		Debug.Log("HP "+data.hp);
		Debug.Log("x "+data.x+" - y"+data.y);
		Debug.Log("nbbombe "+data.nbBombe);


		pipe.Add(data);
		/*
		GameObject newP = Instantiate(spritePrefab, new Vector2(data.x, data.y), Quaternion.identity);
		PlayerInfo newPlayer = new PlayerInfo(data, newP);
		players.Add(newPlayer);*/
	}

	public void changePosition(Vector3 s, Vector3 e, bool dir){//float x, float y) {
		//Debug.Log("test post");
		// On converti dans le sens inverse
		int x = (int) ( s.x+7.5f );
		int y = (int) -( s.y-5.5f );
		int cx = (int) ( e.x+7.5f );
		int cy = (int) -( e.y-5.5f );
		
		msgsend_pos newMsg;
		newMsg.x = Math.Abs(x);
		newMsg.y = Math.Abs(y);
		newMsg.cx = Math.Abs(cx);
		newMsg.cy = Math.Abs(cy);
		// Direction à faire
		if(dir)
			newMsg.direction = 'd';
		else
			newMsg.direction = 'i';
			
		msg_codeid msgId;
		msgId.id = 1;
		SendStruct<msg_codeid>(msgId);
		SendStruct<msgsend_pos>(newMsg);
	}
	
	

    void Update() {
		
		
		// Pipe du PlayerInfo
		bool t = false;
		foreach( msg_newplayer p in pipe ) {

			//GameObject newP = Instantiate(spritePrefab, new Vector2((float) p.x, (float) p.y), Quaternion.identity);
			PlayerTest newP = PManager.GetComponent<PlayerManager>().addPlayer(p.id, p.x, p.y);
			PlayerInfo newPlayer = new PlayerInfo(p, newP);
			players.Add(newPlayer);
			t = true;
		}
		// Faudrait le faire seulement si on avait fait le foreach
		if(t)
			pipe = new ArrayList();
		



		t = false;
	for(int i = 0; i < pipemsg.Count; i++) {
			msg pm = (msg) pipemsg[i];
		//foreach( msg pm in pipemsg ) {
							//Debug.Log("déplace "+pipemsg.Count);
			foreach(PlayerInfo p in players) {
				t = true;
				if(p.getId() == pm.id) {
					if(pm.direction == 'd')
						p.movePos(pm.cx-pm.x, pm.y-pm.cy);
					else if(pm.direction == 'i')
						p.changePos(pm.x, pm.y);
				}
			}
		}

	// Clear plutot?
		if(t)
			pipemsg.Clear();

		

      if(Input.GetKeyDown("space") && isConnected)  {
        	/*Debug.Log("test");
			msgsend_pos newMsg;
			newMsg.x = 0;
			newMsg.y = 1;
			newMsg.cx = 0;
			newMsg.cy = 2;
			Debug.Log("test : "+newMsg.cy);
			newMsg.direction = 'd';

			msg_codeid msgId;
			msgId.id = 1;
			Debug.Log("J'eenvoie id");
			SendStruct<msg_codeid>(msgId);
			Debug.Log("J'envoie struct");
			SendStruct<msgsend_pos>(newMsg); */
        }
    }


}