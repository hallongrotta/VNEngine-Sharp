using System;
using System.Collections.Generic;
using System.Text;

namespace ExtPlugins
{
    public class DHH_AI
        : ExtPlugin
    {

        public object dhh_main;

        public string dhh_path;

        public string dhh_tempfile;

        public DHH_AI()
            : base("DHH_AI4")
        {
            if (this.isDetected)
            {
                this.dhh_main = DHH_Main.instance;
                this.dhh_path = path.realpath(GetPrivate(this.dhh_main, "path"));
                this.dhh_tempfile = path.join(this.dhh_path, "VNGETemp");
                this.delTempFiles();
            }
        }

        public object getEnable()
        {
            if (this.isDetected)
            {
                return GetPrivate(this.dhh_main, "GraphicEnable");
            }
        }

        public object setEnable(object enable)
        {
            if (this.isDetected)
            {
                this.dhh_main.SetEnable(enable);
            }
        }

        public object exportGraphSetting()
        {
            if (this.isDetected)
            {
                this.delTempFiles();
                var idx = 0;
                while (true)
                {
                    var indexedFile = this.dhh_tempfile + idx.ToString();
                    if (!os.path.exists(indexedFile))
                    {
                        break;
                    }
                    else
                    {
                        idx += 1;
                    }
                }
                CallPrivate(this.dhh_main, "SaveGraphicSetting", new List<string> {
                    indexedFile
                });
                var fp = open(indexedFile, "r");
                var settxt = fp.read();
                fp.close();
                return settxt;
            }
            else
            {
                return null;
            }
        }

        public object importGraphSetting(object setting)
        {
            if (this.isDetected)
            {
                this.delTempFiles();
                var idx = 0;
                while (true)
                {
                    try
                    {
                        var indexedFile = this.dhh_tempfile + idx.ToString();
                        var fp = open(indexedFile, "w");
                        fp.write(setting);
                        fp.flush();
                        fp.close();
                        break;
                    }
                    catch
                    {
                        idx += 1;
                    }
                }
                // LoadGraphicSetting didn't close file stream...
                CallPrivate(this.dhh_main, "LoadGraphicSetting", new List<string> {
                    indexedFile
                });
            }
        }

        public object delTempFiles()
        {
            if (this.isDetected)
            {
                var tempfiles = new List<object>();
                foreach (var f in os.listdir(this.dhh_path))
                {
                    if (f.startswith("VNGETemp"))
                    {
                        tempfiles.append(os.path.join(this.dhh_path, f));
                    }
                }
                foreach (var f in tempfiles)
                {
                    try
                    {
                        os.remove(f);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
