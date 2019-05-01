using huqiang.UI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPage : Page
{
    class View
    {
        public ModelElement LeftUp;
        public ModelElement Center;
        public ModelElement List;
        public ModelElement friend;
        public ModelElement Right;
        public ScrollY FriendsRanking;
    }
    ModelElement txt;
    System.Random ran;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.FindModel("baseUI", "RankingList");
        base.Initial(parent, dat);
        model.SetParent(parent);
        var view =  model.ComponentReflection<View>();
        view.LeftUp.activeSelf= false;
        view.Right.activeSelf = false;
        view.friend.activeSelf = false;
        List<string> data = new List<string>();
        for (int i = 0; i < 100; i++)
            data.Add("sdfsdfsdf" + i);
        view.FriendsRanking.BindingData = data;
        view.FriendsRanking.Refresh();
        //ran = new System.Random();
        //txt.baseEvent.Click = (o, e) =>
        //{
        //    Debug.Log("click");
        //    txt.data.localPosition = new Vector3(ran.Next(-400, 400), 0, 0);
        //    txt.IsChanged = true;
        //};
    }
}
