# Redis 简介

- 教程：https://www.runoob.com/redis/redis-install.html

Redis 是完全开源的，遵守 BSD 协议，是一个高性能的 key-value 数据库。

Redis 与其他 key - value 缓存产品有以下三个特点：

- Redis支持数据的持久化，可以将内存中的数据保存在磁盘中，重启的时候可以再次加载进行使用。
- Redis不仅仅支持简单的key-value类型的数据，同时还提供list，set，zset，hash等数据结构的存储。
- Redis支持数据的备份，即master-slave模式的数据备份

# Redis 优势

- 性能极高 – Redis能读的速度是110000次/s,写的速度是81000次/s 。
- 丰富的数据类型 – Redis支持二进制案例的 Strings, Lists, Hashes, Sets 及 Ordered Sets 数据类型操作。
- 原子 – Redis的所有操作都是原子性的，意思就是要么成功执行要么失败完全不执行。单个操作是原子性的。多个操作也支持事务，即原子性，通过MULTI和EXEC指令包起来。
- 丰富的特性 – Redis还支持 publish/subscribe, 通知, key 过期等等特性

## Redis与其他key-value存储有什么不同？

- Redis有着更为复杂的数据结构并且提供对他们的原子性操作，这是一个不同于其他数据库的进化路径。Redis的数据类型都是基于基本数据结构的同时对程序员透明，无需进行额外的抽象。
- Redis运行在内存中但是可以持久化到磁盘，所以在对不同数据集进行高速读写时需要权衡内存，因为数据量不能大于硬件内存。在内存数据库方面的另一个优点是，相比在磁盘上相同的复杂的数据结构，在内存中操作起来非常简单，这样Redis可以做很多内部复杂性很强的事情。同时，在磁盘格式方面他们是紧凑的以追加的方式产生的，因为他们并不需要进行随机访问。

# Redis 安装

## Windows 下安装

**下载地址：**https://github.com/tporadowski/redis/releases。

① Redis 支持 32 位和 64 位。这个需要根据你系统平台的实际情况选择，这里我们下载 **Redis-x64-xxx.zip**压缩包到 C 盘，解压后，将文件夹重新命名为 **redis**

![](F:\MyGithub\Redis\img\1.png)

② 打开一个 **cmd** 窗口 使用 cd 命令切换目录到 **C:\redis** 运行

```shell
redis-server.exe redis.windows.conf
```

如果想方便的话，可以把 redis 的路径加到系统的环境变量里，这样就省得再输路径了，后面的那个 redis.windows.conf 可以省略，如果省略，会启用默认的。输入之后，会显示如下界面

![](F:\MyGithub\Redis\img\2.png)

③ 客户端连接

这时候另启一个 cmd 窗口，原来的不要关闭，不然就无法访问服务端了。切换到 redis 目录下运行:

```
redis-cli.exe -h 127.0.0.1 -p 6379
```

## Linux 安装

**下载地址：**http://redis.io/download，下载最新稳定版本

执行完 **make** 命令后，redis-6.0.8 的 **src** 目录下会出现编译后的 redis 服务程序 redis-server，还有用于测试的客户端程序 redis-cli：

```
# wget http://download.redis.io/releases/redis-6.0.8.tar.gz
# tar xzf redis-6.0.8.tar.gz
# cd redis-6.0.8
# make
```

 下面启动 redis 服务：

```
# cd src
# ./redis-server
```

注意这种方式启动 redis 使用的是默认配置。也可以通过启动参数告诉 redis 使用指定配置文件使用下面命令启动。

```
# cd src
# ./redis-server ../redis.conf
```

**redis.conf** 是一个默认的配置文件。我们可以根据需要使用自己的配置文件。

启动 redis 服务进程后，就可以使用测试客户端程序 redis-cli 和 redis 服务交互了。 比如：

```shell
# cd src
# ./redis-cli
redis> set foo bar
OK
redis> get foo
"bar
```

## Docker安装

```shell
docker run -p 8085:6380 -d redis
```

![](F:\MyGithub\Redis\img\22.png)

```shell
docker exec -it 88e0eca8114f /bin/bash #进入安装目录
redis -cli #连接客户端 
set name clay #添加值
get name
```

![](F:\MyGithub\Redis\img\23.png)

# Redis 配置

Redis 的配置文件位于 Redis 安装目录下，文件名为 **redis.conf**(Windows 名为 redis.windows.conf)。

你可以通过修改 redis.conf 文件或使用 **CONFIG set** 命令来修改配置

**CONFIG SET** 命令基本语法：

```shell
redis 127.0.0.1:6379> CONFIG SET loglevel "notice"
OK
redis 127.0.0.1:6379> CONFIG GET loglevel

1) "loglevel"
2) "notice"
```

redis.conf 配置项说明如下
![](F:\MyGithub\Redis\img\3.png)

![](F:\MyGithub\Redis\img\4.png)

![](F:\MyGithub\Redis\img\5.png)

![](F:\MyGithub\Redis\img\6.png)

![](F:\MyGithub\Redis\img\7.png)

# IO多路复用

- 相关博客：https://segmentfault.com/a/1190000041488709

所谓的I/O多路复用，就是通过一种机制，让一个线程可以监视多个链接描述符，一旦某个描述符就绪（一般都是读就绪或者写就绪），就能够通知程序进行相应的读写操作。 这种机制的使用需要select 、 poll 、 epoll 来配合。 多个连接共用一个阻塞对象，应用程序只需要在一个阻塞对象上等待，无需阻塞等待所有连接

# Redis 数据类型

Redis支持八种数据类型：string（字符串），hash（哈希），list（列表），set（集合）及zset(sorted set：有序集合)，Bitmaps，hyperlogloss，streams

## 字符串(String)

- string 是 redis 最基本的类型，你可以理解成与 Memcached 一模一样的类型，一个 key 对应一个 value。
- string 类型是二进制安全的。意思是 redis 的 string 可以包含任何数据。比如jpg图片或者序列化的对象。
- string 类型是 Redis 最基本的数据类型，string 类型的值最大能存储 512MB。

### 常用命令

### 数据结构

string类型对应C语言里面的两种编码格式

-    如果使用raw编码，则每次开辟空间都会留一些空间，如果数据长度变了，则内存也会继续变大。。
-    如果你使用embstr ：它每次最多开辟64个字节的空间，只有44个字节时存储我们数据的。
-    如果你在操作的redis的时候，内容长度小于等于44，则会自动选择embstr编码开辟空间
-    如果你操作redis的时候，内容长度大于44的，使用ram编码，浪费 空间 
-    还有一个int:只是针对于写的数据是数值，才行。。切记只有整型才是int类型  

![](F:\MyGithub\Redis\img\8.png)

```shell
object encoding name #查看编码类型
```

![](F:\MyGithub\Redis\img\24.png)

### 应用实例

**秒杀**

```
{
                    //////命令行参数启动
                    //dotnet RedisSeckill.dll --id=1 minute=18
                    var builder = new ConfigurationBuilder().AddCommandLine(args);
                    var configuration = builder.Build();
                    string id = configuration["id"];
                    int minute = int.Parse(configuration["minute"]);
                    Console.WriteLine("开始" + id);

                    using (RedisClient client = new RedisClient("127.0.0.1", 6379))
                    {
                        //首先给数据库预支了秒杀商品的数量
                        client.Set<int>("number", 10);
                    }
                    Seckill.Show(id, minute);

   }
```

```
	public class Seckill
	{
		public static void Show(string id, int minute)
		{
			#region 自减1，返回自减后的值
			//开启10个线程去抢购
			Console.WriteLine($"在{minute}分0秒正式开启秒杀！");
			var flag = true;
			while (flag)
			{
				if (DateTime.Now.Minute == minute)
				{
					flag = false;
					for (int i = 0; i < 10; i++)
					{
						string name = $"客户端{id}号:{i}";
						Task.Run(() =>
						{
							using (RedisClient client = new RedisClient("127.0.0.1", 6379))
							{
								
								//本来是二步走
								//1 订单+1
								//2 库存-1 //同一个时间片。只会执行一条指令
								var num = client.Decr("number");
								if (num < 0)
								{
									Console.WriteLine(name + "抢购失败");
								}
								//>=0
								else
								{
									Console.WriteLine(name + "**********抢购成功***************");
								}
							}
						});
						Thread.Sleep(10);
					}
				}
				Thread.Sleep(10);
			}
			Console.ReadLine();
			#endregion
		}
	}
```

![](F:\MyGithub\Redis\img\9.png)

## 哈希(Hash)

Redis hash 是一个 string 类型的 field（字段） 和 value（值） 的映射表，hash 特别适合用于存储对象。

Redis 中每个 hash 可以存储 2的正负32次方（40多亿）

### 数据结构

- ZipList 类似压缩的数组类型，插入和查询的时候是从头至尾检索，因此当filed的个数超过512或者任意一个file的值的长度大于64个字节的时候用hashTable
- HashTable

![](F:\MyGithub\Redis\img\10.png)

## 集合(Set)

- Redis 的 Set 是 String 类型的无序集合。集合成员是唯一的，这就意味着集合中不能出现重复的数据。
- 集合对象的编码可以是 intset 或者 hashtable。
- Redis 中集合是通过哈希表实现的，所以添加，删除，查找的复杂度都是 O(1)。
- 集合中最大的成员数为 2的正负32次方(4294967295, 每个集合可存储40多亿个成员)。

### 数据结构

- intzset：存的值是数字类型，长度不超过512
- HashTable

![](F:\MyGithub\Redis\img\11.png)

### 应用场景

- 无序去重
- 交集
- 并集

## 有序集合(sorted set)

- Redis 有序集合和集合一样也是 string 类型元素的集合,且不允许重复的成员。
- 不同的是每个元素都会关联一个 double 类型的分数。redis 正是通过分数来为集合中的成员进行从小到大的排序。
- 有序集合的成员是唯一的,但分数(score)却可以重复。
- 集合是通过哈希表实现的，所以添加，删除，查找的复杂度都是 O(1)。 集合中最大的成员数为 232 - 1 (4294967295, 每个集合可存储40多亿个成员)。

### 数据结构

- 
  ziplist作为zset的存储结构时，格式如下图，细节就不多说了，我估计大家都看得懂，紧挨着的是元素memeber和分值socore，整体数据是有序格式

![](F:\MyGithub\Redis\img\12.png)

- skiplist作为zset的存储结构，整体存储结构如下图，核心点主要是包括一个dict对象和一个skiplist对象。dict保存key/value，key为元素，value为分值；skiplist保存的有序的元素列表，每个元素包括元素和分值。两种数据结构下的元素指向相同的位置

![](F:\MyGithub\Redis\img\13.png)

### 应用场景

- 交叉并补
- 有序
- 去重

## 列表(List)

- Redis列表是简单的字符串列表，按照插入顺序排序。你可以添加一个元素到列表的头部（左边）或者尾部（右边）
- 一个列表最多可以包含 232 - 1 个元素 (4294967295, 每个列表超过40亿个元素)。

### 数据结构

- 双向链表：增删快，查找慢

## BitMaps

是一个节省内存的数据结构。。0,1...

## Hyperlogloss

基准统计数据结构类型,使用少量的内存，存放大量的数据。。。统计的结果有误差

## Bloom Filte(布隆过滤器)

- 存储数据----占用很少的内存存储跟多的数据。。
- 判断数据在不在存储的里面 ---存在一个查询效率的问题

bit === 8个位==1一个字节
--- 建立一个数据结构--bit[] bit数组
通过布隆过滤器判断key在不在里面，有一个误判的问题。。。
bit数组越长，hash的方法越多，则占用内存越多，但是误判的概率就小。
bit数组越短，hash方法越少，则占用的内存越少，但是误判的概率就大。。。

安装布隆过滤器

```shell
docker run -p 6379:6379 --name redis-redisbloom redislabs/rebloom:latest
```

通过cli用 RedisBloom

```shell
docker exec -it redis-redisbloom bash
redis-cli
```

```shell
127.0.0.1:6379> bf.add myfilter a # bf Bloom Filte缩写，myfilter 自定义key
(integer) 1
127.0.0.1:6379> bf.add myfillter b
(integer) 1
127.0.0.1:6379> bf.exists myfillter a
(integer) 0
127.0.0.1:6379> bf.exists myfilter a
(integer) 1
```

![](F:\MyGithub\Redis\img\25.png)

```c#
    using (RedisClient client = new RedisClient("192.168.1.211", 6379))
    {
        client.Custom("bf.add", "myfilter", "dddd");
        Console.WriteLine(client.Custom("bf.exists", "myfilter", "dddd").Text);
    }
	Console.WriteLine("ok");
```



### 应用场景

- 队列
- 栈
- 发布订阅（查看当前目录代码）
  ![](F:\MyGithub\Redis\img\14.png)

# Redis事务

Redis 事务可以一次执行多个命令， 并且带有以下三个重要的保证：

- 批量操作在发送 EXEC 命令前被放入队列缓存。
- 收到 EXEC 命令后进入事务执行，事务中任意命令执行失败，其余的命令依然被执行。
- 在事务执行过程，其他客户端提交的命令请求不会插入到事务执行命令序列中

## Redis 事务命令

![](F:\MyGithub\Redis\img\15.png)

## **代码实现**

1.当我们未使用client.Watch("a","b","c")监听的时候，代码执行using (var trans = client.CreateTransaction())创建事务的时候，再通过redis管理工具修改数据的，flag=true，当我们使用client.Watch("a","b","c")之后，再去通过redis管理工具修改数据，flag返回false。

```c#
using (RedisClient client = new RedisClient("127.0.0.1", 6379))
				{
					//删除当前数据库中的所有Key  默认删除的是db0
					client.FlushDb();
					//删除所有数据库中的key 
					client.FlushAll();
					client.Set("a", "1");
					client.Set("b", "1");
					client.Set("c", "1");
					////获取当前这三个key的版本号 实现事务
					//client.Watch("a","b","c");
					using (var trans = client.CreateTransaction())
					{
						trans.QueueCommand(p => p.Set("a", "3"));
						trans.QueueCommand(p => p.Set("b", "3"));
						trans.QueueCommand(p => p.Set("c", "3"));

						var flag = trans.Commit();
						Console.WriteLine(flag);
					}
                }
```

- 建议：如果要使用redis的事务，则需要和Watch方法一起使用，而且最好，监听的这些字段都包含在所有的key里面

![](F:\MyGithub\Redis\img\26.png)

![](F:\MyGithub\Redis\img\27.png)

## lua

可以把lua理解成数据存储过程，但是实质不一样，而且lua具有原子性，而且性能要比我们直接操作指令性能要高---lua是c语言编写的，redis也是c语言编写，不存在编译过程--- 

```c#
using (var client = new RedisClient("192.168.1.211", 6379))
			{
				//Console.WriteLine(client.ExecLuaAsString(@"return  redis.call('get','name')"));
				//库存
				//Console.WriteLine(client.ExecLuaAsString(@"redis.call('set','number','10')"));
				var lua = @"local count = redis.call('get',KEYS[1])
				                        if(tonumber(count)>=0)
				                        then
				                            redis.call('INCR',ARGV[1])
				                            return redis.call('DECR',KEYS[1])
				                        else
				                            return -1
				                        end";
				Console.WriteLine(client.ExecLuaAsString(lua, keys: new[] { "number" }, args: new[] { "ordercount" }));


			}
```



# Redis持久化

- 参考连接：https://segmentfault.com/a/1190000039208726

Redis是一个内存数据库，所有的数据将保存在内存中，这与传统的MySQL、Oracle、SqlServer等关系型数据库直接把数据保存到硬盘相比，Redis的读写效率非常高。但是保存在内存中也有一个很大的缺陷，一旦断电或者宕机，内存数据库中的内容将会全部丢失。为了弥补这一缺陷，Redis提供了把内存数据持久化到硬盘文件，以及通过备份文件来恢复数据的功能，即Redis持久化机制

## RDB

RDB快照用官方的话来说：RDB持久化方案是按照指定时间间隔对你的数据集生成的时间点快照（point-to-time snapshot）。它以紧缩的二进制文件保存Redis数据库某一时刻所有数据对象的内存快照，可用于Redis的数据备份、转移与恢复。到目前为止，仍是官方的默认支持方案。

### 1. 持久化流程

在Redis内完成RDB持久化的方法有rdbSave和rdbSaveBackground两个函数方法（源码文件rdb.c中），先简单说下两者差别：

- rdbSave：是同步执行的，方法调用后就会立刻启动持久化流程。由于Redis是单线程模型，持久化过程中会阻塞，Redis无法对外提供服务；(save)
- rdbSaveBackground：是后台（异步）执行的，该方法会fork出子进程，真正的持久化过程是在子进程中执行的（调用rdbSave），主进程会继续提供服务；(bgsave)

RDB持久化的触发必然离不开以上两个方法，触发的方式分为手动和自动。手动触发容易理解，是指我们通过Redis客户端人为的对Redis服务端发起持久化备份指令，然后Redis服务端开始执行持久化流程，这里的指令有save和bgsave。自动触发是Redis根据自身运行要求，在满足预设条件时自动触发的持久化流程，自动触发的场景有如下几个（[摘自这篇文章](https://link.segmentfault.com/?enc=r5J75flbLcicK%2FZqFJHITg%3D%3D.h0A7v3vqCVFfl950RXFmintiF5K9pqq0RWuSlwzzWTeijG05wanXNavHRQxOED6K)）：

- serverCron中`save m n`配置规则自动触发；
- 从节点全量复制时，主节点发送rdb文件给从节点完成复制操作，主节点会出发bgsave；
- 执行`debug reload`命令重新加载redis时；
- 默认情况下（未开启AOF）执行shutdown命令时，自动执行bgsave；

![](F:\MyGithub\Redis\img\16.png)

### save规则及检查

serverCron是Redis内的一个周期性函数，每隔100毫秒执行一次，它的其中一项工作就是：根据配置文件中save规则来判断当前需要进行自动持久化流程，如果满足条件则尝试开始持久化。了解一下这部分的实现。

在`redisServer配置文件`中有几个与RDB持久化有关的字段，我从代码中摘出来，中英文对照着看下

```shell
# 表示900秒（15分钟）内至少有1个key的值发生变化，则执行
save 900 1
# 表示300秒（5分钟）内至少有1个key的值发生变化，则执行
save 300 10
# 表示60秒（1分钟）内至少有10000个key的值发生变化，则执行
save 60 10000
# 该配置将会关闭RDB方式的持久化
save ""
```

## AOF

上一节我们知道RDB是一种时间点（point-to-time）快照，适合数据备份及灾难恢复，由于工作原理的“先天性缺陷”无法保证实时性持久化，这对于缓存丢失零容忍的系统来说是个硬伤，于是就有了AOF

#### AOF工作原理

- AOF是Append Only File的缩写，它是Redis的完全持久化策略，这些集合按照Redis Server的处理顺序追加到文件中。当重启Redis时，Redis就可以从头读取AOF中的指令并重放，进而恢复关闭前的数据状态。

- AOF持久化默认是关闭的，修改redis.conf以下信息并重启，即可开启AOF持久化功能。

```shell
# no-关闭，yes-开启，默认no
appendonly yes
appendfilename appendonly.aof

```

AOF本质是为了持久化，持久化对象是Redis内每一个key的状态，持久化的目的是为了在Reids发生故障重启后能够恢复至重启前或故障前的状态。相比于RDB，AOF采取的策略是按照执行顺序持久化每一条能够引起Redis中对象状态变更的命令，命令是有序的、有选择的。把aof文件转移至任何一台Redis Server，从头到尾按序重放这些命令即可恢复如初

- Redis Server启动后，aof文件一直在追加命令，文件会越来越大。文件越大，Redis重启后恢复耗时越久；文件太大，转移工作就越难；不加管理，可能撑爆硬盘。很显然，需要在合适的时机对文件进行精简。例子中的5条incr指令很明显的可以替换为为一条`set`命令，存在很大的压缩空间。
- 众所周知，文件I/O是操作系统性能的短板，为了提高效率，文件系统设计了一套复杂的缓存机制，Redis操作命令的追加操作只是把数据写入了缓冲区（aof_buf），从缓冲区到写入物理文件在性能与安全之间权衡会有不同的选择。
- 文件压缩即意味着重写，重写时即可依据已有的aof文件做命令整合，也可以先根据当前Redis内数据的状态做快照，再把存储快照过程中的新增的命令做追加。
- aof备份后的文件是为了恢复数据，结合aof文件的格式、完整性等因素，Redis也要设计一套完整的方案做支持

#### 持久化流程

从流程上来看，AOF的工作原理可以概括为几个步骤：命令追加（append）、文件写入与同步（fsync）、文件重写（rewrite）、重启加载（load），接下来依次了解每个步骤的细节。

![](F:\MyGithub\Redis\img\17.png)

##### ①命令追加

当 AOF 持久化功能处于打开状态时，Redis 在执行完一个写命令之后，会以协议格式(也就是RESP，即 Redis 客户端和服务器交互的通信协议 )把被执行的写命令追加到 Redis 服务端维护的 AOF 缓冲区末尾。**需要注意的是：如果命令追加时正在进行AOF重写，这些命令还会追加到重写缓冲区**

##### ②文件写入与同步

Redis每次事件轮训结束前（`beforeSleep`）都会调用函数`flushAppendOnlyFile`，`flushAppendOnlyFile`会把AOF缓冲区（`aof_buf`）中的数据写入内核缓冲区，并且根据`appendfsync`配置来决定采用何种策略把内核缓冲区中的数据写入磁盘，即调用`fsync()`。该配置有三个可选项`always`、`no`、`everysec`，具体说明如下：

```shell
#appendfsync always：每次都调用fsync()，是安全性最高、性能最差的一种策略。
#appendfsync no：不会调用fsync()。性能最好，安全性最差。
appendfsync everysec：#仅在满足同步条件时调用fsync()。这是官方建议的同步策略，也是默认配置，做到兼顾性能和数据安全性，理论上只有在系统突然宕机的情况下丢失1秒的数据。
```

**注意：上面介绍的策略受配置项`no-appendfsync-on-rewrite`的影响，它的作用是告知Redis：AOF文件重写期间是否禁止调用fsync()，默认是no。**

如果`appendfsync`设置为`always`或`everysec`，后台正在进行的`BGSAVE`或者`BGREWRITEAOF`消耗过多的磁盘I/O，在某些Linux系统配置下，Redis对fsync()的调用可能阻塞很长时间。然而这个问题还没有修复，因为即使是在不同的线程中执行`fsync()`，同步写入操作也会被阻塞。

为了缓解此问题，可以使用该选项，以防止在进行`BGSAVE`或`BGREWRITEAOF`时在主进程中调用fsync(）。

- 设置为`yes`意味着，如果子进程正在进行`BGSAVE`或`BGREWRITEAOF`，AOF的持久化能力就与`appendfsync`设置为`no`有着相同的效果。最糟糕的情况下，这可能会导致30秒的缓存数据丢失。
- 如果你的系统有上面描述的延迟问题，就把这个选项设置为`yes`，否则保持为`no`。

##### ③文件重写

如前面提到的，Redis长时间运行，命令不断写入AOF，文件会越来越大，不加控制可能影响宿主机的安全。

为了解决AOF文件体积问题，Redis引入了AOF文件重写功能，它会根据Redis内数据对象的最新状态生成新的AOF文件，新旧文件对应的数据状态一致，但是新文件会具有较小的体积。重写既减少了AOF文件对磁盘空间的占用，又可以提高Redis重启时数据恢复的速度。还是下面这个例子，旧文件中的6条命令等同于新文件中的1条命令，压缩效果显而易见

![](F:\MyGithub\Redis\img\18.png)

**我们说，AOF文件太大时会触发AOF文件重写，那到底是多大呢？有哪些情况会触发重写操作呢？**
**
与RDB方式一样，AOF文件重写既可以手动触发，也会自动触发。手动触发直接调用`bgrewriteaof`命令，如果当时无子进程执行会立刻执行，否则安排在子进程结束后执行。自动触发由Redis的周期性方法`serverCron`检查在满足一定条件时触发。先了解两个配置项：

- auto-aof-rewrite-percentage：代表当前AOF文件大小（aof_current_size）和上一次重写后AOF文件大小（aof_base_size）相比，增长的比例。
- auto-aof-rewrite-min-size：表示运行`BGREWRITEAOF`时AOF文件占用空间最小值，默认为64MB

Redis启动时把`aof_base_size`初始化为当时aof文件的大小，Redis运行过程中，当AOF文件重写操作完成时，会对其进行更新；`aof_current_size`为`serverCron`执行时AOF文件的实时大小。当满足以下两个条件时，AOF文件重写就会触发：

```shell
#增长比例：(aof_current_size - aof_base_size) / aof_base_size > auto-aof-rewrite-percentage
#文件大小：aof_current_size > auto-aof-rewrite-min-size
```

##### ④ 数据加载

Redis启动后通过`loadDataFromDisk`函数执行数据加载工作。这里需要注意，虽然持久化方式可以选择AOF、RDB或者两者兼用，但是数据加载时必须做出选择，两种方式各自加载一遍就乱套了。

理论上，AOF持久化比RDB具有更好的实时性，当开启了AOF持久化方式，**Redis在数据加载时优先考虑AOF方式**。而且，Redis 4.0版本后AOF支持了混合持久化，加载AOF文件需要考虑版本兼容性

**加载流程图**

![](F:\MyGithub\Redis\img\20.png)

如果在AOF命令追加过程中发生宕机，由于延迟写的技术特点，AOF的RESP命令可能不完整（被截断）。遇到这种情况时，Redis会按照配置项`aof-load-truncated`执行不同的处理策略。这个配置是告诉Redis启动时读取aof文件，如果发现文件被截断（不完整）时该如何处理：

- yes：则尽可能多的加载数据，并以日志的方式通知用户；
- no：则以系统错误的方式崩溃，并禁止启动，需要用户修复文件后再重启

#### AOF文件格式含义

![](F:\MyGithub\Redis\img\19.png)

## 混合模式

- redis..4X 版本之后，混合模式--- 

- 混合模式【备份，启动快】

- 备份的时候，我通过aof备份，我还原的时候根据rdb...

aof--文件重写的问题：当数据大的时候，当我们进行aof操作的时候，把aof文件变成rdb，然后之后的操作直接取做日志追加。。

```shell
bgrewriteaof #重写命令，同时会吧aof文件转换成rdb文件
```

![](F:\MyGithub\Redis\img\28.png)

## 总结

Redis提供了两种持久化的选择：**RDB支持以特定的实践间隔为数据集生成时间点快照；AOF把Redis Server收到的每条写指令持久化到日志中，待Redis重启时通过重放命令恢复数据**。日志格式为RESP协议，对日志文件只做append操作，无损坏风险。并且当AOF文件过大时可以自动重写压缩文件。

#### RDB vs AOF

##### RDB优点

- RDB是一个紧凑压缩的二进制文件，代表Redis在某一个时间点上的数据快照，非常适合用于备份、全量复制等场景。
- RDB对灾难恢复、数据迁移非常友好，RDB文件可以转移至任何需要的地方并重新加载。
- RDB是Redis数据的内存快照，数据恢复速度较快，相比于AOF的命令重放有着更高的性能。
- 备份慢，启动快

##### RDB缺点

- RDB方式无法做到实时或秒级持久化。因为持久化过程是通过fork子进程后由子进程完成的，子进程的内存只是在fork操作那一时刻父进程的数据快照，而fork操作后父进程持续对外服务，内部数据时刻变更，子进程的数据不再更新，两者始终存在差异，所以无法做到实时性。
- RDB持久化过程中的fork操作，会导致内存占用加倍，而且父进程数据越多，fork过程越长。
- Redis请求高并发可能会频繁命中save规则，导致fork操作及持久化备份的频率不可控；
- RDB文件有文件格式要求，不同版本的Redis会对文件格式进行调整，存在老版本无法兼容新版本的问题。

##### AOF优点

- AOF持久化有更好的实时性，我们可以选择三种不同的方式（appendfsync）：no、every second、always，every second作为默认的策略具有最好的性能，极端情况下可能会丢失一秒的数据。
- AOF文件只有append操作，无复杂的seek等文件操作，没有损坏风险。即使最后写入数据被截断，也很容易使用`redis-check-aof`工具修复；
- 当AOF文件变大时，Redis可在后台自动重写。重写过程中旧文件会持续写入，重写完成后新文件将变得更小，并且重写过程中的增量命令也会append到新文件。
- AOF文件以已于理解与解析的方式包含了对Redis中数据的所有操作命令。即使不小心错误的清除了所有数据，只要没有对AOF文件重写，我们就可以通过移除最后一条命令找回所有数据。
- AOF已经支持混合持久化，文件大小可以有效控制，并提高了数据加载时的效率。
- 备份快，启动慢


##### AOF缺点

- 对于相同的数据集合，AOF文件通常会比RDB文件大；
- 在特定的fsync策略下，AOF会比RDB略慢。一般来讲，fsync_every_second的性能仍然很高，fsync_no的性能与RDB相当。但是在巨大的写压力下，RDB更能提供最大的低延时保障。
- 在AOF上，Redis曾经遇到一些几乎不可能在RDB上遇到的罕见bug。一些特殊的指令（如BRPOPLPUSH）导致重新加载的数据与持久化之前不一致，Redis官方曾经在相同的条件下进行测试，但是无法复现问题

#### 使用建议

对RDB和AOF两种持久化方式的工作原理、执行流程及优缺点了解后，我们来思考下，实际场景中应该怎么权衡利弊，合理的使用两种持久化方式。如果仅仅是使用Redis作为缓存工具，所有数据可以根据持久化数据库进行重建，则可关闭持久化功能，做好预热、缓存穿透、击穿、雪崩之类的防护工作即可。

一般情况下，Redis会承担更多的工作，如分布式锁、排行榜、注册中心等，持久化功能在灾难恢复、数据迁移方面将发挥较大的作用。建议遵循几个原则：

- 不要把Redis作为数据库，所有数据尽可能可由应用服务自动重建。
- 使用4.0以上版本Redis，使用AOF+RDB混合持久化功能。
- 合理规划Redis最大占用内存，防止AOF重写或save过程中资源不足。
- 避免单机部署多实例。
- 生产环境多为集群化部署，可在slave开启持久化能力，让master更好的对外提供写服务。
- 备份文件应自动上传至异地机房或云存储，做好灾难备份。

#### 关于fork()

通过上面的分析，我们都知道RDB的快照、AOF的重写都需要fork，这是一个重量级操作，会对Redis造成阻塞。因此为了不影响Redis主进程响应，我们需要尽可能降低阻塞。

- 降低fork的频率，比如可以手动来触发RDB生成快照、与AOF重写；
- 控制Redis最大使用内存，防止fork耗时过长；
- 使用更高性能的硬件；
- 合理配置Linux的内存分配策略，避免因为物理内存不足导致fork失败。

# Redis环境性能测试

```shell
redis-benchmark -h 127.0.0.1 -p 6379 -c 50 -n 10000
redis-benchmark -h 127.0.0.1 -p 6086 -c 50 -n 10000 -t get
```

![](F:\MyGithub\Redis\img\21.png)

# Redis集群

Redis的集群方案大致有三种：1）redis cluster集群方案；2）master/slave主从方案；3）哨兵模式来进行主从替换以及故障恢复。

## 主从（master/slave）

保存主节点信息：配置slaveof之后会在从节点保存主节点的信息。主从建立socket连接：定时发现主节点以及尝试建立连接。
发送ping命令：从节点定时发送ping给主节点，主节点返回PONG。若主节点没有返回PONG或因阻塞无法响应导致超时，则主从断开，在下次定时任务时会从新ping主节点。

- 同步数据集：首次连接，全量同步。
- 命令持续复制：全量同步完成后，保持增量同步。

![](F:\MyGithub\Redis\img\44.png)

### docker部署

① 创建容器

```shell
docker run -d -p 5001:6379 --name redis1  redis  #主
docker run -d -p 5002:6379 --name redis2  redis #从
docker run -d -p 5003:6379 --name redis3  redis #从

docker exec -it  redis1 /bin/bash
docker exec -it  redis2 /bin/bash
docker exec -it  redis3 /bin/bash
```

② 配置（两种方式）

- 命令行模式：在从服务器命令行中执行下面的命令即可成为该主服务器的从节点：

```shell
#在从服务器执行下面的命令成为或取消成为某节点的从节点
#slaveof  主服务器的IP  端口号 （阿里云服务器安全组规则记得配置5001端口）
slaveof  120.25.102.199 5001
#取消成为任何服务器的从服务器
slaveof no one
#从服务器只读(推荐配置)
config set slave-read-only yes
#查看主从信息
info replication
```

`slaveof` 命令是异步的，不会阻塞。 同时，从服务器现有的数据会先被清空，然后才会同步主服务器的数据

- 配置文件：在从服务器配置文件中添加下面的配置然后重启从服务器即可：

```shell
#在从节点配置文件中新增下面两个配置即可指定成为某个主节点的从节点
#slaveof 主节点地址 主节点端口
slaveof  host port
 
#从服务器只读(推荐配置)
slave-read-only yes
```

![](F:\MyGithub\Redis\img\29.png)

- 主节点加数据

![](F:\MyGithub\Redis\img\30.png)

- 从节点读取数据

![](F:\MyGithub\Redis\img\31.png)

### windows部署

- master-6382.conf

```shell
port 6382
repl-diskless-sync yes
repl-diskless-sync-delay 0
databases 2000
maxmemory 2gb
dir "../Temp"
appendonly no
dbfilename "master-6382.rdb"
save ""
```

- replica-6383.conf

```shell
port 6383
slaveof 127.0.0.1 6382
repl-diskless-sync yes
repl-diskless-sync-delay 0
databases 2000
maxmemory 2gb
appendonly no
dir "../Temp"
dbfilename "replica-6383.rdb"
save ""
```

## Sentinel(哨兵)

Sentinel(哨兵)是用于监控redis集群中Master状态的工具，是Redis 的高可用性解决方案，sentinel哨兵模式已经被集成在redis2.4之后的版本中。sentinel是redis高可用的解决方案，sentinel系统可以监视一个或者多个redis master服务，以及这些master服务的所有从服务；当某个master服务下线时，自动将该master下的某个从服务升级为master服务替代已下线的master服务继续处理请求。

**哨兵，最少是三台服务器，必须是单数（投票防止平票的情况）---**

![](F:\MyGithub\Redis\img\32.png)

### 基本概念

- 参考：https://www.cnblogs.com/kevingrace/p/9004460.html

**sentinel在内部有3个定时任务**
1）每10秒每个sentinel会对master和slave执行info命令，这个任务达到两个目的：
a）发现slave节点
b）确认主从关系
2）每2秒每个sentinel通过master节点的channel交换信息（pub/sub）。master节点上有一个发布订阅的频道(__sentinel__:hello)。sentinel节点通过__sentinel__:hello频道进行信息交换(对节点的"看法"和自身的信息)，达成共识。
3）每1秒每个sentinel对其他sentinel和redis节点执行ping操作（相互监控），这个其实是一个心跳检测，是失败判定的依据

**主观下线**
所谓主观下线（Subjectively Down， 简称 SDOWN）指的是单个Sentinel实例对服务器做出的下线判断，即单个sentinel认为某个服务下线（有可能是接收不到订阅，之间的网络不通等等原因）。
主观下线就是说如果服务器在down-after-milliseconds给定的毫秒数之内， 没有返回 Sentinel 发送的 PING 命令的回复， 或者返回一个错误， 那么 Sentinel 将这个服务器标记为主观下线（SDOWN ）。
sentinel会以每秒一次的频率向所有与其建立了命令连接的实例（master，从服务，其他sentinel）发ping命令，通过判断ping回复是有效回复，还是无效回复来判断实例时候在线（对该sentinel来说是“主观在线”）。
sentinel配置文件中的down-after-milliseconds设置了判断主观下线的时间长度，如果实例在down-after-milliseconds毫秒内，返回的都是无效回复，那么sentinel回认为该实例已（主观）下线，修改其flags状态为SRI_S_DOWN。如果多个sentinel监视一个服务，有可能存在多个sentinel的down-after-milliseconds配置不同，这个在实际生产中要注意。

**客观下线**
客观下线（Objectively Down， 简称 ODOWN）指的是多个 Sentinel 实例在对同一个服务器做出 SDOWN 判断， 并且通过 SENTINEL is-master-down-by-addr 命令互相交流之后， 得出的服务器下线判断，然后开启failover。
客观下线就是说只有在足够数量的 Sentinel 都将一个服务器标记为主观下线之后， 服务器才会被标记为客观下线（ODOWN）。
只有当master被认定为客观下线时，才会发生故障迁移。
当sentinel监视的某个服务主观下线后，sentinel会询问其它监视该服务的sentinel，看它们是否也认为该服务主观下线，接收到足够数量（这个值可以配置）的sentinel判断为主观下线，既任务该服务客观下线，并对其做故障转移操作。
sentinel通过发送 SENTINEL is-master-down-by-addr ip port current_epoch runid，（ip：主观下线的服务id，port：主观下线的服务端口，current_epoch：sentinel的纪元，runid：*表示检测服务下线状态，如果是sentinel 运行id，表示用来选举领头sentinel）来询问其它sentinel是否同意服务下线。
一个sentinel接收另一个sentinel发来的is-master-down-by-addr后，提取参数，根据ip和端口，检测该服务时候在该sentinel主观下线，并且回复is-master-down-by-addr，回复包含三个参数：down_state（1表示已下线，0表示未下线），leader_runid（领头sentinal id），leader_epoch（领头sentinel纪元）。
sentinel接收到回复后，根据配置设置的下线最小数量，达到这个值，既认为该服务客观下线。
客观下线条件只适用于主服务器： 对于任何其他类型的 Redis 实例， Sentinel 在将它们判断为下线前不需要进行协商， 所以从服务器或者其他 Sentinel 永远不会达到客观下线条件。只要一个 Sentinel 发现某个主服务器进入了客观下线状态， 这个 Sentinel 就可能会被其他 Sentinel 推选出， 并对失效的主服务器执行自动故障迁移操作。

**总结来说，故障转移分为三个步骤：**

1）从下线的主服务的所有从服务里面挑选一个从服务，将其转成主服务

2）已下线主服务的所有从服务改为复制新的主服务

3）将已下线的主服务设置成新的主服务的从服务，当其回复正常时，复制新的主服务，变成新的主服务的从服务

**选举规则**

- 健康度：从节点响应时间快；
- 完整性：从节点消费主节点的offset偏移量尽可能的高 ；
- 稳定性：若仍有多个从节点，则根据从节点的创建时间选择最有资历的节点升级为主节点

### 环境搭建

- 参考配置：F:/学习资料/架构进阶/架构班二期/20210429Architect02Course002Redis-2/Redis/bak/哨兵/哨兵.html

master节点

![](F:\MyGithub\Redis\img\34.png)

从节点

![](F:\MyGithub\Redis\img\35.png)

主节点宕机后，从节点被选为主节点

![](F:\MyGithub\Redis\img\36.png)

## 集群

- 某个主节点和所有从节点全部挂掉，我们集群不可用。
- 如果半数以上的主节点挂掉，则集群不可用
- 如果集群任意master挂掉,而且没有从节点，则集群不可用

![](F:\MyGithub\Redis\img\43.png)

### 环境搭建

① 配置文件

~~~shell
# bind 127.0.0.1 //加上注释#
protected-mode no //关闭保护模式
port 6391  //绑定自定义端口
# daemonize yes //禁止redis后台运行
pidfile /var/run/redis_6391.pid
cluster-enabled yes //开启集群 把注释#去掉
cluster-config-file nodes_6391.conf //集群的配置 配置文件首次启动自动生成
appendonly yes //开启aof
cluster-announce-ip 120.25.102.199   //要宣布的IP地址。nat模式要指定宿主机IP
cluster-announce-port 6391  //要宣布的数据端口。
cluster-announce-bus-port 16391  //要宣布的集群总线端口
~~~

② docker-compose.yml配置

~~~shell
version: "3"
services:
  redis-master1:
    image: redis # 基础镜像
    container_name: node1 # 容器名称
    working_dir: /config # 切换工作目录
    environment: # 环境变量
      - PORT=6391 # 会使用config/nodes-${PORT}.conf这个配置文件
    ports: # 映射端口，对外提供服务
      - 6391:6391 # redis的服务端口
      - 16391:16391 # redis集群监控端口
    stdin_open: true # 标准输入打开
    tty: true # 后台运行不退出
    network_mode: host # 使用host模式
    privileged: true # 拥有容器内命令执行的权限
    volumes:
      - /mydata/redis-cluster/config:/config #配置文件目录映射到宿主机
    entrypoint: # 设置服务默认的启动程序
      - /bin/bash
      - redis.sh
  redis-master2:
    image: redis
    working_dir: /config
    container_name: node2
    environment:
      - PORT=6392
    ports:
      - 6392:6392
      - 16392:16392
    stdin_open: true
    network_mode: host
    tty: true
    privileged: true
    volumes:
      - /mydata/redis-cluster/config:/config
    entrypoint:
      - /bin/bash
      - redis.sh
  redis-master3:
    image: redis
    container_name: node3
    working_dir: /config
    environment:
      - PORT=6393
    ports:
      - 6393:6393
      - 16393:16393
    stdin_open: true
    network_mode: host
    tty: true
    privileged: true
    volumes:
      - /mydata/redis-cluster/config:/config
    entrypoint:
      - /bin/bash
      - redis.sh
  redis-slave1:
    image: redis
    container_name: node4
    working_dir: /config
    environment:
      - PORT=6394
    ports:
      - 6394:6394
      - 16394:16394
    stdin_open: true
    network_mode: host
    tty: true
    privileged: true
    volumes:
      - /mydata/redis-cluster/config:/config
    entrypoint:
      - /bin/bash
      - redis.sh
  redis-slave2:
    image: redis
    working_dir: /config
    container_name: node5
    environment:
      - PORT=6395
    ports:
      - 6395:6395
      - 16395:16395
    stdin_open: true
    network_mode: host
    tty: true
    privileged: true
    volumes:
      - /mydata/redis-cluster/config:/config
    entrypoint:
      - /bin/bash
      - redis.sh
  redis-slave3:
    image: redis
    container_name: node6
    working_dir: /config
    environment:
      - PORT=6396
    ports:
      - 6396:6396
      - 16396:16396
    stdin_open: true
    network_mode: host
    tty: true
    privileged: true
    volumes:
      - /mydata/redis-cluster/config:/config
    entrypoint:
      - /bin/bash
      - redis.sh
~~~

③ 创建目录并将配置文件和yml拷贝到该目录下

~~~shell
mkdir /mydata/redis-cluster/config -p
cd /mydata/redis-cluster/config
~~~

![](F:\MyGithub\Redis\img\38.png)

④启动节点

~~~shell
cd /mydata/redis-cluster/config/
#启动节点
docker-compose up -d
​
#创建集群 
#方案一
docker run --rm -it zvelo/redis-trib create --replicas 1 120.25.102.199:6391 120.25.102.199:6392 120.25.102.199:6393 120.25.102.199:6394 120.25.102.199:6395 120.25.102.199:6396

#方案二
​
#随便进入一个redis容器
docker exec -it 容器id /bin/bash
​
redis-cli  --cluster create  120.25.102.199:6391 120.25.102.199:6392 120.25.102.199:6393 120.25.102.199:6394 120.25.102.199:6395 120.25.102.199:6396 --cluster-replicas 1
​
​
#验证测试
redis-cli -h 120.25.102.199 -p 6391 -c
​
#查看集群信息
cluster info
​
#查看节点信息
cluster nodes
~~~

![](F:\MyGithub\Redis\img\39.png)

### 主从扩容

- 参考：https://blog.csdn.net/m0_60615714/article/details/124478411

- 当大量数据请求/高并发，原有的集群无法支撑，需要添加一主一从

  ① 新增两个实例，一主一从，将配置文件和yml文件拷贝到目录下面

  ~~~shell
  version: "3"
  services:
    redis-master4:
      image: redis # 基础镜像
      container_name: node7 # 容器名称
      working_dir: /config # 切换工作目录
      environment: # 环境变量
        - PORT=6397 # 会使用config/nodes-${PORT}.conf这个配置文件
      ports: # 映射端口，对外提供服务
        - 6397:6397 # redis的服务端口
        - 16397:16397 # redis集群监控端口
      stdin_open: true # 标准输入打开
      tty: true # 后台运行不退出
      network_mode: host # 使用host模式
      privileged: true # 拥有容器内命令执行的权限
      volumes:
        - /mydata/redis-cluster/config:/config #配置文件目录映射到宿主机
      entrypoint: # 设置服务默认的启动程序
        - /bin/bash
        - redis.sh
    redis-slave4:
      image: redis
      container_name: node8
      working_dir: /config
      environment:
        - PORT=6398
      ports:
        - 6398:6398
        - 16398:16398
      stdin_open: true
      network_mode: host
      tty: true
      privileged: true
      volumes:
        - /mydata/redis-cluster/config:/config
      entrypoint:
        - /bin/bash
        - redis.sh
  ~~~

  ② 创建容器

  ```shell
  docker-compose up -d
  ```

  ![](F:\MyGithub\Redis\img\42.png)

  ③ 依次按以下命令

~~~shell
[root@localhost ~]# docker run -d --name redis-node7 --net  host --privileged=true -v /data/redis/share/redis-node7:/data redis:6.0.8 --cluster-enabled yes --appendonly yes --port 6387
[root@localhost ~]# docker run -d --name redis-node8 --net  host --privileged=true -v /data/redis/share/redis-node8:/data redis:6.0.8 --cluster-enabled yes --appendonly yes --port 6388
[root@localhost ~]# docker ps 
CONTAINER ID   IMAGE         COMMAND                  CREATED         STATUS         PORTS     NAMES
9457f6038124   redis     "/bin/bash redis.sh"   48 minutes ago   Up 47 minutes             node8
ebc847c658f0   redis     "/bin/bash redis.sh"   48 minutes ago   Up 47 minutes             node7
c12087278df7   redis     "/bin/bash redis.sh"   23 hours ago     Up 23 hours               node5
3cf13409ec2f   redis     "/bin/bash redis.sh"   23 hours ago     Up 23 hours               node1
ca3389a3e6b9   redis     "/bin/bash redis.sh"   23 hours ago     Up 23 hours               node4
4dbead078779   redis     "/bin/bash redis.sh"   23 hours ago     Up 23 hours               node2
d28a4d90349b   redis     "/bin/bash redis.sh"   23 hours ago     Up 23 hours               node6
e2d83b0358c2   redis     "/bin/bash redis.sh"   23 hours ago     Up 23 hours               node3
[root@Developer ~]#
 
#将node7加入集群
[root@localhost ~]# docker exec -it node7 /bin/bash
root@Developer:/data# redis-cli --cluster add-node 120.25.102.199:6397 120.25.102.199:6391
root@Developer:/data# redis-cli --cluster check  120.25.102.199:6397 120.25.102.199:6391

root@Developer:/config#  redis-cli --cluster check  120.25.102.199:6391
120.25.102.199:6391 (91898462...) -> 0 keys | 4795 slots | 1 slaves.
120.25.102.199:6393 (31754245...) -> 0 keys | 4795 slots | 1 slaves.
120.25.102.199:6397 (4e67e0bb...) -> 1 keys | 1999 slots | 0 slaves.#此时还没有从节点
120.25.102.199:6392 (98a84d5a...) -> 0 keys | 4795 slots | 1 slaves.

root@localhost:/data# redis-cli -h 120.25.102.199  -p 6392 -c 
127.0.0.1:6382> cluster nodes
120.25.102.199:6392> cluster nodes
9769d78ec26ea81b938d9addfbd7831d1dfc996b 120.25.102.199:6395@16395 slave 98a84d5ad202e5715d2363a4f40f0a7708b6c69b 0 1657807412000 2 connected
dc88e63bf671ae88e746f073d844f03b6c73b496 120.25.102.199:6394@16394 slave 91898462f1b1343cd53b9067de589b3114784f8e 0 1657807413054 1 connected
4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1 120.25.102.199:6397@16397 master - 0 1657807412051 0 connected
91898462f1b1343cd53b9067de589b3114784f8e 120.25.102.199:6391@16391 master - 0 1657807412000 1 connected 0-5460
0f6a1d5e94ad67a06c0c31f841a0e777df69f0e1 120.25.102.199:6396@16396 slave 31754245aabdfbc111ed7ab70c0b939454850c96 0 1657807408038 3 connected
98a84d5ad202e5715d2363a4f40f0a7708b6c69b 120.25.102.199:6392@16392 myself,master - 0 1657807411000 2 connected 5461-10922
31754245aabdfbc111ed7ab70c0b939454850c96 120.25.102.199:6393@16393 master - 0 1657807411047 3 connected 10923-16383
 
#重新分配槽号
root@Developer:/config# redis-cli --cluster reshard 120.25.102.199:6391
>>> Performing Cluster Check (using node 120.25.102.199:6391)
M: 91898462f1b1343cd53b9067de589b3114784f8e 120.25.102.199:6391
   slots:[0-5460] (5461 slots) master
   1 additional replica(s)
M: 31754245aabdfbc111ed7ab70c0b939454850c96 120.25.102.199:6393
   slots:[10923-16383] (5461 slots) master
   1 additional replica(s)
S: dc88e63bf671ae88e746f073d844f03b6c73b496 120.25.102.199:6394
   slots: (0 slots) slave
   replicates 91898462f1b1343cd53b9067de589b3114784f8e
M: 4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1 120.25.102.199:6397
   slots: (0 slots) master
S: 0f6a1d5e94ad67a06c0c31f841a0e777df69f0e1 120.25.102.199:6396
   slots: (0 slots) slave
   replicates 31754245aabdfbc111ed7ab70c0b939454850c96
M: 98a84d5ad202e5715d2363a4f40f0a7708b6c69b 120.25.102.199:6392
   slots:[5461-10922] (5462 slots) master
   1 additional replica(s)
S: 9769d78ec26ea81b938d9addfbd7831d1dfc996b 120.25.102.199:6395
   slots: (0 slots) slave
   replicates 98a84d5ad202e5715d2363a4f40f0a7708b6c69b
[OK] All nodes agree about slots configuration.
>>> Check for open slots...
>>> Check slots coverage...
[OK] All 16384 slots covered.
How many slots do you want to move (from 1 to 16384)? 2000 #1.分配卡槽数量
What is the receiving node ID? 4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1 #2.选择接受分配的卡槽，这里是6397对应的id
Please enter all the source node IDs.
  Type 'all' to use all the nodes as source nodes for the hash slots.
  Type 'done' once you entered all the source nodes IDs.
Source node #1: all #3.选择all

 
#发现redis-node7的槽号是原来的三个master分一点给node7
root@Developer:/config#  redis-cli --cluster check  120.25.102.199:6391
120.25.102.199:6391 (91898462...) -> 0 keys | 4795 slots | 1 slaves.
120.25.102.199:6393 (31754245...) -> 0 keys | 4795 slots | 1 slaves.
120.25.102.199:6397 (4e67e0bb...) -> 1 keys | 1999 slots | 0 slaves. #已分配2000卡槽
120.25.102.199:6392 (98a84d5a...) -> 0 keys | 4795 slots | 1 slaves.
 
#为node7分配slave
root@Developer:/config# redis-cli -h 120.25.102.199  -p 6392 -c
120.25.102.199:6392> cluster nodes
9769d78ec26ea81b938d9addfbd7831d1dfc996b 120.25.102.199:6395@16395 slave 98a84d5ad202e5715d2363a4f40f0a7708b6c69b 0 1657808419000 2 connected
dc88e63bf671ae88e746f073d844f03b6c73b496 120.25.102.199:6394@16394 slave 91898462f1b1343cd53b9067de589b3114784f8e 0 1657808417000 1 connected
4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1 120.25.102.199:6397@16397 master - 0 1657808419869 7 connected 0-665 5461-6127 10923-11588
91898462f1b1343cd53b9067de589b3114784f8e 120.25.102.199:6391@16391 master - 0 1657808418000 1 connected 666-5460
0f6a1d5e94ad67a06c0c31f841a0e777df69f0e1 120.25.102.199:6396@16396 slave 31754245aabdfbc111ed7ab70c0b939454850c96 0 1657808421877 3 connected
98a84d5ad202e5715d2363a4f40f0a7708b6c69b 120.25.102.199:6392@16392 myself,master - 0 1657808420000 2 connected 6128-10922
31754245aabdfbc111ed7ab70c0b939454850c96 120.25.102.199:6393@16393 master - 0 1657808420873 3 connected 11589-16383
#分配子节点给node7
120.25.102.199:6392>
root@Developer:/config# redis-cli --cluster add-node 120.25.102.199:6398 120.25.102.199:6397 --cluster-slave --cluster-master-id 4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1

 
#查看集群状态
root@Developer:/config# redis-cli --cluster check  120.25.102.199:6391
120.25.102.199:6391 (91898462...) -> 0 keys | 4795 slots | 1 slaves.
120.25.102.199:6393 (31754245...) -> 0 keys | 4795 slots | 1 slaves.
120.25.102.199:6397 (4e67e0bb...) -> 1 keys | 1999 slots | 1 slaves.#已分配子节点
120.25.102.199:6392 (98a84d5a...) -> 0 keys | 4795 slots | 1 slaves.
[OK] 1 keys in 4 masters.
0.00 keys per slot on average.
>>> Performing Cluster Check (using node 120.25.102.199:6391)
M: 91898462f1b1343cd53b9067de589b3114784f8e 120.25.102.199:6391
   slots:[666-5460] (4795 slots) master
   1 additional replica(s)
M: 31754245aabdfbc111ed7ab70c0b939454850c96 120.25.102.199:6393
   slots:[11589-16383] (4795 slots) master
   1 additional replica(s)
S: dc88e63bf671ae88e746f073d844f03b6c73b496 120.25.102.199:6394
   slots: (0 slots) slave
   replicates 91898462f1b1343cd53b9067de589b3114784f8e
M: 4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1 120.25.102.199:6397
   slots:[0-665],[5461-6127],[10923-11588] (1999 slots) master
   1 additional replica(s)
S: 0f6a1d5e94ad67a06c0c31f841a0e777df69f0e1 120.25.102.199:6396
   slots: (0 slots) slave
   replicates 31754245aabdfbc111ed7ab70c0b939454850c96
M: 98a84d5ad202e5715d2363a4f40f0a7708b6c69b 120.25.102.199:6392
   slots:[6128-10922] (4795 slots) master
   1 additional replica(s)
S: ef51acbbc355684d1f86213a992625f8ec79222d 120.25.102.199:6398
   slots: (0 slots) slave
   replicates 4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1
S: 9769d78ec26ea81b938d9addfbd7831d1dfc996b 120.25.102.199:6395
   slots: (0 slots) slave
   replicates 98a84d5ad202e5715d2363a4f40f0a7708b6c69b

~~~

### 主从缩容

~~~shell
#获取6398的docker-id并删除
[root@Developer ~]# docker exec -it node7 bash
root@Developer:/config# redis-cli --cluster check 120.25.102.199:6391
120.25.102.199:6391 (91898462...) -> 0 keys | 4795 slots | 1 slaves.
120.25.102.199:6393 (31754245...) -> 0 keys | 4795 slots | 1 slaves.
120.25.102.199:6397 (4e67e0bb...) -> 1 keys | 1999 slots | 1 slaves.
120.25.102.199:6392 (98a84d5a...) -> 0 keys | 4795 slots | 1 slaves.
[OK] 1 keys in 4 masters.
0.00 keys per slot on average.
>>> Performing Cluster Check (using node 120.25.102.199:6391)
M: 91898462f1b1343cd53b9067de589b3114784f8e 120.25.102.199:6391
   slots:[666-5460] (4795 slots) master
   1 additional replica(s)
M: 31754245aabdfbc111ed7ab70c0b939454850c96 120.25.102.199:6393
   slots:[11589-16383] (4795 slots) master
   1 additional replica(s)
S: dc88e63bf671ae88e746f073d844f03b6c73b496 120.25.102.199:6394
   slots: (0 slots) slave
   replicates 91898462f1b1343cd53b9067de589b3114784f8e
M: 4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1 120.25.102.199:6397
   slots:[0-665],[5461-6127],[10923-11588] (1999 slots) master
   1 additional replica(s)
S: 0f6a1d5e94ad67a06c0c31f841a0e777df69f0e1 120.25.102.199:6396
   slots: (0 slots) slave
   replicates 31754245aabdfbc111ed7ab70c0b939454850c96
M: 98a84d5ad202e5715d2363a4f40f0a7708b6c69b 120.25.102.199:6392
   slots:[6128-10922] (4795 slots) master
   1 additional replica(s)
S: ef51acbbc355684d1f86213a992625f8ec79222d 120.25.102.199:6398
   slots: (0 slots) slave
   replicates 4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1
S: 9769d78ec26ea81b938d9addfbd7831d1dfc996b 120.25.102.199:6395
   slots: (0 slots) slave
   replicates 98a84d5ad202e5715d2363a4f40f0a7708b6c69b
[OK] All nodes agree about slots configuration.
>>> Check for open slots...
>>> Check slots coverage...
[OK] All 16384 slots covered.
root@Developer:/config# redis-cli --cluster del-node 120.25.102.199:6398 ef51acbbc355684d1f86213a992625f8ec79222d

>>> Removing node ef51acbbc355684d1f86213a992625f8ec79222d from cluster 120.25.102.199:6398
>>> Sending CLUSTER FORGET messages to the cluster...
>>> Sending CLUSTER RESET SOFT to the deleted node.
 
#重新分配槽位，将剩余的曹位全部分配给其中一个node
root@Developer:/config# redis-cli --cluster reshard 120.25.102.199:6391
>>> Performing Cluster Check (using node 120.25.102.199:6391)
M: 91898462f1b1343cd53b9067de589b3114784f8e 120.25.102.199:6391
   slots:[666-5460] (4795 slots) master
   1 additional replica(s)
M: 31754245aabdfbc111ed7ab70c0b939454850c96 120.25.102.199:6393
   slots:[11589-16383] (4795 slots) master
   1 additional replica(s)
S: dc88e63bf671ae88e746f073d844f03b6c73b496 120.25.102.199:6394
   slots: (0 slots) slave
   replicates 91898462f1b1343cd53b9067de589b3114784f8e
M: 4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1 120.25.102.199:6397
   slots:[0-665],[5461-6127],[10923-11588] (1999 slots) master
S: 0f6a1d5e94ad67a06c0c31f841a0e777df69f0e1 120.25.102.199:6396
   slots: (0 slots) slave
   replicates 31754245aabdfbc111ed7ab70c0b939454850c96
M: 98a84d5ad202e5715d2363a4f40f0a7708b6c69b 120.25.102.199:6392
   slots:[6128-10922] (4795 slots) master
   1 additional replica(s)
S: 9769d78ec26ea81b938d9addfbd7831d1dfc996b 120.25.102.199:6395
   slots: (0 slots) slave
   replicates 98a84d5ad202e5715d2363a4f40f0a7708b6c69b
[OK] All nodes agree about slots configuration.
>>> Check for open slots...
>>> Check slots coverage...
[OK] All 16384 slots covered.
How many slots do you want to move (from 1 to 16384)? 2000
What is the receiving node ID? 91898462f1b1343cd53b9067de589b3114784f8e
Please enter all the source node IDs.
  Type 'all' to use all the nodes as source nodes for the hash slots.
  Type 'done' once you entered all the source nodes IDs.
Source node #1: all

 
 
#删除node7
root@localhost:/data# redis-cli --cluster del-node 120.25.102.199:6397 4e67e0bb1d5294823428033bf2f6d9ad5d5f2af1
 
#查询集群状态
[root@Developer ~]# docker exec -it node1 bash
root@Developer:/config# redis-cli --cluster check 120.25.102.199:6391
120.25.102.199:6391 (91898462...) -> 0 keys | 16384 slots | 5 slaves.
[OK] 0 keys in 1 masters.
0.00 keys per slot on average.
>>> Performing Cluster Check (using node 120.25.102.199:6391)
M: 91898462f1b1343cd53b9067de589b3114784f8e 120.25.102.199:6391
   slots:[0-16383] (16384 slots) master
   5 additional replica(s)
S: 31754245aabdfbc111ed7ab70c0b939454850c96 120.25.102.199:6393
   slots: (0 slots) slave
   replicates 91898462f1b1343cd53b9067de589b3114784f8e
S: dc88e63bf671ae88e746f073d844f03b6c73b496 120.25.102.199:6394
   slots: (0 slots) slave
   replicates 91898462f1b1343cd53b9067de589b3114784f8e
S: 0f6a1d5e94ad67a06c0c31f841a0e777df69f0e1 120.25.102.199:6396
   slots: (0 slots) slave
   replicates 91898462f1b1343cd53b9067de589b3114784f8e
S: 98a84d5ad202e5715d2363a4f40f0a7708b6c69b 120.25.102.199:6392
   slots: (0 slots) slave
   replicates 91898462f1b1343cd53b9067de589b3114784f8e
S: 9769d78ec26ea81b938d9addfbd7831d1dfc996b 120.25.102.199:6395
   slots: (0 slots) slave
   replicates 91898462f1b1343cd53b9067de589b3114784f8e
[OK] All nodes agree about slots configuration.
>>> Check for open slots...
>>> Check slots coverage...

 
~~~

# 实战调优

~~~c#
使用maxmemory配置是为了将Redis配置为对数据集使
用指定的内存量
//直接修改redis.conf文件
maxmemory 100mb
//也可以这样，直接连接redis执行
config set maxmemory 100mb
//可以通过指令查看当前配置的最大结果
config get maxmemory
设置maxmemory为零将导致没有内存限制
~~~

- 当内存使用值超过了maxmemory配置时，redis可以使用以下策略进行数据淘汰。
-  noeviction：（默认策略）当达到内存限制并且客户端尝试执行可能导致使用更多内存的命令时返回错。
- volatile-lru：删除设置了过期时间的key而且最近最少使用key（LRU算法淘汰）。
- allkeys-lru：删除所有比较最近最少使用的key（LRU算法淘汰）。
- volatile-lfu：删除设置了过期时间的key而且使用频率最低key（LFU算法淘汰）。
- allkeys-lfu：删除所有使用频率最低的key（LFU算法淘汰）
- volatile-random：设置了过期时间的key使用随机淘汰。
- allkeys-random：所有key使用随机淘汰。
- volatile-ttl：设置了过期时间的key根据过期时间淘汰，越早过期越早淘汰。

# 问题总结

哨兵或者主从里面的数据问题：

## 1.脑裂问题

出现了主节点和哨兵之间网络原因，而且有多数以上的哨兵认为主节点宕机，则再从会从节点现在一个主，这个时候客户端代码还是可以连接到之前的主节点的，可以写数据，此时哨兵选举了新的主节点，然后之前的主网络恢复了，然后之前的主节点备份现在的主节点数据，造成数据不完整。。。

## 2.异步复制数据丢失问题

因为是异步复制数据，如果主节点和从节点直接数据复制太慢，在这之间主节点宕机，而且是真的宕机，这个时候从节点替换主节点，丢失了数据。。

哨兵--不管怎么样的配置都没有办法保证数据百分之白不丢失，只能尽可能少量丢数据

怎么解决上面两个问题呢？

需要改配置文件：

1.至少有几个从节点。配置=0，代表的是，当主节点和从节点之间互通的时候，发现从节点小于一个的时候，则从节点不会再继续给客户端提供服务。。 解决脑裂问题。。 

2.偏移量配置。。主节点和从节点数据之前偏移量只差，如果偏移量之差比配置小，则主节点也不会提供服务。

![](F:\MyGithub\Redis\img\33.png)

第一个配置是主节点最少能连接到几个从节点

第二配置是主节点和从节点的ack时间差-响应（数据复制完整性问题，偏移量）

## 3.缓存穿透

缓存穿透，就是用户想要查询一个数据，在 redis 中查询不到，即没有在缓存中命中，那么就会直接去持久化的 mysql 中进行查询，发现也没有这个数据，那么本次查询就失败了。当用户巨多的时候，查询缓存都没有查询到，那么这些全部都去查询持久化的 mysql 数据库，压力全部打到 mysql 上面，这就是缓存穿透。

解决方案有一般有 2 种方式

1. 使用布隆过滤器
2. 缓存空的对象

