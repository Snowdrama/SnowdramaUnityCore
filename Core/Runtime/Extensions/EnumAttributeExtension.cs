using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumNameAttribute : System.Attribute
{
    // Keep a variable internally ...
    protected string text;
    // The constructor is called when the attribute is set.
    public EnumNameAttribute(string setText)
    {
        text = setText;
    }


    // .. and show a copy to the outside world.
    public string Name
    {
        get { return text; }
        set { text = value; }
    }

    public override string ToString()
    {
        return Name;
    }
}
