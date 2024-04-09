using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Fun2Play: SO Architecture")]
    [Tooltip("Get the value from a Scriptable Float Object.")]
    public class ScriptableFloatGetValue : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ScriptableFloat))]
        [Tooltip("Select the Scriptable Float Object")]
        public ScriptableFloat scriptableFloat;

        [Tooltip("Store the value in a float variable")]
        [UIHint(UIHint.Variable)]
        public FsmFloat value;

        public override void Reset()
        {
            scriptableFloat = null;
            value = null;
        }

        public override void OnEnter()
        {
            DoGetScriptableFloatValue();
            Finish();
        }

        private void DoGetScriptableFloatValue()
        {
            value.Value = scriptableFloat.value;
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return scriptableFloat + ": Get Value";
        }
#endif
    }
}
