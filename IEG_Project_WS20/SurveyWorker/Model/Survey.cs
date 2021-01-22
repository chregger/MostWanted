using System.Collections.Generic;

namespace SurveyWorker.Model
{
    public class Survey
    {
        public int Id { get; set; }

        public string SurveyName { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }
    }
}
