using System;
using System.Collections.Generic;
using System.Text;

namespace PolichatApp.API
{
    public class Message
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Text { get; set; }

        public DateTime Added { get; set; }
    }
}
