using System.Collections.Generic;
using Studio;
using UnityEngine;

namespace VNActor
{
    public class Folder
        : Prop
    {
        public new OCIFolder objctrl;

        public Folder(OCIFolder objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        public string name
        {
            get => objctrl.name;
            set
            {
                var folder = objctrl;
                objctrl.name = value;
            }
        }

        public override Vector3 Position
        {
            get => objctrl.objectInfo.changeAmount.pos;
            set
            {
                var item = objctrl;
                item.objectInfo.changeAmount.pos = value;
            }
        }

        public override Vector3 Rotation
        {
            get => objctrl.objectInfo.changeAmount.rot;
            set
            {
                var item = objctrl;
                item.objectInfo.changeAmount.rot = value;
            }
        }

        public static Folder add(string name)
        {
            var fold = AddObjectFolder.Add();
            var obj = new Folder(fold);
            if (name != null) obj.name = name;
            return obj;
        }

        public static List<Folder> find_all(string name)
        {
            var studio = Studio.Studio.Instance;
            var ar = new List<Folder>();
            var dobjctrl = studio.dicInfo;
            foreach (var key in dobjctrl.Keys)
            {
                // key is TreeNodeObject
                var objctrl = dobjctrl[key];
                if (objctrl is OCIFolder folder)
                {
                    var txt = folder.name;
                    // ar.append((objctrl.name, objctrl, key))
                    if (txt == name)
                        ar.Add(new Folder(folder));
                    // this will process for all folders we found
                }
            }

            return ar;
        }

        public static Folder find_single(string name)
        {
            var ar = find_all(name);
            Folder obj = null;
            if (ar.Count > 0) obj = ar[0];
            return obj;
        }

        public static List<Folder> find_all_startswith(string name)
        {
            var studio = Studio.Studio.Instance;
            var ar = new List<Folder>();
            var dobjctrl = studio.dicInfo;
            foreach (var key in dobjctrl.Keys)
            {
                // key is TreeNodeObject
                var objctrl = dobjctrl[key];
                if (objctrl is OCIFolder folder)
                {
                    var txt = folder.name;

                    if (txt.StartsWith(name))
                        ar.Add(new Folder(folder));
                    // this will process for all folders we found
                }
            }

            return ar;
        }

        public static Folder find_single_startswith(string name)
        {
            var ar = find_all_startswith(name);
            Folder obj = null;
            if (ar.Count > 0) obj = ar[0];
            return obj;
        }

        public void delete_all_children()
        {
            var ar = treeNodeObject.child;
            var ar2 = new List<NeoOCI>();
            foreach (var treeobj in ar) ar2.Add(create_from_treenode(treeobj));
            foreach (var obj in ar2) obj.delete();
        }
    }
}