using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Will restore an Animator to it's full state whenever it's game object is disabled or enabled.
/// </summary>
/// <note>
/// Requires the animator to be BENEATH this component in the component heirarchy on the object.
/// Nothing special required for script execution order.
/// </note>
[RequireComponent(typeof(Animator))]
public class AnimationSaver : MonoBehaviour
{
    private Animator _animator;

    private readonly Dictionary<int, AnimatorStateInfo> _stateInfoByLayer 
                                        = new Dictionary<int, AnimatorStateInfo>();
    private readonly Dictionary<AnimatorControllerParameter, object> _parameterValues 
                                        = new Dictionary<AnimatorControllerParameter, object>();

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (!_animator)
        {
            return;
        }

        foreach (KeyValuePair<int, AnimatorStateInfo> layerAndStateInfo in _stateInfoByLayer)
        {
            int layer = layerAndStateInfo.Key;
            AnimatorStateInfo stateInfo = layerAndStateInfo.Value;
            _animator.Play(stateInfo.shortNameHash, layer, stateInfo.normalizedTime);
        }

        foreach (KeyValuePair<AnimatorControllerParameter, object> parameterAndValue in _parameterValues)
        {
            AnimatorControllerParameter parameter = parameterAndValue.Key;
            object value = parameterAndValue.Value;
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Bool:
                    _animator.SetBool(parameter.name, (bool)value);
                    break;
                case AnimatorControllerParameterType.Float:
                    _animator.SetFloat(parameter.name, (float)value);
                    break;
                case AnimatorControllerParameterType.Int:
                    _animator.SetInteger(parameter.name, (int)value);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    if ((bool)value)
                    {
                        _animator.SetTrigger(parameter.name);
                    }

                    break;
                default:
                    continue;
            }
        }

        ResetInternalState();
    }

    private void OnDisable()
    {
        ResetInternalState();
        if (!_animator)
        {
            return;
        }

        for (int i = 0; i < _animator.layerCount; ++i)
        {
            _stateInfoByLayer[i] = _animator.GetCurrentAnimatorStateInfo(i);
        }

        foreach (AnimatorControllerParameter parameter in _animator.parameters)
        {
            object value;
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Bool:
                case AnimatorControllerParameterType.Trigger:
                    value = _animator.GetBool(parameter.name);
                    break;
                case AnimatorControllerParameterType.Float:
                    value = _animator.GetFloat(parameter.name);
                    break;
                case AnimatorControllerParameterType.Int:
                    value = _animator.GetInteger(parameter.name);
                    break;
                default:
                    continue;
            }

            _parameterValues[parameter] = value;
        }
    }

    private void ResetInternalState()
    {
        _stateInfoByLayer.Clear();
        _parameterValues.Clear();
    }
}
