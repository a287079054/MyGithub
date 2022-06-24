官网：https://www.grpc.io/

```c#
dotnet dev-certs https --clean  //清除证书
dotnet dev-certs https --trust  //信任证书
```

## Protos和C#类型

- 其他类型参考：https://docs.microsoft.com/en-us/aspnet/core/grpc/protobuf?view=aspnetcore-3.1

**标量值始终具有默认值，并且不能设置为`null`. 此约束包括哪些是 C# 类`string`。默认为空字符串值，默认为空字节值。尝试将它们设置为会引发错误。`ByteString``string``ByteString``null`**

<img src="F:\MyGithub\TImg\1.png" style="zoom: 80%;" />

### 日期和时间类型

本机标量类型不提供日期和时间值，相当于 .NET 的[DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset)、[DateTime](https://docs.microsoft.com/en-us/dotnet/api/system.datetime)和[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)。这些类型可以通过使用一些 Protobuf 的*Well-Known Types*扩展来指定。这些扩展为跨受支持平台的复杂字段类型提供代码生成和运行时支持。

![](F:\MyGithub\TImg\2.png)

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

<img src="F:\MyGithub\TImg\3.png" style="zoom:80%;" />

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

![](F:\MyGithub\TImg\4.png)

3.按 Ctrl+F5 在没有调试器的情况下运行。

当项目尚未配置为使用 SSL 时，Visual Studio 会显示以下对话框：

![](F:\MyGithub\TImg\5.png)

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

![](F:\MyGithub\TImg\6.png)

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

![](F:\MyGithub\TImg\7.png)

- 修改项目文件内容

![](F:\MyGithub\TImg\8.png)

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

