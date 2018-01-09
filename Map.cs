using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public static int maxColumns = 9;                                   // Colums amount
    public static int maxRows = 9;                                      // Rows
    public static bool isStart = false;

    public Color levelColor;

    public Data[,] mTable = new Data[maxColumns, maxRows];              // Matrix that holds game map state
    public GameObject prefabBox;                                        // The box, and "add points" bubble prefabs
    public Demo demo;                                                   // BAsic game class;

    public Material activeMaterial1;                                    // Additional materials
    public Material activeMaterial2;
    public Material deaMaterial;

    public class Data
    {
        public GameObject iBox;                                         // A box gameObject - game atom
        public float dYmax;                                             // Maximum position.Y value of box
        public float dest;                                              // Current movement dirrection
        public float speed = 0.0f;                                      // Local movement speed

        public int points = 0;                                          // Used in beamOn coroutine
        public int counter = 0;                                         // Bangs left
        public bool status = true;                                      // Showes can it move or be activated
        public bool bonus = false;                                      // Bonus beam

        public Data(int x, int y)
        {
            speed = ((maxColumns - 1) / 2.0f - Mathf.Abs(x - (maxColumns - 1) / 2.0f) + (maxRows - 1) / 2.0f - Mathf.Abs(y - (maxRows - 1) / 2.0f)) + 0.7f;
            dest = dYmax = speed / 2.2f;
        }
    }

    public void LevelUp(Color color)
    {

        if (!isStart)
        {
            levelColor = color;

            prefabBox.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);

            Setup();
        }

    }

    public void Setup()
    {
        if (isStart)
            return;
        else
            isStart = true;

        demo.DropNull();

//      Generating coordinates of bonuses

        Vector2Int[] bonusPoints = new Vector2Int[Demo.maxBonuses];
        int counter = 0;

        while (counter < Demo.maxBonuses)
        {
            for (int c = 0; c < maxColumns && counter < Demo.maxBonuses; c++)
                for (int r = 0; r < maxRows && counter < Demo.maxBonuses; r++)
                    if (Random.value < 0.01f)
                    {
                        Debug.Log("111 + " + counter + " " + Demo.maxBonuses);
                        bonusPoints[counter] = new Vector2Int(c, r);
                        counter++;
                    }
        }

//      Lets play Saturnus

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        for (int c = 0; c < maxColumns; c++)
        {
            for (int r = 0; r < maxRows; r++)
            {
                Vector3 point = new Vector3(transform.position.x + c, transform.position.y + 0.1f, transform.position.z + r);

                mTable[c, r] = new Data(c, r);

                mTable[c, r].iBox = Instantiate(prefabBox, point, Quaternion.identity);
                mTable[c, r].iBox.GetComponent<blowBox>().SetCoordinates(c, r);
                mTable[c, r].iBox.transform.SetParent(transform);
                mTable[c, r].iBox.SetActive(true);
            }
        }

        foreach (Vector2Int bP in bonusPoints)
        {
            mTable[bP.x, bP.y].bonus = true;
            mTable[bP.x, bP.y].speed += 0.1f;
        }

        demo.mainUI.UpdateScore();
        demo.mainUI.UpdateBeamsCounter();

        isStart = false;
    }

    public void AddPoints(int x, int y, int points)
    {
        if (x < maxColumns && y < maxRows)
        {
            if (mTable[x, y].bonus && mTable[x, y].status)
            {
                mTable[x, y].points = points;
                mTable[x, y].status = false;

                Demo.beamsCount++;

                int maxN = x + 2 < maxColumns ? x + 2 : maxColumns;
                int maxM = y + 2 < maxRows ? y + 2 : maxRows;

                for (int n = x - 1 > 0 ? x - 1 : 0; n < maxN; n++)
                    for (int m = y - 1 > 0 ? y - 1 : 0; m < maxM; m++)
                    {
                        if (mTable[n, m].bonus && mTable[n, m].status && (n != x || m != y) )
                        {
                            AddPoints(n, m, points);
                            Debug.Log("second bonus");
                        }

                        mTable[n, m].points = points;
                        mTable[n, m].status = false;
                    }
            }

            mTable[x, y].points = points;
            mTable[x, y].status = false;
        }
    }

    public bool HitLocation(Vector2 point, out int x, out int y)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(point.x, point.y, 0.0f));

        RaycastHit hitInfo = new RaycastHit();
        Physics.Raycast(ray, out hitInfo, Mathf.Infinity);
        if (hitInfo.collider.gameObject.tag == "box")
        {
            x = hitInfo.collider.gameObject.GetComponent<blowBox>().coordinates.x;
            y = hitInfo.collider.gameObject.GetComponent<blowBox>().coordinates.y;

            return true;
        }

        x = y = 0;
        return false;
    }

}