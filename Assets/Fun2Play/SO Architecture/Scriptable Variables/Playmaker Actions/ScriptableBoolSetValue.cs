using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Fun2Play: SO Architecture")]
    [Tooltip("Set the value of a Scriptable Bool.")]
    public class ScriptableBoolSetValue : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ScriptableBool))]
        [Tooltip("Select the Scriptable Bool Object")]
        public FsmObject scriptableBool;

        [Tooltip("The bool value you want the Scriptable Bool to be set to.")]
        public FsmBool value;

        private ScriptableBool _scriptableBool;

        public override void Reset()
        {
            scriptableBool = null;
            value = false;
        }

        public override void OnEnter()
        {
            DoSetScriptableBoolValue();
            Finish();
        }

        private void DoSetScriptableBoolValue()
        {
            _scriptableBool = scriptableBool.Value as ScriptableBool;
            _scriptableBool.SetValue(value.Value);
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return scriptableBool + ": Set Value " + value;
        }
#endif
    }
}
