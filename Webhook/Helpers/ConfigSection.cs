using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Webhook.Helpers
{
    public class ConfigSection : ConfigurationSection
    {
        public static ConfigSection Webhook
        {
            get { return (ConfigSection)ConfigurationManager.GetSection("webhook"); }
        }

        public Dictionary<string, UrlConfigurationElement> Data
        {
            get
            {
                return Hooks.OfType<UrlConfigurationElement>().ToDictionary(x => x.Name, x => x);
            }
        }

        [ConfigurationProperty("hooks", IsDefaultCollection = true)]
        public UrlConfigurationElementCollection Hooks
        {
            get { return (UrlConfigurationElementCollection)this["hooks"]; }
            set { this["hooks"] = value; }
        }

        [ConfigurationCollection(typeof(UrlConfigurationElement))]
        public class UrlConfigurationElementCollection : ConfigurationElementCollection
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new UrlConfigurationElement();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((UrlConfigurationElement)element).Name;
            }

            [ConfigurationProperty("enable", IsKey = true, IsRequired = false)]
            public bool Enable
            {
                get { return (bool)this["enable"]; }
                set { this["enable"] = value; }
            }

            [ConfigurationProperty("defaultUrl", IsRequired = false)]
            public string DefaultUrl
            {
                get { return (string)this["defaultUrl"]; }
                set { this["defaultUrl"] = value; }
            }

            [ConfigurationProperty("timeout", IsRequired = false, DefaultValue = 60000)]
            public int Timeout
            {
                get { return (int)this["timeout"]; }
                set { this["timeout"] = value; }
            }
        }

        public class UrlConfigurationElement : ConfigurationElement
        {
            [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
            public string Name
            {
                get { return (string)this["name"]; }
                set { this["name"] = value; }
            }

            [ConfigurationProperty("url", IsRequired = false)]
            public string Url
            {
                get { return (string)this["url"]; }
                set { this["url"] = value; }
            }

            [ConfigurationProperty("endpoint", IsRequired = true)]
            public string Endpoint
            {
                get { return (string)this["endpoint"]; }
                set { this["endpoint"] = value; }
            }

            [ConfigurationProperty("method", IsRequired = true)]
            public string Method
            {
                get { return (string)this["method"]; }
                set { this["method"] = value; }
            }

            [ConfigurationProperty("enable", IsRequired = false, DefaultValue = true)]
            public bool Enable
            {
                get { return (bool)this["enable"]; }
                set { this["enable"] = value; }
            }
        }
    }
}