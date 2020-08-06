using Studio;

namespace VNActor
{
    abstract public class HSNeoOCIProp : HSNeoOCI
    {

        public HSNeoOCIProp(ObjectCtrlInfo objctrl) : base(objctrl)
        {
        }

        public Prop as_prop
        {
            get
            {
                return (Prop)this;
            }
        }

        abstract public IDataClass export_full_status();
    }
}
