using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace GrpcGreeterService.LessonServer
{
    public class ZhaoxiLessonService : ZhaoxiLesson.ZhaoxiLessonBase
    {
        [Authorize("grpcEMail")]//Asp.NetCore
        public override Task<ZhaoxiLessonReply> FindLesson(ZhaoxiLessonRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ZhaoxiLessonReply()
            {
                Lesson = new ZhaoxiLessonReply.Types.LessonModel()
                {
                    Id = request.Id,
                    Name = "架构师蜕变营",
                    Remark = "温暖的大家庭，靠谱的一家人"
                }
            });
        }
    }
}
