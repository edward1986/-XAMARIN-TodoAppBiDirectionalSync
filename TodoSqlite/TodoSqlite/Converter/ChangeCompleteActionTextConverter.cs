﻿using System;
using System.Globalization;
using Xamarin.Forms;
namespace TodoSqlite.Converter
{
    public class ChangeCompleteActionTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isCompleted = (int)value;
            return isCompleted == 1 ? "Uncomplete" : "Complete";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not used since we only want to convert a boolean to text, and not the other way around
            throw new NotImplementedException();
        }
    }
}
