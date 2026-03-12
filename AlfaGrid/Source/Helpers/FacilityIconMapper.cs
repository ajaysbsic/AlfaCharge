using AlfaGrid.Source.Models;

namespace AlfaGrid.Source.Helpers
{
    public static class FacilityIconMapper
    {
        /// <summary>
        /// Maps facility names to Material Design Icons unicode characters
        /// </summary>
        public static string GetMaterialIcon(string facilityName)
        {
            if (string.IsNullOrWhiteSpace(facilityName))
                return MaterialIcons.Place;

            // Normalize the facility name (lowercase, trim)
            var name = facilityName.Trim().ToLowerInvariant();

            return name switch
            {
                // Accommodation
                "hotel" => MaterialIcons.Hotel,
                "motel" => MaterialIcons.Hotel,
                
                // Food & Dining
                "restaurant" => MaterialIcons.Restaurant,
                "cafe" or "café" or "coffee shop" => MaterialIcons.Cafe,
                "fast food" or "fastfood" => MaterialIcons.FastFood,
                "bar" or "pub" => MaterialIcons.Bar,
                "pizza" => MaterialIcons.Pizza,
                "bakery" => MaterialIcons.Bakery,
                "ice cream" => MaterialIcons.IceCream,
                
                // Shopping
                "mall" or "shopping mall" => MaterialIcons.Mall,
                "supermarket" or "grocery store" or "groceries" => MaterialIcons.Supermarket,
                "convenience store" or "convenience" => MaterialIcons.Convenience,
                "shopping" => MaterialIcons.ShoppingCart,
                "gift shop" => MaterialIcons.Gift,
                "florist" => MaterialIcons.Florist,
                "jewelry" => MaterialIcons.Jewelry,
                "clothing store" => MaterialIcons.Clothing,
                
                // Transportation & Parking
                "parking lot" or "parking" => MaterialIcons.Parking,
                "car wash" => MaterialIcons.CarWash,
                "gas station" or "petrol station" => MaterialIcons.Gas,
                "ev station" or "charging station" or "ev charging" => MaterialIcons.EvStation,
                "bus station" => MaterialIcons.BusStation,
                "subway" or "metro" => MaterialIcons.Subway,
                "airport" => MaterialIcons.Airport,
                "train station" => MaterialIcons.Train,
                "taxi stand" => MaterialIcons.Taxi,
                
                // Services
                "wifi" or "wi-fi" => MaterialIcons.Wifi,
                "atm" => MaterialIcons.ATM,
                "restroom" or "toilet" or "wc" => MaterialIcons.Restroom,
                "pharmacy" => MaterialIcons.Pharmacy,
                "hospital" => MaterialIcons.Hospital,
                "doctor" or "clinic" => MaterialIcons.Hospital,
                "dentist" => MaterialIcons.Dentist,
                "veterinary" or "vet" => MaterialIcons.Veterinary,
                "bank" => MaterialIcons.Bank,
                "post office" => MaterialIcons.PostOffice,
                "police" or "police station" => MaterialIcons.Police,
                "fire station" => MaterialIcons.Fire,
                "laundry" => MaterialIcons.Laundry,
                "dry cleaning" => MaterialIcons.DryCleaning,
                "salon" or "hair salon" or "barber" => MaterialIcons.Salon,
                "spa" => MaterialIcons.Spa,
                "car repair" or "garage" or "mechanic" => MaterialIcons.CarRepair,
                "repair" or "repair shop" => MaterialIcons.Repair,
                "printer" or "print shop" => MaterialIcons.Printer,
                
                // Sports & Recreation
                "sport" or "sports" or "sports facility" => MaterialIcons.Sport,
                "gym" or "fitness center" or "fitness" => MaterialIcons.Gym,
                "pool" or "swimming pool" => MaterialIcons.Pool,
                "playground" => MaterialIcons.Playground,
                "park" => MaterialIcons.Park,
                "beach" => MaterialIcons.Beach,
                "stadium" => MaterialIcons.Stadium,
                "casino" => MaterialIcons.Casino,
                "games" or "gaming" => MaterialIcons.Games,
                "recreation area" or "recreation" => MaterialIcons.Playground,
                
                // Culture & Education
                "museum" => MaterialIcons.Museum,
                "theater" or "theatre" => MaterialIcons.Theater,
                "cinema" or "movie theater" => MaterialIcons.Cinema,
                "library" => MaterialIcons.Library,
                "school" => MaterialIcons.School,
                "university" or "college" => MaterialIcons.University,
                
                // Religious
                "church" => MaterialIcons.Church,
                "mosque" => MaterialIcons.Mosque,
                "synagogue" => MaterialIcons.Synagogue,
                
                // Nature
                "garden" => MaterialIcons.Garden,
                "mountain" => MaterialIcons.Mountain,
                "lake" => MaterialIcons.Lake,
                "nature" => MaterialIcons.Park,
                
                // Default fallback
                _ => MaterialIcons.Place
            };
        }

        /// <summary>
        /// Creates a Facility object with Material Design icon
        /// </summary>
        public static Facility CreateFacility(string name)
        {
            var icon = GetMaterialIcon(name);
            return new Facility(name, icon, isIconFont: true);
        }

        /// <summary>
        /// Converts a facility from JSON (with image icon) to Material Design icon
        /// </summary>
        public static Facility ConvertToMaterialIcon(Facility facility)
        {
            if (facility == null)
                return new Facility("Unknown", MaterialIcons.Place, isIconFont: true);

            var materialIcon = GetMaterialIcon(facility.Name);
            return new Facility(facility.Name, materialIcon, isIconFont: true);
        }
    }
}
