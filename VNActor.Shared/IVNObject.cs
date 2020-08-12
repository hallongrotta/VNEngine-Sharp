namespace VNActor
{
    interface IVNObject
    {
        void import_status(IDataClass status);

        IDataClass export_full_status();

    }
}
