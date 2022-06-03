using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GeneratingObstacles : MonoBehaviour
{
    public static GameObject Obstacles;
    static int x;
    static int y;
    static int z;
    static float modifier = 3.90625f;
    static int range = 128;
    static int numberOfObstacles = 1000;

    [MenuItem("GameObject/3D Object/CreateGameObjects")]

    static void Create()
    {
        Obstacles = GameObject.Find("Obstacle");
        for (int i = 0; i < numberOfObstacles / 8; i++)
        {
            x = Random.Range(1, range);
            y = Random.Range(1, range);
            z = Random.Range(1, range);
            Vector3 offset = new Vector3(modifier / 2, modifier / 2, modifier / 2);
            Vector3 pos = new Vector3(x, y, z) * modifier + offset;
            Instantiate(Obstacles, pos, new Quaternion(0, 0, 0, 0));
        }
        for (int i = 0; i < numberOfObstacles / 8; i++)
        {
            x = Random.Range(1, range);
            y = Random.Range(1, range);
            z = -Random.Range(1, range);
            Vector3 offset = new Vector3(modifier / 2, modifier / 2, -modifier / 2);
            Vector3 pos = new Vector3(x, y, z) * modifier + offset;
            Instantiate(Obstacles, pos, new Quaternion(0, 0, 0, 0));
        }
        for (int i = 0; i < numberOfObstacles / 8; i++)
        {
            x = Random.Range(1, range);
            y = -Random.Range(1, range);
            z = Random.Range(1, range);
            Vector3 offset = new Vector3(modifier / 2, -modifier / 2, modifier / 2);
            Vector3 pos = new Vector3(x, y, z) * modifier + offset;
            Instantiate(Obstacles, pos, new Quaternion(0, 0, 0, 0));
        }
        for (int i = 0; i < numberOfObstacles / 8; i++)
        {
            x = Random.Range(1, range);
            y = -Random.Range(1, range);
            z = -Random.Range(1, range);
            Vector3 offset = new Vector3(modifier / 2, -modifier / 2, -modifier / 2);
            Vector3 pos = new Vector3(x, y, z) * modifier + offset;
            Instantiate(Obstacles, pos, new Quaternion(0, 0, 0, 0));
        }
        for (int i = 0; i < numberOfObstacles / 8; i++)
        {
            x = -Random.Range(1, range);
            y = Random.Range(1, range);
            z = Random.Range(1, range);
            Vector3 offset = new Vector3(-modifier / 2, modifier / 2, modifier / 2);
            Vector3 pos = new Vector3(x, y, z) * modifier + offset;
            Instantiate(Obstacles, pos, new Quaternion(0, 0, 0, 0));
        }
        for (int i = 0; i < numberOfObstacles / 8; i++)
        {
            x = -Random.Range(1, range);
            y = -Random.Range(1, range);
            z = Random.Range(1, range);
            Vector3 offset = new Vector3(-modifier / 2, -modifier / 2, modifier / 2);
            Vector3 pos = new Vector3(x, y, z) * modifier + offset;
            Instantiate(Obstacles, pos, new Quaternion(0, 0, 0, 0));
        }
        for (int i = 0; i < numberOfObstacles / 8; i++)
        {
            x = -Random.Range(1, range);
            y = Random.Range(1, range);
            z = -Random.Range(1, range);
            Vector3 offset = new Vector3(-modifier / 2, modifier / 2, -modifier / 2);
            Vector3 pos = new Vector3(x, y, z) * modifier + offset;
            Instantiate(Obstacles, pos, new Quaternion(0, 0, 0, 0));
        }
        for (int i = 0; i < numberOfObstacles / 8; i++)
        {
            x = -Random.Range(1, range);
            y = -Random.Range(1, range);
            z = -Random.Range(1, range);
            Vector3 offset = new Vector3(-modifier / 2, -modifier / 2, -modifier / 2);
            Vector3 pos = new Vector3(x, y, z) * modifier + offset;
            Instantiate(Obstacles, pos, new Quaternion(0, 0, 0, 0));
        }


    }

}