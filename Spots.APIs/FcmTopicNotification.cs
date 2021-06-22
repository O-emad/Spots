using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;

namespace Spots.APIs
{
    public class FcmTopicNotification
    {
        private readonly IWebHostEnvironment env;
        public string Result { get; set; }
        public FcmTopicNotification(IWebHostEnvironment env)
        {
            this.env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task OnGetAsync(string title, string body, string topic)
        {
            var path = env.ContentRootPath;
            path = path + "\\SpotsFcm.json";
            FirebaseApp app = null;
            try
            {
                app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(path)
                }, "Spots");
            }
            catch (Exception ex)
            {
                app = FirebaseApp.GetInstance("Spots");
            }

            var fcm = FirebaseAdmin.Messaging.FirebaseMessaging.GetMessaging(app);
            Message message = new Message()
            {
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                //Data = new Dictionary<string, string>()
                // {
                //     { "AdditionalData1", "data 1" },
                //     { "AdditionalData2", "data 2" },
                //     { "AdditionalData3", "data 3" },
                // },
                
                Topic = topic
            };

            Result = await fcm.SendAsync(message);
        }

    }
}
