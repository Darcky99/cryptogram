using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Fun2Play: SO Architecture")]
    [Tooltip("Get the value from a Scriptable Bool Object.")]
    public class ScriptableBoolGetValue : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ScriptableBool))]
        [Tooltip("Select the Scriptable Bool Object")]
        public FsmObject scriptableBool;

        [Tooltip("Store the value in a bool variable")]
        [UIHint(UIHint.Variable)]
        public FsmBool value;

        private ScriptableBool _scriptableBool;

        public override void Reset()
        {
            scriptableBool = null;
            value = null;
        }

        public override void OnEnter()
        {
            DoGetScriptableBoolValue();
            Finish();
        }

        private void DoGetScriptableBoolValue()
        {
            _scriptableBool = scriptableBool.Value as ScriptableBool;
            value.Value = _scriptableBool.value;
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return scriptableBool + ": Get Value";
        }
#endif
    }
}
