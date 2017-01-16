using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.GSWeb
{
    public static class Constants
    {
        public const byte NOT_SUBSCRIBED = 0;
        public const byte NEW_READER_ADD_ERROR = 1;
        public const byte NEW_READER_EXISTS = 2;
        public const byte NEW_READER_ADDED = 3;
        public const byte READER_REMOVED = 4;
        public const byte REMOVE_NOTFOUND = 5;
        public const decimal PROOF_COST = 6.99M;
        public const string FACEBOOK = "https://www.facebook.com/SevenScienceQuestions";
        public const string AUTHOR_EMAIL = "padgett59@wowway.com";
    }

    public enum OrderStage
    {
        Initial,
        Update,
        Final
    }

}