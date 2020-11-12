using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WorldLogic : MonoBehaviour
{


    [Header("Loading logic")] [SerializeField]
    private Slider _loadSlider;

    [SerializeField] private int _totalSteps;
    [SerializeField] private int _finishedSteps;
    [SerializeField] private int _totalPlacedBlocks;
    [SerializeField] private TMP_Text _totalStepsUI;
    [SerializeField] private TMP_Text _finishedStepsUI;
    [SerializeField] private TMP_Text _totalPlacedBlocksUI;

    [Header("UI")] [SerializeField] private TMP_InputField _walkerAmountInput;
    [SerializeField] private TMP_InputField _worldSizeX;
    [SerializeField] private TMP_InputField _worldSizeY;
    [SerializeField] private TextMeshProUGUI _Count;
    [SerializeField] private Toggle _useIenumaratorToggle;

    [Header("World Stuff")] 
    [SerializeField] private int _walkerAmount;
    [SerializeField] private GameObject _walkerPrefab;
    [SerializeField] private Vector3 _worldSize;
    [SerializeField] private int _maxBlocks;
    [SerializeField] private List<Biomes> _biomeCubes;
    
    public List<Biomes> BiomeCubes => _biomeCubes;
    
    [Header("Player")]


    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _oceanPrefab;
    [SerializeField] private float _renderDistance;
    
    
    private GameObject _parent;

    private bool _useIenumarator;

    public Dictionary<Vector3, BiomeTypes> _positionsDictionary = new Dictionary<Vector3, BiomeTypes>();

    private List<GameObject> _walkers = new List<GameObject>();

    private bool _finished;
    
    private Vector3 _startPos;

    public int Done;
    
    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;
    private int _height;
    private int _width;
    private void Start()
    {
        rend = GetComponent<Renderer>();
        _width =  Mathf.RoundToInt(_worldSize.x);
        _height = Mathf.RoundToInt(_worldSize.y);
        noiseTex = new Texture2D(_width, _height);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;
        rend.material.mainTextureOffset = new Vector2(0.02f, 0f);
    }

    public void Generate()
    {
        StopAllCoroutines();
        
        for (int i = 0; i < _walkers.Count; i++)
        {
            _walkers[i].GetComponent<Walker>().StopAllCoroutines();
        }
        for (int i = 0; i < _walkers.Count; i++)
        {
            StopCoroutine(_walkers[i].GetComponent<Walker>().WalkIenum());
            Destroy(_walkers[i]);
        }
        _positionsDictionary.Clear();
        _walkers.Clear();
        Destroy(_parent);

        if (_useIenumaratorToggle.isOn)
            _useIenumarator = true;
        else
            _useIenumarator = false;

        int.TryParse(_walkerAmountInput.text, out _walkerAmount);
        int _x, _y;
        int.TryParse(_worldSizeX.text, out _x);
        int.TryParse(_worldSizeY.text, out _y);

        // Camera.main.transform.position = new Vector3(_x - 50, _y - 50, -10);
        // Camera.main.GetComponent<Camera>().orthographicSize = (_x + 50f);
        
        _worldSize = new Vector3(_x, _y);
        Done = 0;

        _finishedSteps = 0;
        _finishedStepsUI.text = _finishedSteps.ToString();

        _totalSteps = 0;
        _totalStepsUI.text = _totalSteps.ToString();
        
        _totalPlacedBlocks = 0;
        _totalPlacedBlocksUI.text = _totalPlacedBlocks.ToString();


        StartCoroutine(GenerateWalkers());
    }
    public void StepPlus()
    {
        _finishedSteps++;

        _finishedStepsUI.text = _finishedSteps.ToString();
    }
    public void BlockPlus()
    {
        _totalPlacedBlocks++;

        _totalPlacedBlocksUI.text = _totalPlacedBlocks.ToString();
    }
    public void CheckForStop()
    {
        if (_totalPlacedBlocks > _maxBlocks)
        {
            StopAllCoroutines();
            for (int i = 0; i < _walkers.Count; i++)
            {
                StopCoroutine(_walkers[i].GetComponent<Walker>().WalkIenum());
                Destroy(_walkers[i]);
            }
            _finished = true;
            _walkers.Clear();
            DrawTexture();
        }
    }
    private Biomes PickCube(int _mostExpectedBiome, int _secondMostExcpectedBiome) {
        
        Biomes _biomeType = new Biomes();
        
        int _randomNumber = UnityEngine.Random.Range(0, _biomeCubes.Count + 10);
        
        if (_randomNumber == 0 || _randomNumber == 1 || _randomNumber == 2 || _randomNumber == 3 || _randomNumber == 4  
            || _randomNumber == 5 || _randomNumber == 6 || _randomNumber == 7  || _randomNumber == 8 || _randomNumber == 9 
            || _randomNumber == 10)
            _biomeType = _biomeCubes[_mostExpectedBiome];
        else if(_randomNumber == 11 || _randomNumber == 12 || _randomNumber == 13 || _randomNumber == 14)
            _biomeType = _biomeCubes[_secondMostExcpectedBiome];
        else
        {
            int _index = Random.Range(0, _biomeCubes.Count);
            _biomeType = _biomeCubes[_index];
        }
        return _biomeType;
    }
    
    IEnumerator GenerateWalkers()
    {
        _parent = new GameObject();

        for (int i = 0; i < _walkerAmount; i++)
        {
            float _posX;
            float _posY;

            _posX = Random.Range(20, _worldSize.x - 4);
            _posY = Random.Range(20, _worldSize.y - 4);

            Vector3 _pos = new Vector3(Mathf.RoundToInt(_posX), Mathf.RoundToInt(_posY), 0);
            GameObject _walker = Instantiate(_walkerPrefab, _pos, Quaternion.identity);
            _walkers.Add(_walker);
            int _walkerPlace = _walkers.Count - 1;
            Walker _walker_logic = _walkers[_walkerPlace].AddComponent<Walker>();

            int _maxStep = Random.Range(200, 10000);
            //Debug.Log(_maxStep);
            _walker_logic.StartPos = _pos;
            
            //Choose a random biome 
            if(_posY > 0)
                _walker_logic.Cube = PickCube(4, 5);
            if(_posY > 83)
                _walker_logic.Cube = PickCube(2, 3);
            if(_posY > 166)
                _walker_logic.Cube = PickCube(0, 1);
            
            _walker_logic.Parent = _parent;
            _walker_logic.WorldSize = _worldSize;
            _walker_logic.MaxX = Random.Range(10, 20);
            _walker_logic.MaxY = Random.Range(10, 20);
            _walker_logic.MaxStep = Mathf.RoundToInt(_maxStep);
            _walker_logic.WorldManager = this;
            _walker_logic.name = "Walker " + i;
            _totalSteps = _totalSteps + _maxStep;
            // Debug.Log(_totalSteps);
            _totalStepsUI.text = _totalSteps.ToString();

            if (_useIenumarator)
                StartCoroutine(_walker_logic.WalkIenum());
            else
                _walker_logic.WalkFast();
            yield return  new WaitForSeconds(0.001f);
        }
    }

    #region  Draw the world on a texture

    private void DrawTexture()
    {
        float y = 0.0f;

        while (y < _height)
        {
            float x = 0.0F;

            while (x < _width)
            {
                int _amount = (int) y * _width + (int) x;
                //Debug.Log(_amount);
                pix[_amount] = GetColor(new Vector3(x, y, 0));
                x++;
            }
            y++;
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();

    }
    private Color GetColor(Vector3 _pos)
    {
        Color _color = new Color();
        
        BiomeTypes _biomeType;
        if (_positionsDictionary.TryGetValue(_pos, out _biomeType))
        {
            switch (_biomeType)
            {
                case BiomeTypes.GRASS:
                    _color = Color.green;
                    break;
                case BiomeTypes.FOREST:
                    _color = new Color(74, 43, 5, 255);
                    break;
                case BiomeTypes.SAND:
                    _color = Color.yellow;
                    break;
                case BiomeTypes.WATER:
                    _color = Color.blue;
                    break;
                case BiomeTypes.SNOW:
                    _color = Color.white;
                    break;
                case BiomeTypes.SWAMP:
                    _color = new Color(111, 139, 66, 255);
                    break;
            }
        }
        else
        {
            _color = Color.blue;
            return _color;
        }
        return _color;
    }

    #endregion

    // #region Player
    //
    // private void CreatePlayer()
    // {
    //     float _spawnPosX = Random.Range(0, _worldSize.x - 10);
    //     float _spawnPosY = Random.Range(0, _worldSize.y - 10);
    //     GameObject _player = Instantiate(_playerPrefab, new Vector3(_spawnPosX, _spawnPosY, 0), Quaternion.identity);
    //     var _playerScr = _player.AddComponent<Player>();
    //     _playerScr.World = this;
    //     _playerScr.RenderDistance = _renderDistance;
    //     _playerScr.OceanPrefab = _oceanPrefab;
    //     _playerScr.GenerateBlocks();
    // }
    //
    // #endregion
    
    

}

[Serializable]
public enum BiomeTypes
{
    GRASS,
    FOREST,
    SAND,
    WATER,
    SNOW,
    SWAMP
}

[Serializable]
public class Biomes
{
    public BiomeTypes BiomeType;
    public GameObject Prefab;
}

