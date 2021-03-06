﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
/// <summary>
/// Levels02.
/// John G. Toland 3/10/17
/// Richard Oneal 04/06/2017
/// Gerard Fierro 04/13/2017
/// This script is intended to use on level 02.
/// This script controls the progress of the level.
/// including the boss fight and when to spawn the boss.
/// Seperating the progress of the level and the over all gamecontroller allows for 
/// better readability. 
/// The addition of this script also allows form making all variables in GameController private, 
/// acessing these varibles are now done with getters and setters.
/// </summary>
public class Level_02 : MonoBehaviour
{
	//all level variables
	private SceneLoaderHandler SLH;
	private GameController gc;
	public GameObject[] hazards;
	public GameObject FinalEnemy;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public int numOfWavesInLvl;
	private int spawnWaveCount;
	private bool bossWaveStarts;
	public int BossHazardCount;
	private PauseNavGUI pB;
	private bool NEED_NEW_LVL = true;
	private Lvl05BossHealth healthScript = null;
	private bool BOSS_IS_DEAD = false;


	// Use this for initialization
	void Start()
	{
		GameObject SLHo = GameObject.Find ("JOHNS_NAV_GUI_MOBILE");
		SLH = SLHo.GetComponent<SceneLoaderHandler> ();

		GameObject pauseNavGUI = GameObject.FindGameObjectWithTag ("PauseBtn");
		if(pauseNavGUI != null){
			pB = pauseNavGUI.GetComponent<PauseNavGUI> ();
		}
		//currentScene = SceneManager.GetActiveScene ();
		//get instance of gameController for access to game progress fucntions within your level
		GameObject gcObject = GameObject.FindGameObjectWithTag("GameController");
		if (gcObject != null)
		{
			gc = gcObject.GetComponent<GameController>();
		}
	}

	// Update is called once per frame, this is were you will check to see if it is time for your boss wave to spawn.
	void Update()
	{

		///spawning the boss for level_02
		if (bossWaveStarts)
		{
			StartCoroutine(SpawnBosslvl02());
			healthScript = FinalEnemy.GetComponent<Lvl05BossHealth> ();
			GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().setCAN_FIRE (false);
			bossWaveStarts = false;
		}

		if(!gc.isGameOver() && NEED_NEW_LVL && !pB.GameIsPaused () && healthScript) {

			if (BOSS_IS_DEAD) {
				print (healthScript.getHealth ());
				NEED_NEW_LVL = false;
				StartCoroutine(LoadNewLvl());
				BOSS_IS_DEAD = false;
			}

		}
	}

	public void setBOSS_IS_DEAD(bool isIt){
		BOSS_IS_DEAD = isIt;
	}

	#region methodsToStartCoroutines
	/// <summary>
	/// Starts the generic lvl Coroutine.
	/// </summary>
	public void StartLvlTwo()
	{
		StartCoroutine(SpawnWaves());
	}
	#endregion

	/// <summary>
	/// Checks the player progress in lvl.
	/// </summary>
	/// <returns><c>true</c>, if player progress in lvl was checked, <c>false</c> otherwise.</returns>
	/// <param name="isRegularWave">If set to <c>true</c> is regular wave.</param>
	public bool checkPlayerProgressInLvl(bool isRegularWave)
	{
		if (isRegularWave)
		{
			spawnWaveCount++;
			print("wave count = " + spawnWaveCount);
			if (gc.isGameOver())
			{
				gc.setRestart(true);
				return false;
			}
			else if (gc.isPlayerDead())
			{
				spawnWaveCount = 0;
				print("inside player is dead");
				gc.ReSpawn();
				gc.setPlayerDead(false);
				return true;
			}
			else if (spawnWaveCount == numOfWavesInLvl && !gc.isGameOver())
			{
				enteringBossWave(gc.getLvlCount());
				return false;
			}
			else
			{
				return true;
			}
		}
		else
		{
			spawnWaveCount++;
			print("wave count = " + spawnWaveCount);
			if (gc.isGameOver())
			{
				gc.setRestart(true);
				return false;
			}
			else if (gc.isPlayerDead())
			{
				spawnWaveCount = numOfWavesInLvl;
				gc.ReSpawn();
				gc.setPlayerDead(false);
				return true;
			}
			else
			{
				return true;
			}
		}
	}

	IEnumerator LoadNewLvl(){
		yield return new WaitForSeconds (3);
		if (gc.getLvlCount() >= 5){
			gc.levelCompleted();
			gc.resetLvlCount();
			yield return new WaitForSeconds (3);
			SLH.LoadNewSceneInt (1);
		}else{
			gc.levelCompleted ();
			yield return new WaitForSeconds (3);
			//level count + 3 (compensation for the login scene and player seleciton scene)
			SLH.LoadNewSceneInt (gc.getLvlCount ()+3);
		}
	}
	/// <summary>
	/// Enterings the boss wave.
	/// </summary>
	/// <param name="lvlCount">Lvl count.</param>
	public void enteringBossWave(int lvlCount)
	{
		switch (lvlCount)
		{
		case 1:
			bossWaveStarts = true;
			break;
		case 2:
			//level 2 boss wave case
			bossWaveStarts = true;                        ///used for testing
			break;
		case 3:
			//level 3 boss wave case
			bossWaveStarts = true;                        ///used for testing
			break;
		case 4:
			//level 4 boss wave case
			bossWaveStarts = true;                        ///used for testing
			break;
		case 5:
			//level 5 boss wave case
			bossWaveStarts = true;                        ///used for testing
			break;
		default:
			//nothing to do here
			break;
		}
	}

	#region GenericLvl waveSpawning
	/// <summary>
	/// Spawns the waves.
	/// </summary>
	/// <returns>The waves.</returns>
	IEnumerator SpawnWaves()
	{
		///we must wait for 3 seconds for the database to load and update the new information for each level.
		yield return new WaitForSeconds(3f);
		gc.setGameOverText(true);
		yield return new WaitForSeconds(startWait);
		gc.setGameOverText(false);
		while (true)
		{
			for (int i = 0; i < hazardCount; i++)
			{
				GameObject hazard = hazards[Random.Range(0, hazards.Length)];//Picks random hazard from hazards array
				Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate(hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds(spawnWait);
			}
			yield return new WaitForSeconds(waveWait);
			if (!checkPlayerProgressInLvl(true))
			{
				break;
			}
		}
	}

	/// <summary>
	/// Spawns the boss wave level 02.
	/// </summary>
	/// <returns>The boss wave level 02.</returns>
	IEnumerator SpawnBosslvl02()
	{
		yield return new WaitForSeconds(startWait);
		//while (true)
		//{
		//GameObject bossObject = GameObject.FindGameObjectWithTag("BossLvl_02");
		GameObject boss = FinalEnemy;
		Vector3 spawnPosition = new Vector3(0, 0, 25);
		Quaternion spawnRotation = Quaternion.identity;
		Instantiate(boss, spawnPosition, spawnRotation);
		yield return new WaitForSeconds(0.1f);

		yield return new WaitForSeconds(waveWait);
		checkPlayerProgressInLvl (true);

		//spawnWaveCount++;
		print("wave count inside bosswave = " + spawnWaveCount);



	}


	#endregion
}
//finito