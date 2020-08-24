using Studio;
using System.Collections.Generic;
using UnityEngine;

namespace VNActor
{
    public class Folder
        : NeoOCI
    {

        new public OCIFolder objctrl;

        public Folder(OCIFolder objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        public static Folder add(string name)
        {
            OCIFolder fold = AddObjectFolder.Add();
            Folder obj = new Folder(fold);
            if (name != null)
            {
                obj.name = name;
            }
            return obj;
        }

        public static List<Folder> find_all(string name)
        {
            Studio.Studio studio = Studio.Studio.Instance;
            List<Folder> ar = new List<Folder>();
            Dictionary<TreeNodeObject, ObjectCtrlInfo> dobjctrl = studio.dicInfo;
            foreach (TreeNodeObject key in dobjctrl.Keys)
            {
                // key is TreeNodeObject
                ObjectCtrlInfo objctrl = dobjctrl[key];
                if (objctrl is OCIFolder folder)
                {
                    string txt = folder.name;
                    // ar.append((objctrl.name, objctrl, key))
                    if (txt == name)
                    {
                        ar.Add(new Folder(folder));
                        // this will process for all folders we found
                    }
                }
            }
            return ar;
        }

        public static Folder find_single(string name)
        {
            List<Folder> ar = Folder.find_all(name);
            Folder obj = null;
            if (ar.Count > 0)
            {
                obj = ar[0];
            }
            return obj;
        }

        public static List<Folder> find_all_startswith(string name)
        {
            Studio.Studio studio = Studio.Studio.Instance;
            List<Folder> ar = new List<Folder>();
            Dictionary<TreeNodeObject, ObjectCtrlInfo> dobjctrl = studio.dicInfo;
            foreach (TreeNodeObject key in dobjctrl.Keys)
            {
                // key is TreeNodeObject
                ObjectCtrlInfo objctrl = dobjctrl[key];
                if (objctrl is OCIFolder folder)
                {
                    string txt = folder.name;

                    if (txt.StartsWith(name))
                    {
                        ar.Add(new Folder(folder));
                        // this will process for all folders we found
                    }
                }
            }
            return ar;
        }

        public static Folder find_single_startswith(string name)
        {
            List<Folder> ar = Folder.find_all_startswith(name);
            Folder obj = null;
            if (ar.Count > 0)
            {
                obj = ar[0];
            }
            return obj;
        }

        public string name
        {
            get
            {
                return this.objctrl.name;
            }
            set
            {
                OCIFolder folder = (OCIFolder)this.objctrl;
                this.objctrl.name = value;
            }
        }

        public void delete_all_children()
        {
            List<TreeNodeObject> ar = this.treeNodeObject.child;
            List<NeoOCI> ar2 = new List<NeoOCI>();
            foreach (var treeobj in ar)
            {
                ar2.Add(NeoOCI.create_from_treenode(treeobj));
            }
            foreach (NeoOCI obj in ar2)
            {
                obj.delete();
            }
        }

        override public Vector3 pos
        {
            get
            {
                return this.objctrl.objectInfo.changeAmount.pos;
            }
            set
            {
                var item = this.objctrl;
                item.objectInfo.changeAmount.pos = value;
            }
        }

        override public Vector3 rot
        {
            get
            {
                return this.objctrl.objectInfo.changeAmount.rot;
            }
            set
            {
                var item = this.objctrl;
                item.objectInfo.changeAmount.rot = value;
            }
        }
    }
}
