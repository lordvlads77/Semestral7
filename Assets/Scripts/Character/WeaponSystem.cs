using Controllers;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Character
{
    public class WeaponSystem : Singleton<WeaponSystem>
    {
        [FormerlySerializedAs("OneHandedWeapon")]
        [FormerlySerializedAs("sword")]
        [Header("Sword GameObject Variable")] 
        [FormerlySerializedAs("_sword")] [SerializeField]
        private GameObject oneHandedWeapon = default;
        [FormerlySerializedAs("TwoHandedWeapon")]
        [FormerlySerializedAs("halberd")]
        [Header("Halberd GameObject Variable")]
        [FormerlySerializedAs("polearm")] [FormerlySerializedAs("_polearm")] [SerializeField]
        private GameObject twoHandedWeapon = default;
        [Header("Two Handed Weapon in Hands")]
        [SerializeField] private GameObject twoHandedHand = default;
        [FormerlySerializedAs("_isWeaponInUse")] [SerializeField] private bool isWeaponInUse = false;
        [FormerlySerializedAs("_player")] [SerializeField] private LivingEntity player;
        [FormerlySerializedAs("_enemy")] [SerializeField] private GameObject enemy;
        [SerializeField] private bool areHandsInUse = false;

        [FormerlySerializedAs("_animator")]
        [Header("Animator Reference")]
        [SerializeField] private Animator animator = default;
        
        [Header("State Manager Ref")]
        [SerializeField] private StateManager stateManager = default;

        protected override void OnAwake()
        {
            if (player == null)
            {
                player = GameObject.FindWithTag("Player").GetComponent<LivingEntity>();
            }
        }

        public void Unarmed()
        {
            areHandsInUse = true;
        }

        public void WithdrawOneHandedWeapon()
        {
            if (isWeaponInUse == false) {
                isWeaponInUse = true;
                AnimationController.Instance.OneHandWeaponWithdraw(animator);
                AnimationController.Instance.WeaponType(animator, 1);
                //Add VFX Calling Here if applicable
                //Add SFX Calling Here
                oneHandedWeapon.SetActive(true);
            }
            else EDebug.Log("You already have a weapon equipped");
            if (twoHandedHand.activeInHierarchy) {
                SheathTwoHandedWeapon();
                WithdrawOneHandedWeapon();
                EDebug.Log("Switching to one handed weapon");
            }
        }
        
        public void SheathOneHandedWeapon()
        {
            isWeaponInUse = true;
            if (isWeaponInUse) {
                isWeaponInUse = false;
                //Add VFX Calling Here if applicable
                //Add SFX Calling Here
                AnimationController.Instance.WeaponType(animator, 0);
                if (!oneHandedWeapon.activeInHierarchy) EDebug.Log("You already Sheathed your weapon");
                else
                {
                    AnimationController.Instance.OneHandWeaponSheath(animator);
                    oneHandedWeapon.SetActive(false);
                }
            }
            else EDebug.Log("You already sheathed your weapon");
        }

        public void WithdrawTwoHandedWeapon()
        {
            if (isWeaponInUse == false) {
                isWeaponInUse = true;
                AnimationController.Instance.TwoHandsWeaponWithdraw(animator);
                AnimationController.Instance.WeaponType(animator, 2);
                //Add VFX Calling Here if applicable
                //Add SFX Calling Here
                twoHandedWeapon.SetActive(false);
                twoHandedHand.SetActive(true);
            }
            else EDebug.Log("You already have a weapon equipped");
            if (oneHandedWeapon.activeInHierarchy) {
                SheathOneHandedWeapon();
                WithdrawTwoHandedWeapon();
                EDebug.Log("Switching to two handed weapon");
            }
        }
        
        public void SheathTwoHandedWeapon()
        {
            if (isWeaponInUse) {
                isWeaponInUse = false;
                AnimationController.Instance.TwoHandsWeaponSheath(animator);
                AnimationController.Instance.WeaponType(animator, 0);
                //Add VFX Calling Here if applicable
                //Add SFX Calling Here
                twoHandedHand.SetActive(false);
                twoHandedWeapon.SetActive(true);
            }
            else
                EDebug.Log("You already sheathed your weapon");
        }

        public void LoweringHands()
        {
            if (isWeaponInUse == false & areHandsInUse) AnimationController.Instance.LowerHands(animator);
        }

        public void HandsArebeingUsed()
        {
            areHandsInUse = true;
            if (areHandsInUse) AnimationController.Instance.HandsUsing(animator);
        }

        public void Attack()
        {
            AnimationController.Instance.WeaponType(animator, 0);
            CombatUtils.Attack(player, enemy.GetComponent<LivingEntity>());
            if (player.GetComponent<LivingEntity>().Weapon == WeaponType.Unarmed ) {
                AnimationController.Instance.UsingHands(animator);
                stateManager.EnterFightingState(FightingState.UnarmedFighting, player.GetComponent<MovementManager>());
                HandsArebeingUsed();
                areHandsInUse = true;
                EDebug.Log("hAnDs ArR iN uSe");
            }
        }
        
        

    }
}
