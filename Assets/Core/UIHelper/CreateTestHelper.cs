using huqiang;
using huqiang.UIEvent;
using UnityEngine;

public class CreateTestHelper:TestHelper
{
    public void Build()
    {
        if(!Application.isPlaying)
        {
            if (App.uiroot != null)
            {
                App.uiroot.Dispose();
            }
            int c = transform.childCount - 1;
            for (; c >= 0; c--)
            {
                GameObject.DestroyImmediate(transform.GetChild(c).gameObject);
            }
            var caret = InputCaret.Caret;
            if (caret != null)
                GameObject.DestroyImmediate(caret.gameObject);
            Initital();
            CreateTestPage();
            App.Update();
        }
    }
    public virtual void CreateTestPage()
    {

    }
}
