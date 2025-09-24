using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace projekat
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "api/{controller}/{action}/{id}"
            );
        }
    }

    public class DateParser : DateTimeConverterBase
    {
        private readonly string format;

        public DateParser(string date)
        {
            format = date;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString(format, CultureInfo.InvariantCulture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.Value == null)
                    return null;

                return DateTime.ParseExact(reader.Value.ToString(), format, CultureInfo.InvariantCulture);
            }
            catch
            {
                return DateTime.Parse(reader.Value.ToString());
            }
        }
    }
}
