using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;

public class SimpleConditionalActions : MonoBehaviour
{
    // Add this component to an emtpy gameobject and set a reference tag, when all the tagged objects are disabled,
    // feedbacks and actions will be played before this script disable itself.
    // v1.0 / Muppo (2020)

    // If tag is empty, this script will disable itself.
    public string referenceTag;
    public bool ConditionFulfilled = false;
    public float FinishDelay = 0f;
    public MMFeedbacks FinishFeedback;
    public UnityEvent FinishAction;

     /// TEST ON INSPECTOR
    [MMInspectorButton("PlayEffects")]
        public bool TestButton;

    private bool Done;

    //
    public virtual void Start()
    {
        if (string.IsNullOrEmpty(referenceTag))
        {
            DisableThis();
        }
        Done = false;
    }

    //
    public virtual void LateUpdate()
    {     
        if (Done)
        {
            return;
        }

        if ((GameObject.FindGameObjectsWithTag(referenceTag).Length == 0) || ConditionFulfilled)
        {
            FinishSequence();
            Done = true;
        }
    }

    //
    public virtual void FinishSequence()
    {
        if (FinishDelay > 0) {
            StartCoroutine(PlayDelay());
        } else {
            PlayEffects();
        }
    }

    //
    public virtual IEnumerator PlayDelay()
    {
        yield return new WaitForSeconds (FinishDelay);
        PlayEffects();
    }

    //
    public virtual void PlayEffects()
    {
        if (FinishFeedback != null)
            FinishFeedback?.PlayFeedbacks();

        if (FinishAction != null)
            FinishAction.Invoke();

        DisableThis();
    }

    //
    public virtual void DisableThis()
    {
        this.enabled = false;
    }
}