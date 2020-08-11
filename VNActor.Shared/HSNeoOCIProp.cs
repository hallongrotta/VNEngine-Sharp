using Studio;

namespace VNActor
{
    abstract public class HSNeoOCIProp : HSNeoOCI, IVNObject
    {

        public HSNeoOCIProp(ObjectCtrlInfo objctrl) : base(objctrl)
        {
        }

        public abstract IDataClass export_full_status();
        public abstract void import_status(IDataClass status);
    }
}
