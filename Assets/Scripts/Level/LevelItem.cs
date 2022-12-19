using UnityEngine;

// class controls level item's collisions & upper/lower items of current item

namespace Level
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private bool _isStartItem = false;
        [SerializeField] private LevelItem _upperItem;
        [SerializeField] private LevelItem _lowerItem;

        public bool IsStartItem => _isStartItem;

        public LevelItem UpperItem
        {
            get => _upperItem;
            set
            {
                if (value == _upperItem)
                {
                    return;
                }
                _upperItem = value;
            }
        }

        public LevelItem LowerItem
        {
            get => _lowerItem;
            set
            {
                if (value == _lowerItem)
                {
                    return;
                }
                _lowerItem = value;
            }
        }

        public delegate void OnCollisionEnteredHandler(LevelItem item);
        public event OnCollisionEnteredHandler OnTriggerEntered;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEntered?.Invoke(this);
        }
    }
}
