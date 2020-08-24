using Studio;
using System.Collections.Generic;
using System.Linq;
using VNActor;

namespace VNEngine
{
    public static class SceneFolders
    {

        public static string actor_folder_prefix = "vnactor:";
        public static string prop_folder_prefix = "vnprop:";
        public static string light_folder_prefix = "vnlight:";

        public static List<OCIFolder> scene_get_all_folders_raw()
        {
            var ar = new List<OCIFolder>();
            var dobjctrl = Studio.Studio.Instance.dicObjectCtrl;
            foreach (OCIFolder folder in dobjctrl.Values.OfType<OCIFolder>())
            {
                ar.Add(folder);
            }
            return ar;
        }

        public static void LoadTrackedActorsAndProps()
        {
            List<OCIFolder> folders = scene_get_all_folders_raw();
            AllActors.Clear();
            AllProps.Clear();

            foreach (OCIFolder fld in folders)
            {
                string fldName = fld.name;
                if (fldName.StartsWith(actor_folder_prefix))
                {

                    string actorAlias;
                    string actorColor = "ffffff";
                    string actorTitle = null;

                    // analysis actor tag
                    var tagElements = fldName.Split(':');
                    if (tagElements.Length == 2)
                    {
                        actorAlias = tagElements[1];
                    }
                    else if (tagElements.Length == 3)
                    {
                        actorAlias = tagElements[1];
                        actorColor = tagElements[2];
                    }
                    else
                    {
                        actorAlias = tagElements[1];
                        actorColor = tagElements[2];
                        actorTitle = tagElements[3];
                    }

                    if (!AllActors.ContainsKey(actorAlias))
                    {
                        var hsociChar = NeoOCI.create_from_treenode(fld.treeNodeObject.parent.parent.parent);

                        if (hsociChar is VNActor.Actor chara)
                        {
                            if (actorTitle is null)
                            {
                                actorTitle = hsociChar.text_name;
                            }

                            AllActors[actorAlias] = chara;

                            //register_char(actorAlias, actorColor, actorTitle);

                            StudioController.Instance.GetLogger.LogDebug("Registered actor: '" + actorAlias + "' as " + actorTitle + " (#" + actorColor + ")");
                        }
                    }
                }
                else if (fldName.StartsWith(prop_folder_prefix))
                {
                    // analysis props tag

                    string propAlias = fldName.Substring(prop_folder_prefix.Length).Trim();
                    // register props

                    if (!AllProps.ContainsKey(propAlias))
                    {
                        NeoOCI oci = NeoOCI.create_from_treenode(fld.treeNodeObject.parent);

                        if (oci is Item propOci)
                        {
                            AllProps[propAlias] = propOci;
                            StudioController.Instance.GetLogger.LogDebug("Registered prop: '" + Utils.to_roman(propAlias) + "' as " + Utils.to_roman(oci.text_name));
                        }
                    }
                }
                else if (fldName.StartsWith(light_folder_prefix))
                {
                    string propAlias = fldName.Substring(light_folder_prefix.Length).Trim();
                    // register props

                    if (!AllProps.ContainsKey(propAlias))
                    {
                        NeoOCI oci = NeoOCI.create_from_treenode(fld.treeNodeObject.child[0]);

                        if (oci is VNActor.Light propOci)
                        {
                            AllProps[propAlias] = propOci;
                            StudioController.Instance.GetLogger.LogDebug("Registered light: '" + Utils.to_roman(propAlias) + "' as " + Utils.to_roman(oci.text_name));
                        }
                    }
                }
            }
        }

        public static Dictionary<string, VNActor.Actor> AllActors { get; } = new Dictionary<string, VNActor.Actor>();

        public static Dictionary<string, Prop> AllProps { get; private set; } = new Dictionary<string, VNActor.Prop>();

        public static NeoOCI scenef_get_prop(string id)
        {
            if (AllProps.ContainsKey(id))
            {
                NeoOCI obj = AllProps[id];
                return obj;
            }
            return null;
        }

        public static Prop scenef_get_propf(string id)
        {
            if (AllProps.ContainsKey(id))
            {
                Prop obj = AllProps[id];
                return obj;
            }
            return null;
        }

        public static VNActor.Light scenef_get_light(string id)
        {
            if (AllProps.ContainsKey(id))
            {
                Prop obj = AllProps[id];
                return (VNActor.Light)obj;
            }
            return null;
        }

        public static VNActor.Actor scenef_get_actor(string id)
        {
            if (AllActors.ContainsKey(id))
            {
                VNActor.Actor obj = AllActors[id];
                return obj;
            }
            return null;
        }

        public static void scenef_reg_actor(string id, VNActor.Actor actor)
        {
            AllActors[id] = actor;
        }

        public static void scenef_reg_prop(string id, Item prop)
        {
            AllProps[id] = prop;
        }
    }
}
