//------------------------------------------------------------------------------
// This file was originally auto generated, but is not being manually edited
// due to bugginess with refreshing the model from the database
//------------------------------------------------------------------------------

namespace theEducators.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ArticlesWaitingForApporval : Article
    {
        public ArticlesWaitingForApporval()
        {
            this.Tags = new HashSet<Tag>();
        }

        //one added property to find the original article to replace
        //nullable in case this is a new article that has never been approved before
        public Nullable<int> OriginalArticleId { get; set; }
    

    }
}
