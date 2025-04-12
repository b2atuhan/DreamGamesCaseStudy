using UnityEngine;
public enum CubeColorType
{
    Red,
    Green,
    Blue,
    Yellow
}

public class Cube : CubeItem
{
    public CubeColorType colorType;

    public void SetColor(CubeColorType type)
    {
        colorType = type;
        // You can set visual color here too
    }

    public override void OnTapped()
    {
        if (!InputBlocker.IsInputEnabled) return;

        CubeManager.Instance.HandleCubeTap(this);
    }

    public void ShowRocketIcon(bool visible, float alpha = 1f)
    {
        Transform icon = transform.Find("RocketIcon");

        if (icon != null)
        {
            var sr = icon.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = sr.color;
                color.a = visible ? alpha : 0f;
                sr.color = color;
            }

            icon.gameObject.SetActive(visible);
        }
    }


}
