namespace Doctyme.Model.ViewModels
{
    public class ReviewCalculationModel
    {
        public int OneStar { get; set; }
        public int TwoStar { get; set; }
        public int ThreeStar { get; set; }
        public int FourStar { get; set; }
        public int FiveStar { get; set; }

        public decimal SumOfStars { get { return OneStar + TwoStar + ThreeStar + FourStar + FiveStar; } }

        public decimal MultiplyOfStars
        {
            get
            {
                return (OneStar * 1) + (TwoStar * 2) + (ThreeStar * 3) + (FourStar * 4) + (FiveStar * 5);
            }
        }

        public decimal TotalRatings
        {
            get
            {
                if (MultiplyOfStars > 0 && SumOfStars > 0)
                    return MultiplyOfStars / SumOfStars;
                return 0;
            }
        }
    }
}
