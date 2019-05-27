using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/*
NetworkManager gere les players, avec playerInfo qui contient lui un PlayerTest aussi ?


*/


public class PlayerTest {

	// Variable nécessaire au bon fonctionnement du script
    [SerializeField]
    private Tilemap tileMap;
    private Rigidbody2D rb2d;
    private GameObject contentPlayer;
	private GameObject PManager;
	


	// Variable concernant le joueur
    // Il a son real x et y et son x et y par rapport à la grille
    private int x;
    private int y;
	private int id;
	
	
	
	// Pour l'animation du mouvement
	private Vector3 lastPos;
    private bool canMove = true;
	private IEnumerator corout;

    public PlayerTest(int id, int x, int y, GameObject tmpP) {
		this.x = x;
		this.y = y;
		this.id = id;
		
        //rb2d = t.GetComponent<Rigidbody2D>();
        contentPlayer = tmpP;
        rb2d = contentPlayer.GetComponent<Rigidbody2D>();
        
        tileMap = GameObject.FindGameObjectWithTag("obstacles").GetComponentInParent<Tilemap>();
        //playersList = GameObject.FindGameObjectsWithTag("Player");
		PManager = GameObject.FindWithTag("PlayerManager");

    }

	// Changement de position direct
    public void changePosition(int x, int y) {
        // On traduit le x et y par le vrai x et y

        // mettre en const
        float rX = (float) -7.5f+x;
        float rY = (float) 5.5f-y;

       // lastPos = transform.position;

       // Vector3 startPoint = contentPlayer.transform.position;
       // lastPos = startPoint;
     //   Vector3 endPoint = startPoint + new Vector3(x, y, 0);
		//rb2d.MovePosition(new Vector3(rX, rY, 0));
		// Sinon pas instantané avec rb2d??
		contentPlayer.transform.position = new Vector3(rX, rY, 0);
		rb2d.MovePosition(new Vector3(rX, rY, 0));
		lastPos = contentPlayer.transform.position;
      //  StartCoroutine(smoothTranslation(endPoint));
    }
	
	// Animation du mouvement
	public void movePosition(int x, int y) {
		if(corout != null) {
			PManager.GetComponent<PlayerManager>().askStopCoroutine(corout);
			corout = null;
		}
		lastPos = contentPlayer.transform.position;

        Vector3 startPoint = contentPlayer.transform.position;
        lastPos = startPoint;
        Vector3 endPoint = startPoint + new Vector3(x, y, 0);
		
		// Faut stopper la corountine après aussi non?...
       // StartCoroutine(smoothTranslation(endPoint), );
	   
	   // Mettre le PManager en static pour eviter de rappeler à chaque foi
	   corout = smoothTranslation(endPoint);
	   PManager.GetComponent<PlayerManager>().askCoroutine(corout);
	 //  PManager.GetComponent<PlayerManager>().askStopCoroutine(endPoint, this);

	}
	
	// Car la coroutine en monobehavior seulement
	public IEnumerator smoothTranslation(Vector3 end) {
        float sqrDist = (contentPlayer.transform.position - end).sqrMagnitude;
        while (sqrDist > float.Epsilon) {
			// Le move time faut que je fasse le truc du stats mais pour l'instant je mets en dur
            Vector3 newPos = Vector3.MoveTowards(rb2d.position, end, (1 / 0.5f) * Time.deltaTime);
            rb2d.MovePosition(newPos);
            sqrDist = (contentPlayer.transform.position - end).sqrMagnitude;
            yield return null;
        }
		corout = null;
    }
	



}

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject TPlayerPrefab;

	private ArrayList players;
    private PlayerTest tmp;
	
	private bool test = false;
	
	//public void askCoroutine(Vector3 end, PlayerTest p, IEnumerator corout) {
	public void askCoroutine(IEnumerator corout) {
		//StartCoroutine(p.smoothTranslation(end));
		StartCoroutine(corout);
	}
	
	//public void askStopCoroutine(Vector3 end, PlayerTest p, IEnumerator corout) {
	public void askStopCoroutine(IEnumerator corout) {
		//StopCoroutine(p.smoothTranslation(end));
		StopCoroutine(corout);
	}
	
	public PlayerTest addPlayer(int id, int x, int y) {
		return new PlayerTest(0, 0, 0, Instantiate(TPlayerPrefab, new Vector2(0, 0), Quaternion.identity));
	}
	

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test PlayerManager !");

        // 5.5 => -10.5
        // -7.5 => 8.5

      //  PlayerTest tmp2 = new PlayerTest(0, 0, Instantiate(TPlayerPrefab, new Vector2(10, 0), Quaternion.identity));
    }
	


    // Update is called once per frame
    void Update()
    {
        if(tmp != null && !test) {
		/*	test = true;
			Debug.Log("Test du movement");
			tmp.changePosition(0, 0);

			tmp.movePosition(1, 0);*/
		}
        //tmp.changePosition(1, 0);     
    }
}
