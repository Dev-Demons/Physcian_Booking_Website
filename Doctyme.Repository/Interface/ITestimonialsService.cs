using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Repository.Interface
{
    public interface ITestimonialsService
    {
        TestimonialItem GetTestimonial(int id);
        int SaveTestimonial(TestimonialItem item);
        TestimonialSearchResults GetTestimonialsForIndex(string SearchString, int PageIndex, int PageSize, string Sort);
        int TestimonialStatusUpdate(StatusUpdateModel item);
        List<TestimonialsForHome> GetTestiMonialsForHome();
    }
}
