﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator anim;
    public string location = "Field";
    public string unloadScene = "Scene1_Farm";

    public float transitionTime = 1f;

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.name == Tags.Player)
        {
            LoadNextScene();
            SpawnPoint(coll);
        }
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadScene(location));
    }

    IEnumerator LoadScene(string Location)
    {
        //Play fade animation.
        anim.SetTrigger("Start");

        //Wait
        yield return new WaitForSeconds(transitionTime);

        //Additive Scene Loading
        SceneManager.LoadSceneAsync(Location, LoadSceneMode.Additive);

        //Unload Scene
        SceneManager.UnloadSceneAsync(unloadScene);
    }
     
     public void SpawnPoint(Collider2D coll)
     {
         //Teleport player to spawn game object.
        GameObject spawnPoint = GameObject.Find("SpawnPoint");
        
        coll.transform.position = spawnPoint.transform.position;

     }
}
