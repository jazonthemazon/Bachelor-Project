using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 1760)] private double _frequency;
    [SerializeField] [Range(0, 1)] private float _gain;
    [SerializeField] [Range(0, 1)] private float _volume;

    private double _increment;
    private double _phase;
    private double _samplingFrequency;

    private InputSystem_Actions _inputSystemActions;

    private void Awake()
    {
        _samplingFrequency = AudioSettings.outputSampleRate;
        _inputSystemActions = new();
    }

    private void OnEnable()
    {
        _inputSystemActions.Enable();
        _inputSystemActions.Player.Jump.performed += ctx => StartNote();
        _inputSystemActions.Player.Jump.canceled += ctx => StopNote();
    }

    private void OnDisable()
    {
        _inputSystemActions.Disable();
    }

    private void StartNote()
    {
        _gain = _volume;
    }

    private void StopNote()
    {
        _gain = 0f;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        _increment = _frequency * 2.0 * Mathf.PI / _samplingFrequency;

        for (var i = 0; i < data.Length; i += channels)
        {
            _phase += _increment;

            // sine
            float value = _gain * Mathf.Sin((float)_phase);

            // square
            //value = (Mathf.Sin((float)_phase) >= 0f) ? _gain : -_gain;

            // saw
            value = Mathf.Lerp(-_gain, _gain , (float)_phase / (Mathf.PI * 2));

            for (int channel = 0; channel < channels; channel++)
            {
                data[i + channel] = value;
            }

            if (_phase > Mathf.PI * 2) _phase -= Mathf.PI * 2;
        }
    }
}