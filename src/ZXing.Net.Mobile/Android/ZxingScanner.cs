using System;
using System.Threading.Tasks;
using Android.Content;
using ZXing;
using Android.Content.PM;

namespace ZXing.Mobile
{

	public class MobileBarcodeScanner : MobileBarcodeScannerBase
	{
		public MobileBarcodeScanner(Context context)
		{
			this.Context = context;
		}

		public ScreenOrientation Orientation;

		public Context Context { get; private set; }

		public Android.Views.View CustomOverlay { get; set; }
		//public int CaptureSound { get;set; }
			
		bool torch = false;

		public override Task<Result> Scan(MobileBarcodeScanningOptions options)
		{
			var task = Task.Factory.StartNew(() =>{
			      
				var waitScanResetEvent = new System.Threading.ManualResetEvent(false);

				var scanIntent = new Intent(this.Context, typeof(ZxingActivity));

				scanIntent.AddFlags(ActivityFlags.NewTask);

				ZxingActivity.UseCustomView = this.UseCustomOverlay;
				ZxingActivity.CustomOverlayView = this.CustomOverlay;
				ZxingActivity.ScanningOptions = options;
				ZxingActivity.TopText = TopText;
				ZxingActivity.BottomText = BottomText;
				ZxingActivity.Orientation = Orientation;

				Result scanResult = null;

				ZxingActivity.OnCanceled += () =>{
					waitScanResetEvent.Set();
				};

				ZxingActivity.OnScanCompleted += (Result result) =>{
					scanResult = result;
					waitScanResetEvent.Set();
				};

				this.Context.StartActivity(scanIntent);

				waitScanResetEvent.WaitOne();

				return scanResult;
			});

			return task;
		}

		public override void Cancel()
		{
			ZxingActivity.RequestCancel();
		}

		public override void AutoFocus()
		{
			ZxingActivity.RequestAutoFocus();
		}

		public override void Torch(bool on)
		{
			torch = on;
			ZxingActivity.RequestTorch(on);
		}

		public override void ToggleTorch()
		{
			Torch(!torch);
		}

		public override bool IsTorchOn
		{
			get
			{
				return torch;
			}
		}

	}
	
}
