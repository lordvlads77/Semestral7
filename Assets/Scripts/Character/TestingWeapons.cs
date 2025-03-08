using UnityEngine;

namespace Character
{
    public class TestingWeapons : MonoBehaviour
    {
        private Input.Actions _input;
        
        private void Awake()
        {
            _input = gameObject.GetComponent<Input.Actions>();
            if (!_input) _input = gameObject.AddComponent<Input.Actions>();
        }

        private void Update()
        {
            switch (_input.CurrentWeapon)
            {
                default:
                case Input.WeaponType.Unarmed:
                    WeaponSystem.Instance.Unarmed();
                    break;
                case Input.WeaponType.NamePending1:
                    WeaponSystem.Instance.UsingSword();
                    break;
                case Input.WeaponType.NamePending2:
                    WeaponSystem.Instance.UsingHalberd();
                    break;
            }
        }
    }
}
