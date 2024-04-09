using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Fun2Play: SO Architecture")]
    [Tooltip("Set the value of a Scriptable Int.")]
    public class ScriptableIntSetValue : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ScriptableInt))]
        [Tooltip("Select the Scriptable Int Object")]
        public FsmObject scriptableInt;

        [Tooltip("The int value you want the Scriptable Int to be set to.")]
        public FsmInt value;

        private ScriptableInt _scriptableInt;

        public override void Reset()
        {
            scriptableInt = null;
            value = 0;
        }

        public override void OnEnter()
        {
            DoSetScriptableIntValue();
            Finish();
        }

        private void DoSetScriptableIntValue()
        {
            _scriptableInt = scriptableInt.Value as ScriptableInt;
            _scriptableInt.SetValue(value.Value);
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return scriptableInt + ": Set Value " + value;
        }
#endif
    }
}
