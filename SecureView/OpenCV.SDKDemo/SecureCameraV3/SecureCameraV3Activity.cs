using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenCV.Core;
using OpenCV.Android;
using Android.Util;
using Size = Android.Hardware.Camera.Size;
using Java.Text;
using Java.Util;
using Java.Lang;
using JavaObject = Java.Lang.Object;
using Android.Content.PM;

using Tesseract.Droid;
using System.Threading.Tasks;
using Android.Graphics;
using System.IO;
using OpenCV.ImgProc;

namespace OpenCV.SDKDemo.SecureCameraV3
{
    [Activity(Label = "SecureCameraV3Activity"
     ,ScreenOrientation = ScreenOrientation.Landscape
     /*, Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen"*/)]
    public class SecureCameraV3Activity : Activity, CameraBridgeViewBase.ICvCameraViewListener2
    {
        public const string TAG = "OCVSample::SecureCameraV3Activity";

        private SecureCameraV3View mOpenCvCameraView;
        private Button DecryptButton;
        private ImageView imageView;
        private List<Size> mResolutionList;
        private IMenuItem[] mEffectMenuItems;
        private ISubMenu mColorEffectsMenu;
        private IMenuItem[] mResolutionMenuItems;
        private ISubMenu mResolutionMenu;
        private BaseLoaderCallback mLoaderCallback;
        bool PictureMode = true;

        private static readonly Scalar RECT_COLOR = new Scalar(0, 255, 0, 255);
        //Mat mRgba;
        private Mat mRgba;
        TesseractApi api;

        public SecureCameraV3Activity()
        {
            Log.Info(TAG, "Instantiated new " + GetType().ToString());
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Info(TAG, "called onCreate");
            base.OnCreate(savedInstanceState);

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            api = new TesseractApi(Application.Context, AssetsDeployment.OncePerInitialization);

            Task.Run(() => api.Init("eng")).Wait();

            SetContentView(Resource.Layout.SecureCameraV3View);

            mOpenCvCameraView = FindViewById<SecureCameraV3View>(Resource.Id.securecamerav3_java_surface_view);
            DecryptButton = FindViewById<Button>(Resource.Id.decryptButton);
            imageView = FindViewById<ImageView>(Resource.Id.picturedisplay);

            mOpenCvCameraView.setData(imageView, api/*, mRgba*/);
            DecryptButton.Click += DecryptButton_Click;

            imageView.Visibility = ViewStates.Gone;
            mOpenCvCameraView.Visibility = ViewStates.Visible;

            mOpenCvCameraView.SetCvCameraViewListener2(this);
            mLoaderCallback = new Callback(this, mOpenCvCameraView);
            //mLoaderCallback = new Callback(this, mOpenCvCameraView, this);


        }

        private void DecryptButton_Click(object sender, EventArgs e)
        {
            Log.Info(TAG, "onTouch event");

            SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd_HH-mm-ss");
            string currentDateandTime = sdf.Format(new Date());
            string fileName = global::Android.OS.Environment.ExternalStorageDirectory.Path +
                                   "/sample_picture_" + currentDateandTime + ".jpg";

            mOpenCvCameraView.TakePicture(PictureMode, fileName);

            if (PictureMode)
            {
                Toast.MakeText(this, fileName + " saved", ToastLength.Short).Show();
            }

            PictureMode = !PictureMode;
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (mOpenCvCameraView != null)
                mOpenCvCameraView.DisableView();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!OpenCVLoader.InitDebug())
            {
                Log.Debug(TAG, "Internal OpenCV library not found. Usin?g OpenCV Manager for initialization");
                OpenCVLoader.InitAsync(OpenCVLoader.OpencvVersion300, this, mLoaderCallback);
            }
            else
            {
                Log.Debug(TAG, "OpenCV library found inside package. Using it!");
                mLoaderCallback.OnManagerConnected(LoaderCallbackInterface.Success);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mOpenCvCameraView != null)
                mOpenCvCameraView.DisableView();
        }

        public void OnCameraViewStarted(int width, int height)
        {
            //mOpenCvCameraView.setFramrate(10);
            //mOpenCvCameraView.setFramrate(10);
            mRgba = new Mat();
            mOpenCvCameraView.setPictureSize(1280, 720);
            //mOpenCvCameraView.setResolution2(640, 480);
            //mOpenCvCameraView.setFramrate(8);
            Toast.MakeText(this, "YO !!!!", ToastLength.Short).Show();
        }

        public void OnCameraViewStopped()
        {
            mRgba.Release();
        }

        public Mat OnCameraFrame(CameraBridgeViewBase.ICvCameraViewFrame inputFrame)
        {
            mRgba = inputFrame.Rgba();

            //if (OCRGo)
            //{
            //    try
            //    {
            //        //Bitmap bitmap = Bitmap.CreateBitmap(mRgba.Cols(), mRgba.Rows(), Bitmap.Config.Argb8888);
            //        Bitmap bitmap = Bitmap.CreateBitmap(mRgba.Cols(), mRgba.Rows(), Bitmap.Config.Argb8888);
            //        OpenCV.Android.Utils.MatToBitmap(mRgba, bitmap);

            //        //using(var stream = new ByteArrayOutputStream())
            //        using (MemoryStream stream = new MemoryStream())
            //        {
            //            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 75, stream);
            //            byte[] bitmapData = stream.ToArray();


            //            //SearchText(bitmapData);

            //            bool success = api.SetImage(bitmapData).Result;
            //            if (success)
            //            {
            //                IEnumerable<Tesseract.Result> symbols = api.Results(Tesseract.PageIteratorLevel.Symbol);
            //                foreach (Tesseract.Result symbol in symbols)
            //                {
            //                    Imgproc.PutText(mRgba, symbol.Text, new Core.Point(symbol.Box.X, symbol.Box.Y),
            //                    Core.Core.FontHersheySimplex, 1.0, new Scalar(255, 255, 0));


            //                    Imgproc.Rectangle(mRgba, new Core.Point(symbol.Box.X, symbol.Box.Y), new Core.Point(symbol.Box.X + symbol.Box.Width, symbol.Box.Y + symbol.Box.Height), RECT_COLOR);
            //                }
            //            }
            //        }


            //        //Imgproc.PutText(mRgba, mResources.GetString(Resource.String.undistorted), new Point(mWidth * 0.6, mHeight * 0.1),
            //        //        Core.Core.FontHersheySimplex, 1.0, new Scalar(255, 255, 0));

            //        //Imgproc.Rectangle(mRgba, new Core.Point(100, 100), new Core.Point(300, 300), RECT_COLOR);
            //    }
            //    catch { }
            //}

            return mRgba;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            IList<string> effects = mOpenCvCameraView.getEffectList();

            if (effects == null)
            {
                Log.Error(TAG, "Color effects are not supported by device!");
                return true;
            }

            mColorEffectsMenu = menu.AddSubMenu("Color Effect");
            mEffectMenuItems = new IMenuItem[effects.Count];

            int idx = 0;
            foreach (var item in effects)
            {
                string element = item;
                mEffectMenuItems[idx] = mColorEffectsMenu.Add(1, idx, Menu.None, element);
                idx++;
            }

            mResolutionMenu = menu.AddSubMenu("Resolution");
            mResolutionList = mOpenCvCameraView.getResolutionList();
            mResolutionMenuItems = new IMenuItem[mResolutionList.Count];


            idx = 0;
            foreach (var item in mResolutionList)
            {
                Size element = item;
                mResolutionMenuItems[idx] = mResolutionMenu.Add(2, idx, Menu.None,
                        element.Width.ToString() + "x" + element.Height.ToString());
                idx++;
            }

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Log.Info(TAG, "called onOptionsItemSelected; selected item: " + item);
            if (item.GroupId == 1)
            {
                mOpenCvCameraView.setEffect((string)item.TitleFormatted.ToString());
                Toast.MakeText(this, mOpenCvCameraView.getEffect(), ToastLength.Short).Show();
            }
            else if (item.GroupId == 2)
            {
                int id = item.ItemId;
                Size resolution = mResolutionList[id];
                mOpenCvCameraView.setResolution(resolution);
                resolution = mOpenCvCameraView.getResolution();
                string caption = resolution.Width.ToString() + "x" + resolution.Height.ToString();
                Toast.MakeText(this, caption, ToastLength.Short).Show();
            }

            return true;
        }

    }

    class Callback : BaseLoaderCallback
    {
        private readonly Context _context;
        private readonly SecureCameraV3View mOpenCvCameraView;


        public Callback(Context context, SecureCameraV3View view)
            : base(context)
        {
            _context = context;
            mOpenCvCameraView = view;
            //imageView = imageview;
        }
        public override void OnManagerConnected(int status)
        {
            switch (status)
            {
                case LoaderCallbackInterface.Success:
                    {
                        Log.Info(SecureCameraV3Activity.TAG, "OpenCV loaded successfully");
                        mOpenCvCameraView.EnableView();
                        //imageView.SetOnTouchListener(_listener);
                    }
                    break;
                default:
                    {
                        base.OnManagerConnected(status);
                    }
                    break;
            }
        }
    }
}