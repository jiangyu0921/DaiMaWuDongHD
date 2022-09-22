using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seckill.Common.Helper
{
    public class DateHelper
    {
        /// <summary>
        /// 获取指定日期的凌晨
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ToDayStartHour()
        {
            var dateString = DateTime.Now.ToString("yyyy-MM-dd");
            dateString += " 00:00:00";
            return DateTime.Parse(dateString);
        }

        /// <summary>
        /// 指定时间往后N个时间间隔
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static List<DateTime> GetDates(int hours)
        {
            List<DateTime> dates = new();
            //循环12次
            DateTime date = ToDayStartHour(); //凌晨
            for (int i = 0; i < hours; i++)
            {
                //每次递增2小时,将每次递增的时间存入到List<Date>集合中
                date.AddHours(i * 2);
                dates.Add(date.AddHours(i * 2));
            }
            return dates;
        }

        /// <summary>
        /// 获取时间菜单
        /// </summary>
        /// <returns></returns>
        public static List<DateTime> GetDateMenus()
        {
            //定义一个List<Date>集合，存储所有时间段
            List<DateTime> dates = GetDates(12);
            //判断当前时间属于哪个时间范围
            DateTime now = DateTime.Now;
            foreach (DateTime cdate in dates)
            {
                //开始时间<=当前时间<开始时间+2小时
                if (cdate.Ticks <= now.Ticks && now.Ticks < cdate.AddHours(2).Ticks)
                {
                    now = cdate;
                    break;
                }
            }

            //当前需要显示的时间菜单
            List<DateTime> dateMenus = new();
            for (int i = 0; i < 5; i++)
            {
                dateMenus.Add(now.AddHours(i * 2));
            }
            return dateMenus;
        }

        /// <summary>
        /// 时间转成yyyyMMddHH
        /// </summary>
        /// <param name="date"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string Data2str(DateTime date)
        {
            return date.ToString("yyyyMMddHH");
        }
    }
}
