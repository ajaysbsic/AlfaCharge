using System;
namespace AlfaGrid.Framework.FrameworkConfig
{
	public class FrameworkConfigManager
	{
        public IFrameworkConfig FrameworkConfig = null;

        private FrameworkConfigManager()
		{
		}

        private static readonly Lazy<FrameworkConfigManager> lazy =
            new(() => new FrameworkConfigManager());

        public static FrameworkConfigManager Instance => lazy.Value;

		public void Init(IFrameworkConfig config)
		{
            FrameworkConfig = config;
        }
    }
}

