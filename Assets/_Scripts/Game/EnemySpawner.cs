using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public Transform enemyParent;
    int enemyID;

    // Spawn points
    List<Vector3> points;
    Vector3 point1;

    // Floor position
    Transform floor;

    public int numPoints = 3;

    private void Start() 
    {
        floor = PlayerManager.instance.floor.transform;

        points = new List<Vector3>();

        enemyID = 1;

        Debug.Log(floor.position.y);

        // 4362 entry
        points.Add(new Vector3(-1.58000004f, floor.position.y, -1.60000002f));
        // 4361 entry
        points.Add(new Vector3(-3.63000011f, floor.position.y, -1.60000002f));
        // 4360 entry
        points.Add(new Vector3(-4.34000015f, floor.position.y, 0.200000003f));

        // Windows
        points.Add(new Vector3(2.70469856f, floor.position.y, -1.52645588f));
        points.Add(new Vector3(-2.19470334f, floor.position.y, -1.39500523f));
        points.Add(new Vector3(-4.21154022f, floor.position.y, -1.25105476f));
        points.Add(new Vector3(0.584874094f, floor.position.y, 3.54408693f));
        points.Add(new Vector3(3.78896952f, floor.position.y, 6.323318f));

        InvokeRepeating("EnemySpawn", 15, 15);
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(3.0f);
        EnemySpawn();
    }

    void EnemySpawn()
    {
        int index = RandomPoint();
        GameObject enemyobj = Instantiate(enemy, points[index], Quaternion.identity);
        enemyobj.name = "Zombie" + enemyID;
        enemyobj.transform.SetParent(enemyParent);

        enemyID++;
    }

    int RandomPoint()
    {
        int rand = Random.Range(0,numPoints);

        return rand;
    }
}
