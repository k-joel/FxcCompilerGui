using FxcCompilerGui.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FxcCompilerGui.Model
{
    internal class FxcCompilerWrapper
    {
        public static string[] ProfileStrings =
            {
                // effect types
                "fx_2_0", "fx_4_0", "fx_4_1", "fx_5_0",
                // vertex shader types
                "vs_1_1", "vs_2_0", "vs_2_a", "vs_2_sw", "vs_3_0", "vs_3_sw", "vs_4_0", "vs_4_1", "vs_5_0", "vs_5_1",
                // pixel shader types
                "ps_2_0", "ps_2_a", "ps_2_b", "ps_2_sw", "ps_3_0", "ps_3_sw", "ps_4_0", "ps_4_1", "ps_5_0", "ps_5_1",
                // geometry shader
                "gs_4_0", "gs_4_1", "gs_5_0", "gs_5_1",
                // compute shader
                "cs_4_0", "cs_4_1", "cs_5_0", "cs_5_1",
                // domain shader
                "ds_5_0", "ds_5_1",
                // hull shader
                "hs_5_0", "hs_5_1",
            };

        public static string[] OptimizationLevelStrings =
            {
                "Level 0",
                "Level 1 (default)",
                "Level 2 (reserved)",
                "Level 3 (reserved)"
            };

        public FxcCompilerWrapper(string fxcPath, string shaderPath, int profileIndex)
        {
            FxcCompilerPath = fxcPath;
            ShaderPath = shaderPath;
            ProfileIndex = profileIndex;
            OptimizationLevel = 1;

            Output = new OutputLog();
            Output.WriteLine("Program Started!");

            Defines = new List<string>();
            Includes = new List<string>();
        }

        public OutputLog Output { get; set; }

        public List<string> Defines { get; set; }

        public List<string> Includes { get; set; }

        public string FxcCompilerPath { get; set; }

        public string ShaderPath { get; set; }

        public string EntryPoint { get; set; }

        public int ProfileIndex { get; set; }

        public bool UseLegacyCompiler { get; set; }

        public bool DisableOptimizations { get; set; }

        public int OptimizationLevel { get; set; }

        public bool EnableDebugInfo { get; set; }

        public bool TreatWarningsAsErrors { get; set; }

        public bool AvoidFlowControl { get; set; }

        public bool PreferFlowControl { get; set; }

        public bool DisableEffectPerformanceMode { get; set; }

        public bool EnableBackwardsCompatMode { get; set; }

        public bool EnableStrictMode { get; set; }

        public bool DisableValidation { get; set; }

        public bool OutputAssembly { get; set; }

        public bool OutputPdbInfo { get; set; }

        public bool OutputLibrary { get; set; }

        public bool OutputWarnAndErr { get; set; }

        public bool PreprocessOnly { get; set; }

        public string BuildCommandLineString()
        {
            return FxcCompilerPath + " " + BuildArgs();
        }

        public void Compile()
        {
            if (string.IsNullOrEmpty(ShaderPath) || !File.Exists(ShaderPath))
            {
                Output.WriteLine("Error: No shader file or cannot find shader code file at specified location");
                return;
            }

            if (string.IsNullOrEmpty(FxcCompilerPath) || !File.Exists(FxcCompilerPath))
            {
                Output.WriteLine("Error: Cannot find fxc.exe at the specified location");
                return;
            }

            if (Path.GetFileName(FxcCompilerPath) != "fxc.exe")
            {
                Output.WriteLine("Error: Invalid compiler file. Please select fxc.exe");
                return;
            }

            string shaderPathOutput = ShaderPath + ".o";
            if (File.Exists(shaderPathOutput))
            {
                Output.WriteLine("Deleting existing file...");
                File.Delete(shaderPathOutput);
            }

            Output.WriteLine("Compiling...");

            var fxargs = BuildArgs();

            var psi = new ProcessStartInfo(FxcCompilerPath);
            psi.Arguments = fxargs;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            var prc = new Process();
            prc.StartInfo = psi;
            prc.OutputDataReceived += (sender, args) => Debug.WriteLine(args.Data);
            prc.ErrorDataReceived += (sender, args) => Debug.WriteLine(args.Data);
            prc.Start();

            var output = prc.StandardOutput.ReadToEnd();
            var errors = prc.StandardError.ReadToEnd();
            prc.WaitForExit();

            Output.WriteLine(output);
            Output.WriteLine(errors);
        }

        private string BuildArgs()
        {
            string output = "";

            if (!PreprocessOnly)
                output += " /T " + ProfileStrings[ProfileIndex];

            if (!string.IsNullOrEmpty(EntryPoint))
                output += " /E " + EntryPoint;

            string defines = Defines.Aggregate("", (current, define) => current + (" /D " + define));
            output += defines;

            string includes = Includes.Aggregate("", (current, include) => current + (" /I " + include));
            output += includes;

            if (DisableOptimizations)
                output += " /Od";
            else
                output += " /O" + OptimizationLevel.ToString(CultureInfo.InvariantCulture);

            if (UseLegacyCompiler)
                output += " /LD";
            if (AvoidFlowControl)
                output += " /Gfa";
            if (PreferFlowControl)
                output += " /Gfp";
            if (TreatWarningsAsErrors)
                output += " /WX";
            if (EnableDebugInfo)
                output += " /Zi";
            if (DisableEffectPerformanceMode)
                output += " /Gdp";
            if (EnableBackwardsCompatMode)
                output += " /Gec";
            if (EnableStrictMode)
                output += " /Ges";
            if (DisableValidation)
                output += " /Vd";
            if (OutputAssembly)
                output += " /Fc " + ShaderPath + ".asm";
            if (OutputPdbInfo)
                output += " /Fd " + ShaderPath + ".pdb";
            if (OutputLibrary)
                output += " /Fl " + ShaderPath + ".lib";
            if (OutputWarnAndErr)
                output += " /Fe " + ShaderPath + ".err.txt";

            if (PreprocessOnly)
                output += " /P " + ShaderPath + ".pp.hlsl";
            else
                output += " /Fo " + ShaderPath + ".o";

            output += " " + ShaderPath;
            return output;
        }
    }
}