using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class TimeTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ClassLevelId { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public string Monday { get; set; }
        public string M6_7 { get; set; }
        public string M7_8 { get; set; }
        public string M8_9 { get; set; }
        public string M9_10 { get; set; }
        public string M10_11 { get; set; }
        public string M11_12 { get; set; }
        public string M12_13 { get; set; }
        public string M13_14 { get; set; }
        public string M14_15 { get; set; }
        public string M15_16 { get; set; }
        public string M16_17 { get; set; }
        public string M17_18 { get; set; }
        public string Tuesday { get; set; }
        public string T6_7 { get; set; }
        public string T7_8 { get; set; }
        public string T8_9 { get; set; }
        public string T9_10 { get; set; }
        public string T10_11 { get; set; }
        public string T11_12 { get; set; }
        public string T12_13 { get; set; }
        public string T13_14 { get; set; }
        public string T14_15 { get; set; }
        public string T15_16 { get; set; }
        public string T16_17 { get; set; }
        public string T17_18 { get; set; }
        public string Wednessday { get; set; }
        public string W6_7 { get; set; }
        public string W7_8 { get; set; }
        public string W8_9 { get; set; }
        public string W9_10 { get; set; }
        public string W10_11 { get; set; }
        public string W11_12 { get; set; }
        public string W12_13 { get; set; }
        public string W13_14 { get; set; }
        public string W14_15 { get; set; }
        public string W15_16 { get; set; }
        public string W16_17 { get; set; }
        public string W17_18 { get; set; }
        public string Thursday { get; set; }
        public string Th6_7 { get; set; }
        public string Th7_8 { get; set; }
        public string Th8_9 { get; set; }
        public string Th9_10 { get; set; }
        public string Th10_11 { get; set; }
        public string Th11_12 { get; set; }
        public string Th12_13 { get; set; }
        public string Th13_14 { get; set; }
        public string Th14_15 { get; set; }
        public string Th15_16 { get; set; }
        public string Th16_17 { get; set; }
        public string Th17_18 { get; set; }
        public string Friday { get; set; }
        public string F6_7 { get; set; }
        public string F7_8 { get; set; }
        public string F8_9 { get; set; }
        public string F9_10 { get; set; }
        public string F10_11 { get; set; }
        public string F11_12 { get; set; }
        public string F12_13 { get; set; }
        public string F13_14 { get; set; }
        public string F14_15 { get; set; }
        public string F15_16 { get; set; }
        public string F16_17 { get; set; }
        public string F17_18 { get; set; }

        public string Time6_7 { get; set; }
        public string Time7_8 { get; set; }
        public string Time8_9 { get; set; }
        public string Time9_10 { get; set; }
        public string Time10_11 { get; set; }
        public string Time11_12 { get; set; }
        public string Time12_13 { get; set; }
        public string Time13_14 { get; set; }
        public string Time14_15 { get; set; }
        public string Time15_16 { get; set; }
        public string Time16_17 { get; set; }
        public string Time17_18 { get; set; }
    }
}