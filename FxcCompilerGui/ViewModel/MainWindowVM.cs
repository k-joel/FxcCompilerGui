using FxcCompilerGui.Model;
using FxcCompilerGui.Utils;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace FxcCompilerGui.ViewModel
{
    internal class MainWindowVM : BaseVM
    {
        public MainWindowVM()
        {
            //
            string fxcPath = Properties.Settings.Default.FxcCompilerPath;
            string shaderPath = Properties.Settings.Default.PrevShaderPath;
            int profileIndex = Properties.Settings.Default.PrevProfileIndex;

            // Use the Property.Settings data instead
            //LoadXmlSettings(out fxcLoc, out shaderLoc);

            _fxcWrapper = new FxcCompilerWrapper(fxcPath, shaderPath, profileIndex);
            _fxcWrapper.Output.BufferChanged += (sender, args) => RaisePropertyChanged("OutputLogString");

            Defines = new ObservableCollection<string>(_fxcWrapper.Defines);
            Includes = new ObservableCollection<string>(_fxcWrapper.Includes);
        }

        public event EventHandler RequestClose;

        public static string[] ProfileStrings => FxcCompilerWrapper.ProfileStrings;

        public static string[] OptLevelStrings => FxcCompilerWrapper.OptimizationLevelStrings;

        public string OutputLogString => _fxcWrapper.Output.Buffer;

        public string CommandLineString => _fxcWrapper.BuildCommandLineString();

        public string FxcCompilerPath
        {
            get { return _fxcWrapper.FxcCompilerPath; }
            set
            {
                _fxcWrapper.FxcCompilerPath = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public string ShaderPath
        {
            get { return _fxcWrapper.ShaderPath; }
            set
            {
                _fxcWrapper.ShaderPath = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public int ProfileStringIndex
        {
            get { return _fxcWrapper.ProfileIndex; }
            set
            {
                if (value == -1) return;
                _fxcWrapper.ProfileIndex = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public int OptLevelStringIndex
        {
            get { return _fxcWrapper.OptimizationLevel; }
            set
            {
                if (value == -1) return;
                _fxcWrapper.OptimizationLevel = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public string EntryPoint
        {
            get { return _fxcWrapper.EntryPoint; }
            set
            {
                _fxcWrapper.EntryPoint = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool DisableOpt
        {
            get { return _fxcWrapper.DisableOptimizations; }
            set
            {
                _fxcWrapper.DisableOptimizations = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool UseLegacyCompiler
        {
            get { return _fxcWrapper.UseLegacyCompiler; }
            set
            {
                _fxcWrapper.UseLegacyCompiler = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool EnableDebugInfo
        {
            get { return _fxcWrapper.EnableDebugInfo; }
            set
            {
                _fxcWrapper.EnableDebugInfo = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool TreatWarningsAsErrors
        {
            get { return _fxcWrapper.TreatWarningsAsErrors; }
            set
            {
                _fxcWrapper.TreatWarningsAsErrors = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool AvoidFlowControl
        {
            get { return _fxcWrapper.AvoidFlowControl; }
            set
            {
                _fxcWrapper.AvoidFlowControl = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool PreferFlowControl
        {
            get { return _fxcWrapper.PreferFlowControl; }
            set
            {
                _fxcWrapper.PreferFlowControl = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool DisableEffectPerfMode
        {
            get { return _fxcWrapper.DisableEffectPerformanceMode; }
            set
            {
                _fxcWrapper.DisableEffectPerformanceMode = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool EnableBackwardsCompatMode
        {
            get { return _fxcWrapper.EnableBackwardsCompatMode; }
            set
            {
                _fxcWrapper.EnableBackwardsCompatMode = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool EnableStrictMode
        {
            get { return _fxcWrapper.EnableStrictMode; }
            set
            {
                _fxcWrapper.EnableStrictMode = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool DisableValidation
        {
            get { return _fxcWrapper.DisableValidation; }
            set
            {
                _fxcWrapper.DisableValidation = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool OutputAsm
        {
            get { return _fxcWrapper.OutputAssembly; }
            set
            {
                _fxcWrapper.OutputAssembly = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool OutputPdb
        {
            get { return _fxcWrapper.OutputPdbInfo; }
            set
            {
                _fxcWrapper.OutputPdbInfo = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool OutputLib
        {
            get { return _fxcWrapper.OutputLibrary; }
            set
            {
                _fxcWrapper.OutputLibrary = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool OutputWarnErr
        {
            get { return _fxcWrapper.OutputWarnAndErr; }
            set
            {
                _fxcWrapper.OutputWarnAndErr = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public bool PreprocessOnly
        {
            get { return _fxcWrapper.PreprocessOnly; }
            set
            {
                _fxcWrapper.PreprocessOnly = value;
                RaisePropertyChanged();
                RaiseCmdLinePropertyChanged();
            }
        }

        public string SelectedDefine
        {
            get { return _selectedDefine; }
            set
            {
                if (value == null && Defines.Contains(_selectedDefine)) return;
                _selectedDefine = value;
                RaisePropertyChanged();
            }
        }

        public string NewDefine
        {
            get { return SelectedDefine; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!Defines.Contains(value))
                        Defines.Add(value);
                }
                else if (!string.IsNullOrEmpty(SelectedDefine))
                {
                    Defines.Remove(SelectedDefine);
                }

                SelectedDefine = value;
            }
        }

        public string SelectedInclude
        {
            get { return _selectedInclude; }
            set
            {
                if (value == null && Includes.Contains(_selectedInclude)) return;
                _selectedInclude = value;
                RaisePropertyChanged();
            }
        }

        public string NewInclude
        {
            get { return SelectedInclude; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!Includes.Contains(value))
                        Includes.Add(value);
                }
                else if (!string.IsNullOrEmpty(SelectedInclude))
                {
                    Includes.Remove(SelectedInclude);
                }

                SelectedInclude = value;
            }
        }

        public ObservableCollection<string> Defines { get; private set; }

        public ObservableCollection<string> Includes { get; private set; }

        public DelegateCommand CompileCommand => _compileCommand ?? (_compileCommand = new DelegateCommand(OnCompileCommand));

        public DelegateCommand BrowseShaderCommand => _browseShaderCommand ?? (_browseShaderCommand = new DelegateCommand(OnBrowseShaderCommand));

        public DelegateCommand BrowseFxcPathCommand => _browseFxcPathCommand ?? (_browseFxcPathCommand = new DelegateCommand(OnBrowseFxcPathCommand));

        public DelegateCommand ClearLogCommand => _clearLog ?? (_clearLog = new DelegateCommand(OnClearLogCommand));

        public DelegateCommand ExitCommand => _exitCommand ?? (_exitCommand = new DelegateCommand(OnExitCommand));

        private void OnExitCommand(object obj)
        {
            RaiseRequestClose();
        }

        private void RaiseCmdLinePropertyChanged()
        {
            RaisePropertyChanged("CommandLineString");
        }

        private void RaiseRequestClose()
        {
            var handler = RequestClose;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnBrowseFxcPathCommand(object obj)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = GetValidDirectoryOrCurrent(FxcCompilerPath);
            ofd.Title = "Open the FXC Compiler";
            ofd.Filter = "Fxc|fxc.exe";
            ofd.FilterIndex = 0;
            if (ofd.ShowDialog() == true)
                FxcCompilerPath = ofd.FileName;
        }

        private void OnBrowseShaderCommand(object obj)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = GetValidDirectoryOrCurrent(ShaderPath);
            ofd.Title = "Open an HLSL shader file";
            ofd.Filter = "Effects|*.fx|VertexShaders|*.vsh|PixelShaders|*.psh|GeometryShaders|*.gsh|HLSL|*.hlsl|All Files|*.*";
            ofd.FilterIndex = 0;
            if (ofd.ShowDialog() == true)
                ShaderPath = ofd.FileName;
        }

        private void OnClearLogCommand(object obj)
        {
            _fxcWrapper.Output.Clear();
        }

        private void OnCompileCommand(object obj)
        {
            _fxcWrapper.Compile();
        }

        private string GetValidDirectoryOrCurrent(string path)
        {
            string dir = Environment.CurrentDirectory;
            if (!string.IsNullOrEmpty(path))
            {
                string tmpdir = Path.GetDirectoryName(Path.GetFullPath(path));
                if (Directory.Exists(tmpdir))
                    dir = tmpdir;
            }

            return dir;
        }

        private void LoadXmlSettings(out string fxcPathOut, out string shaderPathOut)
        {
            string fxcPath = null, shaderPath = null;
            try
            {
                var xml = XmlReader.Create("settings.xml");
                while (xml.Read())
                {
                    if (xml.NodeType != XmlNodeType.Element) continue;
                    switch (xml.Name)
                    {
                        case "FxcCompilerPath":
                        xml.Read();
                        fxcPath = xml.Value;
                        break;

                        case "PreviousShaderPath":
                        xml.Read();
                        shaderPath = xml.Value;
                        break;
                    }
                }
                xml.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                fxcPath = null;
                shaderPath = null;
            }

            fxcPathOut = fxcPath;
            shaderPathOut = shaderPath;
        }

        private void SaveXmlSettings()
        {
            var xml = XmlWriter.Create("settings.xml", new XmlWriterSettings { Indent = true });
            xml.WriteStartDocument();
            xml.WriteStartElement("Settings");
            xml.WriteElementString("FxcCompilerPath", _fxcWrapper.FxcCompilerPath);
            xml.WriteElementString("PreviousShaderPath", _fxcWrapper.ShaderPath);
            xml.WriteEndElement();
            xml.WriteEndDocument();
            xml.Close();
        }

        public void OnClosing()
        {
            //SaveXmlSettings();

            Properties.Settings.Default.FxcCompilerPath = _fxcWrapper.FxcCompilerPath;
            Properties.Settings.Default.PrevShaderPath = _fxcWrapper.ShaderPath;
            Properties.Settings.Default.PrevProfileIndex = _fxcWrapper.ProfileIndex;
            Properties.Settings.Default.Save();
        }

        private string _selectedDefine;
        private string _selectedInclude;

        private DelegateCommand _compileCommand;
        private DelegateCommand _browseShaderCommand;
        private DelegateCommand _browseFxcPathCommand;
        private DelegateCommand _clearLog;
        private DelegateCommand _exitCommand;

        private readonly FxcCompilerWrapper _fxcWrapper;
    }
}