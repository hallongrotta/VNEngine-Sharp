using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNActor;
using VNActor.KKS;
using VNEngine;
using static SceneSaveState.Camera;

namespace SceneSaveState
{
    internal partial class Scene
    {
        internal Dictionary<string, Text.TextData> texts;

        internal Scene(Dictionary<string, Character.ActorData> actors, Dictionary<string, Item.ItemData> items,
            Dictionary<string, Light.LightData> lights, Dictionary<string, NEOPropData> props, List<CamData> cams, Dictionary<string, Text.TextData> texts) : this(actors, items, lights, props, cams)
        {
            this.texts = texts;
        }
    }
}
