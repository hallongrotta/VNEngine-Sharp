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
        }

        public virtual bool Visible
        {
            get =>
                // get visible status
                objctrl.treeNodeObject.visible;
            set => objctrl.treeNodeObject.visible = value;
        }

        public abstract Vector3 Position { get; set; }
        public abstract Vector3 Rotation { get; set; }

        public bool visible_treenode
        {
            get => objctrl.treeNodeObject.visible;
            set
            {
                if (objctrl.treeNodeObject.visible != value) objctrl.treeNodeObject.SetVisible(value);
            }
        }

        public TreeNodeObject treeNodeObject
        {
            get
            {
                var obj = objctrl.treeNodeObject;
                return obj;
            }
        }

        public string text_name => objctrl.treeNodeObject.textName;

        public static Character create_from(OCIChar objctrl)
        {
            return new Character(objctrl);
        }

        public static Item create_from(OCIItem objctrl)
        {
            return new Item(objctrl);
        }

        public static Light create_from(OCILight objctrl)
        {
            return new Light(objctrl);
        }

        public static Route create_from(OCIRoute objctrl)
        {
            return new Route(objctrl);
        }

        public static Folder create_from(OCIFolder objctrl)
        {
            return new Folder(objctrl);
        }

        //public static HSNeoOCI create_from(ObjectCtrlInfo objctrl) { return new HSNeoOCI(objctrl); }

        public static NeoOCI create_from_treenode(TreeNodeObject treenode)
        {
            if (treenode == null) return null;
            var studio = Studio.Studio.Instance;
            var dobjctrl = studio.dicInfo;
            var obj = dobjctrl[treenode];
            if (obj != null)
            {
                if (obj is OCIItem item)
                    return create_from(item);
                if (obj is OCIFolder fld)
                    return create_from(fld);
                if (obj is OCIChar chara)
                    return create_from(chara);
                if (obj is OCIRoute route)
                    return create_from(route);
                if (obj is OCILight light)
                    return create_from(light);
                return null;
            }

            return null;
        }


        public static NeoOCI create_from_selected()
        {
            var studio = Studio.Studio.Instance;
            return create_from_treenode(studio.treeNodeCtrl.selectNode);
        }

        public void set_parent_treenodeobject(TreeNodeObject parentTreeNode)
        {
            var studio = Studio.Studio.Instance;
            studio.treeNodeCtrl.SetParent(treeNodeObject, parentTreeNode);
        }

        public void set_parent(NeoOCI parent)
        {
            set_parent_treenodeobject(parent.treeNodeObject);
        }

        public void delete()
        {
            var studio = Studio.Studio.Instance;
            studio.treeNodeCtrl.DeleteNode(treeNodeObject);
        }
    }
}