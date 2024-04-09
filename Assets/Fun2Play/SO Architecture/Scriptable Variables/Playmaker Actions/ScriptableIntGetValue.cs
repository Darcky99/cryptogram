using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Fun2Play: SO Architecture")]
    [Tooltip("Get the value from a Scriptable Int Object.")]
    public class ScriptableIntGetValue : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ScriptableInt))]
        [Tooltip("Select the Scriptable Int Object")]
        public FsmObject scriptableInt;

        [Tooltip("Store the value in an int variable")]
        [UIHint(UIHint.Variable)]
        public FsmInt value;

        private ScriptableInt _scriptableInt;

        public override void Reset()
        {
            scriptableInt = null;
            value = null;
        }

        public override void OnEnter()
        {
            DoGetScriptableIntValue();
            Finish();
        }

        private void DoGetScriptableIntValue()
        {
            _scriptableInt = scriptableInt.Value as ScriptableInt;
            value.Value = _scriptableInt.value;
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return scriptableInt + ": Get Value";
        }
#endif
    }
}
