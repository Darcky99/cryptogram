using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Fun2Play: SO Architecture")]
    [Tooltip("Increase or decrease the value of a Scriptable Float. To decrease simply set a negative value.")]
    public class ScriptableFloatAddValue : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ScriptableFloat))]
        [Tooltip("Select the Scriptable Float Object")]
        public FsmObject scriptableFloat;

        [Tooltip("The value you want the Scriptable Float to increase/decrease by")]
        public FsmFloat value;

        private ScriptableFloat _scriptableFloat;

        public override void Reset()
        {
            scriptableFloat = null;
            value = 0f;
        }

        public override void OnEnter()
        {
            DoAddScriptableFloatValue();
            Finish();
        }

        private void DoAddScriptableFloatValue()
        {
            _scriptableFloat = scriptableFloat.Value as ScriptableFloat;
            _scriptableFloat.AddValue(value.Value);
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            string prefix = (value.Value >= 0) ? ": Add " : ": Subtract ";
            return scriptableFloat + prefix + value.Value.ToString();
        }
#endif
    }
}
