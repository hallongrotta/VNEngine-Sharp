using Studio;

namespace VNActor
{
    abstract public class Prop
        : NeoOCI, IVNObject
    {

        public Prop(ObjectCtrlInfo objctrl) : base(objctrl)
        {
        }

        public abstract IDataClass export_full_status();
        public abstract void import_status(IDataClass status);
    }
}
