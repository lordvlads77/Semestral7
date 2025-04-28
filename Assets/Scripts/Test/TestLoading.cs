using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoading : MonoBehaviour
{
    [Tooltip("☑ = load the level selected\n☐ = DON'T load the level selected")]
    public bool shouldLoad = false;

    [Tooltip("Attach the Loading script here for this to work")]
    [SerializeField] private Loading loadingScript;

    void Start()
    {
        Debug.Assert(loadingScript != null,$"Attach script type of {nameof(Loading)} for this to work",this);
    }

    void Update()
    {

        if (shouldLoad)
        {
            shouldLoad = false;
            loadingScript.LoadScene();
        }
        
    }
}
