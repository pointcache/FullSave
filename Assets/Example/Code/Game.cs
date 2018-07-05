using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public GameObject enemyPrefab;

    public float TimeBetweenSpawns = 5f;
    public int MaxEnemies = 1;
    private int enemiesCount;

    // Use this for initialization
    void Start () {
        StartCoroutine(Spawn());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Spawn()
    {
        while (true)
        {
            if (MaxEnemies < 0 || enemiesCount <= MaxEnemies)
            {
                Vector2 pos = Random.insideUnitCircle * 7;
                Instantiate(enemyPrefab, pos, Quaternion.identity);
                enemiesCount++;
            }
            yield return new WaitForSeconds(TimeBetweenSpawns);
        }
    }
}
