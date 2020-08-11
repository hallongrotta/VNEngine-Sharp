using Studio;

namespace VNActor
{
    abstract public class HSNeoOCIProp : HSNeoOCI
    {

        public HSNeoOCIProp(ObjectCtrlInfo objctrl) : base(objctrl)
        {
        }

        public Item as_prop
        {
            get
            {
                return (Item)this;
            }
        }

        abstract public IDataClass export_full_status();
    }
}
