using Studio;
using System.Collections.Generic;
using UnityEngine;

namespace VNActor
{
    public class HSNeoOCIFolder
        : HSNeoOCIProp
    {

        new public OCIFolder objctrl;

        public HSNeoOCIFolder(OCIFolder objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        public static HSNeoOCIFolder add(string name)
        {
            OCIFolder fold = AddObjectFolder.Add();
            HSNeoOCIFolder obj = new HSNeoOCIFolder(fold);
            if (name != null)
            {
                obj.name = name;
            }
            return obj;
        }

        public static List<HSNeoOCIFolder> find_all(string name)
        {
            Studio.Studio studio = Studio.Studio.Instance;
            List<HSNeoOCIFolder> ar = new List<HSNeoOCIFolder>();
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
                        ar.Add(new HSNeoOCIFolder(folder));
                        // this will process for all folders we found
                    }
                }
            }
            return ar;
        }

        public static HSNeoOCIFolder find_single(string name)
        {
            List<HSNeoOCIFolder> ar = HSNeoOCIFolder.find_all(name);
            HSNeoOCIFolder obj = null;
            if (ar.Count > 0)
            {
                obj = ar[0];
            }
            return obj;
        }

        public static List<HSNeoOCIFolder> find_all_startswith(string name)
        {
            Studio.Studio studio = Studio.Studio.Instance;
            List<HSNeoOCIFolder> ar = new List<HSNeoOCIFolder>();
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
                        ar.Add(new HSNeoOCIFolder(folder));
                        // this will process for all folders we found
                    }
                }
            }
            return ar;
        }

        public static HSNeoOCIFolder find_single_startswith(string name)
        {
            List<HSNeoOCIFolder> ar = HSNeoOCIFolder.find_all_startswith(name);
            HSNeoOCIFolder obj = null;
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
            List<HSNeoOCI> ar2 = new List<HSNeoOCI>();
            foreach (var treeobj in ar)
            {
                ar2.Add(HSNeoOCI.create_from_treenode(treeobj));
            }
            foreach (HSNeoOCI obj in ar2)
            {
                obj.delete();
            }
        }

        public Vector3 pos
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

        public Vector3 rot
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
