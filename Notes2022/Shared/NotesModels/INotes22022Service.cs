using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Notes2022.Shared
{
    [ServiceContract]
    public interface INotes2022Service
    {
        [OperationContract]
        public Task<AboutModel> GetAboutModel();

        [OperationContract]
        public ValueTask<HomePageModel> GetAdminPageData(string userName);

        [OperationContract]
        public ValueTask<HomePageModel> GetHomePageData(string userName);

        [OperationContract]
        public ValueTask<List<NoteAccess>> GetAccessList(string fileid);

        public Task UpdateAccessItem(UpdateAccessRequest req);

        public Task CreateAccessItem(UpdateAccessRequest req);

        public Task DeleteAccessItem(UpdateAccessRequest req);

        public Task SendEmail(EmailModel req);
    }
}
