using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameObject _oceanPrefab;
    
    private Vector3 _lastBlock;
    private Vector3 _newBlock;
    
    private List<GameObject> _instantiatedBlocks = new List<GameObject>();
    
    public GameObject OceanPrefab
    {
        set => _oceanPrefab = value;
    }

    private float _renderDistance;

    public float RenderDistance
    {
        set => _renderDistance = value;
    }

    private WorldLogic _world; 

    public WorldLogic World
    {
        set => _world = value;
    }

    private int _instantiateCounter = 0;

    private void Update()
    {
        float _xpos = Mathf.RoundToInt(transform.position.x);
        float _ypos = Mathf.RoundToInt(transform.position.y);
            
        _newBlock = new Vector3(_xpos, _ypos, 0);
        
        if (_lastBlock != _newBlock)
        {
            GenerateBlocks();
        }
    }
    public void GenerateBlocks()
    {
        _lastBlock = _newBlock;
        float _YPos = Mathf.RoundToInt(this.transform.position.y- _renderDistance);
        
        for (int y = 0; y < _renderDistance * 2; y++)
        {
            float _XPos = Mathf.RoundToInt(this.transform.position.x - _renderDistance);
            
            for (int x = 0; x < _renderDistance * 2; x++)
            {
                BiomeTypes _biomeType;
                GameObject _blockToInstantiate = _oceanPrefab;
                
                if (_world._positionsDictionary.TryGetValue(new Vector3(_XPos, _YPos, 0), out _biomeType))
                {
                    switch (_biomeType)
                    {
                        case BiomeTypes.GRASS:
                            _blockToInstantiate = _world.BiomeCubes[0].Prefab;
                            break;
                        case BiomeTypes.FOREST:
                            _blockToInstantiate = _world.BiomeCubes[1].Prefab;
                            break;
                        case BiomeTypes.SAND:
                            _blockToInstantiate = _world.BiomeCubes[2].Prefab;
                            break;
                        case BiomeTypes.WATER:
                            _blockToInstantiate = _world.BiomeCubes[3].Prefab;
                            break;
                        case BiomeTypes.SNOW:
                            _blockToInstantiate = _world.BiomeCubes[4].Prefab;
                            break;
                        case BiomeTypes.SWAMP:
                            _blockToInstantiate = _world.BiomeCubes[5].Prefab;
                            break;
                    }
                }
                _instantiateCounter++;
                GameObject _instantiatedBlock = Instantiate(_blockToInstantiate, new Vector3(_XPos, _YPos, 1), Quaternion.identity);
                _instantiatedBlocks.Add(_instantiatedBlock);
                CheckBlockDistance();
                _XPos++;
            }
            _YPos++;
        }
    }

    private void CheckBlockDistance()
    {
        
        // List<GameObject> _removed = new List<GameObject>();
        //
        // for (int i = 0; i < _instantiatedBlocks.Count; i++)
        // {
        //     float _dist = Vector3.Distance(_instantiatedBlocks[i].transform.position, transform.position);
        //
        //     if (_dist > _renderDistance)
        //     {
        //         Destroy(_instantiatedBlocks[i]);
        //         _removed.Add(_instantiatedBlocks[i]);
        //     }
        // }
        //
        // for (int i = 0; i < _removed.Count; i++)
        // {
        //     for (int j = 0; j < _instantiatedBlocks.Count; j++)
        //     {
        //         if (_removed[i] == _instantiatedBlocks[j])
        //         {
        //             _instantiatedBlocks.Remove(_instantiatedBlocks[j]);
        //         }
        //     }
        // }
    }
    
}
