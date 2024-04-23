using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnpoint : MonoBehaviour
{
    public static List<EnemySpawnpoint> enemySpawnpoints = new List<EnemySpawnpoint>();
    
    private void OnEnable()
    {
        enemySpawnpoints.Add(this);
    }
    private void OnDisable()
    {
        enemySpawnpoints.Remove(this);
    }
    public static Vector3 GetRandomSpawnPos()
    {
        if(enemySpawnpoints.Count == 0)
        {
            return Vector3.zero;
        }
        return enemySpawnpoints[Random.Range(0,enemySpawnpoints.Count)].transform.position;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, (float) 0.5);
    }
}
