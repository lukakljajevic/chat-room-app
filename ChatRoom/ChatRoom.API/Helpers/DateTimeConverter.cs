using System.ComponentModel;
using System.Globalization;

namespace ChatRoom.API.Helpers;

public class DateTimeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string dateString && !string.IsNullOrEmpty(dateString))
        {
            if (DateTime.TryParse(dateString, out var date))
                return date;
                
            return null;
        }
        
        return base.ConvertFrom(context, culture, value);
    }
}