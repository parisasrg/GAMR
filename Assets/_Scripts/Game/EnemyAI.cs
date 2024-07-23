using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private void Update() {
        GetComponent<NavMeshAgent>().SetDestination(new Vector3(-0.819999993f,-1.1f,-3.47000003f));
    }
}
