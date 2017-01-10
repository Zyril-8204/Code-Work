#define RUNNING_NET45 // Put this as first line of file
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XamarinApp.Common;
using XamarinApp.LocalDb.Helpers;
using XamarinApp.Logging;
using XamarinApp.Settings;
using XamarinApp.ViewModels;
using static XamarinApp.Xapp;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xamarin.Forms;

namespace XamarinApp
{
    public class ViewModelBase : Models.ModelBase
    {
        #region Inheritable convenience objects
        /// <summary>Convenient way for each VM to reach up to the AppVM
        /// 
        /// </summary>
        [DoNotPersist]
        [XmlIgnore]
        [JsonIgnore]
        public XappViewModel AppVM => Application.Current.BindingContext as XappViewModel;

        #region fileHelper (IFileHelper)
        private IFileHelper _fileHelper;
        [DoNotPersist]
        [XmlIgnore]
        [JsonIgnore]
        public IFileHelper FileHelper
        {
            [DebuggerStepThrough]
            get
            {
                if (_fileHelper == null) FileHelper = DependencyService.Get<IFileHelper>();
                return _fileHelper;
            }

            [DebuggerStepThrough]
            set
            {
                if (_fileHelper == value) return;
                _fileHelper = value;
            }
        }
        #endregion fileHelper  (IFileHelper)

        #endregion Inheritable convenience objects 

        #region Logging
        #region logger
        private ILogger _logger;
        [DoNotPersist]
        [XmlIgnore]
        [JsonIgnore]
        public ILogger logger
        {
            [DebuggerStepThrough]
            get
            {
                return _logger;
            }

            [DebuggerStepThrough]
            set
            {
                if (_logger == value) return;
                OnPropertyChanging(() => logger);
                _logger = value;
                OnPropertyChanged(() => logger);
            }
        }
        #endregion logger

        /// <summary>Log file name will correspond to creating module
        /// 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public virtual bool ConfigureLogger(string moduleName)
        {
            string logs = Xapp.GetResource("LogDirectoryPath") as string;

            if (FileHelper != null)
            {
                LogTypes logType = LogHelper.GetLogType();
                logs = FileHelper.CreateFolder(logs);

                // break the loggerconfiguration. chaining to .Logger(Action<>....) always defaults to Information level 
                var logConfig = new LoggerConfiguration()
                    .MinimumLevel
                    .Is(LogHelper.GetMinLevel(moduleName));

                if (logType == LogTypes.FILE || logType == LogTypes.BOTH)
                {
                    string fileName = System.IO.Path.ChangeExtension(moduleName, "txt");
                    string logFilePath = System.IO.Path.Combine(logs, fileName);
                    var stream = FileHelper.CreateTextStreamWriter(logFilePath);
                    if (stream != null)
                    {
                        logConfig.WriteTo.TextWriter(stream);
                    }
                }

                if (logType == LogTypes.SQLITE || logType == LogTypes.BOTH)
                {
                    logConfig.WriteTo.SqliteLogger(FileHelper?.SqLitePlatform, moduleName);
                }

                Log.Logger = logConfig.CreateLogger();

                logger = Log.Logger;

#if DEBUG                
                logger?.Debug($"{moduleName.ToFirstCap()}VM created");
                //logger?.Information($"{moduleName.ToFirstCap()}VM created");
                //logger?.Warning($"{moduleName.ToFirstCap()}VM created");
                //logger?.Error($"{moduleName.ToFirstCap()}VM created");
#endif
                return true;
            }

            return false;
        }

        #endregion Logging 

        #region Constructor
        public ViewModelBase()
        {
        }

        #endregion // Constructor

        #region ITile properties
        #region TileColor

        private Xamarin.Forms.Color _TileColor = Xamarin.Forms.Color.Fuchsia;
        [DoNotPersist]
        [XmlIgnore]
        [JsonIgnore]
        public Xamarin.Forms.Color TileColor
        {
            [DebuggerStepThrough]
            get { return _TileColor; }

            [DebuggerStepThrough]
            set
            {
                if (_TileColor == value) return;
                OnPropertyChanging(() => TileColor);
                _TileColor = value;
                OnPropertyChanged(() => TileColor);
            }
        }

        #endregion TileColor


        #region Row1Number (string)
        private string _Row1Number;
        [XmlIgnore]
        [JsonIgnore]
        [DoNotPersist]
        public string Row1Number
        {
            [DebuggerStepThrough]
            get
            {
                //if (_Row1Number == null) Row1Number = new string?();
                return _Row1Number;
            }

            [DebuggerStepThrough]
            set
            {
                if (_Row1Number == value) return;
                OnPropertyChanging(() => Row1Number);
                _Row1Number = value;
                OnPropertyChanged(() => Row1Number);
                //UpdateDynamicSetting(()=> Row1Number, value);
            }
        }
        #endregion Row1Number  (string)

        #region Row2Number (string)
        private string _Row2Number;
        [XmlIgnore]
        [JsonIgnore]
        [DoNotPersist]
        public string Row2Number
        {
            [DebuggerStepThrough]
            get
            {
                //if (_Row2Number == null) Row2Number = new string?();
                return _Row2Number;
            }

            [DebuggerStepThrough]
            set
            {
                if (_Row2Number == value) return;
                OnPropertyChanging(() => Row2Number);
                _Row2Number = value;
                OnPropertyChanged(() => Row2Number);
                //UpdateDynamicSetting(()=> Row2Number, value);
            }
        }
        #endregion Row2Number  (string)


        #region Row1Text

        private string _Row1Text = "";
        [DoNotPersist]
        [XmlIgnore]
        [JsonIgnore]
        public string Row1Text
        {
            [DebuggerStepThrough]
            get { return _Row1Text; }

            [DebuggerStepThrough]
            set
            {
                if (_Row1Text == value) return;
                OnPropertyChanging(() => Row1Text);
                _Row1Text = value;
                OnPropertyChanged(() => Row1Text);
            }
        }

        #endregion Row1Text

        #region Row2Text

        private string _Row2Text = "";
        [DoNotPersist]
        [XmlIgnore]
        [JsonIgnore]
        public string Row2Text
        {
            [DebuggerStepThrough]
            get { return _Row2Text; }

            [DebuggerStepThrough]
            set
            {
                if (_Row2Text == value) return;
                OnPropertyChanging(() => Row2Text);
                _Row2Text = value;
                OnPropertyChanged(() => Row2Text);
            }
        }

        #endregion Row2Text

        #region TileName

        private string _TileName = "VMB";
        [DoNotPersist]
        [XmlIgnore]
        [JsonIgnore]
        public string TileName
        {
            [DebuggerStepThrough]
            get { return _TileName; }

            [DebuggerStepThrough]
            set
            {
                if (_TileName == value) return;
                OnPropertyChanging(() => TileName);
                _TileName = value;
                OnPropertyChanged(() => TileName);
            }
        }

        #endregion TileName

        #region TileIcon
        //private string _TileIcon = "locationList-tile-icon-red.png";
        private string _TileIcon = "icon.png";
        //private string _TileIcon = "icon2.png";
        [DoNotPersist]
        [XmlIgnore]
        [JsonIgnore]
        public string TileIcon
        {
            [DebuggerStepThrough]
            get
            {
                if (Device.OS == TargetPlatform.Windows  && !string.IsNullOrEmpty(_TileIcon))
                {
                    return System.IO.Path.Combine("Assets", _TileIcon);
                }

                return _TileIcon;
            }

            [DebuggerStepThrough]
            set
            {
                if (_TileIcon == value) return;
                _TileIcon = value;
                OnPropertyChanged(() => TileIcon);
            }
        }
        #endregion TileIcon

        #region ConnectionIcon
        //private string _ConnectionIcon = "locationList-tile-icon-red.png";
        private string _ConnectionIcon = string.Empty;
        //private string _ConnectionIcon = "icon2.png";
        [DoNotPersist]
        [XmlIgnore]
        [JsonIgnore]
        public string ConnectionIcon
        {
            [DebuggerStepThrough]
            get
            {
                if (Device.OS == TargetPlatform.Windows)
                {
                    return System.IO.Path.Combine("Assets", _ConnectionIcon);
                }

                return _ConnectionIcon;
            }
            [DebuggerStepThrough]
            set
            {
                if (_ConnectionIcon == value) return;
                _ConnectionIcon = value;
                OnPropertyChanged(() => ConnectionIcon);
            }
        }
        #endregion ConnectionIcon


        #endregion ITile properties

        #region VMsettingsBlock (SettingsBlock)
        private SettingsBlock _VMsettingsBlock;
        [DoNotPersist]
        [JsonIgnore]
        [XmlIgnore]
        public SettingsBlock VMsettingsBlock
        {
            [DebuggerStepThrough]
            get
            {
                if(_VMsettingsBlock == null)
                    VMsettingsBlock = AppVM?.SettingsVM?.LoadSettingsFile(this.TileName);

                return _VMsettingsBlock;
            }

            [DebuggerStepThrough]
            set
            {
                if (_VMsettingsBlock == value) return;
                OnPropertyChanging(() => VMsettingsBlock);
                _VMsettingsBlock = value;
                OnPropertyChanged(() => VMsettingsBlock);
            }
        }
        #endregion VMsettingsBlock  (SettingsBlock)

        #region IsBusy (bool)
        private int _IsBusy;
        /// <summary>Binded to the ActivityIndicator in the title bar to tell the user we are just busy not frozen
        /// 
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [DoNotPersist]
        public bool IsBusy
        {
            //[DebuggerStepThrough]
            get
            {
                //if (_IsBusy == null) IsBusy = new bool();
                return _IsBusy > 0;
            }

            //[DebuggerStepThrough]
            set
            {
                //if (_IsBusy == value) return;
                OnPropertyChanging(() => IsBusy);
                //_IsBusy = value;
                if (value) _IsBusy++;
                if (!value) _IsBusy--;
                _IsBusy = _IsBusy < 0 ? 0 : _IsBusy;
                OnPropertyChanged(() => IsBusy);
                AppVM.IsBusy = value;//Allows us to track WHO is busy on an individual basis as well as let the app show the indicator if ANY child VM is busy.
            }
        }

        #endregion IsBusy  (bool)


        protected bool IsFirstLogin = true;

        /// <summary>Call to LoadPersistentData() as well as logging;
        /// 
        /// </summary>
        public virtual void OnStart()
        {
            logger?.Information("{tilename} started. Whir, click beep", this.TileName);
            if (IsFirstLogin)
            {
                logger?.Information("{tilename} Loading persistent data", this.TileName);
                LoadPersistentData();
            }

            IsDirty = false;

        }

        /// <summary>Logs going to sleep
        /// 
        /// </summary>
        public virtual void OnSleep()
        {
            logger?.Information("{tilename} Being backgrounded.  Nighty night ZZzzzz...", this.TileName);
        }

        /// <summary>Logs waking up
        /// 
        /// </summary>
        public virtual void OnResume()
        {
            logger?.Information("{tilename} Waking up", this.TileName);
        }

        public virtual void OnPause()
        { }

        /// <summary>Logs closing
        /// 
        /// </summary>
        public virtual void OnClose()
        {
            logger?.Information("{tilename} Closing", this.TileName);
        }
    }
}

