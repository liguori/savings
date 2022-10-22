using System;
using System.Collections.Generic;
using System.Text;

namespace Savings.Model
{
    public class ReportCategoryData
    {
        public string Category { get; set; }
        public string CategoryIcon { get; set; }
        public CategoryResumDataItem[] Data { get; set; }
    }

    public class CategoryResumDataItem
    {
        public string Period { get; set; }
        public double Amount { get; set; }
    }

}
