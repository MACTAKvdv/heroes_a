using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityArtifact", menuName = "Game/Entity/Artifact")]
public class ScriptableEntityArtifact : ScriptableEntityPerk
{
    [Header("Options Artifact")]
    public AnimationCurve Curve;
    public Sprite sprite;
    public Sprite spriteMap;

    public int Cost;

}
