using System.Collections.Generic;
using UnityEngine;

// class controlls level items spawning and destroying

namespace Level
{
    public class LevelBuilder : MonoBehaviour
    {
        [SerializeField] private Transform _levelSpawnPoint;
        [SerializeField] private List<LevelItem> _levelItemPrefabs = new List<LevelItem>();
        [SerializeField][Range(3, 5)] private int _maxItemsSpawned = 3;

        public delegate void OnSpawnListChangedHandler(List<LevelItem> spawnedItems);

        private List<LevelItem> _spawnedItems = new List<LevelItem>();
        private float _distanceBetweenParts = 35.8f;

        private void OnValidate()
        {
            if(_levelSpawnPoint.childCount < _maxItemsSpawned - 1)
            {
                Debug.LogError($"Spawnn point should have at least {_maxItemsSpawned - 1} start items.");
            }
        }

        private void Awake()
        {
            Init();
        }

        public void SpawnNewItem(LevelItem currentItem, bool isUpperOfCurrent)
        {
            RemoveUselessItem(currentItem, isUpperOfCurrent);
            SpawnNewLevelItem(currentItem, isUpperOfCurrent);
        }

        private void SpawnNewLevelItem(LevelItem currentItem, bool isUpperOfCurrent)
        {
            var newItem = Instantiate(_levelItemPrefabs[Random.Range(0, _levelItemPrefabs.Count - 1)], _levelSpawnPoint);
            var offset = isUpperOfCurrent ? Vector3.up * _distanceBetweenParts : -Vector3.up * _distanceBetweenParts;
            newItem.transform.position = currentItem.transform.position + offset;
            _spawnedItems.Add(newItem);

            if (isUpperOfCurrent)
            {
                currentItem.UpperItem = newItem;
                newItem.LowerItem = currentItem;
            }
            else
            {
                currentItem.LowerItem = newItem;
                newItem.UpperItem = currentItem;
            }

            ChangeSubscribes();
        }

        private void RemoveUselessItem(LevelItem currentItem, bool lookForUpper)
        {
            if (_spawnedItems.Count < _maxItemsSpawned)
            {
                return;
            }

            var uselessItem = currentItem;
            foreach (var item in _spawnedItems)
            {
                if (!lookForUpper && item.LowerItem == currentItem.UpperItem)
                {
                    uselessItem = item;
                }

                if (lookForUpper && item.UpperItem == currentItem.LowerItem)
                {
                    uselessItem = item;
                }
            }

            if (uselessItem == currentItem)
            {
                return;
            }

            Destroy(uselessItem.gameObject);
            _spawnedItems.Remove(uselessItem);

            foreach (var item in _spawnedItems)
            {
                if (!lookForUpper && item.UpperItem == uselessItem)
                {
                    item.UpperItem = null;
                }

                if (lookForUpper && item.LowerItem == uselessItem)
                {
                    item.LowerItem = null;
                }
            }

            ChangeSubscribes();
        }

        private void Init()
        {
            foreach (Transform child in _levelSpawnPoint)
            {
                if (child.TryGetComponent<LevelItem>(out var item))
                {
                    _spawnedItems.Add(item);
                }
            }

            _distanceBetweenParts =  Vector3.Distance(_spawnedItems[0].transform.position, _spawnedItems[1].transform.position);
            ChangeSubscribes();
        }

        private void ChangeSubscribes()
        {
            ResetSpawnList();
            foreach (var item in _spawnedItems)
            {
                item.OnTriggerEntered += OnTriggerEntered;
            }
        }

        private void ResetSpawnList()
        {
            foreach (var item in _spawnedItems)
            {
                item.OnTriggerEntered -= OnTriggerEntered;
            }
        }

        private void OnTriggerEntered(LevelItem item)
        {
            if (!item.IsStartItem && item.LowerItem == null)
            {
                SpawnNewItem(item, false);
            }

            if (item.UpperItem == null)
            {
                SpawnNewItem(item, true);
            }
        }
    }
}
