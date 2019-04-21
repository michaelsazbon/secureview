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
using OpenCV.Android;
using OpenCV.Core;
using Android.Hardware;
using Android.Util;
using Java.IO;
using Size = Android.Hardware.Camera.Size;
using Graphics = Android.Graphics;
using Java.Text;
using Java.Util;
using Tesseract.Droid;
using OpenCV.ImgProc;
using System.Threading.Tasks;

namespace OpenCV.SDKDemo.SecureCameraV3
{
    public class SecureCameraV3View : JavaCameraView, Camera.IPictureCallback
    {
        private const string TAG = "SecureCameraV3View";
        private String mPictureFileName;
        ImageView imageView;
        TesseractApi api;
        
        private static readonly Scalar RECT_COLOR = new Scalar(0, 255, 0, 255);
        //bool PictureMode = true;

        public SecureCameraV3View(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {

        }

        public IList<String> getEffectList()
        {
            return MCamera.GetParameters().SupportedColorEffects;
        }

        public bool isEffectSupported()
        {
            return (MCamera.GetParameters().ColorEffect != null);
        }

        public String getEffect()
        {
            return MCamera.GetParameters().ColorEffect;
        }

        public void setEffect(String effect)
        {
            Camera.Parameters param = MCamera.GetParameters();
            //IList<Java.Lang.Integer> sp = param.SupportedPictureFormats;
            //IList<Size> sp2 = param.SupportedPictureSizes;
            param.ColorEffect = effect;
            MCamera.SetParameters(param);
        }

        public void setPictureSize(int width, int height)
        {
            Camera.Parameters param = MCamera.GetParameters();
            //List<Java.Lang.Integer> toto = param.SupportedPreviewFrameRates.ToList();
            //int y = 0;
            param.SetPictureSize(width, height);
            MCamera.SetParameters(param);
        }

        public void setData(ImageView ImageView, TesseractApi Api/*, Mat MRgba*/)
        {
            imageView = ImageView;
            api = Api;
            //mRgba = MRgba;
        }

        public void setFramrate(int fps)
        {
            Camera.Parameters param = MCamera.GetParameters();
            //List<Java.Lang.Integer> toto = param.SupportedPreviewFrameRates.ToList();
            //int y = 0;
            param.PreviewFrameRate = fps;
            MCamera.SetParameters(param);
        }

        public List<Size> getResolutionList()
        {
            return MCamera.GetParameters().SupportedPreviewSizes.ToList();
        }

        public void setResolution(Size resolution)
        {
            DisconnectCamera();
            MMaxHeight = (int)resolution.Height;
            MMaxWidth = (int)resolution.Width;
            ConnectCamera(Width, Height);
        }

        public void setResolution2(int width, int height)
        {
            DisconnectCamera();
            MMaxHeight = height;
            MMaxWidth = width;
            ConnectCamera(Width, Height);
        }

        public Size getResolution()
        {
            return MCamera.GetParameters().PreviewSize;
        }

        public void TakePicture(bool PictureMode, string fileName)
        {
            Log.Info(TAG, "Taking picture");

            this.mPictureFileName = fileName;
            // Postview and jpeg are sent in the same buffers if the queue is not empty when performing a capture.
            // Clear up buffers to avoid mCamera.takePicture to be stuck because of a memory issue
            //MCamera.SetPreviewCallback(null);

            // PictureCallback is implemented by the current class
            //MCamera.TakePicture(null, null, this);

            if (PictureMode)
            {
                //this.mPictureFileName = fileName;
                // Postview and jpeg are sent in the same buffers if the queue is not empty when performing a capture.
                // Clear up buffers to avoid mCamera.takePicture to be stuck because of a memory issue
                MCamera.SetPreviewCallback(null);

                // PictureCallback is implemented by the current class

                MCamera.TakePicture(null, null, this);

            }
            else
            {
                imageView.Visibility = ViewStates.Gone;
                Visibility = ViewStates.Visible;
                MCamera.StartPreview();
                MCamera.SetPreviewCallback(this);
            }

            PictureMode = !PictureMode;
        }

        //public void TakePicture(bool aprespicture)
        //{
        //    Log.Info(TAG, "Taking picture");



        //    //this.imageView = imageView;

        //    if (!aprespicture)
        //    {
        //        //this.mPictureFileName = fileName;
        //        // Postview and jpeg are sent in the same buffers if the queue is not empty when performing a capture.
        //        // Clear up buffers to avoid mCamera.takePicture to be stuck because of a memory issue
        //        MCamera.SetPreviewCallback(null);

        //        // PictureCallback is implemented by the current class

        //        MCamera.TakePicture(null, null, this);

        //    }
        //    else
        //    {
        //        imageView.Visibility = ViewStates.Gone;
        //        Visibility = ViewStates.Visible;
        //        MCamera.StartPreview();
        //        MCamera.SetPreviewCallback(this);
        //    }

        //    //PictureMode = !PictureMode;
        //}

        //public void OnPictureTaken(byte[] data, Camera camera)
        //{
        //    Log.Info(TAG, "Saving a bitmap to file");
        //    // The camera preview was automatically stopped. Start it again.
        //    //MCamera.StartPreview();
        //    //MCamera.SetPreviewCallback(this);



        //    // Write the image in a file (in jpeg format)
        //    try
        //    {

        //        SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd_HH-mm-ss");
        //        string currentDateandTime = sdf.Format(new Date());
        //        string fileName = global::Android.OS.Environment.ExternalStorageDirectory.Path +
        //                               "/sample_picture_" + currentDateandTime + ".jpg";
        //        FileOutputStream fos = new FileOutputStream(fileName);

        //        fos.Write(data);
        //        fos.Close();


        //        Graphics.Bitmap bitmap = Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length);

        //        //imageView = FindViewById<ImageView>(Resource.Id.picturedisplay);
        //        Visibility = ViewStates.Gone;
        //        imageView.Visibility = ViewStates.Visible;
        //        imageView.SetImageBitmap(bitmap);

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(TAG, "Exception in photoCallback", ex);
        //    }

        //}

        public Task<bool> OCR(byte[] data)
        {
            return Task.Run(() =>
            {
                return api.SetImage(data);
            });
        }

        public async void OnPictureTaken(byte[] data, Camera camera)
        {
            Log.Info(TAG, "Saving a bitmap to file");
            // The camera preview was automatically stopped. Start it again.
            //MCamera.StartPreview();
            //MCamera.SetPreviewCallback(this);

            // Write the image in a file (in jpeg format)
            try
            {
                FileOutputStream fos = new FileOutputStream(mPictureFileName);

                fos.Write(data);
                fos.Close();

                //Graphics.Bitmap bitmap = Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length);

                ////imageView = FindViewById<ImageView>(Resource.Id.picturedisplay);
                //Visibility = ViewStates.Gone;
                //imageView.Visibility = ViewStates.Visible;
                //imageView.SetImageBitmap(bitmap);


                try
                {
                    Mat mRgba = new Mat();

                    Graphics.Bitmap bitmap = Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length);
                    OpenCV.Android.Utils.BitmapToMat(bitmap, mRgba);

                    //bool success = api.SetImage(data).Result;
                    //bool success = await OCR(data);
                    bool success = await api.SetImage(data);
                    if (success)
                    {
                        IEnumerable<Tesseract.Result> symbols = api.Results(Tesseract.PageIteratorLevel.Symbol);
                        foreach (Tesseract.Result symbol in symbols)
                        {
                            Imgproc.PutText(mRgba, symbol.Text, new Core.Point(symbol.Box.X, symbol.Box.Y),
                            Core.Core.FontHersheySimplex, 1.0, new Scalar(255, 255, 0));


                            Imgproc.Rectangle(mRgba, new Core.Point(symbol.Box.X, symbol.Box.Y), new Core.Point(symbol.Box.X + symbol.Box.Width, symbol.Box.Y + symbol.Box.Height), RECT_COLOR);
                        }


                        Graphics.Bitmap bitmapsortie = Graphics.Bitmap.CreateBitmap(mRgba.Cols(), mRgba.Rows(), Graphics.Bitmap.Config.Argb8888);
                        OpenCV.Android.Utils.MatToBitmap(mRgba, bitmapsortie);


                        Visibility = ViewStates.Gone;
                        imageView.Visibility = ViewStates.Visible;
                        imageView.SetImageBitmap(bitmapsortie);
                    }



                    //Imgproc.PutText(mRgba, mResources.GetString(Resource.String.undistorted), new Point(mWidth * 0.6, mHeight * 0.1),
                    //        Core.Core.FontHersheySimplex, 1.0, new Scalar(255, 255, 0));

                    //Imgproc.Rectangle(mRgba, new Core.Point(100, 100), new Core.Point(300, 300), RECT_COLOR);
                }
                catch(Exception ex)
                {
                    Log.Error("PictureDemo", "Exception in photoCallback", ex);
                }

            }
            catch (Exception ex)
            {
                Log.Error("PictureDemo", "Exception in photoCallback", ex);
            }

        }
    }
}