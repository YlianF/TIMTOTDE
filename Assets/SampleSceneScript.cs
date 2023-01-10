using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleSceneScript : MonoBehaviour
{
    public string LevelToLoad;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(LevelToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
