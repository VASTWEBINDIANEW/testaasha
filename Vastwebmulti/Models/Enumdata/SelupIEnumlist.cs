using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Vastwebmulti.Models.Enumdata
{
    public enum SelupIEnumlist
    {
        [Display(Name = "Paytm")]
        Paytm = 1,
        [Display(Name = "PhonePe")]
        PhonePe = 2,
        [Display(Name = "GPay")]
        GPay = 3,
        [Display(Name = "Amazon")]
        Amazon = 4,
        [Display(Name = "WhatsApp")]
        WhatsApp = 5

    }
    public static class Extension
    {
        public static MvcHtmlString EnumDropDownListFor<TModel, TProperty, TEnum>(this HtmlHelper<TModel> htmlHelper,
                                                                                    Expression<Func<TModel, TProperty>> expression,
                                                                                    TEnum selectedValue)
        {
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum))
                                        .Cast<TEnum>();
            IEnumerable<SelectListItem> items = from value in values
                                                select new SelectListItem()
                                                {
                                                    Text = value.ToString(),
                                                    Value = value.ToString(),
                                                    Selected = (value.Equals(selectedValue))
                                                };
            return SelectExtensions.DropDownListFor(htmlHelper, expression, items);
        }
    }
}