using Studio;

namespace VNActor
{
    abstract public class Prop
        : NeoOCI, IVNObject<Prop>
    {
        public Prop(ObjectCtrlInfo objctrl) : base(objctrl)
        {
        }

        public IDataClass<Prop> export_full_status()
        {
            return new NEOPropData(this);
        }

        public void import_status(IDataClass<Prop> status)
        {
            status.Apply(this);
        }
    }
}
