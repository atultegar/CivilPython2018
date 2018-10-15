using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Autodesk.AutoCAD;

using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Autodesk.Civil.Runtime;
using Autodesk.Civil.Settings;

namespace CivilPython2018
{
    public class CommandAndFuncions
    {
        [CommandMethod("-PYLOAD")]
        public static void PythonLoadCmdLine()
        {
            PythonLoad(true);
        }

        [CommandMethod("PYLOAD")]
        public static void PythonLoadUI()
        {
            PythonLoad(false);
        }

        public static void PythonLoad(bool useCmdLine)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            short fd = (short)Application.GetSystemVariable("FILEDIA");

            // As the user to select a .py file

            PromptOpenFileOptions pfo = new PromptOpenFileOptions("Select Python Script to load");
            pfo.Filter = "Python Script (*.py)|*.py";
            pfo.PreferCommandLine = (useCmdLine || fd == 0);
            PromptFileNameResult pr = ed.GetFileNameForOpen(pfo);

            // And then try to load and execute it

            if (pr.Status == PromptStatus.OK)
                ExecutePythonScript(pr.StringResult);

        }

        [LispFunction("PYLOAD")]
        public ResultBuffer PythonLoadLISP(ResultBuffer rb)
        {
            const int RTSTR = 5005;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            if (rb == null)
            {
                ed.WriteMessage("\nError: too few arguments\n");
            }
            else
            {
                // We are only really interested in the first argument

                Array args = rb.AsArray();
                TypedValue tv = (TypedValue)args.GetValue(0);

                // Which should be the filename of our script

                if (tv != null && tv.TypeCode == RTSTR)
                {
                    // If we manage to execute it, let's return the
                    // filename as the result of the function
                    // (just as (arxload) does)

                    bool success = ExecutePythonScript(Convert.ToString(tv.Value));
                    return
                        (success ?
                            new ResultBuffer(
                                    new TypedValue(RTSTR, tv.Value)
                                    )
                                    : null);
                }
            }
            return null;
        }

        private static bool ExecutePythonScript(string file)
        {
            // If the file exists, let's load and execute it
            // (we could/should probably add some more robust
            // exception handling here)

            bool ret = System.IO.File.Exists(file);
            if (ret)
            {
                ScriptEngine engine = Python.CreateEngine();
                engine.ExecuteFile(file);
            }
            return ret;
        }
    }
}
