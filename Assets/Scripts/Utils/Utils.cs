using UnityEngine;

namespace Utils
{
    public enum GameStates : byte
    {
        Joining,
        Playing,
        Paused,
        GameOver,
        Idle,
    }

    public enum CameraTypes
    {
        FreeLook,
        Locked 
    }
    
    public enum WeaponType
    {
        LightSword,
        GreatSword,
        NamePending3,
        NamePending4,
        Unarmed
    }
    
    public static class MathUtils
    {
        public static Vector3[] CanonBasis(Transform trans)
        {
            Vector3 camForward = trans.forward;
            Vector3 camRight = trans.right;
            camForward.y = 0;
            camRight.y = 0;
            return new[] { camForward.normalized, camRight.normalized };
        }
    }

    public static class CombatUtils
    {
        public static void Attack(LivingEntity attacker, LivingEntity target)
        {
            if (attacker == null || target == null) EDebug.LogError("Attacker or target is null! D: ");
            WeaponType weapon = attacker.Weapon;
            
        }
    }
    
}
