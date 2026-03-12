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