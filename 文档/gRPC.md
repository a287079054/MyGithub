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

