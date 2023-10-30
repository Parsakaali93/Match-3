using UnityEngine;
using System.Collections;
using DG.Tweening;

public class game : MonoBehaviour {
	public static int gameFieldSize = 7;
	public static float gameSpeed = 0.35f;
	
	public static float gemSize = 1;
	
	public static Transform[ , ] gameField;
	
	// Every place where a gem can be is stored here.
	public static Vector3[ , ] gridPoint;
	
	public Sprite[] gemMaterials = new Sprite[6];
	public Transform gem;
	float gridStartX = 0;
	float gridStartY = 0;

    public static int SelectedX = -1;
	public static int SelectedY = -1;
	public static Transform SelectedGem;
	
	public static int Selected2X = -1;
	public static int Selected2Y = -1;
	public static Transform SelectedGem2;
	
	public Transform background;
	public Transform gridSquare;
	
	public static bool moving = false;
	
	public static bool checkingGems = false;
	
	public static bool allStill = false;

    public static int enemyHP = 100;
	// Use this for initialization
	void Start () {
		
	//Instantiate(background, new Vector3(0, 0, 25), background.rotation);
		
	gemSize = 18f / (float)gameFieldSize;
	gridStartX = (gameFieldSize * gemSize / 2) * - 1 + gemSize/2;
	gridStartY = ((gameFieldSize * gemSize / 2) * - 1 + gemSize/2)* -1;
		
	gridPoint = new Vector3[gameFieldSize, gameFieldSize];
	gameField = new Transform[gameFieldSize, gameFieldSize];
//	randomizeGems();
	instantiateFromArray();

        for (int y = 0; y < gameFieldSize; y++)
        {
            for (int x = 0; x < gameFieldSize; x++)
            {
                gameField[y, x].GetComponent<gem>().arrived = true;
            }
        }
    }
	
	void OnGUI()
	{
	if(SelectedGem != null)
	{
	GUI.Label(new Rect(10, 20, 300, 30), "<size=20>GEM INFO:</size>");
	GUI.Label(new Rect(10, 50, 100, 30), "X: " + SelectedX + "   Y: " + SelectedY);
	GUI.Label(new Rect(10, 65, 300, 30), "Left: " + SelectedGem.GetComponent<gem>().left + "  Right: " + SelectedGem.GetComponent<gem>().right + "  Up: " + SelectedGem.GetComponent<gem>().up + "  Down: " + SelectedGem.GetComponent<gem>().down);
	GUI.Label(new Rect(10, 80, 300, 30), "Needs Removing: " + SelectedGem.GetComponent<gem>().needsToBeRemoved);
	}
	}

    //	void randomizeGems()
    //	{
    //		for(int y = 0; y < gameFieldSize; y++)
    //			for(int x = 0; x < gameFieldSize; x++)
    //			{
    //				gameField[y, x] = gem;
    //			}
    //	}

        /*
    public static void performFullCheck()
    {
        allStill = false;
        checkingGems = true;

        for (int y = 0; y < gameFieldSize; y++)
        {
            for (int x = 0; x < gameFieldSize; x++)
            {
                gameField[y, x].GetComponent<gem>().checkNeighbors();
            }
        }

        checkingGems = false;
    }*/

    public static void performSwitch()
	{
		moving = false;

		gem selected1 = SelectedGem.GetComponent<gem>();
		gem selected2 = SelectedGem2.GetComponent<gem>();
		
		int tempX = selected2.x;
		int tempY = selected2.y;
		Transform tempTransform = gameField[selected1.y, selected1.x];
		
		gameField[selected1.y, selected1.x] = gameField[selected2.y, selected2.x];
		gameField[selected2.y, selected2.x] = tempTransform;
		
		selected1.moving = true;
		selected2.moving = true;

        selected1.MoveToTarget(SelectedGem2.position, 0.2f);
        selected2.MoveToTarget(SelectedGem.position, 0.2f);

        //selected1.destination = SelectedGem2.position;
		//selected2.destination = SelectedGem.position;
		selected2.x = selected1.x;
		selected2.y = selected1.y;
		selected1.x = tempX;
		selected1.y = tempY;
		selected1.arrived = false;
		selected2.arrived = false;
		
//		Vector3 selectedGemPosition = SelectedGem.position;
//		SelectedGem.position = SelectedGem2.position;
//		SelectedGem2.position = selectedGemPosition;
		
		SelectedX = -1;
		SelectedY = -1;
		Selected2X = -1;
		Selected2Y = -1;
		
		//performFullCheck();
	}
	
	void instantiateFromArray()
	{
		for(int y = 0; y < gameFieldSize; y++)
		{
			for(int x = 0; x < gameFieldSize; x++)
			{
				Transform instantiatedGem = Instantiate(gem, new Vector3(gridStartX + x * gemSize, gridStartY - y * gemSize, 20f), gem.rotation)as Transform;
				Transform sq = Instantiate(gridSquare, new Vector3(gridStartX + x * gemSize, gridStartY - y * gemSize, 23f), gridSquare.rotation) as Transform;
				sq.localScale = new Vector3(gemSize, gemSize, 0);
                instantiatedGem.eulerAngles = Vector3.zero;
				instantiatedGem.localScale = new Vector3(gemSize, gemSize, 0);
				instantiatedGem.GetComponent<gem>().x = x;
				instantiatedGem.GetComponent<gem>().y = y;
				instantiatedGem.GetComponent<gem>().destination = instantiatedGem.position;
				gridPoint[y, x] = instantiatedGem.position;
				gameField[y, x] = instantiatedGem;
			}
		}
	}

    void checkAndDestroy()
    {
        for (int y = 0; y < gameFieldSize; y++)
        {
            for (int x = 0; x < gameFieldSize; x++)
            {
                gameField[y, x].GetComponent<gem>().checkNeighbors();
            }
        }

        for (int y = 0; y < gameFieldSize; y++)
        {
            for (int x = 0; x < gameFieldSize; x++)
            {
                if (gameField[y, x].GetComponent<gem>().needsToBeRemoved)
                {
                    gameField[y, x].GetComponent<gem>().destroyThisGem();
                }
            }
        }
    }

	// Update is called once per frame
	void Update ()
    {
	int amountArrived = 0;
		
	for(int y = 0; y < gameFieldSize; y++)
	{
		for(int x = 0; x < gameFieldSize; x++)
		{
			if(gameField[y, x].GetComponent<gem>().arrived)
			{
				amountArrived++;
			}
		}
	}

        print(amountArrived);
        if (amountArrived == gameFieldSize * gameFieldSize && DOTween.TotalPlayingTweens() == 0)
        {
            allStill = true;

            checkAndDestroy();
            allStill = false;
        }

        else
            allStill = false;


		
	}
}
