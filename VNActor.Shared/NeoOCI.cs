using Studio;
using UnityEngine;

namespace VNActor
{
    public abstract class NeoOCI
    {

        public ObjectCtrlInfo objctrl;

        public NeoOCI(ObjectCtrlInfo objctrl)
        {
            this.objctrl = objctrl;
            return;
        }

        public abstract Vector3 pos { get; set; }
        public abstract Vector3 rot { get; set; }

        public static Actor create_from(OCIChar objctrl) { return new Actor(objctrl); }

        public static Item create_from(OCIItem objctrl) { return new Item(objctrl); }

        public static Light create_from(OCILight objctrl) { return new Light(objctrl); }

        public static Route create_from(OCIRoute objctrl) { return new Route(objctrl); }

        public static Folder create_from(OCIFolder objctrl) { return new Folder(objctrl); }

        //public static HSNeoOCI create_from(ObjectCtrlInfo objctrl) { return new HSNeoOCI(objctrl); }

        public static NeoOCI create_from_treenode(TreeNodeObject treenode)
        {
            if (treenode == null)
            {
                return null;
            }
            Studio.Studio studio = Studio.Studio.Instance;
            var dobjctrl = studio.dicInfo;
            ObjectCtrlInfo obj = dobjctrl[treenode];
            if (obj != null)
            {
                if (obj is OCIItem item)
                {
                    return create_from(item);
                }
                else if (obj is OCIFolder fld)
                {
                    return create_from(fld);
                }
                else if (obj is OCIChar chara)
                {
                    return create_from(chara);
                }
                else if (obj is OCIRoute route)
                {
                    return create_from(route);
                }
                else if (obj is OCILight light)
                {
                    return create_from(light);
                }
                else
                {
                    return null;
                }
            }
            return null;
        }


        public static NeoOCI create_from_selected()
        {
            Studio.Studio studio = Studio.Studio.Instance;
            return NeoOCI.create_from_treenode(studio.treeNodeCtrl.selectNode);
        }

        public bool visible_treenode
        {
            get
            {
                return this.objctrl.treeNodeObject.visible;
            }
            set
            {
                if (this.objctrl.treeNodeObject.visible != value)
                {
                    this.objctrl.treeNodeObject.SetVisible(value);
                }
            }
        }

        public TreeNodeObject treeNodeObject
        {
            get
            {
                TreeNodeObject obj = this.objctrl.treeNodeObject;
                return obj;
            }
        }

        public void set_parent_treenodeobject(TreeNodeObject parentTreeNode)
        {
            Studio.Studio studio = Studio.Studio.Instance;
            studio.treeNodeCtrl.SetParent(this.treeNodeObject, parentTreeNode);
        }

        public void set_parent(NeoOCI parent)
        {
            this.set_parent_treenodeobject(parent.treeNodeObject);
        }

        public void delete()
        {
            Studio.Studio studio = Studio.Studio.Instance;
            studio.treeNodeCtrl.DeleteNode(this.treeNodeObject);
        }

        public string text_name
        {
            get
            {
                return this.objctrl.treeNodeObject.textName;
            }
        }
    }
}
