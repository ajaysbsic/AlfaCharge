# Custom Map Pin Icons Implementation Guide

## Current Implementation Status

? **Completed:**
1. Current location set to Alfanar Industrial City (24.53129, 46.93705)
2. Pin tracking dictionary created to identify which pins should use custom icons
3. Map pins added for all charging locations
4. Current location pin added separately

## Custom Pin Icons Setup

To display custom pin icons ("charging_station.png" for charging locations and "current_location_icon.png" for current location), you need to implement platform-specific handlers.

### For Android:

1. **Add images to Android Resources:**
   - Place `charging_station.png` in `Platforms/Android/Resources/drawable/`
   - Place `current_location_icon.png` in `Platforms/Android/Resources/drawable/`
   - Ensure images are 80x80 pixels for best results

2. **Create Custom Map Handler:**

Create file: `Platforms/Android/Handlers/CustomMapHandler.cs`

```csharp
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Platform;

namespace AlfaGrid.Platforms.Android.Handlers
{
    public class CustomMapHandler : MapHandler
    {
        private GoogleMap? _googleMap;
        private Dictionary<Pin, Marker>? _markers;

        protected override void ConnectHandler(MapView platformView)
        {
            base.ConnectHandler(platformView);
            _markers = new Dictionary<Pin, Marker>();
            platformView.GetMapAsync(new CustomMapCallback(this));
        }

        private void OnMapReady(GoogleMap googleMap)
        {
            _googleMap = googleMap;
            UpdatePins();
        }

        private void UpdatePins()
        {
            if (_googleMap == null || VirtualView?.Pins == null)
                return;

            _googleMap.Clear();
            _markers?.Clear();

            foreach (var pin in VirtualView.Pins)
            {
                AddPin(pin);
            }
        }

        private void AddPin(IMapPin pin)
        {
            var markerOptions = new MarkerOptions();
            markerOptions.SetPosition(new LatLng(pin.Location.Latitude, pin.Location.Longitude));
            markerOptions.SetTitle(pin.Label);
            markerOptions.SetSnippet(pin.Address);

            // Determine icon based on pin label or other property
            var iconName = pin.Label == "My Location" ? "current_location_icon" : "charging_station";
            
            try
            {
                var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                var resourceId = context.Resources.GetIdentifier(iconName, "drawable", context.PackageName);
                
                if (resourceId != 0)
                {
                    var bitmap = global::Android.Graphics.BitmapFactory.DecodeResource(context.Resources, resourceId);
                    if (bitmap != null)
                    {
                        var scaledBitmap = global::Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 80, 80, false);
                        markerOptions.SetIcon(BitmapDescriptorFactory.FromBitmap(scaledBitmap));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading custom pin icon: {ex.Message}");
            }

            var marker = _googleMap.AddMarker(markerOptions);
            if (marker != null && pin is Pin mapPin)
            {
                _markers?.Add(mapPin, marker);
            }
        }

        private class CustomMapCallback : Java.Lang.Object, IOnMapReadyCallback
        {
            private readonly CustomMapHandler _handler;

            public CustomMapCallback(CustomMapHandler handler)
            {
                _handler = handler;
            }

            public void OnMapReady(GoogleMap googleMap)
            {
                _handler.OnMapReady(googleMap);
            }
        }
    }
}
```

3. **Register Handler in MauiProgram.cs:**

```csharp
using Microsoft.Maui.Maps.Handlers;

#if ANDROID
using AlfaGrid.Platforms.Android.Handlers;
#endif

// In CreateMauiApp method, add:
.ConfigureMauiHandlers(handlers =>
{
#if ANDROID
    handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, CustomMapHandler>();
#endif
});
```

### For iOS:

1. **Add images to iOS Resources:**
   - Place `charging_station.png` and `charging_station@2x.png` (retina) in `Platforms/iOS/Resources/`
   - Place `current_location_icon.png` and `current_location_icon@2x.png` in `Platforms/iOS/Resources/`
   - Ensure build action is set to `BundleResource`

2. **Create Custom Map Handler:**

Create file: `Platforms/iOS/Handlers/CustomMapHandler.cs`

```csharp
using MapKit;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using UIKit;

namespace AlfaGrid.Platforms.iOS.Handlers
{
    public class CustomMapHandler : MapHandler
    {
        protected override void ConnectHandler(MKMapView platformView)
        {
            base.ConnectHandler(platformView);
            platformView.GetViewForAnnotation = GetViewForAnnotation;
        }

        private MKAnnotationView? GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (annotation is MKUserLocation)
                return null;

            var annotationView = mapView.DequeueReusableAnnotation("CustomPin") as MKMarkerAnnotationView;
            
            if (annotationView == null)
            {
                annotationView = new MKMarkerAnnotationView(annotation, "CustomPin");
            }
            else
            {
                annotationView.Annotation = annotation;
            }

            // Determine icon based on annotation title
            var iconName = annotation.Title == "My Location" ? "current_location_icon" : "charging_station";
            
            try
            {
                var image = UIImage.FromBundle(iconName);
                if (image != null)
                {
                    annotationView.Image = image;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading custom pin icon: {ex.Message}");
            }

            annotationView.CanShowCallout = true;
            return annotationView;
        }
    }
}
```

3. **Register Handler in MauiProgram.cs:**

```csharp
#if IOS
using AlfaGrid.Platforms.iOS.Handlers;
#endif

// Update ConfigureMauiHandlers:
.ConfigureMauiHandlers(handlers =>
{
#if ANDROID
    handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, CustomMapHandler>();
#elif IOS
    handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, CustomMapHandler>();
#endif
});
```

## Current Status

The code is structured to support custom pin icons through the `_pinIcons` dictionary in `HomePage.xaml.cs`. This dictionary tracks:
- Current location pin ? should use "current_location_icon"
- Charging station pins ? should use "charging_station"

Once you implement the platform-specific handlers above, the custom icons will be displayed on the map.

## Alternative Approach

If you don't need true custom icons and can work with standard map markers, you can use different `PinType` values:
- `PinType.Place` - Standard red pin
- `PinType.SavedPin` - Saved location marker
- `PinType.SearchResult` - Search result marker

However, for true custom icons matching your design, you must implement the platform-specific handlers as described above.
