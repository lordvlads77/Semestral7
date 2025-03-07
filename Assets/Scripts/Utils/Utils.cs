using UnityEngine;

namespace Utils
{
    public enum GameStates : byte
    {
        Joining,
        Playing,
        Paused,
        GameOver,
    }

    public enum CameraTypes
    {
        FreeLook,
        Locked 
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
    
}
