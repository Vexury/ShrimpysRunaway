using TMPro;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private TMP_Text rankLabel;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text scoreLabel;

    [Header("Dev")]
    [SerializeField] private string[] devNames;
    [SerializeField] private TMP_Text devBadge;
    [ColorUsage(true, true)]
    [SerializeField] private Color devShimmerColor = new Color(2f, 1.6f, 0.1f);

    [Header("Top 3 Shimmer")]
    [ColorUsage(true, true)]
    [SerializeField] private Color rank1Color = new Color(2.2f, 1.6f, 0f,   1f);
    [ColorUsage(true, true)]
    [SerializeField] private Color rank2Color = new Color(1.4f, 1.4f, 1.6f, 1f);
    [ColorUsage(true, true)]
    [SerializeField] private Color rank3Color = new Color(1.6f, 0.8f, 0.2f, 1f);

    [Header("Shimmer Settings")]
    [SerializeField] private Color baseColor    = Color.white;
    [SerializeField] private float shimmerSpeed  = 2f;
    [SerializeField] private float shimmerSpread = 0.5f;

    private bool isDev;
    private int rank;

    // Per-character vertex color range for rank shimmer.
    // _FaceColor is set to (m, m, m) where m = max HDR component.
    // Low vertex × (m,m,m) = white. High vertex × (m,m,m) = HDR rank color.
    private Color rankShimmerLow;
    private Color rankShimmerHigh;

    public void Populate(string playerName, string score, int rank)
    {
        this.rank = rank;
        if (rankLabel != null)  rankLabel.text  = $"#{rank}";
        if (nameLabel != null)  nameLabel.text  = playerName;
        if (scoreLabel != null) scoreLabel.text = score;

        isDev = IsDevName(playerName);
        if (devBadge != null) devBadge.text = isDev ? "dev" : "";

        if (nameLabel != null)
        {
            if (rank >= 1 && rank <= 3)
            {
                Color hdr = rank == 1 ? rank1Color : rank == 2 ? rank2Color : rank3Color;
                // Neutral HDR white face color — keeps all hue channels available so we
                // can reach both white AND the rank color purely via vertex color.
                float m = Mathf.Max(Mathf.Max(hdr.r, hdr.g), hdr.b);
                if (m < 1f) m = 1f;
                nameLabel.fontMaterial.SetColor("_FaceColor", new Color(m, m, m, 1f));
                rankShimmerLow  = new Color(1f / m,     1f / m,     1f / m,     1f); // × face = white
                rankShimmerHigh = new Color(hdr.r / m,  hdr.g / m,  hdr.b / m,  1f); // × face = HDR rank color
            }
            else
            {
                nameLabel.fontMaterial.SetColor("_FaceColor", Color.white);
            }
        }
    }

    private void Update()
    {
        if (isDev && devBadge != null)
            Shimmer(devBadge, devShimmerColor);

        if (rank >= 1 && rank <= 3 && nameLabel != null)
            ShimmerRank(nameLabel);
    }

    private void ShimmerRank(TMP_Text label)
    {
        label.ForceMeshUpdate();
        TMP_TextInfo textInfo = label.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            float t = Mathf.Sin(Time.unscaledTime * shimmerSpeed - i * shimmerSpread) * 0.5f + 0.5f;
            Color32 c = Color.Lerp(rankShimmerLow, rankShimmerHigh, t);

            Color32[] colors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
            int v = charInfo.vertexIndex;
            colors[v] = colors[v + 1] = colors[v + 2] = colors[v + 3] = c;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            label.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    private void Shimmer(TMP_Text label, Color highlight)
    {
        label.ForceMeshUpdate();
        TMP_TextInfo textInfo = label.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            float t = Mathf.Sin(Time.unscaledTime * shimmerSpeed - i * shimmerSpread) * 0.5f + 0.5f;
            Color32 color = Color.Lerp(baseColor, highlight, t);

            Color32[] colors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
            int v = charInfo.vertexIndex;
            colors[v] = colors[v + 1] = colors[v + 2] = colors[v + 3] = color;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            label.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    private bool IsDevName(string playerName)
    {
        if (devNames == null) return false;
        foreach (var name in devNames)
            if (string.Equals(name, playerName, System.StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }
}
