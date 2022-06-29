官网：https://www.grpc.io/

```c#
dotnet dev-certs https --clean  //清除证书
dotnet dev-certs https --trust  //信任证书
```

- 客户端流，服务端流，双向流调用：https://www.nuget.org/packages/Grpc.Net.Client

## Protos和C#类型

- 其他类型参考：https://docs.microsoft.com/en-us/aspnet/core/grpc/protobuf?view=aspnetcore-3.1

**标量值始终具有默认值，并且不能设置为`null`. 此约束包括哪些是 C# 类`string`。默认为空字符串值，默认为空字节值。尝试将它们设置为会引发错误。`ByteString``string``ByteString``null`**

<img src="F:\MyGithub\gRPC\Img\1.png" style="zoom: 80%;" />

### 日期和时间类型

本机标量类型不提供日期和时间值，相当于 .NET 的[DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset)、[DateTime](https://docs.microsoft.com/en-us/dotnet/api/system.datetime)和[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)。这些类型可以通过使用一些 Protobuf 的*Well-Known Types*扩展来指定。这些扩展为跨受支持平台的复杂字段类型提供代码生成和运行时支持。

![](F:\MyGithub\gRPC\Img\2.png)

C# 类中生成的属性不是 .NET 日期和时间类型。属性使用命名空间中的`Timestamp`和`Duration`类`Google.Protobuf.WellKnownTypes`。这些类提供了与 、 和 相互转换`DateTimeOffset`的`DateTime`方法`TimeSpan`。

```c#
// Create Timestamp and Duration from .NET DateTimeOffset and TimeSpan.
var meeting = new Meeting
{
    Time = Timestamp.FromDateTimeOffset(meetingTime), // also FromDateTime()
    Duration = Duration.FromTimeSpan(meetingLength)
};

// Convert Timestamp and Duration to .NET DateTimeOffset and TimeSpan.
var time = meeting.Time.ToDateTimeOffset();
var duration = meeting.Duration?.ToTimeSpan();
```

该`Timestamp`类型适用于 UTC 时间。`DateTimeOffset`值的偏移量始终为零，并且`DateTime.Kind`属性始终为`DateTimeKind.Utc`.

### 可空类型

C# 的 Protobuf 代码生成使用本机类型，例如`int`for `int32`. 所以这些值总是包含在内，不能是`null`.对于需要显式的值`null`，例如`int?`在 C# 代码中使用，Protobuf 的 Well-Known Types 包括编译为可为空的 C# 类型的包装器。要使用它们，请导入`wrappers.proto`您的`.proto`文件，如以下代码：

```c#
syntax = "proto3"
import "google/protobuf/wrappers.proto"
message Person {
    // ...
    google.protobuf.Int32Value age = 5;
}
```

`wrappers.proto`类型不会在生成的属性中公开。Protobuf 自动将它们映射到 C# 消息中适当的 .NET 可空类型。例如，一个`google.protobuf.Int32Value`字段生成一个`int?`属性。引用类型属性如`string`和`ByteString`不变，除非`null`可以无误地分配给它们。

<img src="F:\MyGithub\gRPC\Img\3.png" style="zoom:80%;" />

### 字节(Bytes)

`bytes`Protobuf 支持标量值类型的二进制有效负载。C# 中生成`ByteString`的属性用作属性类型。

用于`ByteString.CopyFrom(byte[] data)`从字节数组创建新实例

```c#
var data = await File.ReadAllBytesAsync(path);
var payload = new PayloadResponse();
payload.Data = ByteString.CopyFrom(data);
```

`ByteString``ByteString.Span`使用or直接访问数据`ByteString.Memory`。或调用`ByteString.ToByteArray()`将实例转换回字节数组：

```c#
var payload = await client.GetPayload(new PayloadRequest());
await File.WriteAllBytesAsync(path, payload.Data.ToByteArray());
```

### 小数点

Protobuf 本身并不支持 .NET`decimal`类型，只是`double`和`float`. Protobuf 项目中一直在讨论向 Well-Known Types 添加标准十进制类型的可能性，并为支持它的语言和框架提供平台支持。尚未实施任何事情。

可以创建消息定义来表示`decimal`适用于 .NET 客户端和服务器之间的安全序列化的类型。但是其他平台上的开发人员必须了解所使用的格式并实现自己的处理方式。

### 列表(Lists)

Protobuf 中的列表是通过**`repeated`**在字段上使用前缀关键字来指定的。以下示例显示了如何创建列表：

```c#
message Person {
    // ...
    repeated string roles = 8;
}
```

在生成的代码中，`repeated`字段由`Google.Protobuf.Collections.RepeatedField<T>`泛型类型表示。

```c#
public class Person
{
    // ...
    public RepeatedField<string> Roles { get; }
}
```

`RepeatedField<T>`实现[IList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)。因此，您可以使用 LINQ 查询或将其转换为数组或列表。`RepeatedField<T>`属性没有公共设置器。应将项目添加到现有集合中。

```c#
var person = new Person();
// Add one item.
person.Roles.Add("user");

// Add all items from another collection.
var roles = new [] { "admin", "manager" };
person.Roles.Add(roles);
```

###  Dictionaries

IDictionary<TKey,TValue>类型在 Protobuf 中使用`map<key_type, value_type>`.

```C#
message Person {
    // ...
    map<string, string> attributes = 9;
}
```

在生成的 .NET 代码中，`map`字段由`Google.Protobuf.Collections.MapField<TKey, TValue>`泛型类型表示。`MapField<TKey, TValue>`实现[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)。像`repeated`属性一样，`map`属性没有公共设置器。应将项目添加到现有集合中

```c#
var person = new Person();

// Add one item.
person.Attributes["created_by"] = "James";

// Add all items from another collection.
var attributes = new Dictionary<string, string>
{
    ["last_modified"] = DateTime.UtcNow.ToString()
};
person.Attributes.Add(attributes);
```



## 客户端和服务 SSL/TLS 配置不匹配

**解决方案：https://docs.microsoft.com/en-us/aspnet/core/grpc/troubleshoot?view=aspnetcore-3.1#call-a-grpc-service-with-an-untrustedinvalid-certificate**

gRPC 模板和示例默认使用[传输层安全性 (TLS)](https://tools.ietf.org/html/rfc5246)来保护 gRPC 服务。gRPC 客户端需要使用安全连接才能成功调用受保护的 gRPC 服务。

您可以在应用启动时写入的日志中验证 ASP.NET Core gRPC 服务是否使用 TLS。该服务将监听 HTTPS 端点

```c#
info: Microsoft.Hosting.Lifetime[0]   
      Now listening on: https://localhost:5001 //https默认使用TLS安全传输
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
```

.NET Core 客户端必须`https`在服务器地址中使用才能通过安全连接进行调用：

```c#
static async Task Main(string[] args)
{
    // The port number(5001) must match the port of the gRPC server.
    var channel = GrpcChannel.ForAddress("https://localhost:5001");//对应服务端的https
    var client = new Greet.GreeterClient(channel);
}
```

## 使用不受信任/无效的证书调用 gRPC 服务

NET gRPC 客户端要求服务具有受信任的证书。在没有可信证书的情况下调用 gRPC 服务返回如下错误信息：

> 未处理的异常。System.Net.Http.HttpRequestException：无法建立 SSL 连接，请参阅内部异常。---> System.Security.Authentication.AuthenticationException：根据验证程序，远程证书无效

方式1：如果是https请求，并且客户端的框架是.netcore 3.x

```c#
var httpHandler = new HttpClientHandler();
// Return `true` to allow certificates that are untrusted/invalid
httpHandler.ServerCertificateCustomValidationCallback = 
    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

var channel = GrpcChannel.ForAddress("https://localhost:5001",
    new GrpcChannelOptions { HttpHandler = httpHandler });
var client = new Greet.GreeterClient(channel);
```

方式2：http请求,客户端的框架是.netcore 3.x，`System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport`仅 .NET Core 3.x 需要该开关。它在 .NET 5 中什么都不做，也不是必需的。

```c#
   AppContext.SetSwitch(
       "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
    using (var channel = GrpcChannel.ForAddress("http://localhost:5001"))
    {
   		 var client = new CustomMath.CustomMathClient(channel);
    }
```

## 代码实现

- 文档：https://docs.microsoft.com/en-us/aspnet/core/tutorials/grpc/grpc-start?view=aspnetcore-3.1&tabs=visual-studio

### 创建服务端

1. 启动 Visual Studio 并选择**Create a new project**。或者，从 Visual Studio**文件**菜单中，选择**新建**>**项目**。
2. 在**Create a new project**对话框中，选择**gRPC Service**并选择**Next**：

![](F:\MyGithub\gRPC\Img\4.png)

3.按 Ctrl+F5 在没有调试器的情况下运行。

当项目尚未配置为使用 SSL 时，Visual Studio 会显示以下对话框：

![](F:\MyGithub\gRPC\Img\5.png)

gRPC 模板配置为使用[传输层安全性 (TLS)](https://tools.ietf.org/html/rfc5246)。gRPC 客户端需要使用 HTTPS 来调用服务器。

#### 检查项目文件

- *greet.proto*：*Protos/greet.proto*文件定义`Greeter`gRPC 并用于生成 gRPC 服务器资产。有关更多信息，请参阅[gRPC 简介](https://docs.microsoft.com/en-us/aspnet/core/grpc/?view=aspnetcore-3.1)。
- *Services*文件夹：包含服务的实现`Greeter`。
- `appsettings.json`：包含配置数据，例如 Kestrel 使用的协议。有关详细信息，请参阅[ASP.NET Core 中的配置](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1)。
- `Program.cs`：包含 gRPC 服务的入口点。有关详细信息，请参阅[ASP.NET Core 中的 .NET 通用主机](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1)。
- `Startup.cs`：包含配置应用行为的代码。更多信息，请参阅[应用启动](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-3.1)。

###  .NET 控制台应用程序中创建 gRPC 客户端

- 打开 Visual Studio 的第二个实例并选择**Create a new project**。
- 在**Create a new project**对话框中，选择控制台应用.NetCore。
- 在**Project name**文本框中，输入**GrpcGreeterClient**并选择**Create**。

Nuget程序包管理控制台安装包

```c#
Install-Package Grpc.Net.Client
Install-Package Google.Protobuf
Install-Package Grpc.Tools
```

#### 添加 greet.proto

- 在 gRPC 客户端项目中创建一个*Protos文件夹。*
- 将gRPC Greeter 服务中的*Protos\greet.proto文件复制到 gRPC 客户端项目中的**Protos*文件夹。
- 将文件内的命名空间更新为`greet.proto`项目的命名空间：

```c#
option csharp_namespace = "GrpcGreeterClient";
```

- 添加一个项目组，其中包含一个`<Protobuf>`引用*greet.proto*文件的元素：

```c#
<ItemGroup>
  <Protobuf Include="Protos\greet.proto" GrpcServices="Client" />
</ItemGroup>
```

#### 创建 Greeter 客户端

- 构建客户端项目以在`GrpcGreeterClient`命名空间中创建类型。

这些`GrpcGreeterClient`类型由构建过程自动生成。工具包[Grpc.Tools](https://www.nuget.org/packages/Grpc.Tools/)基于*greet.proto*文件生成以下文件：

- `GrpcGreeterClient\obj\Debug\[TARGET_FRAMEWORK]\Protos\Greet.cs`：填充、序列化和检索请求和响应消息类型的协议缓冲区代码。
- `GrpcGreeterClient\obj\Debug\[TARGET_FRAMEWORK]\Protos\GreetGrpc.cs`：包含生成的客户端类。

有关[Grpc.Tools](https://www.nuget.org/packages/Grpc.Tools/)自动生成的 C# 资产的更多信息，请参阅[使用 C# 的 gRPC 服务：生成的 C# 资产](https://docs.microsoft.com/en-us/aspnet/core/grpc/basics?view=aspnetcore-3.1#generated-c-assets)。

**`Program.cs`使用以下代码更新 gRPC 客户端文件：**

- Greeter.GreeterClient需要重新生成客户端控制台项目会自动生成

```c#
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GrpcGreeterClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
```

以上就能在控制台正常调用gRPC服务了

### 使用 gRPC Greeter 服务测试 gRPC 客户端

- 在 Greeter 服务中，按下`Ctrl+F5`以启动不带调试器的服务器。
- 在`GrpcGreeterClient`项目中，按`Ctrl+F5`启动不带调试器的客户端
  在解决方案中同时启动多个项目，将service和client同时执行

*客户端通过包含其名称GreeterClient*的消息向服务发送问候。服务发送消息“Hello GreeterClient”作为响应。“Hello GreeterClient”响应显示在命令提示符中：

```
Greeting: Hello GreeterClient
Press any key to exit...
```

gRPC 服务在写入命令提示符的日志中记录成功调用的详细信息：

```
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\GH\aspnet\docs\4\Docs\aspnetcore\tutorials\grpc\grpc-start\sample\GrpcGreeter
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/2 POST https://localhost:5001/Greet.Greeter/SayHello application/grpc
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'gRPC - /Greet.Greeter/SayHello'
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
      Executed endpoint 'gRPC - /Greet.Greeter/SayHello'
info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished in 78.32260000000001ms 200 application/grpc
```

本文中的代码需要 ASP.NET Core HTTPS 开发证书来保护 gRPC 服务。如果 .NET gRPC 客户端失败并显示消息`The remote certificate is invalid according to the validation procedure.`或`The SSL connection could not be established.`，则开发证书不受信任。要解决此问题，请参阅[使用不受信任/无效的证书调用 gRPC 服务](https://docs.microsoft.com/en-us/aspnet/core/grpc/troubleshoot?view=aspnetcore-3.1#call-a-grpc-service-with-an-untrustedinvalid-certificate)

### 添加自定义proto服务

- 新增CustomMath.proto文件，可以copy其他.proto文件，再修改服务类型，注意命名空间保持一致

![](F:\MyGithub\gRPC\Img\6.png)

- 新增CustomMathService实现

```
 public class CustomMathService : CustomMath.CustomMathBase//自动生成
    {
        private readonly ILogger<CustomMathService> _logger;
        public CustomMathService(ILogger<CustomMathService> logger)
        {
            _logger = logger;
        }


        public override Task<HelloReplyMath> SayHello(HelloRequestMath request, ServerCallContext context)
        {
            return Task.FromResult<HelloReplyMath>(new HelloReplyMath()
            {
                Message = $"This is {request.Id}-{request.Name}"
            });
        }

        public override Task<CountResult> Count(Empty request, ServerCallContext context)
        {

            return Task.FromResult(new CountResult()
            {
                Count = DateTime.Now.Year
            });
        }

        public override Task<ResponseResult> Plus(RequestPara request, ServerCallContext context)
        {
            int iResult = request.ILeft + request.IRight;
            ResponseResult responseResult = new ResponseResult()
            {
                Result = iResult,
                Message = "Success"
            };

            return Task.FromResult(responseResult);
        }
        /// <summary>
        /// 不是一次性计算完，全部返回结果，而是分批次返回
        /// 15*500 然后一起返回
        /// 
        /// 500  500  500  500==
        /// 
        /// yield状态机
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task SelfIncreaseServer(IntArrayModel request, IServerStreamWriter<BathTheCatResp> responseStream, ServerCallContext context)
        {
            foreach (var item in request.Number)
            {
                int number = item;
                Console.WriteLine($"This is {number} invoke");
                await responseStream.WriteAsync(new BathTheCatResp() { Message = $"number++ ={++number}！" });
                await Task.Delay(500);//此处主要是为了方便客户端能看出流调用的效果
            }
            //难道不是一次性返回全部数据，而是一点点的返回？
        }

        public override async Task<IntArrayModel> SelfIncreaseClient(IAsyncStreamReader<BathTheCatReq> requestStream, ServerCallContext context)
        {
            IntArrayModel intArrayModel = new IntArrayModel();
            while (await requestStream.MoveNext())
            {
                intArrayModel.Number.Add(requestStream.Current.Id + 1);
                Console.WriteLine($"SelfIncreaseClient Number {requestStream.Current.Id} 获取到并处理.");
                Thread.Sleep(100);
            }
            return intArrayModel;
        }

        public override async Task SelfIncreaseDouble(IAsyncStreamReader<BathTheCatReq> requestStream, IServerStreamWriter<BathTheCatResp> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                Console.WriteLine($"SelfIncreaseDouble Number {requestStream.Current.Id} 获取到并处理.");
                await responseStream.WriteAsync(new BathTheCatResp() { Message = $"number++ ={requestStream.Current.Id + 1}！" });
                await Task.Delay(500);//此处主要是为了方便客户端能看出流调用的效果
            }

        }
    }
```

- Starup-configure方法添加服务

```c#
   app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<CustomMathService>();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
```

- 将CustomMath.proto拷贝到客户端，并修改命名空间

![](F:\MyGithub\gRPC\Img\7.png)

- 修改项目文件内容

![](F:\MyGithub\gRPC\Img\8.png)

- 客户端调用

```c#
 using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new CustomMath.CustomMathClient(channel);
                //var invoker = channel.Intercept(new CustomClientInterceptor());

                Console.WriteLine("***************单次调用************");
                {
                    var reply = await client.SayHelloAsync(new HelloRequestMath { Name = "Eleven" });
                    Console.WriteLine($"CustomMath {Thread.CurrentThread.ManagedThreadId} 服务返回数据:{reply.Message} ");
                }
 			}
```

### .NetCore调用RPC服务

1. 添加安装包

- Google.Protobuf
- Grpc.Net.Client
- Grpc.Net.ClientFactory
- Grpc.Tools
  <img src="F:\MyGithub\gRPC\Img\9.png" style="zoom:67%;" />

2. 新建Protos文件夹，将.proto文件拷贝到目录下，并修改项目属性

   ```c#
   	<ItemGroup>
   		<Protobuf Include="Protos\CustomMath.proto" GrpcServices="Client" />
   	</ItemGroup>
   ```

3. starup-ConfigureServices方法

   ```c#
      public void ConfigureServices(IServiceCollection services)
           {
               services.AddRazorPages();
               #region gRPC
               //集中管理
               services.AddGrpcClient<CustomMath.CustomMathClient>(options =>
               {
                   options.Address = new Uri("https://localhost:5001");
                   //options.Interceptors.Add(new CustomClientLoggerInterceptor());
               });
               #endregion
           }
   ```

4. Controller调用grpc方法

   ```c#
     public class gRPCController : Controller
       {
           private readonly ILogger<gRPCController> _logger;
           private readonly CustomMath.CustomMathClient _customMathClient;
           //private readonly ZhaoxiLesson.ZhaoxiLessonClient _lessonClient;
           //private readonly ZhaoxiUser.ZhaoxiUserClient _userClient;
           public gRPCController(ILogger<gRPCController> logger
               , CustomMath.CustomMathClient customMathClient
               //, ZhaoxiLesson.ZhaoxiLessonClient lessonClient
               //, ZhaoxiUser.ZhaoxiUserClient userClient
               )
           {
               _logger = logger;
               this._customMathClient = customMathClient;
               //this._lessonClient = lessonClient;
               //this._userClient = userClient;
           }
   
           public async Task<IActionResult> Index()
           {
               //using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
               //{
               //    var client = new CustomMath.CustomMathClient(channel);
   
               //    Console.WriteLine("***************单次调用************");
               //    {
               //        var reply = await client.SayHelloAsync(new HelloRequestMath { Name = "Eleven" });
               //        Console.WriteLine($"CustomMath {Thread.CurrentThread.ManagedThreadId} 服务返回数据:{reply.Message} ");
               //    }
               //}
               {
                   var reply = await this._customMathClient.SayHelloAsync(new HelloRequestMath { Name = "Eleven1" });
                   Console.WriteLine($"CustomMath {Thread.CurrentThread.ManagedThreadId} 服务返回数据1:{reply.Message} ");
               }
               {
                   var reply = this._customMathClient.SayHello(new HelloRequestMath { Name = "Eleven2" });
                   Console.WriteLine($"CustomMath {Thread.CurrentThread.ManagedThreadId} 服务返回数据2:{reply.Message} ");
               }
               {
                   //var reply = await this._lessonClient.FindLessonAsync(new ZhaoxiLessonRequest() { Id = 123 });
   
                   //#region MyRegion
                   //string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiRWxldmVuIiwiRU1haWwiOiI1NzI2NTE3N0BxcS5jb20iLCJBY2NvdW50IjoieHV5YW5nQHpoYW94aUVkdS5OZXQiLCJBZ2UiOiIzMyIsIklkIjoiMTIzIiwiTW9iaWxlIjoiMTg2NjQ4NzY2NzEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsIlNleCI6IjEiLCJuYmYiOjE1OTA3NTgzNDcsImV4cCI6MTU5MDc2MTg4NywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1NzI2IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1NzI2In0.7vMHx62XENyhkksCjnT5AeT78K3zG-z7B3hzv8DGPDI";
                   //var headers = new Metadata { { "Authorization", $"Bearer {token}" } };
   
                   //var reply = await this._lessonClient.FindLessonAsync(new ZhaoxiLessonRequest() { Id = 123 },
                   //    headers: headers);
                   //#endregion
   
                   //var reply = await this._lessonClient.FindLessonAsync(new ZhaoxiLessonRequest() { Id = 123 });
                   //Console.WriteLine($"_lessonClient {Thread.CurrentThread.ManagedThreadId} 服务返回数据3:{Newtonsoft.Json.JsonConvert.SerializeObject(reply.Lesson)} ");
               }
   
               {
                   //var reply = await this._userClient.FindUserAsync(new ZhaoxiUserRequest() { Id = 123 });
                   //Console.WriteLine($"_userClient {Thread.CurrentThread.ManagedThreadId} 服务返回数据4:{Newtonsoft.Json.JsonConvert.SerializeObject(reply.User)} ");
                   //base.ViewBag.Luck = reply.User.Name;
               }
               return View();
           }
       }
   ```

### AOP

项目新建公共类库：GrpcGreeter.Framework

![](F:\MyGithub\gRPC\Img\10.png)

#### 服务端

- 新建类：CustomServerLoggerInterceptor

```c#
 public class CustomServerLoggerInterceptor : Interceptor
    {
        //private readonly ILogger<CustomServerLoggerInterceptor> _logger;

        //public CustomServerLoggerInterceptor(ILogger<CustomServerLoggerInterceptor> logger)
        //{
        //    _logger = logger;
        //}

        /// <summary>
        /// 简单RPC--异步式调用
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            LogAOP<TRequest, TResponse>(MethodType.Unary, context);
            return continuation(request, context);
        }

        private void LogAOP<TRequest, TResponse>(MethodType methodType, ServerCallContext context)
             where TRequest : class
             where TResponse : class
        {
            Console.WriteLine("****************AOP 开始*****************");
            Console.WriteLine($"{context.RequestHeaders[0]}---{context.Host}--{context.Method}--{context.Peer}");
            Console.WriteLine($"Type: {methodType}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
            Console.WriteLine("****************AOP 结束*****************");
        }
    }
```

- configservice方法

```c#
  public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(
             option =>
             { 
                 //AOP,类似特性标记
                 option.Interceptors.Add<CustomServerLoggerInterceptor>();
             });
        }
```

#### 客户端

- 公共类库新建CustomClientLoggerInterceptor类

```c#
 public class CustomClientLoggerInterceptor : Interceptor
    {
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
           TRequest request,
           ClientInterceptorContext<TRequest, TResponse> context,
           AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            this.LogAOP(context.Method);
            return continuation(request, context);
        }

        private void LogAOP<TRequest, TResponse>(Method<TRequest, TResponse> method)
            where TRequest : class
            where TResponse : class
        {
            Console.WriteLine("****************AOP 开始*****************");
            Console.WriteLine($"{method.Name}---{method.FullName}--{method.ServiceName}");
            Console.WriteLine($"Type: {method.Type}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
            Console.WriteLine("****************AOP 结束*****************");
        }

    }
```

- configservice方法

```c#
       services.AddGrpcClient<CustomMath.CustomMathClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");
                options.Interceptors.Add(new CustomClientLoggerInterceptor());
            });
```

- Controller调用grpc方法

### JWT

#### 服务提供者

- 新建项目：Zhaoxi.gRPCDemo.LessonServer

- 添加Protos文件夹，添加ZhaoxiLesson.proto文件

- 添加ZhaoxiLessonService服务实现类

- 修改Zhaoxi.gRPCDemo.LessonServer项目文件

  ```c#
   <ItemGroup>
      <Protobuf Include="Protos\greet.proto" GrpcServices="Server" />
      <Protobuf Include="Protos\ZhaoxiLesson.proto" GrpcServices="Server" />
    </ItemGroup>
  ```

- appsettings.json：添加JWT配置

  ```c#
    "JWTTokenOptions": {
      "Audience": "http://localhost:5726",
      "Issuer": "http://localhost:5726",
      "SecurityKey": "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDI2a2EJ7m872v0afyoSDJT2o1+SitIeJSWtLJU8/Wz2m7gStexajkeD+Lka6DSTy8gt9UwfgVQo6uKjVLG5Ex7PiGOODVqAEghBuS7JzIYU5RvI543nNDAPfnJsas96mSA7L/mD7RTE2drj6hf3oZjJpMPZUQI/B1Qjb5H3K3PNwIDAQAB"
    }
  ```

- Starup-ConfigureServices：添加校验规则

  ```c#
    #region jwt校验  HS
              JWTTokenOptions tokenOptions = new JWTTokenOptions();
              this.Configuration.Bind("JWTTokenOptions", tokenOptions);
              services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                      ValidateIssuer = true,//是否验证Issuer
                      ValidateAudience = true,//是否验证Audience
                      ValidateLifetime = true,//是否验证失效时间
                      ValidateIssuerSigningKey = true,//是否验证SecurityKey
                      ValidAudience = tokenOptions.Audience,//
                      ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//拿到SecurityKey
                  };
              });
              services.AddAuthorization(options =>
              {
                  options.AddPolicy("grpcEMail", policyBuilder =>
                  policyBuilder.RequireAssertion(context =>
                     context.User.HasClaim(c => c.Type == "EMail")
                     && context.User.Claims.First(c => c.Type.Equals("EMail")).Value.EndsWith("@qq.com")));
              });
              #endregion
  ```

- Configure方法

  ```c#
     app.UseEndpoints(endpoints =>
              {
                  endpoints.MapGrpcService<GreeterService>();
                  endpoints.MapGrpcService<ZhaoxiLessonService>();//注册服务
                  endpoints.MapGet("/", async context =>
                  {
                      await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                  });
              });
  ```

  

#### 权限鉴定中心

- 新建项目：GrpcGreeter.AuthenticationCenterJWT

- 配置文件：appsettings.json

  ```c#
  "JWTTokenOptions": {
      "Audience": "http://localhost:5726", //GrpcGreeter.Web.MVC项目启动的地址
      "Issuer": "http://localhost:5726",
      "SecurityKey": "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDI2a2EJ7m872v0afyoSDJT2o1+SitIeJSWtLJU8/Wz2m7gStexajkeD+Lka6DSTy8gt9UwfgVQo6uKjVLG5Ex7PiGOODVqAEghBuS7JzIYU5RvI543nNDAPfnJsas96mSA7L/mD7RTE2drj6hf3oZjJpMPZUQI/B1Qjb5H3K3PNwIDAQAB"
    }
  ```

- StarUp-ConfigureServices方法

  ```c#
    #region HS256
    services.AddScoped<IJWTService, JWTHSService>();
    services.Configure<JWTTokenOptions>(this.Configuration.GetSection("JWTTokenOptions"));
    #endregion
    #region RS256
    //services.AddScoped<IJWTService, JWTRSService>();
    //services.Configure<JWTTokenOptions>(this.Configuration.GetSection("JWTTokenOptions"));
    #endregion
  ```

#### 服务调用方

- GrpcGreeter.Web.MVC

- 添加ZhaoxiLesson.proto文件，修改GrpcGreeter.Web.MVC项目文件

```c#
<ItemGroup>
		<Protobuf Include="Protos\CustomMath.proto" GrpcServices="Client" />
		<Protobuf Include="Protos\ZhaoxiLesson.proto" GrpcServices="Client" />
	</ItemGroup>
```

- Starup-ConfigureService方法

```c#
 public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            #region gRPC
            //集中管理
            services.AddGrpcClient<CustomMath.CustomMathClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");
                options.Interceptors.Add(new CustomClientLoggerInterceptor());
            });

            services.AddGrpcClient<ZhaoxiLesson.ZhaoxiLessonClient>(options =>
            {
                options.Address = new Uri("https://localhost:6001");
                options.Interceptors.Add(new CustomClientLoggerInterceptor());
            })//这里可以采用全局配置，通过JWTTokenHelper.GetJWTToken()获取token后传入Authorization
            .ConfigureChannel(grpcOptions =>
            {
                var callCredentials = CallCredentials.FromInterceptor(async (context, metadata) =>
                {
                    string token = JWTTokenHelper.GetJWTToken().Result;//即时获取的--加一层缓存
                    Console.WriteLine($"token:{token}");
                    metadata.Add("Authorization", $"Bearer {token}");
                });
                grpcOptions.Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials);
                //请求都带上token，也可以在调用方法时传递： var replyPlus = await client.PlusAsync(requestPara, headers);
            });
            #endregion
        }
```

- GrpcController控制器调用方法

  ```c#
    public class GrpcController : Controller
      {
          private readonly ILogger<GrpcController> _logger;
          private readonly ZhaoxiLesson.ZhaoxiLessonClient _lessonClient;
          public GrpcController(ILogger<GrpcController> logger,
       ZhaoxiLesson.ZhaoxiLessonClient lessonClient
              )
          {
              _logger = logger;
              this._lessonClient = lessonClient;
          }
  
          public async Task<IActionResult> Index()
          {
              //全局配置那边获取token后直接调用方法
             var reply = await this._lessonClient.FindLessonAsync(new ZhaoxiLessonRequest() { Id = 123 });
              #region MyRegion 
                  //string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiRWxldmVuIiwiRU1haWwiOiI1NzI2NTE3N0BxcS5jb20iLCJBY2NvdW50IjoieHV5YW5nQHpoYW94aUVkdS5OZXQiLCJBZ2UiOiIzMyIsIklkIjoiMTIzIiwiTW9iaWxlIjoiMTg2NjQ4NzY2NzEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsIlNleCI6IjEiLCJuYmYiOjE2NTYzMTcxOTksImV4cCI6MTY1NjMyMDczOSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1NzI2IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1NzI2In0.A4siN2Z8ZsYFML6OVnjxSv5KJzAP8L2LhgMl5orDhSQ";
             //全局未配置token,则将token在调用的时候传入
             //var headers = new Metadata { { "Authorization", $"Bearer {token}" } };
             //var reply = await this._lessonClient.FindLessonAsync(new ZhaoxiLessonRequest() { Id = 123 },
                  //    headers: headers);
              #endregion
              return View();
          }
      }
  ```

- JWTTokenHelper类

```c#
public class JWTTokenHelper
    {
        public class JWTTokenResult
        {
            public bool result { get; set; }
            public string token { get; set; }
        }

        /// <summary>
        /// 后台通过post请求登陆获取token
        /// </summary>
        /// <returns></returns>
        public async static Task<string> GetJWTToken()
        {
            //string result = await PostWebQuest();
            string result = await PostClient();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<JWTTokenResult>(result).token;
        }

        #region HttpClient实现Post请求
        /// <summary>
        /// HttpClient实现Post请求
        /// </summary>
        private async static Task<string> PostClient()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                {"Name","Eleven" },
                {"Password","123456" }
            };
            string url = "https://localhost:5002/api/Authentication/Login?name=Eleven&password=123456";
            HttpClientHandler handler = new HttpClientHandler();
            using (var http = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(dic);
                var response = await http.PostAsync(url, content);
                Console.WriteLine(response.StatusCode); //确保HTTP成功状态值
                return await response.Content.ReadAsStringAsync();
            }
        }
        #endregion

        #region  HttpWebRequest实现post请求
        /// <summary>
        /// HttpWebRequest实现post请求
        /// </summary>
        private async static Task<string> PostWebQuest()
        {
            var user = new
            {
                Name = "Eleven",
                Password = "123456"
            };
            //此处地址是jwt 登陆获取token
            string url = "http://localhost:5002/api/Authentication/Login?name=Eleven&password=123456";
            var postData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Timeout = 30 * 1000;//设置30s的超时
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.118 Safari/537.36";
            request.ContentType = "application/json";
            request.Method = "POST";
            byte[] data = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = data.Length;
            Stream postStream = request.GetRequestStream();
            postStream.Write(data, 0, data.Length);
            postStream.Close();

            using (var res = request.GetResponse() as HttpWebResponse)
            {
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                    return await reader.ReadToEndAsync();
                }
                else
                {
                    throw new Exception($"请求异常{res.StatusCode}");
                }
            }
        }
        #endregion
    }
```

## Nginx集群

#### 服务端

- 新建GrpcGreeterService.UserServer服务，通过cmd命令启动3个实例

  ```c#
  dotnet GrpcGreeterService.UserServer.dll --urls=https://*:7001
  dotnet GrpcGreeterService.UserServer.dll --urls=https://*:7002
  dotnet GrpcGreeterService.UserServer.dll --urls=https://*:7003
  ```

#### 客户端

- 添加.proto文件

- 修改项目配置

  ```c#
  <ItemGroup>
  		<Protobuf Include="Protos\ZhaoxiUser.proto" GrpcServices="Client" />
  	</ItemGroup>
  
  ```

- Starup-ConfigureServices配置客户端

  ```c#
     services.AddGrpcClient<ZhaoxiUser.ZhaoxiUserClient>(options =>
              {
                  options.Address = new Uri("https://localhost:443");//nginx配置文件中的监听地址
              }).ConfigureChannel(grpcOptions =>
              {
                  Console.WriteLine("This ZhaoxiUser.ZhaoxiUserClien ConfigureChannel");
                  grpcOptions.HttpHandler = new HttpClientHandler()
                  {
                      ServerCertificateCustomValidationCallback = (msg, cert, chain, error) => true
                  };
              });
  ```

- nginx.conf文件配置

```shell

#user  nobody;
worker_processes  1;
#error_log  logs/error.log;
#error_log  logs/error.log  notice;
#error_log  logs/error.log  info;

#pid        logs/nginx.pid;


events {
    worker_connections  1024;
}


http {
    include       mime.types;
    default_type  application/octet-stream;

    #log_format  main  '$remote_addr - $remote_user [$time_local] "$request" '
    #                  '$status $body_bytes_sent "$http_referer" '
    #                  '"$http_user_agent" "$http_x_forwarded_for"';

    #access_log  logs/access.log  main;

    sendfile        on;
    #tcp_nopush     on;

    #keepalive_timeout  0;
    keepalive_timeout  65;

    #gzip  on;

   server {
       listen       8077;#80默认端口可能被占用，修改为其他端口
       server_name  localhost;
   
       #charset koi8-r;
   
       #access_log  logs/host.access.log  main;
   
       location / {
           root   html;
           index  index.html index.htm;
       }
   
       #error_page  404              /404.html;
   
       # redirect server error pages to the static page /50x.html
       #
       error_page   500 502 503 504  /50x.html;
       location = /50x.html {
           root   html;
       }
   
       # proxy the PHP scripts to Apache listening on 127.0.0.1:80
       #
       #location ~ \.php$ {
       #    proxy_pass   http://127.0.0.1;
       #}
   
       # pass the PHP scripts to FastCGI server listening on 127.0.0.1:9000
       #
       #location ~ \.php$ {
       #    root           html;
       #    fastcgi_pass   127.0.0.1:9000;
       #    fastcgi_index  index.php;
       #    fastcgi_param  SCRIPT_FILENAME  /scripts$fastcgi_script_name;
       #    include        fastcgi_params;
       #}
   
       # deny access to .htaccess files, if Apache's document root
       # concurs with nginx's one
       #
       #location ~ /\.ht {
       #    deny  all;
       #}
   }


    # another virtual host using mix of IP-, name-, and port-based configuration
    #
    #server {
    #    listen       8000;
    #    listen       somename:8080;
    #    server_name  somename  alias  another.alias;

    #    location / {
    #        root   html;
    #        index  index.html index.htm;
    #    }
    #}


    # HTTPS server
    #反向代理
   #server {
   #    listen       443 ssl http2;
   #    server_name  localhost;
   #
   #    ssl_certificate      cert.pem;
   #    ssl_certificate_key  cert.key;
   #    #
   #    #ssl_session_cache    shared:SSL:1m;
   #    #ssl_session_timeout  5m;
   #    #
   #    #ssl_ciphers  HIGH:!aNULL:!MD5;
   #    #ssl_prefer_server_ciphers  on;
   #
   #    location / {
   #        grpc_pass grpcs://localhost:7001;
   #    }
   #}

    #负载均衡
    server {
        listen       443 ssl http2; #客户端调用地址
        server_name  localhost;
   
        ssl_certificate      cert.pem;#签名文件
        ssl_certificate_key  cert.key;
        #
        #ssl_session_cache    shared:SSL:1m;
        #ssl_session_timeout  5m;
        #
        #ssl_ciphers  HIGH:!aNULL:!MD5;
        #ssl_prefer_server_ciphers  on;
   
        location / {
            grpc_pass grpcs://ganet;
        }
    }
    #集群地址
    upstream ganet{
        server localhost:7001;
        server localhost:7002;
        server localhost:7003;
        #轮询策略
    }
}

```

- 启动nginx

```
nginx -c ./conf/nginx.conf
```

- nginx目录中的cert.key，cert.pem文件 通过工具生成，暂时未找到怎么生成
- nginx启动成功之后，访问客户端
  ![](F:\MyGithub\gRPC\Img\13.png)

## grpc&WebApi

- 参考：https://docs.microsoft.com/en-us/aspnet/core/grpc/comparison?view=aspnetcore-3.1

![](F:\MyGithub\gRPC\Img\12.png)

## grpc优点

gRPC 消息使用[Protobuf](https://developers.google.com/protocol-buffers/docs/overview)进行序列化，这是一种高效的二进制消息格式。Protobuf 在服务器和客户端上的序列化速度非常快。Protobuf 序列化会产生较小的消息负载，这在移动应用程序等带宽有限的场景中很重要。

gRPC 是为 HTTP/2 设计的，它是 HTTP 的主要修订版，与 HTTP 1.x 相比具有显着的性能优势：

- 二进制成帧和压缩。HTTP/2 协议在发送和接收方面都非常紧凑和高效。
- 通过单个 TCP 连接多路复用多个 HTTP/2 调用。多路复用消除[了行头阻塞](https://en.wikipedia.org/wiki/Head-of-line_blocking)。

HTTP/2 不是 gRPC 独有的。许多请求类型，包括带有 JSON 的 HTTP API，都可以使用 HTTP/2 并从其性能改进中受益。

##  gRPC缺点

#### 有限的浏览器支持

今天从浏览器直接调用 gRPC 服务是不可能的。gRPC 大量使用 HTTP/2 功能，并且没有浏览器提供对 Web 请求所需的控制级别以支持 gRPC 客户端。例如，浏览器不允许调用者要求使用 HTTP/2，或提供对底层 HTTP/2 帧的访问。

将 gRPC 引入浏览器应用程序有两种常见的方法：

- [gRPC-Web](https://grpc.io/docs/tutorials/basic/web.html)是 gRPC 团队的一项附加技术，可在浏览器中提供 gRPC 支持。gRPC-Web 允许浏览器应用程序受益于 gRPC 的高性能和低网络使用率。gRPC-Web 并不支持 gRPC 的所有功能。不支持客户端和双向流式传输，并且对服务器流式传输的支持有限。

  .NET Core 支持 gRPC-Web。有关详细信息，请参阅[在浏览器应用程序中使用 gRPC](https://docs.microsoft.com/en-us/aspnet/core/grpc/browser?view=aspnetcore-3.1)。

- [通过使用HTTP 元数据](https://cloud.google.com/service-infrastructure/docs/service-management/reference/rpc/google.api#google.api.HttpRule)`.proto`注释文件，可以从 gRPC 服务自动创建 RESTful JSON Web API 。这允许应用程序同时支持 gRPC 和 JSON Web API，而无需重复为两者构建单独服务的工作。

  .NET Core 具有从 gRPC 服务创建 JSON Web API 的实验性支持。有关更多信息，请参阅[gRPC JSON 转码](https://docs.microsoft.com/en-us/aspnet/core/grpc/httpapi?view=aspnetcore-3.1)。

#### 人类不可读

HTTP API 请求以文本形式发送，可供人类阅读和创建。

gRPC 消息默认使用 Protobuf 编码。虽然 Protobuf 的发送和接收效率很高，但它的二进制格式不是人类可读的。Protobuf 需要`.proto`文件中指定的消息接口描述才能正确反序列化。需要额外的工具来分析网络上的 Protobuf 有效负载并手动编写请求。

存在诸如[服务器反射](https://github.com/grpc/grpc/blob/master/doc/server-reflection.md)和[gRPC 命令行工具](https://github.com/grpc/grpc/blob/master/doc/command_line_tool.md)之类的功能来协助处理二进制 Protobuf 消息。此外，Protobuf 消息支持[与 JSON 之间的转换](https://developers.google.com/protocol-buffers/docs/proto3#json)。内置的 JSON 转换提供了一种在调试时将 Protobuf 消息转换为人类可读形式和从人类可读形式转换的有效方法。

#### 替代框架方案

在以下情况下，建议使用其他框架而不是 gRPC：

- **浏览器可访问的 API**：浏览器不完全支持 gRPC。gRPC-Web 可以提供浏览器支持，但它有局限性并引入了服务器代理。
- **广播实时通信**：gRPC 支持通过流进行实时通信，但不存在将消息广播到已注册连接的概念。例如，在聊天室场景中，新的聊天消息应该发送到聊天室中的所有客户端，每个 gRPC 调用都需要单独将新的聊天消息流式传输到客户端。[SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.1)是适用于这种情况的有用框架。SignalR 具有持久连接的概念和对广播消息的内置支持。

## gRPC 推荐场景

gRPC 非常适合以下场景：

- **微服务： gRPC**专为低延迟和高吞吐量通信而设计。gRPC 非常适合效率至关重要的轻量级微服务。
- **点对点实时通信**：gRPC 对双向流有出色的支持。gRPC 服务可以实时推送消息，无需轮询。
- **多语言环境**：gRPC 工具支持所有流行的开发语言，使 gRPC 成为多语言环境的不错选择。
- **网络受限环境**：gRPC 消息使用轻量级消息格式 Protobuf 进行序列化。gRPC 消息始终小于等效的 JSON 消息。
- **进程间通信 (IPC)**：IPC 传输（如 Unix 域套接字和命名管道）可以与 gRPC 一起使用，以便在同一台机器上的应用程序之间进行通信。有关更多信息，请参阅[使用 gRPC 的进程间通信](https://docs.microsoft.com/en-us/aspnet/core/grpc/interprocess?view=aspnetcore-3.1)。

