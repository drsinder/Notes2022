using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Notes2022.Server.Protos;


namespace Notes2022.Server.Services
{
    public class Notes2022Service : Notes2022gRPC.Notes2022gRPCBase
    {

        public override Task<GetAboutResponse> GetAbout(Empty request, ServerCallContext context)
        { 
            var response = new GetAboutResponse();
            response.About = new About();

            response.About.PrimeAdminName = Globals.PrimeAdminName;
            response.About.PrimeAdminEmail = Globals.PrimeAdminEmail;
            response.About.StartupDateTime = Globals.StartupDateTime.ToTimestamp();

            return Task.FromResult(response);
        }

        //public override Task<NoteAccessList> GetAccessList(GetAccessListRequest getAccessListRequest, ServerCallContext context)
        //{
        //    int Id = getAccessListRequest.FileId;
        //    List<NoteAccess> list = await _db.NoteAccess.Where(p => p.NoteFileId == Id).OrderBy(p => p.ArchiveId).ToListAsync();
        //    return list;

        //    return null;
        //}

    }
}
