using AlfaGrid.Source.AppConfiguration.EnviornmentConfig;

namespace AlfaGrid.Source.AppConfiguration
{
    public class AppConfigurationManager
    {
        public IConfig Config { get; set; }

        private static readonly Lazy<AppConfigurationManager> lazy =
            new(() => new AppConfigurationManager());

        public static AppConfigurationManager Instance => lazy.Value;

        public void Init(AppEnvironmentEnum appEnvironmentEnum)
        {
            switch (appEnvironmentEnum)
            {
                case AppEnvironmentEnum.DEVELOPMENT:
                    {
                        Config = new Development();
                    }
                    break;
                case AppEnvironmentEnum.TEST:
                    {
                        Config = new Test();
                    }
                    break;
                case AppEnvironmentEnum.QA:
                    {
                        Config = new Qa();
                    }
                    break;
                case AppEnvironmentEnum.STAGE:
                    {
                        Config = new Stage();
                    }
                    break;
                case AppEnvironmentEnum.PRODUCTION:
                    {
                        Config = new Production();
                    }
                    break;
            }
        }
    }
}