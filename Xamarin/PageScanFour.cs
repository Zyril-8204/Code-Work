using System;
using System.Diagnostics;
using Android.Graphics;
using XamarinApp.ViewModels;
using Pdi.Android;
using Pdi.Barlib;
using Java.Nio;
using Exception = System.Exception;
using Object = Java.Lang.Object;
using XamarinApp.Common;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using XamarinApp.Common.Scan;
using XamarinApp.HoursOfService.Repository.Tables;

namespace XamarinApp.Droid.Scanning
{
    public class PageScanFour : Object, IDocumentScanner, Pdiscan.ICallback
    {
        private bool verbose = true;
        /// <summary>
        ///We pass this activity to the PageScanFour because the driver requires
        ///certain events & commands to be sent on the same activity that
        ///did the initialize, even though other things such as hearing
        ///PageEnd events can take place on their own object-thread.
        /// </summary>
        private MainActivity mainActivity;
        private string _workingSaveDirectory;


        #region Constructor(s)

        public PageScanFour(MainActivity ma)
        {
            mainActivity = ma;
            imagingHelper = new ImagingHelper();

            RaiseLogThis("Initialize");
            Pdiscan.Initialize(mainActivity);

            if (verbose) RaiseLogThis("Subscribe");
            Pdiscan.SetCallback(this);

            try
            {
                Pdiscan.Connect();

                if (Pdiscan.IsConnected) ConfigureDevice();
                RaiseConnectivityChanged(IsConnected);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GET Error {ex.Message}");
            }

        }

        #endregion Constructor(s) 

        #region Events

        #region LogThis event
        public event EventHandler<string> LogThis;

        protected void RaiseLogThis(string e)
        {
            EventHandler<string> handler = LogThis;
            handler?.Invoke(this, e);
        }
        #endregion LogThis

        #region NewImage event
        /// <summary>Argument is the path to the saved bitmap
        /// 
        /// </summary>
        public event EventHandler<ScanInfo> NewImage;

        protected void RaiseNewImage(ScanInfo e)
        {
            EventHandler<ScanInfo> handler = NewImage;
            handler?.Invoke(this, e);
        }
        #endregion NewImage

        #region ConnectivityChanged event
        /// <summary>Bool event arg indicates if is now connected.  You can also check the IsConnected property to confirm.
        /// 
        /// </summary>
        public event EventHandler<bool> ConnectivityChanged;


        protected void RaiseConnectivityChanged(bool e)
        {
            EventHandler<bool> handler = ConnectivityChanged;
            handler?.Invoke(this, e);
        }
        #endregion ConnectivityChanged


        #region RaisePaperJam		
        /// <summary>Bool event arg indicates if is now jammed.		
        /// 		
        /// </summary>		
        public event EventHandler<bool> PaperJam;
        protected void RaisePaperJam(bool e)
        {
            EventHandler<bool> handler = PaperJam;
            handler?.Invoke(this, e);
        }
        #endregion RaisePaperJam		

        #region DeviceFirstTimeConnect
        public event EventHandler DeviceFirstTimeConnect;
        protected void RaiseDeviceFirstTimeConnect()
        {
            EventHandler handler = DeviceFirstTimeConnect;
            handler?.Invoke(this, null);
        }
        #endregion DeviceFirstTimeConnect

        #endregion Events 

        #region Properties 

        #region ImagingHelper
        public XamarinApp.Common.ImagingHelper imagingHelper { get; set; }
        #endregion ImagingHelper

        #region IsConnected
        public bool IsConnected
        {
            get { return Pdiscan.IsConnected; }
        }
        #endregion IsConnected

        #region IsPaperJam
        private bool _IsPaperJam;
        public bool IsPaperJam
        {
            [DebuggerStepThrough]
            get
            {
                return _IsPaperJam;
            }
            [DebuggerStepThrough]
            set
            {
                if (_IsPaperJam == value) return;
                if (value == true)
                {
                    Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 1), StillJammed);
                }
                _IsPaperJam = value;
            }
        }
        #endregion IsPaperJam

        #region WorkingSaveDirectory
        private string _WorkingSaveDirectory;
        public string WorkingSaveDirectory
        {
            [DebuggerStepThrough]
            get
            {
                return _WorkingSaveDirectory;
            }

            [DebuggerStepThrough]
            set
            {
                if (_WorkingSaveDirectory == value) return;
                //OnPropertyChanging(() => WorkingSaveDirectory);
                _WorkingSaveDirectory = value;
                //NotifyPropertyChanged("WorkingSaveDirectory");
                //OnPropertyChanged(() => WorkingSaveDirectory);

                System.IO.Directory.CreateDirectory(value);
            }
        }

        public object BarcodeEngine { get; set; }

        #endregion WorkingSaveDirectory

        #region Brightness
        private object _Brightness = "20"; // default

        /// <summary>Expecting an int, but will convert a string or object
        /// 
        /// </summary>
        public object Brightness
        {
            [DebuggerStepThrough]
            get
            {
                //return _Brightness;
                return Pdiscan.BitonalThreshold.ToString();
            }

            //[DebuggerStepThrough]
            set
            {
                if (_Brightness == value) return;
                _Brightness = value;
                var brightness = 0;
                if (value is string)
                {
                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        int.TryParse(value.ToString(), out brightness);
                    }
                }
                else brightness = (int)value;
                brightness = brightness < 15 ? 15 : brightness;
                brightness = brightness > 85 ? 85 : brightness;

                if (IsConnected)
                {
                    var a = Pdiscan.CapBitonialThreshold();
                    DisableFeeder();
                    Pdiscan.BitonalThreshold = (int)brightness;
                }
                EnableFeeder();
            }
        }

        #endregion Brightness

        #endregion Properties

        #region App state change handlers 
        public bool OnResume()
        {
            try
            {
                if (!Pdiscan.IsConnected) Pdiscan.ReConnect();
                RaiseLogThis(string.Format("{0} by {1} is {2}",
                                    Pdiscan.Product,
                                    Pdiscan.Manufacturer,
                                    Pdiscan.IsConnected ? "ready" : "DISconnected"));
                RaiseConnectivityChanged(IsConnected);

                return true;
            }
            catch (Exception ex)
            {
                RaiseLogThis(string.Format("Error: {0}", ex.Message));
                return false;
            }
        }

        #endregion App state change handlers

        private bool isFirstScan = true;



        #region PDIScan ICallback members

        public bool StillJammed()
        {
            Pdiscan.EjectPage(Pdiscan.E_ejectDirection.Front, true);
            Eject();
            EnableFeeder();
            IsPaperJam = false;
            return false;
        }


        /// <summary>Pdiscan callback handler. Do not rename. Receives the new scan.
        /// 
        /// </summary>
        /// <param name="bmpFront"></param>
        /// <param name="bmpBack"></param>
        public void PageEnd(Bitmap bmpFront, Bitmap bmpBack)
        {
            try
            {
                RaiseLogThis("hit PageEnd");
                if (bmpFront == null)
                {
                    RaiseLogThis("bmpFront is null");
                    return;
                }
                //PS4 doesn't do double-sided so this is CYA
                bmpBack?.Recycle();
                RaiseLogThis("bmpBack recycled");



                DisableFeeder();
                RaiseLogThis("feeder disabled");
                RaiseLogThis(string.Format("New Scan {0}, {1}",
                                             bmpFront.GetBitmapInfo().Width,
                                            bmpFront.GetBitmapInfo().Height));


                var fileName = DateTime.Now.ToString("HHmmssfff");

                var savedPath = imagingHelper.ExportAsPNG(bmpFront, WorkingSaveDirectory, fileName);
                var barcodes = ReadBarcode(savedPath);
                ScanInfo info = new ScanInfo(savedPath, barcodes)
                {
                    ScannerVersion = $"{Pdiscan.Version} ({Pdiscan.FirmwareVersion})"
                };
                RaiseNewImage(info);

                return;/*
                //Important: Saves blocks until the entire file is written before we raise the 
                //notification. We can't just react to a new file notice because in that case the
                //the file is STARTED but not necessarily FINISHED.
                if (!string.IsNullOrWhiteSpace(savedPath) &&
                    System.IO.File.Exists(savedPath))
                {
                    var imageinfo = barcodes == null ? savedPath : savedPath + barcodes;
                    RaiseLogThis(string.Format("Image info: {0}", imageinfo));
                    RaiseNewImage(imageinfo);
                }
                //if (!Pdiscan.IsFeederEnabled)
                //{
                //    EnableFeeder();
                //   Let the VM decide when to re-enable the feeder, after its done processing
                //}
                bmpFront?.Recycle();
                bmpFront?.Dispose();
                RaiseConnectivityChanged(true);
                if (isFirstScan)
                {
                    isFirstScan = false;
                    Eject();
                }
                //Pdiscan.EjectPage(Pdiscan.E_ejectDirection.Rear, false);
                */
            }
            catch (Exception ex)
            {
                RaiseLogThis(string.Format("PageEnd Ex> {0}", ex.Message));
                RaiseLogThis(ex.InnerException?.Message);
            }
        }

        public void ScanningError(int errorNumber, string errorMessage)
        {
            try
            {
                switch (errorNumber)
                {
                    case Pdiscan.ErrorCodes.PageStart:
                        //The start of a scan isn't really an error as far as we are concerned
                        // so if we paper jam we don't set to front scanning
                        //Pdiscan.EjectPage(Pdiscan.E_ejectDirection.AutoRear, false);
                        break;
                    case Pdiscan.ErrorCodes.PageEnd:
                        //mainActivity.RunOnUiThread(() =>
                        //{
                        if (isFirstScan)
                        {
                            isFirstScan = false;
                            //DisableFeeder();
                            //bool wasEnabled = Pdiscan.IsFeederEnabled;
                            mainActivity.RunOnUiThread(() =>
                            {
                                DisableFeeder();
                                Pdiscan.EjectPage(Pdiscan.E_ejectDirection.Rear, true);
                            });

                            //Eject();
                            //if (!wasEnabled) DisableFeeder();
                        }
                        //});
                        break;
                    case Pdiscan.ErrorCodes.ScannerAvailable://No scanner attached at app launch. Now there is a scanner available for the first time.
                                                             ////TODO: Raise event for first connection instructions
                                                             //RaiseDeviceFirstTimeConnect();
                                                             //break;
                    case Pdiscan.ErrorCodes.ScannerAttached://Scanner that was already attached has re-attached to be more precise
                        ConfigureDevice();//Just in case it isn't the SAME scanner, or it went through a power cycle.
                        RaiseConnectivityChanged(true);
                        RaiseLogThis("Scanner Attached");
                        break;
                    case Pdiscan.ErrorCodes.ScannerDetached:
                        RaiseConnectivityChanged(false);
                        RaiseLogThis("Scanner detached");
                        break;
                    case Pdiscan.ErrorCodes.ScannerCoverOpen:
                    case Pdiscan.ErrorCodes.PaperJam://TODO: RaiseEvent for VM to handle
                        RaiseLogThis("Paper jammed");
                        IsPaperJam = true;
                        RaisePaperJam(true);
                        break;
                    default:
                        RaiseLogThis(string.Format("PS4Error> {0}, '{1}'",
                                                    errorNumber,
                                                    errorMessage));
                        break;
                }
            }
            catch (Exception ex)
            {
                RaiseLogThis(string.Format("PS4Error ex> {0}", ex.ToString()));
                if (ex.Message != null) RaiseLogThis(string.Format("PS4Error ex> {0}", ex.Message));
            }
        }

        // Keycode is our dev license with pdi, do not change!
        static int keyCode = 24196;
        //public pdibarcode BarcodeEngine;
        #endregion PDIScan ICallback members

        /// <summary>Return string list of barcodes, or null if none</string>
        /// <para><example></example></para>
        /// </summary>
        /// <param name="bmpPath"></param>
        /// <returns>Return string list of barcodes, or null if none</returns>
        public List<string> ReadBarcode(string bmpPath)
        {
            if (string.IsNullOrWhiteSpace(bmpPath)) return null;
            pdibarcode BarcodeEngine = this.BarcodeEngine as pdibarcode;

            //Let's try to open the file with Write permissions.  If that fails the file may still be getting written.
            DateTime giveUp = DateTime.Now.AddSeconds(10);
            while (DateTime.Now < giveUp)
            {
                try
                {
                    var dummyStream = File.OpenWrite(bmpPath);
                    dummyStream.Dispose();
                    dummyStream = null;
                    break;//We have success
                }
                catch (Exception)
                {
                    RaiseLogThis("Unable to open file to read barcodes");
                }
                //unable to open the file with adequate permissions to prove its done
                return null;
            }


            if (!BarcodeEngine.IsInitialized)
            {
                RaiseLogThis("Barcode engine refuses to initialize");
                return null;
            }
            try
            {
                // set an upper bounds for storage on byte array so we don't go out of memory.
                var byteArrayForBitmap = new byte[16 * 1024];
                var options = new BitmapFactory.Options();
                options.InTempStorage = byteArrayForBitmap;// set the bounds for the storage so we don't run out of memory while loading the scanned image
                Bitmap bitmap = BitmapFactory.DecodeFile(bmpPath, options);// go get the bitmap and load it.
                // scale the bitmap so we don't run out of memory when reading barcodes
                //Bitmap TmpBM = Bitmap.CreateScaledBitmap(bitmap, (int)(bitmap.GetBitmapInfo().Width / 2), (int)(bitmap.GetBitmapInfo().Height / 2), true);

                BarcodeEngine.SetBitmap(bitmap, 0);// 0 = default == 8000 pixels per meter == 203.2 DPI
                BarcodeEngine.SetSearchOption(pdibarcode.E_option.Scanmode, pdibarcode.SearchMode.ScHorzvert);
                BarcodeEngine.SetSearchOption(pdibarcode.E_option.BarcodeType, pdibarcode.BarcodeType.BcCodeall);

                /* 
                 * Each bar code must have a white(or quiet) zone in front and back of the bar code.
                 * The specifications of the various bar codes require a minimum of 0.25 inch.
                 * However in some cases this is not respected and this option can be used to change the quiet zone.
                 * Value is expressed in 1 / 100 of an inch.
                 * Default is 0.25 or 25.
                 * Please make sure the value is bigger than the widest white bar in the bar code.
                 * If not the white bars will be confused with the quiet zone and therefore the bar code will not read properly. 
                */
                // Images are scaled down by a factor of 2 so quietzone must be as well.
                BarcodeEngine.SetSearchOption(pdibarcode.E_option.Quietzone, (25));
                // If this value is zero then the bar code reader will
                // stop reading bar-codes once it has found one bar code.
                // In case this value is set to none zero, the reader will 
                // continue to look for more bar codes until it has exhausted 
                // all possibilities.
                // In other words 0 or non-zero as a binary. 0-Multiplebarcodes=false. !0-Multiplebarcodes=true
                BarcodeEngine.SetSearchOption(pdibarcode.E_option.Multiplebarcodes, 1);
                int result = BarcodeEngine.DecodeBarcodes();
                var results = new List<string>();
                if (result != 0)
                {
                    for (int i = 1; i <= result; i++)
                    {
                        var r = BarcodeEngine.GetBarcodeValue(i);
                        results.Add(r);
                    }
                }
                if (!results.Any()) return null;
                return results;/*
                bitmap?.Recycle();
                bitmap?.Dispose();

                if (results.Count >= 1)
                {
                    var barcodes = new StringBuilder();
                    foreach (var r in results)
                    {
                        barcodes.Append("," + r);
                    }
                    RaiseLogThis(string.Format("Barcodes found: {0}", barcodes.ToString()));
                    return barcodes.ToString();
                }*/
            }
            catch (Exception ex)
            {
                RaiseLogThis(string.Format("PageScanFour > ReadBarcode > ex: {0}",
                                                ex.Message));
                return null;
            }
            return null;
        }

        void DisableFeeder()
        {
            if (!Pdiscan.IsConnected) return;
            try
            {
                // ReSharper disable once UseNullPropagation
                if (mainActivity != null)
                {
                    mainActivity.RunOnUiThread(() =>
                    {
                        if (verbose) RaiseLogThis("PageEnd> Feed DISable-ING");
                        Pdiscan.DisableFeeder();
                        if (verbose) RaiseLogThis("PageEnd> Feeder DISable-ED");
                    });
                }

            }
            catch (Exception ex)
            {
                RaiseLogThis(string.Format("PS4Error ex> {0}", ex.Message));
            }
        }
        public void EnableFeeder()
        {
            if (!Pdiscan.IsConnected) return;
            try
            {
                if (mainActivity != null)
                {
                    mainActivity.RunOnUiThread(() =>
                    {
                        //if (isFirstScan)
                        //{
                        //    isFirstScan = false;
                        //    Eject();
                        //}
                        Pdiscan.EjectPage(Pdiscan.E_ejectDirection.AutoRear, false);

                        //if (verbose) RaiseLogThis("PageEnd> Feed Enable-ING");
                        Pdiscan.EnableFeeder();
                        //if (verbose) RaiseLogThis("PageEnd> Feeder Enabl-ED");
                        RaiseConnectivityChanged(true);
                    });
                }

            }
            catch (Exception ex)
            {
                if (verbose) RaiseLogThis(string.Format("PS4Error ex> {0}", ex.Message));
            }
        }

        public void Eject()
        {
            if (!Pdiscan.IsConnected) return;
            DisableFeeder();
            //Pdiscan.EjectPage(Pdiscan.E_ejectDirection.Rear, false);
            Pdiscan.EjectPage(Pdiscan.E_ejectDirection.AutoRear, false);
        }

        public async void ConfigureDevice()
        {
            try
            {
                if (verbose) RaiseLogThis("Connect");
                //if (Pdiscan.IsConnected)
                //{
                //    Pdiscan.Disconnect();
                //    await Task.Delay(250);//I'm suspecting the PDI driver doesn't do well with races
                //}
                if (!Pdiscan.IsConnected)
                {
                    Pdiscan.Connect();
                    //await Task.Delay(250);//Returns out of method from here
                }
                if (Pdiscan.IsConnected)
                {
                    if (verbose) RaiseLogThis("Configure");
                    Pdiscan.Color = Pdiscan.E_colorDepth.Bw;
                    Pdiscan.TransportSpeed = Pdiscan.E_transportSpeed.Half;
                    Pdiscan.Resolution = Pdiscan.E_resolution.Dpi200;
                    Pdiscan.SinglePageMode = true;
                    // Set the maximum length of the document that will be scanned.
                    // This value will be used to set the jam detection on the scanner.
                    // This value is also used internally to limit the size of the memory buffers one needs while scanning.
                    // Setting the value to the lowest value needed in an application 
                    // will help in the Jam detection and with the memory management.
                    // Default - 1400
                    //Pdiscan.MaxDocumentLength = 1400; 

                    //await Task.Delay(250);//Returns out of method from here


                    Pdiscan.EjectPage(Pdiscan.E_ejectDirection.AutoRear, false);
                    EnableFeeder();
                    Pdiscan.EjectPage(Pdiscan.E_ejectDirection.Rear, false);//Alert the user its good

                    RaiseLogThis(string.Format("{0} by {1} is {2}",
                                        Pdiscan.Product,
                                        Pdiscan.Manufacturer,
                                        Pdiscan.IsConnected ? "ready" : "DISconnected"));
                    RaiseConnectivityChanged(IsConnected);
                }
            }
            catch (Exception ex)
            {
                RaiseLogThis(string.Format("Error subscribing to scanner {0}",
                                                ex.Message));
                //Pdiscan.Terminate();
            }
        }

        public string GetStatus()
        {
            var alpha = Pdiscan.Resolution;
            var bravo = Pdiscan.Color;
            var charlie = Pdiscan.IsConnected;
            var delta = Pdiscan.TransportSpeed;

            var status = string.Format("Res:{0}, Color:{1}, Trans:{2}, Con:{3}",
                alpha, bravo, delta, charlie);
            return status;
        }
    }
    [Flags]
    public enum pdiscan_errors
    {
        Rear_Left_Sensor_Covered = 0x01,
        Rear_Right_Sensor_Covered = 0x02,
        Brander_Position_Sensor_Covered = 0x04,
        Hi_Speed_Mode = 0x08,
        Download_Needed = 0x10,
        Future_Use_Not_Defined = 0x20,
        Scanner_Enabled = 0x40,
        Always_Set_to_1_0x80 = 0x80,
        Front1_Left_Sensor_Covered = 0x0100,
        Front2_M1_Sensor_Covered = 0x0200,
        Front3_M2_Sensor_Covered = 0x0400,
        Front4_M3_Sensor_Covered = 0x0800,
        Front5_M4_Sensor_Covered = 0x1000,
        Front6_M5_Sensor_Covered = 0x2000,
        Front_7_Right_Sensor_Covered = 0x4000,
        Always_Set_to_1_0x8000 = 0x8000,
        Scanner_Ready = 0x010000,
        XMT_Aborted_Com_Error = 0x020000,
        Document_Jam = 0x040000,
        Scan_Array_Pixel_Error = 0x080000,
        In_Diagnostic_Mode = 0x100000,
        Doc_in_Scanner = 0x200000,
        Calibration_of_unit_needed = 0x400000,
        Always_Set_to_1_0x800000 = 0x800000,
    }
}