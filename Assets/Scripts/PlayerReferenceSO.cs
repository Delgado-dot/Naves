using UnityEngine;

/// <summary>
/// ScriptableObject que actua como puente desacoplado entre el
/// jugador y el sistema de spawners. El jugador registra su
/// transform y los spawners leen PlayerTransform sin referencia directa.
/// </summary>
[CreateAssetMenu(menuName = "Naves/Player Reference", fileName = "PlayerReference")]
public class PlayerReferenceSO : ScriptableObject
{
    public Transform PlayerTransform { get; private set; }

    public void Register(Transform player)
    {
        PlayerTransform = player;
    }

    public void Clear()
    {
        PlayerTransform = null;
    }
}
