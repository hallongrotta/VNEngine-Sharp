using Studio;
using System;
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

        public static void AddToTrack<T>(T objctrl) where T: ObjectCtrlInfo
        {
            if (objctrl is OCIItem i) AddToTrack(i);
            else if (objctrl is OCIChar c) AddToTrack(c);
            else if (objctrl is OCIFolder f) AddToTrack(f);
            else if (objctrl is OCILight l) AddToTrack(l);
            else if (objctrl is OCIRoute r) AddToTrack(r);
        }

        public static void AddToTrack(OCIChar objctrl)
        {
            string id = ResolveID("act", new VNActor.Actor(objctrl), AllActors);
            if (id != null)
            {
                var tagfld = Folder.add(actor_folder_prefix + id);
                tagfld.set_parent_treenodeobject(objctrl.treeNodeObject.child[0].child[0]);
            }
        }

        public static void AddToTrack(OCIItem objctrl)
        {
            var newid = ResolveID("item", new Item(objctrl), AllProps);
            if (newid != null)
            {
                var tagfld = Folder.add(SceneFolders.prop_folder_prefix + newid);
                tagfld.set_parent_treenodeobject(objctrl.treeNodeObject);
            }
        }
        public static void AddToTrack(OCILight objctrl)
        {
            var l = new Light(objctrl);
            var newid = ResolveID("light", l, AllProps);
            if (newid != null)
            {
                var tagfld = Folder.add(SceneFolders.light_folder_prefix + newid);
                l.set_parent(tagfld);
            }
        }

        public static void AddToTrack(OCIFolder objctrl)
        {
            var newid = ResolveID("folder", new Folder(objctrl), AllProps);
            if (newid != null)
            {
                var tagfld = Folder.add(SceneFolders.prop_folder_prefix + newid);
                tagfld.set_parent_treenodeobject(objctrl.treeNodeObject);
            }
        }

        public static void AddToTrack(OCIRoute objctrl)
        {
            var newid = ResolveID("route", new Route(objctrl), AllProps);
            if (newid != null)
            {
                var tagfld = Folder.add("-propgrandpa:" + newid);
                tagfld.set_parent_treenodeobject(objctrl.treeNodeObject.child[0]);
            }
        }

        private static string ResolveID<T>(string baseid, T obj, Dictionary<string, T> dict) where T: NeoOCI
        {
            foreach (Prop p in AllProps.Values)
            {
                if (p.objctrl == obj.objctrl)
                {
                    return null;
                }
            }
            foreach (var i in Enumerable.Range(0, 1000 - 0))
            {
                var id = baseid + i;
                if (!dict.ContainsKey(id))
                {
                    return id;
                }
            }
            return null;
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

                        if (oci is Prop propOci)
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

        public static Folder createFld(string txt, NeoOCI parent = null, bool ret = true)
        {
            var fld = Folder.add(txt);
            if (parent is Folder)
            {
                fld.set_parent(parent);
            }
            if (ret == true)
            {
                return fld;
            }
            else
            {
                throw new Exception("create folder failed");
            }
        }

        public static Folder createFldIfNo(string txt, Folder parent, int childNum)
        {
            Folder fld;

            if (parent.treeNodeObject.child.Count <= childNum)
            {
                //print "create folder! %s" % txt
                fld = Folder.add(txt);
                fld.set_parent(parent);
                return fld;
            }
            else
            {
                var chld = parent.treeNodeObject.child[childNum];
                fld = NeoOCI.create_from_treenode(chld) as Folder;
                if (chld.textName != txt)
                {
                    //print "hit! upd folder! %s" % txt
                    fld.name = txt;
                    //return fld
                }
                else
                {
                    //print "hit!! no creation! %s" % txt
                }
                return fld;
            }
        }

        public static void restrict_to_child(Folder fld, int numchilds)
        {
            if (fld.treeNodeObject.child.Count > numchilds)
            {
                var ar = fld.treeNodeObject.child;
                var ar2 = new List<NeoOCI>();
                foreach (var treeobj in ar)
                {
                    ar2.Add(NeoOCI.create_from_treenode(treeobj));
                }
                foreach (var i in Enumerable.Range(0, ar2.Count))
                {
                    if (i >= numchilds)
                    {
                        Console.WriteLine(String.Format("deleted! {0}", i.ToString()));
                        ar2[i].delete();
                    }
                }
            }
        }

    }
}
