using Android.App;
using Android.OS;
using Android.Widget;
using OpenCV.SDKDemo.SecureCameraV3;

namespace OpenCV.SDKDemo
{
    [Activity(Label = "OpenCV.SDKDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            FindViewById<Button>(Resource.Id.secureViewV3)
                .Click += (s, e) => StartActivity(typeof(SecureCameraV3Activity));

        }
    }
}

