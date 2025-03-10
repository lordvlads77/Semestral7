using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Character
{
    public class WeaponSystem : Singleton<WeaponSystem>
    {
        [Header("Sword GameObject Variable")] 
        [FormerlySerializedAs("_sword")] [SerializeField]
        private GameObject sword = default;
        [Header("Halberd GameObject Variable")]
        [FormerlySerializedAs("polearm")] [FormerlySerializedAs("_polearm")] [SerializeField]
        private GameObject halberd = default;
    
        public void Unarmed()
        {
            sword.SetActive(false);
            halberd.SetActive(false);
        }

        public void UsingSword()
        {
            //Add Animation Calling Here
            //Add VFX Calling Here if applicable
            //Add SFX Calling Here
            sword.SetActive(true);
            halberd.SetActive(false);
        }

        public void UsingHalberd()
        {
            //Add Animation Calling Here
            //Add VFX Calling Here if applicable
            //Add SFX Calling Here
            halberd.SetActive(true);
            sword.SetActive(false);
        }

    }
}
