using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class EnvironmentGenerator : MonoBehaviour
{
    [SerializeField] private SpriteShapeController spriteShapeController;
    [SerializeField, Range(3, 500)] private int _levelLength = 50;
    [SerializeField, Range(1f, 10f)] private float _xMultiplier = 2f;
    [SerializeField, Range(1f, 10f)] private float _yMultiplier = 2f;
    [SerializeField, Range(0f, 1f)] private float _smoothness = 0.5f;
    [SerializeField] private float _noiseSteps = 0.5f;
    [SerializeField] private float _bottom = 10;
    Vector3 _lastPos;

    private void OnValidate()
    {
        // spriteShapeController.spline.Clear();
        GenerateEnvironment();
    }

    void GenerateEnvironment()
    {
        spriteShapeController.spline.Clear();
        _yMultiplier = 0;
        for (int i = 0; i < _levelLength; i++)
        {
            if (i % 5 == 0)
                _yMultiplier += 0.5f;
            if (i % 10 == 0)
                _yMultiplier += 0.3f;
            _lastPos = transform.position + new Vector3(i * _xMultiplier, Mathf.PerlinNoise(0, i * _noiseSteps) * _yMultiplier, 0);
            spriteShapeController.spline.InsertPointAt(i, _lastPos);

            if (i != 0 && i != _levelLength - 1)
            {
                spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                spriteShapeController.spline.SetLeftTangent(i, Vector3.left * _xMultiplier * _smoothness);
                spriteShapeController.spline.SetRightTangent(i, Vector3.right * _xMultiplier * _smoothness);
            }
        }

        // Add bottom points to close the shape
        spriteShapeController.spline.InsertPointAt(_levelLength, new Vector3(_lastPos.x, transform.position.y - _bottom, 0));
        spriteShapeController.spline.InsertPointAt(_levelLength + 1, new Vector3(transform.position.x, transform.position.y - _bottom, 0));
    }

}