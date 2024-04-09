using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Fun2Play: SO Architecture")]
    [Tooltip("Set the value of a Scriptable Float.")]
    public class ScriptableFloatSetValue : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ScriptableFloat))]
        [Tooltip("Select the Scriptable Float Object")]
        public ScriptableFloat scriptableFloat;

        [Tooltip("The float value you want the Scriptable Float to be set to.")]
        public FsmFloat value;

        public override void Reset()
        {
            scriptableFloat = null;
            value = 0f;
        }

        public override void OnEnter()
        {
            DoSetScriptableFloatValue();
            Finish();
        }

        private void DoSetScriptableFloatValue()
        {
            scriptableFloat.SetValue(value.Value);
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return scriptableFloat + ": Set Value " + value;
        }
#endif
    }
}
