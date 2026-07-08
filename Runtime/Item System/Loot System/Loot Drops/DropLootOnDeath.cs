using UnityEngine;

namespace SF.LootModule
{
    using SF.SpawnModule;
    public class DropLootOnDeath : MonoBehaviour
    {
        [SerializeField] private GameObject _lootPrefab;
        [SerializeField] private Health _health;

        private void Start()
        {
            _health ??= GetComponent<Health>();

            if (_health != null)
                _health.DeathHandler += OnDeath;
        }

        private void OnDeath(Health obj)
        {
            if (_lootPrefab == null)
                return;

            Instantiate(_lootPrefab, transform.position, Quaternion.identity);
        }
    }
}