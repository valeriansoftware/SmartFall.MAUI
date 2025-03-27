using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.Media;
using Android.OS;

namespace SmartFall.Platforms.Android
{
    [Service(ForegroundServiceType = ForegroundService.TypeMediaPlayback)]
    public class SmartFallService : Service, ISensorEventListener
    {
        private SensorManager _sensorManager;
        private Sensor _accelerometer, _gyroscope;
        private const double GRAVITY_THRESHOLD = 2.0; // Device in free fall
        private const double IMPACT_THRESHOLD = 70.0; // Impact with the surface
        private bool isFalling = false;
        private long lastFallTime = 0;

        public override IBinder OnBind(Intent intent) => null;

        public override void OnCreate()
        {
            base.OnCreate();
            _sensorManager = (SensorManager)GetSystemService(SensorService);
            _accelerometer = _sensorManager?.GetDefaultSensor(SensorType.Accelerometer);
            _gyroscope = _sensorManager?.GetDefaultSensor(SensorType.Gyroscope);

            if (_accelerometer != null)
                _sensorManager.RegisterListener(this, _accelerometer, SensorDelay.Ui);

            if (_gyroscope != null)
                _sensorManager.RegisterListener(this, _gyroscope, SensorDelay.Ui);

            // Foreground Service Notification
            var notification = new Notification.Builder(this, "smartfall_channel")
                .SetContentTitle("SmartFall is working")
                .SetContentText("Fall tracking is enabled.")
                //.SetSmallIcon(Resource.Drawable.ic_launcher)
                .Build();

            StartForeground(1, notification);
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy) { }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                DetectFall(e);
            }
        }

        private void DetectFall(SensorEvent e)
        {
            double acceleration = Math.Sqrt(e.Values[0] * e.Values[0] +
                                           e.Values[1] * e.Values[1] +
                                           e.Values[2] * e.Values[2]);

            long currentTime = Java.Lang.JavaSystem.CurrentTimeMillis();

            if (acceleration < GRAVITY_THRESHOLD) // Device falls (free fall)
            {
                isFalling = true;
            }
            else if (isFalling && acceleration > IMPACT_THRESHOLD && (currentTime - lastFallTime) > 2000)
            {
                // Device hit the surface
                PlaySound();
                lastFallTime = currentTime;
                isFalling = false;
            }
        }

        private void PlaySound()
        {
            var player = MediaPlayer.Create(this, Resource.Raw.Tom_Scream);
            player.Start();
        }

        public override void OnDestroy()
        {
            _sensorManager?.UnregisterListener(this);
            base.OnDestroy();
        }
    }
}
