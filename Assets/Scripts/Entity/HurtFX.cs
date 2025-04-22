using System.Collections;
using UnityEngine;
using Utils;

namespace Entity
{
    public class HurtFX : MonoBehaviour
    {
        public void Hit(HurtFXVars fxVars)
        {
            if (fxVars.DamageFXCoroutine != null)
                StopCoroutine(fxVars.DamageFXCoroutine);
            fxVars.DamageFXCoroutine = StartCoroutine(DamageFX(fxVars));
        }
        
        private IEnumerator DamageFX(HurtFXVars fxVars)
        {
            Material hitMaterial;
            if (fxVars.hitMaterials == null || fxVars.hitMaterials.Length == 0) {
                hitMaterial = Resources.Load<Material>("Materials/Red");
                if (hitMaterial == null) {
                    EDebug.LogError("Material 'Red' not found in 'Assets/Resources/Materials'");
                    hitMaterial = new Material(Shader.Find("Unlit/Color")) { color = Color.red }; 
                } 
            }
            else hitMaterial = fxVars.hitMaterials[Random.Range(0, fxVars.hitMaterials.Length)];
            Material[] newMaterials = new Material[fxVars.ogMaterials.Length];
            for (int i = 0; i < fxVars.ogMaterials.Length; i++)
            { newMaterials[i] = hitMaterial; }
            float blinkInterval = fxVars.animTime / (fxVars.blinks * 2);
            for (int i = 0; i < fxVars.blinks; i++)
            {
                fxVars.renderer.materials = newMaterials;
                yield return new WaitForSeconds(blinkInterval);

                fxVars.renderer.materials = fxVars.ogMaterials;
                yield return new WaitForSeconds(blinkInterval);
            }
            fxVars.renderer.materials = fxVars.ogMaterials;
        }
        
    }
}
