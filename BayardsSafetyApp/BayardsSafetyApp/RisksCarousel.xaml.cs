using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RisksCarousel : CarouselPage //Page that allows to navigate through all risks of the task via swipes
    {
        List<RiskDetails> _risks;
        public RisksCarousel(List<RiskDetails> risks, string riskId, string title)
        {
            InitializeComponent();
            _risks = risks;

            foreach (var risk in _risks)
            {
                Children.Add(risk);
            }
            CurrentPage = Children.FirstOrDefault(x => ((RiskDetails)x).RiskId == riskId);
            Title = title;
        }

    }
}
