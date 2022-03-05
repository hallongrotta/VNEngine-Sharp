namespace VNActor
{
    public interface IVNObject<T>
    {
        void import_status(IDataClass<T> status);

        IDataClass<T> export_full_status();
    }
}