using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct NewsForPlace
{
    public List<GameObject> newsPrefabs;
}

public enum GameState
{
    InGame, 
    Pause,
    Victory,
    Loose
}

public class InGameManager : MonoBehaviour {

    #region Attributes

    private static InGameManager instance;
    public static InGameManager GetSingleton
    {
        get { return instance; }
    }

    public List<GameObject> newsPopPoint;
    public List<NewsForPlace> newsPrefabs;
    public GameObject profilerView;
    public GameObject victoryPannel;
    public GameObject gameOverPanel;
    public GameObject countdownPanel;
    public GameObject bulletPanel;
    public List<GameObject> bullets;
    public GameObject textIp;
    public Text timerText;
    public int numberPlacesFound = 0;

    public GameState State = GameState.InGame;
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
    public int PlaceFound()
    {
        numberPlacesFound++;
        return numberPlacesFound;
    }

    public NewsForPlace GetNewsForPlace(int placeNumber)
    {
        var realIndex = placeNumber-1;
        if (realIndex >= newsPrefabs.Count)
            throw new System.Exception("numberPlace >= newsPrefabs.Count");

        return newsPrefabs[realIndex];
    }
    #endregion
}
