using System.Globalization;
using Microsoft.Maui.Controls;

namespace AlfaGrid.Resources.Localization
{
    public class AppResources
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
        {
            ["en"] = new Dictionary<string, string>
            {
                // Common
                ["AppName"] = "AlfaGrid",
                ["OK"] = "OK",
                ["Cancel"] = "Cancel",
                ["Apply"] = "Apply",
                ["Reset"] = "Reset",
                ["Error"] = "Error",
                ["Success"] = "Success",
                ["Loading"] = "Loading...",
                ["SelectLanguage"] = "Select Language",
                
                // Login Page
                ["Login_Title"] = "Welcome Back",
                ["Login_Subtitle"] = "Sign in to continue",
                ["Login_Email"] = "Email",
                ["Login_Password"] = "Password",
                ["Login_ForgotPassword"] = "Forgot Password?",
                ["Login_Button"] = "Login",
                ["Login_NoAccount"] = "Don't have an account?",
                ["Login_SignUp"] = "Sign Up",
                ["Login_LanguageLabel"] = "Language",
                
                // Home Page
                ["Home_SearchPlaceholder"] = "Find Charging Location",
                ["Home_Directions"] = "Directions",
                ["Home_Reserve"] = "Reserve",
                ["Home_ScanQR"] = "Scan QR",
                ["Home_Favorite"] = "Favorite",
                ["Home_GoBack"] = "Go back",
                ["Home_ViewFullDetails"] = "Tap anywhere to view full details",
                ["Home_NoReviews"] = "No Reviews Available",
                ["Home_Available"] = "Available",
                
                // Filter Page
                ["Filter_Title"] = "Filters",
                ["Filter_Sorting"] = "Sorting",
                ["Filter_Time"] = "Time",
                ["Filter_Rating"] = "Rating",
                ["Filter_AllRatings"] = "All Ratings",
                ["Filter_OnlyAbove"] = "Only Above",
                ["Filter_SelectMinRating"] = "Select minimum rating",
                ["Filter_StarsAndAbove"] = "{0} stars and above",
                ["Filter_Locations"] = "Locations",
                ["Filter_24HoursOpen"] = "24 Hours Open",
                ["Filter_AvailableNow"] = "Available Now",
                ["Filter_FreeParking"] = "Free Parking",
                ["Filter_Wifi"] = "Wifi",
                ["Filter_ConnectorTypes"] = "Connector Types",
                ["Filter_ApplyingFilters"] = "Applying filters...",
                
                // Location Details
                ["LocationDetails_Title"] = "Location Details",
                ["LocationDetails_Overview"] = "Overview",
                ["LocationDetails_Photos"] = "Photos",
                ["LocationDetails_Reviews"] = "Reviews",
                ["LocationDetails_Facilities"] = "FACILITIES",
                ["LocationDetails_Information"] = "INFORMATION",
                ["LocationDetails_StartCharging"] = "START CHARGING",
                ["LocationDetails_NoImages"] = "No location images available",
                ["LocationDetails_Socket"] = "SOCKET",
                ["LocationDetails_MaxPower"] = "Max Power",
                ["LocationDetails_TariffDescription"] = "Tariff Description",
                ["LocationDetails_FreeTariff"] = "Free Tariff",
                
                // Settings
                ["Settings_Title"] = "Settings",
                ["Settings_General"] = "General",
                ["Settings_Language"] = "Language",
                ["Settings_Notifications"] = "Notifications",
                ["Settings_PushNotifications"] = "Push Notifications",
                ["Settings_EmailNotifications"] = "Email Notifications",
                ["Settings_Account"] = "Account",
                ["Settings_ChangePassword"] = "Change Password",
                ["Settings_DeleteAccount"] = "Delete Account",
                ["Settings_About"] = "About",
                ["Settings_TermsAndConditions"] = "Terms and Conditions",
                ["Settings_PrivacyPolicy"] = "Privacy Policy",
                ["Settings_Version"] = "Version",
                ["Settings_Logout"] = "Logout",
                ["Settings_LanguageChanged"] = "Language changed successfully. The app will restart.",
                
                // Profile
                ["Profile_Title"] = "Profile",
                ["Profile_MyProfile"] = "My Profile",
                ["Profile_FullName"] = "Full Name",
                ["Profile_Email"] = "Email",
                ["Profile_Phone"] = "Phone Number",
                ["Profile_SaveChanges"] = "Save Changes",
                
                // Menu
                ["Menu_Home"] = "Home",
                ["Menu_Profile"] = "Profile",
                ["Menu_MyChargingProfile"] = "My Charging Profile",
                ["Menu_Reservations"] = "Reservations",
                ["Menu_Settings"] = "Settings",
                
                // QR Scanner
                ["QRScanner_Title"] = "Scan QR Code",
                ["QRScanner_Scanning"] = "Scanning...",
                
                // Register
                ["Register_Title"] = "Create Account",
                ["Register_Subtitle"] = "Sign up to get started",
                ["Register_FullName"] = "Full Name",
                ["Register_Email"] = "Email",
                ["Register_Phone"] = "Phone Number",
                ["Register_Password"] = "Password",
                ["Register_ConfirmPassword"] = "Confirm Password",
                ["Register_Button"] = "Sign Up",
                ["Register_HaveAccount"] = "Already have an account?",
                ["Register_SignIn"] = "Sign In",
                
                // Payment
                ["Payment_Title"] = "Payment Method",
                ["Payment_Subtitle"] = "Add your card details to start charging",
                ["Payment_CardNumber"] = "Card Number",
                ["Payment_ExpiryDate"] = "Expiry Date",
                ["Payment_CVV"] = "CVV",
                ["Payment_CardholderName"] = "Cardholder Name",
                ["Payment_SaveCard"] = "Save this card for future use",
                ["Payment_Continue"] = "Continue",
                
                // Charging Session
                ["ChargingSession_Title"] = "Charge Session",
                ["ChargingSession_Online"] = "Online",
                ["ChargingSession_Charging"] = "CHARGING",
                ["ChargingSession_Stopping"] = "STOPPING",
                ["ChargingSession_Station"] = "Station",
                ["ChargingSession_StartTime"] = "Start Time",
                ["ChargingSession_Duration"] = "Duration",
                ["ChargingSession_EnergyConsumed"] = "Energy Consumed",
                ["ChargingSession_ConnectorType"] = "CONNECTOR TYPE",
                ["ChargingSession_SessionCost"] = "Session Cost",
                ["ChargingSession_LastUpdated"] = "Last updated at",
                ["ChargingSession_Warning"] = "Do not unplug connector before stopping",
                ["ChargingSession_StopCharging"] = "Stop Charging",
                ["ChargingSession_ConfirmStop"] = "Are you sure you want to stop the charging session?",
                ["ChargingSession_Yes"] = "Yes, Stop",
                ["ChargingSession_Complete"] = "Charging Complete",
                ["ChargingSession_Summary"] = "Your charging session has ended successfully.",
                ["ChargingSession_StopError"] = "Failed to stop charging. Please try again.",
                ["ChargingSession_MustStopFirst"] = "Please stop the charging session before going back.",
                ["ChargingSession_Support"] = "Support",
                ["ChargingSession_SupportMessage"] = "For assistance, please call our support team at +966 XXX XXXX",
                
                // Ending Session
                ["EndingSession_Title"] = "Ending Session",
                ["EndingSession_Message"] = "Please wait while we stop your charging session",
                
                // Session Details
                ["SessionDetails_Title"] = "Session",
                ["SessionDetails_TotalCost"] = "Total Cost",
                ["SessionDetails_CurrentChargingCost"] = "Current Charging Cost",
                ["SessionDetails_EnergyCharges"] = "Energy Charges",
                ["SessionDetails_TimeCharges"] = "Time Charges",
                ["SessionDetails_ParkingCharges"] = "Parking Charges",
                ["SessionDetails_FixedCharges"] = "Fixed Charges",
                ["SessionDetails_ChargingDuration"] = "Charging Duration",
                ["SessionDetails_IdleDuration"] = "Idle Duration",
                ["SessionDetails_EstEndBatterySoC"] = "Est. End Battery SoC",
                ["SessionDetails_EnergyAdded"] = "Energy Added",
                ["SessionDetails_RemoteStopped"] = "Session has been stopped remotely",
                ["SessionDetails_Continue"] = "Continue",
            },
            
            ["ar"] = new Dictionary<string, string>
            {
                // Common
                ["AppName"] = "ألفا جريد",
                ["OK"] = "حسناً",
                ["Cancel"] = "إلغاء",
                ["Apply"] = "تطبيق",
                ["Reset"] = "إعادة تعيين",
                ["Error"] = "خطأ",
                ["Success"] = "نجاح",
                ["Loading"] = "جاري التحميل...",
                ["SelectLanguage"] = "اختر اللغة",
                
                // Login Page
                ["Login_Title"] = "مرحبًا بعودتك",
                ["Login_Subtitle"] = "قم بتسجيل الدخول للاستمرار",
                ["Login_Email"] = "البريد الإلكتروني",
                ["Login_Password"] = "كلمة المرور",
                ["Login_ForgotPassword"] = "نسيت كلمة المرور؟",
                ["Login_Button"] = "تسجيل الدخول",
                ["Login_NoAccount"] = "لا تملك حساب؟",
                ["Login_SignUp"] = "اشترك الآن",
                ["Login_LanguageLabel"] = "اللغة",
                
                // Home Page
                ["Home_SearchPlaceholder"] = "ابحث عن مكان الشحن",
                ["Home_Directions"] = "الاتجاهات",
                ["Home_Reserve"] = "احجز",
                ["Home_ScanQR"] = "امسح QR",
                ["Home_Favorite"] = "المفضل",
                ["Home_GoBack"] = "عودة",
                ["Home_ViewFullDetails"] = "اضغط في أي مكان لعرض التفاصيل الكاملة",
                ["Home_NoReviews"] = "لا توجد تقييمات متاحة",
                ["Home_Available"] = "متاح",
                
                // Filter Page
                ["Filter_Title"] = "التصفية",
                ["Filter_Sorting"] = "الفرز",
                ["Filter_Time"] = "الوقت",
                ["Filter_Rating"] = "التقييم",
                ["Filter_AllRatings"] = "جميع التقييمات",
                ["Filter_OnlyAbove"] = "فقط الأعلى من",
                ["Filter_SelectMinRating"] = "حدد الحد الأدنى من التقييم",
                ["Filter_StarsAndAbove"] = "{0} نجوم وأكثر",
                ["Filter_Locations"] = "المواقع",
                ["Filter_24HoursOpen"] = "مفتوح 24 ساعة",
                ["Filter_AvailableNow"] = "متاح الآن",
                ["Filter_FreeParking"] = "موقف سيارات مجاني",
                ["Filter_Wifi"] = "واي فاي",
                ["Filter_ConnectorTypes"] = "أنواع الموصلات",
                ["Filter_ApplyingFilters"] = "تطبيق الفلاتر...",
                
                // Location Details
                ["LocationDetails_Title"] = "تفاصيل الموقع",
                ["LocationDetails_Overview"] = "نظرة عامة",
                ["LocationDetails_Photos"] = "صور",
                ["LocationDetails_Reviews"] = "تقييمات",
                ["LocationDetails_Facilities"] = "المرافق",
                ["LocationDetails_Information"] = "المعلومات",
                ["LocationDetails_StartCharging"] = "ابدأ الشحن",
                ["LocationDetails_NoImages"] = "لا توجد صور متاحة للموقع",
                ["LocationDetails_Socket"] = "المقبس",
                ["LocationDetails_MaxPower"] = "أقصى طاقة",
                ["LocationDetails_TariffDescription"] = "وصف التعريفة",
                ["LocationDetails_FreeTariff"] = "تعريفة مجانية",
                
                // Settings
                ["Settings_Title"] = "الإعدادات",
                ["Settings_General"] = "عام",
                ["Settings_Language"] = "اللغة",
                ["Settings_Notifications"] = "الإشعارات",
                ["Settings_PushNotifications"] = "إشعارات الدفع",
                ["Settings_EmailNotifications"] = "إشعارات البريد الإلكتروني",
                ["Settings_Account"] = "الحساب",
                ["Settings_ChangePassword"] = "تغيير كلمة المرور",
                ["Settings_DeleteAccount"] = "حذف الحساب",
                ["Settings_About"] = "حول",
                ["Settings_TermsAndConditions"] = "الشروط والأحكام",
                ["Settings_PrivacyPolicy"] = "سياسة الخصوصية",
                ["Settings_Version"] = "الإصدار",
                ["Settings_Logout"] = "تسجيل الخروج",
                ["Settings_LanguageChanged"] = "تم تغيير اللغة بنجاح. ستتم إعادة تشغيل التطبيق.",
                
                // Profile
                ["Profile_Title"] = "الملف الشخصي",
                ["Profile_MyProfile"] = "ملفي الشخصي",
                ["Profile_FullName"] = "الاسم الكامل",
                ["Profile_Email"] = "البريد الإلكتروني",
                ["Profile_Phone"] = "رقم الهاتف",
                ["Profile_SaveChanges"] = "حفظ التغييرات",
                
                // Menu
                ["Menu_Home"] = "الرئيسية",
                ["Menu_Profile"] = "الملف الشخصي",
                ["Menu_MyChargingProfile"] = "ملف الشحن الخاص بي",
                ["Menu_Reservations"] = "الحجوزات",
                ["Menu_Settings"] = "الإعدادات",
                
                // QR Scanner
                ["QRScanner_Title"] = "امسح رمز الاستجابة السريعة",
                ["QRScanner_Scanning"] = "جاري المسح...",
                
                // Register
                ["Register_Title"] = "إنشاء حساب",
                ["Register_Subtitle"] = "سجل للحصول على البدء",
                ["Register_FullName"] = "الاسم الكامل",
                ["Register_Email"] = "البريد الإلكتروني",
                ["Register_Phone"] = "رقم الهاتف",
                ["Register_Password"] = "كلمة المرور",
                ["Register_ConfirmPassword"] = "تأكيد كلمة المرور",
                ["Register_Button"] = "اشترك الآن",
                ["Register_HaveAccount"] = "هل لديك حساب بالفعل؟",
                ["Register_SignIn"] = "تسجيل الدخول",
                
                // Payment
                ["Payment_Title"] = "طريقة الدفع",
                ["Payment_Subtitle"] = "أضف تفاصيل بطاقتك لبدء الشحن",
                ["Payment_CardNumber"] = "رقم بطاقة الائتمان",
                ["Payment_ExpiryDate"] = "تاريخ الانتهاء",
                ["Payment_CVV"] = "رمز CVV",
                ["Payment_CardholderName"] = "اسم حامل البطاقة",
                ["Payment_SaveCard"] = "احفظ هذه البطاقة للاستخدام المستقبلي",
                ["Payment_Continue"] = "متابعة",
                
                // Charging Session
                ["ChargingSession_Title"] = "جلسة الشحن",
                ["ChargingSession_Online"] = "متصل",
                ["ChargingSession_Charging"] = "جاري الشحن",
                ["ChargingSession_Stopping"] = "جاري الإيقاف",
                ["ChargingSession_Station"] = "المحطة",
                ["ChargingSession_StartTime"] = "وقت البدء",
                ["ChargingSession_Duration"] = "المدة",
                ["ChargingSession_EnergyConsumed"] = "الطاقة المستهلكة",
                ["ChargingSession_ConnectorType"] = "نوع الموصل",
                ["ChargingSession_SessionCost"] = "تكلفة الجلسة",
                ["ChargingSession_LastUpdated"] = "آخر تحديث في",
                ["ChargingSession_Warning"] = "لا تفصل الموصل قبل الإيقاف",
                ["ChargingSession_StopCharging"] = "إيقاف الشحن",
                ["ChargingSession_ConfirmStop"] = "هل أنت مت确定 من رغبتك في إيقاف جلسة الشحن؟",
                ["ChargingSession_Yes"] = "نعم، أوقف",
                ["ChargingSession_Complete"] = "اكتمل الشحن",
                ["ChargingSession_Summary"] = "انتهت جلسة الشحن الخاصة بك بنجاح.",
                ["ChargingSession_StopError"] = "فشل إيقاف الشحن. يرجى المحاولة مرة أخرى.",
                ["ChargingSession_MustStopFirst"] = "يرجى إيقاف جلسة الشحن قبل العودة.",
                ["ChargingSession_Support"] = "الدعم",
                ["ChargingSession_SupportMessage"] = "للحصول على المساعدة، يرجى الاتصال بفريق الدعم على +966 XXX XXXX",
                
                // Ending Session
                ["EndingSession_Title"] = "إنهاء الجلسة",
                ["EndingSession_Message"] = "يرجى الانتظار بينما نوقف جلسة الشحن الخاصة بك",
                
                // Session Details
                ["SessionDetails_Title"] = "الجلسة",
                ["SessionDetails_TotalCost"] = "التكلفة الإجمالية",
                ["SessionDetails_CurrentChargingCost"] = "تكلفة الشحن الحالية",
                ["SessionDetails_EnergyCharges"] = "رسوم الطاقة",
                ["SessionDetails_TimeCharges"] = "رسوم الوقت",
                ["SessionDetails_ParkingCharges"] = "رسوم الوقوف",
                ["SessionDetails_FixedCharges"] = "الرسوم الثابتة",
                ["SessionDetails_ChargingDuration"] = "مدة الشحن",
                ["SessionDetails_IdleDuration"] = "مدة عدم النشاط",
                ["SessionDetails_EstEndBatterySoC"] = "نسبة شحن البطارية المتوقعة",
                ["SessionDetails_EnergyAdded"] = "الطاقة المضافة",
                ["SessionDetails_RemoteStopped"] = "تم إيقاف الجلسة عن بُعد",
                ["SessionDetails_Continue"] = "متابعة",
            }
        };

        private static string _currentLanguage = "en";

        public static string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value && Translations.ContainsKey(value))
                {
                    _currentLanguage = value;
                    CultureInfo.CurrentUICulture = new CultureInfo(value);
                    CultureInfo.CurrentCulture = new CultureInfo(value);
                }
            }
        }

        public static string GetString(string key)
        {
            if (Translations.TryGetValue(_currentLanguage, out var languageStrings) &&
                languageStrings.TryGetValue(key, out var value))
            {
                return value;
            }

            if (_currentLanguage != "en" &&
                Translations.TryGetValue("en", out var englishStrings) &&
                englishStrings.TryGetValue(key, out var fallbackValue))
            {
                return fallbackValue;
            }

            return key;
        }

        public static string GetFormattedString(string key, params object[] args)
        {
            var format = GetString(key);
            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }

        public static bool IsRTL => _currentLanguage == "ar";
        public static FlowDirection FlowDirection => IsRTL ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
    }
}
