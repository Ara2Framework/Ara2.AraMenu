// Copyright (c) 2010-2016, Rafael Leonel Pontani. All rights reserved.
// For licensing, see LICENSE.md or http://www.araframework.com.br/license
// This file is part of AraFramework project details visit http://www.arafrework.com.br
// AraFramework - Rafael Leonel Pontani, 2016-4-14
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ara2.Components
{
    public interface IAraMenuItens
    {
        int Id {get;}
        object tag {get;set;}
        string Key { get; }
        string Caption { get; set; }
        //AraEvent<T> Event { get; set; }
        IAraMenuItens GetById(int vId);
        AraMenu Menu { get; set; }
        void Clear();
        string GetScript();
        void OnEvent();
        void CriaEventoClick();
    }

    public delegate void DAraMenuItensClick(AraMenuItens Object);

    [Serializable]
    [Obsolete]
    public class AraMenuItens:AraMenuItens<DAraMenuItensClick>
    {
        public AraMenuItens(AraMenu vMenu):
            base(vMenu)
        {
            
        }

        public AraMenuItens(string vCaption) :
            this(vCaption, vCaption)
        {
        }

        public AraMenuItens(string vKey, string vCaption) :
            this(vKey, vCaption, null)
        {
        }

        public AraMenuItens(string vKey, string vCaption, DAraMenuItensClick vClick) :
            this(vKey, vCaption, vClick, null)
        {
        }

        public AraMenuItens(string vKey, string vCaption, DAraMenuItensClick vClick, Hashtable vParans)
            :base(vKey,vCaption,vClick)
        {
            Parans = vParans;
            this.EventParams = new object[] { this };
        }

        [Obsolete]
        public Hashtable Parans { get; set; }
    };

    [Serializable]
    public class AraMenuItens<T> : List<IAraMenuItens>, IAraMenuItens
    {
        public object tag { get; set; }

        private string _Key = "";
        public string Key
        {
            get { return _Key; }
        }

        public string Caption { get; set; }

        public AraEvent<T> Event
        {
            get;
            set;
        }
        public object[] EventParams= new object[]{};

        private int _Id = -1;

        public int Id
        {
            get { return _Id; }
        }


        private AraObjectInstance<AraMenu> _Menu = new AraObjectInstance<AraMenu>();
        public AraMenu Menu
        {
            get { return _Menu.Object; }
            set
            {
                _Menu.Object = value;
                if (_Id == -1)
                    _Id = Menu.NewCodIten;
            }
        }

        public AraMenuItens(AraMenu vMenu)
        {
            Menu = vMenu;
        }

        public AraMenuItens(string vKey, string vCaption, T vEvent, params object[] vParametros):
            this(vKey,vCaption)
        {
  
            Event += vEvent;
            EventParams = vParametros;
        }

        public AraMenuItens(string vKey, string vCaption)
        {
            _Key = vKey;
            Caption = vCaption;
            Event = new AraEvent<T>();
        }

        public string GetScript()
        {
            if (Key != "")
            {

                string vTmp = "<li>";
                vTmp += "<a href='#'  id=\"" + IdHtml + "\" IdInternal=\"" + _Id + "\">" + Caption + "</a>";

                if (this.Count > 0)
                {
                    vTmp += "<ul>";
                    foreach (IAraMenuItens Iten in this)
                    {
                        vTmp += Iten.GetScript();
                    }
                    vTmp += "</ul>";
                }
                vTmp += "</li>";
                return vTmp;
            }
            else
            {
                string vTmp = "";

                foreach (IAraMenuItens Iten in this)
                {
                    vTmp += Iten.GetScript();
                }
                //vTmp += "</ul>";
                return vTmp;
            }
        }

        public string IdHtml
        {
            get
            {
                return "Menu_" + Menu.InstanceID + "_" + _Id;
            }
        }

        public void CriaEventoClick()
        {
            if (Key != "")
            {
                Menu.TickScriptCall();
                Tick.GetTick().Script.Send(@"vObj.CriaEventoClick('" + IdHtml + "','" + Key + "');\n");
            }

            foreach (IAraMenuItens Iten in this)
            {
                Iten.CriaEventoClick();
            }
        }

        public void OnEvent()
        {
            try
            {
                if (this.Event.InvokeEvent != null)
                    ((Delegate)(object)this.Event.InvokeEvent).DynamicInvoke(EventParams);
            }
            catch (Exception err)
            {
                throw new Exception("Error on AraMenuItens.Click.\n" + err.Message);
            }
        }

        public IAraMenuItens this[string vKey]
        {
            get
            {
                return this.Where(a=>a.Key == vKey).FirstOrDefault();
            }
        }

        public IAraMenuItens Add(IAraMenuItens vIten)
        {
            vIten.Menu = Menu;
            base.Add(vIten);
            return vIten;
        }

        public void AddRange(List<IAraMenuItens> vItens)
        {
            foreach(IAraMenuItens vIten in vItens)
                vIten.Menu = Menu;

            base.AddRange(vItens);
        }



        public IAraMenuItens GetById(int vId)
        {

            if (Id == vId)
                return this;

            foreach (IAraMenuItens Iten in this)
            {
                if (Iten.Id == vId)
                    return Iten;

                IAraMenuItens vTmp = Iten.GetById(vId);
                if (vTmp != null)
                    return vTmp;

            }

            return null;
        }

        public void Clear()
        {
            foreach (IAraMenuItens Iten in this.ToArray())
            {
                Iten.Clear();
                this.Remove(Iten);
            }
        }
    };
}
