using UnityEngine;
using Utils;

namespace Character
{
    public class TestingWeapons : MonoBehaviour
    {
        private Input.Actions _input;
        
        private void Awake()
        {
            _input = Input.Actions.Instance;
            if (_input == null) _input = gameObject.GetComponent<Input.Actions>();
            if (_input == null) _input = gameObject.AddComponent<Input.Actions>();
        }

        private void Cosa(LivingEntity entity)
        {
            if (entity.Weapon == WeaponType.Unarmed) return;
        }

        /*private void Update() Got rid of this for now... 
        {                       I'm moving the weapon types so that they're compatible with all Entities
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
        }*/
    }
}
