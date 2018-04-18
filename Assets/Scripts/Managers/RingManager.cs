using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RingManager : MonoBehaviour {

    public static RingManager instance = null;
    public List<RingBehaviour> Rings = new List<RingBehaviour>();

	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //Let SceneChange subscribe to a scene change so it runs the method.
        SceneManager.activeSceneChanged += SceneChange;
    }
    private void SceneChange(Scene arg0, Scene arg1)
    {
        if (arg1.name == "GameScene")
        {
            InstantiateList();
        }
    }

    private void InstantiateList()
    {
        foreach (GameObject ringObject in GameObject.FindGameObjectsWithTag("Player"))
        {
            Rings.Add(ringObject.GetComponent<RingBehaviour>());
        }
        Physics2D.IgnoreLayerCollision(9, 8, true);
    }

    /// <summary>
    /// Set the parameter (ring) to own layer so it ignores the other rings.
    /// </summary>
    /// <param name="ring"></param>
    /// <param name="ignore"></param>
    public void SetCollisionHandlingOnRing(RingBehaviour ring, bool ignore)
    {
        foreach (RingBehaviour r in Rings)
        {
            if (r != ring)
            {
                ring.gameObject.layer = 9;
            }
        }
    }

    /// <summary>
    /// Reset the parameter (ring) collision handling to collide with other rings
    /// </summary>
    /// <param name="ring"></param>
    public void ResetRingCollisionHandling(RingBehaviour ring)
    {
        foreach (RingBehaviour r in Rings)
        {
            if(r != ring)
            {
                ring.gameObject.layer = 8;
            }
        }
    }

    /// <summary>
    /// Get rings as list, except the one that called a function
    /// </summary>
    /// <param name="ring"></param>
    /// <returns>List<RingBehaviour> -ring that called the function</returns>
    public List<RingBehaviour> GetOtherRings(RingBehaviour ring)
    {
        List<RingBehaviour> rings = new List<RingBehaviour>();
        foreach (RingBehaviour r in Rings)
        {
            if (r != ring)
            {
                rings.Add(r);
            }
        }
        return rings;
    }

    /// <summary>
    /// To clear the list after won game.
    /// </summary>
    public void ResetList()
    {
        Rings.Clear();
    }



}
