using System;

namespace ExtPlugins
{
    public class ExtPlugin
    {

        public bool isDetected;

        public object name;

        public ExtPlugin(object name = null)
        {
            this.isDetected = false;
            this.name = name;
            //print "ExtPlugin:", name
            this.initFlag();
        }

        [staticmethod]
        public static string calc_plugin_dir()
        {
            var rootfolder = path.realpath(path.join(Application.dataPath, ".."));
            // os.path.splitext(__file__)[0] + '.ini'
            var pydirname = path.dirname(@__file__);
            var pluginpath = path.realpath(path.join(pydirname, "..", ".."));
            return path.relpath(pluginpath, rootfolder);
        }

        [staticmethod]
        public static int exists(object name)
        {
            // usually check IPA folder
            // plgdir = os.path.join(os.getcwd(), "Plugins/")
            //
            // if get_engine_id() == "charastudio":
            //     # in CharaStudio check BepInEx folder
            //     plgdir = os.path.join(os.getcwd(), "BepInEx/")
            //
            // if get_engine_id() == "neov2":
            //     # in StudioNeoV2 check BepInEx/plugins folder
            //     plgdir = os.path.join(os.getcwd(), "BepInEx/Plugins/")
            var plgdir = ExtPlugin.calc_plugin_dir();
            return os.path.isfile(plgdir + "/" + name + ".dll");
        }

        // init flags
        public void initFlag()
        {
            //plgdir = os.path.join(os.getcwd(), "Plugins/")
            if (ExtPlugin.exists(this.name))
            {
                this.isDetected = true;
                clr.AddReference(this.name);
                //print self.name + ".dll detected."
            }
            else
            {
                this.isDetected = false;
                Console.WriteLine(this.name + ".dll not found.");
            }
        }
    }

}
