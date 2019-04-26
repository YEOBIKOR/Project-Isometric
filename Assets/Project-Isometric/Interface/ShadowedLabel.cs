using UnityEngine;
using Isometric.Interface;

public class ShadowedLabel : InterfaceObject
{
    private FLabel _label;
    private FLabel _labelShadow;

    public string text
    {
        get
        { return _label.text; }
        set
        { _label.text = value; _labelShadow.text = value; }
    }

    public ShadowedLabel(MenuFlow menu, string text) : base (menu)
    {
        _label = new FLabel("font", text);
        _labelShadow = new FLabel("font", text);

        _labelShadow.y = -1f;
        _labelShadow.color = Color.black;

        AddElement(_labelShadow);
        AddElement(_label);
    }
}
