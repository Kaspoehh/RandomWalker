using System.Collections;
using System.Threading;
using UnityEngine;

public class Walker : MonoBehaviour
{
    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    public Texture2D NoiseTex
    {
        set => noiseTex = value;
    }

    public Color[] Pix
    {
        set => pix = value;
    }

    public Renderer Rend
    {
        set => rend = value;
    }

    private Biomes _cube;
    private GameObject _parent;
    public GameObject Parent
    {
        set => _parent = value;
    }

    public Biomes Cube
    {
        set => _cube = value;
    }

    private Vector3 _startPos;
    private Vector3 _worldSize;
    
    public Vector3 StartPos
    {
        set => _startPos = value;
    }
    public Vector3 WorldSize
    {
        set => _worldSize = value;
    }

    private float _maxX;
    private float _maxY;

    public float MaxX
    {
        set => _maxX = value;
    }
    public float MaxY
    {
        set => _maxY = value;
    }

    private int _maxStep;

    public int MaxStep
    {
        get => _maxStep;
        set => _maxStep = value;
    }

    private WorldLogic _worldManager;
    
    public WorldLogic WorldManager
    {
        set => _worldManager = value;
    }
    
    public IEnumerator WalkIenum()
    {
        int i = 0;
        // while (i > _maxStep)
        // {
        //     Destroy(this.gameObject);
        // }
        //
        while (i >= _maxStep || i <= _maxStep)
        {
            //Debug.Log(i + " " + this.gameObject.name);

            Vector3 _targetPosition = GetPosition();
            if (this.transform.position.x >= _startPos.x + _maxX || this.transform.position.y >= _startPos.y + _maxY)
            {
                i++;
                _worldManager.StepPlus();

                if (_targetPosition.x > _worldSize.x || _targetPosition.x < 3 ||
                    _targetPosition.y > _worldSize.y || _targetPosition.y < 3)
                {
                    i++;
                    _worldManager.StepPlus();
                    yield return new WaitForSeconds(0.00001f);
                }

                yield return new WaitForSeconds(0.00001f);
            }

            if (CanMoveToPosition(_targetPosition))
            {
                i++;
                _worldManager.BlockPlus();
                _worldManager.StepPlus();
                _worldManager.CheckForStop();
                this.transform.position = _targetPosition;
                GameObject _block = Instantiate(_cube.Prefab, this.transform.position, Quaternion.identity);
                _block.transform.SetParent(_parent.transform);
                _worldManager._positionsDictionary.Add(this.transform.position, _cube.BiomeType);
                yield return new WaitForSeconds(0.00001f);
            }
            else
            {
                _worldManager.StepPlus();
                i++;
                this.transform.position = _targetPosition;
                yield return new WaitForSeconds(0.00001f);
            }
        }

        //}
        //_worldManager.WalkerFinished();
    }
    
    public void WalkFast()
    {
        for (int i = 0; i < _maxStep; i++)
        {
            
            Vector3 _targetPosition = GetPosition();
            //Debug.Log(_targetPosition);
            if (CanMoveToPosition(_targetPosition))
            {    
                _worldManager.CheckForStop();
                _worldManager.BlockPlus();
                _worldManager.StepPlus();
                this.transform.position = _targetPosition;
                GameObject _block = Instantiate(_cube.Prefab, this.transform.position, Quaternion.identity);
                _block.transform.SetParent(_parent.transform);
                 Debug.Log(this.transform.position);
                _worldManager._positionsDictionary.Add(this.transform.position, _cube.BiomeType);
            }
            else
            {
                _worldManager.StepPlus();
                this.transform.position = _targetPosition;
            }
        }
        //_worldManager.WalkerFinished();
    }

    private Vector3 GetPosition()
    {
        Vector3 targetPosition = this.transform.position + dir();
        return targetPosition;
    }

    private bool CanMoveToPosition(Vector3 _targetPosition)
    {
        bool _canMoveToPosition = false;
        BiomeTypes _go;

        if (!_worldManager._positionsDictionary.TryGetValue(_targetPosition, out _go))
        {
            _canMoveToPosition = true;
        }
        
        return _canMoveToPosition;
    }

    private Vector3 dir()
    {
        //0 up
        //1 down
        //2 right
        //3 left

        int _directionInt = Random.Range(0, 4);
        Vector3 _direction = new Vector3();
        switch (_directionInt)
        {
            case 0:
                _direction = new Vector3(1, 0,0);
                break;
            case 1:
                _direction = new Vector3(-1, 0,0);
                break;
            case 2:
                _direction = new Vector3(0, 1,0);
                break;
            case 3:
                _direction = new Vector3(0, -1,0);
                break;
        }
        return _direction;
    }
}
