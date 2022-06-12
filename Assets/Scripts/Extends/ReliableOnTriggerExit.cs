using System.Collections.Generic;
using UnityEngine;

public class ReliableOnTriggerExit : MonoBehaviour
{
    public delegate void _OnTriggerExit(Collider c);

    private Collider _thisCollider;
    private bool _ignoreNotifyTriggerExit = false;

    private Dictionary<GameObject, _OnTriggerExit> _waitingForOnTriggerExit = new Dictionary<GameObject, _OnTriggerExit>();

    private void OnDisable()
    {
        if (gameObject.activeInHierarchy == false)
            CallCallbacks();
    }

    private void Update()
    {
        if (_thisCollider == null)
        {
            CallCallbacks();

            Component.Destroy(this);
        }
        else if (_thisCollider.enabled == false)
        {
            CallCallbacks();
        }
    }

    public static void NotifyTriggerEnter(Collider c, GameObject caller, _OnTriggerExit onTriggerExit)
    {
        ReliableOnTriggerExit thisComponent = null;
        ReliableOnTriggerExit[] ftncs = c.gameObject.GetComponents<ReliableOnTriggerExit>();

        foreach (ReliableOnTriggerExit ftnc in ftncs)
        {
            if (ftnc._thisCollider == c)
            {
                thisComponent = ftnc;
                break;
            }
        }

        if (thisComponent == null)
        {
            thisComponent = c.gameObject.AddComponent<ReliableOnTriggerExit>();
            thisComponent._thisCollider = c;
        }

        if (thisComponent._waitingForOnTriggerExit.ContainsKey(caller) == false)
        {
            thisComponent._waitingForOnTriggerExit.Add(caller, onTriggerExit);
            thisComponent.enabled = true;
        }
        else
        {
            thisComponent._ignoreNotifyTriggerExit = true;
            thisComponent._waitingForOnTriggerExit[caller].Invoke(c);
            thisComponent._ignoreNotifyTriggerExit = false;
        }
    }

    public static void NotifyTriggerExit(Collider c, GameObject caller)
    {
        if (c == null)
            return;

        ReliableOnTriggerExit thisComponent = null;
        ReliableOnTriggerExit[] ftncs = c.gameObject.GetComponents<ReliableOnTriggerExit>();

        foreach (ReliableOnTriggerExit ftnc in ftncs)
        {
            if (ftnc._thisCollider == c)
            {
                thisComponent = ftnc;
                break;
            }
        }

        if (thisComponent != null && thisComponent._ignoreNotifyTriggerExit == false)
        {
            thisComponent._waitingForOnTriggerExit.Remove(caller);
            if (thisComponent._waitingForOnTriggerExit.Count == 0)
            {
                thisComponent.enabled = false;
            }
        }
    }

    private void CallCallbacks()
    {
        _ignoreNotifyTriggerExit = true;
        foreach (var v in _waitingForOnTriggerExit)
        {
            if (v.Key == null)
            {
                continue;
            }

            v.Value.Invoke(_thisCollider);
        }
        _ignoreNotifyTriggerExit = false;
        _waitingForOnTriggerExit.Clear();
        enabled = false;
    }
}