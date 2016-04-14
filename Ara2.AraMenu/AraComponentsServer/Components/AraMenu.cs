// Copyright (c) 2010-2016, Rafael Leonel Pontani. All rights reserved.
// For licensing, see LICENSE.md or http://www.araframework.com.br/license
// This file is part of AraFramework project details visit http://www.arafrework.com.br
// AraFramework - Rafael Leonel Pontani, 2016-4-14
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Ara2.Components
{
    [Serializable]
    public class AraMenu : AraComponentVisual
    {
        public AraMenu(IAraContainerClient ConteinerFather)
            : base(AraObjectClienteServer.Create(ConteinerFather, "a", new Dictionary<string, string> { { "href", "#" }, { "style", "width:1px;height:1px;left:-5px;top:-5px;position:absolute;" } }), ConteinerFather, "AraMenu")
        {
            Itens = new AraMenuItens(this);
            this.EventInternal += AraMenu_EventInternal;
        }

        public override void LoadJS()
        {
            Tick vTick = Tick.GetTick();
            vTick.Session.AddCss("Ara2/Components/AraMenu/files/fg_menu.css");
            vTick.Session.AddCss("Ara2/Components/AraMenu/files/fg_menu_add.css");
            vTick.Session.AddJs("Ara2/Components/AraMenu/files/fg_menu.js");
            vTick.Session.AddJs("Ara2/Components/AraMenu/AraMenu.js");
        }


        public AraMenuItens Itens;
        
        public object Tag;

        private bool _FlyOut = true;
        /// <summary>
        /// Defeut: True
        /// </summary>
        public bool FlyOut
        {
            get { return _FlyOut; }
            set { _FlyOut = value; }
        }

        public string ButtonShowCaption = "";

        
        public void AraMenu_EventInternal(String vFunction)
        {
            switch (vFunction.ToUpper())
            {
                case "CLICK":
                    try
                    {
                        Tick Tick = Tick.GetTick();
                        if (Tick.Page.Request["key"] != "-1")
                        {
                            IAraMenuItens tabof = Itens.GetById(Convert.ToInt32(Tick.Page.Request["key"]));
                            tabof.OnEvent();
                        }
                        else
                            if (Click != null)
                                Click(this, new AraEventMouse());
                    }
                    catch (Exception err)
                    {
                        throw new Exception("Error on AraMenu.EventInternal('" + vFunction + "').\n" + err.Message);
                    }

                break;
            }
        }

       
        public void Commit()
        {
            string vTmp = @"
            {
                ButtonShowCaption:'" + AraTools.StringToStringJS(ButtonShowCaption) + @"',
                content:'" + AraTools.StringToStringJS("<ul>" + Itens.GetScript() + "</ul>") + @"',
                flyOut:" + (FlyOut == true ? "true" : "false") + @",
                positionOpts:{
                    posX: 'left', 
			        posY: 'bottom',
			        offsetX: 0,
			        offsetY: 0,
			        directionH: 'right',
			        directionV: 'down', 
			        detectH: false, // do horizontal collision detection  
			        detectV: false, // do vertical collision detection
			        linkToFront: false
                }
            }";


            this.TickScriptCall();
            Tick.GetTick().Script.Send(" vObj.Create(" + vTmp + "); \n");
            Itens.CriaEventoClick();
        }

        /// <summary>
        /// Show on mouse position
        /// </summary>
        public void Show()
        {
            this.TickScriptCall();
            Tick.GetTick().Script.Send(" vObj.showMenu(); \n");
        }

        public void Show(Ara2.Components.IAraObject ComponentVisual)
        {
            Show(ComponentVisual, AraMenuShowSide.BottomLeftCorner);
        }

        public enum AraMenuShowSide 
        {
            UpperLeftCorner=1,
            BottomLeftCorner=2,
            BottomRightCorner=3
        }
        public void Show(Ara2.Components.IAraObject vAraObj, AraMenuShowSide Side)
        {
            var vParans = new
            {
                vObjId = vAraObj.InstanceID,
                Side = (int)Side
            };

            this.TickScriptCall();
            Tick.GetTick().Script.Send(" vObj.showMenu(" + Json.DynamicJson.Serialize(vParans) + "); \n");
        }


        /// <summary>
        /// Show on left top
        /// </summary>
        /// <param name="vLeft"></param>
        /// <param name="vTop"></param>
        public void Show(decimal vLeft, decimal vTop)
        {
            var vParans = new
            {
                left = vLeft,
                top = vTop
            };

            this.TickScriptCall();
            Tick.GetTick().Script.Send(" vObj.showMenu(" + Json.DynamicJson.Serialize(vParans) + "); \n");
        }

        private int _NewCodIten=0;
        public int NewCodIten
        {
            get
            {
                _NewCodIten++;
                return _NewCodIten;
            }
        }

        private bool _Enabled = true;
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                _Enabled = value;

                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetEnabled(" + (_Enabled == true ? "true" : "false") + "); \n");
            }
        }

        public delegate void DClick(AraMenu vAraMenu, AraEventMouse vMouse);
        public event DClick Click;

        public void Dispose()
        {
            this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.destruct(); \n");

            base.Dispose();
        }

    }

    

    
}
