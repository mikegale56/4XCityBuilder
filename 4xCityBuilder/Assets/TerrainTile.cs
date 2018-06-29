using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTile : MonoBehaviour {

    public GameObject pre;

    public int worldWidth = 20;
    public int worldHeight = 20;

    public float cubeSize = 2;

    private float positionX;
    private float positionY;
    private float positionZ;

    void Start()
    {
        chessBoardCube = Resources.Load<GameObject>("Prefabs/pre");
        positionX = -11.24f;
        positionY = 4.8f;
        positionZ = 0.0f;
    }

    //Probably unwanted in update
    void Update()
    {
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                GameObject block = Instantiate(pre, Vector3.zero, Quaternion.identity) as GameObject;
                block.transform.parent = transform;

                if (x % 2 == 0)
                {
                    if (y % 2 == 0)
                    {
                        block.GetComponent<Renderer>().material.color = Color.black;
                    }
                    else
                    {
                        block.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
                else
                {
                    if (y % 2 == 0)
                    {
                        block.GetComponent<Renderer>().material.color = Color.red;
                    }
                    else
                    {
                        block.GetComponent<Renderer>().material.color = Color.black;
                    }
                }

                float xP = positionX + x * cubeSize;
                float yP = positionY + y * cubeSize;

                block.transform.localScale = new Vector3(cubeSize, cubeSize, 1);
                block.transform.localPosition = new Vector3(xP, yP, positionZ);
            }
        }
    }
}
