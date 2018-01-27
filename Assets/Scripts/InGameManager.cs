using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour {

    #region Attributes

    private static InGameManager instance;
    public static InGameManager GetSingleton
    {
        get { return instance; }
    }

    public int numberPlacesFound = 0;
    #endregion

    #region Initialization
    public void SetSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);

    }

    private void Awake()
    {
        SetSingleton();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        numberPlacesFound = 0;
    }
    #endregion

    #region Core
    public void PlaceFound()
    {
        numberPlacesFound++;
    }
    #endregion
}
