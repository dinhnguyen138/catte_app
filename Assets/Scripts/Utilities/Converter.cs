using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class Converter
{
    public static string ConvertToMoney(Int64 amount)
    {
        double viewValue = amount;
        string viewType = "";
        if (Math.Abs(amount) > 1000000000)
        {
            viewValue = amount / 1000000000;
            viewType = "B";
        }
        if (Math.Abs(amount) > 1000000)
        {
            viewValue = amount / 1000000;
            viewType = "M";
        }
        if (Math.Abs(amount) > 1000)
        {
            viewValue = amount / 1000;
            viewType = "K";
        }

        string ret = string.Format("${0:#,#.##}{1}", viewValue, viewType);
        return ret;
    }

    public static string ConvertToMoneyBasic(long amount)
    {
        string ret = string.Format("${0:0,0", amount);
        return ret;
    }
}
