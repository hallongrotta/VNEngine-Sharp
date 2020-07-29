using Studio;

namespace VNActor
{
    public class HSNeoOCI
    {

        public ObjectCtrlInfo objctrl;

        public HSNeoOCI(ObjectCtrlInfo objctrl)
        {
            this.objctrl = objctrl;
            return;
        }

        public static HSNeoOCI create_from(OCIChar objctrl) { return new HSNeoOCIChar(objctrl); }

        public static HSNeoOCI create_from(OCIItem objctrl) { return new HSNeoOCIItem(objctrl); }

        public static HSNeoOCI create_from(OCILight objctrl) { return new HSNeoOCILight(objctrl); }

        public static HSNeoOCI create_from(OCIRoute objctrl) { return new HSNeoOCIRoute(objctrl); }

        public static HSNeoOCI create_from(OCIFolder objctrl) { return new HSNeoOCIFolder(objctrl); }

        public static HSNeoOCI create_from(ObjectCtrlInfo objctrl) { return new HSNeoOCI(objctrl); }

        public static T create_from_treenode<T>(TreeNodeObject treenode) where T : HSNeoOCI
        {
            if (treenode == null)
            {
                return null;
            }
            Studio.Studio studio = Studio.Studio.Instance;
            var dobjctrl = studio.dicInfo;
            var obj = dobjctrl[treenode];
            if (obj != null)
            {
                return (T)create_from(obj);
            }
            return null;
        }

        public static HSNeoOCI create_from_treenode(TreeNodeObject treenode)
        {
            if (treenode == null)
            {
                return null;
            }
            Studio.Studio studio = Studio.Studio.Instance;
            var dobjctrl = studio.dicInfo;
            var obj = dobjctrl[treenode];
            if (obj != null)
            {
                return create_from(obj);
            }
            return null;
        }


        public static HSNeoOCI create_from_selected()
        {
            Studio.Studio studio = Studio.Studio.Instance;
            return HSNeoOCI.create_from_treenode(studio.treeNodeCtrl.selectNode);
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

        public void set_parent(HSNeoOCI parent)
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
