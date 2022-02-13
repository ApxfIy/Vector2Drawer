using Apxfly.CustomInspector;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [Vector2Selector] [SerializeField] private Vector2 _shadowDirection = new Vector2(1,1);
    [Vector2Selector] [SerializeField] private Vector2Int _shadowDirectionInt;

    [SerializeField] private bool _useInt;

    [SerializeField] private Shadow _shadow;

    private void Update()
    {
        _shadow.effectDistance = _useInt ? _shadowDirectionInt : _shadowDirection;
    }
}

