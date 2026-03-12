using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Platform;
using AlfaGrid.Source.View;
using IMauiMap = Microsoft.Maui.Maps.IMap;

namespace AlfaGrid.Platforms.Android.Handlers
{
    public class CustomMapHandler : MapHandler
    {
        private GoogleMap? _googleMap;
        private MapView? _mapView;

        public CustomMapHandler() : base(MapperCustom)
        {
        }

        private static IPropertyMapper<IMauiMap, IMapHandler> MapperCustom = new PropertyMapper<IMauiMap, IMapHandler>(Mapper)
        {
            [nameof(IMauiMap.Pins)] = MapPins
        };

        protected override MapView CreatePlatformView()
        {
            _mapView = base.CreatePlatformView();
            _mapView.GetMapAsync(new MapReadyCallback(this));
            return _mapView;
        }

        private void OnMapReady(GoogleMap googleMap)
        {
            _googleMap = googleMap;
            
            if (_googleMap != null && VirtualView != null)
            {
                UpdatePins();
            }
        }

        private static new void MapPins(IMapHandler handler, IMauiMap map)
        {
            if (handler is CustomMapHandler customHandler)
            {
                customHandler.UpdatePins();
            }
        }

        private void UpdatePins()
        {
            if (_googleMap == null || VirtualView?.Pins == null)
                return;

            _googleMap.Clear();

            foreach (var pin in VirtualView.Pins)
            {
                AddPin(pin);
            }
        }

        private void AddPin(IMapPin pin)
        {
            if (_googleMap == null)
                return;

            var markerOptions = new MarkerOptions();
            markerOptions.SetPosition(new LatLng(pin.Location.Latitude, pin.Location.Longitude));
            markerOptions.SetTitle(pin.Label);
            markerOptions.SetSnippet(pin.Address);

            // Determine icon based on pin label
            var iconName = pin.Label == "My Location" ? "current_location_icon" : "charging_station";

            try
            {
                var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                if (context != null)
                {
                    var resourceId = context.Resources?.GetIdentifier(iconName, "drawable", context.PackageName) ?? 0;

                    if (resourceId != 0)
                    {
                        var bitmap = global::Android.Graphics.BitmapFactory.DecodeResource(context.Resources, resourceId);
                        if (bitmap != null)
                        {
                            // Scale bitmap to appropriate size (80x80 pixels)
                            var scaledBitmap = global::Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 80, 80, false);
                            markerOptions.SetIcon(BitmapDescriptorFactory.FromBitmap(scaledBitmap));
                            
                            System.Diagnostics.Debug.WriteLine($"Successfully loaded custom icon: {iconName} for pin: {pin.Label}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Bitmap decode failed for: {iconName}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Resource not found: {iconName}. Make sure the image is in Platforms/Android/Resources/drawable/");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading custom pin icon '{iconName}': {ex.Message}");
            }

            _googleMap.AddMarker(markerOptions);
        }

        private class MapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
        {
            private readonly CustomMapHandler _handler;

            public MapReadyCallback(CustomMapHandler handler)
            {
                _handler = handler;
            }

            public void OnMapReady(GoogleMap googleMap)
            {
                _handler.OnMapReady(googleMap);
            }
        }

        protected override void DisconnectHandler(MapView platformView)
        {
            _googleMap?.Dispose();
            _googleMap = null;
            
            base.DisconnectHandler(platformView);
        }
    }
}