using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [Apxfly.Editor.Attributes.Vector2Selector] 
    public Vector2 ShadowDirection = new Vector2(1,1);

    [Apxfly.Editor.Attributes.Vector2Selector] 
    public Vector2Int ShadowDirectionInt;

    [SerializeField] private bool _useInt;

    [SerializeField] private Shadow _shadow;

    private void Update()
    {
        _shadow.effectDistance = _useInt ? ShadowDirectionInt : ShadowDirection;
    }
}

