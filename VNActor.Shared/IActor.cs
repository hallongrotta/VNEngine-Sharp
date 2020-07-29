using static VNActor.Actor;

namespace VNActor
{
    public interface IActor
    {

        int sex
        {
            get;
        }

        float height
        {
            get;
        }

        float breast
        {
            get;
        }
        AnimeOption_s anime_option_param { get; set; }
        IDataClass export_full_status();
    }
}
