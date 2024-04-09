using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Fun2Play: SO Architecture")]
    [Tooltip("Increase or decrease the value of a Scriptable Int. To decrease simply set a negative value.")]
    public class ScriptableIntAddValue : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ScriptableInt))]
        [Tooltip("Select the Scriptable Int Object")]
        public FsmObject scriptableInt;

        [Tooltip("The value you want the Scriptable Int to increase/decrease by")]
        public FsmInt value;

        private ScriptableInt _scriptableInt;

        public override void Reset()
        {
            scriptableInt = null;
            value = 0;
        }

        public override void OnEnter()
        {
            DoAddScriptableIntValue();
            Finish();
        }

        private void DoAddScriptableIntValue()
        {
            _scriptableInt = scriptableInt.Value as ScriptableInt;
            _scriptableInt.AddValue(value.Value);
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            string prefix = (value.Value >= 0) ? ": Add " : ": Subtract ";
            return scriptableInt + prefix + value.Value.ToString();
        }
#endif
    }
}
