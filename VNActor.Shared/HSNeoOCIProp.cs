using Studio;

namespace VNActor
{
    public class HSNeoOCIProp : HSNeoOCI
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
    }
}
