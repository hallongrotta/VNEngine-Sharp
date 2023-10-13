using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNActor;
using VNActor.KKS;
using VNEngine;
using static VNEngine.System;
using static SceneSaveState.Camera;

namespace SceneSaveState
{
    
    public partial class Scene
    {

        [Key("texts")]
        public Dictionary<string, Text.TextData> texts;

        public Scene(
            string Name,
            Dictionary<string, Text.TextData> texts,
            Dictionary<string, Character.ActorData> actors, 
            List<CamData> cams, 
            Dictionary<string, Item.ItemData> items,
            Dictionary<string, Light.LightData> lights,
            Dictionary<string, NEOPropData> props,
            SystemData sys,
            List<View> views
            ) : base()
        {
            this.cams = null;
            if ( cams != null )
            {
                foreach (var c in cams)
                {
                    Add(new View(c));
                }
            }
            this.actors = actors;
            this.props = props;
            this.items = items;
            this.lights = lights;
            this.sys = sys;
            this.Name = Name;
            this.texts = texts;
            if (views != null)
            {
                this.views = views;
            }
        }
    }
}
