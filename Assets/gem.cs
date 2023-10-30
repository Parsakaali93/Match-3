using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class gem : MonoBehaviour {
	
	public int type = 0;
	public int x;
	public int y;

	// The place where this gem is heading. If this gem is swapped with
	// another its destination is the other gem's current location.
	public Vector3 destination;
	public bool arrived = false;
	
	// How many neighboring gems of the same type does this one have around it?
	public int up = 0;
	public int down = 0;
	public int left = 0;
	public int right = 0;
	
	public bool needsToBeRemoved = false;
	
	// Can't target gems that are moving
	public bool moving;

	// Use this for initialization
	void Start () 
	{
		type = Random.Range(0, 5);

        if (type == 0)
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;

        if(type == 1)
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.green;

        if (type == 2)
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;

        if (type == 3)
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;

        if (type == 4)
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.yellow;

        transform.GetComponent<SpriteRenderer>().sprite = GameObject.Find("Game").GetComponent<game>().gemMaterials[type];
	}
	
	void OnMouseEnter()
	{
		transform.localScale = new Vector3(transform.localScale.x + 5 / (float)game.gameFieldSize, transform.localScale.y + 5 / (float)game.gameFieldSize, transform.localScale.z);
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
	}
	
	public void checkNeighbors()
	{
		up = 0;
		right = 0;
		left = 0;
		down = 0;
		int neighborNum = 1;
		needsToBeRemoved = false;
		
		while(x + neighborNum < game.gameFieldSize)
		{
			if(game.gameField[y, x + neighborNum].GetComponent<gem>().type == type)
			{
				right++;
				neighborNum++;
			}
			
			else
				break;
		}
		
		neighborNum = 1;
		
		while(x - neighborNum >= 0)
		{
			if(game.gameField[y, x - neighborNum].GetComponent<gem>().type == type)
			{
				left++;
				neighborNum++;
			}
			
			else
				break;
		}
		
		neighborNum = 1;
		
		while(y + neighborNum < game.gameFieldSize)
		{
			if(game.gameField[y + neighborNum,x].GetComponent<gem>().type == type)
			{
				down++;
				neighborNum++;
			}
			
			else
				break;
		}
		
		neighborNum = 1;
		
		while(y - neighborNum >= 0)
		{
			if(game.gameField[y - neighborNum,x].GetComponent<gem>().type == type)
			{
				up++;
				neighborNum++;
			}
			
			else
				break;
		}
		
		if(left + right >= 2)
		{
			for(int i = 1; i <= left; i++)
			{
				game.gameField[y, x - i].GetComponent<gem>().needsToBeRemoved = true;	
			}
			
			for(int i = 1; i <= right; i++)
			{
				game.gameField[y, x + i].GetComponent<gem>().needsToBeRemoved = true;	
			}
			
			needsToBeRemoved = true;
		}
		
		if(up + down >= 2)
		{
			for(int i = 1; i <= up; i++)
			{
				game.gameField[y - i, x].GetComponent<gem>().needsToBeRemoved = true;	
			}
			
			for(int i = 1; i <= down; i++)
			{
				game.gameField[y + i, x].GetComponent<gem>().needsToBeRemoved = true;	
			}
			
			needsToBeRemoved = true;
		}
	}
	
	void OnMouseOver()
	{
		if(Input.GetMouseButtonDown(1))
		{
			destroyThisGem();
		}	
	}
	
	void OnMouseDown()
	{
		if(Input.GetMouseButtonDown(0))
		{
			if(!moving)
			{
				if(game.SelectedX == -1 && game.SelectedY == -1)
				{
				//checkNeighbors();
				game.SelectedX = x;
				game.SelectedY = y;
				game.SelectedGem = transform;
				}
				
				else
				{
				game.Selected2X = x;
				game.Selected2Y = y;
				game.SelectedGem2 = transform;
				
				game.performSwitch();
				}
			}
		}
	}
	
	void OnMouseExit()
	{
		transform.localScale = new Vector3(transform.localScale.x - 5/(float)game.gameFieldSize, transform.localScale.y - 5/(float)game.gameFieldSize, transform.localScale.z);
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
	}
	
    IEnumerator move(float f)
    {
        yield return new WaitForSeconds(f);

        arrived = true;
        moving = false;
    }

    public void MoveToTarget(Vector3 target, float time)
    {
        destination = target;

        moving = true;
        arrived = false;

        transform.DOMove(target, time, false);
        StartCoroutine("move", time);
    }

	public void destroyThisGem()
	{

            if(type == 2)
            {
            game.enemyHP--;
            GameObject.Find("Enemy HP").GetComponent<Text>().text = game.enemyHP.ToString();
            }

			Destroy(gameObject);

			needsToBeRemoved = false;
			
			for(int thisY = y; thisY >= 0; thisY--)
			{
				if(thisY - 1 >= 0)
				{
					game.gameField[thisY, x] = game.gameField[thisY - 1, x];

                //game.gameField[thisY, x].GetComponent<gem>().destination = game.gridPoint[thisY, x];

                DOTween.Kill(game.gameField[thisY, x], false);
                    game.gameField[thisY, x].GetComponent<gem>().MoveToTarget(game.gridPoint[thisY, x], (1f + (thisY/10f)) * Random.Range(0.4f, 0.55f));

                    game.gameField[thisY, x].GetComponent<gem>().y = thisY;
				}
				
				else
				{
					Transform instantiatedGem = Instantiate(GameObject.Find("Game").GetComponent<game>().gem, new Vector3(transform.position.x, -transform.position.y + game.gameFieldSize * game.gemSize, 20), GameObject.Find("Game").GetComponent<game>().gem.rotation) as Transform;
					game.gameField[0, x] = instantiatedGem;
					instantiatedGem.localScale = new Vector3(game.gemSize, game.gemSize, 0);
					instantiatedGem.GetComponent<gem>().x = x;
					instantiatedGem.GetComponent<gem>().y = 0;
                    // instantiatedGem.GetComponent<gem>().destination = game.gridPoint[0, x];
                    
                        instantiatedGem.GetComponent<gem>().MoveToTarget(game.gridPoint[0, x], Random.Range(0.55f, 0.7f));

                    instantiatedGem.GetComponent<gem>().arrived = false; 
				}
			}
			
			game.SelectedX = -1;
			game.SelectedY = -1;
			game.Selected2X = -1;
			game.Selected2Y = -1;
	}
	
	
	// Update is called once per frame
	void Update () 
	{
        //if (transform.position.y > 11f)
        //    transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

		//if(needsToBeRemoved)
		//	destroyThisGem();
		
		//if(arrived && game.allStill)
		//    checkNeighbors();
		
//		if(needsToBeRemoved)
//			destroyThisGem();
		
        /*
		if(destination.x > transform.position.x)
		{
			transform.position = new Vector3(transform.position.x + game.gameSpeed, transform.position.y, transform.position.z);	
		}
		
		if(destination.x < transform.position.x)
		{
			transform.position = new Vector3(transform.position.x - game.gameSpeed, transform.position.y, transform.position.z);	
		}
		
		if(destination.y > transform.position.y)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + game.gameSpeed, transform.position.z);	
		}
		
		if(destination.y < transform.position.y)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - game.gameSpeed, transform.position.z);	
		}
		
		if(Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(destination.x, destination.y)) < 0.1f)
		{
			transform.position = destination;
			moving = false;
            arrived = true;
		}

        */
	}
}
